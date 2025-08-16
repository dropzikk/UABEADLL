using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

internal static class AnimationUtilities
{
	public static (bool Difference, Rectangle Bounds) DeDuplicatePixels<TPixel>(Configuration configuration, ImageFrame<TPixel>? previousFrame, ImageFrame<TPixel> currentFrame, ImageFrame<TPixel>? nextFrame, ImageFrame<TPixel> resultFrame, Color replacement, bool blend, ClampingMode clampingMode = ClampingMode.None) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<Rgba32> buffer = configuration.MemoryAllocator.Allocate<Rgba32>(currentFrame.Width * 4, AllocationOptions.Clean);
		Span<Rgba32> span = buffer.GetSpan();
		Span<Rgba32> span2 = span.Slice(0, currentFrame.Width);
		span = buffer.GetSpan();
		Span<Rgba32> span3 = span.Slice(currentFrame.Width, currentFrame.Width);
		span = buffer.GetSpan();
		Span<Rgba32> span4 = span.Slice(currentFrame.Width * 2, currentFrame.Width);
		span = buffer.GetSpan();
		int num = currentFrame.Width * 3;
		Span<Rgba32> span5 = span.Slice(num, span.Length - num);
		Rgba32 rgba = replacement;
		int num2 = int.MinValue;
		int value = int.MaxValue;
		int num3 = int.MaxValue;
		int num4 = int.MinValue;
		bool item = false;
		for (int i = 0; i < currentFrame.Height; i++)
		{
			if (previousFrame != null)
			{
				PixelOperations<TPixel>.Instance.ToRgba32(configuration, previousFrame.DangerousGetPixelRowMemory(i).Span, span2);
			}
			PixelOperations<TPixel>.Instance.ToRgba32(configuration, currentFrame.DangerousGetPixelRowMemory(i).Span, span3);
			if (nextFrame != null)
			{
				PixelOperations<TPixel>.Instance.ToRgba32(configuration, nextFrame.DangerousGetPixelRowMemory(i).Span, span4);
			}
			ref Vector256<byte> source = ref Unsafe.As<Rgba32, Vector256<byte>>(ref MemoryMarshal.GetReference(span2));
			ref Vector256<byte> source2 = ref Unsafe.As<Rgba32, Vector256<byte>>(ref MemoryMarshal.GetReference(span3));
			ref Vector256<byte> source3 = ref Unsafe.As<Rgba32, Vector256<byte>>(ref MemoryMarshal.GetReference(span4));
			ref Vector256<byte> source4 = ref Unsafe.As<Rgba32, Vector256<byte>>(ref MemoryMarshal.GetReference(span5));
			int num5 = 0;
			uint num6 = 0u;
			bool flag = false;
			int length = span3.Length;
			int num7 = span3.Length;
			if (Avx2.IsSupported && num7 >= 8)
			{
				Vector256<uint> right = ((previousFrame != null) ? Vector256.Create(rgba.PackedValue) : Vector256<uint>.Zero);
				Vector256<uint> vector = Vector256<uint>.Zero;
				if (blend)
				{
					vector = Avx2.CompareEqual(vector, vector);
				}
				while (num7 >= 8)
				{
					Vector256<uint> left = Unsafe.Add(ref source, num6).AsUInt32();
					Vector256<uint> vector2 = Unsafe.Add(ref source2, num6).AsUInt32();
					Vector256<uint> vector3 = Avx2.CompareEqual(left, vector2);
					Vector256<uint> vector4 = Avx2.BlendVariable(vector2, right, Avx2.And(vector3, vector));
					if (nextFrame != null)
					{
						Vector256<int> right2 = Avx2.ShiftRightLogical(Unsafe.Add(ref source3, num6).AsUInt32(), 24).AsInt32();
						vector3 = Avx2.AndNot(Avx2.CompareGreaterThan(Avx2.ShiftRightLogical(vector2, 24).AsInt32(), right2).AsUInt32(), vector3);
					}
					Unsafe.Add(ref source4, num6) = vector4.AsByte();
					uint num8 = (uint)Avx2.MoveMask(vector3.AsByte());
					num8 = ~num8;
					if (num8 != 0)
					{
						int val = num5 + BitOperations.TrailingZeroCount(num8) / 4;
						int val2 = num5 + (8 - BitOperations.LeadingZeroCount(num8) / 4);
						num3 = Math.Min(num3, val);
						num4 = Math.Max(num4, val2);
						flag = true;
						item = true;
					}
					num6++;
					num5 += 8;
					num7 -= 8;
				}
			}
			if (Sse2.IsSupported && num7 >= 4)
			{
				num6 *= 2;
				Vector128<uint> right3 = ((previousFrame != null) ? Vector128.Create(rgba.PackedValue) : Vector128<uint>.Zero);
				Vector128<uint> vector5 = Vector128<uint>.Zero;
				if (blend)
				{
					vector5 = Sse2.CompareEqual(vector5, vector5);
				}
				while (num7 >= 4)
				{
					Vector128<uint> left2 = Unsafe.Add(ref Unsafe.As<Vector256<byte>, Vector128<uint>>(ref source), num6);
					Vector128<uint> vector6 = Unsafe.Add(ref Unsafe.As<Vector256<byte>, Vector128<uint>>(ref source2), num6);
					Vector128<uint> vector7 = Sse2.CompareEqual(left2, vector6);
					Vector128<uint> vector8 = SimdUtils.HwIntrinsics.BlendVariable(vector6, right3, Sse2.And(vector7, vector5));
					if (nextFrame != null)
					{
						Vector128<int> right4 = Sse2.ShiftRightLogical(Unsafe.Add(ref Unsafe.As<Vector256<byte>, Vector128<uint>>(ref source3), num6), 24).AsInt32();
						vector7 = Sse2.AndNot(Sse2.CompareGreaterThan(Sse2.ShiftRightLogical(vector6, 24).AsInt32(), right4).AsUInt32(), vector7);
					}
					Unsafe.Add(ref Unsafe.As<Vector256<byte>, Vector128<uint>>(ref source4), num6) = vector8;
					ushort num9 = (ushort)Sse2.MoveMask(vector7.AsByte());
					num9 = (ushort)(~num9);
					if (num9 != 0)
					{
						int val3 = num5 + SimdUtils.HwIntrinsics.TrailingZeroCount(num9) / 4;
						int val4 = num5 + (4 - SimdUtils.HwIntrinsics.LeadingZeroCount(num9) / 4);
						num3 = Math.Min(num3, val3);
						num4 = Math.Max(num4, val4);
						flag = true;
						item = true;
					}
					num6++;
					num5 += 4;
					num7 -= 4;
				}
			}
			if (AdvSimd.IsSupported && num7 >= 4)
			{
				num6 *= 2;
				Vector128<uint> right5 = ((previousFrame != null) ? Vector128.Create(rgba.PackedValue) : Vector128<uint>.Zero);
				Vector128<uint> vector9 = Vector128<uint>.Zero;
				if (blend)
				{
					vector9 = AdvSimd.CompareEqual(vector9, vector9);
				}
				while (num7 >= 4)
				{
					Vector128<uint> left3 = Unsafe.Add(ref Unsafe.As<Vector256<byte>, Vector128<uint>>(ref source), num6);
					Vector128<uint> vector10 = Unsafe.Add(ref Unsafe.As<Vector256<byte>, Vector128<uint>>(ref source2), num6);
					Vector128<uint> vector11 = AdvSimd.CompareEqual(left3, vector10);
					Vector128<uint> vector12 = SimdUtils.HwIntrinsics.BlendVariable(vector10, right5, AdvSimd.And(vector11, vector9));
					if (nextFrame != null)
					{
						Vector128<int> right6 = AdvSimd.ShiftRightLogical(Unsafe.Add(ref Unsafe.As<Vector256<byte>, Vector128<uint>>(ref source3), num6), 24).AsInt32();
						vector11 = AdvSimd.BitwiseClear(vector11, AdvSimd.CompareGreaterThan(AdvSimd.ShiftRightLogical(vector10, 24).AsInt32(), right6).AsUInt32());
					}
					Unsafe.Add(ref Unsafe.As<Vector256<byte>, Vector128<uint>>(ref source4), num6) = vector12;
					ulong num10 = ~AdvSimd.ExtractNarrowingLower(vector11).AsUInt64().ToScalar();
					if (num10 != 0L)
					{
						int val5 = num5 + BitOperations.TrailingZeroCount(num10) / 16;
						int val6 = num5 + (4 - BitOperations.LeadingZeroCount(num10) / 16);
						num3 = Math.Min(num3, val5);
						num4 = Math.Max(num4, val6);
						flag = true;
						item = true;
					}
					num6++;
					num5 += 4;
					num7 -= 4;
				}
			}
			for (num5 = num7; num5 > 0; num5--)
			{
				num6 = (uint)(length - num5);
				Rgba32 rgba2 = Unsafe.Add(ref MemoryMarshal.GetReference(span2), num6);
				Rgba32 rgba3 = Unsafe.Add(ref MemoryMarshal.GetReference(span3), num6);
				Rgba32 rgba4 = Unsafe.Add(ref MemoryMarshal.GetReference(span4), num6);
				ref Rgba32 reference = ref Unsafe.Add(ref MemoryMarshal.GetReference(span5), num6);
				bool flag2 = rgba3.Rgba == ((previousFrame != null) ? rgba2.Rgba : rgba.Rgba);
				Rgba32 rgba5 = ((blend && flag2) ? ((Rgba32)replacement) : rgba3);
				flag2 &= nextFrame == null || rgba4.Rgba >> 24 >= rgba3.Rgba >> 24;
				reference = rgba5;
				if (!flag2)
				{
					num3 = Math.Min(num3, (int)num6);
					num4 = Math.Max(num4, (int)(num6 + 1));
					flag = true;
					item = true;
				}
			}
			if (flag)
			{
				if (num2 == int.MinValue)
				{
					num2 = i;
				}
				value = i + 1;
			}
			PixelOperations<TPixel>.Instance.FromRgba32(configuration, span5, resultFrame.DangerousGetPixelRowMemory(i).Span);
		}
		Rectangle item2 = Rectangle.FromLTRB(num3 = Numerics.Clamp(num3, 0, resultFrame.Width - 1), num2 = Numerics.Clamp(num2, 0, resultFrame.Height - 1), Numerics.Clamp(num4, num3 + 1, resultFrame.Width), Numerics.Clamp(value, num2 + 1, resultFrame.Height));
		if (clampingMode == ClampingMode.Even)
		{
			item2.Width = Math.Min(resultFrame.Width, item2.Width + (item2.X & 1));
			item2.Height = Math.Min(resultFrame.Height, item2.Height + (item2.Y & 1));
			item2.X = Math.Max(0, item2.X - (item2.X & 1));
			item2.Y = Math.Max(0, item2.Y - (item2.Y & 1));
		}
		return (Difference: item, Bounds: item2);
	}
}
