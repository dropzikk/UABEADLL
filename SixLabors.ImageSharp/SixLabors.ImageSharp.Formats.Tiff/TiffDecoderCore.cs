using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff;

internal class TiffDecoderCore : IImageDecoderInternals
{
	private readonly Configuration configuration;

	private readonly MemoryAllocator memoryAllocator;

	private readonly bool skipMetadata;

	private readonly uint maxFrames;

	private BufferedReadStream inputStream;

	private ByteOrder byteOrder;

	private bool isBigTiff;

	public TiffBitsPerSample BitsPerSample { get; set; }

	public int BitsPerPixel { get; set; }

	public ushort[] ColorMap { get; set; }

	public TiffColorType ColorType { get; set; }

	public Rational[] ReferenceBlackAndWhite { get; set; }

	public Rational[] YcbcrCoefficients { get; set; }

	public ushort[] YcbcrSubSampling { get; set; }

	public TiffDecoderCompressionType CompressionType { get; set; }

	public FaxCompressionOptions FaxCompressionOptions { get; set; }

	public TiffFillOrder FillOrder { get; set; }

	public TiffExtraSampleType? ExtraSamplesType { get; set; }

	public byte[] JpegTables { get; set; }

	public uint? OldJpegCompressionStartOfImageMarker { get; set; }

	public TiffPlanarConfiguration PlanarConfiguration { get; set; }

	public TiffPhotometricInterpretation PhotometricInterpretation { get; set; }

	public TiffSampleFormat SampleFormat { get; set; }

	public TiffPredictor Predictor { get; set; }

	public DecoderOptions Options { get; }

	public Size Dimensions { get; private set; }

	public TiffDecoderCore(DecoderOptions options)
	{
		Options = options;
		configuration = options.Configuration;
		skipMetadata = options.SkipMetadata;
		maxFrames = options.MaxFrames;
		memoryAllocator = configuration.MemoryAllocator;
	}

	public Image<TPixel> Decode<TPixel>(BufferedReadStream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		List<ImageFrame<TPixel>> list = new List<ImageFrame<TPixel>>();
		List<ImageFrameMetadata> list2 = new List<ImageFrameMetadata>();
		try
		{
			inputStream = stream;
			DirectoryReader directoryReader = new DirectoryReader(stream, configuration.MemoryAllocator);
			IList<ExifProfile> list3 = directoryReader.Read();
			byteOrder = directoryReader.ByteOrder;
			isBigTiff = directoryReader.IsBigTiff;
			uint num = 0u;
			foreach (ExifProfile item in list3)
			{
				cancellationToken.ThrowIfCancellationRequested();
				ImageFrame<TPixel> imageFrame = DecodeFrame<TPixel>(item, cancellationToken);
				list.Add(imageFrame);
				list2.Add(imageFrame.Metadata);
				if (++num == maxFrames)
				{
					break;
				}
			}
			ImageMetadata metadata = TiffDecoderMetadataCreator.Create(list2, skipMetadata, directoryReader.ByteOrder, directoryReader.IsBigTiff);
			ImageFrame<TPixel> imageFrame2 = list[0];
			Dimensions = imageFrame2.Size();
			foreach (ImageFrame<TPixel> item2 in list)
			{
				if (item2.Size() != imageFrame2.Size())
				{
					TiffThrowHelper.ThrowNotSupported("Images with different sizes are not supported");
				}
			}
			return new Image<TPixel>(configuration, metadata, list);
		}
		catch
		{
			foreach (ImageFrame<TPixel> item3 in list)
			{
				item3.Dispose();
			}
			throw;
		}
	}

	public ImageInfo Identify(BufferedReadStream stream, CancellationToken cancellationToken)
	{
		inputStream = stream;
		DirectoryReader directoryReader = new DirectoryReader(stream, configuration.MemoryAllocator);
		IList<ExifProfile> list = directoryReader.Read();
		List<ImageFrameMetadata> list2 = new List<ImageFrameMetadata>();
		foreach (ExifProfile item in list)
		{
			list2.Add(CreateFrameMetadata(item));
		}
		ExifProfile exifProfile = list[0];
		ImageMetadata metadata = TiffDecoderMetadataCreator.Create(list2, skipMetadata, directoryReader.ByteOrder, directoryReader.IsBigTiff);
		int imageWidth = GetImageWidth(exifProfile);
		int imageHeight = GetImageHeight(exifProfile);
		return new ImageInfo(new PixelTypeInfo((int)list2[0].GetTiffMetadata().BitsPerPixel.Value), new Size(imageWidth, imageHeight), metadata, list2);
	}

	private ImageFrame<TPixel> DecodeFrame<TPixel>(ExifProfile tags, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		ImageFrameMetadata metadata = CreateFrameMetadata(tags);
		bool num = this.VerifyAndParse(tags, metadata.GetTiffMetadata());
		ImageFrame<TPixel> imageFrame = new ImageFrame<TPixel>(width: GetImageWidth(tags), height: GetImageHeight(tags), configuration: configuration, metadata: metadata);
		if (num)
		{
			DecodeImageWithTiles(tags, imageFrame, cancellationToken);
		}
		else
		{
			DecodeImageWithStrips(tags, imageFrame, cancellationToken);
		}
		return imageFrame;
	}

	private ImageFrameMetadata CreateFrameMetadata(ExifProfile tags)
	{
		ImageFrameMetadata imageFrameMetadata = new ImageFrameMetadata();
		if (!skipMetadata)
		{
			imageFrameMetadata.ExifProfile = tags;
		}
		TiffFrameMetadata.Parse(imageFrameMetadata.GetTiffMetadata(), tags);
		return imageFrameMetadata;
	}

	private void DecodeImageWithStrips<TPixel>(ExifProfile tags, ImageFrame<TPixel> frame, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		IExifValue<Number> exifValue;
		int rowsPerStrip = ((!tags.TryGetValue(ExifTag.RowsPerStrip, out exifValue)) ? int.MaxValue : ((int)exifValue.Value));
		Array array = (Array)tags.GetValueInternal(ExifTag.StripOffsets).GetValue();
		Array array2 = (Array)tags.GetValueInternal(ExifTag.StripByteCounts).GetValue();
		Span<ulong> span;
		using (ConvertNumbers(array, out span))
		{
			Span<ulong> span2;
			using (ConvertNumbers(array2, out span2))
			{
				if (PlanarConfiguration == TiffPlanarConfiguration.Planar)
				{
					DecodeStripsPlanar(frame, rowsPerStrip, span, span2, cancellationToken);
				}
				else
				{
					DecodeStripsChunky(frame, rowsPerStrip, span, span2, cancellationToken);
				}
			}
		}
	}

	private void DecodeImageWithTiles<TPixel>(ExifProfile tags, ImageFrame<TPixel> frame, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Buffer2D<TPixel> pixelBuffer = frame.PixelBuffer;
		int width = pixelBuffer.Width;
		int height = pixelBuffer.Height;
		if (!tags.TryGetValue(ExifTag.TileWidth, out IExifValue<Number> exifValue))
		{
			ArgumentNullException.ThrowIfNull(exifValue, "valueWidth");
		}
		if (!tags.TryGetValue(ExifTag.TileLength, out IExifValue<Number> exifValue2))
		{
			ArgumentNullException.ThrowIfNull(exifValue2, "valueLength");
		}
		int num = (int)exifValue.Value;
		int num2 = (int)exifValue2.Value;
		int tilesAcross = (width + num - 1) / num;
		int tilesDown = (height + num2 - 1) / num2;
		IExifValue valueInternal = tags.GetValueInternal(ExifTag.TileOffsets);
		IExifValue valueInternal2 = tags.GetValueInternal(ExifTag.TileByteCounts);
		Array array;
		Array array2;
		if (valueInternal == null)
		{
			valueInternal = tags.GetValueInternal(ExifTag.StripOffsets);
			valueInternal2 = tags.GetValueInternal(ExifTag.StripByteCounts);
			array = (Array)valueInternal.GetValue();
			array2 = (Array)valueInternal2.GetValue();
		}
		else
		{
			array = (Array)valueInternal.GetValue();
			array2 = (Array)valueInternal2.GetValue();
		}
		Span<ulong> span;
		using (ConvertNumbers(array, out span))
		{
			Span<ulong> span2;
			using (ConvertNumbers(array2, out span2))
			{
				if (PlanarConfiguration == TiffPlanarConfiguration.Planar)
				{
					DecodeTilesPlanar(frame, num, num2, tilesAcross, tilesDown, span, span2, cancellationToken);
				}
				else
				{
					DecodeTilesChunky(frame, num, num2, tilesAcross, tilesDown, span, span2, cancellationToken);
				}
			}
		}
	}

	private void DecodeStripsPlanar<TPixel>(ImageFrame<TPixel> frame, int rowsPerStrip, Span<ulong> stripOffsets, Span<ulong> stripByteCounts, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		int channels = BitsPerSample.Channels;
		int num = stripOffsets.Length / channels;
		int bitsPerPixel = BitsPerPixel;
		Buffer2D<TPixel> pixelBuffer = frame.PixelBuffer;
		IMemoryOwner<byte>[] array = new IMemoryOwner<byte>[channels];
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				int length = CalculateStripBufferSize(frame.Width, rowsPerStrip, i);
				array[i] = memoryAllocator.Allocate<byte>(length);
			}
			using TiffBaseDecompressor tiffBaseDecompressor = CreateDecompressor<TPixel>(frame.Width, bitsPerPixel);
			TiffBasePlanarColorDecoder<TPixel> tiffBasePlanarColorDecoder = CreatePlanarColorDecoder<TPixel>();
			for (int j = 0; j < num; j++)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int num2 = ((j < num - 1 || frame.Height % rowsPerStrip == 0) ? rowsPerStrip : (frame.Height % rowsPerStrip));
				int num3 = j;
				for (int k = 0; k < channels; k++)
				{
					tiffBaseDecompressor.Decompress(inputStream, stripOffsets[num3], stripByteCounts[num3], num2, array[k].GetSpan(), cancellationToken);
					num3 += num;
				}
				tiffBasePlanarColorDecoder.Decode(array, pixelBuffer, 0, rowsPerStrip * j, frame.Width, num2);
			}
		}
		finally
		{
			IMemoryOwner<byte>[] array2 = array;
			for (int l = 0; l < array2.Length; l++)
			{
				array2[l]?.Dispose();
			}
		}
	}

	private void DecodeStripsChunky<TPixel>(ImageFrame<TPixel> frame, int rowsPerStrip, Span<ulong> stripOffsets, Span<ulong> stripByteCounts, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (rowsPerStrip == int.MaxValue)
		{
			rowsPerStrip = frame.Height;
		}
		int length = CalculateStripBufferSize(frame.Width, rowsPerStrip);
		int bitsPerPixel = BitsPerPixel;
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
		Span<byte> span = buffer.GetSpan();
		Buffer2D<TPixel> pixelBuffer = frame.PixelBuffer;
		using TiffBaseDecompressor tiffBaseDecompressor = CreateDecompressor<TPixel>(frame.Width, bitsPerPixel);
		TiffBaseColorDecoder<TPixel> tiffBaseColorDecoder = CreateChunkyColorDecoder<TPixel>();
		for (int i = 0; i < stripOffsets.Length; i++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int num = ((i < stripOffsets.Length - 1 || frame.Height % rowsPerStrip == 0) ? rowsPerStrip : (frame.Height % rowsPerStrip));
			int num2 = rowsPerStrip * i;
			if (num2 + num > frame.Height)
			{
				break;
			}
			tiffBaseDecompressor.Decompress(inputStream, stripOffsets[i], stripByteCounts[i], num, span, cancellationToken);
			tiffBaseColorDecoder.Decode(span, pixelBuffer, 0, num2, frame.Width, num);
		}
	}

	private void DecodeTilesPlanar<TPixel>(ImageFrame<TPixel> frame, int tileWidth, int tileLength, int tilesAcross, int tilesDown, Span<ulong> tileOffsets, Span<ulong> tileByteCounts, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Buffer2D<TPixel> pixelBuffer = frame.PixelBuffer;
		int width = pixelBuffer.Width;
		int height = pixelBuffer.Height;
		int bitsPerPixel = BitsPerPixel;
		int channels = BitsPerSample.Channels;
		int num = tileOffsets.Length / channels;
		IMemoryOwner<byte>[] array = new IMemoryOwner<byte>[channels];
		try
		{
			int length = RoundUpToMultipleOfEight(tileWidth * bitsPerPixel) * tileLength;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
			}
			using TiffBaseDecompressor tiffBaseDecompressor = CreateDecompressor<TPixel>(frame.Width, bitsPerPixel);
			TiffBasePlanarColorDecoder<TPixel> tiffBasePlanarColorDecoder = CreatePlanarColorDecoder<TPixel>();
			int num2 = 0;
			int num3 = height;
			for (int j = 0; j < tilesDown; j++)
			{
				int num4 = width;
				int top = j * tileLength;
				bool flag = j == tilesDown - 1;
				for (int k = 0; k < tilesAcross; k++)
				{
					int left = k * tileWidth;
					bool flag2 = k == tilesAcross - 1;
					int num5 = num2;
					for (int l = 0; l < channels; l++)
					{
						cancellationToken.ThrowIfCancellationRequested();
						tiffBaseDecompressor.Decompress(inputStream, tileOffsets[num5], tileByteCounts[num5], tileLength, array[l].GetSpan(), cancellationToken);
						num5 += num;
					}
					if (flag2 && num4 < tileWidth)
					{
						for (int m = 0; m < channels; m++)
						{
							Span<byte> span = array[m].GetSpan();
							for (int n = 0; n < tileLength; n++)
							{
								int start = n * tileWidth;
								Span<byte> destination = span.Slice(n * num4, num4);
								span.Slice(start, num4).CopyTo(destination);
							}
						}
					}
					tiffBasePlanarColorDecoder.Decode(array, pixelBuffer, left, top, flag2 ? num4 : tileWidth, flag ? num3 : tileLength);
					num4 -= tileWidth;
					num2++;
				}
				num3 -= tileLength;
			}
		}
		finally
		{
			IMemoryOwner<byte>[] array2 = array;
			for (int num6 = 0; num6 < array2.Length; num6++)
			{
				array2[num6]?.Dispose();
			}
		}
	}

	private void DecodeTilesChunky<TPixel>(ImageFrame<TPixel> frame, int tileWidth, int tileLength, int tilesAcross, int tilesDown, Span<ulong> tileOffsets, Span<ulong> tileByteCounts, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Buffer2D<TPixel> pixelBuffer = frame.PixelBuffer;
		int width = pixelBuffer.Width;
		int height = pixelBuffer.Height;
		int bitsPerPixel = BitsPerPixel;
		int num = RoundUpToMultipleOfEight(width * bitsPerPixel);
		int num2 = RoundUpToMultipleOfEight(tileWidth * bitsPerPixel);
		int length = num2 * tileLength;
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
		using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(tilesDown * tileLength * num, AllocationOptions.Clean);
		Span<byte> span = buffer.GetSpan();
		Span<byte> span2 = buffer2.GetSpan();
		using TiffBaseDecompressor tiffBaseDecompressor = CreateDecompressor<TPixel>(frame.Width, bitsPerPixel);
		TiffBaseColorDecoder<TPixel> tiffBaseColorDecoder = CreateChunkyColorDecoder<TPixel>();
		int num3 = 0;
		for (int i = 0; i < tilesDown; i++)
		{
			int num4 = width;
			for (int j = 0; j < tilesAcross; j++)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int num5 = i * tileLength * num;
				bool num6 = j == tilesAcross - 1;
				tiffBaseDecompressor.Decompress(inputStream, tileOffsets[num3], tileByteCounts[num3], tileLength, span, cancellationToken);
				int num7 = 0;
				num5 += num2 * j;
				int length2 = (num6 ? RoundUpToMultipleOfEight(bitsPerPixel * num4) : num2);
				for (int k = 0; k < tileLength; k++)
				{
					Span<byte> destination = span2.Slice(num5, length2);
					span.Slice(num7, length2).CopyTo(destination);
					num7 += num2;
					num5 += num;
				}
				num4 -= tileWidth;
				num3++;
			}
		}
		tiffBaseColorDecoder.Decode(span2, pixelBuffer, 0, 0, width, height);
	}

	private TiffBaseColorDecoder<TPixel> CreateChunkyColorDecoder<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
	{
		return TiffColorDecoderFactory<TPixel>.Create(configuration, memoryAllocator, ColorType, BitsPerSample, ExtraSamplesType, ColorMap, ReferenceBlackAndWhite, YcbcrCoefficients, YcbcrSubSampling, byteOrder);
	}

	private TiffBasePlanarColorDecoder<TPixel> CreatePlanarColorDecoder<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
	{
		return TiffColorDecoderFactory<TPixel>.CreatePlanar(ColorType, BitsPerSample, ExtraSamplesType, ColorMap, ReferenceBlackAndWhite, YcbcrCoefficients, YcbcrSubSampling, byteOrder);
	}

	private TiffBaseDecompressor CreateDecompressor<TPixel>(int frameWidth, int bitsPerPixel) where TPixel : unmanaged, IPixel<TPixel>
	{
		return TiffDecompressorsFactory.Create(Options, CompressionType, memoryAllocator, PhotometricInterpretation, frameWidth, bitsPerPixel, ColorType, Predictor, FaxCompressionOptions, JpegTables, OldJpegCompressionStartOfImageMarker.GetValueOrDefault(), FillOrder, byteOrder);
	}

	private IMemoryOwner<ulong> ConvertNumbers(Array array, out Span<ulong> span)
	{
		if (array is Number[] array2)
		{
			IMemoryOwner<ulong> memoryOwner = memoryAllocator.Allocate<ulong>(array2.Length);
			span = memoryOwner.GetSpan();
			for (int i = 0; i < array2.Length; i++)
			{
				span[i] = (uint)array2[i];
			}
			return memoryOwner;
		}
		span = (ulong[])array;
		return null;
	}

	private int CalculateStripBufferSize(int width, int height, int plane = -1)
	{
		int num = 0;
		if (PlanarConfiguration == TiffPlanarConfiguration.Chunky)
		{
			num = BitsPerPixel;
		}
		else
		{
			switch (plane)
			{
			case 0:
				num = BitsPerSample.Channel0;
				break;
			case 1:
				num = BitsPerSample.Channel1;
				break;
			case 2:
				num = BitsPerSample.Channel2;
				break;
			case 3:
				num = BitsPerSample.Channel3;
				break;
			default:
				TiffThrowHelper.ThrowNotSupported("More then 4 color channels are not supported");
				break;
			}
		}
		return (width * num + 7) / 8 * height;
	}

	private static int GetImageWidth(ExifProfile exifProfile)
	{
		if (!exifProfile.TryGetValue(ExifTag.ImageWidth, out IExifValue<Number> exifValue))
		{
			TiffThrowHelper.ThrowInvalidImageContentException("The TIFF image frame is missing the ImageWidth");
		}
		return (int)exifValue.Value;
	}

	private static int GetImageHeight(ExifProfile exifProfile)
	{
		if (!exifProfile.TryGetValue(ExifTag.ImageLength, out IExifValue<Number> exifValue))
		{
			TiffThrowHelper.ThrowImageFormatException("The TIFF image frame is missing the ImageLength");
		}
		return (int)exifValue.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int RoundUpToMultipleOfEight(int value)
	{
		return (int)((uint)(value + 7) / 8u);
	}
}
