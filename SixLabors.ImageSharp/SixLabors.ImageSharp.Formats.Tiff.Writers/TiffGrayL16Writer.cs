using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Writers;

internal sealed class TiffGrayL16Writer<TPixel> : TiffCompositeColorWriter<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public override int BitsPerPixel => 16;

	public TiffGrayL16Writer(ImageFrame<TPixel> image, MemoryAllocator memoryAllocator, Configuration configuration, TiffEncoderEntriesCollector entriesCollector)
		: base(image, memoryAllocator, configuration, entriesCollector)
	{
	}

	protected override void EncodePixels(Span<TPixel> pixels, Span<byte> buffer)
	{
		PixelOperations<TPixel>.Instance.ToL16Bytes(base.Configuration, pixels, buffer, pixels.Length);
	}
}
