using System;
using System.Numerics;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class PaletteTiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly ushort bitsPerSample0;

	private readonly TPixel[] palette;

	private const float InvMax = 1.5259022E-05f;

	public PaletteTiffColor(TiffBitsPerSample bitsPerSample, ushort[] colorMap)
	{
		bitsPerSample0 = bitsPerSample.Channel0;
		int colorCount = 1 << (int)bitsPerSample0;
		palette = GeneratePalette(colorMap, colorCount);
	}

	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		BitReader bitReader = new BitReader(data);
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i).Slice(left, width);
			for (int j = 0; j < span.Length; j++)
			{
				int num = bitReader.ReadBits(bitsPerSample0);
				span[j] = palette[num];
			}
			bitReader.NextRow();
		}
	}

	private static TPixel[] GeneratePalette(ushort[] colorMap, int colorCount)
	{
		TPixel[] array = new TPixel[colorCount];
		int num = colorCount * 2;
		for (int i = 0; i < array.Length; i++)
		{
			float x = (float)(int)colorMap[i] * 1.5259022E-05f;
			float y = (float)(int)colorMap[colorCount + i] * 1.5259022E-05f;
			float z = (float)(int)colorMap[num + i] * 1.5259022E-05f;
			array[i].FromScaledVector4(new Vector4(x, y, z, 1f));
		}
		return array;
	}
}
