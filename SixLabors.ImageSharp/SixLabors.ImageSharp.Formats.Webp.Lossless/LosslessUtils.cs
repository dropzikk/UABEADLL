using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal static class LosslessUtils
{
	private const int PrefixLookupIdxMax = 512;

	private const int LogLookupIdxMax = 256;

	private const int ApproxLogMax = 4096;

	private const int ApproxLogWithCorrectionMax = 65536;

	private const double Log2Reciprocal = 1.4426950408889634;

	public static int FindMatchLength(ReadOnlySpan<uint> array1, ReadOnlySpan<uint> array2, int bestLenMatch, int maxLimit)
	{
		if (array1[bestLenMatch] != array2[bestLenMatch])
		{
			return 0;
		}
		return VectorMismatch(array1, array2, maxLimit);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int VectorMismatch(ReadOnlySpan<uint> array1, ReadOnlySpan<uint> array2, int length)
	{
		int i = 0;
		ref uint reference = ref MemoryMarshal.GetReference(array1);
		for (ref uint reference2 = ref MemoryMarshal.GetReference(array2); i < length && Unsafe.Add(ref reference, (uint)i) == Unsafe.Add(ref reference2, (uint)i); i++)
		{
		}
		return i;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int MaxFindCopyLength(int len)
	{
		if (len >= 4095)
		{
			return 4095;
		}
		return len;
	}

	public static int PrefixEncodeBits(int distance, ref int extraBits)
	{
		if (distance < 512)
		{
			(int Code, int ExtraBits) tuple = WebpLookupTables.PrefixEncodeCode[distance];
			int item = tuple.Code;
			int item2 = tuple.ExtraBits;
			extraBits = item2;
			return item;
		}
		return PrefixEncodeBitsNoLut(distance, ref extraBits);
	}

	public static int PrefixEncode(int distance, ref int extraBits, ref int extraBitsValue)
	{
		if (distance < 512)
		{
			(int Code, int ExtraBits) tuple = WebpLookupTables.PrefixEncodeCode[distance];
			int item = tuple.Code;
			int item2 = tuple.ExtraBits;
			extraBits = item2;
			extraBitsValue = WebpLookupTables.PrefixEncodeExtraBitsValue[distance];
			return item;
		}
		return PrefixEncodeNoLut(distance, ref extraBits, ref extraBitsValue);
	}

	public static void AddGreenToBlueAndRed(Span<uint> pixelData)
	{
		if (Avx2.IsSupported && pixelData.Length >= 8)
		{
			Vector256<byte> mask = Vector256.Create(1, byte.MaxValue, 1, byte.MaxValue, 5, byte.MaxValue, 5, byte.MaxValue, 9, byte.MaxValue, 9, byte.MaxValue, 13, byte.MaxValue, 13, byte.MaxValue, 17, byte.MaxValue, 17, byte.MaxValue, 21, byte.MaxValue, 21, byte.MaxValue, 25, byte.MaxValue, 25, byte.MaxValue, 29, byte.MaxValue, 29, byte.MaxValue);
			nuint num = (uint)pixelData.Length;
			nuint num2 = 0u;
			do
			{
				ref uint source = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num2);
				Vector256<byte> vector = Unsafe.As<uint, Vector256<uint>>(ref source).AsByte();
				Vector256<byte> right = Avx2.Shuffle(vector, mask);
				Vector256<byte> vector2 = Avx2.Add(vector, right);
				Unsafe.As<uint, Vector256<uint>>(ref source) = vector2.AsUInt32();
				num2 += 8;
			}
			while (num2 <= num - 8);
			if (num2 != num)
			{
				ref Span<uint> reference = ref pixelData;
				int num3 = (int)num2;
				AddGreenToBlueAndRedScalar(reference.Slice(num3, reference.Length - num3));
			}
		}
		else if (Ssse3.IsSupported && pixelData.Length >= 4)
		{
			Vector128<byte> mask2 = Vector128.Create(1, byte.MaxValue, 1, byte.MaxValue, 5, byte.MaxValue, 5, byte.MaxValue, 9, byte.MaxValue, 9, byte.MaxValue, 13, byte.MaxValue, 13, byte.MaxValue);
			nuint num4 = (uint)pixelData.Length;
			nuint num5 = 0u;
			do
			{
				ref uint source2 = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num5);
				Vector128<byte> vector3 = Unsafe.As<uint, Vector128<uint>>(ref source2).AsByte();
				Vector128<byte> right2 = Ssse3.Shuffle(vector3, mask2);
				Vector128<byte> vector4 = Sse2.Add(vector3, right2);
				Unsafe.As<uint, Vector128<uint>>(ref source2) = vector4.AsUInt32();
				num5 += 4;
			}
			while (num5 <= num4 - 4);
			if (num5 != num4)
			{
				ref Span<uint> reference = ref pixelData;
				int num3 = (int)num5;
				AddGreenToBlueAndRedScalar(reference.Slice(num3, reference.Length - num3));
			}
		}
		else if (Sse2.IsSupported && pixelData.Length >= 4)
		{
			nuint num6 = (uint)pixelData.Length;
			nuint num7 = 0u;
			do
			{
				ref uint source3 = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num7);
				Vector128<byte> vector5 = Unsafe.As<uint, Vector128<uint>>(ref source3).AsByte();
				Vector128<byte> vector6 = Sse2.Add(right: Sse2.ShuffleHigh(Sse2.ShuffleLow(Sse2.ShiftRightLogical(vector5.AsUInt16(), 8), 160), 160).AsByte(), left: vector5.AsByte());
				Unsafe.As<uint, Vector128<uint>>(ref source3) = vector6.AsUInt32();
				num7 += 4;
			}
			while (num7 <= num6 - 4);
			if (num7 != num6)
			{
				ref Span<uint> reference = ref pixelData;
				int num3 = (int)num7;
				AddGreenToBlueAndRedScalar(reference.Slice(num3, reference.Length - num3));
			}
		}
		else
		{
			AddGreenToBlueAndRedScalar(pixelData);
		}
	}

	private static void AddGreenToBlueAndRedScalar(Span<uint> pixelData)
	{
		int length = pixelData.Length;
		for (int i = 0; i < length; i++)
		{
			uint num = pixelData[i];
			uint num2 = (num >> 8) & 0xFF;
			uint num3 = num & 0xFF00FF;
			num3 += (num2 << 16) | num2;
			num3 &= 0xFF00FF;
			pixelData[i] = (num & 0xFF00FF00u) | num3;
		}
	}

	public static void SubtractGreenFromBlueAndRed(Span<uint> pixelData)
	{
		if (Avx2.IsSupported && pixelData.Length >= 8)
		{
			Vector256<byte> mask = Vector256.Create(1, byte.MaxValue, 1, byte.MaxValue, 5, byte.MaxValue, 5, byte.MaxValue, 9, byte.MaxValue, 9, byte.MaxValue, 13, byte.MaxValue, 13, byte.MaxValue, 17, byte.MaxValue, 17, byte.MaxValue, 21, byte.MaxValue, 21, byte.MaxValue, 25, byte.MaxValue, 25, byte.MaxValue, 29, byte.MaxValue, 29, byte.MaxValue);
			nuint num = (uint)pixelData.Length;
			nuint num2 = 0u;
			do
			{
				ref uint source = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num2);
				Vector256<byte> vector = Unsafe.As<uint, Vector256<uint>>(ref source).AsByte();
				Vector256<byte> right = Avx2.Shuffle(vector, mask);
				Vector256<byte> vector2 = Avx2.Subtract(vector, right);
				Unsafe.As<uint, Vector256<uint>>(ref source) = vector2.AsUInt32();
				num2 += 8;
			}
			while (num2 <= num - 8);
			if (num2 != num)
			{
				ref Span<uint> reference = ref pixelData;
				int num3 = (int)num2;
				SubtractGreenFromBlueAndRedScalar(reference.Slice(num3, reference.Length - num3));
			}
		}
		else if (Ssse3.IsSupported && pixelData.Length >= 4)
		{
			Vector128<byte> mask2 = Vector128.Create(1, byte.MaxValue, 1, byte.MaxValue, 5, byte.MaxValue, 5, byte.MaxValue, 9, byte.MaxValue, 9, byte.MaxValue, 13, byte.MaxValue, 13, byte.MaxValue);
			nuint num4 = (uint)pixelData.Length;
			nuint num5 = 0u;
			do
			{
				ref uint source2 = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num5);
				Vector128<byte> vector3 = Unsafe.As<uint, Vector128<uint>>(ref source2).AsByte();
				Vector128<byte> right2 = Ssse3.Shuffle(vector3, mask2);
				Vector128<byte> vector4 = Sse2.Subtract(vector3, right2);
				Unsafe.As<uint, Vector128<uint>>(ref source2) = vector4.AsUInt32();
				num5 += 4;
			}
			while (num5 <= num4 - 4);
			if (num5 != num4)
			{
				ref Span<uint> reference = ref pixelData;
				int num3 = (int)num5;
				SubtractGreenFromBlueAndRedScalar(reference.Slice(num3, reference.Length - num3));
			}
		}
		else if (Sse2.IsSupported && pixelData.Length >= 4)
		{
			nuint num6 = (uint)pixelData.Length;
			nuint num7 = 0u;
			do
			{
				ref uint source3 = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num7);
				Vector128<byte> vector5 = Unsafe.As<uint, Vector128<uint>>(ref source3).AsByte();
				Vector128<byte> vector6 = Sse2.Subtract(right: Sse2.ShuffleHigh(Sse2.ShuffleLow(Sse2.ShiftRightLogical(vector5.AsUInt16(), 8), 160), 160).AsByte(), left: vector5.AsByte());
				Unsafe.As<uint, Vector128<uint>>(ref source3) = vector6.AsUInt32();
				num7 += 4;
			}
			while (num7 <= num6 - 4);
			if (num7 != num6)
			{
				ref Span<uint> reference = ref pixelData;
				int num3 = (int)num7;
				SubtractGreenFromBlueAndRedScalar(reference.Slice(num3, reference.Length - num3));
			}
		}
		else
		{
			SubtractGreenFromBlueAndRedScalar(pixelData);
		}
	}

	private static void SubtractGreenFromBlueAndRedScalar(Span<uint> pixelData)
	{
		int length = pixelData.Length;
		for (int i = 0; i < length; i++)
		{
			uint num = pixelData[i];
			uint num2 = (num >> 8) & 0xFF;
			uint num3 = (((num >> 16) & 0xFF) - num2) & 0xFF;
			uint num4 = ((num & 0xFF) - num2) & 0xFF;
			pixelData[i] = (num & 0xFF00FF00u) | (num3 << 16) | num4;
		}
	}

	public static void ColorIndexInverseTransform(Vp8LTransform transform, Span<uint> pixelData)
	{
		int num = 8 >> transform.Bits;
		int xSize = transform.XSize;
		int ySize = transform.YSize;
		Span<uint> span = transform.Data.GetSpan();
		int num2 = 0;
		if (num < 8)
		{
			int num3 = (1 << transform.Bits) - 1;
			int num4 = (1 << num) - 1;
			uint[] array = new uint[xSize * ySize];
			int num5 = 0;
			for (int i = 0; i < ySize; i++)
			{
				uint num6 = 0u;
				for (int j = 0; j < xSize; j++)
				{
					if ((j & num3) == 0)
					{
						num6 = GetArgbIndex(pixelData[num5++]);
					}
					array[num2++] = span[(int)(num6 & num4)];
					num6 >>= num;
				}
			}
			array.AsSpan().CopyTo(pixelData);
			return;
		}
		for (int k = 0; k < ySize; k++)
		{
			for (int l = 0; l < xSize; l++)
			{
				uint argbIndex = GetArgbIndex(pixelData[num2]);
				pixelData[num2] = span[(int)argbIndex];
				num2++;
			}
		}
	}

	public static void ColorSpaceInverseTransform(Vp8LTransform transform, Span<uint> pixelData)
	{
		int xSize = transform.XSize;
		int ySize = transform.YSize;
		int num = 1 << transform.Bits;
		int num2 = num - 1;
		int num3 = xSize & ~num2;
		int num4 = xSize - num3;
		int num5 = SubSampleSize(xSize, transform.Bits);
		int num6 = 0;
		int num7 = (num6 >> transform.Bits) * num5;
		Span<uint> span = transform.Data.GetSpan();
		int i = 0;
		while (num6 < ySize)
		{
			int index = num7;
			Vp8LMultipliers m = default(Vp8LMultipliers);
			int num8 = i + num3;
			int num9 = i + xSize;
			for (; i < num8; i += num)
			{
				ColorCodeToMultipliers(span[index++], ref m);
				TransformColorInverse(m, pixelData.Slice(i, num));
			}
			if (i < num9)
			{
				ColorCodeToMultipliers(span[index], ref m);
				TransformColorInverse(m, pixelData.Slice(i, num4));
				i += num4;
			}
			num6++;
			if ((num6 & num2) == 0)
			{
				num7 += num5;
			}
		}
	}

	public static void TransformColor(Vp8LMultipliers m, Span<uint> pixelData, int numPixels)
	{
		if (Avx2.IsSupported && numPixels >= 8)
		{
			Vector256<byte> right = Vector256.Create(0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue);
			Vector256<byte> right2 = Vector256.Create(byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0);
			Vector256<int> vector = MkCst32(Cst5b(m.GreenToRed), Cst5b(m.GreenToBlue));
			Vector256<int> vector2 = MkCst32(Cst5b(m.RedToBlue), 0);
			nuint num = 0u;
			do
			{
				ref uint source = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num);
				Vector256<uint> vector3 = Unsafe.As<uint, Vector256<uint>>(ref source);
				Vector256<byte> vector4 = Avx2.Subtract(right: Avx2.And(Avx2.Add(right: Avx2.MultiplyHigh(Avx2.ShuffleHigh(Avx2.ShuffleLow(Avx2.And(vector3.AsByte(), right).AsInt16(), 160).AsInt16(), 160).AsInt16(), vector.AsInt16()).AsByte(), left: Avx2.ShiftRightLogical(Avx2.MultiplyHigh(Avx2.ShiftLeftLogical(vector3.AsInt16(), 8).AsInt16(), vector2.AsInt16()).AsInt32(), 16).AsByte()), right2), left: vector3.AsByte());
				Unsafe.As<uint, Vector256<uint>>(ref source) = vector4.AsUInt32();
				num += 8;
			}
			while (num <= (uint)(numPixels - 8));
			if (num != (uint)numPixels)
			{
				Vp8LMultipliers m2 = m;
				ref Span<uint> reference = ref pixelData;
				int num2 = (int)num;
				TransformColorScalar(m2, reference.Slice(num2, reference.Length - num2), numPixels - (int)num);
			}
		}
		else if (Sse2.IsSupported && numPixels >= 4)
		{
			Vector128<byte> right5 = Vector128.Create(0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue);
			Vector128<byte> right6 = Vector128.Create(byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0);
			Vector128<int> vector5 = MkCst16(Cst5b(m.GreenToRed), Cst5b(m.GreenToBlue));
			Vector128<int> vector6 = MkCst16(Cst5b(m.RedToBlue), 0);
			nuint num3 = 0u;
			do
			{
				ref uint source2 = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num3);
				Vector128<uint> vector7 = Unsafe.As<uint, Vector128<uint>>(ref source2);
				Vector128<byte> vector8 = Sse2.Subtract(right: Sse2.And(Sse2.Add(right: Sse2.MultiplyHigh(Sse2.ShuffleHigh(Sse2.ShuffleLow(Sse2.And(vector7.AsByte(), right5).AsInt16(), 160).AsInt16(), 160).AsInt16(), vector5.AsInt16()).AsByte(), left: Sse2.ShiftRightLogical(Sse2.MultiplyHigh(Sse2.ShiftLeftLogical(vector7.AsInt16(), 8).AsInt16(), vector6.AsInt16()).AsInt32(), 16).AsByte()), right6), left: vector7.AsByte());
				Unsafe.As<uint, Vector128<uint>>(ref source2) = vector8.AsUInt32();
				num3 += 4;
			}
			while ((int)num3 <= numPixels - 4);
			if ((int)num3 != numPixels)
			{
				Vp8LMultipliers m3 = m;
				ref Span<uint> reference = ref pixelData;
				int num2 = (int)num3;
				TransformColorScalar(m3, reference.Slice(num2, reference.Length - num2), numPixels - (int)num3);
			}
		}
		else
		{
			TransformColorScalar(m, pixelData, numPixels);
		}
	}

	private static void TransformColorScalar(Vp8LMultipliers m, Span<uint> data, int numPixels)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint num = data[i];
			sbyte color = U32ToS8(num >> 8);
			sbyte b = U32ToS8(num >> 16);
			int num2 = b & 0xFF;
			int num3 = (int)(num & 0xFF);
			num2 -= ColorTransformDelta((sbyte)m.GreenToRed, color);
			num2 &= 0xFF;
			num3 -= ColorTransformDelta((sbyte)m.GreenToBlue, color);
			num3 -= ColorTransformDelta((sbyte)m.RedToBlue, b);
			num3 &= 0xFF;
			data[i] = (num & 0xFF00FF00u) | (uint)(num2 << 16) | (uint)num3;
		}
	}

	public static void TransformColorInverse(Vp8LMultipliers m, Span<uint> pixelData)
	{
		if (Avx2.IsSupported && pixelData.Length >= 8)
		{
			Vector256<byte> right = Vector256.Create(0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue);
			Vector256<int> vector = MkCst32(Cst5b(m.GreenToRed), Cst5b(m.GreenToBlue));
			Vector256<int> vector2 = MkCst32(Cst5b(m.RedToBlue), 0);
			nuint num;
			for (num = 0u; num <= (uint)(pixelData.Length - 8); num += 8)
			{
				ref uint source = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num);
				Vector256<uint> vector3 = Unsafe.As<uint, Vector256<uint>>(ref source);
				Vector256<byte> vector4 = Avx2.And(vector3.AsByte(), right);
				Vector256<short> vector5 = Avx2.ShiftLeftLogical(Avx2.Add(right: Avx2.MultiplyHigh(Avx2.ShuffleHigh(Avx2.ShuffleLow(vector4.AsInt16(), 160).AsInt16(), 160).AsInt16(), vector.AsInt16()).AsByte(), left: vector3.AsByte()).AsInt16(), 8);
				Vector256<byte> vector6 = Avx2.Or(Avx2.ShiftRightLogical(Avx2.Add(Avx2.ShiftRightLogical(Avx2.MultiplyHigh(vector5, vector2.AsInt16()).AsInt32(), 8).AsByte(), vector5.AsByte()).AsInt16(), 8).AsByte(), vector4);
				Unsafe.As<uint, Vector256<uint>>(ref source) = vector6.AsUInt32();
			}
			if (num != (uint)pixelData.Length)
			{
				Vp8LMultipliers m2 = m;
				ref Span<uint> reference = ref pixelData;
				int num2 = (int)num;
				TransformColorInverseScalar(m2, reference.Slice(num2, reference.Length - num2));
			}
		}
		else if (Sse2.IsSupported && pixelData.Length >= 4)
		{
			Vector128<byte> right3 = Vector128.Create(0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue, 0, byte.MaxValue);
			Vector128<int> vector7 = MkCst16(Cst5b(m.GreenToRed), Cst5b(m.GreenToBlue));
			Vector128<int> vector8 = MkCst16(Cst5b(m.RedToBlue), 0);
			nuint num3;
			for (num3 = 0u; num3 <= (uint)(pixelData.Length - 4); num3 += 4)
			{
				ref uint source2 = ref Unsafe.Add(ref MemoryMarshal.GetReference(pixelData), num3);
				Vector128<uint> vector9 = Unsafe.As<uint, Vector128<uint>>(ref source2);
				Vector128<byte> vector10 = Sse2.And(vector9.AsByte(), right3);
				Vector128<short> vector11 = Sse2.ShiftLeftLogical(Sse2.Add(right: Sse2.MultiplyHigh(Sse2.ShuffleHigh(Sse2.ShuffleLow(vector10.AsInt16(), 160).AsInt16(), 160).AsInt16(), vector7.AsInt16()).AsByte(), left: vector9.AsByte()).AsInt16(), 8);
				Vector128<byte> vector12 = Sse2.Or(Sse2.ShiftRightLogical(Sse2.Add(Sse2.ShiftRightLogical(Sse2.MultiplyHigh(vector11, vector8.AsInt16()).AsInt32(), 8).AsByte(), vector11.AsByte()).AsInt16(), 8).AsByte(), vector10);
				Unsafe.As<uint, Vector128<uint>>(ref source2) = vector12.AsUInt32();
			}
			if (num3 != (uint)pixelData.Length)
			{
				Vp8LMultipliers m3 = m;
				ref Span<uint> reference = ref pixelData;
				int num2 = (int)num3;
				TransformColorInverseScalar(m3, reference.Slice(num2, reference.Length - num2));
			}
		}
		else
		{
			TransformColorInverseScalar(m, pixelData);
		}
	}

	private static void TransformColorInverseScalar(Vp8LMultipliers m, Span<uint> pixelData)
	{
		for (int i = 0; i < pixelData.Length; i++)
		{
			uint num = pixelData[i];
			sbyte color = (sbyte)(num >> 8);
			int num2 = (int)((num >> 16) & 0xFF);
			int num3 = (int)(num & 0xFF);
			num2 += ColorTransformDelta((sbyte)m.GreenToRed, color);
			num2 &= 0xFF;
			num3 += ColorTransformDelta((sbyte)m.GreenToBlue, color);
			num3 += ColorTransformDelta((sbyte)m.RedToBlue, (sbyte)num2);
			num3 &= 0xFF;
			pixelData[i] = (num & 0xFF00FF00u) | (uint)(num2 << 16) | (uint)num3;
		}
	}

	public unsafe static void PredictorInverseTransform(Vp8LTransform transform, Span<uint> pixelData, Span<uint> outputSpan)
	{
		fixed (uint* ptr = pixelData)
		{
			fixed (uint* ptr3 = outputSpan)
			{
				uint* ptr2 = ptr;
				uint* ptr4 = ptr3;
				int xSize = transform.XSize;
				Span<uint> span = transform.Data.GetSpan();
				PredictorAdd0(ptr2, 1, ptr4);
				PredictorAdd1(ptr2 + 1, xSize - 1, ptr4 + 1);
				ptr2 += xSize;
				ptr4 += xSize;
				int num = 1;
				int ySize = transform.YSize;
				int num2 = 1 << transform.Bits;
				int num3 = num2 - 1;
				int num4 = SubSampleSize(xSize, transform.Bits);
				int num5 = (num >> transform.Bits) * num4;
				Span<short> scratch = stackalloc short[8];
				while (num < ySize)
				{
					int num6 = num5;
					int num7 = 1;
					PredictorAdd2(ptr2, ptr4 - xSize, 1, ptr4);
					while (num7 < xSize)
					{
						uint num8 = (span[num6++] >> 8) & 0xF;
						int num9 = (num7 & ~num3) + num2;
						if (num9 > xSize)
						{
							num9 = xSize;
						}
						switch (num8)
						{
						case 0u:
							PredictorAdd0(ptr2 + num7, num9 - num7, ptr4 + num7);
							break;
						case 1u:
							PredictorAdd1(ptr2 + num7, num9 - num7, ptr4 + num7);
							break;
						case 2u:
							PredictorAdd2(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 3u:
							PredictorAdd3(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 4u:
							PredictorAdd4(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 5u:
							PredictorAdd5(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 6u:
							PredictorAdd6(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 7u:
							PredictorAdd7(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 8u:
							PredictorAdd8(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 9u:
							PredictorAdd9(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 10u:
							PredictorAdd10(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 11u:
							PredictorAdd11(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7, scratch);
							break;
						case 12u:
							PredictorAdd12(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						case 13u:
							PredictorAdd13(ptr2 + num7, ptr4 + num7 - xSize, num9 - num7, ptr4 + num7);
							break;
						}
						num7 = num9;
					}
					ptr2 += xSize;
					ptr4 += xSize;
					num++;
					if ((num & num3) == 0)
					{
						num5 += num4;
					}
				}
			}
		}
		outputSpan.CopyTo(pixelData);
	}

	public static void ExpandColorMap(int numColors, Span<uint> transformData, Span<uint> newColorMap)
	{
		newColorMap[0] = transformData[0];
		Span<byte> span = MemoryMarshal.Cast<uint, byte>(transformData);
		Span<byte> span2 = MemoryMarshal.Cast<uint, byte>(newColorMap);
		int num = 4 * numColors;
		int i;
		for (i = 4; i < num; i++)
		{
			span2[i] = (byte)((span[i] + span2[i - 4]) & 0xFF);
		}
		for (int num2 = 4 * newColorMap.Length; i < num2; i++)
		{
			span2[i] = 0;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint SubPixels(uint a, uint b)
	{
		uint num = 16711935 + (a & 0xFF00FF00u) - (b & 0xFF00FF00u);
		uint num2 = (uint)(-16711936 + (int)(a & 0xFF00FF)) - (b & 0xFF00FF);
		return (num & 0xFF00FF00u) | (num2 & 0xFF00FF);
	}

	public static void BundleColorMap(Span<byte> row, int width, int xBits, Span<uint> dst)
	{
		if (xBits > 0)
		{
			int num = 1 << 3 - xBits;
			int num2 = (1 << xBits) - 1;
			uint num3 = 4278190080u;
			for (int i = 0; i < width; i++)
			{
				int num4 = i & num2;
				if (num4 == 0)
				{
					num3 = 4278190080u;
				}
				num3 |= (uint)(row[i] << 8 + num * num4);
				dst[i >> xBits] = num3;
			}
		}
		else
		{
			for (int i = 0; i < width; i++)
			{
				dst[i] = (uint)(0xFF000000u | (row[i] << 8));
			}
		}
	}

	public static float CombinedShannonEntropy(Span<int> x, Span<int> y)
	{
		if (Avx2.IsSupported)
		{
			double num = 0.0;
			Vector256<int> source = Vector256<int>.Zero;
			ref int reference = ref MemoryMarshal.GetReference(x);
			ref int reference2 = ref MemoryMarshal.GetReference(y);
			Vector256<int> vector = Vector256<int>.Zero;
			Vector256<int> vector2 = Vector256<int>.Zero;
			ref int reference3 = ref Unsafe.As<Vector256<int>, int>(ref source);
			for (nuint num2 = 0u; num2 < 256; num2 += 8)
			{
				Vector256<int> vector3 = Unsafe.As<int, Vector256<int>>(ref Unsafe.Add(ref reference, num2));
				Vector256<int> right = Unsafe.As<int, Vector256<int>>(ref Unsafe.Add(ref reference2, num2));
				if (Avx2.MoveMask(Avx2.CompareEqual(vector3, Vector256<int>.Zero).AsByte()) != -1)
				{
					Vector256<int> vector4 = Avx2.Add(vector3, right);
					vector = Avx2.Add(vector, vector4);
					vector2 = Avx2.Add(vector2, vector3);
					Unsafe.As<int, Vector256<int>>(ref reference3) = vector4;
					if (reference3 != 0)
					{
						num -= (double)FastSLog2((uint)reference3);
						if (Unsafe.Add(ref reference, num2) != 0)
						{
							num -= (double)FastSLog2((uint)Unsafe.Add(ref reference, num2));
						}
					}
					if (Unsafe.Add(ref reference3, 1) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference3, 1));
						if (Unsafe.Add(ref reference, num2 + 1) != 0)
						{
							num -= (double)FastSLog2((uint)Unsafe.Add(ref reference, num2 + 1));
						}
					}
					if (Unsafe.Add(ref reference3, 2) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference3, 2));
						if (Unsafe.Add(ref reference, num2 + 2) != 0)
						{
							num -= (double)FastSLog2((uint)Unsafe.Add(ref reference, num2 + 2));
						}
					}
					if (Unsafe.Add(ref reference3, 3) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference3, 3));
						if (Unsafe.Add(ref reference, num2 + 3) != 0)
						{
							num -= (double)FastSLog2((uint)Unsafe.Add(ref reference, num2 + 3));
						}
					}
					if (Unsafe.Add(ref reference3, 4) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference3, 4));
						if (Unsafe.Add(ref reference, num2 + 4) != 0)
						{
							num -= (double)FastSLog2((uint)Unsafe.Add(ref reference, num2 + 4));
						}
					}
					if (Unsafe.Add(ref reference3, 5) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference3, 5));
						if (Unsafe.Add(ref reference, num2 + 5) != 0)
						{
							num -= (double)FastSLog2((uint)Unsafe.Add(ref reference, num2 + 5));
						}
					}
					if (Unsafe.Add(ref reference3, 6) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference3, 6));
						if (Unsafe.Add(ref reference, num2 + 6) != 0)
						{
							num -= (double)FastSLog2((uint)Unsafe.Add(ref reference, num2 + 6));
						}
					}
					if (Unsafe.Add(ref reference3, 7) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference3, 7));
						if (Unsafe.Add(ref reference, num2 + 7) != 0)
						{
							num -= (double)FastSLog2((uint)Unsafe.Add(ref reference, num2 + 7));
						}
					}
				}
				else
				{
					vector = Avx2.Add(vector, right);
					if (Unsafe.Add(ref reference2, num2) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference2, num2));
					}
					if (Unsafe.Add(ref reference2, num2 + 1) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference2, num2 + 1));
					}
					if (Unsafe.Add(ref reference2, num2 + 2) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference2, num2 + 2));
					}
					if (Unsafe.Add(ref reference2, num2 + 3) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference2, num2 + 3));
					}
					if (Unsafe.Add(ref reference2, num2 + 4) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference2, num2 + 4));
					}
					if (Unsafe.Add(ref reference2, num2 + 5) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference2, num2 + 5));
					}
					if (Unsafe.Add(ref reference2, num2 + 6) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference2, num2 + 6));
					}
					if (Unsafe.Add(ref reference2, num2 + 7) != 0)
					{
						num -= (double)FastSLog2((uint)Unsafe.Add(ref reference2, num2 + 7));
					}
				}
			}
			int v = Numerics.ReduceSum(vector2);
			int v2 = Numerics.ReduceSum(vector);
			num += (double)(FastSLog2((uint)v) + FastSLog2((uint)v2));
			return (float)num;
		}
		double num3 = 0.0;
		uint num4 = 0u;
		uint num5 = 0u;
		for (int i = 0; i < 256; i++)
		{
			uint num6 = (uint)x[i];
			if (num6 != 0)
			{
				uint num7 = num6 + (uint)y[i];
				num4 += num6;
				num3 -= (double)FastSLog2(num6);
				num5 += num7;
				num3 -= (double)FastSLog2(num7);
			}
			else if (y[i] != 0)
			{
				num5 += (uint)y[i];
				num3 -= (double)FastSLog2((uint)y[i]);
			}
		}
		num3 += (double)(FastSLog2(num4) + FastSLog2(num5));
		return (float)num3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte TransformColorRed(sbyte greenToRed, uint argb)
	{
		sbyte color = U32ToS8(argb >> 8);
		return (byte)(((int)(argb >> 16) - ColorTransformDelta(greenToRed, color)) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte TransformColorBlue(sbyte greenToBlue, sbyte redToBlue, uint argb)
	{
		sbyte color = U32ToS8(argb >> 8);
		sbyte color2 = U32ToS8(argb >> 16);
		return (byte)(((int)(argb & 0xFF) - ColorTransformDelta(greenToBlue, color) - ColorTransformDelta(redToBlue, color2)) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastLog2(uint v)
	{
		if (v >= 256)
		{
			return FastLog2Slow(v);
		}
		return WebpLookupTables.Log2Table[v];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastSLog2(uint v)
	{
		if (v >= 256)
		{
			return FastSLog2Slow(v);
		}
		return WebpLookupTables.SLog2Table[v];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ColorCodeToMultipliers(uint colorCode, ref Vp8LMultipliers m)
	{
		m.GreenToRed = (byte)(colorCode & 0xFF);
		m.GreenToBlue = (byte)((colorCode >> 8) & 0xFF);
		m.RedToBlue = (byte)((colorCode >> 16) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int NearLosslessBits(int nearLosslessQuality)
	{
		return 5 - nearLosslessQuality / 20;
	}

	private static float FastSLog2Slow(uint v)
	{
		if (v < 65536)
		{
			int num = 0;
			uint num2 = 1u;
			float num3 = v;
			uint num4 = v;
			do
			{
				num++;
				v >>= 1;
				num2 <<= 1;
			}
			while (v >= 256);
			int num5 = (int)(23 * (num4 & (num2 - 1)) >> 4);
			return num3 * (WebpLookupTables.Log2Table[v] + (float)num) + (float)num5;
		}
		return (float)(1.4426950408889634 * (double)v * Math.Log(v));
	}

	private static float FastLog2Slow(uint v)
	{
		if (v < 65536)
		{
			int num = 0;
			uint num2 = 1u;
			uint num3 = v;
			do
			{
				num++;
				v >>= 1;
				num2 <<= 1;
			}
			while (v >= 256);
			double num4 = WebpLookupTables.Log2Table[v] + (float)num;
			if (num3 >= 4096)
			{
				int num5 = (int)(23 * (num3 & (num2 - 1))) >> 4;
				num4 += (double)num5 / (double)num3;
			}
			return (float)num4;
		}
		return (float)(1.4426950408889634 * Math.Log(v));
	}

	private static int PrefixEncodeBitsNoLut(int distance, ref int extraBits)
	{
		int num = BitOperations.Log2((uint)(--distance));
		int num2 = (distance >> num - 1) & 1;
		extraBits = num - 1;
		return 2 * num + num2;
	}

	private static int PrefixEncodeNoLut(int distance, ref int extraBits, ref int extraBitsValue)
	{
		int num = BitOperations.Log2((uint)(--distance));
		int num2 = (distance >> num - 1) & 1;
		extraBits = num - 1;
		extraBitsValue = distance & ((1 << extraBits) - 1);
		return 2 * num + num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd0(uint* input, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			output[i] = AddPixels(input[i], 4278190080u);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd1(uint* input, int numberOfPixels, uint* output)
	{
		uint b = output[-1];
		for (int i = 0; i < numberOfPixels; i++)
		{
			b = (output[i] = AddPixels(input[i], b));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd2(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor2(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd3(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor3(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd4(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor4(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd5(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor5(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd6(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor6(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd7(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor7(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd8(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor8(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd9(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor9(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd10(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor10(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd11(uint* input, uint* upper, int numberOfPixels, uint* output, Span<short> scratch)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor11(output[i - 1], upper + i, scratch);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd12(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor12(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void PredictorAdd13(uint* input, uint* upper, int numberOfPixels, uint* output)
	{
		for (int i = 0; i < numberOfPixels; i++)
		{
			uint b = Predictor13(output[i - 1], upper + i);
			output[i] = AddPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor2(uint left, uint* top)
	{
		return *top;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor3(uint left, uint* top)
	{
		return top[1];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor4(uint left, uint* top)
	{
		return top[-1];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor5(uint left, uint* top)
	{
		return Average3(left, *top, top[1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor6(uint left, uint* top)
	{
		return Average2(left, top[-1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor7(uint left, uint* top)
	{
		return Average2(left, *top);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor8(uint left, uint* top)
	{
		return Average2(top[-1], *top);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor9(uint left, uint* top)
	{
		return Average2(*top, top[1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor10(uint left, uint* top)
	{
		return Average4(left, top[-1], *top, top[1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor11(uint left, uint* top, Span<short> scratch)
	{
		return Select(*top, left, top[-1], scratch);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor12(uint left, uint* top)
	{
		return ClampedAddSubtractFull(left, *top, top[-1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Predictor13(uint left, uint* top)
	{
		return ClampedAddSubtractHalf(left, *top, top[-1]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub0(uint* input, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			output[i] = SubPixels(input[i], 4278190080u);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub1(uint* input, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			output[i] = SubPixels(input[i], input[i - 1]);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub2(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor2(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub3(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor3(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub4(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor4(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub5(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor5(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub6(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor6(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub7(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor7(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub8(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor8(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub9(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor9(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub10(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor10(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub11(uint* input, uint* upper, int numPixels, uint* output, Span<short> scratch)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor11(input[i - 1], upper + i, scratch);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub12(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor12(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PredictorSub13(uint* input, uint* upper, int numPixels, uint* output)
	{
		for (int i = 0; i < numPixels; i++)
		{
			uint b = Predictor13(input[i - 1], upper + i);
			output[i] = SubPixels(input[i], b);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int SubSampleSize(int size, int samplingBits)
	{
		return size + (1 << samplingBits) - 1 >> samplingBits;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint AddPixels(uint a, uint b)
	{
		uint num = (a & 0xFF00FF00u) + (b & 0xFF00FF00u);
		uint num2 = (a & 0xFF00FF) + (b & 0xFF00FF);
		return (num & 0xFF00FF00u) | (num2 & 0xFF00FF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Cst5b(int x)
	{
		return (short)((short)(x << 8) >> 5);
	}

	private static uint ClampedAddSubtractFull(uint c0, uint c1, uint c2)
	{
		if (Sse2.IsSupported)
		{
			Vector128<byte> vector = Sse2.UnpackLow(Sse2.ConvertScalarToVector128UInt32(c0).AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector2 = Sse2.UnpackLow(Sse2.ConvertScalarToVector128UInt32(c1).AsByte(), Vector128<byte>.Zero);
			Vector128<short> obj = Sse2.Subtract(right: Sse2.UnpackLow(Sse2.ConvertScalarToVector128UInt32(c2).AsByte(), Vector128<byte>.Zero).AsInt16(), left: Sse2.Add(vector.AsInt16(), vector2.AsInt16()));
			return Sse2.ConvertToUInt32(Sse2.PackUnsignedSaturate(obj, obj).AsUInt32());
		}
		int num = AddSubtractComponentFull((int)(c0 >> 24), (int)(c1 >> 24), (int)(c2 >> 24));
		int num2 = AddSubtractComponentFull((int)((c0 >> 16) & 0xFF), (int)((c1 >> 16) & 0xFF), (int)((c2 >> 16) & 0xFF));
		int num3 = AddSubtractComponentFull((int)((c0 >> 8) & 0xFF), (int)((c1 >> 8) & 0xFF), (int)((c2 >> 8) & 0xFF));
		int num4 = AddSubtractComponentFull((int)(c0 & 0xFF), (int)(c1 & 0xFF), (int)(c2 & 0xFF));
		return (uint)((num << 24) | (num2 << 16) | (num3 << 8) | num4);
	}

	private static uint ClampedAddSubtractHalf(uint c0, uint c1, uint c2)
	{
		if (Sse2.IsSupported)
		{
			Vector128<byte> vector = Sse2.UnpackLow(Sse2.ConvertScalarToVector128UInt32(c0).AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector2 = Sse2.UnpackLow(Sse2.ConvertScalarToVector128UInt32(c1).AsByte(), Vector128<byte>.Zero);
			Vector128<byte> vector3 = Sse2.UnpackLow(Sse2.ConvertScalarToVector128UInt32(c2).AsByte(), Vector128<byte>.Zero);
			Vector128<short> vector4 = Sse2.ShiftRightLogical(Sse2.Add(vector2.AsInt16(), vector.AsInt16()), 1);
			Vector128<short> left = Sse2.Subtract(vector4, vector3.AsInt16());
			Vector128<short> right = Sse2.CompareGreaterThan(vector3.AsInt16(), vector4.AsInt16());
			Vector128<short> right2 = Sse2.ShiftRightArithmetic(Sse2.Subtract(left, right), 1);
			Vector128<short> vector5 = Sse2.Add(vector4, right2).AsInt16();
			return Sse2.ConvertToUInt32(Sse2.PackUnsignedSaturate(vector5, vector5).AsUInt32());
		}
		uint num = Average2(c0, c1);
		int num2 = AddSubtractComponentHalf((int)(num >> 24), (int)(c2 >> 24));
		int num3 = AddSubtractComponentHalf((int)((num >> 16) & 0xFF), (int)((c2 >> 16) & 0xFF));
		int num4 = AddSubtractComponentHalf((int)((num >> 8) & 0xFF), (int)((c2 >> 8) & 0xFF));
		int num5 = AddSubtractComponentHalf((int)(num & 0xFF), (int)(c2 & 0xFF));
		return (uint)((num2 << 24) | (num3 << 16) | (num4 << 8) | num5);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int AddSubtractComponentHalf(int a, int b)
	{
		return (int)Clip255((uint)(a + (a - b) / 2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int AddSubtractComponentFull(int a, int b, int c)
	{
		return (int)Clip255((uint)(a + b - c));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Clip255(uint a)
	{
		if (a >= 256)
		{
			return ~a >> 24;
		}
		return a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector128<int> MkCst16(int hi, int lo)
	{
		return Vector128.Create((hi << 16) | (lo & 0xFFFF));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector256<int> MkCst32(int hi, int lo)
	{
		return Vector256.Create((hi << 16) | (lo & 0xFFFF));
	}

	private unsafe static uint Select(uint a, uint b, uint c, Span<short> scratch)
	{
		if (Sse2.IsSupported)
		{
			fixed (short* reference = &MemoryMarshal.GetReference(scratch))
			{
				Vector128<byte> vector = Sse2.ConvertScalarToVector128UInt32(a).AsByte();
				Vector128<byte> vector2 = Sse2.ConvertScalarToVector128UInt32(b).AsByte();
				Vector128<byte> vector3 = Sse2.ConvertScalarToVector128UInt32(c).AsByte();
				Vector128<byte> left = Sse2.SubtractSaturate(vector, vector3);
				Vector128<byte> right = Sse2.SubtractSaturate(vector3, vector);
				Vector128<byte> left2 = Sse2.SubtractSaturate(vector2, vector3);
				Vector128<byte> right2 = Sse2.SubtractSaturate(vector3, vector2);
				Vector128<byte> left3 = Sse2.Or(left, right);
				Vector128<byte> left4 = Sse2.Or(left2, right2);
				Vector128<byte> vector4 = Sse2.UnpackLow(left3, Vector128<byte>.Zero);
				Vector128<ushort> source = Sse2.Subtract(Sse2.UnpackLow(left4, Vector128<byte>.Zero).AsUInt16(), vector4.AsUInt16());
				Sse2.Store((ushort*)reference, source);
				if (reference[3] + reference[2] + reference[1] + *reference > 0)
				{
					return b;
				}
				return a;
			}
		}
		if (Sub3((int)(a >> 24), (int)(b >> 24), (int)(c >> 24)) + Sub3((int)((a >> 16) & 0xFF), (int)((b >> 16) & 0xFF), (int)((c >> 16) & 0xFF)) + Sub3((int)((a >> 8) & 0xFF), (int)((b >> 8) & 0xFF), (int)((c >> 8) & 0xFF)) + Sub3((int)(a & 0xFF), (int)(b & 0xFF), (int)(c & 0xFF)) > 0)
		{
			return b;
		}
		return a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Sub3(int a, int b, int c)
	{
		int value = b - c;
		int value2 = a - c;
		return Math.Abs(value) - Math.Abs(value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Average2(uint a0, uint a1)
	{
		return (((a0 ^ a1) & 0xFEFEFEFEu) >> 1) + (a0 & a1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Average3(uint a0, uint a1, uint a2)
	{
		return Average2(Average2(a0, a2), a1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Average4(uint a0, uint a1, uint a2, uint a3)
	{
		return Average2(Average2(a0, a1), Average2(a2, a3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint GetArgbIndex(uint idx)
	{
		return (idx >> 8) & 0xFF;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ColorTransformDelta(sbyte colorPred, sbyte color)
	{
		return colorPred * color >> 5;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static sbyte U32ToS8(uint v)
	{
		return (sbyte)(v & 0xFF);
	}
}
