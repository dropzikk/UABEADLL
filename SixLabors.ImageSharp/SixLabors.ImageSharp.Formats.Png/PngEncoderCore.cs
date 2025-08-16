using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png.Chunks;
using SixLabors.ImageSharp.Formats.Png.Filters;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Formats.Png;

internal sealed class PngEncoderCore : IImageEncoderInternals, IDisposable
{
	private struct ScratchBuffer
	{
		private const int Size = 26;

		private unsafe fixed byte scratch[26];

		public unsafe Span<byte> Span => MemoryMarshal.CreateSpan(ref scratch[0], 26);
	}

	private const int MaxBlockSize = 65535;

	private readonly MemoryAllocator memoryAllocator;

	private readonly Configuration configuration;

	private ScratchBuffer chunkDataBuffer;

	private readonly PngEncoder encoder;

	private float? gamma;

	private PngColorType colorType;

	private byte bitDepth;

	private PngFilterMethod filterMethod;

	private PngInterlaceMode interlaceMode;

	private PngChunkFilter chunkFilter;

	private bool use16Bit;

	private int bytesPerPixel;

	private int width;

	private int height;

	private IMemoryOwner<byte> previousScanline;

	private IMemoryOwner<byte> currentScanline;

	private const string ColorProfileName = "ICC Profile";

	private IQuantizer? quantizer;

	private int derivedTransparencyIndex = -1;

	public PngEncoderCore(Configuration configuration, PngEncoder encoder)
	{
		this.configuration = configuration;
		memoryAllocator = configuration.MemoryAllocator;
		this.encoder = encoder;
		quantizer = encoder.Quantizer;
	}

	public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(image, "image");
		Guard.NotNull(stream, "stream");
		width = image.Width;
		height = image.Height;
		ImageMetadata metadata = image.Metadata;
		PngMetadata pngMetadata = GetPngMetadata(image);
		SanitizeAndSetEncoderOptions<TPixel>(encoder, pngMetadata, out use16Bit, out bytesPerPixel);
		stream.Write(PngConstants.HeaderBytes);
		ImageFrame<TPixel> imageFrame = null;
		ImageFrame<TPixel> imageFrame2 = image.Frames.RootFrame;
		int num = 0;
		bool flag = encoder.TransparentColorMode == PngTransparentColorMode.Clear;
		if (flag)
		{
			imageFrame2 = (imageFrame = imageFrame2.Clone());
			ClearTransparentPixels(imageFrame2);
		}
		IndexedImageFrame<TPixel> indexedImageFrame = CreateQuantizedImageAndUpdateBitDepth(pngMetadata, imageFrame2, imageFrame2.Bounds(), null);
		WriteHeaderChunk(stream);
		WriteGammaChunk(stream);
		WriteCicpChunk(stream, metadata);
		WriteColorProfileChunk(stream, metadata);
		WritePaletteChunk(stream, indexedImageFrame);
		WriteTransparencyChunk(stream, pngMetadata);
		WritePhysicalChunk(stream, metadata);
		WriteExifChunk(stream, metadata);
		WriteXmpChunk(stream, metadata);
		WriteTextChunks(stream, pngMetadata);
		if (image.Frames.Count > 1)
		{
			WriteAnimationControlChunk(stream, (uint)image.Frames.Count - ((!pngMetadata.AnimateRootFrame) ? 1u : 0u), pngMetadata.RepeatCount);
		}
		if (!pngMetadata.AnimateRootFrame || image.Frames.Count == 1)
		{
			FrameControl frameControl = new FrameControl((uint)width, (uint)height);
			WriteDataChunks(frameControl, imageFrame2.PixelBuffer.GetRegion(), indexedImageFrame, stream, isFrame: false);
			num++;
		}
		if (image.Frames.Count > 1)
		{
			imageFrame2 = image.Frames[num];
			PngFrameMetadata pngFrameMetadata = GetPngFrameMetadata(imageFrame2);
			PngDisposalMethod disposalMethod = pngFrameMetadata.DisposalMethod;
			FrameControl frameControl2 = WriteFrameControlChunk(stream, pngFrameMetadata, imageFrame2.Bounds(), 0u);
			uint num2 = 1u;
			if (pngMetadata.AnimateRootFrame)
			{
				WriteDataChunks(frameControl2, imageFrame2.PixelBuffer.GetRegion(), indexedImageFrame, stream, isFrame: false);
			}
			else
			{
				num2 += WriteDataChunks(frameControl2, imageFrame2.PixelBuffer.GetRegion(), indexedImageFrame, stream, isFrame: true);
			}
			num++;
			ReadOnlyMemory<TPixel>? previousPalette = indexedImageFrame?.Palette.ToArray();
			ImageFrame<TPixel> imageFrame3 = image.Frames.RootFrame;
			using ImageFrame<TPixel> imageFrame4 = new ImageFrame<TPixel>(image.Configuration, imageFrame3.Size());
			for (; num < image.Frames.Count; num++)
			{
				ImageFrame<TPixel> previousFrame = ((disposalMethod == PngDisposalMethod.RestoreToBackground) ? null : imageFrame3);
				imageFrame2 = image.Frames[num];
				ImageFrame<TPixel> nextFrame = ((num < image.Frames.Count - 1) ? image.Frames[num + 1] : null);
				pngFrameMetadata = GetPngFrameMetadata(imageFrame2);
				bool blend = pngFrameMetadata.BlendMethod == PngBlendMethod.Over;
				Rectangle item = AnimationUtilities.DeDuplicatePixels(image.Configuration, previousFrame, imageFrame2, nextFrame, imageFrame4, Color.Transparent, blend).Bounds;
				if (flag)
				{
					ClearTransparentPixels(imageFrame4);
				}
				frameControl2 = WriteFrameControlChunk(stream, pngFrameMetadata, item, num2);
				indexedImageFrame?.Dispose();
				indexedImageFrame = CreateQuantizedImageAndUpdateBitDepth(pngMetadata, imageFrame4, item, previousPalette);
				num2 += WriteDataChunks(frameControl2, imageFrame4.PixelBuffer.GetRegion(item), indexedImageFrame, stream, isFrame: true) + 1;
				imageFrame3 = imageFrame2;
				disposalMethod = pngFrameMetadata.DisposalMethod;
			}
		}
		WriteEndChunk(stream);
		stream.Flush();
		imageFrame?.Dispose();
		indexedImageFrame?.Dispose();
	}

	public void Dispose()
	{
		previousScanline?.Dispose();
		currentScanline?.Dispose();
	}

	private static PngMetadata GetPngMetadata<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (image.Metadata.TryGetPngMetadata(out PngMetadata metadata))
		{
			return (PngMetadata)metadata.DeepClone();
		}
		if (image.Metadata.TryGetGifMetadata(out GifMetadata metadata2))
		{
			return PngMetadata.FromAnimatedMetadata(metadata2.ToAnimatedImageMetadata());
		}
		if (image.Metadata.TryGetWebpMetadata(out WebpMetadata metadata3))
		{
			return PngMetadata.FromAnimatedMetadata(metadata3.ToAnimatedImageMetadata());
		}
		return new PngMetadata();
	}

	private static PngFrameMetadata GetPngFrameMetadata<TPixel>(ImageFrame<TPixel> frame) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (frame.Metadata.TryGetPngMetadata(out PngFrameMetadata metadata))
		{
			return (PngFrameMetadata)metadata.DeepClone();
		}
		if (frame.Metadata.TryGetGifMetadata(out GifFrameMetadata metadata2))
		{
			return PngFrameMetadata.FromAnimatedMetadata(metadata2.ToAnimatedImageFrameMetadata());
		}
		if (frame.Metadata.TryGetWebpFrameMetadata(out WebpFrameMetadata metadata3))
		{
			return PngFrameMetadata.FromAnimatedMetadata(metadata3.ToAnimatedImageFrameMetadata());
		}
		return new PngFrameMetadata();
	}

	private static void ClearTransparentPixels<TPixel>(ImageFrame<TPixel> clone) where TPixel : unmanaged, IPixel<TPixel>
	{
		clone.ProcessPixelRows(delegate(PixelAccessor<TPixel> accessor)
		{
			Rgba32 dest = default(Rgba32);
			Rgba32 source = Color.Transparent;
			for (int i = 0; i < accessor.Height; i++)
			{
				Span<TPixel> rowSpan = accessor.GetRowSpan(i);
				for (int j = 0; j < accessor.Width; j++)
				{
					rowSpan[j].ToRgba32(ref dest);
					if (dest.A == 0)
					{
						rowSpan[j].FromRgba32(source);
					}
				}
			}
		});
	}

	private IndexedImageFrame<TPixel>? CreateQuantizedImageAndUpdateBitDepth<TPixel>(PngMetadata metadata, ImageFrame<TPixel> frame, Rectangle bounds, ReadOnlyMemory<TPixel>? previousPalette) where TPixel : unmanaged, IPixel<TPixel>
	{
		IndexedImageFrame<TPixel> indexedImageFrame = CreateQuantizedFrame(encoder, colorType, bitDepth, metadata, frame, bounds, previousPalette);
		bitDepth = CalculateBitDepth(colorType, bitDepth, indexedImageFrame);
		return indexedImageFrame;
	}

	private void CollectGrayscaleBytes<TPixel>(ReadOnlySpan<TPixel> rowSpan) where TPixel : unmanaged, IPixel<TPixel>
	{
		Span<byte> span = currentScanline.GetSpan();
		if (colorType == PngColorType.Grayscale)
		{
			if (use16Bit)
			{
				using (IMemoryOwner<L16> buffer = memoryAllocator.Allocate<L16>(rowSpan.Length))
				{
					Span<L16> span2 = buffer.GetSpan();
					ref L16 reference = ref MemoryMarshal.GetReference(span2);
					PixelOperations<TPixel>.Instance.ToL16(configuration, rowSpan, span2);
					int num = 0;
					int num2 = 0;
					while (num < span2.Length)
					{
						L16 l = Unsafe.Add(ref reference, (uint)num);
						BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num2, 2), l.PackedValue);
						num++;
						num2 += 2;
					}
					return;
				}
			}
			if (bitDepth == 8)
			{
				PixelOperations<TPixel>.Instance.ToL8Bytes(configuration, rowSpan, span, rowSpan.Length);
				return;
			}
			using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(rowSpan.Length, AllocationOptions.Clean);
			int num3 = 255 / (ColorNumerics.GetColorCountForBitDepth(bitDepth) - 1);
			Span<byte> span3 = buffer2.GetSpan();
			PixelOperations<TPixel>.Instance.ToL8Bytes(configuration, rowSpan, span3, rowSpan.Length);
			PngEncoderHelpers.ScaleDownFrom8BitArray(span3, span, bitDepth, num3);
			return;
		}
		if (use16Bit)
		{
			using (IMemoryOwner<La32> buffer3 = memoryAllocator.Allocate<La32>(rowSpan.Length))
			{
				Span<La32> span4 = buffer3.GetSpan();
				ref La32 reference2 = ref MemoryMarshal.GetReference(span4);
				PixelOperations<TPixel>.Instance.ToLa32(configuration, rowSpan, span4);
				int num4 = 0;
				int num5 = 0;
				while (num4 < span4.Length)
				{
					La32 la = Unsafe.Add(ref reference2, (uint)num4);
					BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num5, 2), la.L);
					BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num5 + 2, 2), la.A);
					num4++;
					num5 += 4;
				}
				return;
			}
		}
		PixelOperations<TPixel>.Instance.ToLa16Bytes(configuration, rowSpan, span, rowSpan.Length);
	}

	private void CollectTPixelBytes<TPixel>(ReadOnlySpan<TPixel> rowSpan) where TPixel : unmanaged, IPixel<TPixel>
	{
		Span<byte> span = currentScanline.GetSpan();
		switch (bytesPerPixel)
		{
		case 4:
			PixelOperations<TPixel>.Instance.ToRgba32Bytes(configuration, rowSpan, span, rowSpan.Length);
			return;
		case 3:
			PixelOperations<TPixel>.Instance.ToRgb24Bytes(configuration, rowSpan, span, rowSpan.Length);
			return;
		case 8:
		{
			using IMemoryOwner<Rgba64> buffer = memoryAllocator.Allocate<Rgba64>(rowSpan.Length);
			Span<Rgba64> span2 = buffer.GetSpan();
			ref Rgba64 reference = ref MemoryMarshal.GetReference(span2);
			PixelOperations<TPixel>.Instance.ToRgba64(configuration, rowSpan, span2);
			int num = 0;
			int num2 = 0;
			while (num < rowSpan.Length)
			{
				Rgba64 rgba = Unsafe.Add(ref reference, (uint)num);
				BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num2, 2), rgba.R);
				BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num2 + 2, 2), rgba.G);
				BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num2 + 4, 2), rgba.B);
				BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num2 + 6, 2), rgba.A);
				num++;
				num2 += 8;
			}
			return;
		}
		}
		using IMemoryOwner<Rgb48> buffer2 = memoryAllocator.Allocate<Rgb48>(rowSpan.Length);
		Span<Rgb48> span3 = buffer2.GetSpan();
		ref Rgb48 reference2 = ref MemoryMarshal.GetReference(span3);
		PixelOperations<TPixel>.Instance.ToRgb48(configuration, rowSpan, span3);
		int num3 = 0;
		int num4 = 0;
		while (num3 < rowSpan.Length)
		{
			Rgb48 rgb = Unsafe.Add(ref reference2, (uint)num3);
			BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num4, 2), rgb.R);
			BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num4 + 2, 2), rgb.G);
			BinaryPrimitives.WriteUInt16BigEndian(span.Slice(num4 + 4, 2), rgb.B);
			num3++;
			num4 += 6;
		}
	}

	private void CollectPixelBytes<TPixel>(ReadOnlySpan<TPixel> rowSpan, IndexedImageFrame<TPixel>? quantized, int row) where TPixel : unmanaged, IPixel<TPixel>
	{
		switch (colorType)
		{
		case PngColorType.Palette:
			if (bitDepth < 8)
			{
				PngEncoderHelpers.ScaleDownFrom8BitArray(quantized.DangerousGetRowSpan(row), currentScanline.GetSpan(), bitDepth);
			}
			else
			{
				quantized?.DangerousGetRowSpan(row).CopyTo(currentScanline.GetSpan());
			}
			break;
		case PngColorType.Grayscale:
		case PngColorType.GrayscaleWithAlpha:
			CollectGrayscaleBytes(rowSpan);
			break;
		default:
			CollectTPixelBytes(rowSpan);
			break;
		}
	}

	private void FilterPixelBytes(ref Span<byte> filter, ref Span<byte> attempt)
	{
		int sum;
		switch (filterMethod)
		{
		case PngFilterMethod.None:
			NoneFilter.Encode(currentScanline.GetSpan(), filter);
			break;
		case PngFilterMethod.Sub:
			SubFilter.Encode(currentScanline.GetSpan(), filter, bytesPerPixel, out sum);
			break;
		case PngFilterMethod.Up:
			UpFilter.Encode(currentScanline.GetSpan(), previousScanline.GetSpan(), filter, out sum);
			break;
		case PngFilterMethod.Average:
			AverageFilter.Encode(currentScanline.GetSpan(), previousScanline.GetSpan(), filter, (uint)bytesPerPixel, out sum);
			break;
		case PngFilterMethod.Paeth:
			PaethFilter.Encode(currentScanline.GetSpan(), previousScanline.GetSpan(), filter, bytesPerPixel, out sum);
			break;
		default:
			ApplyOptimalFilteredScanline(ref filter, ref attempt);
			break;
		}
	}

	private void CollectAndFilterPixelRow<TPixel>(ReadOnlySpan<TPixel> rowSpan, ref Span<byte> filter, ref Span<byte> attempt, IndexedImageFrame<TPixel>? quantized, int row) where TPixel : unmanaged, IPixel<TPixel>
	{
		CollectPixelBytes(rowSpan, quantized, row);
		FilterPixelBytes(ref filter, ref attempt);
	}

	private void EncodeAdam7IndexedPixelRow(ReadOnlySpan<byte> row, ref Span<byte> filter, ref Span<byte> attempt)
	{
		if (bitDepth < 8)
		{
			PngEncoderHelpers.ScaleDownFrom8BitArray(row, currentScanline.GetSpan(), bitDepth);
		}
		else
		{
			row.CopyTo(currentScanline.GetSpan());
		}
		FilterPixelBytes(ref filter, ref attempt);
	}

	private void ApplyOptimalFilteredScanline(ref Span<byte> filter, ref Span<byte> attempt)
	{
		if (colorType == PngColorType.Palette || height == 1 || bitDepth < 8)
		{
			NoneFilter.Encode(currentScanline.GetSpan(), filter);
			return;
		}
		Span<byte> span = currentScanline.GetSpan();
		Span<byte> span2 = previousScanline.GetSpan();
		int num = int.MaxValue;
		SubFilter.Encode(span, attempt, bytesPerPixel, out var sum);
		if (sum < num)
		{
			num = sum;
			RuntimeUtility.Swap(ref filter, ref attempt);
		}
		UpFilter.Encode(span, span2, attempt, out sum);
		if (sum < num)
		{
			num = sum;
			RuntimeUtility.Swap(ref filter, ref attempt);
		}
		AverageFilter.Encode(span, span2, attempt, (uint)bytesPerPixel, out sum);
		if (sum < num)
		{
			num = sum;
			RuntimeUtility.Swap(ref filter, ref attempt);
		}
		PaethFilter.Encode(span, span2, attempt, bytesPerPixel, out sum);
		if (sum < num)
		{
			RuntimeUtility.Swap(ref filter, ref attempt);
		}
	}

	private void WriteHeaderChunk(Stream stream)
	{
		new PngHeader(width, height, bitDepth, colorType, 0, 0, interlaceMode).WriteTo(chunkDataBuffer.Span);
		WriteChunk(stream, PngChunkType.Header, chunkDataBuffer.Span, 0, 13);
	}

	private void WriteAnimationControlChunk(Stream stream, uint framesCount, uint playsCount)
	{
		new AnimationControl(framesCount, playsCount).WriteTo(chunkDataBuffer.Span);
		WriteChunk(stream, PngChunkType.AnimationControl, chunkDataBuffer.Span, 0, 8);
	}

	private void WritePaletteChunk<TPixel>(Stream stream, IndexedImageFrame<TPixel>? quantized) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (quantized == null)
		{
			return;
		}
		ReadOnlyMemory<TPixel> palette = quantized.Palette;
		int length = palette.Span.Length;
		int length2 = length * Unsafe.SizeOf<Rgb24>();
		bool flag = false;
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length2);
		using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(length);
		ref Rgb24 reference = ref MemoryMarshal.GetReference(MemoryMarshal.Cast<byte, Rgb24>(buffer.GetSpan()));
		ref byte reference2 = ref MemoryMarshal.GetReference(buffer2.GetSpan());
		using IMemoryOwner<Rgba32> buffer3 = quantized.Configuration.MemoryAllocator.Allocate<Rgba32>(length);
		Span<Rgba32> span = buffer3.GetSpan();
		PixelOperations<TPixel> instance = PixelOperations<TPixel>.Instance;
		Configuration obj = quantized.Configuration;
		palette = quantized.Palette;
		instance.ToRgba32(obj, palette.Span, span);
		ref Rgba32 reference3 = ref MemoryMarshal.GetReference(span);
		for (int i = 0; i < length; i++)
		{
			Rgba32 rgba = Unsafe.Add(ref reference3, (uint)i);
			byte b = rgba.A;
			Unsafe.Add(ref reference, (uint)i) = rgba.Rgb;
			if (b > encoder.Threshold)
			{
				b = byte.MaxValue;
			}
			flag = flag || b < byte.MaxValue;
			Unsafe.Add(ref reference2, (uint)i) = b;
		}
		WriteChunk(stream, PngChunkType.Palette, buffer.GetSpan(), 0, length2);
		if (flag)
		{
			WriteChunk(stream, PngChunkType.Transparency, buffer2.GetSpan(), 0, length);
		}
	}

	private void WritePhysicalChunk(Stream stream, ImageMetadata meta)
	{
		if (!chunkFilter.HasFlag(PngChunkFilter.ExcludePhysicalChunk))
		{
			PngPhysical.FromMetadata(meta).WriteTo(chunkDataBuffer.Span);
			WriteChunk(stream, PngChunkType.Physical, chunkDataBuffer.Span, 0, 9);
		}
	}

	private void WriteExifChunk(Stream stream, ImageMetadata meta)
	{
		if ((chunkFilter & PngChunkFilter.ExcludeExifChunk) != PngChunkFilter.ExcludeExifChunk && meta.ExifProfile != null && meta.ExifProfile.Values.Count != 0)
		{
			meta.SyncProfiles();
			WriteChunk(stream, PngChunkType.Exif, meta.ExifProfile.ToByteArray());
		}
	}

	private void WriteXmpChunk(Stream stream, ImageMetadata meta)
	{
		if ((chunkFilter & PngChunkFilter.ExcludeTextChunks) == PngChunkFilter.ExcludeTextChunks || meta.XmpProfile == null)
		{
			return;
		}
		byte[] data = meta.XmpProfile.Data;
		int? num = data?.Length;
		if ((num ?? 0) == 0)
		{
			return;
		}
		int length = data.Length + PngConstants.XmpKeyword.Length + 5;
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length);
		Span<byte> span = buffer.GetSpan();
		PngConstants.XmpKeyword.CopyTo(span);
		int length2 = PngConstants.XmpKeyword.Length;
		ref Span<byte> reference = ref span;
		int num2 = length2;
		Span<byte> span2 = reference.Slice(num2, reference.Length - num2);
		span2[4] = 0;
		span2[3] = 0;
		span2[2] = 0;
		span2[1] = 0;
		span2[0] = 0;
		length2 += 5;
		reference = ref span;
		num2 = length2;
		data.CopyTo(reference.Slice(num2, reference.Length - num2));
		WriteChunk(stream, PngChunkType.InternationalText, span);
	}

	private void WriteCicpChunk(Stream stream, ImageMetadata metaData)
	{
		if (metaData.CicpProfile != null)
		{
			if (metaData.CicpProfile.MatrixCoefficients != 0)
			{
				throw new NotSupportedException("CICP matrix coefficients other than Identity are not supported in PNG");
			}
			Span<byte> data = chunkDataBuffer.Span.Slice(0, 4);
			data[0] = (byte)metaData.CicpProfile.ColorPrimaries;
			data[1] = (byte)metaData.CicpProfile.TransferCharacteristics;
			data[2] = (byte)metaData.CicpProfile.MatrixCoefficients;
			data[3] = (metaData.CicpProfile.FullRange ? ((byte)1) : ((byte)0));
			WriteChunk(stream, PngChunkType.Cicp, data);
		}
	}

	private void WriteColorProfileChunk(Stream stream, ImageMetadata metaData)
	{
		if (metaData.IccProfile == null)
		{
			return;
		}
		byte[] dataBytes = metaData.IccProfile.ToByteArray();
		byte[] zlibCompressedBytes = GetZlibCompressedBytes(dataBytes);
		int length = "ICC Profile".Length + zlibCompressedBytes.Length + 2;
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length);
		Span<byte> span = buffer.GetSpan();
		PngConstants.Encoding.GetBytes("ICC Profile").CopyTo(span);
		int length2 = "ICC Profile".Length;
		span[length2++] = 0;
		span[length2++] = 0;
		int num = length2;
		zlibCompressedBytes.CopyTo(span.Slice(num, span.Length - num));
		WriteChunk(stream, PngChunkType.EmbeddedColorProfile, span);
	}

	private void WriteTextChunks(Stream stream, PngMetadata meta)
	{
		if ((chunkFilter & PngChunkFilter.ExcludeTextChunks) == PngChunkFilter.ExcludeTextChunks)
		{
			return;
		}
		foreach (PngTextData textDatum in meta.TextData)
		{
			if (textDatum.Value.Any((char c) => c > 'ÿ') || !string.IsNullOrWhiteSpace(textDatum.LanguageTag) || !string.IsNullOrWhiteSpace(textDatum.TranslatedKeyword))
			{
				byte[] bytes = PngConstants.Encoding.GetBytes(textDatum.Keyword);
				byte[] array = ((textDatum.Value.Length > encoder.TextCompressionThreshold) ? GetZlibCompressedBytes(PngConstants.TranslatedEncoding.GetBytes(textDatum.Value)) : PngConstants.TranslatedEncoding.GetBytes(textDatum.Value));
				byte[] bytes2 = PngConstants.TranslatedEncoding.GetBytes(textDatum.TranslatedKeyword);
				byte[] bytes3 = PngConstants.LanguageEncoding.GetBytes(textDatum.LanguageTag);
				int length = bytes.Length + array.Length + bytes2.Length + bytes3.Length + 5;
				using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length);
				Span<byte> span = buffer.GetSpan();
				bytes.CopyTo(span);
				int num = bytes.Length;
				span[num++] = 0;
				if (textDatum.Value.Length > encoder.TextCompressionThreshold)
				{
					span[num++] = 1;
				}
				else
				{
					span[num++] = 0;
				}
				span[num++] = 0;
				ref Span<byte> reference = ref span;
				int num2 = num;
				bytes3.CopyTo(reference.Slice(num2, reference.Length - num2));
				num += bytes3.Length;
				span[num++] = 0;
				reference = ref span;
				num2 = num;
				bytes2.CopyTo(reference.Slice(num2, reference.Length - num2));
				num += bytes2.Length;
				span[num++] = 0;
				reference = ref span;
				num2 = num;
				array.CopyTo(reference.Slice(num2, reference.Length - num2));
				WriteChunk(stream, PngChunkType.InternationalText, span);
			}
			else if (textDatum.Value.Length > encoder.TextCompressionThreshold)
			{
				byte[] zlibCompressedBytes = GetZlibCompressedBytes(PngConstants.Encoding.GetBytes(textDatum.Value));
				int length2 = textDatum.Keyword.Length + zlibCompressedBytes.Length + 2;
				using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(length2);
				Span<byte> span2 = buffer2.GetSpan();
				PngConstants.Encoding.GetBytes(textDatum.Keyword).CopyTo(span2);
				int length3 = textDatum.Keyword.Length;
				span2[length3++] = 0;
				span2[length3++] = 0;
				ref Span<byte> reference = ref span2;
				int num2 = length3;
				zlibCompressedBytes.CopyTo(reference.Slice(num2, reference.Length - num2));
				WriteChunk(stream, PngChunkType.CompressedText, span2);
			}
			else
			{
				int length4 = textDatum.Keyword.Length + textDatum.Value.Length + 1;
				using IMemoryOwner<byte> buffer3 = memoryAllocator.Allocate<byte>(length4);
				Span<byte> span3 = buffer3.GetSpan();
				PngConstants.Encoding.GetBytes(textDatum.Keyword).CopyTo(span3);
				int length5 = textDatum.Keyword.Length;
				span3[length5++] = 0;
				byte[] bytes4 = PngConstants.Encoding.GetBytes(textDatum.Value);
				ref Span<byte> reference = ref span3;
				int num2 = length5;
				bytes4.CopyTo(reference.Slice(num2, reference.Length - num2));
				WriteChunk(stream, PngChunkType.Text, span3);
			}
		}
	}

	private byte[] GetZlibCompressedBytes(byte[] dataBytes)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (ZlibDeflateStream zlibDeflateStream = new ZlibDeflateStream(memoryAllocator, memoryStream, encoder.CompressionLevel))
		{
			zlibDeflateStream.Write(dataBytes);
		}
		return memoryStream.ToArray();
	}

	private void WriteGammaChunk(Stream stream)
	{
		if ((chunkFilter & PngChunkFilter.ExcludeGammaChunk) != PngChunkFilter.ExcludeGammaChunk && gamma > 0f)
		{
			uint value = (uint)(gamma * 100000f).Value;
			BinaryPrimitives.WriteUInt32BigEndian(chunkDataBuffer.Span.Slice(0, 4), value);
			WriteChunk(stream, PngChunkType.Gamma, chunkDataBuffer.Span, 0, 4);
		}
	}

	private void WriteTransparencyChunk(Stream stream, PngMetadata pngMetadata)
	{
		if (!pngMetadata.TransparentColor.HasValue)
		{
			return;
		}
		Span<byte> span = chunkDataBuffer.Span;
		if (pngMetadata.ColorType == PngColorType.Rgb)
		{
			if (use16Bit)
			{
				Rgb48 rgb = pngMetadata.TransparentColor.Value.ToPixel<Rgb48>();
				BinaryPrimitives.WriteUInt16LittleEndian(span, rgb.R);
				BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(2, 2), rgb.G);
				BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(4, 2), rgb.B);
				WriteChunk(stream, PngChunkType.Transparency, chunkDataBuffer.Span, 0, 6);
			}
			else
			{
				span.Clear();
				Rgb24 rgb2 = pngMetadata.TransparentColor.Value.ToRgb24();
				span[1] = rgb2.R;
				span[3] = rgb2.G;
				span[5] = rgb2.B;
				WriteChunk(stream, PngChunkType.Transparency, chunkDataBuffer.Span, 0, 6);
			}
		}
		else if (pngMetadata.ColorType == PngColorType.Grayscale)
		{
			if (use16Bit)
			{
				L16 l = pngMetadata.TransparentColor.Value.ToPixel<L16>();
				BinaryPrimitives.WriteUInt16LittleEndian(span, l.PackedValue);
				WriteChunk(stream, PngChunkType.Transparency, chunkDataBuffer.Span, 0, 2);
			}
			else
			{
				L8 l2 = pngMetadata.TransparentColor.Value.ToPixel<L8>();
				span.Clear();
				span[1] = l2.PackedValue;
				WriteChunk(stream, PngChunkType.Transparency, chunkDataBuffer.Span, 0, 2);
			}
		}
	}

	private FrameControl WriteFrameControlChunk(Stream stream, PngFrameMetadata frameMetadata, Rectangle bounds, uint sequenceNumber)
	{
		FrameControl result = new FrameControl(sequenceNumber, (uint)bounds.Width, (uint)bounds.Height, (uint)bounds.Left, (uint)bounds.Top, (ushort)frameMetadata.FrameDelay.Numerator, (ushort)frameMetadata.FrameDelay.Denominator, frameMetadata.DisposalMethod, frameMetadata.BlendMethod);
		result.WriteTo(chunkDataBuffer.Span);
		WriteChunk(stream, PngChunkType.FrameControl, chunkDataBuffer.Span, 0, 26);
		return result;
	}

	private uint WriteDataChunks<TPixel>(FrameControl frameControl, Buffer2DRegion<TPixel> frame, IndexedImageFrame<TPixel>? quantized, Stream stream, bool isFrame) where TPixel : unmanaged, IPixel<TPixel>
	{
		byte[] array;
		int num;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (ZlibDeflateStream deflateStream = new ZlibDeflateStream(memoryAllocator, memoryStream, encoder.CompressionLevel))
			{
				if (interlaceMode == PngInterlaceMode.Adam7)
				{
					if (quantized != null)
					{
						EncodeAdam7IndexedPixels(quantized, deflateStream);
					}
					else
					{
						EncodeAdam7Pixels(frame, deflateStream);
					}
				}
				else
				{
					EncodePixels(frame, quantized, deflateStream);
				}
			}
			array = memoryStream.ToArray();
			num = array.Length;
		}
		int num2 = 65535;
		if (isFrame)
		{
			num2 -= 4;
		}
		int num3 = num / num2;
		if (num % num2 != 0)
		{
			num3++;
		}
		for (int i = 0; i < num3; i++)
		{
			int num4 = num - i * num2;
			if (num4 > num2)
			{
				num4 = num2;
			}
			if (isFrame)
			{
				uint sequenceNumber = (uint)(frameControl.SequenceNumber + 1 + i);
				WriteFrameDataChunk(stream, sequenceNumber, array, i * num2, num4);
			}
			else
			{
				WriteChunk(stream, PngChunkType.Data, array, i * num2, num4);
			}
		}
		return (uint)num3;
	}

	private void AllocateScanlineBuffers(int bytesPerScanline)
	{
		previousScanline?.Dispose();
		currentScanline?.Dispose();
		previousScanline = memoryAllocator.Allocate<byte>(bytesPerScanline, AllocationOptions.Clean);
		currentScanline = memoryAllocator.Allocate<byte>(bytesPerScanline, AllocationOptions.Clean);
	}

	private void EncodePixels<TPixel>(Buffer2DRegion<TPixel> pixels, IndexedImageFrame<TPixel>? quantized, ZlibDeflateStream deflateStream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = CalculateScanlineLength(pixels.Width);
		int length = num + 1;
		AllocateScanlineBuffers(num);
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
		using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
		Span<byte> filter = buffer.GetSpan();
		Span<byte> attempt = buffer2.GetSpan();
		for (int i = 0; i < pixels.Height; i++)
		{
			CollectAndFilterPixelRow(pixels.DangerousGetRowSpan(i), ref filter, ref attempt, quantized, i);
			deflateStream.Write(filter);
			SwapScanlineBuffers();
		}
	}

	private void EncodeAdam7Pixels<TPixel>(Buffer2DRegion<TPixel> pixels, ZlibDeflateStream deflateStream) where TPixel : unmanaged, IPixel<TPixel>
	{
		for (int i = 0; i < 7; i++)
		{
			int num = Adam7.FirstRow[i];
			int num2 = Adam7.FirstColumn[i];
			int num3 = Adam7.ComputeBlockWidth(pixels.Width, i);
			int num4 = ((bytesPerPixel <= 1) ? ((num3 * bitDepth + 7) / 8) : (num3 * bytesPerPixel));
			int length = num4 + 1;
			AllocateScanlineBuffers(num4);
			using IMemoryOwner<TPixel> buffer = memoryAllocator.Allocate<TPixel>(num3);
			using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
			using IMemoryOwner<byte> buffer3 = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
			Span<TPixel> span = buffer.GetSpan();
			Span<byte> filter = buffer2.GetSpan();
			Span<byte> attempt = buffer3.GetSpan();
			for (int j = num; j < pixels.Height; j += Adam7.RowIncrement[i])
			{
				Span<TPixel> span2 = pixels.DangerousGetRowSpan(j);
				int num5 = num2;
				int num6 = 0;
				while (num5 < pixels.Width)
				{
					span[num6] = span2[num5];
					num5 += Adam7.ColumnIncrement[i];
					num6++;
				}
				CollectAndFilterPixelRow<TPixel>(span, ref filter, ref attempt, null, -1);
				deflateStream.Write(filter);
				SwapScanlineBuffers();
			}
		}
	}

	private void EncodeAdam7IndexedPixels<TPixel>(IndexedImageFrame<TPixel> quantized, ZlibDeflateStream deflateStream) where TPixel : unmanaged, IPixel<TPixel>
	{
		for (int i = 0; i < 7; i++)
		{
			int num = Adam7.FirstRow[i];
			int num2 = Adam7.FirstColumn[i];
			int num3 = Adam7.ComputeBlockWidth(quantized.Width, i);
			int num4 = ((bytesPerPixel <= 1) ? ((num3 * bitDepth + 7) / 8) : (num3 * bytesPerPixel));
			int length = num4 + 1;
			AllocateScanlineBuffers(num4);
			using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(num3);
			using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
			using IMemoryOwner<byte> buffer3 = memoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
			Span<byte> span = buffer.GetSpan();
			Span<byte> filter = buffer2.GetSpan();
			Span<byte> attempt = buffer3.GetSpan();
			for (int j = num; j < quantized.Height; j += Adam7.RowIncrement[i])
			{
				ReadOnlySpan<byte> readOnlySpan = quantized.DangerousGetRowSpan(j);
				int num5 = num2;
				int num6 = 0;
				while (num5 < quantized.Width)
				{
					span[num6] = readOnlySpan[num5];
					num5 += Adam7.ColumnIncrement[i];
					num6++;
				}
				EncodeAdam7IndexedPixelRow(span, ref filter, ref attempt);
				deflateStream.Write(filter);
				SwapScanlineBuffers();
			}
		}
	}

	private void WriteEndChunk(Stream stream)
	{
		WriteChunk(stream, PngChunkType.End, null);
	}

	private void WriteChunk(Stream stream, PngChunkType type, Span<byte> data)
	{
		WriteChunk(stream, type, data, 0, data.Length);
	}

	private void WriteChunk(Stream stream, PngChunkType type, Span<byte> data, int offset, int length)
	{
		Span<byte> span = stackalloc byte[8];
		BinaryPrimitives.WriteInt32BigEndian(span, length);
		BinaryPrimitives.WriteUInt32BigEndian(span.Slice(4, 4), (uint)type);
		stream.Write(span);
		uint num = Crc32.Calculate(span.Slice(4, span.Length - 4));
		if (data.Length > 0 && length > 0)
		{
			stream.Write(data, offset, length);
			num = Crc32.Calculate(num, data.Slice(offset, length));
		}
		BinaryPrimitives.WriteUInt32BigEndian(span, num);
		stream.Write(span, 0, 4);
	}

	private void WriteFrameDataChunk(Stream stream, uint sequenceNumber, Span<byte> data, int offset, int length)
	{
		Span<byte> span = stackalloc byte[12];
		BinaryPrimitives.WriteInt32BigEndian(span, length + 4);
		BinaryPrimitives.WriteUInt32BigEndian(span.Slice(4, 4), 1717846356u);
		BinaryPrimitives.WriteUInt32BigEndian(span.Slice(8, 4), sequenceNumber);
		stream.Write(span);
		uint num = Crc32.Calculate(span.Slice(4, span.Length - 4));
		if (data.Length > 0 && length > 0)
		{
			stream.Write(data, offset, length);
			num = Crc32.Calculate(num, data.Slice(offset, length));
		}
		BinaryPrimitives.WriteUInt32BigEndian(span, num);
		stream.Write(span, 0, 4);
	}

	private int CalculateScanlineLength(int width)
	{
		int num = ((bitDepth == 16) ? 16 : 8);
		int num2 = width * bitDepth * bytesPerPixel;
		int num3 = num2 % num;
		if (num3 != 0)
		{
			num2 += num - num3;
		}
		return num2 / num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SwapScanlineBuffers()
	{
		RuntimeUtility.Swap(ref previousScanline, ref currentScanline);
	}

	private void SanitizeAndSetEncoderOptions<TPixel>(PngEncoder encoder, PngMetadata pngMetadata, out bool use16Bit, out int bytesPerPixel) where TPixel : unmanaged, IPixel<TPixel>
	{
		gamma = encoder.Gamma ?? pngMetadata.Gamma;
		colorType = encoder.ColorType ?? pngMetadata.ColorType ?? SuggestColorType<TPixel>();
		if (!encoder.FilterMethod.HasValue)
		{
			filterMethod = ((colorType != PngColorType.Palette) ? PngFilterMethod.Paeth : PngFilterMethod.None);
		}
		byte b = (byte)(encoder.BitDepth ?? pngMetadata.BitDepth ?? SuggestBitDepth<TPixel>());
		if (Array.IndexOf(PngConstants.ColorTypes[colorType], b) == -1)
		{
			b = 8;
		}
		bitDepth = b;
		use16Bit = b == 16;
		bytesPerPixel = CalculateBytesPerPixel(colorType, use16Bit);
		interlaceMode = (encoder.InterlaceMethod ?? pngMetadata.InterlaceMethod).Value;
		chunkFilter = (encoder.SkipMetadata ? PngChunkFilter.ExcludeAll : encoder.ChunkFilter.GetValueOrDefault());
	}

	private IndexedImageFrame<TPixel>? CreateQuantizedFrame<TPixel>(QuantizingImageEncoder encoder, PngColorType colorType, byte bitDepth, PngMetadata metadata, ImageFrame<TPixel> frame, Rectangle bounds, ReadOnlyMemory<TPixel>? previousPalette) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (colorType != PngColorType.Palette)
		{
			return null;
		}
		if (previousPalette.HasValue)
		{
			using (PaletteQuantizer<TPixel> paletteQuantizer = new PaletteQuantizer<TPixel>(configuration, this.quantizer.Options, previousPalette.Value, derivedTransparencyIndex))
			{
				paletteQuantizer.BuildPalette(encoder.PixelSamplingStrategy, frame);
				return paletteQuantizer.QuantizeFrame(frame, bounds);
			}
		}
		if (this.quantizer == null)
		{
			if (metadata.ColorTable.HasValue)
			{
				ReadOnlySpan<Color> span = metadata.ColorTable.Value.Span;
				int num = -1;
				for (int num2 = span.Length - 1; num2 >= 0; num2--)
				{
					if (span[num2].ToScaledVector4().W == 0f)
					{
						num = num2;
						break;
					}
				}
				derivedTransparencyIndex = num;
				this.quantizer = new PaletteQuantizer(metadata.ColorTable.Value, new QuantizerOptions
				{
					Dither = null
				}, derivedTransparencyIndex);
			}
			else
			{
				this.quantizer = new WuQuantizer(new QuantizerOptions
				{
					MaxColors = ColorNumerics.GetColorCountForBitDepth(bitDepth)
				});
			}
		}
		using IQuantizer<TPixel> quantizer = this.quantizer.CreatePixelSpecificQuantizer<TPixel>(frame.Configuration);
		quantizer.BuildPalette(encoder.PixelSamplingStrategy, frame);
		return quantizer.QuantizeFrame(frame, bounds);
	}

	private static byte CalculateBitDepth<TPixel>(PngColorType colorType, byte bitDepth, IndexedImageFrame<TPixel>? quantizedFrame) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (colorType == PngColorType.Palette)
		{
			byte val = (byte)Numerics.Clamp(ColorNumerics.GetBitsNeededForColorDepth(quantizedFrame.Palette.Length), 1, 8);
			byte b = Math.Max(bitDepth, val);
			byte b2;
			switch (b)
			{
			case 3:
				b2 = 4;
				break;
			case 5:
			case 6:
			case 7:
				b2 = 8;
				break;
			default:
				b2 = b;
				break;
			}
			b = b2;
			bitDepth = b;
		}
		if (Array.IndexOf(PngConstants.ColorTypes[colorType], bitDepth) < 0)
		{
			throw new NotSupportedException("Bit depth is not supported or not valid.");
		}
		return bitDepth;
	}

	private static int CalculateBytesPerPixel(PngColorType? pngColorType, bool use16Bit)
	{
		return pngColorType switch
		{
			PngColorType.Grayscale => (!use16Bit) ? 1 : 2, 
			PngColorType.GrayscaleWithAlpha => use16Bit ? 4 : 2, 
			PngColorType.Palette => 1, 
			PngColorType.Rgb => use16Bit ? 6 : 3, 
			_ => use16Bit ? 8 : 4, 
		};
	}

	private static PngColorType SuggestColorType<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel val = default(TPixel);
		if (!(val is A8))
		{
			if (!(val is Argb32))
			{
				if (!(val is Bgr24))
				{
					if (!(val is Bgra32))
					{
						if (!(val is L8))
						{
							if (!(val is L16))
							{
								if (!(val is La16))
								{
									if (!(val is La32))
									{
										if (!(val is Rgb24))
										{
											if (!(val is Rgba32))
											{
												if (!(val is Rgb48))
												{
													if (!(val is Rgba64))
													{
														if (val is RgbaVector)
														{
															return PngColorType.RgbWithAlpha;
														}
														return PngColorType.RgbWithAlpha;
													}
													return PngColorType.RgbWithAlpha;
												}
												return PngColorType.Rgb;
											}
											return PngColorType.RgbWithAlpha;
										}
										return PngColorType.Rgb;
									}
									return PngColorType.GrayscaleWithAlpha;
								}
								return PngColorType.GrayscaleWithAlpha;
							}
							return PngColorType.Grayscale;
						}
						return PngColorType.Grayscale;
					}
					return PngColorType.RgbWithAlpha;
				}
				return PngColorType.Rgb;
			}
			return PngColorType.RgbWithAlpha;
		}
		return PngColorType.GrayscaleWithAlpha;
	}

	private static PngBitDepth SuggestBitDepth<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel val = default(TPixel);
		if (!(val is A8))
		{
			if (!(val is Argb32))
			{
				if (!(val is Bgr24))
				{
					if (!(val is Bgra32))
					{
						if (!(val is L8))
						{
							if (!(val is L16))
							{
								if (!(val is La16))
								{
									if (!(val is La32))
									{
										if (!(val is Rgb24))
										{
											if (!(val is Rgba32))
											{
												if (!(val is Rgb48))
												{
													if (!(val is Rgba64))
													{
														if (val is RgbaVector)
														{
															return PngBitDepth.Bit16;
														}
														return PngBitDepth.Bit8;
													}
													return PngBitDepth.Bit16;
												}
												return PngBitDepth.Bit16;
											}
											return PngBitDepth.Bit8;
										}
										return PngBitDepth.Bit8;
									}
									return PngBitDepth.Bit16;
								}
								return PngBitDepth.Bit8;
							}
							return PngBitDepth.Bit16;
						}
						return PngBitDepth.Bit8;
					}
					return PngBitDepth.Bit8;
				}
				return PngBitDepth.Bit8;
			}
			return PngBitDepth.Bit8;
		}
		return PngBitDepth.Bit8;
	}
}
