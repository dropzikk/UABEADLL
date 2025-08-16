using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Png.Filters;

internal static class PaethFilter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Decode(Span<byte> scanline, Span<byte> previousScanline, int bytesPerPixel)
	{
		if (Ssse3.IsSupported && bytesPerPixel == 4)
		{
			DecodeSsse3(scanline, previousScanline);
		}
		else if (AdvSimd.Arm64.IsSupported && bytesPerPixel == 4)
		{
			DecodeArm(scanline, previousScanline);
		}
		else
		{
			DecodeScalar(scanline, previousScanline, (uint)bytesPerPixel);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeSsse3(Span<byte> scanline, Span<byte> previousScanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		Vector128<byte> vector = Vector128<byte>.Zero;
		Vector128<byte> vector2 = Vector128<byte>.Zero;
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= 4)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector128<byte> vector3 = vector;
			Vector128<byte> vector4 = vector2;
			vector = Sse2.UnpackLow(Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref Unsafe.Add(ref reference2, num2))).AsByte(), Vector128<byte>.Zero);
			vector2 = Sse2.UnpackLow(Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref source)).AsByte(), Vector128<byte>.Zero);
			Vector128<short> vector5 = Sse2.Subtract(vector.AsInt16(), vector3.AsInt16());
			Vector128<short> vector6 = Sse2.Subtract(vector4.AsInt16(), vector3.AsInt16());
			Vector128<short> vector7 = Sse2.Add(vector5.AsInt16(), vector6.AsInt16());
			vector5 = Ssse3.Abs(vector5.AsInt16()).AsInt16();
			vector6 = Ssse3.Abs(vector6.AsInt16()).AsInt16();
			Vector128<short> left = Sse2.Min(Ssse3.Abs(vector7.AsInt16()).AsInt16(), Sse2.Min(vector5, vector6));
			Vector128<byte> right = SimdUtils.HwIntrinsics.BlendVariable(SimdUtils.HwIntrinsics.BlendVariable(vector3, vector, Sse2.CompareEqual(left, vector6).AsByte()), vector4, Sse2.CompareEqual(left, vector5).AsByte());
			vector2 = Sse2.Add(vector2, right);
			Unsafe.As<byte, int>(ref source) = Sse2.ConvertToInt32(Sse2.PackUnsignedSaturate(vector2.AsInt16(), vector2.AsInt16()).AsInt32());
			num -= 4;
			num2 += 4;
		}
	}

	public static void DecodeArm(Span<byte> scanline, Span<byte> previousScanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		Vector128<byte> vector = Vector128<byte>.Zero;
		Vector128<byte> vector2 = Vector128<byte>.Zero;
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= 4)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector128<byte> vector3 = vector;
			Vector128<byte> vector4 = vector2;
			vector = AdvSimd.Arm64.ZipLow(Vector128.CreateScalar(Unsafe.As<byte, int>(ref Unsafe.Add(ref reference2, num2))).AsByte(), Vector128<byte>.Zero).AsByte();
			vector2 = AdvSimd.Arm64.ZipLow(Vector128.CreateScalar(Unsafe.As<byte, int>(ref source)).AsByte(), Vector128<byte>.Zero).AsByte();
			Vector128<short> vector5 = AdvSimd.Subtract(vector.AsInt16(), vector3.AsInt16());
			Vector128<short> vector6 = AdvSimd.Subtract(vector4.AsInt16(), vector3.AsInt16());
			Vector128<short> vector7 = AdvSimd.Add(vector5.AsInt16(), vector6.AsInt16());
			vector5 = AdvSimd.Abs(vector5.AsInt16()).AsInt16();
			vector6 = AdvSimd.Abs(vector6.AsInt16()).AsInt16();
			Vector128<short> left = AdvSimd.Min(AdvSimd.Abs(vector7.AsInt16()).AsInt16(), AdvSimd.Min(vector5, vector6));
			Vector128<byte> right = SimdUtils.HwIntrinsics.BlendVariable(SimdUtils.HwIntrinsics.BlendVariable(vector3, vector, AdvSimd.CompareEqual(left, vector6).AsByte()), vector4, AdvSimd.CompareEqual(left, vector5).AsByte());
			vector2 = AdvSimd.Add(vector2, right);
			Vector64<byte> vector8 = AdvSimd.ExtractNarrowingSaturateUnsignedLower(vector2.AsInt16());
			Unsafe.As<byte, int>(ref source) = Vector128.Create(vector8, vector8).AsInt32().ToScalar();
			num -= 4;
			num2 += 4;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeScalar(Span<byte> scanline, Span<byte> previousScanline, uint bytesPerPixel)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		nuint num = bytesPerPixel + 1;
		nuint num2;
		for (num2 = 1u; num2 < num; num2++)
		{
			ref byte reference3 = ref Unsafe.Add(ref reference, num2);
			byte b = Unsafe.Add(ref reference2, num2);
			reference3 += b;
		}
		for (; num2 < (uint)scanline.Length; num2++)
		{
			ref byte reference4 = ref Unsafe.Add(ref reference, num2);
			byte left = Unsafe.Add(ref reference, num2 - bytesPerPixel);
			byte above = Unsafe.Add(ref reference2, num2);
			byte upperLeft = Unsafe.Add(ref reference2, num2 - bytesPerPixel);
			reference4 += PaethPredictor(left, above, upperLeft);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Encode(ReadOnlySpan<byte> scanline, ReadOnlySpan<byte> previousScanline, Span<byte> result, int bytesPerPixel, out int sum)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(previousScanline);
		ref byte reference3 = ref MemoryMarshal.GetReference(result);
		sum = 0;
		reference3 = 4;
		nuint num = 0u;
		while (num < (uint)bytesPerPixel)
		{
			byte b = Unsafe.Add(ref reference, num);
			byte above = Unsafe.Add(ref reference2, num);
			num++;
			ref byte reference4 = ref Unsafe.Add(ref reference3, num);
			reference4 = (byte)(b - PaethPredictor(0, above, 0));
			sum += Numerics.Abs((sbyte)reference4);
		}
		if (Avx2.IsSupported)
		{
			Vector256<byte> zero = Vector256<byte>.Zero;
			Vector256<int> vector = Vector256<int>.Zero;
			nuint num2 = num - (uint)bytesPerPixel;
			while ((int)num <= scanline.Length - Vector256<byte>.Count)
			{
				Vector256<byte> left = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference, num));
				Vector256<byte> left2 = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference, num2));
				Vector256<byte> above2 = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference2, num));
				Vector256<byte> upleft = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference2, num2));
				Vector256<byte> vector2 = Avx2.Subtract(left, PaethPredictor(left2, above2, upleft));
				Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference3, num + 1)) = vector2;
				num += (uint)Vector256<byte>.Count;
				vector = Avx2.Add(vector, Avx2.SumAbsoluteDifferences(Avx2.Abs(vector2.AsSByte()), zero).AsInt32());
				num2 += (uint)Vector256<byte>.Count;
			}
			sum += Numerics.EvenReduceSum(vector);
		}
		else if (Vector.IsHardwareAccelerated)
		{
			Vector<uint> accumulator = Vector<uint>.Zero;
			nuint num3 = num - (uint)bytesPerPixel;
			while ((int)num <= scanline.Length - Vector<byte>.Count)
			{
				Vector<byte> vector3 = Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference, num));
				Vector<byte> left3 = Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference, num3));
				Vector<byte> above3 = Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference2, num));
				Vector<byte> upperLeft = Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference2, num3));
				Vector<byte> vector4 = vector3 - PaethPredictor(left3, above3, upperLeft);
				Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference3, num + 1)) = vector4;
				num += (uint)Vector<byte>.Count;
				Numerics.Accumulate(ref accumulator, Vector.AsVectorByte(Vector.Abs(Vector.AsVectorSByte(vector4))));
				num3 += (uint)Vector<byte>.Count;
			}
			for (int i = 0; i < Vector<uint>.Count; i++)
			{
				sum += (int)accumulator[i];
			}
		}
		nuint num4 = num - (uint)bytesPerPixel;
		while ((int)num < scanline.Length)
		{
			byte b2 = Unsafe.Add(ref reference, num);
			byte left4 = Unsafe.Add(ref reference, num4);
			byte above4 = Unsafe.Add(ref reference2, num);
			byte upperLeft2 = Unsafe.Add(ref reference2, num4);
			num++;
			ref byte reference5 = ref Unsafe.Add(ref reference3, num);
			reference5 = (byte)(b2 - PaethPredictor(left4, above4, upperLeft2));
			sum += Numerics.Abs((sbyte)reference5);
			num4++;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte PaethPredictor(byte left, byte above, byte upperLeft)
	{
		int num = left + above - upperLeft;
		int num2 = Numerics.Abs(num - left);
		int num3 = Numerics.Abs(num - above);
		int num4 = Numerics.Abs(num - upperLeft);
		if (num2 <= num3 && num2 <= num4)
		{
			return left;
		}
		if (num3 <= num4)
		{
			return above;
		}
		return upperLeft;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector256<byte> PaethPredictor(Vector256<byte> left, Vector256<byte> above, Vector256<byte> upleft)
	{
		Vector256<byte> zero = Vector256<byte>.Zero;
		Vector256<byte> vector = Avx2.SubtractSaturate(above, upleft);
		Vector256<byte> vector2 = Avx2.SubtractSaturate(left, upleft);
		Vector256<byte> vector3 = Avx2.Or(Avx2.SubtractSaturate(upleft, above), vector);
		Vector256<byte> vector4 = Avx2.Or(Avx2.SubtractSaturate(upleft, left), vector2);
		Vector256<byte> left2 = Avx2.Min(Avx2.Or(Avx2.CompareEqual(Avx2.CompareEqual(vector, zero), Avx2.CompareEqual(vector2, zero)), Avx2.Or(Avx2.SubtractSaturate(vector4, vector3), Avx2.SubtractSaturate(vector3, vector4))), vector4);
		return Avx2.BlendVariable(Avx2.BlendVariable(upleft, above, Avx2.CompareEqual(left2, vector4)), left, Avx2.CompareEqual(Avx2.Min(left2, vector3), vector3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector<byte> PaethPredictor(Vector<byte> left, Vector<byte> above, Vector<byte> upperLeft)
	{
		Vector.Widen(left, out var low, out var high);
		Vector.Widen(above, out var low2, out var high2);
		Vector.Widen(upperLeft, out var low3, out var high3);
		Vector<short> low4 = PaethPredictor(Vector.AsVectorInt16(low), Vector.AsVectorInt16(low2), Vector.AsVectorInt16(low3));
		Vector<short> high4 = PaethPredictor(Vector.AsVectorInt16(high), Vector.AsVectorInt16(high2), Vector.AsVectorInt16(high3));
		return Vector.AsVectorByte(Vector.Narrow(low4, high4));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector<short> PaethPredictor(Vector<short> left, Vector<short> above, Vector<short> upperLeft)
	{
		Vector<short> vector = left + above - upperLeft;
		Vector<short> left2 = Vector.Abs(vector - left);
		Vector<short> vector2 = Vector.Abs(vector - above);
		Vector<short> right = Vector.Abs(vector - upperLeft);
		Vector<short> left3 = Vector.LessThanOrEqual(left2, vector2);
		Vector<short> right2 = Vector.LessThanOrEqual(left2, right);
		Vector<short> condition = Vector.LessThanOrEqual(vector2, right);
		return Vector.ConditionalSelect(Vector.BitwiseAnd(left3, right2), left, Vector.ConditionalSelect(condition, above, upperLeft));
	}
}
