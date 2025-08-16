using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class WhiteIsZero1TiffColor<TPixel> : TiffBaseColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public override void Decode(ReadOnlySpan<byte> data, Buffer2D<TPixel> pixels, int left, int top, int width, int height)
	{
		nuint num = 0u;
		TPixel val = default(TPixel);
		TPixel val2 = default(TPixel);
		val.FromRgba32(Color.Black);
		val2.FromRgba32(Color.White);
		ref byte reference = ref MemoryMarshal.GetReference(data);
		for (nuint num2 = (uint)top; num2 < (uint)(top + height); num2++)
		{
			ref TPixel reference2 = ref MemoryMarshal.GetReference(pixels.DangerousGetRowSpan((int)num2));
			for (nuint num3 = (uint)left; num3 < (uint)(left + width); num3 += 8)
			{
				byte b = Unsafe.Add(ref reference, num++);
				nuint num4 = Math.Min((uint)(left + width) - num3, 8u);
				if (num4 == 8)
				{
					int num5 = (b >> 7) & 1;
					Unsafe.Add(ref reference2, num3) = ((num5 == 0) ? val2 : val);
					num5 = (b >> 6) & 1;
					Unsafe.Add(ref reference2, num3 + 1) = ((num5 == 0) ? val2 : val);
					num5 = (b >> 5) & 1;
					Unsafe.Add(ref reference2, num3 + 2) = ((num5 == 0) ? val2 : val);
					num5 = (b >> 4) & 1;
					Unsafe.Add(ref reference2, num3 + 3) = ((num5 == 0) ? val2 : val);
					num5 = (b >> 3) & 1;
					Unsafe.Add(ref reference2, num3 + 4) = ((num5 == 0) ? val2 : val);
					num5 = (b >> 2) & 1;
					Unsafe.Add(ref reference2, num3 + 5) = ((num5 == 0) ? val2 : val);
					num5 = (b >> 1) & 1;
					Unsafe.Add(ref reference2, num3 + 6) = ((num5 == 0) ? val2 : val);
					num5 = b & 1;
					Unsafe.Add(ref reference2, num3 + 7) = ((num5 == 0) ? val2 : val);
				}
				else
				{
					for (nuint num6 = 0u; num6 < num4; num6++)
					{
						int num7 = (b >> 7 - (int)num6) & 1;
						Unsafe.Add(ref reference2, num3 + num6) = ((num7 == 0) ? val2 : val);
					}
				}
			}
		}
	}
}
