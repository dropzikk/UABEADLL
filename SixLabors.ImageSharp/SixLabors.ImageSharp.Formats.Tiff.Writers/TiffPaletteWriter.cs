using System;
using System.Buffers;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Formats.Tiff.Writers;

internal sealed class TiffPaletteWriter<TPixel> : TiffBaseColorWriter<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly int maxColors;

	private readonly int colorPaletteSize;

	private readonly int colorPaletteBytes;

	private readonly IndexedImageFrame<TPixel> quantizedFrame;

	private IMemoryOwner<byte> indexedPixelsBuffer;

	public override int BitsPerPixel { get; }

	public TiffPaletteWriter(ImageFrame<TPixel> frame, IQuantizer quantizer, IPixelSamplingStrategy pixelSamplingStrategy, MemoryAllocator memoryAllocator, Configuration configuration, TiffEncoderEntriesCollector entriesCollector, int bitsPerPixel)
		: base(frame, memoryAllocator, configuration, entriesCollector)
	{
		BitsPerPixel = bitsPerPixel;
		maxColors = ((BitsPerPixel == 4) ? 16 : 256);
		colorPaletteSize = maxColors * 3;
		colorPaletteBytes = colorPaletteSize * 2;
		using IQuantizer<TPixel> quantizer2 = quantizer.CreatePixelSpecificQuantizer<TPixel>(base.Configuration, new QuantizerOptions
		{
			MaxColors = maxColors
		});
		quantizer2.BuildPalette(pixelSamplingStrategy, frame);
		quantizedFrame = quantizer2.QuantizeFrame(frame, frame.Bounds());
		AddColorMapTag();
	}

	protected override void EncodeStrip(int y, int height, TiffBaseCompressor compressor)
	{
		int width = base.Image.Width;
		if (BitsPerPixel == 4)
		{
			int num = width >> 1;
			int num2 = (width & 1) * height;
			int length = num * height + num2;
			if (indexedPixelsBuffer == null)
			{
				indexedPixelsBuffer = base.MemoryAllocator.Allocate<byte>(length);
			}
			Span<byte> span = indexedPixelsBuffer.GetSpan();
			int num3 = 0;
			int num4 = y + height;
			for (int i = y; i < num4; i++)
			{
				ReadOnlySpan<byte> readOnlySpan = quantizedFrame.DangerousGetRowSpan(i);
				int num5 = 0;
				for (int j = 0; j < num; j++)
				{
					span[num3] = (byte)((readOnlySpan[num5] << 4) | (readOnlySpan[num5 + 1] & 0xF));
					num5 += 2;
					num3++;
				}
				if (width % 2 != 0)
				{
					span[num3++] = (byte)(readOnlySpan[num5] << 4);
				}
			}
			compressor.CompressStrip(span.Slice(0, num3), height);
		}
		else
		{
			int length2 = width * height;
			if (indexedPixelsBuffer == null)
			{
				indexedPixelsBuffer = base.MemoryAllocator.Allocate<byte>(length2);
			}
			Span<byte> span2 = indexedPixelsBuffer.GetSpan();
			int num6 = y + height;
			int num7 = 0;
			for (int k = y; k < num6; k++)
			{
				quantizedFrame.DangerousGetRowSpan(k).CopyTo(span2.Slice(num7 * width, width));
				num7++;
			}
			compressor.CompressStrip(span2.Slice(0, length2), height);
		}
	}

	protected override void Dispose(bool disposing)
	{
		quantizedFrame?.Dispose();
		indexedPixelsBuffer?.Dispose();
	}

	private void AddColorMapTag()
	{
		using IMemoryOwner<byte> buffer = base.MemoryAllocator.Allocate<byte>(colorPaletteBytes);
		Span<byte> span = buffer.GetSpan();
		ReadOnlySpan<TPixel> span2 = quantizedFrame.Palette.Span;
		Span<Rgb48> destinationPixels = MemoryMarshal.Cast<byte, Rgb48>(span[..(span2.Length * 3 * 2)]);
		PixelOperations<TPixel>.Instance.ToRgb48(base.Configuration, span2, destinationPixels);
		int num = maxColors - span2.Length;
		ushort[] array = new ushort[colorPaletteSize];
		int num2 = 0;
		for (int i = 0; i < span2.Length; i++)
		{
			array[num2++] = destinationPixels[i].R;
		}
		num2 += num;
		for (int j = 0; j < span2.Length; j++)
		{
			array[num2++] = destinationPixels[j].G;
		}
		num2 += num;
		for (int k = 0; k < span2.Length; k++)
		{
			array[num2++] = destinationPixels[k].B;
		}
		ExifShortArray entry = new ExifShortArray(ExifTagValue.ColorMap)
		{
			Value = array
		};
		base.EntriesCollector.AddOrReplace(entry);
	}
}
