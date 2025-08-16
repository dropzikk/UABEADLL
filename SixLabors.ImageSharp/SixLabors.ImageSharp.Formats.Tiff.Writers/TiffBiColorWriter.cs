using System;
using System.Buffers;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.ImageSharp.Formats.Tiff.Writers;

internal sealed class TiffBiColorWriter<TPixel> : TiffBaseColorWriter<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly Image<TPixel> imageBlackWhite;

	private IMemoryOwner<byte> pixelsAsGray;

	private IMemoryOwner<byte> bitStrip;

	public override int BitsPerPixel => 1;

	public TiffBiColorWriter(ImageFrame<TPixel> image, MemoryAllocator memoryAllocator, Configuration configuration, TiffEncoderEntriesCollector entriesCollector)
		: base(image, memoryAllocator, configuration, entriesCollector)
	{
		imageBlackWhite = new Image<TPixel>(configuration, new ImageMetadata(), new ImageFrame<TPixel>[1] { image.Clone() });
		imageBlackWhite.Mutate(delegate(IImageProcessingContext img)
		{
			img.BinaryDither(KnownDitherings.FloydSteinberg);
		});
	}

	protected override void EncodeStrip(int y, int height, TiffBaseCompressor compressor)
	{
		int width = base.Image.Width;
		if (compressor.Method == TiffCompression.CcittGroup3Fax || compressor.Method == TiffCompression.Ccitt1D || compressor.Method == TiffCompression.CcittGroup4Fax)
		{
			int stripPixels = width * height;
			if (pixelsAsGray == null)
			{
				pixelsAsGray = base.MemoryAllocator.Allocate<byte>(stripPixels);
			}
			imageBlackWhite.ProcessPixelRows(delegate(PixelAccessor<TPixel> accessor)
			{
				Span<byte> span = pixelsAsGray.GetSpan();
				int num = y + height;
				int num2 = 0;
				for (int i = y; i < num; i++)
				{
					Span<TPixel> rowSpan = accessor.GetRowSpan(i);
					Span<byte> destBytes = span.Slice(num2 * width, width);
					PixelOperations<TPixel>.Instance.ToL8Bytes(base.Configuration, rowSpan, destBytes, width);
					num2++;
				}
				compressor.CompressStrip(span.Slice(0, stripPixels), height);
			});
			return;
		}
		int length = base.BytesPerRow * height;
		if (bitStrip == null)
		{
			bitStrip = base.MemoryAllocator.Allocate<byte>(length);
		}
		if (pixelsAsGray == null)
		{
			pixelsAsGray = base.MemoryAllocator.Allocate<byte>(width);
		}
		Span<byte> span2 = pixelsAsGray.GetSpan();
		Span<byte> rows = bitStrip.Slice(0, length);
		rows.Clear();
		Buffer2D<TPixel> pixelBuffer = imageBlackWhite.Frames.RootFrame.PixelBuffer;
		int num3 = 0;
		int num4 = y + height;
		for (int j = y; j < num4; j++)
		{
			int num5 = 0;
			int num6 = 0;
			int num7 = num3 * base.BytesPerRow;
			Span<byte> span3 = rows.Slice(num7, rows.Length - num7);
			Span<TPixel> span4 = pixelBuffer.DangerousGetRowSpan(j);
			PixelOperations<TPixel>.Instance.ToL8Bytes(base.Configuration, span4, span2, width);
			for (int k = 0; k < base.Image.Width; k++)
			{
				int num8 = 7 - num5;
				if (span2[k] == byte.MaxValue)
				{
					span3[num6] |= (byte)(1 << num8);
				}
				num5++;
				if (num5 == 8)
				{
					num6++;
					num5 = 0;
				}
			}
			num3++;
		}
		compressor.CompressStrip(rows, height);
	}

	protected override void Dispose(bool disposing)
	{
		imageBlackWhite?.Dispose();
		pixelsAsGray?.Dispose();
		bitStrip?.Dispose();
	}
}
