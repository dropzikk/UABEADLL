using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class WhiteIsZero4TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		TPixel val = default(TPixel);
		int num = 0;
		bool flag = (width & 1) == 1;
		L8 source = default(L8);
		for (int i = top; i < top + height; i++)
		{
			Span<TPixel> span = pixels.DangerousGetRowSpan(i);
			int num2 = left;
			while (num2 < left + width - 1)
			{
				byte b = data[num++];
				byte packedValue = (byte)((15 - ((b & 0xF0) >> 4)) * 17);
				source.PackedValue = packedValue;
				val.FromL8(source);
				span[num2++] = val;
				byte packedValue2 = (byte)((15 - (b & 0xF)) * 17);
				source.PackedValue = packedValue2;
				val.FromL8(source);
				span[num2++] = val;
			}
			if (flag)
			{
				byte b2 = data[num++];
				byte packedValue3 = (byte)((15 - ((b2 & 0xF0) >> 4)) * 17);
				source.PackedValue = packedValue3;
				val.FromL8(source);
				span[left + width - 1] = val;
			}
		}
	}
}
