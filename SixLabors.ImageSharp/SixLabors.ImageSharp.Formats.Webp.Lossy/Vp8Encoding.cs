using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal static class Vp8Encoding
{
	private const int KC1 = 85627;

	private const int KC2 = 35468;

	private static readonly byte[] Clip1 = GetClip1();

	private const int I16DC16 = 0;

	private const int I16TM16 = 16;

	private const int I16VE16 = 512;

	private const int I16HE16 = 528;

	private const int C8DC8 = 1024;

	private const int C8TM8 = 1040;

	private const int C8VE8 = 1280;

	private const int C8HE8 = 1296;

	public static readonly int[] Vp8I16ModeOffsets = new int[4] { 0, 16, 512, 528 };

	public static readonly int[] Vp8UvModeOffsets = new int[4] { 1024, 1040, 1280, 1296 };

	private const int I4DC4 = 1536;

	private const int I4TM4 = 1540;

	private const int I4VE4 = 1544;

	private const int I4HE4 = 1548;

	private const int I4RD4 = 1552;

	private const int I4VR4 = 1556;

	private const int I4LD4 = 1560;

	private const int I4VL4 = 1564;

	private const int I4HD4 = 1664;

	private const int I4HU4 = 1668;

	public static readonly int[] Vp8I4ModeOffsets = new int[10] { 1536, 1540, 1544, 1548, 1552, 1556, 1560, 1564, 1664, 1668 };

	private static byte[] GetClip1()
	{
		byte[] array = new byte[766];
		for (int i = -255; i <= 510; i++)
		{
			array[255 + i] = Clip8b(i);
		}
		return array;
	}

	public static void ITransformTwo(Span<byte> reference, Span<short> input, Span<byte> dst, Span<int> scratch)
	{
		if (Sse2.IsSupported)
		{
			ref short reference2 = ref MemoryMarshal.GetReference(input);
			Vector128<long> left = Vector128.Create(Unsafe.As<short, long>(ref reference2), 0L);
			Vector128<long> left2 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 4)), 0L);
			Vector128<long> left3 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 8)), 0L);
			Vector128<long> left4 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 12)), 0L);
			Vector128<long> right = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 16)), 0L);
			Vector128<long> right2 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 20)), 0L);
			Vector128<long> right3 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 24)), 0L);
			Vector128<long> right4 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 28)), 0L);
			left = Sse2.UnpackLow(left, right);
			left2 = Sse2.UnpackLow(left2, right2);
			left3 = Sse2.UnpackLow(left3, right3);
			left4 = Sse2.UnpackLow(left4, right4);
			InverseTransformVerticalPass(left, left3, left2, left4, out var tmp, out var tmp2, out var tmp3, out var tmp4);
			LossyUtils.Vp8Transpose_2_4x4_16b(tmp, tmp2, tmp3, tmp4, out var output, out var output2, out var output3, out var output4);
			InverseTransformHorizontalPass(output, output3, output2, output4, out var shifted, out var shifted2, out var shifted3, out var shifted4);
			LossyUtils.Vp8Transpose_2_4x4_16b(shifted, shifted2, shifted3, shifted4, out output, out output2, out output3, out output4);
			Vector128<byte> zero = Vector128<byte>.Zero;
			Vector128<byte> zero2 = Vector128<byte>.Zero;
			Vector128<byte> zero3 = Vector128<byte>.Zero;
			Vector128<byte> zero4 = Vector128<byte>.Zero;
			ref byte reference3 = ref MemoryMarshal.GetReference(reference);
			zero = Vector128.Create(Unsafe.As<byte, long>(ref reference3), 0L).AsByte();
			zero2 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 32)), 0L).AsByte();
			zero3 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 64)), 0L).AsByte();
			zero4 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 96)), 0L).AsByte();
			zero = Sse2.UnpackLow(zero, Vector128<byte>.Zero);
			zero2 = Sse2.UnpackLow(zero2, Vector128<byte>.Zero);
			zero3 = Sse2.UnpackLow(zero3, Vector128<byte>.Zero);
			zero4 = Sse2.UnpackLow(zero4, Vector128<byte>.Zero);
			Vector128<short> vector = Sse2.Add(zero.AsInt16(), output.AsInt16());
			Vector128<short> vector2 = Sse2.Add(zero2.AsInt16(), output2.AsInt16());
			Vector128<short> vector3 = Sse2.Add(zero3.AsInt16(), output3.AsInt16());
			Vector128<short> vector4 = Sse2.Add(zero4.AsInt16(), output4.AsInt16());
			zero = Sse2.PackUnsignedSaturate(vector, vector);
			zero2 = Sse2.PackUnsignedSaturate(vector2, vector2);
			zero3 = Sse2.PackUnsignedSaturate(vector3, vector3);
			zero4 = Sse2.PackUnsignedSaturate(vector4, vector4);
			ref byte reference4 = ref MemoryMarshal.GetReference(dst);
			Unsafe.As<byte, Vector64<byte>>(ref reference4) = zero.GetLower();
			Unsafe.As<byte, Vector64<byte>>(ref Unsafe.Add(ref reference4, 32)) = zero2.GetLower();
			Unsafe.As<byte, Vector64<byte>>(ref Unsafe.Add(ref reference4, 64)) = zero3.GetLower();
			Unsafe.As<byte, Vector64<byte>>(ref Unsafe.Add(ref reference4, 96)) = zero4.GetLower();
		}
		else
		{
			ITransformOne(reference, input, dst, scratch);
			ref Span<byte> reference5 = ref reference;
			Span<byte> reference6 = reference5.Slice(4, reference5.Length - 4);
			Span<short> input2 = input.Slice(16, input.Length - 16);
			reference5 = ref dst;
			ITransformOne(reference6, input2, reference5.Slice(4, reference5.Length - 4), scratch);
		}
	}

	public static void ITransformOne(Span<byte> reference, Span<short> input, Span<byte> dst, Span<int> scratch)
	{
		if (Sse2.IsSupported)
		{
			ref short reference2 = ref MemoryMarshal.GetReference(input);
			Vector128<long> @in = Vector128.Create(Unsafe.As<short, long>(ref reference2), 0L);
			Vector128<long> in2 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 4)), 0L);
			Vector128<long> in3 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 8)), 0L);
			Vector128<long> in4 = Vector128.Create(Unsafe.As<short, long>(ref Unsafe.Add(ref reference2, 12)), 0L);
			InverseTransformVerticalPass(@in, in3, in2, in4, out var tmp, out var tmp2, out var tmp3, out var tmp4);
			LossyUtils.Vp8Transpose_2_4x4_16b(tmp, tmp2, tmp3, tmp4, out var output, out var output2, out var output3, out var output4);
			InverseTransformHorizontalPass(output, output3, output2, output4, out var shifted, out var shifted2, out var shifted3, out var shifted4);
			LossyUtils.Vp8Transpose_2_4x4_16b(shifted, shifted2, shifted3, shifted4, out output, out output2, out output3, out output4);
			Vector128<byte> zero = Vector128<byte>.Zero;
			Vector128<byte> zero2 = Vector128<byte>.Zero;
			Vector128<byte> zero3 = Vector128<byte>.Zero;
			_ = Vector128<byte>.Zero;
			ref byte reference3 = ref MemoryMarshal.GetReference(reference);
			zero = Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref reference3)).AsByte();
			zero2 = Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref Unsafe.Add(ref reference3, 32))).AsByte();
			zero3 = Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref Unsafe.Add(ref reference3, 64))).AsByte();
			Vector128<byte> left = Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref Unsafe.Add(ref reference3, 96))).AsByte();
			zero = Sse2.UnpackLow(zero, Vector128<byte>.Zero);
			zero2 = Sse2.UnpackLow(zero2, Vector128<byte>.Zero);
			zero3 = Sse2.UnpackLow(zero3, Vector128<byte>.Zero);
			Vector128<byte> vector = Sse2.UnpackLow(left, Vector128<byte>.Zero);
			Vector128<short> vector2 = Sse2.Add(zero.AsInt16(), output.AsInt16());
			Vector128<short> vector3 = Sse2.Add(zero2.AsInt16(), output2.AsInt16());
			Vector128<short> vector4 = Sse2.Add(zero3.AsInt16(), output3.AsInt16());
			Vector128<short> vector5 = Sse2.Add(vector.AsInt16(), output4.AsInt16());
			zero = Sse2.PackUnsignedSaturate(vector2, vector2);
			zero2 = Sse2.PackUnsignedSaturate(vector3, vector3);
			zero3 = Sse2.PackUnsignedSaturate(vector4, vector4);
			Vector128<byte> vector6 = Sse2.PackUnsignedSaturate(vector5, vector5);
			ref byte reference4 = ref MemoryMarshal.GetReference(dst);
			int num = Sse2.ConvertToInt32(zero.AsInt32());
			int num2 = Sse2.ConvertToInt32(zero2.AsInt32());
			int num3 = Sse2.ConvertToInt32(zero3.AsInt32());
			int num4 = Sse2.ConvertToInt32(vector6.AsInt32());
			Unsafe.As<byte, int>(ref reference4) = num;
			Unsafe.As<byte, int>(ref Unsafe.Add(ref reference4, 32)) = num2;
			Unsafe.As<byte, int>(ref Unsafe.Add(ref reference4, 64)) = num3;
			Unsafe.As<byte, int>(ref Unsafe.Add(ref reference4, 96)) = num4;
		}
		else
		{
			Span<int> span = scratch.Slice(0, 16);
			for (int i = 0; i < 4; i++)
			{
				int num5 = input[0] + input[8];
				int num6 = input[0] - input[8];
				int num7 = Mul(input[4], 35468) - Mul(input[12], 85627);
				int num8 = Mul(input[4], 85627) + Mul(input[12], 35468);
				span[0] = num5 + num8;
				span[1] = num6 + num7;
				span[2] = num6 - num7;
				span[3] = num5 - num8;
				ref Span<int> reference5 = ref span;
				span = reference5.Slice(4, reference5.Length - 4);
				input = input.Slice(1, input.Length - 1);
			}
			span = scratch;
			for (int i = 0; i < 4; i++)
			{
				int num9 = span[0] + 4;
				int num10 = num9 + span[8];
				int num11 = num9 - span[8];
				int num12 = Mul(span[4], 35468) - Mul(span[12], 85627);
				int num13 = Mul(span[4], 85627) + Mul(span[12], 35468);
				Store(dst, reference, 0, i, num10 + num13);
				Store(dst, reference, 1, i, num11 + num12);
				Store(dst, reference, 2, i, num11 - num12);
				Store(dst, reference, 3, i, num10 - num13);
				ref Span<int> reference5 = ref span;
				span = reference5.Slice(1, reference5.Length - 1);
			}
		}
	}

	private static void InverseTransformVerticalPass(Vector128<long> in0, Vector128<long> in2, Vector128<long> in1, Vector128<long> in3, out Vector128<short> tmp0, out Vector128<short> tmp1, out Vector128<short> tmp2, out Vector128<short> tmp3)
	{
		Vector128<short> left = Sse2.Add(in0.AsInt16(), in2.AsInt16());
		Vector128<short> left2 = Sse2.Subtract(in0.AsInt16(), in2.AsInt16());
		Vector128<short> right = Vector128.Create((short)20091).AsInt16();
		Vector128<short> right2 = Vector128.Create((short)(-30068)).AsInt16();
		Vector128<short> left3 = Sse2.MultiplyHigh(in1.AsInt16(), right2);
		Vector128<short> right3 = Sse2.MultiplyHigh(in3.AsInt16(), right);
		Vector128<short> left4 = Sse2.Subtract(in1.AsInt16(), in3.AsInt16());
		Vector128<short> right4 = Sse2.Subtract(left3, right3);
		Vector128<short> right5 = Sse2.Add(left4, right4);
		Vector128<short> left5 = Sse2.MultiplyHigh(in1.AsInt16(), right);
		Vector128<short> right6 = Sse2.MultiplyHigh(in3.AsInt16(), right2);
		Vector128<short> left6 = Sse2.Add(in1.AsInt16(), in3.AsInt16());
		Vector128<short> right7 = Sse2.Add(left5, right6);
		Vector128<short> right8 = Sse2.Add(left6, right7);
		tmp0 = Sse2.Add(left, right8);
		tmp1 = Sse2.Add(left2, right5);
		tmp2 = Sse2.Subtract(left2, right5);
		tmp3 = Sse2.Subtract(left, right8);
	}

	private static void InverseTransformHorizontalPass(Vector128<long> t0, Vector128<long> t2, Vector128<long> t1, Vector128<long> t3, out Vector128<short> shifted0, out Vector128<short> shifted1, out Vector128<short> shifted2, out Vector128<short> shifted3)
	{
		Vector128<short> left = Sse2.Add(t0.AsInt16(), Vector128.Create((short)4));
		Vector128<short> left2 = Sse2.Add(left, t2.AsInt16());
		Vector128<short> left3 = Sse2.Subtract(left, t2.AsInt16());
		Vector128<short> right = Vector128.Create((short)20091).AsInt16();
		Vector128<short> right2 = Vector128.Create((short)(-30068)).AsInt16();
		Vector128<short> left4 = Sse2.MultiplyHigh(t1.AsInt16(), right2);
		Vector128<short> right3 = Sse2.MultiplyHigh(t3.AsInt16(), right);
		Vector128<short> left5 = Sse2.Subtract(t1.AsInt16(), t3.AsInt16());
		Vector128<short> right4 = Sse2.Subtract(left4, right3);
		Vector128<short> right5 = Sse2.Add(left5, right4);
		Vector128<short> left6 = Sse2.MultiplyHigh(t1.AsInt16(), right);
		Vector128<short> right6 = Sse2.MultiplyHigh(t3.AsInt16(), right2);
		Vector128<short> left7 = Sse2.Add(t1.AsInt16(), t3.AsInt16());
		Vector128<short> right7 = Sse2.Add(left6, right6);
		Vector128<short> right8 = Sse2.Add(left7, right7);
		Vector128<short> value = Sse2.Add(left2, right8);
		Vector128<short> value2 = Sse2.Add(left3, right5);
		Vector128<short> value3 = Sse2.Subtract(left3, right5);
		Vector128<short> value4 = Sse2.Subtract(left2, right8);
		shifted0 = Sse2.ShiftRightArithmetic(value, 3);
		shifted1 = Sse2.ShiftRightArithmetic(value2, 3);
		shifted2 = Sse2.ShiftRightArithmetic(value3, 3);
		shifted3 = Sse2.ShiftRightArithmetic(value4, 3);
	}

	public static void FTransform2(Span<byte> src, Span<byte> reference, Span<short> output, Span<short> output2, Span<int> scratch)
	{
		if (Sse2.IsSupported)
		{
			ref byte reference2 = ref MemoryMarshal.GetReference(src);
			ref byte reference3 = ref MemoryMarshal.GetReference(reference);
			Vector128<long> vector = Vector128.Create(Unsafe.As<byte, long>(ref reference2), 0L);
			Vector128<long> vector2 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference2, 32)), 0L);
			Vector128<long> vector3 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference2, 64)), 0L);
			Vector128<long> vector4 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference2, 96)), 0L);
			Vector128<long> vector5 = Vector128.Create(Unsafe.As<byte, long>(ref reference3), 0L);
			Vector128<long> vector6 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 32)), 0L);
			Vector128<long> vector7 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 64)), 0L);
			Vector128<long> vector8 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 96)), 0L);
			Vector128<byte> vector9 = Sse2.UnpackLow(vector.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector10 = Sse2.UnpackLow(vector2.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector11 = Sse2.UnpackLow(vector3.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector12 = Sse2.UnpackLow(vector4.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector13 = Sse2.UnpackLow(vector5.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector14 = Sse2.UnpackLow(vector6.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector15 = Sse2.UnpackLow(vector7.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector16 = Sse2.UnpackLow(vector8.AsByte(), Vector128<byte>.Zero);
			Vector128<short> vector17 = Sse2.Subtract(vector9.AsInt16(), vector13.AsInt16());
			Vector128<short> vector18 = Sse2.Subtract(vector10.AsInt16(), vector14.AsInt16());
			Vector128<short> vector19 = Sse2.Subtract(vector11.AsInt16(), vector15.AsInt16());
			Vector128<short> vector20 = Sse2.Subtract(vector12.AsInt16(), vector16.AsInt16());
			Vector128<int> vector21 = Sse2.UnpackLow(vector17.AsInt32(), vector18.AsInt32());
			Vector128<int> vector22 = Sse2.UnpackLow(vector19.AsInt32(), vector20.AsInt32());
			Vector128<int> vector23 = Sse2.UnpackHigh(vector17.AsInt32(), vector18.AsInt32());
			Vector128<int> vector24 = Sse2.UnpackHigh(vector19.AsInt32(), vector20.AsInt32());
			FTransformPass1SSE2(vector21.AsInt16(), vector22.AsInt16(), out var @out, out var out2);
			FTransformPass1SSE2(vector23.AsInt16(), vector24.AsInt16(), out var out3, out var out4);
			FTransformPass2SSE2(@out, out2, output);
			FTransformPass2SSE2(out3, out4, output2);
		}
		else
		{
			FTransform(src, reference, output, scratch);
			ref Span<byte> reference4 = ref src;
			Span<byte> src2 = reference4.Slice(4, reference4.Length - 4);
			reference4 = ref reference;
			FTransform(src2, reference4.Slice(4, reference4.Length - 4), output2, scratch);
		}
	}

	public static void FTransform(Span<byte> src, Span<byte> reference, Span<short> output, Span<int> scratch)
	{
		if (Sse2.IsSupported)
		{
			ref byte reference2 = ref MemoryMarshal.GetReference(src);
			ref byte reference3 = ref MemoryMarshal.GetReference(reference);
			Vector128<long> vector = Vector128.Create(Unsafe.As<byte, long>(ref reference2), 0L);
			Vector128<long> vector2 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference2, 32)), 0L);
			Vector128<long> vector3 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference2, 64)), 0L);
			Vector128<long> vector4 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference2, 96)), 0L);
			Vector128<long> vector5 = Vector128.Create(Unsafe.As<byte, long>(ref reference3), 0L);
			Vector128<long> vector6 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 32)), 0L);
			Vector128<long> vector7 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 64)), 0L);
			Vector128<long> vector8 = Vector128.Create(Unsafe.As<byte, long>(ref Unsafe.Add(ref reference3, 96)), 0L);
			Vector128<short> vector9 = Sse2.UnpackLow(vector.AsInt16(), vector2.AsInt16());
			Vector128<short> vector10 = Sse2.UnpackLow(vector3.AsInt16(), vector4.AsInt16());
			Vector128<short> vector11 = Sse2.UnpackLow(vector5.AsInt16(), vector6.AsInt16());
			Vector128<short> vector12 = Sse2.UnpackLow(vector7.AsInt16(), vector8.AsInt16());
			Vector128<byte> vector13 = Sse2.UnpackLow(vector9.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector14 = Sse2.UnpackLow(vector10.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector15 = Sse2.UnpackLow(vector11.AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector16 = Sse2.UnpackLow(vector12.AsByte(), Vector128<byte>.Zero);
			Vector128<short> row = Sse2.Subtract(vector13.AsInt16(), vector15.AsInt16());
			Vector128<short> row2 = Sse2.Subtract(vector14.AsInt16(), vector16.AsInt16());
			FTransformPass1SSE2(row, row2, out var @out, out var out2);
			FTransformPass2SSE2(@out, out2, output);
			return;
		}
		Span<int> span = scratch.Slice(0, 16);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			int num3 = src[num + 3] - reference[num2 + 3];
			int num4 = src[num + 2] - reference[num2 + 2];
			int num5 = src[num + 1] - reference[num2 + 1];
			int num6 = src[num] - reference[num2];
			int num7 = num6 + num3;
			int num8 = num5 + num4;
			int num9 = num5 - num4;
			int num10 = num6 - num3;
			span[3 + i * 4] = num10 * 2217 - num9 * 5352 + 937 >> 9;
			span[2 + i * 4] = (num7 - num8) * 8;
			span[1 + i * 4] = num9 * 2217 + num10 * 5352 + 1812 >> 9;
			span[i * 4] = (num7 + num8) * 8;
			num += 32;
			num2 += 32;
		}
		for (int i = 0; i < 4; i++)
		{
			int num11 = span[12 + i];
			int num12 = span[8 + i];
			int num13 = span[4 + i] + num12;
			int num14 = span[4 + i] - num12;
			int num15 = span[i] + num11;
			int num16 = span[i] - num11;
			output[12 + i] = (short)(num16 * 2217 - num14 * 5352 + 51000 >> 16);
			output[8 + i] = (short)(num15 - num13 + 7 >> 4);
			output[4 + i] = (short)((num14 * 2217 + num16 * 5352 + 12000 >> 16) + ((num16 != 0) ? 1 : 0));
			output[i] = (short)(num15 + num13 + 7 >> 4);
		}
	}

	public static void FTransformPass1SSE2(Vector128<short> row01, Vector128<short> row23, out Vector128<int> out01, out Vector128<int> out32)
	{
		Vector128<short> vector = Sse2.ShuffleHigh(row01, 177);
		Vector128<short> vector2 = Sse2.ShuffleHigh(row23, 177);
		Vector128<long> vector3 = Sse2.UnpackLow(vector.AsInt64(), vector2.AsInt64());
		Vector128<long> vector4 = Sse2.UnpackHigh(vector.AsInt64(), vector2.AsInt64());
		Vector128<short> left = Sse2.Add(vector3.AsInt16(), vector4.AsInt16());
		Vector128<short> left2 = Sse2.Subtract(vector3.AsInt16(), vector4.AsInt16());
		Vector128<int> left3 = Sse2.MultiplyAddAdjacent(left, Vector128.Create(8, 0, 8, 0, 8, 0, 8, 0, 8, 0, 8, 0, 8, 0, 8, 0).AsInt16());
		Vector128<int> right = Sse2.MultiplyAddAdjacent(left, Vector128.Create(8, 0, 248, byte.MaxValue, 8, 0, 248, byte.MaxValue, 8, 0, 248, byte.MaxValue, 8, 0, 248, byte.MaxValue).AsInt16());
		Vector128<int> left4 = Sse2.MultiplyAddAdjacent(left2, Vector128.Create(232, 20, 169, 8, 232, 20, 169, 8, 232, 20, 169, 8, 232, 20, 169, 8).AsInt16());
		Vector128<int> left5 = Sse2.MultiplyAddAdjacent(left2, Vector128.Create(169, 8, 24, 235, 169, 8, 24, 235, 169, 8, 24, 235, 169, 8, 24, 235).AsInt16());
		Vector128<int> value = Sse2.Add(left4, Vector128.Create(1812));
		Vector128<int> value2 = Sse2.Add(left5, Vector128.Create(937));
		Vector128<int> left6 = Sse2.ShiftRightArithmetic(value, 9);
		Vector128<int> right2 = Sse2.ShiftRightArithmetic(value2, 9);
		Vector128<short> left7 = Sse2.PackSignedSaturate(left3, right);
		Vector128<short> right3 = Sse2.PackSignedSaturate(left6, right2);
		Vector128<short> vector5 = Sse2.UnpackLow(left7, right3);
		Vector128<short> vector6 = Sse2.UnpackHigh(left7, right3);
		Vector128<int> value3 = Sse2.UnpackHigh(vector5.AsInt32(), vector6.AsInt32());
		out01 = Sse2.UnpackLow(vector5.AsInt32(), vector6.AsInt32());
		out32 = Sse2.Shuffle(value3, 78);
	}

	public static void FTransformPass2SSE2(Vector128<int> v01, Vector128<int> v32, Span<short> output)
	{
		Vector128<short> vector = Sse2.Subtract(v01.AsInt16(), v32.AsInt16());
		Vector128<short> left = Sse2.UnpackLow(Sse2.UnpackHigh(vector.AsInt64(), vector.AsInt64()).AsInt16(), vector.AsInt16());
		Vector128<int> left2 = Sse2.MultiplyAddAdjacent(left, Vector128.Create(169, 8, 232, 20, 169, 8, 232, 20, 169, 8, 232, 20, 169, 8, 232, 20).AsInt16());
		Vector128<int> left3 = Sse2.MultiplyAddAdjacent(left, Vector128.Create(24, 235, 169, 8, 24, 235, 169, 8, 24, 235, 169, 8, 24, 235, 169, 8).AsInt16());
		Vector128<int> value = Sse2.Add(left2, Vector128.Create(77536));
		Vector128<int> value2 = Sse2.Add(left3, Vector128.Create(51000));
		Vector128<int> vector2 = Sse2.ShiftRightArithmetic(value, 16);
		Vector128<int> vector3 = Sse2.ShiftRightArithmetic(value2, 16);
		Vector128<short> left4 = Sse2.PackSignedSaturate(vector2, vector2);
		Vector128<short> vector4 = Sse2.PackSignedSaturate(vector3, vector3);
		Vector128<short> vector5 = Sse2.Add(left4, Sse2.CompareEqual(vector, Vector128<short>.Zero));
		Vector128<short> vector6 = Sse2.Add(v01.AsInt16(), v32.AsInt16());
		Vector128<short> left5 = Sse2.Add(vector6.AsInt16(), Vector128.Create((short)7));
		Vector128<short> right = Sse2.UnpackHigh(vector6.AsInt64(), vector6.AsInt64()).AsInt16();
		Vector128<short> value3 = Sse2.Add(left5, right);
		Vector128<short> value4 = Sse2.Subtract(left5, right);
		Vector128<short> vector7 = Sse2.ShiftRightArithmetic(value3, 4);
		Vector128<short> vector8 = Sse2.ShiftRightArithmetic(value4, 4);
		Vector128<long> vector9 = Sse2.UnpackLow(vector7.AsInt64(), vector5.AsInt64());
		Vector128<long> vector10 = Sse2.UnpackLow(vector8.AsInt64(), vector4.AsInt64());
		ref short reference = ref MemoryMarshal.GetReference(output);
		Unsafe.As<short, Vector128<short>>(ref reference) = vector9.AsInt16();
		Unsafe.As<short, Vector128<short>>(ref Unsafe.Add(ref reference, 8)) = vector10.AsInt16();
	}

	public static void FTransformWht(Span<short> input, Span<short> output, Span<int> scratch)
	{
		Span<int> span = scratch.Slice(0, 16);
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			int num2 = input[num + 16] + input[num + 48];
			int num3 = input[num + 16] - input[num + 48];
			int num4 = input[num] + input[num + 32];
			int num5 = input[num] - input[num + 32];
			span[3 + i * 4] = num4 - num2;
			span[2 + i * 4] = num5 - num3;
			span[1 + i * 4] = num5 + num3;
			span[i * 4] = num4 + num2;
			num += 64;
		}
		for (int i = 0; i < 4; i++)
		{
			int num6 = span[12 + i];
			int num7 = span[8 + i];
			int num8 = span[4 + i] + num6;
			int num9 = span[4 + i] - num6;
			int num10 = span[i] + num7;
			int num11 = span[i] - num7;
			int num12 = num10 + num8;
			int num13 = num11 + num9;
			int num14 = num11 - num9;
			int num15 = num10 - num8;
			output[12 + i] = (short)(num15 >> 1);
			output[8 + i] = (short)(num14 >> 1);
			output[4 + i] = (short)(num13 >> 1);
			output[i] = (short)(num12 >> 1);
		}
	}

	public static void EncPredLuma16(Span<byte> dst, Span<byte> left, Span<byte> top)
	{
		DcMode(dst, left, top, 16, 16, 5);
		ref Span<byte> reference = ref dst;
		VerticalPred(reference.Slice(512, reference.Length - 512), top, 16);
		reference = ref dst;
		HorizontalPred(reference.Slice(528, reference.Length - 528), left, 16);
		reference = ref dst;
		TrueMotion(reference.Slice(16, reference.Length - 16), left, top, 16);
	}

	public static void EncPredChroma8(Span<byte> dst, Span<byte> left, Span<byte> top)
	{
		ref Span<byte> reference = ref dst;
		DcMode(reference.Slice(1024, reference.Length - 1024), left, top, 8, 8, 4);
		reference = ref dst;
		VerticalPred(reference.Slice(1280, reference.Length - 1280), top, 8);
		reference = ref dst;
		HorizontalPred(reference.Slice(1296, reference.Length - 1296), left, 8);
		reference = ref dst;
		TrueMotion(reference.Slice(1040, reference.Length - 1040), left, top, 8);
		reference = ref dst;
		dst = reference.Slice(8, reference.Length - 8);
		if (top != default(Span<byte>))
		{
			reference = ref top;
			top = reference.Slice(8, reference.Length - 8);
		}
		if (left != default(Span<byte>))
		{
			reference = ref left;
			left = reference.Slice(16, reference.Length - 16);
		}
		reference = ref dst;
		DcMode(reference.Slice(1024, reference.Length - 1024), left, top, 8, 8, 4);
		reference = ref dst;
		VerticalPred(reference.Slice(1280, reference.Length - 1280), top, 8);
		reference = ref dst;
		HorizontalPred(reference.Slice(1296, reference.Length - 1296), left, 8);
		reference = ref dst;
		TrueMotion(reference.Slice(1040, reference.Length - 1040), left, top, 8);
	}

	public static void EncPredLuma4(Span<byte> dst, Span<byte> top, int topOffset, Span<byte> vals)
	{
		ref Span<byte> reference = ref dst;
		Dc4(reference.Slice(1536, reference.Length - 1536), top, topOffset);
		reference = ref dst;
		Tm4(reference.Slice(1540, reference.Length - 1540), top, topOffset);
		reference = ref dst;
		Ve4(reference.Slice(1544, reference.Length - 1544), top, topOffset, vals);
		reference = ref dst;
		He4(reference.Slice(1548, reference.Length - 1548), top, topOffset);
		reference = ref dst;
		Rd4(reference.Slice(1552, reference.Length - 1552), top, topOffset);
		reference = ref dst;
		Vr4(reference.Slice(1556, reference.Length - 1556), top, topOffset);
		reference = ref dst;
		Ld4(reference.Slice(1560, reference.Length - 1560), top, topOffset);
		reference = ref dst;
		Vl4(reference.Slice(1564, reference.Length - 1564), top, topOffset);
		reference = ref dst;
		Hd4(reference.Slice(1664, reference.Length - 1664), top, topOffset);
		reference = ref dst;
		Hu4(reference.Slice(1668, reference.Length - 1668), top, topOffset);
	}

	private static void VerticalPred(Span<byte> dst, Span<byte> top, int size)
	{
		if (top != default(Span<byte>))
		{
			for (int i = 0; i < size; i++)
			{
				Span<byte> span = top.Slice(0, size);
				int num = i * 32;
				span.CopyTo(dst.Slice(num, dst.Length - num));
			}
		}
		else
		{
			Fill(dst, 127, size);
		}
	}

	public static void HorizontalPred(Span<byte> dst, Span<byte> left, int size)
	{
		if (left != default(Span<byte>))
		{
			left = left.Slice(1, left.Length - 1);
			for (int i = 0; i < size; i++)
			{
				dst.Slice(i * 32, size).Fill(left[i]);
			}
		}
		else
		{
			Fill(dst, 129, size);
		}
	}

	public static void TrueMotion(Span<byte> dst, Span<byte> left, Span<byte> top, int size)
	{
		if (left != default(Span<byte>))
		{
			if (top != default(Span<byte>))
			{
				Span<byte> span = Clip1.AsSpan(255 - left[0]);
				for (int i = 0; i < size; i++)
				{
					ref Span<byte> reference = ref span;
					int num = left[i + 1];
					Span<byte> span2 = reference.Slice(num, reference.Length - num);
					for (int j = 0; j < size; j++)
					{
						dst[j] = span2[top[j]];
					}
					reference = ref dst;
					dst = reference.Slice(32, reference.Length - 32);
				}
			}
			else
			{
				HorizontalPred(dst, left, size);
			}
		}
		else if (top != default(Span<byte>))
		{
			VerticalPred(dst, top, size);
		}
		else
		{
			Fill(dst, 129, size);
		}
	}

	private static void DcMode(Span<byte> dst, Span<byte> left, Span<byte> top, int size, int round, int shift)
	{
		int num = 0;
		if (top != default(Span<byte>))
		{
			for (int i = 0; i < size; i++)
			{
				num += top[i];
			}
			if (left != default(Span<byte>))
			{
				ref Span<byte> reference = ref left;
				left = reference.Slice(1, reference.Length - 1);
				for (int i = 0; i < size; i++)
				{
					num += left[i];
				}
			}
			else
			{
				num += num;
			}
			num = num + round >> shift;
		}
		else if (left != default(Span<byte>))
		{
			ref Span<byte> reference = ref left;
			left = reference.Slice(1, reference.Length - 1);
			for (int i = 0; i < size; i++)
			{
				num += left[i];
			}
			num += num;
			num = num + round >> shift;
		}
		else
		{
			num = 128;
		}
		Fill(dst, num, size);
	}

	private static void Dc4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		uint num = 4u;
		for (int i = 0; i < 4; i++)
		{
			num += (uint)(top[topOffset + i] + top[topOffset - 5 + i]);
		}
		Fill(dst, (int)(num >> 3), 4);
	}

	private static void Tm4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		Span<byte> span = Clip1.AsSpan(255 - top[topOffset - 1]);
		for (int i = 0; i < 4; i++)
		{
			ref Span<byte> reference = ref span;
			int num = top[topOffset - 2 - i];
			Span<byte> span2 = reference.Slice(num, reference.Length - num);
			for (int j = 0; j < 4; j++)
			{
				dst[j] = span2[top[topOffset + j]];
			}
			reference = ref dst;
			dst = reference.Slice(32, reference.Length - 32);
		}
	}

	private static void Ve4(Span<byte> dst, Span<byte> top, int topOffset, Span<byte> vals)
	{
		vals[0] = LossyUtils.Avg3(top[topOffset - 1], top[topOffset], top[topOffset + 1]);
		vals[1] = LossyUtils.Avg3(top[topOffset], top[topOffset + 1], top[topOffset + 2]);
		vals[2] = LossyUtils.Avg3(top[topOffset + 1], top[topOffset + 2], top[topOffset + 3]);
		vals[3] = LossyUtils.Avg3(top[topOffset + 2], top[topOffset + 3], top[topOffset + 4]);
		for (int i = 0; i < 4; i++)
		{
			int num = i * 32;
			vals.CopyTo(dst.Slice(num, dst.Length - num));
		}
	}

	private static void He4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		byte a = top[topOffset - 1];
		byte b = top[topOffset - 2];
		byte b2 = top[topOffset - 3];
		byte b3 = top[topOffset - 4];
		byte b4 = top[topOffset - 5];
		uint value = (uint)(16843009 * LossyUtils.Avg3(a, b, b2));
		BinaryPrimitives.WriteUInt32BigEndian(dst, value);
		value = (uint)(16843009 * LossyUtils.Avg3(b, b2, b3));
		ref Span<byte> reference = ref dst;
		BinaryPrimitives.WriteUInt32BigEndian(reference.Slice(32, reference.Length - 32), value);
		value = (uint)(16843009 * LossyUtils.Avg3(b2, b3, b4));
		reference = ref dst;
		BinaryPrimitives.WriteUInt32BigEndian(reference.Slice(64, reference.Length - 64), value);
		value = (uint)(16843009 * LossyUtils.Avg3(b3, b4, b4));
		reference = ref dst;
		BinaryPrimitives.WriteUInt32BigEndian(reference.Slice(96, reference.Length - 96), value);
	}

	private static void Rd4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		byte b = top[topOffset - 1];
		byte b2 = top[topOffset - 2];
		byte b3 = top[topOffset - 3];
		byte b4 = top[topOffset - 4];
		byte c = top[topOffset - 5];
		byte b5 = top[topOffset];
		byte b6 = top[topOffset + 1];
		byte b7 = top[topOffset + 2];
		byte a = top[topOffset + 3];
		LossyUtils.Dst(dst, 0, 3, LossyUtils.Avg3(b3, b4, c));
		byte v = LossyUtils.Avg3(b2, b3, b4);
		LossyUtils.Dst(dst, 0, 2, v);
		LossyUtils.Dst(dst, 1, 3, v);
		byte v2 = LossyUtils.Avg3(b, b2, b3);
		LossyUtils.Dst(dst, 0, 1, v2);
		LossyUtils.Dst(dst, 1, 2, v2);
		LossyUtils.Dst(dst, 2, 3, v2);
		byte v3 = LossyUtils.Avg3(b5, b, b2);
		LossyUtils.Dst(dst, 0, 0, v3);
		LossyUtils.Dst(dst, 1, 1, v3);
		LossyUtils.Dst(dst, 2, 2, v3);
		LossyUtils.Dst(dst, 3, 3, v3);
		byte v4 = LossyUtils.Avg3(b6, b5, b);
		LossyUtils.Dst(dst, 1, 0, v4);
		LossyUtils.Dst(dst, 2, 1, v4);
		LossyUtils.Dst(dst, 3, 2, v4);
		byte v5 = LossyUtils.Avg3(b7, b6, b5);
		LossyUtils.Dst(dst, 2, 0, v5);
		LossyUtils.Dst(dst, 3, 1, v5);
		LossyUtils.Dst(dst, 3, 0, LossyUtils.Avg3(a, b7, b6));
	}

	private static void Vr4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		byte b = top[topOffset - 1];
		byte b2 = top[topOffset - 2];
		byte b3 = top[topOffset - 3];
		byte a = top[topOffset - 4];
		byte b4 = top[topOffset];
		byte b5 = top[topOffset + 1];
		byte b6 = top[topOffset + 2];
		byte b7 = top[topOffset + 3];
		byte v = LossyUtils.Avg2(b, b4);
		LossyUtils.Dst(dst, 0, 0, v);
		LossyUtils.Dst(dst, 1, 2, v);
		byte v2 = LossyUtils.Avg2(b4, b5);
		LossyUtils.Dst(dst, 1, 0, v2);
		LossyUtils.Dst(dst, 2, 2, v2);
		byte v3 = LossyUtils.Avg2(b5, b6);
		LossyUtils.Dst(dst, 2, 0, v3);
		LossyUtils.Dst(dst, 3, 2, v3);
		LossyUtils.Dst(dst, 3, 0, LossyUtils.Avg2(b6, b7));
		LossyUtils.Dst(dst, 0, 3, LossyUtils.Avg3(a, b3, b2));
		LossyUtils.Dst(dst, 0, 2, LossyUtils.Avg3(b3, b2, b));
		byte v4 = LossyUtils.Avg3(b2, b, b4);
		LossyUtils.Dst(dst, 0, 1, v4);
		LossyUtils.Dst(dst, 1, 3, v4);
		byte v5 = LossyUtils.Avg3(b, b4, b5);
		LossyUtils.Dst(dst, 1, 1, v5);
		LossyUtils.Dst(dst, 2, 3, v5);
		byte v6 = LossyUtils.Avg3(b4, b5, b6);
		LossyUtils.Dst(dst, 2, 1, v6);
		LossyUtils.Dst(dst, 3, 3, v6);
		LossyUtils.Dst(dst, 3, 1, LossyUtils.Avg3(b5, b6, b7));
	}

	private static void Ld4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		byte a = top[topOffset];
		byte b = top[topOffset + 1];
		byte b2 = top[topOffset + 2];
		byte b3 = top[topOffset + 3];
		byte b4 = top[topOffset + 4];
		byte b5 = top[topOffset + 5];
		byte b6 = top[topOffset + 6];
		byte b7 = top[topOffset + 7];
		LossyUtils.Dst(dst, 0, 0, LossyUtils.Avg3(a, b, b2));
		byte v = LossyUtils.Avg3(b, b2, b3);
		LossyUtils.Dst(dst, 1, 0, v);
		LossyUtils.Dst(dst, 0, 1, v);
		byte v2 = LossyUtils.Avg3(b2, b3, b4);
		LossyUtils.Dst(dst, 2, 0, v2);
		LossyUtils.Dst(dst, 1, 1, v2);
		LossyUtils.Dst(dst, 0, 2, v2);
		byte v3 = LossyUtils.Avg3(b3, b4, b5);
		LossyUtils.Dst(dst, 3, 0, v3);
		LossyUtils.Dst(dst, 2, 1, v3);
		LossyUtils.Dst(dst, 1, 2, v3);
		LossyUtils.Dst(dst, 0, 3, v3);
		byte v4 = LossyUtils.Avg3(b4, b5, b6);
		LossyUtils.Dst(dst, 3, 1, v4);
		LossyUtils.Dst(dst, 2, 2, v4);
		LossyUtils.Dst(dst, 1, 3, v4);
		byte v5 = LossyUtils.Avg3(b5, b6, b7);
		LossyUtils.Dst(dst, 3, 2, v5);
		LossyUtils.Dst(dst, 2, 3, v5);
		LossyUtils.Dst(dst, 3, 3, LossyUtils.Avg3(b6, b7, b7));
	}

	private static void Vl4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		byte a = top[topOffset];
		byte b = top[topOffset + 1];
		byte b2 = top[topOffset + 2];
		byte b3 = top[topOffset + 3];
		byte b4 = top[topOffset + 4];
		byte b5 = top[topOffset + 5];
		byte b6 = top[topOffset + 6];
		byte c = top[topOffset + 7];
		LossyUtils.Dst(dst, 0, 0, LossyUtils.Avg2(a, b));
		byte v = LossyUtils.Avg2(b, b2);
		LossyUtils.Dst(dst, 1, 0, v);
		LossyUtils.Dst(dst, 0, 2, v);
		byte v2 = LossyUtils.Avg2(b2, b3);
		LossyUtils.Dst(dst, 2, 0, v2);
		LossyUtils.Dst(dst, 1, 2, v2);
		byte v3 = LossyUtils.Avg2(b3, b4);
		LossyUtils.Dst(dst, 3, 0, v3);
		LossyUtils.Dst(dst, 2, 2, v3);
		LossyUtils.Dst(dst, 0, 1, LossyUtils.Avg3(a, b, b2));
		byte v4 = LossyUtils.Avg3(b, b2, b3);
		LossyUtils.Dst(dst, 1, 1, v4);
		LossyUtils.Dst(dst, 0, 3, v4);
		byte v5 = LossyUtils.Avg3(b2, b3, b4);
		LossyUtils.Dst(dst, 2, 1, v5);
		LossyUtils.Dst(dst, 1, 3, v5);
		byte v6 = LossyUtils.Avg3(b3, b4, b5);
		LossyUtils.Dst(dst, 3, 1, v6);
		LossyUtils.Dst(dst, 2, 3, v6);
		LossyUtils.Dst(dst, 3, 2, LossyUtils.Avg3(b4, b5, b6));
		LossyUtils.Dst(dst, 3, 3, LossyUtils.Avg3(b5, b6, c));
	}

	private static void Hd4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		byte b = top[topOffset - 1];
		byte b2 = top[topOffset - 2];
		byte b3 = top[topOffset - 3];
		byte b4 = top[topOffset - 4];
		byte a = top[topOffset - 5];
		byte b5 = top[topOffset];
		byte b6 = top[topOffset + 1];
		byte c = top[topOffset + 2];
		byte v = LossyUtils.Avg2(b2, b);
		LossyUtils.Dst(dst, 0, 0, v);
		LossyUtils.Dst(dst, 2, 1, v);
		byte v2 = LossyUtils.Avg2(b3, b2);
		LossyUtils.Dst(dst, 0, 1, v2);
		LossyUtils.Dst(dst, 2, 2, v2);
		byte v3 = LossyUtils.Avg2(b4, b3);
		LossyUtils.Dst(dst, 0, 2, v3);
		LossyUtils.Dst(dst, 2, 3, v3);
		LossyUtils.Dst(dst, 0, 3, LossyUtils.Avg2(a, b4));
		LossyUtils.Dst(dst, 3, 0, LossyUtils.Avg3(b5, b6, c));
		LossyUtils.Dst(dst, 2, 0, LossyUtils.Avg3(b, b5, b6));
		byte v4 = LossyUtils.Avg3(b2, b, b5);
		LossyUtils.Dst(dst, 1, 0, v4);
		LossyUtils.Dst(dst, 3, 1, v4);
		byte v5 = LossyUtils.Avg3(b3, b2, b);
		LossyUtils.Dst(dst, 1, 1, v5);
		LossyUtils.Dst(dst, 3, 2, v5);
		byte v6 = LossyUtils.Avg3(b4, b3, b2);
		LossyUtils.Dst(dst, 1, 2, v6);
		LossyUtils.Dst(dst, 3, 3, v6);
		LossyUtils.Dst(dst, 1, 3, LossyUtils.Avg3(a, b4, b3));
	}

	private static void Hu4(Span<byte> dst, Span<byte> top, int topOffset)
	{
		byte a = top[topOffset - 2];
		byte b = top[topOffset - 3];
		byte b2 = top[topOffset - 4];
		byte b3 = top[topOffset - 5];
		LossyUtils.Dst(dst, 0, 0, LossyUtils.Avg2(a, b));
		byte v = LossyUtils.Avg2(b, b2);
		LossyUtils.Dst(dst, 2, 0, v);
		LossyUtils.Dst(dst, 0, 1, v);
		byte v2 = LossyUtils.Avg2(b2, b3);
		LossyUtils.Dst(dst, 2, 1, v2);
		LossyUtils.Dst(dst, 0, 2, v2);
		LossyUtils.Dst(dst, 1, 0, LossyUtils.Avg3(a, b, b2));
		byte v3 = LossyUtils.Avg3(b, b2, b3);
		LossyUtils.Dst(dst, 3, 0, v3);
		LossyUtils.Dst(dst, 1, 1, v3);
		byte v4 = LossyUtils.Avg3(b2, b3, b3);
		LossyUtils.Dst(dst, 3, 1, v4);
		LossyUtils.Dst(dst, 1, 2, v4);
		LossyUtils.Dst(dst, 3, 2, b3);
		LossyUtils.Dst(dst, 2, 2, b3);
		LossyUtils.Dst(dst, 0, 3, b3);
		LossyUtils.Dst(dst, 1, 3, b3);
		LossyUtils.Dst(dst, 2, 3, b3);
		LossyUtils.Dst(dst, 3, 3, b3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Fill(Span<byte> dst, int value, int size)
	{
		for (int i = 0; i < size; i++)
		{
			dst.Slice(i * 32, size).Fill((byte)value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte Clip8b(int v)
	{
		if ((v & -256) != 0)
		{
			if (v >= 0)
			{
				return byte.MaxValue;
			}
			return 0;
		}
		return (byte)v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Store(Span<byte> dst, Span<byte> reference, int x, int y, int v)
	{
		dst[x + y * 32] = LossyUtils.Clip8B(reference[x + y * 32] + (v >> 3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Mul(int a, int b)
	{
		return a * b >> 16;
	}
}
