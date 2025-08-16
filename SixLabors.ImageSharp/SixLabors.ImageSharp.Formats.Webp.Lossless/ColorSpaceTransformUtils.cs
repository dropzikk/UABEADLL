using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal static class ColorSpaceTransformUtils
{
	public static void CollectColorBlueTransforms(Span<uint> bgra, int stride, int tileWidth, int tileHeight, int greenToBlue, int redToBlue, Span<int> histo)
	{
		if (Avx2.IsSupported && tileWidth >= 16)
		{
			Span<ushort> span = stackalloc ushort[16];
			Vector256<byte> mask = Vector256.Create(byte.MaxValue, 2, byte.MaxValue, 6, byte.MaxValue, 10, byte.MaxValue, 14, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 18, byte.MaxValue, 22, byte.MaxValue, 26, byte.MaxValue, 30, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			Vector256<byte> mask2 = Vector256.Create(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 2, byte.MaxValue, 6, byte.MaxValue, 10, byte.MaxValue, 14, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 18, byte.MaxValue, 22, byte.MaxValue, 26, byte.MaxValue, 30);
			Vector256<byte> right = Vector256.Create(byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0);
			Vector256<byte> right2 = Vector256.Create(0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue);
			Vector256<byte> right3 = Vector256.Create(byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0);
			Vector256<short> right4 = Vector256.Create(LosslessUtils.Cst5b(redToBlue));
			Vector256<short> right5 = Vector256.Create(LosslessUtils.Cst5b(greenToBlue));
			for (int i = 0; i < tileHeight; i++)
			{
				ref Span<uint> reference = ref bgra;
				int num = i * stride;
				ref uint reference2 = ref MemoryMarshal.GetReference(reference.Slice(num, reference.Length - num));
				for (nuint num2 = 0u; num2 <= (uint)(tileWidth - 16); num2 += 16)
				{
					nuint elementOffset = num2;
					nuint elementOffset2 = num2 + 8;
					Vector256<byte> vector = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference2, elementOffset)).AsByte();
					Vector256<byte> vector2 = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference2, elementOffset2)).AsByte();
					Vector256<byte> left = Avx2.Shuffle(vector, mask);
					Vector256<byte> right6 = Avx2.Shuffle(vector2, mask2);
					Vector256<byte> vector3 = Avx2.Or(left, right6);
					Vector256<byte> vector4 = Avx2.And(vector, right);
					Vector256<ushort> vector5 = Avx2.PackUnsignedSaturate(right: Avx2.And(vector2, right).AsInt32(), left: vector4.AsInt32());
					Vector256<byte> vector6 = Avx2.And(vector5.AsByte(), right2);
					Vector256<byte> vector7 = Avx2.And(Avx2.Subtract(right: Avx2.MultiplyHigh(vector3.AsInt16(), right4).AsByte(), left: Avx2.Subtract(right: Avx2.MultiplyHigh(vector6.AsInt16(), right5).AsByte(), left: vector5.AsByte())), right3);
					Unsafe.As<ushort, Vector256<ushort>>(ref MemoryMarshal.GetReference(span)) = vector7.AsUInt16();
					for (int j = 0; j < 16; j++)
					{
						histo[span[j]]++;
					}
				}
			}
			int num3 = tileWidth & 0xF;
			if (num3 > 0)
			{
				ref Span<uint> reference = ref bgra;
				int num = tileWidth - num3;
				CollectColorBlueTransformsNoneVectorized(reference.Slice(num, reference.Length - num), stride, num3, tileHeight, greenToBlue, redToBlue, histo);
			}
		}
		else if (Sse41.IsSupported)
		{
			Span<ushort> span2 = stackalloc ushort[8];
			Vector128<byte> mask3 = Vector128.Create(byte.MaxValue, 2, byte.MaxValue, 6, byte.MaxValue, 10, byte.MaxValue, 14, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			Vector128<byte> mask4 = Vector128.Create(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 2, byte.MaxValue, 6, byte.MaxValue, 10, byte.MaxValue, 14);
			Vector128<byte> right10 = Vector128.Create(byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0, byte.MaxValue, byte.MaxValue, 0, 0);
			Vector128<byte> right11 = Vector128.Create(0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue);
			Vector128<byte> right12 = Vector128.Create(byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0);
			Vector128<short> right13 = Vector128.Create(LosslessUtils.Cst5b(redToBlue));
			Vector128<short> right14 = Vector128.Create(LosslessUtils.Cst5b(greenToBlue));
			for (int k = 0; k < tileHeight; k++)
			{
				ref Span<uint> reference = ref bgra;
				int num = k * stride;
				ref uint reference3 = ref MemoryMarshal.GetReference(reference.Slice(num, reference.Length - num));
				for (nuint num4 = 0u; (int)num4 <= tileWidth - 8; num4 += 8)
				{
					nuint elementOffset3 = num4;
					nuint elementOffset4 = num4 + 4;
					Vector128<byte> vector8 = Unsafe.As<uint, Vector128<uint>>(ref Unsafe.Add(ref reference3, elementOffset3)).AsByte();
					Vector128<byte> vector9 = Unsafe.As<uint, Vector128<uint>>(ref Unsafe.Add(ref reference3, elementOffset4)).AsByte();
					Vector128<byte> left2 = Ssse3.Shuffle(vector8, mask3);
					Vector128<byte> right15 = Ssse3.Shuffle(vector9, mask4);
					Vector128<byte> vector10 = Sse2.Or(left2, right15);
					Vector128<byte> vector11 = Sse2.And(vector8, right10);
					Vector128<ushort> vector12 = Sse41.PackUnsignedSaturate(right: Sse2.And(vector9, right10).AsInt32(), left: vector11.AsInt32());
					Vector128<byte> vector13 = Sse2.And(vector12.AsByte(), right11);
					Vector128<byte> vector14 = Sse2.And(Sse2.Subtract(right: Sse2.MultiplyHigh(vector10.AsInt16(), right13).AsByte(), left: Sse2.Subtract(right: Sse2.MultiplyHigh(vector13.AsInt16(), right14).AsByte(), left: vector12.AsByte())), right12);
					Unsafe.As<ushort, Vector128<ushort>>(ref MemoryMarshal.GetReference(span2)) = vector14.AsUInt16();
					for (int l = 0; l < 8; l++)
					{
						histo[span2[l]]++;
					}
				}
			}
			int num5 = tileWidth & 7;
			if (num5 > 0)
			{
				ref Span<uint> reference = ref bgra;
				int num = tileWidth - num5;
				CollectColorBlueTransformsNoneVectorized(reference.Slice(num, reference.Length - num), stride, num5, tileHeight, greenToBlue, redToBlue, histo);
			}
		}
		else
		{
			CollectColorBlueTransformsNoneVectorized(bgra, stride, tileWidth, tileHeight, greenToBlue, redToBlue, histo);
		}
	}

	private static void CollectColorBlueTransformsNoneVectorized(Span<uint> bgra, int stride, int tileWidth, int tileHeight, int greenToBlue, int redToBlue, Span<int> histo)
	{
		int num = 0;
		while (tileHeight-- > 0)
		{
			for (int i = 0; i < tileWidth; i++)
			{
				histo[LosslessUtils.TransformColorBlue((sbyte)greenToBlue, (sbyte)redToBlue, bgra[num + i])]++;
			}
			num += stride;
		}
	}

	public static void CollectColorRedTransforms(Span<uint> bgra, int stride, int tileWidth, int tileHeight, int greenToRed, Span<int> histo)
	{
		if (Avx2.IsSupported && tileWidth >= 16)
		{
			Vector256<byte> right = Vector256.Create(65280).AsByte();
			Vector256<byte> right2 = Vector256.Create((short)255).AsByte();
			Vector256<short> right3 = Vector256.Create(LosslessUtils.Cst5b(greenToRed));
			Span<ushort> span = stackalloc ushort[16];
			for (int i = 0; i < tileHeight; i++)
			{
				ref Span<uint> reference = ref bgra;
				int num = i * stride;
				ref uint reference2 = ref MemoryMarshal.GetReference(reference.Slice(num, reference.Length - num));
				for (nuint num2 = 0u; num2 <= (uint)(tileWidth - 16); num2 += 16)
				{
					nuint elementOffset = num2;
					nuint elementOffset2 = num2 + 8;
					Vector256<byte> vector = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference2, elementOffset)).AsByte();
					Vector256<byte> vector2 = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference2, elementOffset2)).AsByte();
					Vector256<byte> vector3 = Avx2.And(vector, right);
					Vector256<byte> vector4 = Avx2.And(vector2, right);
					Vector256<ushort> vector5 = Avx2.PackUnsignedSaturate(vector3.AsInt32(), vector4.AsInt32());
					Vector256<int> left = Avx2.ShiftRightLogical(vector.AsInt32(), 16);
					Vector256<int> right4 = Avx2.ShiftRightLogical(vector2.AsInt32(), 16);
					Vector256<ushort> vector6 = Avx2.PackUnsignedSaturate(left, right4);
					Vector256<byte> vector7 = Avx2.And(Avx2.Subtract(right: Avx2.MultiplyHigh(vector5.AsInt16(), right3).AsByte(), left: vector6.AsByte()), right2);
					Unsafe.As<ushort, Vector256<ushort>>(ref MemoryMarshal.GetReference(span)) = vector7.AsUInt16();
					for (int j = 0; j < 16; j++)
					{
						histo[span[j]]++;
					}
				}
			}
			int num3 = tileWidth & 0xF;
			if (num3 > 0)
			{
				ref Span<uint> reference = ref bgra;
				int num = tileWidth - num3;
				CollectColorRedTransformsNoneVectorized(reference.Slice(num, reference.Length - num), stride, num3, tileHeight, greenToRed, histo);
			}
		}
		else if (Sse41.IsSupported)
		{
			Vector128<byte> right6 = Vector128.Create(65280).AsByte();
			Vector128<byte> right7 = Vector128.Create((short)255).AsByte();
			Vector128<short> right8 = Vector128.Create(LosslessUtils.Cst5b(greenToRed));
			Span<ushort> span2 = stackalloc ushort[8];
			for (int k = 0; k < tileHeight; k++)
			{
				ref Span<uint> reference = ref bgra;
				int num = k * stride;
				ref uint reference3 = ref MemoryMarshal.GetReference(reference.Slice(num, reference.Length - num));
				for (nuint num4 = 0u; (int)num4 <= tileWidth - 8; num4 += 8)
				{
					nuint elementOffset3 = num4;
					nuint elementOffset4 = num4 + 4;
					Vector128<byte> vector8 = Unsafe.As<uint, Vector128<uint>>(ref Unsafe.Add(ref reference3, elementOffset3)).AsByte();
					Vector128<byte> vector9 = Unsafe.As<uint, Vector128<uint>>(ref Unsafe.Add(ref reference3, elementOffset4)).AsByte();
					Vector128<byte> vector10 = Sse2.And(vector8, right6);
					Vector128<byte> vector11 = Sse2.And(vector9, right6);
					Vector128<ushort> vector12 = Sse41.PackUnsignedSaturate(vector10.AsInt32(), vector11.AsInt32());
					Vector128<int> left2 = Sse2.ShiftRightLogical(vector8.AsInt32(), 16);
					Vector128<int> right9 = Sse2.ShiftRightLogical(vector9.AsInt32(), 16);
					Vector128<ushort> vector13 = Sse41.PackUnsignedSaturate(left2, right9);
					Vector128<byte> vector14 = Sse2.And(Sse2.Subtract(right: Sse2.MultiplyHigh(vector12.AsInt16(), right8).AsByte(), left: vector13.AsByte()), right7);
					Unsafe.As<ushort, Vector128<ushort>>(ref MemoryMarshal.GetReference(span2)) = vector14.AsUInt16();
					for (int l = 0; l < 8; l++)
					{
						histo[span2[l]]++;
					}
				}
			}
			int num5 = tileWidth & 7;
			if (num5 > 0)
			{
				ref Span<uint> reference = ref bgra;
				int num = tileWidth - num5;
				CollectColorRedTransformsNoneVectorized(reference.Slice(num, reference.Length - num), stride, num5, tileHeight, greenToRed, histo);
			}
		}
		else
		{
			CollectColorRedTransformsNoneVectorized(bgra, stride, tileWidth, tileHeight, greenToRed, histo);
		}
	}

	private static void CollectColorRedTransformsNoneVectorized(Span<uint> bgra, int stride, int tileWidth, int tileHeight, int greenToRed, Span<int> histo)
	{
		int num = 0;
		while (tileHeight-- > 0)
		{
			for (int i = 0; i < tileWidth; i++)
			{
				histo[LosslessUtils.TransformColorRed((sbyte)greenToRed, bgra[num + i])]++;
			}
			num += stride;
		}
	}
}
