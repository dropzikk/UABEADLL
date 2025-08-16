using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Writers;

internal sealed class TiffRgbWriter<TPixel> : TiffCompositeColorWriter<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public override int BitsPerPixel => 24;

	public TiffRgbWriter(ImageFrame<TPixel> image, MemoryAllocator memoryAllocator, Configuration configuration, TiffEncoderEntriesCollector entriesCollector)
		: base(image, memoryAllocator, configuration, entriesCollector)
	{
	}

	protected override void EncodePixels(Span<TPixel> pixels, Span<byte> buffer)
	{
		PixelOperations<TPixel>.Instance.ToRgb24Bytes(base.Configuration, pixels, buffer, pixels.Length);
	}
}
