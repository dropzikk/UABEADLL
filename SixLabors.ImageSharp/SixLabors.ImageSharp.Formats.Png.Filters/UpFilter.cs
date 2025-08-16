using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Png.Filters;

internal static class UpFilter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Decode(Span<byte> scanline, Span<byte> previousScanline)
	{
		if (Avx2.IsSupported)
		{
			DecodeAvx2(scanline, previousScanline);
		}
		else if (Sse2.IsSupported)
		{
			DecodeSse2(scanline, previousScanline);
		}
		else if (AdvSimd.IsSupported)
		{
			DecodeArm(scanline, previousScanline);
		}
		else
		{
			DecodeScalar(scanline, previousScanline);
		}
	}

	private static void DecodeAvx2(Span<byte> scanline, Span<byte> previousScanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= Vector256<byte>.Count)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector256<byte> right = Unsafe.As<byte, Vector256<byte>>(ref source);
			Vector256<byte> left = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference2, num2));
			Unsafe.As<byte, Vector256<byte>>(ref source) = Avx2.Add(left, right);
			num2 += (uint)Vector256<byte>.Count;
			num -= Vector256<byte>.Count;
		}
		for (nuint num3 = num2; num3 < (uint)scanline.Length; num3++)
		{
			ref byte reference3 = ref Unsafe.Add(ref reference, num2);
			byte b = Unsafe.Add(ref reference2, num2);
			reference3 += b;
			num2++;
		}
	}

	private static void DecodeSse2(Span<byte> scanline, Span<byte> previousScanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= Vector128<byte>.Count)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector128<byte> right = Unsafe.As<byte, Vector128<byte>>(ref source);
			Vector128<byte> left = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference2, num2));
			Unsafe.As<byte, Vector128<byte>>(ref source) = Sse2.Add(left, right);
			num2 += (uint)Vector128<byte>.Count;
			num -= Vector128<byte>.Count;
		}
		for (nuint num3 = num2; num3 < (uint)scanline.Length; num3++)
		{
			ref byte reference3 = ref Unsafe.Add(ref reference, num2);
			byte b = Unsafe.Add(ref reference2, num2);
			reference3 += b;
			num2++;
		}
	}

	private static void DecodeArm(Span<byte> scanline, Span<byte> previousScanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= 16)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector128<byte> left = Unsafe.As<byte, Vector128<byte>>(ref source);
			Vector128<byte> right = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference2, num2));
			Unsafe.As<byte, Vector128<byte>>(ref source) = AdvSimd.Add(left, right);
			num2 += 16;
			num -= 16;
		}
		for (nuint num3 = num2; num3 < (uint)scanline.Length; num3++)
		{
			ref byte reference3 = ref Unsafe.Add(ref reference, num2);
			byte b = Unsafe.Add(ref reference2, num2);
			reference3 += b;
			num2++;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeScalar(Span<byte> scanline, Span<byte> previousScanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		for (nuint num = 1u; num < (uint)scanline.Length; num++)
		{
			ref byte reference3 = ref Unsafe.Add(ref reference, num);
			byte b = Unsafe.Add(ref reference2, num);
			reference3 += b;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Encode(ReadOnlySpan<byte> scanline, ReadOnlySpan<byte> previousScanline, Span<byte> result, out int sum)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		ref byte reference3 = ref MemoryMarshal.GetReference(result);
		sum = 0;
		reference3 = 2;
		nuint num = 0u;
		if (Avx2.IsSupported)
		{
			Vector256<byte> zero = Vector256<byte>.Zero;
			Vector256<int> vector = Vector256<int>.Zero;
			while (num <= (uint)(scanline.Length - Vector256<byte>.Count))
			{
				Vector256<byte> left = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference, num));
				Vector256<byte> right = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference2, num));
				Vector256<byte> vector2 = Avx2.Subtract(left, right);
				Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference3, num + 1)) = vector2;
				num += (uint)Vector256<byte>.Count;
				vector = Avx2.Add(vector, Avx2.SumAbsoluteDifferences(Avx2.Abs(vector2.AsSByte()), zero).AsInt32());
			}
			sum += Numerics.EvenReduceSum(vector);
		}
		else if (Vector.IsHardwareAccelerated)
		{
			Vector<uint> accumulator = Vector<uint>.Zero;
			while (num <= (uint)(scanline.Length - Vector<byte>.Count))
			{
				Vector<byte> vector3 = Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference, num));
				Vector<byte> vector4 = Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference2, num));
				Vector<byte> vector5 = vector3 - vector4;
				Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference3, num + 1)) = vector5;
				num += (uint)Vector<byte>.Count;
				Numerics.Accumulate(ref accumulator, Vector.AsVectorByte(Vector.Abs(Vector.AsVectorSByte(vector5))));
			}
			for (int i = 0; i < Vector<uint>.Count; i++)
			{
				sum += (int)accumulator[i];
			}
		}
		while (num < (uint)scanline.Length)
		{
			byte b = Unsafe.Add(ref reference, num);
			byte b2 = Unsafe.Add(ref reference2, num);
			num++;
			ref byte reference4 = ref Unsafe.Add(ref reference3, num);
			reference4 = (byte)(b - b2);
			sum += Numerics.Abs((sbyte)reference4);
		}
	}
}
