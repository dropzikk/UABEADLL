using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;
using SixLabors.ImageSharp.Formats.Tiff.Utils;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

internal static class HorizontalPredictor
{
	public static void Undo(Span<byte> pixelBytes, int width, TiffColorType colorType, bool isBigEndian)
	{
		switch (colorType)
		{
		case TiffColorType.BlackIsZero8:
		case TiffColorType.WhiteIsZero8:
		case TiffColorType.PaletteColor:
			UndoGray8Bit(pixelBytes, width);
			break;
		case TiffColorType.BlackIsZero16:
		case TiffColorType.WhiteIsZero16:
			UndoGray16Bit(pixelBytes, width, isBigEndian);
			break;
		case TiffColorType.BlackIsZero32:
		case TiffColorType.WhiteIsZero32:
			UndoGray32Bit(pixelBytes, width, isBigEndian);
			break;
		case TiffColorType.Rgb888:
		case TiffColorType.CieLab:
			UndoRgb24Bit(pixelBytes, width);
			break;
		case TiffColorType.Rgba8888:
		case TiffColorType.Cmyk:
			UndoRgba32Bit(pixelBytes, width);
			break;
		case TiffColorType.Rgb161616:
			UndoRgb48Bit(pixelBytes, width, isBigEndian);
			break;
		case TiffColorType.Rgba16161616:
			UndoRgba64Bit(pixelBytes, width, isBigEndian);
			break;
		case TiffColorType.Rgb323232:
			UndoRgb96Bit(pixelBytes, width, isBigEndian);
			break;
		case TiffColorType.Rgba32323232:
			UndoRgba128Bit(pixelBytes, width, isBigEndian);
			break;
		}
	}

	public static void ApplyHorizontalPrediction(Span<byte> rows, int width, int bitsPerPixel)
	{
		switch (bitsPerPixel)
		{
		case 8:
			ApplyHorizontalPrediction8Bit(rows, width);
			break;
		case 16:
			ApplyHorizontalPrediction16Bit(rows, width);
			break;
		case 24:
			ApplyHorizontalPrediction24Bit(rows, width);
			break;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ApplyHorizontalPrediction24Bit(Span<byte> rows, int width)
	{
		int num = rows.Length / width;
		for (int i = 0; i < num; i++)
		{
			Span<Rgb24> span = MemoryMarshal.Cast<byte, Rgb24>(rows.Slice(i * width, width));
			for (int num2 = span.Length - 1; num2 >= 1; num2--)
			{
				byte r = (byte)(span[num2].R - span[num2 - 1].R);
				byte g = (byte)(span[num2].G - span[num2 - 1].G);
				byte b = (byte)(span[num2].B - span[num2 - 1].B);
				Rgb24 source = new Rgb24(r, g, b);
				span[num2].FromRgb24(source);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ApplyHorizontalPrediction16Bit(Span<byte> rows, int width)
	{
		int num = rows.Length / width;
		for (int i = 0; i < num; i++)
		{
			Span<L16> span = MemoryMarshal.Cast<byte, L16>(rows.Slice(i * width, width));
			for (int num2 = span.Length - 1; num2 >= 1; num2--)
			{
				ushort packedValue = (ushort)(span[num2].PackedValue - span[num2 - 1].PackedValue);
				span[num2].PackedValue = packedValue;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ApplyHorizontalPrediction8Bit(Span<byte> rows, int width)
	{
		int num = rows.Length / width;
		for (int i = 0; i < num; i++)
		{
			Span<byte> span = rows.Slice(i * width, width);
			for (int num2 = span.Length - 1; num2 >= 1; num2--)
			{
				span[num2] -= span[num2 - 1];
			}
		}
	}

	private static void UndoGray8Bit(Span<byte> pixelBytes, int width)
	{
		int num = pixelBytes.Length / width;
		for (int i = 0; i < num; i++)
		{
			Span<byte> span = pixelBytes.Slice(i * width, width);
			byte b = span[0];
			for (int j = 1; j < width; j++)
			{
				b += span[j];
				span[j] = b;
			}
		}
	}

	private static void UndoGray16Bit(Span<byte> pixelBytes, int width, bool isBigEndian)
	{
		int num = width * 2;
		int num2 = pixelBytes.Length / num;
		if (isBigEndian)
		{
			for (int i = 0; i < num2; i++)
			{
				int num3 = 0;
				Span<byte> span = pixelBytes.Slice(i * num, num);
				ushort num4 = TiffUtils.ConvertToUShortBigEndian(span.Slice(num3, 2));
				num3 += 2;
				for (int j = 1; j < width; j++)
				{
					Span<byte> span2 = span.Slice(num3, 2);
					ushort num5 = TiffUtils.ConvertToUShortBigEndian(span2);
					num4 += num5;
					BinaryPrimitives.WriteUInt16BigEndian(span2, num4);
					num3 += 2;
				}
			}
			return;
		}
		for (int k = 0; k < num2; k++)
		{
			int num6 = 0;
			Span<byte> span3 = pixelBytes.Slice(k * num, num);
			ushort num7 = TiffUtils.ConvertToUShortLittleEndian(span3.Slice(num6, 2));
			num6 += 2;
			for (int l = 1; l < width; l++)
			{
				Span<byte> span4 = span3.Slice(num6, 2);
				ushort num8 = TiffUtils.ConvertToUShortLittleEndian(span4);
				num7 += num8;
				BinaryPrimitives.WriteUInt16LittleEndian(span4, num7);
				num6 += 2;
			}
		}
	}

	private static void UndoGray32Bit(Span<byte> pixelBytes, int width, bool isBigEndian)
	{
		int num = width * 4;
		int num2 = pixelBytes.Length / num;
		if (isBigEndian)
		{
			for (int i = 0; i < num2; i++)
			{
				int num3 = 0;
				Span<byte> span = pixelBytes.Slice(i * num, num);
				uint num4 = TiffUtils.ConvertToUIntBigEndian(span.Slice(num3, 4));
				num3 += 4;
				for (int j = 1; j < width; j++)
				{
					Span<byte> span2 = span.Slice(num3, 4);
					uint num5 = TiffUtils.ConvertToUIntBigEndian(span2);
					num4 += num5;
					BinaryPrimitives.WriteUInt32BigEndian(span2, num4);
					num3 += 4;
				}
			}
			return;
		}
		for (int k = 0; k < num2; k++)
		{
			int num6 = 0;
			Span<byte> span3 = pixelBytes.Slice(k * num, num);
			uint num7 = TiffUtils.ConvertToUIntLittleEndian(span3.Slice(num6, 4));
			num6 += 4;
			for (int l = 1; l < width; l++)
			{
				Span<byte> span4 = span3.Slice(num6, 4);
				uint num8 = TiffUtils.ConvertToUIntLittleEndian(span4);
				num7 += num8;
				BinaryPrimitives.WriteUInt32LittleEndian(span4, num7);
				num6 += 4;
			}
		}
	}

	private static void UndoRgb24Bit(Span<byte> pixelBytes, int width)
	{
		int num = width * 3;
		int num2 = pixelBytes.Length / num;
		for (int i = 0; i < num2; i++)
		{
			Span<Rgb24> span = MemoryMarshal.Cast<byte, Rgb24>(pixelBytes.Slice(i * num, num)).Slice(0, width);
			ref Rgb24 reference = ref MemoryMarshal.GetReference(span);
			byte b = reference.R;
			byte b2 = reference.G;
			byte b3 = reference.B;
			for (int j = 1; j < span.Length; j++)
			{
				ref Rgb24 reference2 = ref span[j];
				b += reference2.R;
				b2 += reference2.G;
				b3 += reference2.B;
				Rgb24 source = new Rgb24(b, b2, b3);
				reference2.FromRgb24(source);
			}
		}
	}

	private static void UndoRgba32Bit(Span<byte> pixelBytes, int width)
	{
		int num = width * 4;
		int num2 = pixelBytes.Length / num;
		for (int i = 0; i < num2; i++)
		{
			Span<Rgba32> span = MemoryMarshal.Cast<byte, Rgba32>(pixelBytes.Slice(i * num, num)).Slice(0, width);
			ref Rgba32 reference = ref MemoryMarshal.GetReference(span);
			byte b = reference.R;
			byte b2 = reference.G;
			byte b3 = reference.B;
			byte b4 = reference.A;
			for (int j = 1; j < span.Length; j++)
			{
				ref Rgba32 reference2 = ref span[j];
				b += reference2.R;
				b2 += reference2.G;
				b3 += reference2.B;
				b4 += reference2.A;
				Rgba32 source = new Rgba32(b, b2, b3, b4);
				reference2.FromRgba32(source);
			}
		}
	}

	private static void UndoRgb48Bit(Span<byte> pixelBytes, int width, bool isBigEndian)
	{
		int num = width * 6;
		int num2 = pixelBytes.Length / num;
		if (isBigEndian)
		{
			for (int i = 0; i < num2; i++)
			{
				int num3 = 0;
				Span<byte> span = pixelBytes.Slice(i * num, num);
				ushort num4 = TiffUtils.ConvertToUShortBigEndian(span.Slice(num3, 2));
				num3 += 2;
				ushort num5 = TiffUtils.ConvertToUShortBigEndian(span.Slice(num3, 2));
				num3 += 2;
				ushort num6 = TiffUtils.ConvertToUShortBigEndian(span.Slice(num3, 2));
				num3 += 2;
				for (int j = 1; j < width; j++)
				{
					Span<byte> span2 = span.Slice(num3, 2);
					ushort num7 = TiffUtils.ConvertToUShortBigEndian(span2);
					num4 += num7;
					BinaryPrimitives.WriteUInt16BigEndian(span2, num4);
					num3 += 2;
					Span<byte> span3 = span.Slice(num3, 2);
					ushort num8 = TiffUtils.ConvertToUShortBigEndian(span3);
					num5 += num8;
					BinaryPrimitives.WriteUInt16BigEndian(span3, num5);
					num3 += 2;
					Span<byte> span4 = span.Slice(num3, 2);
					ushort num9 = TiffUtils.ConvertToUShortBigEndian(span4);
					num6 += num9;
					BinaryPrimitives.WriteUInt16BigEndian(span4, num6);
					num3 += 2;
				}
			}
			return;
		}
		for (int k = 0; k < num2; k++)
		{
			int num10 = 0;
			Span<byte> span5 = pixelBytes.Slice(k * num, num);
			ushort num11 = TiffUtils.ConvertToUShortLittleEndian(span5.Slice(num10, 2));
			num10 += 2;
			ushort num12 = TiffUtils.ConvertToUShortLittleEndian(span5.Slice(num10, 2));
			num10 += 2;
			ushort num13 = TiffUtils.ConvertToUShortLittleEndian(span5.Slice(num10, 2));
			num10 += 2;
			for (int l = 1; l < width; l++)
			{
				Span<byte> span6 = span5.Slice(num10, 2);
				ushort num14 = TiffUtils.ConvertToUShortLittleEndian(span6);
				num11 += num14;
				BinaryPrimitives.WriteUInt16LittleEndian(span6, num11);
				num10 += 2;
				Span<byte> span7 = span5.Slice(num10, 2);
				ushort num15 = TiffUtils.ConvertToUShortLittleEndian(span7);
				num12 += num15;
				BinaryPrimitives.WriteUInt16LittleEndian(span7, num12);
				num10 += 2;
				Span<byte> span8 = span5.Slice(num10, 2);
				ushort num16 = TiffUtils.ConvertToUShortLittleEndian(span8);
				num13 += num16;
				BinaryPrimitives.WriteUInt16LittleEndian(span8, num13);
				num10 += 2;
			}
		}
	}

	private static void UndoRgba64Bit(Span<byte> pixelBytes, int width, bool isBigEndian)
	{
		int num = width * 8;
		int num2 = pixelBytes.Length / num;
		if (isBigEndian)
		{
			for (int i = 0; i < num2; i++)
			{
				int num3 = 0;
				Span<byte> span = pixelBytes.Slice(i * num, num);
				ushort num4 = TiffUtils.ConvertToUShortBigEndian(span.Slice(num3, 2));
				num3 += 2;
				ushort num5 = TiffUtils.ConvertToUShortBigEndian(span.Slice(num3, 2));
				num3 += 2;
				ushort num6 = TiffUtils.ConvertToUShortBigEndian(span.Slice(num3, 2));
				num3 += 2;
				ushort num7 = TiffUtils.ConvertToUShortBigEndian(span.Slice(num3, 2));
				num3 += 2;
				for (int j = 1; j < width; j++)
				{
					Span<byte> span2 = span.Slice(num3, 2);
					ushort num8 = TiffUtils.ConvertToUShortBigEndian(span2);
					num4 += num8;
					BinaryPrimitives.WriteUInt16BigEndian(span2, num4);
					num3 += 2;
					Span<byte> span3 = span.Slice(num3, 2);
					ushort num9 = TiffUtils.ConvertToUShortBigEndian(span3);
					num5 += num9;
					BinaryPrimitives.WriteUInt16BigEndian(span3, num5);
					num3 += 2;
					Span<byte> span4 = span.Slice(num3, 2);
					ushort num10 = TiffUtils.ConvertToUShortBigEndian(span4);
					num6 += num10;
					BinaryPrimitives.WriteUInt16BigEndian(span4, num6);
					num3 += 2;
					Span<byte> span5 = span.Slice(num3, 2);
					ushort num11 = TiffUtils.ConvertToUShortBigEndian(span5);
					num7 += num11;
					BinaryPrimitives.WriteUInt16BigEndian(span5, num7);
					num3 += 2;
				}
			}
			return;
		}
		for (int k = 0; k < num2; k++)
		{
			int num12 = 0;
			Span<byte> span6 = pixelBytes.Slice(k * num, num);
			ushort num13 = TiffUtils.ConvertToUShortLittleEndian(span6.Slice(num12, 2));
			num12 += 2;
			ushort num14 = TiffUtils.ConvertToUShortLittleEndian(span6.Slice(num12, 2));
			num12 += 2;
			ushort num15 = TiffUtils.ConvertToUShortLittleEndian(span6.Slice(num12, 2));
			num12 += 2;
			ushort num16 = TiffUtils.ConvertToUShortLittleEndian(span6.Slice(num12, 2));
			num12 += 2;
			for (int l = 1; l < width; l++)
			{
				Span<byte> span7 = span6.Slice(num12, 2);
				ushort num17 = TiffUtils.ConvertToUShortLittleEndian(span7);
				num13 += num17;
				BinaryPrimitives.WriteUInt16LittleEndian(span7, num13);
				num12 += 2;
				Span<byte> span8 = span6.Slice(num12, 2);
				ushort num18 = TiffUtils.ConvertToUShortLittleEndian(span8);
				num14 += num18;
				BinaryPrimitives.WriteUInt16LittleEndian(span8, num14);
				num12 += 2;
				Span<byte> span9 = span6.Slice(num12, 2);
				ushort num19 = TiffUtils.ConvertToUShortLittleEndian(span9);
				num15 += num19;
				BinaryPrimitives.WriteUInt16LittleEndian(span9, num15);
				num12 += 2;
				Span<byte> span10 = span6.Slice(num12, 2);
				ushort num20 = TiffUtils.ConvertToUShortLittleEndian(span10);
				num16 += num20;
				BinaryPrimitives.WriteUInt16LittleEndian(span10, num16);
				num12 += 2;
			}
		}
	}

	private static void UndoRgb96Bit(Span<byte> pixelBytes, int width, bool isBigEndian)
	{
		int num = width * 12;
		int num2 = pixelBytes.Length / num;
		if (isBigEndian)
		{
			for (int i = 0; i < num2; i++)
			{
				int num3 = 0;
				Span<byte> span = pixelBytes.Slice(i * num, num);
				uint num4 = TiffUtils.ConvertToUIntBigEndian(span.Slice(num3, 4));
				num3 += 4;
				uint num5 = TiffUtils.ConvertToUIntBigEndian(span.Slice(num3, 4));
				num3 += 4;
				uint num6 = TiffUtils.ConvertToUIntBigEndian(span.Slice(num3, 4));
				num3 += 4;
				for (int j = 1; j < width; j++)
				{
					Span<byte> span2 = span.Slice(num3, 4);
					uint num7 = TiffUtils.ConvertToUIntBigEndian(span2);
					num4 += num7;
					BinaryPrimitives.WriteUInt32BigEndian(span2, num4);
					num3 += 4;
					Span<byte> span3 = span.Slice(num3, 4);
					uint num8 = TiffUtils.ConvertToUIntBigEndian(span3);
					num5 += num8;
					BinaryPrimitives.WriteUInt32BigEndian(span3, num5);
					num3 += 4;
					Span<byte> span4 = span.Slice(num3, 4);
					uint num9 = TiffUtils.ConvertToUIntBigEndian(span4);
					num6 += num9;
					BinaryPrimitives.WriteUInt32BigEndian(span4, num6);
					num3 += 4;
				}
			}
			return;
		}
		for (int k = 0; k < num2; k++)
		{
			int num10 = 0;
			Span<byte> span5 = pixelBytes.Slice(k * num, num);
			uint num11 = TiffUtils.ConvertToUIntLittleEndian(span5.Slice(num10, 4));
			num10 += 4;
			uint num12 = TiffUtils.ConvertToUIntLittleEndian(span5.Slice(num10, 4));
			num10 += 4;
			uint num13 = TiffUtils.ConvertToUIntLittleEndian(span5.Slice(num10, 4));
			num10 += 4;
			for (int l = 1; l < width; l++)
			{
				Span<byte> span6 = span5.Slice(num10, 4);
				uint num14 = TiffUtils.ConvertToUIntLittleEndian(span6);
				num11 += num14;
				BinaryPrimitives.WriteUInt32LittleEndian(span6, num11);
				num10 += 4;
				Span<byte> span7 = span5.Slice(num10, 4);
				uint num15 = TiffUtils.ConvertToUIntLittleEndian(span7);
				num12 += num15;
				BinaryPrimitives.WriteUInt32LittleEndian(span7, num12);
				num10 += 4;
				Span<byte> span8 = span5.Slice(num10, 4);
				uint num16 = TiffUtils.ConvertToUIntLittleEndian(span8);
				num13 += num16;
				BinaryPrimitives.WriteUInt32LittleEndian(span8, num13);
				num10 += 4;
			}
		}
	}

	private static void UndoRgba128Bit(Span<byte> pixelBytes, int width, bool isBigEndian)
	{
		int num = width * 16;
		int num2 = pixelBytes.Length / num;
		if (isBigEndian)
		{
			for (int i = 0; i < num2; i++)
			{
				int num3 = 0;
				Span<byte> span = pixelBytes.Slice(i * num, num);
				uint num4 = TiffUtils.ConvertToUIntBigEndian(span.Slice(num3, 4));
				num3 += 4;
				uint num5 = TiffUtils.ConvertToUIntBigEndian(span.Slice(num3, 4));
				num3 += 4;
				uint num6 = TiffUtils.ConvertToUIntBigEndian(span.Slice(num3, 4));
				num3 += 4;
				uint num7 = TiffUtils.ConvertToUIntBigEndian(span.Slice(num3, 4));
				num3 += 4;
				for (int j = 1; j < width; j++)
				{
					Span<byte> span2 = span.Slice(num3, 4);
					uint num8 = TiffUtils.ConvertToUIntBigEndian(span2);
					num4 += num8;
					BinaryPrimitives.WriteUInt32BigEndian(span2, num4);
					num3 += 4;
					Span<byte> span3 = span.Slice(num3, 4);
					uint num9 = TiffUtils.ConvertToUIntBigEndian(span3);
					num5 += num9;
					BinaryPrimitives.WriteUInt32BigEndian(span3, num5);
					num3 += 4;
					Span<byte> span4 = span.Slice(num3, 4);
					uint num10 = TiffUtils.ConvertToUIntBigEndian(span4);
					num6 += num10;
					BinaryPrimitives.WriteUInt32BigEndian(span4, num6);
					num3 += 4;
					Span<byte> span5 = span.Slice(num3, 4);
					uint num11 = TiffUtils.ConvertToUIntBigEndian(span5);
					num7 += num11;
					BinaryPrimitives.WriteUInt32BigEndian(span5, num7);
					num3 += 4;
				}
			}
			return;
		}
		for (int k = 0; k < num2; k++)
		{
			int num12 = 0;
			Span<byte> span6 = pixelBytes.Slice(k * num, num);
			uint num13 = TiffUtils.ConvertToUIntLittleEndian(span6.Slice(num12, 4));
			num12 += 4;
			uint num14 = TiffUtils.ConvertToUIntLittleEndian(span6.Slice(num12, 4));
			num12 += 4;
			uint num15 = TiffUtils.ConvertToUIntLittleEndian(span6.Slice(num12, 4));
			num12 += 4;
			uint num16 = TiffUtils.ConvertToUIntLittleEndian(span6.Slice(num12, 4));
			num12 += 4;
			for (int l = 1; l < width; l++)
			{
				Span<byte> span7 = span6.Slice(num12, 4);
				uint num17 = TiffUtils.ConvertToUIntLittleEndian(span7);
				num13 += num17;
				BinaryPrimitives.WriteUInt32LittleEndian(span7, num13);
				num12 += 4;
				Span<byte> span8 = span6.Slice(num12, 4);
				uint num18 = TiffUtils.ConvertToUIntLittleEndian(span8);
				num14 += num18;
				BinaryPrimitives.WriteUInt32LittleEndian(span8, num14);
				num12 += 4;
				Span<byte> span9 = span6.Slice(num12, 4);
				uint num19 = TiffUtils.ConvertToUIntLittleEndian(span9);
				num15 += num19;
				BinaryPrimitives.WriteUInt32LittleEndian(span9, num15);
				num12 += 4;
				Span<byte> span10 = span6.Slice(num12, 4);
				uint num20 = TiffUtils.ConvertToUIntLittleEndian(span10);
				num16 += num20;
				BinaryPrimitives.WriteUInt32LittleEndian(span10, num16);
				num12 += 4;
			}
		}
	}
}
