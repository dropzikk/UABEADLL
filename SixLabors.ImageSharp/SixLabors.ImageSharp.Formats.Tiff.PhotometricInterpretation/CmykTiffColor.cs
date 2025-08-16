using System;
using System.Numerics;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class CmykTiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private const float Inv255 = 0.003921569f;

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel val = default(TPixel);
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i).Slice(left, width);
			for (int j = 0; j < span.Length; j++)
			{
				Rgb rgb = ColorSpaceConverter.ToRgb(new Cmyk((float)(int)data[num] * 0.003921569f, (float)(int)data[num + 1] * 0.003921569f, (float)(int)data[num + 2] * 0.003921569f, (float)(int)data[num + 3] * 0.003921569f));
				val.FromVector4(new Vector4(rgb.R, rgb.G, rgb.B, 1f));
				span[j] = val;
				num += 4;
			}
		}
	}
}
