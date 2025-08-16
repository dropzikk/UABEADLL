using System;
using System.Buffers;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Writers;

internal abstract class TiffCompositeColorWriter<TPixel> : TiffBaseColorWriter<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private IMemoryOwner<byte> rowBuffer;

	protected TiffCompositeColorWriter(ImageFrame<TPixel> image, MemoryAllocator memoryAllocator, Configuration configuration, TiffEncoderEntriesCollector entriesCollector)
		: base(image, memoryAllocator, configuration, entriesCollector)
	{
	}

	protected override void EncodeStrip(int y, int height, TiffBaseCompressor compressor)
	{
		if (rowBuffer == null)
		{
			rowBuffer = base.MemoryAllocator.Allocate<byte>(base.BytesPerRow * height);
		}
		rowBuffer.Clear();
		Span<byte> span = rowBuffer.GetSpan().Slice(0, base.BytesPerRow * height);
		int width = base.Image.Width;
		using IMemoryOwner<TPixel> buffer = base.MemoryAllocator.Allocate<TPixel>(height * width);
		Span<TPixel> span2 = buffer.GetSpan();
		int num = y + height;
		int num2 = 0;
		for (int i = y; i < num; i++)
		{
			base.Image.PixelBuffer.DangerousGetRowSpan(i).CopyTo(span2.Slice(num2 * width, width));
			num2++;
		}
		EncodePixels(span2, span);
		compressor.CompressStrip(span, height);
	}

	protected abstract void EncodePixels(Span<TPixel> pixels, Span<byte> buffer);

	protected override void Dispose(bool disposing)
	{
		rowBuffer?.Dispose();
	}
}
