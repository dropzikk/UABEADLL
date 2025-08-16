using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Png.Filters;

internal static class SubFilter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Decode(Span<byte> scanline, int bytesPerPixel)
	{
		if (Sse2.IsSupported && bytesPerPixel == 4)
		{
			DecodeSse2(scanline);
		}
		else if (AdvSimd.IsSupported && bytesPerPixel == 4)
		{
			DecodeArm(scanline);
		}
		else
		{
			DecodeScalar(scanline, (uint)bytesPerPixel);
		}
	}

	private static void DecodeSse2(Span<byte> scanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		Vector128<byte> vector = Vector128<byte>.Zero;
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= 4)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector128<byte> right = vector;
			vector = Sse2.ConvertScalarToVector128Int32(Unsafe.As<byte, int>(ref source)).AsByte();
			vector = Sse2.Add(vector, right);
			Unsafe.As<byte, int>(ref source) = Sse2.ConvertToInt32(vector.AsInt32());
			num -= 4;
			num2 += 4;
		}
	}

	public static void DecodeArm(Span<byte> scanline)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		Vector64<byte> vector = Vector64<byte>.Zero;
		int num = scanline.Length;
		nuint num2 = 1u;
		while (num >= 4)
		{
			ref byte source = ref Unsafe.Add(ref reference, num2);
			Vector64<byte> right = vector;
			vector = Vector64.CreateScalar(Unsafe.As<byte, int>(ref source)).AsByte();
			vector = AdvSimd.Add(vector, right);
			Unsafe.As<byte, int>(ref source) = vector.AsInt32().ToScalar();
			num -= 4;
			num2 += 4;
		}
	}

	private static void DecodeScalar(Span<byte> scanline, nuint bytesPerPixel)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		nuint num = bytesPerPixel + 1;
		Unsafe.Add(ref reference, num);
		for (; num < (uint)scanline.Length; num++)
		{
			ref byte reference2 = ref Unsafe.Add(ref reference, num);
			byte b = Unsafe.Add(ref reference, num - bytesPerPixel);
			reference2 += b;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Encode(ReadOnlySpan<byte> scanline, ReadOnlySpan<byte> result, int bytesPerPixel, out int sum)
	{
		ref byte reference = ref MemoryMarshal.GetReference(scanline);
		ref byte reference2 = ref MemoryMarshal.GetReference(result);
		sum = 0;
		reference2 = 1;
		nuint num = 0u;
		while (num < (uint)bytesPerPixel)
		{
			byte b = Unsafe.Add(ref reference, num);
			num++;
			ref byte reference3 = ref Unsafe.Add(ref reference2, num);
			reference3 = b;
			sum += Numerics.Abs((sbyte)reference3);
		}
		if (Avx2.IsSupported)
		{
			Vector256<byte> zero = Vector256<byte>.Zero;
			Vector256<int> vector = Vector256<int>.Zero;
			nuint num2 = num - (uint)bytesPerPixel;
			while (num <= (uint)(scanline.Length - Vector256<byte>.Count))
			{
				Vector256<byte> left = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference, num));
				Vector256<byte> right = Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference, num2));
				Vector256<byte> vector2 = Avx2.Subtract(left, right);
				Unsafe.As<byte, Vector256<byte>>(ref Unsafe.Add(ref reference2, num + 1)) = vector2;
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
			while (num <= (uint)(scanline.Length - Vector<byte>.Count))
			{
				Vector<byte> vector3 = Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference, num));
				Vector<byte> vector4 = Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference, num3));
				Vector<byte> vector5 = vector3 - vector4;
				Unsafe.As<byte, Vector<byte>>(ref Unsafe.Add(ref reference2, num + 1)) = vector5;
				num += (uint)Vector<byte>.Count;
				Numerics.Accumulate(ref accumulator, Vector.AsVectorByte(Vector.Abs(Vector.AsVectorSByte(vector5))));
				num3 += (uint)Vector<byte>.Count;
			}
			for (int i = 0; i < Vector<uint>.Count; i++)
			{
				sum += (int)accumulator[i];
			}
		}
		nuint num4 = num - (uint)bytesPerPixel;
		while (num < (uint)scanline.Length)
		{
			byte b2 = Unsafe.Add(ref reference, num);
			byte b3 = Unsafe.Add(ref reference, num4);
			num++;
			ref byte reference4 = ref Unsafe.Add(ref reference2, num);
			reference4 = (byte)(b2 - b3);
			sum += Numerics.Abs((sbyte)reference4);
			num4++;
		}
	}
}
