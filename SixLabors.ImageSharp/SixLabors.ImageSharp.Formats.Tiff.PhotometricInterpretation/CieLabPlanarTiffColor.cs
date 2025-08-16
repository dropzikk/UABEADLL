using System;
using System.Buffers;
using System.Numerics;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class CieLabPlanarTiffColor<TPixel> : TiffBasePlanarColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private static readonly ColorSpaceConverter ColorSpaceConverter = new ColorSpaceConverter();

	private const float Inv255 = 0.003921569f;

	public override void Decode(IMemoryOwner<byte>[] data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		Span<byte> span = data[0].GetSpan();
		Span<byte> span2 = data[1].GetSpan();
		Span<byte> span3 = data[2].GetSpan();
		TPixel val = default(TPixel);
		int num = 0;
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span4 = pixels.DangerousGetRowSpan(i).Slice(left, width);
			for (int j = 0; j < span4.Length; j++)
			{
				CieLab color = new CieLab((float)(span[num] & 0xFF) * 100f * 0.003921569f, (sbyte)span2[num], (sbyte)span3[num]);
				Rgb rgb = ColorSpaceConverter.ToRgb(in color);
				val.FromVector4(new Vector4(rgb.R, rgb.G, rgb.B, 1f));
				span4[j] = val;
				num++;
			}
		}
	}
}
