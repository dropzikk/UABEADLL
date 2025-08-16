using System;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class WhiteIsZeroTiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly ushort bitsPerSample0;

	private readonly float factor;

	public WhiteIsZeroTiffColor(TiffBitsPerSample bitsPerSample)
	{
		bitsPerSample0 = bitsPerSample.Channel0;
		factor = (float)Math.Pow(2.0, (int)bitsPerSample0) - 1f;
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel val = default(TPixel);
		BitReader bitReader = new BitReader(data);
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i).Slice(left, width);
			for (int j = 0; j < span.Length; j++)
			{
				int num = bitReader.ReadBits(bitsPerSample0);
				float num2 = 1f - (float)num / factor;
				val.FromScaledVector4(new Vector4(num2, num2, num2, 1f));
				span[j] = val;
			}
			bitReader.NextRow();
		}
	}
}
