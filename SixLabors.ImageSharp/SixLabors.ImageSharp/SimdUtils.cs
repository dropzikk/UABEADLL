using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

internal static class SimdUtils
{
	private struct ByteTuple4
	{
		public byte V0;

		public byte V1;

		public byte V2;

		public byte V3;
	}

	public static class ExtendedIntrinsics
	{
		public static bool IsAvailable { get; } = Vector.IsHardwareAccelerated;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ConvertToSingle(Vector<short> source, out Vector<float> dest1, out Vector<float> dest2)
		{
			Vector.Widen(source, out var low, out var high);
			dest1 = Vector.ConvertToSingle(low);
			dest2 = Vector.ConvertToSingle(high);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ByteToNormalizedFloatReduce(ref ReadOnlySpan<byte> source, ref Span<float> dest)
		{
			if (IsAvailable)
			{
				int num = Numerics.ModuloP2(source.Length, Vector<byte>.Count);
				int num2 = source.Length - num;
				if (num2 > 0)
				{
					ByteToNormalizedFloat(source.Slice(0, num2), dest.Slice(0, num2));
					int num3 = num2;
					source = source.Slice(num3, source.Length - num3);
					num3 = num2;
					dest = dest.Slice(num3, dest.Length - num3);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void NormalizedFloatToByteSaturateReduce(ref ReadOnlySpan<float> source, ref Span<byte> dest)
		{
			if (IsAvailable)
			{
				int num = Numerics.ModuloP2(source.Length, Vector<byte>.Count);
				int num2 = source.Length - num;
				if (num2 > 0)
				{
					NormalizedFloatToByteSaturate(source.Slice(0, num2), dest.Slice(0, num2));
					int num3 = num2;
					source = source.Slice(num3, source.Length - num3);
					num3 = num2;
					dest = dest.Slice(num3, dest.Length - num3);
				}
			}
		}

		internal static void ByteToNormalizedFloat(ReadOnlySpan<byte> source, Span<float> dest)
		{
			nuint num = dest.VectorCount<byte>();
			ref Vector<byte> source2 = ref Unsafe.As<byte, Vector<byte>>(ref MemoryMarshal.GetReference(source));
			ref Vector<float> source3 = ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(dest));
			for (nuint num2 = 0u; num2 < num; num2++)
			{
				Vector.Widen(Unsafe.Add(ref source2, num2), out var low, out var high);
				Vector.Widen(low, out var low2, out var high2);
				Vector.Widen(high, out var low3, out var high3);
				Vector<float> vector = ConvertToSingle(low2);
				Vector<float> vector2 = ConvertToSingle(high2);
				Vector<float> vector3 = ConvertToSingle(low3);
				Vector<float> vector4 = ConvertToSingle(high3);
				ref Vector<float> reference = ref Unsafe.Add(ref source3, num2 * 4);
				reference = vector;
				Unsafe.Add(ref reference, 1) = vector2;
				Unsafe.Add(ref reference, 2) = vector3;
				Unsafe.Add(ref reference, 3) = vector4;
			}
		}

		internal static void NormalizedFloatToByteSaturate(ReadOnlySpan<float> source, Span<byte> dest)
		{
			nuint num = dest.VectorCount<byte>();
			ref Vector<float> source2 = ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(source));
			ref Vector<byte> source3 = ref Unsafe.As<byte, Vector<byte>>(ref MemoryMarshal.GetReference(dest));
			for (nuint num2 = 0u; num2 < num; num2++)
			{
				ref Vector<float> reference = ref Unsafe.Add(ref source2, num2 * 4);
				Vector<float> vf = reference;
				Vector<float> vf2 = Unsafe.Add(ref reference, 1);
				Vector<float> vf3 = Unsafe.Add(ref reference, 2);
				Vector<float> vf4 = Unsafe.Add(ref reference, 3);
				Vector<uint> low = ConvertToUInt32(vf);
				Vector<uint> high = ConvertToUInt32(vf2);
				Vector<uint> low2 = ConvertToUInt32(vf3);
				Vector<uint> high2 = ConvertToUInt32(vf4);
				Vector<ushort> low3 = Vector.Narrow(low, high);
				Vector<ushort> high3 = Vector.Narrow(low2, high2);
				Unsafe.Add(ref source3, num2) = Vector.Narrow(low3, high3);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector<uint> ConvertToUInt32(Vector<float> vf)
		{
			Vector<float> vector = new Vector<float>(255f);
			vf *= vector;
			vf += new Vector<float>(0.5f);
			vf = Vector.Min(Vector.Max(vf, Vector<float>.Zero), vector);
			return Vector.AsVectorUInt32(Vector.ConvertToInt32(vf));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector<float> ConvertToSingle(Vector<uint> u)
		{
			return Vector.ConvertToSingle(Vector.AsVectorInt32(u)) * new Vector<float>(0.003921569f);
		}
	}

	public static class FallbackIntrinsics128
	{
		private struct ByteVector4
		{
			public byte X;

			public byte Y;

			public byte Z;

			public byte W;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ByteToNormalizedFloatReduce(ref ReadOnlySpan<byte> source, ref Span<float> dest)
		{
			int num = Numerics.Modulo4(source.Length);
			int num2 = source.Length - num;
			if (num2 > 0)
			{
				ByteToNormalizedFloat(source.Slice(0, num2), dest.Slice(0, num2));
				int num3 = num2;
				source = source.Slice(num3, source.Length - num3);
				num3 = num2;
				dest = dest.Slice(num3, dest.Length - num3);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void NormalizedFloatToByteSaturateReduce(ref ReadOnlySpan<float> source, ref Span<byte> dest)
		{
			int num = Numerics.Modulo4(source.Length);
			int num2 = source.Length - num;
			if (num2 > 0)
			{
				NormalizedFloatToByteSaturate(source.Slice(0, num2), dest.Slice(0, num2));
				int num3 = num2;
				source = source.Slice(num3, source.Length - num3);
				num3 = num2;
				dest = dest.Slice(num3, dest.Length - num3);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void ByteToNormalizedFloat(ReadOnlySpan<byte> source, Span<float> dest)
		{
			uint num = (uint)dest.Length / 4u;
			if (num != 0)
			{
				ref ByteVector4 source2 = ref Unsafe.As<byte, ByteVector4>(ref MemoryMarshal.GetReference(source));
				ref Vector4 source3 = ref Unsafe.As<float, Vector4>(ref MemoryMarshal.GetReference(dest));
				Vector4 vector = default(Vector4);
				for (nuint num2 = 0u; num2 < num; num2++)
				{
					ref ByteVector4 reference = ref Unsafe.Add(ref source2, num2);
					vector.X = (int)reference.X;
					vector.Y = (int)reference.Y;
					vector.Z = (int)reference.Z;
					vector.W = (int)reference.W;
					vector *= 0.003921569f;
					Unsafe.Add(ref source3, num2) = vector;
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void NormalizedFloatToByteSaturate(ReadOnlySpan<float> source, Span<byte> dest)
		{
			uint num = (uint)source.Length / 4u;
			if (num != 0)
			{
				ref Vector4 source2 = ref Unsafe.As<float, Vector4>(ref MemoryMarshal.GetReference(source));
				ref ByteVector4 source3 = ref Unsafe.As<byte, ByteVector4>(ref MemoryMarshal.GetReference(dest));
				Vector4 vector = new Vector4(0.5f);
				Vector4 vector2 = new Vector4(255f);
				for (nuint num2 = 0u; num2 < num; num2++)
				{
					Vector4 value = Unsafe.Add(ref source2, num2);
					value *= vector2;
					value += vector;
					value = Numerics.Clamp(value, Vector4.Zero, vector2);
					ref ByteVector4 reference = ref Unsafe.Add(ref source3, num2);
					reference.X = (byte)value.X;
					reference.Y = (byte)value.Y;
					reference.Z = (byte)value.Z;
					reference.W = (byte)value.W;
				}
			}
		}
	}

	public static class HwIntrinsics
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<int> PermuteMaskDeinterleave8x32()
		{
			return Vector256.Create(0, 0, 0, 0, 4, 0, 0, 0, 1, 0, 0, 0, 5, 0, 0, 0, 2, 0, 0, 0, 6, 0, 0, 0, 3, 0, 0, 0, 7, 0, 0, 0).AsInt32();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<uint> PermuteMaskEvenOdd8x32()
		{
			return Vector256.Create(0, 0, 0, 0, 2, 0, 0, 0, 4, 0, 0, 0, 6, 0, 0, 0, 1, 0, 0, 0, 3, 0, 0, 0, 5, 0, 0, 0, 7, 0, 0, 0).AsUInt32();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<uint> PermuteMaskSwitchInnerDWords8x32()
		{
			return Vector256.Create(0, 0, 0, 0, 1, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 6, 0, 0, 0, 7, 0, 0, 0).AsUInt32();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector256<uint> MoveFirst24BytesToSeparateLanes()
		{
			return Vector256.Create(0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 6, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 7, 0, 0, 0).AsUInt32();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector256<byte> ExtractRgb()
		{
			return Vector256.Create(0, 3, 6, 9, 1, 4, 7, 10, 2, 5, 8, 11, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 0, 3, 6, 9, 1, 4, 7, 10, 2, 5, 8, 11, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector128<byte> ShuffleMaskPad4Nx16()
		{
			return Vector128.Create(0, 1, 2, 128, 3, 4, 5, 128, 6, 7, 8, 128, 9, 10, 11, 128);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector128<byte> ShuffleMaskSlice4Nx16()
		{
			return Vector128.Create(0, 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 128, 128, 128, 128);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector256<byte> ShuffleMaskShiftAlpha()
		{
			return Vector256.Create((byte)0, (byte)1, (byte)2, (byte)4, (byte)5, (byte)6, (byte)8, (byte)9, (byte)10, (byte)12, (byte)13, (byte)14, (byte)3, (byte)7, (byte)11, (byte)15, (byte)0, (byte)1, (byte)2, (byte)4, (byte)5, (byte)6, (byte)8, (byte)9, (byte)10, (byte)12, (byte)13, (byte)14, (byte)3, (byte)7, (byte)11, (byte)15);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<uint> PermuteMaskShiftAlpha8x32()
		{
			return Vector256.Create(0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 6, 0, 0, 0, 3, 0, 0, 0, 7, 0, 0, 0).AsUInt32();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Shuffle4Reduce(ref ReadOnlySpan<float> source, ref Span<float> dest, byte control)
		{
			if (Avx.IsSupported || Sse.IsSupported)
			{
				int num = (Avx.IsSupported ? Numerics.ModuloP2(source.Length, Vector256<float>.Count) : Numerics.ModuloP2(source.Length, Vector128<float>.Count));
				int num2 = source.Length - num;
				if (num2 > 0)
				{
					Shuffle4(source.Slice(0, num2), dest.Slice(0, num2), control);
					int num3 = num2;
					source = source.Slice(num3, source.Length - num3);
					num3 = num2;
					dest = dest.Slice(num3, dest.Length - num3);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Shuffle4Reduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest, byte control)
		{
			if (Avx2.IsSupported || Ssse3.IsSupported)
			{
				int num = (Avx2.IsSupported ? Numerics.ModuloP2(source.Length, Vector256<byte>.Count) : Numerics.ModuloP2(source.Length, Vector128<byte>.Count));
				int num2 = source.Length - num;
				if (num2 > 0)
				{
					Shuffle4(source.Slice(0, num2), dest.Slice(0, num2), control);
					int num3 = num2;
					source = source.Slice(num3, source.Length - num3);
					num3 = num2;
					dest = dest.Slice(num3, dest.Length - num3);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Shuffle3Reduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest, byte control)
		{
			if (Ssse3.IsSupported)
			{
				int num = source.Length % (Vector128<byte>.Count * 3);
				int num2 = source.Length - num;
				if (num2 > 0)
				{
					Shuffle3(source.Slice(0, num2), dest.Slice(0, num2), control);
					int num3 = num2;
					source = source.Slice(num3, source.Length - num3);
					num3 = num2;
					dest = dest.Slice(num3, dest.Length - num3);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Pad3Shuffle4Reduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest, byte control)
		{
			if (Ssse3.IsSupported)
			{
				int num = source.Length % (Vector128<byte>.Count * 3);
				int num2 = source.Length - num;
				int num3 = (int)((uint)(num2 * 4) / 3u);
				if (num2 > 0)
				{
					Pad3Shuffle4(source.Slice(0, num2), dest.Slice(0, num3), control);
					int num4 = num2;
					source = source.Slice(num4, source.Length - num4);
					num4 = num3;
					dest = dest.Slice(num4, dest.Length - num4);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Shuffle4Slice3Reduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest, byte control)
		{
			if (Ssse3.IsSupported)
			{
				int num = source.Length & (Vector128<byte>.Count * 4 - 1);
				int num2 = source.Length - num;
				int num3 = (int)((uint)(num2 * 3) / 4u);
				if (num2 > 0)
				{
					Shuffle4Slice3(source.Slice(0, num2), dest.Slice(0, num3), control);
					int num4 = num2;
					source = source.Slice(num4, source.Length - num4);
					num4 = num3;
					dest = dest.Slice(num4, dest.Length - num4);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Shuffle4(ReadOnlySpan<float> source, Span<float> dest, byte control)
		{
			if (Avx.IsSupported)
			{
				ref Vector256<float> source2 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(source));
				ref Vector256<float> source3 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(dest));
				nint num = (nint)dest.Vector256Count<float>();
				nint num2 = Numerics.Modulo4(num);
				nint num3 = num - num2;
				for (nint num4 = 0; num4 < num3; num4 += 4)
				{
					ref Vector256<float> reference = ref Unsafe.Add(ref source3, num4);
					ref Vector256<float> reference2 = ref Unsafe.Add(ref source2, num4);
					reference = Avx.Permute(reference2, control);
					Unsafe.Add(ref reference, 1) = Avx.Permute(Unsafe.Add(ref reference2, 1), control);
					Unsafe.Add(ref reference, 2) = Avx.Permute(Unsafe.Add(ref reference2, 2), control);
					Unsafe.Add(ref reference, 3) = Avx.Permute(Unsafe.Add(ref reference2, 3), control);
				}
				if (num2 > 0)
				{
					for (nint num5 = num3; num5 < num; num5++)
					{
						Unsafe.Add(ref source3, num5) = Avx.Permute(Unsafe.Add(ref source2, num5), control);
					}
				}
				return;
			}
			ref Vector128<float> source4 = ref Unsafe.As<float, Vector128<float>>(ref MemoryMarshal.GetReference(source));
			ref Vector128<float> source5 = ref Unsafe.As<float, Vector128<float>>(ref MemoryMarshal.GetReference(dest));
			nint num6 = (nint)((uint)dest.Length / (uint)Vector128<float>.Count);
			nint num7 = Numerics.Modulo4(num6);
			nint num8 = num6 - num7;
			for (nint num9 = 0; num9 < num8; num9 += 4)
			{
				ref Vector128<float> reference3 = ref Unsafe.Add(ref source5, num9);
				ref Vector128<float> reference4 = ref Unsafe.Add(ref source4, num9);
				reference3 = Sse.Shuffle(reference4, reference4, control);
				Vector128<float> vector = Unsafe.Add(ref reference4, 1);
				Unsafe.Add(ref reference3, 1) = Sse.Shuffle(vector, vector, control);
				Vector128<float> vector2 = Unsafe.Add(ref reference4, 2);
				Unsafe.Add(ref reference3, 2) = Sse.Shuffle(vector2, vector2, control);
				Vector128<float> vector3 = Unsafe.Add(ref reference4, 3);
				Unsafe.Add(ref reference3, 3) = Sse.Shuffle(vector3, vector3, control);
			}
			if (num7 > 0)
			{
				for (nint num10 = num8; num10 < num6; num10++)
				{
					Vector128<float> vector4 = Unsafe.Add(ref source4, num10);
					Unsafe.Add(ref source5, num10) = Sse.Shuffle(vector4, vector4, control);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Shuffle4(ReadOnlySpan<byte> source, Span<byte> dest, byte control)
		{
			if (Avx2.IsSupported)
			{
				Span<byte> span = stackalloc byte[Vector256<byte>.Count];
				Shuffle.MMShuffleSpan(ref span, control);
				Vector256<byte> mask = Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(span));
				ref Vector256<byte> source2 = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(source));
				ref Vector256<byte> source3 = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(dest));
				nint num = (nint)((uint)dest.Length / (uint)Vector256<byte>.Count);
				nint num2 = Numerics.Modulo4(num);
				nint num3 = num - num2;
				for (nint num4 = 0; num4 < num3; num4 += 4)
				{
					ref Vector256<byte> reference = ref Unsafe.Add(ref source2, num4);
					ref Vector256<byte> reference2 = ref Unsafe.Add(ref source3, num4);
					reference2 = Avx2.Shuffle(reference, mask);
					Unsafe.Add(ref reference2, 1) = Avx2.Shuffle(Unsafe.Add(ref reference, 1), mask);
					Unsafe.Add(ref reference2, 2) = Avx2.Shuffle(Unsafe.Add(ref reference, 2), mask);
					Unsafe.Add(ref reference2, 3) = Avx2.Shuffle(Unsafe.Add(ref reference, 3), mask);
				}
				if (num2 > 0)
				{
					for (nint num5 = num3; num5 < num; num5++)
					{
						Unsafe.Add(ref source3, num5) = Avx2.Shuffle(Unsafe.Add(ref source2, num5), mask);
					}
				}
				return;
			}
			Span<byte> span2 = stackalloc byte[Vector128<byte>.Count];
			Shuffle.MMShuffleSpan(ref span2, control);
			Vector128<byte> mask2 = Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(span2));
			ref Vector128<byte> source4 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(source));
			ref Vector128<byte> source5 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(dest));
			nint num6 = (nint)((uint)dest.Length / (uint)Vector128<byte>.Count);
			nint num7 = Numerics.Modulo4(num6);
			nint num8 = num6 - num7;
			for (nint num9 = 0; num9 < num8; num9 += 4)
			{
				ref Vector128<byte> reference3 = ref Unsafe.Add(ref source4, num9);
				ref Vector128<byte> reference4 = ref Unsafe.Add(ref source5, num9);
				reference4 = Ssse3.Shuffle(reference3, mask2);
				Unsafe.Add(ref reference4, 1) = Ssse3.Shuffle(Unsafe.Add(ref reference3, 1), mask2);
				Unsafe.Add(ref reference4, 2) = Ssse3.Shuffle(Unsafe.Add(ref reference3, 2), mask2);
				Unsafe.Add(ref reference4, 3) = Ssse3.Shuffle(Unsafe.Add(ref reference3, 3), mask2);
			}
			if (num7 > 0)
			{
				for (nint num10 = num8; num10 < num6; num10++)
				{
					Unsafe.Add(ref source5, num10) = Ssse3.Shuffle(Unsafe.Add(ref source4, num10), mask2);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Shuffle3(ReadOnlySpan<byte> source, Span<byte> dest, byte control)
		{
			if (Ssse3.IsSupported)
			{
				Vector128<byte> mask = ShuffleMaskPad4Nx16();
				Vector128<byte> vector = ShuffleMaskSlice4Nx16();
				Vector128<byte> mask2 = Ssse3.AlignRight(vector, vector, 12);
				Span<byte> span = stackalloc byte[Vector128<byte>.Count];
				Shuffle.MMShuffleSpan(ref span, control);
				Vector128<byte> mask3 = Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(span));
				ref Vector128<byte> source2 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(source));
				ref Vector128<byte> source3 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(dest));
				nuint num = source.Vector128Count<byte>();
				for (nuint num2 = 0u; num2 < num; num2 += 3)
				{
					ref Vector128<byte> reference = ref Unsafe.Add(ref source2, num2);
					Vector128<byte> vector2 = reference;
					Vector128<byte> vector3 = Unsafe.Add(ref reference, 1);
					Vector128<byte> vector4 = Unsafe.Add(ref reference, 2);
					Vector128<byte> value = Sse2.ShiftRightLogical128BitLane(vector4, 4);
					vector4 = Ssse3.AlignRight(vector4, vector3, 8);
					vector3 = Ssse3.AlignRight(vector3, vector2, 12);
					vector2 = Ssse3.Shuffle(Ssse3.Shuffle(vector2, mask), mask3);
					vector3 = Ssse3.Shuffle(Ssse3.Shuffle(vector3, mask), mask3);
					vector4 = Ssse3.Shuffle(Ssse3.Shuffle(vector4, mask), mask3);
					value = Ssse3.Shuffle(Ssse3.Shuffle(value, mask), mask3);
					vector2 = Ssse3.Shuffle(vector2, mask2);
					vector3 = Ssse3.Shuffle(vector3, vector);
					vector4 = Ssse3.Shuffle(vector4, mask2);
					value = Ssse3.Shuffle(value, vector);
					vector2 = Ssse3.AlignRight(vector3, vector2, 4);
					value = Ssse3.AlignRight(value, vector4, 12);
					vector3 = Sse2.ShiftLeftLogical128BitLane(vector3, 4);
					vector4 = Sse2.ShiftRightLogical128BitLane(vector4, 4);
					vector3 = Ssse3.AlignRight(vector4, vector3, 8);
					ref Vector128<byte> reference2 = ref Unsafe.Add(ref source3, num2);
					reference2 = vector2;
					Unsafe.Add(ref reference2, 1) = vector3;
					Unsafe.Add(ref reference2, 2) = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Pad3Shuffle4(ReadOnlySpan<byte> source, Span<byte> dest, byte control)
		{
			if (Ssse3.IsSupported)
			{
				Vector128<byte> mask = ShuffleMaskPad4Nx16();
				Vector128<byte> right = Vector128.Create(18374686483949813760uL).AsByte();
				Span<byte> span = stackalloc byte[Vector128<byte>.Count];
				Shuffle.MMShuffleSpan(ref span, control);
				Vector128<byte> mask2 = Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(span));
				ref Vector128<byte> source2 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(source));
				ref Vector128<byte> source3 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(dest));
				nuint num = source.Vector128Count<byte>();
				nuint num2 = 0u;
				nuint num3 = 0u;
				while (num2 < num)
				{
					ref Vector128<byte> reference = ref Unsafe.Add(ref source2, num2);
					Vector128<byte> vector = Unsafe.Add(ref reference, 1);
					Vector128<byte> vector2 = Unsafe.Add(ref reference, 2);
					Vector128<byte> value = Sse2.ShiftRightLogical128BitLane(vector2, 4);
					vector2 = Ssse3.AlignRight(vector2, vector, 8);
					vector = Ssse3.AlignRight(vector, reference, 12);
					ref Vector128<byte> reference2 = ref Unsafe.Add(ref source3, num3);
					reference2 = Ssse3.Shuffle(Sse2.Or(Ssse3.Shuffle(reference, mask), right), mask2);
					Unsafe.Add(ref reference2, 1) = Ssse3.Shuffle(Sse2.Or(Ssse3.Shuffle(vector, mask), right), mask2);
					Unsafe.Add(ref reference2, 2) = Ssse3.Shuffle(Sse2.Or(Ssse3.Shuffle(vector2, mask), right), mask2);
					Unsafe.Add(ref reference2, 3) = Ssse3.Shuffle(Sse2.Or(Ssse3.Shuffle(value, mask), right), mask2);
					num2 += 3;
					num3 += 4;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Shuffle4Slice3(ReadOnlySpan<byte> source, Span<byte> dest, byte control)
		{
			if (Ssse3.IsSupported)
			{
				Vector128<byte> vector = ShuffleMaskSlice4Nx16();
				Vector128<byte> mask = Ssse3.AlignRight(vector, vector, 12);
				Span<byte> span = stackalloc byte[Vector128<byte>.Count];
				Shuffle.MMShuffleSpan(ref span, control);
				Vector128<byte> mask2 = Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(span));
				ref Vector128<byte> source2 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(source));
				ref Vector128<byte> source3 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(dest));
				nuint num = source.Vector128Count<byte>();
				nuint num2 = 0u;
				nuint num3 = 0u;
				while (num2 < num)
				{
					ref Vector128<byte> reference = ref Unsafe.Add(ref source2, num2);
					Vector128<byte> value = reference;
					Vector128<byte> value2 = Unsafe.Add(ref reference, 1);
					Vector128<byte> value3 = Unsafe.Add(ref reference, 2);
					Vector128<byte> value4 = Unsafe.Add(ref reference, 3);
					value = Ssse3.Shuffle(Ssse3.Shuffle(value, mask2), mask);
					value2 = Ssse3.Shuffle(Ssse3.Shuffle(value2, mask2), vector);
					value3 = Ssse3.Shuffle(Ssse3.Shuffle(value3, mask2), mask);
					value4 = Ssse3.Shuffle(Ssse3.Shuffle(value4, mask2), vector);
					value = Ssse3.AlignRight(value2, value, 4);
					value4 = Ssse3.AlignRight(value4, value3, 12);
					value2 = Sse2.ShiftLeftLogical128BitLane(value2, 4);
					value3 = Sse2.ShiftRightLogical128BitLane(value3, 4);
					value2 = Ssse3.AlignRight(value3, value2, 8);
					ref Vector128<byte> reference2 = ref Unsafe.Add(ref source3, num3);
					reference2 = value;
					Unsafe.Add(ref reference2, 1) = value2;
					Unsafe.Add(ref reference2, 2) = value4;
					num2 += 4;
					num3 += 3;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<float> MultiplyAdd(Vector256<float> va, Vector256<float> vm0, Vector256<float> vm1)
		{
			if (Fma.IsSupported)
			{
				return Fma.MultiplyAdd(vm1, vm0, va);
			}
			return Avx.Add(Avx.Multiply(vm0, vm1), va);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector128<float> MultiplyAdd(Vector128<float> va, Vector128<float> vm0, Vector128<float> vm1)
		{
			if (Fma.IsSupported)
			{
				return Fma.MultiplyAdd(vm1, vm0, va);
			}
			if (AdvSimd.IsSupported)
			{
				return AdvSimd.Add(AdvSimd.Multiply(vm0, vm1), va);
			}
			return Sse.Add(Sse.Multiply(vm0, vm1), va);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<float> MultiplySubtract(Vector256<float> vs, Vector256<float> vm0, Vector256<float> vm1)
		{
			if (Fma.IsSupported)
			{
				return Fma.MultiplySubtract(vm1, vm0, vs);
			}
			return Avx.Subtract(Avx.Multiply(vm0, vm1), vs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<float> MultiplyAddNegated(Vector256<float> a, Vector256<float> b, Vector256<float> c)
		{
			if (Fma.IsSupported)
			{
				return Fma.MultiplyAddNegated(a, b, c);
			}
			return Avx.Subtract(c, Avx.Multiply(a, b));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector128<byte> BlendVariable(Vector128<byte> left, Vector128<byte> right, Vector128<byte> mask)
		{
			if (Sse41.IsSupported)
			{
				return Sse41.BlendVariable(left, right, mask);
			}
			if (Sse2.IsSupported)
			{
				return Sse2.Or(Sse2.And(right, mask), Sse2.AndNot(mask, left));
			}
			return AdvSimd.BitwiseSelect(AdvSimd.ShiftRightArithmetic(mask.AsInt16(), 7), right.AsInt16(), left.AsInt16()).AsByte();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector128<uint> BlendVariable(Vector128<uint> left, Vector128<uint> right, Vector128<uint> mask)
		{
			return BlendVariable(left.AsByte(), right.AsByte(), mask.AsByte()).AsUInt32();
		}

		public static ushort LeadingZeroCount(ushort value)
		{
			return (ushort)(BitOperations.LeadingZeroCount(value) - 16);
		}

		public static ushort TrailingZeroCount(ushort value)
		{
			return (ushort)(BitOperations.TrailingZeroCount(value << 16) - 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ByteToNormalizedFloatReduce(ref ReadOnlySpan<byte> source, ref Span<float> dest)
		{
			if (Avx2.IsSupported || Sse2.IsSupported)
			{
				int num = ((!Avx2.IsSupported) ? Numerics.ModuloP2(source.Length, Vector128<byte>.Count) : Numerics.ModuloP2(source.Length, Vector256<byte>.Count));
				int num2 = source.Length - num;
				if (num2 > 0)
				{
					ByteToNormalizedFloat(source.Slice(0, num2), dest.Slice(0, num2));
					int num3 = num2;
					source = source.Slice(num3, source.Length - num3);
					num3 = num2;
					dest = dest.Slice(num3, dest.Length - num3);
				}
			}
		}

		internal unsafe static void ByteToNormalizedFloat(ReadOnlySpan<byte> source, Span<float> dest)
		{
			fixed (byte* ptr = source)
			{
				if (Avx2.IsSupported)
				{
					nuint num = dest.Vector256Count<byte>();
					ref Vector256<float> source2 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(dest));
					Vector256<float> left = Vector256.Create(0.003921569f);
					for (nuint num2 = 0u; num2 < num; num2++)
					{
						nuint num3 = (uint)Vector256<byte>.Count * num2;
						Vector256<int> value = Avx2.ConvertToVector256Int32(ptr + num3);
						Vector256<int> value2 = Avx2.ConvertToVector256Int32(ptr + num3 + Vector256<int>.Count);
						Vector256<int> value3 = Avx2.ConvertToVector256Int32(ptr + num3 + Vector256<int>.Count * 2);
						Vector256<int> value4 = Avx2.ConvertToVector256Int32(ptr + num3 + Vector256<int>.Count * 3);
						Vector256<float> vector = Avx.Multiply(left, Avx.ConvertToVector256Single(value));
						Vector256<float> vector2 = Avx.Multiply(left, Avx.ConvertToVector256Single(value2));
						Vector256<float> vector3 = Avx.Multiply(left, Avx.ConvertToVector256Single(value3));
						Vector256<float> vector4 = Avx.Multiply(left, Avx.ConvertToVector256Single(value4));
						ref Vector256<float> reference = ref Unsafe.Add(ref source2, num2 * 4);
						reference = vector;
						Unsafe.Add(ref reference, 1) = vector2;
						Unsafe.Add(ref reference, 2) = vector3;
						Unsafe.Add(ref reference, 3) = vector4;
					}
					return;
				}
				nuint num4 = dest.Vector128Count<byte>();
				ref Vector128<float> source3 = ref Unsafe.As<float, Vector128<float>>(ref MemoryMarshal.GetReference(dest));
				Vector128<float> left2 = Vector128.Create(0.003921569f);
				Vector128<byte> zero = Vector128<byte>.Zero;
				for (nuint num5 = 0u; num5 < num4; num5++)
				{
					nuint num6 = (uint)Vector128<byte>.Count * num5;
					Vector128<int> value5;
					Vector128<int> value6;
					Vector128<int> value7;
					Vector128<int> value8;
					if (Sse41.IsSupported)
					{
						value5 = Sse41.ConvertToVector128Int32(ptr + num6);
						value6 = Sse41.ConvertToVector128Int32(ptr + num6 + Vector128<int>.Count);
						value7 = Sse41.ConvertToVector128Int32(ptr + num6 + Vector128<int>.Count * 2);
						value8 = Sse41.ConvertToVector128Int32(ptr + num6 + Vector128<int>.Count * 3);
					}
					else
					{
						Vector128<byte> left3 = Sse2.LoadVector128(ptr + num6);
						Vector128<short> left4 = Sse2.UnpackLow(left3, zero).AsInt16();
						Vector128<short> left5 = Sse2.UnpackHigh(left3, zero).AsInt16();
						value5 = Sse2.UnpackLow(left4, zero.AsInt16()).AsInt32();
						value6 = Sse2.UnpackHigh(left4, zero.AsInt16()).AsInt32();
						value7 = Sse2.UnpackLow(left5, zero.AsInt16()).AsInt32();
						value8 = Sse2.UnpackHigh(left5, zero.AsInt16()).AsInt32();
					}
					Vector128<float> vector5 = Sse.Multiply(left2, Sse2.ConvertToVector128Single(value5));
					Vector128<float> vector6 = Sse.Multiply(left2, Sse2.ConvertToVector128Single(value6));
					Vector128<float> vector7 = Sse.Multiply(left2, Sse2.ConvertToVector128Single(value7));
					Vector128<float> vector8 = Sse.Multiply(left2, Sse2.ConvertToVector128Single(value8));
					ref Vector128<float> reference2 = ref Unsafe.Add(ref source3, num5 * 4);
					reference2 = vector5;
					Unsafe.Add(ref reference2, 1) = vector6;
					Unsafe.Add(ref reference2, 2) = vector7;
					Unsafe.Add(ref reference2, 3) = vector8;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void NormalizedFloatToByteSaturateReduce(ref ReadOnlySpan<float> source, ref Span<byte> dest)
		{
			if (Avx2.IsSupported || Sse2.IsSupported)
			{
				int num = ((!Avx2.IsSupported) ? Numerics.ModuloP2(source.Length, Vector128<byte>.Count) : Numerics.ModuloP2(source.Length, Vector256<byte>.Count));
				int num2 = source.Length - num;
				if (num2 > 0)
				{
					NormalizedFloatToByteSaturate(source.Slice(0, num2), dest.Slice(0, num2));
					int num3 = num2;
					source = source.Slice(num3, source.Length - num3);
					num3 = num2;
					dest = dest.Slice(num3, dest.Length - num3);
				}
			}
		}

		internal static void NormalizedFloatToByteSaturate(ReadOnlySpan<float> source, Span<byte> dest)
		{
			if (Avx2.IsSupported)
			{
				nuint num = dest.Vector256Count<byte>();
				ref Vector256<float> source2 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(source));
				ref Vector256<byte> source3 = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(dest));
				Vector256<float> left = Vector256.Create(255f);
				Vector256<int> control = PermuteMaskDeinterleave8x32();
				for (nuint num2 = 0u; num2 < num; num2++)
				{
					ref Vector256<float> reference = ref Unsafe.Add(ref source2, num2 * 4);
					Vector256<float> value = Avx.Multiply(left, reference);
					Vector256<float> value2 = Avx.Multiply(left, Unsafe.Add(ref reference, 1));
					Vector256<float> value3 = Avx.Multiply(left, Unsafe.Add(ref reference, 2));
					Vector256<float> value4 = Avx.Multiply(left, Unsafe.Add(ref reference, 3));
					Vector256<int> left2 = Avx.ConvertToVector256Int32(value);
					Vector256<int> right = Avx.ConvertToVector256Int32(value2);
					Vector256<int> left3 = Avx.ConvertToVector256Int32(value3);
					Vector256<int> right2 = Avx.ConvertToVector256Int32(value4);
					Vector256<short> left4 = Avx2.PackSignedSaturate(left2, right);
					Vector256<short> right3 = Avx2.PackSignedSaturate(left3, right2);
					Vector256<byte> vector = Avx2.PackUnsignedSaturate(left4, right3);
					vector = Avx2.PermuteVar8x32(vector.AsInt32(), control).AsByte();
					Unsafe.Add(ref source3, num2) = vector;
				}
			}
			else
			{
				nuint num3 = dest.Vector128Count<byte>();
				ref Vector128<float> source4 = ref Unsafe.As<float, Vector128<float>>(ref MemoryMarshal.GetReference(source));
				ref Vector128<byte> source5 = ref Unsafe.As<byte, Vector128<byte>>(ref MemoryMarshal.GetReference(dest));
				Vector128<float> left5 = Vector128.Create(255f);
				for (nuint num4 = 0u; num4 < num3; num4++)
				{
					ref Vector128<float> reference2 = ref Unsafe.Add(ref source4, num4 * 4);
					Vector128<float> value5 = Sse.Multiply(left5, reference2);
					Vector128<float> value6 = Sse.Multiply(left5, Unsafe.Add(ref reference2, 1));
					Vector128<float> value7 = Sse.Multiply(left5, Unsafe.Add(ref reference2, 2));
					Vector128<float> value8 = Sse.Multiply(left5, Unsafe.Add(ref reference2, 3));
					Vector128<int> left6 = Sse2.ConvertToVector128Int32(value5);
					Vector128<int> right4 = Sse2.ConvertToVector128Int32(value6);
					Vector128<int> left7 = Sse2.ConvertToVector128Int32(value7);
					Vector128<int> right5 = Sse2.ConvertToVector128Int32(value8);
					Vector128<short> left8 = Sse2.PackSignedSaturate(left6, right4);
					Vector128<short> right6 = Sse2.PackSignedSaturate(left7, right5);
					Unsafe.Add(ref source5, num4) = Sse2.PackUnsignedSaturate(left8, right6);
				}
			}
		}

		internal static void PackFromRgbPlanesAvx2Reduce(ref ReadOnlySpan<byte> redChannel, ref ReadOnlySpan<byte> greenChannel, ref ReadOnlySpan<byte> blueChannel, ref Span<Rgb24> destination)
		{
			ref Vector256<byte> source = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(redChannel));
			ref Vector256<byte> source2 = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(greenChannel));
			ref Vector256<byte> source3 = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(blueChannel));
			ref byte source4 = ref Unsafe.As<Rgb24, byte>(ref MemoryMarshal.GetReference(destination));
			nuint num = redChannel.Vector256Count<byte>();
			Vector256<uint> control = PermuteMaskEvenOdd8x32();
			Vector256<uint> control2 = PermuteMaskShiftAlpha8x32();
			Vector256<byte> right = Vector256.Create(byte.MaxValue);
			Vector256<byte> mask = ShuffleMaskShiftAlpha();
			for (nuint num2 = 0u; num2 < num; num2++)
			{
				Vector256<byte> vector = Unsafe.Add(ref source, num2);
				Vector256<byte> vector2 = Unsafe.Add(ref source2, num2);
				Vector256<byte> vector3 = Unsafe.Add(ref source3, num2);
				vector = Avx2.PermuteVar8x32(vector.AsUInt32(), control).AsByte();
				vector2 = Avx2.PermuteVar8x32(vector2.AsUInt32(), control).AsByte();
				vector3 = Avx2.PermuteVar8x32(vector3.AsUInt32(), control).AsByte();
				Vector256<byte> vector4 = Avx2.UnpackLow(vector, vector2);
				Vector256<byte> vector5 = Avx2.UnpackLow(vector3, right);
				Vector256<byte> value = Avx2.UnpackLow(vector4.AsUInt16(), vector5.AsUInt16()).AsByte();
				Vector256<byte> value2 = Avx2.UnpackHigh(vector4.AsUInt16(), vector5.AsUInt16()).AsByte();
				vector4 = Avx2.UnpackHigh(vector, vector2);
				vector5 = Avx2.UnpackHigh(vector3, right);
				Vector256<byte> value3 = Avx2.UnpackLow(vector4.AsUInt16(), vector5.AsUInt16()).AsByte();
				Vector256<byte> value4 = Avx2.UnpackHigh(vector4.AsUInt16(), vector5.AsUInt16()).AsByte();
				value = Avx2.Shuffle(value, mask);
				value2 = Avx2.Shuffle(value2, mask);
				value3 = Avx2.Shuffle(value3, mask);
				value4 = Avx2.Shuffle(value4, mask);
				value = Avx2.PermuteVar8x32(value.AsUInt32(), control2).AsByte();
				value2 = Avx2.PermuteVar8x32(value2.AsUInt32(), control2).AsByte();
				value3 = Avx2.PermuteVar8x32(value3.AsUInt32(), control2).AsByte();
				value4 = Avx2.PermuteVar8x32(value4.AsUInt32(), control2).AsByte();
				ref byte source5 = ref Unsafe.Add(ref source4, 96 * num2);
				ref byte source6 = ref Unsafe.Add(ref source5, 24);
				ref byte source7 = ref Unsafe.Add(ref source6, 24);
				ref byte source8 = ref Unsafe.Add(ref source7, 24);
				Unsafe.As<byte, Vector256<byte>>(ref source5) = value;
				Unsafe.As<byte, Vector256<byte>>(ref source6) = value2;
				Unsafe.As<byte, Vector256<byte>>(ref source7) = value3;
				Unsafe.As<byte, Vector256<byte>>(ref source8) = value4;
			}
			int num3 = (int)num * Vector256<byte>.Count;
			ref ReadOnlySpan<byte> reference = ref redChannel;
			int num4 = num3;
			redChannel = reference.Slice(num4, reference.Length - num4);
			reference = ref greenChannel;
			num4 = num3;
			greenChannel = reference.Slice(num4, reference.Length - num4);
			reference = ref blueChannel;
			num4 = num3;
			blueChannel = reference.Slice(num4, reference.Length - num4);
			num4 = num3;
			destination = destination.Slice(num4, destination.Length - num4);
		}

		internal static void PackFromRgbPlanesAvx2Reduce(ref ReadOnlySpan<byte> redChannel, ref ReadOnlySpan<byte> greenChannel, ref ReadOnlySpan<byte> blueChannel, ref Span<Rgba32> destination)
		{
			ref Vector256<byte> source = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(redChannel));
			ref Vector256<byte> source2 = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(greenChannel));
			ref Vector256<byte> source3 = ref Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(blueChannel));
			ref Vector256<byte> source4 = ref Unsafe.As<Rgba32, Vector256<byte>>(ref MemoryMarshal.GetReference(destination));
			nuint num = redChannel.Vector256Count<byte>();
			Vector256<uint> control = PermuteMaskEvenOdd8x32();
			Vector256<byte> right = Vector256.Create(byte.MaxValue);
			for (nuint num2 = 0u; num2 < num; num2++)
			{
				Vector256<byte> vector = Unsafe.Add(ref source, num2);
				Vector256<byte> vector2 = Unsafe.Add(ref source2, num2);
				Vector256<byte> vector3 = Unsafe.Add(ref source3, num2);
				vector = Avx2.PermuteVar8x32(vector.AsUInt32(), control).AsByte();
				vector2 = Avx2.PermuteVar8x32(vector2.AsUInt32(), control).AsByte();
				vector3 = Avx2.PermuteVar8x32(vector3.AsUInt32(), control).AsByte();
				Vector256<byte> vector4 = Avx2.UnpackLow(vector, vector2);
				Vector256<byte> vector5 = Avx2.UnpackLow(vector3, right);
				Vector256<byte> vector6 = Avx2.UnpackLow(vector4.AsUInt16(), vector5.AsUInt16()).AsByte();
				Vector256<byte> vector7 = Avx2.UnpackHigh(vector4.AsUInt16(), vector5.AsUInt16()).AsByte();
				vector4 = Avx2.UnpackHigh(vector, vector2);
				vector5 = Avx2.UnpackHigh(vector3, right);
				Vector256<byte> vector8 = Avx2.UnpackLow(vector4.AsUInt16(), vector5.AsUInt16()).AsByte();
				Vector256<byte> vector9 = Avx2.UnpackHigh(vector4.AsUInt16(), vector5.AsUInt16()).AsByte();
				ref Vector256<byte> reference = ref Unsafe.Add(ref source4, num2 * 4);
				reference = vector6;
				Unsafe.Add(ref reference, 1) = vector7;
				Unsafe.Add(ref reference, 2) = vector8;
				Unsafe.Add(ref reference, 3) = vector9;
			}
			int num3 = (int)num * Vector256<byte>.Count;
			ref ReadOnlySpan<byte> reference2 = ref redChannel;
			int num4 = num3;
			redChannel = reference2.Slice(num4, reference2.Length - num4);
			reference2 = ref greenChannel;
			num4 = num3;
			greenChannel = reference2.Slice(num4, reference2.Length - num4);
			reference2 = ref blueChannel;
			num4 = num3;
			blueChannel = reference2.Slice(num4, reference2.Length - num4);
			num4 = num3;
			destination = destination.Slice(num4, destination.Length - num4);
		}

		internal static void UnpackToRgbPlanesAvx2Reduce(ref Span<float> redChannel, ref Span<float> greenChannel, ref Span<float> blueChannel, ref ReadOnlySpan<Rgb24> source)
		{
			ref Vector256<byte> source2 = ref Unsafe.As<Rgb24, Vector256<byte>>(ref MemoryMarshal.GetReference(source));
			ref Vector256<float> source3 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(redChannel));
			ref Vector256<float> source4 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(greenChannel));
			ref Vector256<float> source5 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(blueChannel));
			Vector256<uint> control = MoveFirst24BytesToSeparateLanes();
			Vector256<byte> mask = ExtractRgb();
			nuint num = (uint)source.Length / 8u;
			for (nuint num2 = 0u; num2 < num; num2++)
			{
				Vector256<byte> left = Avx2.Shuffle(Avx2.PermuteVar8x32(Unsafe.AddByteOffset(ref source2, (uint)(24 * num2)).AsUInt32(), control).AsByte(), mask);
				Vector256<byte> left2 = Avx2.UnpackLow(left, Vector256<byte>.Zero);
				Vector256<byte> left3 = Avx2.UnpackHigh(left, Vector256<byte>.Zero);
				Vector256<float> vector = Avx.ConvertToVector256Single(Avx2.UnpackLow(left2, Vector256<byte>.Zero).AsInt32());
				Vector256<float> vector2 = Avx.ConvertToVector256Single(Avx2.UnpackHigh(left2, Vector256<byte>.Zero).AsInt32());
				Vector256<float> vector3 = Avx.ConvertToVector256Single(Avx2.UnpackLow(left3, Vector256<byte>.Zero).AsInt32());
				Unsafe.Add(ref source3, num2) = vector;
				Unsafe.Add(ref source4, num2) = vector2;
				Unsafe.Add(ref source5, num2) = vector3;
			}
			int start = (int)(num * 8);
			redChannel = redChannel.Slice(start);
			greenChannel = greenChannel.Slice(start);
			blueChannel = blueChannel.Slice(start);
			source = source.Slice(start);
		}
	}

	public static class Shuffle
	{
		public const byte MMShuffle0000 = 0;

		public const byte MMShuffle0001 = 1;

		public const byte MMShuffle0002 = 2;

		public const byte MMShuffle0003 = 3;

		public const byte MMShuffle0010 = 4;

		public const byte MMShuffle0011 = 5;

		public const byte MMShuffle0012 = 6;

		public const byte MMShuffle0013 = 7;

		public const byte MMShuffle0020 = 8;

		public const byte MMShuffle0021 = 9;

		public const byte MMShuffle0022 = 10;

		public const byte MMShuffle0023 = 11;

		public const byte MMShuffle0030 = 12;

		public const byte MMShuffle0031 = 13;

		public const byte MMShuffle0032 = 14;

		public const byte MMShuffle0033 = 15;

		public const byte MMShuffle0100 = 16;

		public const byte MMShuffle0101 = 17;

		public const byte MMShuffle0102 = 18;

		public const byte MMShuffle0103 = 19;

		public const byte MMShuffle0110 = 20;

		public const byte MMShuffle0111 = 21;

		public const byte MMShuffle0112 = 22;

		public const byte MMShuffle0113 = 23;

		public const byte MMShuffle0120 = 24;

		public const byte MMShuffle0121 = 25;

		public const byte MMShuffle0122 = 26;

		public const byte MMShuffle0123 = 27;

		public const byte MMShuffle0130 = 28;

		public const byte MMShuffle0131 = 29;

		public const byte MMShuffle0132 = 30;

		public const byte MMShuffle0133 = 31;

		public const byte MMShuffle0200 = 32;

		public const byte MMShuffle0201 = 33;

		public const byte MMShuffle0202 = 34;

		public const byte MMShuffle0203 = 35;

		public const byte MMShuffle0210 = 36;

		public const byte MMShuffle0211 = 37;

		public const byte MMShuffle0212 = 38;

		public const byte MMShuffle0213 = 39;

		public const byte MMShuffle0220 = 40;

		public const byte MMShuffle0221 = 41;

		public const byte MMShuffle0222 = 42;

		public const byte MMShuffle0223 = 43;

		public const byte MMShuffle0230 = 44;

		public const byte MMShuffle0231 = 45;

		public const byte MMShuffle0232 = 46;

		public const byte MMShuffle0233 = 47;

		public const byte MMShuffle0300 = 48;

		public const byte MMShuffle0301 = 49;

		public const byte MMShuffle0302 = 50;

		public const byte MMShuffle0303 = 51;

		public const byte MMShuffle0310 = 52;

		public const byte MMShuffle0311 = 53;

		public const byte MMShuffle0312 = 54;

		public const byte MMShuffle0313 = 55;

		public const byte MMShuffle0320 = 56;

		public const byte MMShuffle0321 = 57;

		public const byte MMShuffle0322 = 58;

		public const byte MMShuffle0323 = 59;

		public const byte MMShuffle0330 = 60;

		public const byte MMShuffle0331 = 61;

		public const byte MMShuffle0332 = 62;

		public const byte MMShuffle0333 = 63;

		public const byte MMShuffle1000 = 64;

		public const byte MMShuffle1001 = 65;

		public const byte MMShuffle1002 = 66;

		public const byte MMShuffle1003 = 67;

		public const byte MMShuffle1010 = 68;

		public const byte MMShuffle1011 = 69;

		public const byte MMShuffle1012 = 70;

		public const byte MMShuffle1013 = 71;

		public const byte MMShuffle1020 = 72;

		public const byte MMShuffle1021 = 73;

		public const byte MMShuffle1022 = 74;

		public const byte MMShuffle1023 = 75;

		public const byte MMShuffle1030 = 76;

		public const byte MMShuffle1031 = 77;

		public const byte MMShuffle1032 = 78;

		public const byte MMShuffle1033 = 79;

		public const byte MMShuffle1100 = 80;

		public const byte MMShuffle1101 = 81;

		public const byte MMShuffle1102 = 82;

		public const byte MMShuffle1103 = 83;

		public const byte MMShuffle1110 = 84;

		public const byte MMShuffle1111 = 85;

		public const byte MMShuffle1112 = 86;

		public const byte MMShuffle1113 = 87;

		public const byte MMShuffle1120 = 88;

		public const byte MMShuffle1121 = 89;

		public const byte MMShuffle1122 = 90;

		public const byte MMShuffle1123 = 91;

		public const byte MMShuffle1130 = 92;

		public const byte MMShuffle1131 = 93;

		public const byte MMShuffle1132 = 94;

		public const byte MMShuffle1133 = 95;

		public const byte MMShuffle1200 = 96;

		public const byte MMShuffle1201 = 97;

		public const byte MMShuffle1202 = 98;

		public const byte MMShuffle1203 = 99;

		public const byte MMShuffle1210 = 100;

		public const byte MMShuffle1211 = 101;

		public const byte MMShuffle1212 = 102;

		public const byte MMShuffle1213 = 103;

		public const byte MMShuffle1220 = 104;

		public const byte MMShuffle1221 = 105;

		public const byte MMShuffle1222 = 106;

		public const byte MMShuffle1223 = 107;

		public const byte MMShuffle1230 = 108;

		public const byte MMShuffle1231 = 109;

		public const byte MMShuffle1232 = 110;

		public const byte MMShuffle1233 = 111;

		public const byte MMShuffle1300 = 112;

		public const byte MMShuffle1301 = 113;

		public const byte MMShuffle1302 = 114;

		public const byte MMShuffle1303 = 115;

		public const byte MMShuffle1310 = 116;

		public const byte MMShuffle1311 = 117;

		public const byte MMShuffle1312 = 118;

		public const byte MMShuffle1313 = 119;

		public const byte MMShuffle1320 = 120;

		public const byte MMShuffle1321 = 121;

		public const byte MMShuffle1322 = 122;

		public const byte MMShuffle1323 = 123;

		public const byte MMShuffle1330 = 124;

		public const byte MMShuffle1331 = 125;

		public const byte MMShuffle1332 = 126;

		public const byte MMShuffle1333 = 127;

		public const byte MMShuffle2000 = 128;

		public const byte MMShuffle2001 = 129;

		public const byte MMShuffle2002 = 130;

		public const byte MMShuffle2003 = 131;

		public const byte MMShuffle2010 = 132;

		public const byte MMShuffle2011 = 133;

		public const byte MMShuffle2012 = 134;

		public const byte MMShuffle2013 = 135;

		public const byte MMShuffle2020 = 136;

		public const byte MMShuffle2021 = 137;

		public const byte MMShuffle2022 = 138;

		public const byte MMShuffle2023 = 139;

		public const byte MMShuffle2030 = 140;

		public const byte MMShuffle2031 = 141;

		public const byte MMShuffle2032 = 142;

		public const byte MMShuffle2033 = 143;

		public const byte MMShuffle2100 = 144;

		public const byte MMShuffle2101 = 145;

		public const byte MMShuffle2102 = 146;

		public const byte MMShuffle2103 = 147;

		public const byte MMShuffle2110 = 148;

		public const byte MMShuffle2111 = 149;

		public const byte MMShuffle2112 = 150;

		public const byte MMShuffle2113 = 151;

		public const byte MMShuffle2120 = 152;

		public const byte MMShuffle2121 = 153;

		public const byte MMShuffle2122 = 154;

		public const byte MMShuffle2123 = 155;

		public const byte MMShuffle2130 = 156;

		public const byte MMShuffle2131 = 157;

		public const byte MMShuffle2132 = 158;

		public const byte MMShuffle2133 = 159;

		public const byte MMShuffle2200 = 160;

		public const byte MMShuffle2201 = 161;

		public const byte MMShuffle2202 = 162;

		public const byte MMShuffle2203 = 163;

		public const byte MMShuffle2210 = 164;

		public const byte MMShuffle2211 = 165;

		public const byte MMShuffle2212 = 166;

		public const byte MMShuffle2213 = 167;

		public const byte MMShuffle2220 = 168;

		public const byte MMShuffle2221 = 169;

		public const byte MMShuffle2222 = 170;

		public const byte MMShuffle2223 = 171;

		public const byte MMShuffle2230 = 172;

		public const byte MMShuffle2231 = 173;

		public const byte MMShuffle2232 = 174;

		public const byte MMShuffle2233 = 175;

		public const byte MMShuffle2300 = 176;

		public const byte MMShuffle2301 = 177;

		public const byte MMShuffle2302 = 178;

		public const byte MMShuffle2303 = 179;

		public const byte MMShuffle2310 = 180;

		public const byte MMShuffle2311 = 181;

		public const byte MMShuffle2312 = 182;

		public const byte MMShuffle2313 = 183;

		public const byte MMShuffle2320 = 184;

		public const byte MMShuffle2321 = 185;

		public const byte MMShuffle2322 = 186;

		public const byte MMShuffle2323 = 187;

		public const byte MMShuffle2330 = 188;

		public const byte MMShuffle2331 = 189;

		public const byte MMShuffle2332 = 190;

		public const byte MMShuffle2333 = 191;

		public const byte MMShuffle3000 = 192;

		public const byte MMShuffle3001 = 193;

		public const byte MMShuffle3002 = 194;

		public const byte MMShuffle3003 = 195;

		public const byte MMShuffle3010 = 196;

		public const byte MMShuffle3011 = 197;

		public const byte MMShuffle3012 = 198;

		public const byte MMShuffle3013 = 199;

		public const byte MMShuffle3020 = 200;

		public const byte MMShuffle3021 = 201;

		public const byte MMShuffle3022 = 202;

		public const byte MMShuffle3023 = 203;

		public const byte MMShuffle3030 = 204;

		public const byte MMShuffle3031 = 205;

		public const byte MMShuffle3032 = 206;

		public const byte MMShuffle3033 = 207;

		public const byte MMShuffle3100 = 208;

		public const byte MMShuffle3101 = 209;

		public const byte MMShuffle3102 = 210;

		public const byte MMShuffle3103 = 211;

		public const byte MMShuffle3110 = 212;

		public const byte MMShuffle3111 = 213;

		public const byte MMShuffle3112 = 214;

		public const byte MMShuffle3113 = 215;

		public const byte MMShuffle3120 = 216;

		public const byte MMShuffle3121 = 217;

		public const byte MMShuffle3122 = 218;

		public const byte MMShuffle3123 = 219;

		public const byte MMShuffle3130 = 220;

		public const byte MMShuffle3131 = 221;

		public const byte MMShuffle3132 = 222;

		public const byte MMShuffle3133 = 223;

		public const byte MMShuffle3200 = 224;

		public const byte MMShuffle3201 = 225;

		public const byte MMShuffle3202 = 226;

		public const byte MMShuffle3203 = 227;

		public const byte MMShuffle3210 = 228;

		public const byte MMShuffle3211 = 229;

		public const byte MMShuffle3212 = 230;

		public const byte MMShuffle3213 = 231;

		public const byte MMShuffle3220 = 232;

		public const byte MMShuffle3221 = 233;

		public const byte MMShuffle3222 = 234;

		public const byte MMShuffle3223 = 235;

		public const byte MMShuffle3230 = 236;

		public const byte MMShuffle3231 = 237;

		public const byte MMShuffle3232 = 238;

		public const byte MMShuffle3233 = 239;

		public const byte MMShuffle3300 = 240;

		public const byte MMShuffle3301 = 241;

		public const byte MMShuffle3302 = 242;

		public const byte MMShuffle3303 = 243;

		public const byte MMShuffle3310 = 244;

		public const byte MMShuffle3311 = 245;

		public const byte MMShuffle3312 = 246;

		public const byte MMShuffle3313 = 247;

		public const byte MMShuffle3320 = 248;

		public const byte MMShuffle3321 = 249;

		public const byte MMShuffle3322 = 250;

		public const byte MMShuffle3323 = 251;

		public const byte MMShuffle3330 = 252;

		public const byte MMShuffle3331 = 253;

		public const byte MMShuffle3332 = 254;

		public const byte MMShuffle3333 = byte.MaxValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte MMShuffle(byte p3, byte p2, byte p1, byte p0)
		{
			return (byte)((p3 << 6) | (p2 << 4) | (p1 << 2) | p0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void MMShuffleSpan(ref Span<byte> span, byte control)
		{
			InverseMMShuffle(control, out var p, out var p2, out var p3, out var p4);
			ref byte reference = ref MemoryMarshal.GetReference(span);
			for (nuint num = 0u; num < (uint)span.Length; num += 4)
			{
				Unsafe.Add(ref reference, num + 0) = (byte)(p4 + num);
				Unsafe.Add(ref reference, num + 1) = (byte)(p3 + num);
				Unsafe.Add(ref reference, num + 2) = (byte)(p2 + num);
				Unsafe.Add(ref reference, num + 3) = (byte)(p + num);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InverseMMShuffle(byte control, out uint p3, out uint p2, out uint p1, out uint p0)
		{
			p3 = (uint)((control >> 6) & 3);
			p2 = (uint)((control >> 4) & 3);
			p1 = (uint)((control >> 2) & 3);
			p0 = (uint)(control & 3);
		}
	}

	public static bool HasVector8 { get; } = Vector.IsHardwareAccelerated && Vector<float>.Count == 8 && Vector<int>.Count == 8;

	public static bool HasVector4 { get; } = Vector.IsHardwareAccelerated && Vector<float>.Count == 4;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector4 PseudoRound(this Vector4 v)
	{
		Vector4 vector = Numerics.Clamp(v, new Vector4(-1f), new Vector4(1f));
		return v + vector * 0.5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector<float> FastRound(this Vector<float> v)
	{
		if (Avx2.IsSupported)
		{
			Vector256<float> source = Avx.RoundToNearestInteger(Unsafe.As<Vector<float>, Vector256<float>>(ref v));
			return Unsafe.As<Vector256<float>, Vector<float>>(ref source);
		}
		Vector<float> right = Vector.BitwiseOr(Vector.BitwiseAnd(Vector.AsVectorSingle(new Vector<int>(int.MinValue)), v), new Vector<float>(8388608f));
		return Vector.Subtract(Vector.Add(v, right), right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ByteToNormalizedFloat(ReadOnlySpan<byte> source, Span<float> dest)
	{
		HwIntrinsics.ByteToNormalizedFloatReduce(ref source, ref dest);
		FallbackIntrinsics128.ByteToNormalizedFloatReduce(ref source, ref dest);
		if (source.Length > 0)
		{
			ConvertByteToNormalizedFloatRemainder(source, dest);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void NormalizedFloatToByteSaturate(ReadOnlySpan<float> source, Span<byte> dest)
	{
		HwIntrinsics.NormalizedFloatToByteSaturateReduce(ref source, ref dest);
		FallbackIntrinsics128.NormalizedFloatToByteSaturateReduce(ref source, ref dest);
		if (source.Length > 0)
		{
			ConvertNormalizedFloatToByteRemainder(source, dest);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ConvertByteToNormalizedFloatRemainder(ReadOnlySpan<byte> source, Span<float> dest)
	{
		ref byte reference = ref MemoryMarshal.GetReference(source);
		ref float reference2 = ref MemoryMarshal.GetReference(dest);
		switch (source.Length)
		{
		default:
			return;
		case 3:
			Unsafe.Add(ref reference2, 2) = (float)(int)Unsafe.Add(ref reference, 2) / 255f;
			goto case 2;
		case 2:
			Unsafe.Add(ref reference2, 1) = (float)(int)Unsafe.Add(ref reference, 1) / 255f;
			break;
		case 1:
			break;
		}
		reference2 = (float)(int)reference / 255f;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ConvertNormalizedFloatToByteRemainder(ReadOnlySpan<float> source, Span<byte> dest)
	{
		ref float reference = ref MemoryMarshal.GetReference(source);
		ref byte reference2 = ref MemoryMarshal.GetReference(dest);
		switch (source.Length)
		{
		default:
			return;
		case 3:
			Unsafe.Add(ref reference2, 2) = ConvertToByte(Unsafe.Add(ref reference, 2));
			goto case 2;
		case 2:
			Unsafe.Add(ref reference2, 1) = ConvertToByte(Unsafe.Add(ref reference, 1));
			break;
		case 1:
			break;
		}
		reference2 = ConvertToByte(reference);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte ConvertToByte(float f)
	{
		return (byte)Numerics.Clamp(f * 255f + 0.5f, 0f, 255f);
	}

	[Conditional("DEBUG")]
	private static void VerifyHasVector8(string operation)
	{
		if (!HasVector8)
		{
			throw new NotSupportedException(operation + " is supported only on AVX2 CPU!");
		}
	}

	[Conditional("DEBUG")]
	private static void VerifySpanInput(ReadOnlySpan<byte> source, Span<float> dest, int shouldBeDivisibleBy)
	{
	}

	[Conditional("DEBUG")]
	private static void VerifySpanInput(ReadOnlySpan<float> source, Span<byte> dest, int shouldBeDivisibleBy)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void PackFromRgbPlanes(ReadOnlySpan<byte> redChannel, ReadOnlySpan<byte> greenChannel, ReadOnlySpan<byte> blueChannel, Span<Rgb24> destination)
	{
		if (Avx2.IsSupported)
		{
			HwIntrinsics.PackFromRgbPlanesAvx2Reduce(ref redChannel, ref greenChannel, ref blueChannel, ref destination);
		}
		else
		{
			PackFromRgbPlanesScalarBatchedReduce(ref redChannel, ref greenChannel, ref blueChannel, ref destination);
		}
		PackFromRgbPlanesRemainder(redChannel, greenChannel, blueChannel, destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void PackFromRgbPlanes(ReadOnlySpan<byte> redChannel, ReadOnlySpan<byte> greenChannel, ReadOnlySpan<byte> blueChannel, Span<Rgba32> destination)
	{
		if (Avx2.IsSupported)
		{
			HwIntrinsics.PackFromRgbPlanesAvx2Reduce(ref redChannel, ref greenChannel, ref blueChannel, ref destination);
		}
		else
		{
			PackFromRgbPlanesScalarBatchedReduce(ref redChannel, ref greenChannel, ref blueChannel, ref destination);
		}
		PackFromRgbPlanesRemainder(redChannel, greenChannel, blueChannel, destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UnpackToRgbPlanes(Span<float> redChannel, Span<float> greenChannel, Span<float> blueChannel, ReadOnlySpan<Rgb24> source)
	{
		if (Avx2.IsSupported)
		{
			HwIntrinsics.UnpackToRgbPlanesAvx2Reduce(ref redChannel, ref greenChannel, ref blueChannel, ref source);
		}
		UnpackToRgbPlanesScalar(redChannel, greenChannel, blueChannel, source);
	}

	private static void PackFromRgbPlanesScalarBatchedReduce(ref ReadOnlySpan<byte> redChannel, ref ReadOnlySpan<byte> greenChannel, ref ReadOnlySpan<byte> blueChannel, ref Span<Rgb24> destination)
	{
		ref ByteTuple4 source = ref Unsafe.As<byte, ByteTuple4>(ref MemoryMarshal.GetReference(redChannel));
		ref ByteTuple4 source2 = ref Unsafe.As<byte, ByteTuple4>(ref MemoryMarshal.GetReference(greenChannel));
		ref ByteTuple4 source3 = ref Unsafe.As<byte, ByteTuple4>(ref MemoryMarshal.GetReference(blueChannel));
		ref Rgb24 reference = ref MemoryMarshal.GetReference(destination);
		nuint num = (uint)redChannel.Length / 4u;
		for (nuint num2 = 0u; num2 < num; num2++)
		{
			ref Rgb24 reference2 = ref Unsafe.Add(ref reference, num2 * 4);
			ref Rgb24 reference3 = ref Unsafe.Add(ref reference2, 1);
			ref Rgb24 reference4 = ref Unsafe.Add(ref reference2, 2);
			ref Rgb24 reference5 = ref Unsafe.Add(ref reference2, 3);
			ref ByteTuple4 reference6 = ref Unsafe.Add(ref source, num2);
			ref ByteTuple4 reference7 = ref Unsafe.Add(ref source2, num2);
			ref ByteTuple4 reference8 = ref Unsafe.Add(ref source3, num2);
			reference2.R = reference6.V0;
			reference2.G = reference7.V0;
			reference2.B = reference8.V0;
			reference3.R = reference6.V1;
			reference3.G = reference7.V1;
			reference3.B = reference8.V1;
			reference4.R = reference6.V2;
			reference4.G = reference7.V2;
			reference4.B = reference8.V2;
			reference5.R = reference6.V3;
			reference5.G = reference7.V3;
			reference5.B = reference8.V3;
		}
		int num3 = (int)(num * 4);
		ref ReadOnlySpan<byte> reference9 = ref redChannel;
		int num4 = num3;
		redChannel = reference9.Slice(num4, reference9.Length - num4);
		reference9 = ref greenChannel;
		num4 = num3;
		greenChannel = reference9.Slice(num4, reference9.Length - num4);
		reference9 = ref blueChannel;
		num4 = num3;
		blueChannel = reference9.Slice(num4, reference9.Length - num4);
		num4 = num3;
		destination = destination.Slice(num4, destination.Length - num4);
	}

	private static void PackFromRgbPlanesScalarBatchedReduce(ref ReadOnlySpan<byte> redChannel, ref ReadOnlySpan<byte> greenChannel, ref ReadOnlySpan<byte> blueChannel, ref Span<Rgba32> destination)
	{
		ref ByteTuple4 source = ref Unsafe.As<byte, ByteTuple4>(ref MemoryMarshal.GetReference(redChannel));
		ref ByteTuple4 source2 = ref Unsafe.As<byte, ByteTuple4>(ref MemoryMarshal.GetReference(greenChannel));
		ref ByteTuple4 source3 = ref Unsafe.As<byte, ByteTuple4>(ref MemoryMarshal.GetReference(blueChannel));
		ref Rgba32 reference = ref MemoryMarshal.GetReference(destination);
		nuint num = (uint)redChannel.Length / 4u;
		destination.Fill(new Rgba32(0, 0, 0, byte.MaxValue));
		for (nuint num2 = 0u; num2 < num; num2++)
		{
			ref Rgba32 reference2 = ref Unsafe.Add(ref reference, num2 * 4);
			ref Rgba32 reference3 = ref Unsafe.Add(ref reference2, 1);
			ref Rgba32 reference4 = ref Unsafe.Add(ref reference2, 2);
			ref Rgba32 reference5 = ref Unsafe.Add(ref reference2, 3);
			ref ByteTuple4 reference6 = ref Unsafe.Add(ref source, num2);
			ref ByteTuple4 reference7 = ref Unsafe.Add(ref source2, num2);
			ref ByteTuple4 reference8 = ref Unsafe.Add(ref source3, num2);
			reference2.R = reference6.V0;
			reference2.G = reference7.V0;
			reference2.B = reference8.V0;
			reference3.R = reference6.V1;
			reference3.G = reference7.V1;
			reference3.B = reference8.V1;
			reference4.R = reference6.V2;
			reference4.G = reference7.V2;
			reference4.B = reference8.V2;
			reference5.R = reference6.V3;
			reference5.G = reference7.V3;
			reference5.B = reference8.V3;
		}
		int num3 = (int)(num * 4);
		ref ReadOnlySpan<byte> reference9 = ref redChannel;
		int num4 = num3;
		redChannel = reference9.Slice(num4, reference9.Length - num4);
		reference9 = ref greenChannel;
		num4 = num3;
		greenChannel = reference9.Slice(num4, reference9.Length - num4);
		reference9 = ref blueChannel;
		num4 = num3;
		blueChannel = reference9.Slice(num4, reference9.Length - num4);
		num4 = num3;
		destination = destination.Slice(num4, destination.Length - num4);
	}

	private static void PackFromRgbPlanesRemainder(ReadOnlySpan<byte> redChannel, ReadOnlySpan<byte> greenChannel, ReadOnlySpan<byte> blueChannel, Span<Rgb24> destination)
	{
		ref byte reference = ref MemoryMarshal.GetReference(redChannel);
		ref byte reference2 = ref MemoryMarshal.GetReference(greenChannel);
		ref byte reference3 = ref MemoryMarshal.GetReference(blueChannel);
		ref Rgb24 reference4 = ref MemoryMarshal.GetReference(destination);
		for (nuint num = 0u; num < (uint)destination.Length; num++)
		{
			ref Rgb24 reference5 = ref Unsafe.Add(ref reference4, num);
			reference5.R = Unsafe.Add(ref reference, num);
			reference5.G = Unsafe.Add(ref reference2, num);
			reference5.B = Unsafe.Add(ref reference3, num);
		}
	}

	private static void PackFromRgbPlanesRemainder(ReadOnlySpan<byte> redChannel, ReadOnlySpan<byte> greenChannel, ReadOnlySpan<byte> blueChannel, Span<Rgba32> destination)
	{
		ref byte reference = ref MemoryMarshal.GetReference(redChannel);
		ref byte reference2 = ref MemoryMarshal.GetReference(greenChannel);
		ref byte reference3 = ref MemoryMarshal.GetReference(blueChannel);
		ref Rgba32 reference4 = ref MemoryMarshal.GetReference(destination);
		for (nuint num = 0u; num < (uint)destination.Length; num++)
		{
			ref Rgba32 reference5 = ref Unsafe.Add(ref reference4, num);
			reference5.R = Unsafe.Add(ref reference, num);
			reference5.G = Unsafe.Add(ref reference2, num);
			reference5.B = Unsafe.Add(ref reference3, num);
			reference5.A = byte.MaxValue;
		}
	}

	private static void UnpackToRgbPlanesScalar(Span<float> redChannel, Span<float> greenChannel, Span<float> blueChannel, ReadOnlySpan<Rgb24> source)
	{
		ref float reference = ref MemoryMarshal.GetReference(redChannel);
		ref float reference2 = ref MemoryMarshal.GetReference(greenChannel);
		ref float reference3 = ref MemoryMarshal.GetReference(blueChannel);
		ref Rgb24 reference4 = ref MemoryMarshal.GetReference(source);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Rgb24 reference5 = ref Unsafe.Add(ref reference4, num);
			Unsafe.Add(ref reference, num) = (int)reference5.R;
			Unsafe.Add(ref reference2, num) = (int)reference5.G;
			Unsafe.Add(ref reference3, num) = (int)reference5.B;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle4(ReadOnlySpan<float> source, Span<float> dest, byte control)
	{
		HwIntrinsics.Shuffle4Reduce(ref source, ref dest, control);
		if (source.Length > 0)
		{
			Shuffle4Remainder(source, dest, control);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle4<TShuffle>(ReadOnlySpan<byte> source, Span<byte> dest, TShuffle shuffle) where TShuffle : struct, IShuffle4
	{
		shuffle.ShuffleReduce(ref source, ref dest);
		if (source.Length > 0)
		{
			shuffle.RunFallbackShuffle(source, dest);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle3<TShuffle>(ReadOnlySpan<byte> source, Span<byte> dest, TShuffle shuffle) where TShuffle : struct, IShuffle3
	{
		shuffle.ShuffleReduce(ref source, ref dest);
		if (source.Length > 0)
		{
			shuffle.RunFallbackShuffle(source, dest);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Pad3Shuffle4<TShuffle>(ReadOnlySpan<byte> source, Span<byte> dest, TShuffle shuffle) where TShuffle : struct, IPad3Shuffle4
	{
		shuffle.ShuffleReduce(ref source, ref dest);
		if (source.Length > 0)
		{
			shuffle.RunFallbackShuffle(source, dest);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle4Slice3<TShuffle>(ReadOnlySpan<byte> source, Span<byte> dest, TShuffle shuffle) where TShuffle : struct, IShuffle4Slice3
	{
		shuffle.ShuffleReduce(ref source, ref dest);
		if (source.Length > 0)
		{
			shuffle.RunFallbackShuffle(source, dest);
		}
	}

	private static void Shuffle4Remainder(ReadOnlySpan<float> source, Span<float> dest, byte control)
	{
		ref float reference = ref MemoryMarshal.GetReference(source);
		ref float reference2 = ref MemoryMarshal.GetReference(dest);
		Shuffle.InverseMMShuffle(control, out var p, out var p2, out var p3, out var p4);
		for (nuint num = 0u; num < (uint)source.Length; num += 4)
		{
			Unsafe.Add(ref reference2, num + 0) = Unsafe.Add(ref reference, p4 + num);
			Unsafe.Add(ref reference2, num + 1) = Unsafe.Add(ref reference, p3 + num);
			Unsafe.Add(ref reference2, num + 2) = Unsafe.Add(ref reference, p2 + num);
			Unsafe.Add(ref reference2, num + 3) = Unsafe.Add(ref reference, p + num);
		}
	}

	[Conditional("DEBUG")]
	internal static void VerifyShuffle4SpanInput<T>(ReadOnlySpan<T> source, Span<T> dest) where T : struct
	{
	}

	[Conditional("DEBUG")]
	private static void VerifyShuffle3SpanInput<T>(ReadOnlySpan<T> source, Span<T> dest) where T : struct
	{
	}

	[Conditional("DEBUG")]
	private static void VerifyPad3Shuffle4SpanInput(ReadOnlySpan<byte> source, Span<byte> dest)
	{
	}

	[Conditional("DEBUG")]
	private static void VerifyShuffle4Slice3SpanInput(ReadOnlySpan<byte> source, Span<byte> dest)
	{
	}
}
