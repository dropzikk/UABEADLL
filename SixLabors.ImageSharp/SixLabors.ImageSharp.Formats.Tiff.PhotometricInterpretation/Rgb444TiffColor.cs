using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class Rgb444TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel val = default(TPixel);
		int num = 0;
		Bgra4444 bgra = default(Bgra4444);
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i);
			for (int j = left; j < left + width; j += 2)
			{
				byte r = (byte)((data[num] & 0xF0) >> 4);
				byte g = (byte)(data[num] & 0xF);
				num++;
				byte b = (byte)((data[num] & 0xF0) >> 4);
				bgra.PackedValue = ToBgraPackedValue(b, g, r);
				val.FromScaledVector4(bgra.ToScaledVector4());
				span[j] = val;
				if (j + 1 >= span.Length)
				{
					num++;
					break;
				}
				r = (byte)(data[num] & 0xF);
				num++;
				g = (byte)((data[num] & 0xF0) >> 4);
				b = (byte)(data[num] & 0xF);
				num++;
				bgra.PackedValue = ToBgraPackedValue(b, g, r);
				val.FromScaledVector4(bgra.ToScaledVector4());
				span[j + 1] = val;
			}
		}
	}

	private static ushort ToBgraPackedValue(byte b, byte g, byte r)
	{
		return (ushort)(b | (g << 4) | (r << 8) | 0xF000);
	}
}
