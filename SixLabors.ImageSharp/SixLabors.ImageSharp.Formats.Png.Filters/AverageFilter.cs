using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Png.Filters;

internal static class AverageFilter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Decode(Span<byte> scanline, Span<byte> previousScanline, int bytesPerPixel)
	{
		if (Sse2.IsSupported && bytesPerPixel == 4)
		{
			DecodeSse2(scanline, previousScanline);
		}
		else if (AdvSimd.IsSupported && bytesPerPixel == 4)
		{
			DecodeArm(scanline, previousScanline);
		}
		else
		{
			DecodeScalar(scanline, previousScanline, (uint)bytesPerPixel);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeSse2(Span<byte> scanline, Span<byte> previousScanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		Vector128<byte> vector = Vector128<byte>.Zero;
		Vector128<byte> right = Vector128.Create((byte)1);
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= 4)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector128<byte> left = vector;
			Vector128<byte> right2 = Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref Unsafe.Add(ref reference2, num2))).AsByte();
			vector = Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref source)).AsByte();
			Vector128<byte> left2 = Sse2.Average(left, right2);
			Vector128<byte> right3 = Sse2.And(Sse2.Xor(left, right2), right);
			left2 = Sse2.Subtract(left2, right3);
			vector = Sse2.Add(vector, left2);
			Unsafe.As<byte, int>(ref source) = Sse2.ConvertToInt32(vector.AsInt32());
			num -= 4;
			num2 += 4;
		}
	}

	public static void DecodeArm(Span<byte> scanline, Span<byte> previousScanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		Vector64<byte> vector = Vector64<byte>.Zero;
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= 4)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector64<byte> left = vector;
			Vector64<byte> right = Vector64.CreateScalar(Unsafe.As<byte, int>(ref Unsafe.Add(ref reference2, num2))).AsByte();
			vector = Vector64.CreateScalar(Unsafe.As<byte, int>(ref source)).AsByte();
			Vector64<byte> right2 = AdvSimd.FusedAddHalving(left, right);
			vector = AdvSimd.Add(vector, right2);
			Unsafe.As<byte, int>(ref source) = vector.AsInt32().ToScalar();
			num -= 4;
			num2 += 4;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeScalar(Span<byte> scanline, Span<byte> previousScanline, uint bytesPerPixel)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		nuint num;
		for (num = 1u; num <= bytesPerPixel; num++)
		{
			ref byte reference3 = ref Unsafe.Add(ref reference, num);
			byte b = Unsafe.Add(ref reference2, num);
			reference3 = (byte)(reference3 + (b >> 1));
		}
		for (; num < (uint)scanline.Length; num++)
		{
			ref byte reference4 = ref Unsafe.Add(ref reference, num);
			byte left = Unsafe.Add(ref reference, num - bytesPerPixel);
			byte above = Unsafe.Add(ref reference2, num);
			reference4 = (byte)(reference4 + Average(left, above));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Encode(ReadOnlySpan<byte> scanline, ReadOnlySpan<byte> previousScanline, Span<byte> result, uint bytesPerPixel, out int sum)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		ref byte reference3 = ref MemoryMarshal.GetReference(result);
		sum = 0;
		reference3 = 3;
		nuint num = 0u;
		while (num < bytesPerPixel)
		{
			byte b = Unsafe.Add(ref reference, num);
			byte b2 = Unsafe.Add(ref reference2, num);
			num++;
			ref byte reference4 = ref Unsafe.Add(ref reference3, num);
			reference4 = (byte)(b - (b2 >> 1));
			sum += Numerics.Abs((sbyte)reference4);
		}
		if (Avx2.IsSupported)
		{
			Vector256<byte> zero = Vector256<byte>.Zero;
			Vector256<int> vector = Vector256<int>.Zero;
			Vector256<byte> right = Avx2.CompareEqual(vector, vector).AsByte();
			nuint num2 = num - bytesPerPixel;
			while (num <= (uint)(scanline.Length - Vector256<byte>.Count))
			{
				Vector256<byte> left = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference, num));
				Vector256<byte> left2 = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference, num2));
				Vector256<byte> right3 = Avx2.Xor(Avx2.Average(right: Avx2.Xor(Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference2, num)), right), left: Avx2.Xor(left2, right)), right);
				Vector256<byte> vector2 = Avx2.Subtract(left, right3);
				Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference3, num + 1)) = vector2;
				num += (uint)Vector256<byte>.Count;
				vector = Avx2.Add(vector, Avx2.SumAbsoluteDifferences(Avx2.Abs(vector2.AsSByte()), zero).AsInt32());
				num2 += (uint)Vector256<byte>.Count;
			}
			sum += Numerics.EvenReduceSum(vector);
		}
		else if (Sse2.IsSupported)
		{
			Vector128<byte> zero2 = Vector128<byte>.Zero;
			Vector128<int> vector3 = Vector128<int>.Zero;
			Vector128<byte> right4 = Sse2.CompareEqual(vector3, vector3).AsByte();
			nuint num3 = num - bytesPerPixel;
			while (num <= (uint)(scanline.Length - Vector128<byte>.Count))
			{
				Vector128<byte> left3 = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference, num));
				Vector128<byte> left4 = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference, num3));
				Vector128<byte> right6 = Sse2.Xor(Sse2.Average(right: Sse2.Xor(Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference2, num)), right4), left: Sse2.Xor(left4, right4)), right4);
				Vector128<byte> vector4 = Sse2.Subtract(left3, right6);
				Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference3, num + 1)) = vector4;
				num += (uint)Vector128<byte>.Count;
				Vector128<byte> left5;
				if (Ssse3.IsSupported)
				{
					left5 = Ssse3.Abs(vector4.AsSByte());
				}
				else
				{
					Vector128<sbyte> right7 = Sse2.CompareGreaterThan(zero2.AsSByte(), vector4.AsSByte());
					left5 = Sse2.Xor(Sse2.Add(vector4.AsSByte(), right7), right7).AsByte();
				}
				vector3 = Sse2.Add(vector3, Sse2.SumAbsoluteDifferences(left5, zero2).AsInt32());
				num3 += (uint)Vector128<byte>.Count;
			}
			sum += Numerics.EvenReduceSum(vector3);
		}
		nuint num4 = num - bytesPerPixel;
		while (num < (uint)scanline.Length)
		{
			byte b3 = Unsafe.Add(ref reference, num);
			byte left6 = Unsafe.Add(ref reference, num4);
			byte above = Unsafe.Add(ref reference2, num);
			num++;
			ref byte reference5 = ref Unsafe.Add(ref reference3, num);
			reference5 = (byte)(b3 - Average(left6, above));
			sum += Numerics.Abs((sbyte)reference5);
			num4++;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Average(byte left, byte above)
	{
		return left + above >> 1;
	}
}
