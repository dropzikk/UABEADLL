using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Tiff.Writers;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Formats.Tiff;

internal sealed class TiffEncoderCore : IImageEncoderInternals
{
	private static readonly ushort ByteOrderMarker = (ushort)(BitConverter.IsLittleEndian ? 18761 : 19789);

	private readonly MemoryAllocator memoryAllocator;

	private Configuration configuration;

	private readonly IQuantizer quantizer;

	private readonly IPixelSamplingStrategy pixelSamplingStrategy;

	private readonly DeflateCompressionLevel compressionLevel;

	private const TiffPredictor DefaultPredictor = TiffPredictor.None;

	private const TiffBitsPerPixel DefaultBitsPerPixel = TiffBitsPerPixel.Bit24;

	private const TiffCompression DefaultCompression = TiffCompression.None;

	private const TiffPhotometricInterpretation DefaultPhotometricInterpretation = TiffPhotometricInterpretation.Rgb;

	private readonly bool skipMetadata;

	private readonly List<(long, uint)> frameMarkers = new List<(long, uint)>();

	internal TiffPhotometricInterpretation? PhotometricInterpretation { get; private set; }

	internal TiffCompression? CompressionType { get; set; }

	internal TiffPredictor? HorizontalPredictor { get; set; }

	internal TiffBitsPerPixel? BitsPerPixel { get; private set; }

	public TiffEncoderCore(TiffEncoder options, MemoryAllocator memoryAllocator)
	{
		this.memoryAllocator = memoryAllocator;
		PhotometricInterpretation = options.PhotometricInterpretation;
		quantizer = options.Quantizer ?? KnownQuantizers.Octree;
		pixelSamplingStrategy = options.PixelSamplingStrategy;
		BitsPerPixel = options.BitsPerPixel;
		HorizontalPredictor = options.HorizontalPredictor;
		CompressionType = options.Compression;
		compressionLevel = options.CompressionLevel ?? DeflateCompressionLevel.Level6;
		skipMetadata = options.SkipMetadata;
	}

	public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(image, "image");
		Guard.NotNull(stream, "stream");
		configuration = image.Configuration;
		TiffFrameMetadata tiffMetadata = image.Frames.RootFrame.Metadata.GetTiffMetadata();
		TiffBitsPerPixel? bitsPerPixel = BitsPerPixel ?? tiffMetadata.BitsPerPixel;
		TiffPhotometricInterpretation? photometricInterpretation = PhotometricInterpretation ?? tiffMetadata.PhotometricInterpretation;
		TiffPredictor predictor = HorizontalPredictor ?? tiffMetadata.Predictor ?? TiffPredictor.None;
		TiffCompression compression = CompressionType ?? tiffMetadata.Compression ?? TiffCompression.None;
		SanitizeAndSetEncoderOptions(bitsPerPixel, image.PixelType.BitsPerPixel, photometricInterpretation, compression, predictor);
		using TiffStreamWriter tiffStreamWriter = new TiffStreamWriter(stream);
		Span<byte> buffer = stackalloc byte[4];
		long ifdOffset = WriteHeader(tiffStreamWriter, buffer);
		Image<TPixel> image2 = image;
		foreach (ImageFrame<TPixel> frame in image.Frames)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ifdOffset = WriteFrame(tiffStreamWriter, frame, image.Metadata, image2, ifdOffset);
			image2 = null;
		}
		long position = tiffStreamWriter.BaseStream.Position;
		foreach (var frameMarker in frameMarkers)
		{
			tiffStreamWriter.WriteMarkerFast(frameMarker.Item1, frameMarker.Item2, buffer);
		}
		tiffStreamWriter.BaseStream.Seek(position, SeekOrigin.Begin);
	}

	public static long WriteHeader(TiffStreamWriter writer, Span<byte> buffer)
	{
		writer.Write(ByteOrderMarker, buffer);
		writer.Write(42, buffer);
		return writer.PlaceMarker(buffer);
	}

	private long WriteFrame<TPixel>(TiffStreamWriter writer, ImageFrame<TPixel> frame, ImageMetadata imageMetadata, Image<TPixel> image, long ifdOffset) where TPixel : unmanaged, IPixel<TPixel>
	{
		using TiffBaseCompressor compressor = TiffCompressorFactory.Create(CompressionType ?? TiffCompression.None, writer.BaseStream, memoryAllocator, frame.Width, (int)BitsPerPixel.Value, compressionLevel, (HorizontalPredictor != TiffPredictor.Horizontal) ? TiffPredictor.None : HorizontalPredictor.Value);
		TiffEncoderEntriesCollector tiffEncoderEntriesCollector = new TiffEncoderEntriesCollector();
		using TiffBaseColorWriter<TPixel> tiffBaseColorWriter = TiffColorWriterFactory.Create(PhotometricInterpretation, frame, quantizer, pixelSamplingStrategy, memoryAllocator, configuration, tiffEncoderEntriesCollector, (int)BitsPerPixel.Value);
		int rowsPerStrip = CalcRowsPerStrip(frame.Height, tiffBaseColorWriter.BytesPerRow, CompressionType);
		tiffBaseColorWriter.Write(compressor, rowsPerStrip);
		if (image != null)
		{
			tiffEncoderEntriesCollector.ProcessMetadata(image, skipMetadata);
		}
		tiffEncoderEntriesCollector.ProcessMetadata(frame, skipMetadata);
		tiffEncoderEntriesCollector.ProcessFrameInfo(frame, imageMetadata);
		tiffEncoderEntriesCollector.ProcessImageFormat(this);
		if (writer.Position % 2 != 0L)
		{
			writer.Write(0);
		}
		frameMarkers.Add((ifdOffset, (uint)writer.Position));
		return WriteIfd(writer, tiffEncoderEntriesCollector.Entries);
	}

	private static int CalcRowsPerStrip(int height, int bytesPerRow, TiffCompression? compression)
	{
		if (compression.HasValue && compression == TiffCompression.Jpeg)
		{
			return height;
		}
		int num = (((compression.HasValue && compression == TiffCompression.Deflate) || (compression.HasValue && compression == TiffCompression.Lzw)) ? 16384 : 8192) / bytesPerRow;
		if (num > 0)
		{
			if (num < height)
			{
				return num;
			}
			return height;
		}
		return 1;
	}

	private long WriteIfd(TiffStreamWriter writer, List<IExifValue> entries)
	{
		if (entries.Count == 0)
		{
			TiffThrowHelper.ThrowArgumentException("There must be at least one entry per IFD.");
		}
		uint num = (uint)((int)writer.Position + (6 + entries.Count * 12));
		List<byte[]> list = new List<byte[]>();
		entries.Sort((IExifValue a, IExifValue b) => (ushort)a.Tag - (ushort)b.Tag);
		Span<byte> span = stackalloc byte[4];
		writer.Write((ushort)entries.Count, span);
		foreach (IExifValue entry in entries)
		{
			writer.Write((ushort)entry.Tag, span);
			writer.Write((ushort)entry.DataType, span);
			writer.Write(ExifWriter.GetNumberOfComponents(entry), span);
			uint length = ExifWriter.GetLength(entry);
			if (length <= 4)
			{
				writer.WritePadded(span[..ExifWriter.WriteValue(entry, span, 0)]);
				continue;
			}
			byte[] array = new byte[length];
			ExifWriter.WriteValue(entry, array, 0);
			list.Add(array);
			writer.Write(num, span);
			num += (uint)(array.Length + array.Length % 2);
		}
		long result = writer.PlaceMarker(span);
		foreach (byte[] item in list)
		{
			writer.Write(item);
			if (item.Length % 2 == 1)
			{
				writer.Write(0);
			}
		}
		return result;
	}

	private void SanitizeAndSetEncoderOptions(TiffBitsPerPixel? bitsPerPixel, int inputBitsPerPixel, TiffPhotometricInterpretation? photometricInterpretation, TiffCompression compression, TiffPredictor predictor)
	{
		if (bitsPerPixel.HasValue)
		{
			switch (bitsPerPixel)
			{
			case TiffBitsPerPixel.Bit1:
				if (IsOneBitCompression(compression))
				{
					SetEncoderOptions(bitsPerPixel, TiffPhotometricInterpretation.WhiteIsZero, compression, TiffPredictor.None);
				}
				else
				{
					SetEncoderOptions(bitsPerPixel, TiffPhotometricInterpretation.BlackIsZero, compression, TiffPredictor.None);
				}
				break;
			case TiffBitsPerPixel.Bit4:
				SetEncoderOptions(bitsPerPixel, TiffPhotometricInterpretation.PaletteColor, compression, TiffPredictor.None);
				break;
			case TiffBitsPerPixel.Bit8:
				SetEncoderOptions(bitsPerPixel, photometricInterpretation ?? TiffPhotometricInterpretation.BlackIsZero, compression, predictor);
				break;
			case TiffBitsPerPixel.Bit16:
				SetEncoderOptions(bitsPerPixel, TiffPhotometricInterpretation.BlackIsZero, compression, predictor);
				break;
			case TiffBitsPerPixel.Bit6:
			case TiffBitsPerPixel.Bit10:
			case TiffBitsPerPixel.Bit12:
			case TiffBitsPerPixel.Bit14:
			case TiffBitsPerPixel.Bit30:
			case TiffBitsPerPixel.Bit36:
			case TiffBitsPerPixel.Bit42:
			case TiffBitsPerPixel.Bit48:
				SetEncoderOptions(TiffBitsPerPixel.Bit24, TiffPhotometricInterpretation.Rgb, compression, TiffPredictor.None);
				break;
			case TiffBitsPerPixel.Bit64:
				SetEncoderOptions(TiffBitsPerPixel.Bit32, TiffPhotometricInterpretation.Rgb, compression, TiffPredictor.None);
				break;
			default:
				SetEncoderOptions(bitsPerPixel, TiffPhotometricInterpretation.Rgb, compression, predictor);
				break;
			}
			if (IsOneBitCompression(CompressionType) && BitsPerPixel != TiffBitsPerPixel.Bit1)
			{
				CompressionType = TiffCompression.None;
			}
			return;
		}
		if (!photometricInterpretation.HasValue)
		{
			if (IsOneBitCompression(CompressionType))
			{
				SetEncoderOptions(TiffBitsPerPixel.Bit1, TiffPhotometricInterpretation.WhiteIsZero, compression, TiffPredictor.None);
				return;
			}
			switch (inputBitsPerPixel)
			{
			case 8:
				SetEncoderOptions(TiffBitsPerPixel.Bit8, TiffPhotometricInterpretation.BlackIsZero, compression, predictor);
				break;
			case 16:
				SetEncoderOptions(TiffBitsPerPixel.Bit16, TiffPhotometricInterpretation.BlackIsZero, compression, predictor);
				break;
			default:
				SetEncoderOptions(TiffBitsPerPixel.Bit24, TiffPhotometricInterpretation.Rgb, compression, predictor);
				break;
			}
			return;
		}
		switch (photometricInterpretation)
		{
		case TiffPhotometricInterpretation.WhiteIsZero:
		case TiffPhotometricInterpretation.BlackIsZero:
			if (IsOneBitCompression(CompressionType))
			{
				SetEncoderOptions(TiffBitsPerPixel.Bit1, photometricInterpretation, compression, TiffPredictor.None);
			}
			else if (inputBitsPerPixel == 16)
			{
				SetEncoderOptions(TiffBitsPerPixel.Bit16, photometricInterpretation, compression, predictor);
			}
			else
			{
				SetEncoderOptions(TiffBitsPerPixel.Bit8, photometricInterpretation, compression, predictor);
			}
			break;
		case TiffPhotometricInterpretation.PaletteColor:
			SetEncoderOptions(TiffBitsPerPixel.Bit8, photometricInterpretation, compression, predictor);
			break;
		case TiffPhotometricInterpretation.Rgb:
			if (IsOneBitCompression(CompressionType))
			{
				compression = TiffCompression.None;
			}
			SetEncoderOptions(TiffBitsPerPixel.Bit24, photometricInterpretation, compression, predictor);
			break;
		default:
			SetEncoderOptions(TiffBitsPerPixel.Bit24, TiffPhotometricInterpretation.Rgb, compression, predictor);
			break;
		}
	}

	private void SetEncoderOptions(TiffBitsPerPixel? bitsPerPixel, TiffPhotometricInterpretation? photometricInterpretation, TiffCompression compression, TiffPredictor predictor)
	{
		BitsPerPixel = bitsPerPixel;
		PhotometricInterpretation = photometricInterpretation;
		CompressionType = compression;
		HorizontalPredictor = predictor;
	}

	public static bool IsOneBitCompression(TiffCompression? compression)
	{
		if (compression.HasValue)
		{
			TiffCompression valueOrDefault = compression.GetValueOrDefault();
			if (valueOrDefault - 2 <= TiffCompression.Ccitt1D)
			{
				return true;
			}
		}
		return false;
	}
}
