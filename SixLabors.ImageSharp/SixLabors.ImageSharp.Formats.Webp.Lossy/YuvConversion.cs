using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal static class YuvConversion
{
	private const int YuvFix = 16;

	private const int YuvHalf = 32768;

	public static void UpSample(Span<byte> topY, Span<byte> bottomY, Span<byte> topU, Span<byte> topV, Span<byte> curU, Span<byte> curV, Span<byte> topDst, Span<byte> bottomDst, int len, byte[] uvBuffer)
	{
		if (Sse41.IsSupported)
		{
			UpSampleSse41(topY, bottomY, topU, topV, curU, curV, topDst, bottomDst, len, uvBuffer);
		}
		else
		{
			UpSampleScalar(topY, bottomY, topU, topV, curU, curV, topDst, bottomDst, len);
		}
	}

	private static void UpSampleScalar(Span<byte> topY, Span<byte> bottomY, Span<byte> topU, Span<byte> topV, Span<byte> curU, Span<byte> curV, Span<byte> topDst, Span<byte> bottomDst, int len)
	{
		int num = len - 1 >> 1;
		uint num2 = LoadUv(topU[0], topV[0]);
		uint num3 = LoadUv(curU[0], curV[0]);
		uint num4 = 3 * num2 + num3 + 131074 >> 2;
		YuvToBgr(topY[0], (int)(num4 & 0xFF), (int)(num4 >> 16), topDst);
		if (bottomY != default(Span<byte>))
		{
			num4 = 3 * num3 + num2 + 131074 >> 2;
			YuvToBgr(bottomY[0], (int)(num4 & 0xFF), (int)(num4 >> 16), bottomDst);
		}
		for (int i = 1; i <= num; i++)
		{
			uint num5 = LoadUv(topU[i], topV[i]);
			uint num6 = LoadUv(curU[i], curV[i]);
			uint num7 = num2 + num5 + num3 + num6 + 524296;
			uint num8 = num7 + 2 * (num5 + num3) >> 3;
			uint num9 = num7 + 2 * (num2 + num6) >> 3;
			num4 = num8 + num2 >> 1;
			uint num10 = num9 + num5 >> 1;
			int num11 = i * 2;
			byte y = topY[num11 - 1];
			uint u = num4 & 0xFF;
			uint v = num4 >> 16;
			ref Span<byte> reference = ref topDst;
			int num12 = (num11 - 1) * 3;
			YuvToBgr(y, (int)u, (int)v, reference.Slice(num12, reference.Length - num12));
			byte y2 = topY[num11];
			uint u2 = num10 & 0xFF;
			uint v2 = num10 >> 16;
			reference = ref topDst;
			num12 = num11 * 3;
			YuvToBgr(y2, (int)u2, (int)v2, reference.Slice(num12, reference.Length - num12));
			if (bottomY != default(Span<byte>))
			{
				num4 = num9 + num3 >> 1;
				num10 = num8 + num6 >> 1;
				byte y3 = bottomY[num11 - 1];
				uint u3 = num4 & 0xFF;
				uint v3 = num4 >> 16;
				reference = ref bottomDst;
				num12 = (num11 - 1) * 3;
				YuvToBgr(y3, (int)u3, (int)v3, reference.Slice(num12, reference.Length - num12));
				byte y4 = bottomY[num11];
				uint u4 = num10 & 0xFF;
				uint v4 = num10 >> 16;
				reference = ref bottomDst;
				num12 = num11 * 3;
				YuvToBgr(y4, (int)u4, (int)v4, reference.Slice(num12, reference.Length - num12));
			}
			num2 = num5;
			num3 = num6;
		}
		if ((len & 1) == 0)
		{
			num4 = 3 * num2 + num3 + 131074 >> 2;
			byte y5 = topY[len - 1];
			uint u5 = num4 & 0xFF;
			uint v5 = num4 >> 16;
			ref Span<byte> reference = ref topDst;
			int num12 = (len - 1) * 3;
			YuvToBgr(y5, (int)u5, (int)v5, reference.Slice(num12, reference.Length - num12));
			if (bottomY != default(Span<byte>))
			{
				num4 = 3 * num3 + num2 + 131074 >> 2;
				byte y6 = bottomY[len - 1];
				uint u6 = num4 & 0xFF;
				uint v6 = num4 >> 16;
				reference = ref bottomDst;
				num12 = (len - 1) * 3;
				YuvToBgr(y6, (int)u6, (int)v6, reference.Slice(num12, reference.Length - num12));
			}
		}
	}

	private static void UpSampleSse41(Span<byte> topY, Span<byte> bottomY, Span<byte> topU, Span<byte> topV, Span<byte> curU, Span<byte> curV, Span<byte> topDst, Span<byte> bottomDst, int len, byte[] uvBuffer)
	{
		Array.Clear(uvBuffer);
		Span<byte> span = uvBuffer.AsSpan(15);
		ref Span<byte> reference = ref span;
		Span<byte> span2 = reference.Slice(32, reference.Length - 32);
		int num = (topU[0] + curU[0] >> 1) + 1;
		int num2 = (topV[0] + curV[0] >> 1) + 1;
		int u = topU[0] + num >> 1;
		int v = topV[0] + num2 >> 1;
		YuvToBgr(topY[0], u, v, topDst);
		if (bottomY != default(Span<byte>))
		{
			int u2 = curU[0] + num >> 1;
			int v2 = curV[0] + num2 >> 1;
			YuvToBgr(bottomY[0], u2, v2, bottomDst);
		}
		ref byte reference2 = ref MemoryMarshal.GetReference(topU);
		ref byte reference3 = ref MemoryMarshal.GetReference(topV);
		ref byte reference4 = ref MemoryMarshal.GetReference(curU);
		ref byte reference5 = ref MemoryMarshal.GetReference(curV);
		int num3;
		int num4;
		if (bottomY != default(Span<byte>))
		{
			num3 = 1;
			num4 = 0;
			while (num3 + 32 + 1 <= len)
			{
				UpSample32Pixels(ref Unsafe.Add(ref reference2, (uint)num4), ref Unsafe.Add(ref reference4, (uint)num4), span);
				UpSample32Pixels(ref Unsafe.Add(ref reference3, (uint)num4), ref Unsafe.Add(ref reference5, (uint)num4), span2);
				ConvertYuvToBgrWithBottomYSse41(topY, bottomY, topDst, bottomDst, span, span2, num3, 3);
				num3 += 32;
				num4 += 16;
			}
		}
		else
		{
			num3 = 1;
			num4 = 0;
			while (num3 + 32 + 1 <= len)
			{
				UpSample32Pixels(ref Unsafe.Add(ref reference2, (uint)num4), ref Unsafe.Add(ref reference4, (uint)num4), span);
				UpSample32Pixels(ref Unsafe.Add(ref reference3, (uint)num4), ref Unsafe.Add(ref reference5, (uint)num4), span2);
				ConvertYuvToBgrSse41(topY, topDst, span, span2, num3, 3);
				num3 += 32;
				num4 += 16;
			}
		}
		if (len > 1)
		{
			int numPixels = (len + 1 >> 1) - (num3 >> 1);
			reference = ref span;
			Span<byte> topDst2 = reference.Slice(128, reference.Length - 128);
			reference = ref topDst2;
			Span<byte> bottomDst2 = reference.Slice(128, reference.Length - 128);
			reference = ref bottomDst2;
			Span<byte> span3 = reference.Slice(128, reference.Length - 128);
			Span<byte> span4;
			if (!(bottomY == default(Span<byte>)))
			{
				reference = ref span3;
				span4 = reference.Slice(32, reference.Length - 32);
			}
			else
			{
				span4 = null;
			}
			Span<byte> span5 = span4;
			reference = ref topU;
			int num5 = num4;
			Span<byte> tb = reference.Slice(num5, reference.Length - num5);
			reference = ref curU;
			num5 = num4;
			UpSampleLastBlock(tb, reference.Slice(num5, reference.Length - num5), numPixels, span);
			reference = ref topV;
			num5 = num4;
			Span<byte> tb2 = reference.Slice(num5, reference.Length - num5);
			reference = ref curV;
			num5 = num4;
			UpSampleLastBlock(tb2, reference.Slice(num5, reference.Length - num5), numPixels, span2);
			num5 = num3;
			topY.Slice(num5, len - num5).CopyTo(span3);
			if (bottomY != default(Span<byte>))
			{
				num5 = num3;
				bottomY.Slice(num5, len - num5).CopyTo(span5);
				ConvertYuvToBgrWithBottomYSse41(span3, span5, topDst2, bottomDst2, span, span2, 0, 3);
			}
			else
			{
				ConvertYuvToBgrSse41(span3, topDst2, span, span2, 0, 3);
			}
			Span<byte> span6 = topDst2.Slice(0, (len - num3) * 3);
			reference = ref topDst;
			num5 = num3 * 3;
			span6.CopyTo(reference.Slice(num5, reference.Length - num5));
			if (bottomY != default(Span<byte>))
			{
				span6 = bottomDst2.Slice(0, (len - num3) * 3);
				reference = ref bottomDst;
				num5 = num3 * 3;
				span6.CopyTo(reference.Slice(num5, reference.Length - num5));
			}
		}
	}

	private static void UpSample32Pixels(ref byte r1, ref byte r2, Span<byte> output)
	{
		Vector128<byte> vector = Unsafe.As<byte, Vector128<byte>>(ref r1);
		Vector128<byte> vector2 = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref r1, 1));
		Vector128<byte> vector3 = Unsafe.As<byte, Vector128<byte>>(ref r2);
		Vector128<byte> vector4 = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref r2, 1));
		Vector128<byte> vector5 = Sse2.Average(vector, vector4);
		Vector128<byte> vector6 = Sse2.Average(vector2, vector3);
		Vector128<byte> vector7 = Sse2.Xor(vector5, vector6);
		Vector128<byte> vector8 = Sse2.Xor(vector, vector4);
		Vector128<byte> vector9 = Sse2.Xor(vector2, vector3);
		Vector128<byte> k = Sse2.Subtract(right: Sse2.And(Sse2.Or(Sse2.Or(vector8, vector9), vector7), Vector128.Create((byte)1)), left: Sse2.Average(vector5, vector6));
		Vector128<byte> m = GetM(k, vector7, vector9, vector6);
		Vector128<byte> m2 = GetM(k, vector7, vector8, vector5);
		PackAndStore(vector, vector2, m, m2, output);
		PackAndStore(vector3, vector4, m2, m, output.Slice(64, output.Length - 64));
	}

	private static void UpSampleLastBlock(Span<byte> tb, Span<byte> bb, int numPixels, Span<byte> output)
	{
		Span<byte> span = stackalloc byte[17];
		Span<byte> span2 = stackalloc byte[17];
		tb.Slice(0, numPixels).CopyTo(span);
		bb.Slice(0, numPixels).CopyTo(span2);
		int num = 17 - numPixels;
		if (num > 0)
		{
			span.Slice(numPixels, num).Fill(span[numPixels - 1]);
			span2.Slice(numPixels, num).Fill(span2[numPixels - 1]);
		}
		UpSample32Pixels(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(span2), output);
	}

	private static Vector128<byte> GetM(Vector128<byte> k, Vector128<byte> st, Vector128<byte> ij, Vector128<byte> input)
	{
		Vector128<byte> left = Sse2.Average(k, input);
		Vector128<byte> left2 = Sse2.And(ij, st);
		Vector128<byte> right = Sse2.Xor(k, input);
		Vector128<byte> right2 = Sse2.And(Sse2.Or(left2, right), Vector128.Create((byte)1));
		return Sse2.Subtract(left, right2);
	}

	private static void PackAndStore(Vector128<byte> a, Vector128<byte> b, Vector128<byte> da, Vector128<byte> db, Span<byte> output)
	{
		Vector128<byte> left = Sse2.Average(a, da);
		Vector128<byte> right = Sse2.Average(b, db);
		Vector128<byte> vector = Sse2.UnpackLow(left, right);
		Vector128<byte> vector2 = Sse2.UnpackHigh(left, right);
		ref byte reference = ref MemoryMarshal.GetReference(output);
		ref byte source = ref Unsafe.Add(ref reference, 16);
		Unsafe.As<byte, Vector128<byte>>(ref reference) = vector;
		Unsafe.As<byte, Vector128<byte>>(ref source) = vector2;
	}

	public static bool ConvertRgbToYuv<TPixel>(Buffer2DRegion<TPixel> frame, Configuration configuration, MemoryAllocator memoryAllocator, Span<byte> y, Span<byte> u, Span<byte> v) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = frame.Width;
		int height = frame.Height;
		int num = width + 1 >> 1;
		using IMemoryOwner<ushort> buffer = memoryAllocator.Allocate<ushort>(4 * num);
		using IMemoryOwner<Bgra32> buffer2 = memoryAllocator.Allocate<Bgra32>(width);
		using IMemoryOwner<Bgra32> buffer3 = memoryAllocator.Allocate<Bgra32>(width);
		Span<ushort> span = buffer.GetSpan();
		Span<Bgra32> span2 = buffer2.GetSpan();
		Span<Bgra32> span3 = buffer3.GetSpan();
		int num2 = 0;
		bool result = false;
		int i;
		for (i = 0; i < height - 1; i += 2)
		{
			Span<TPixel> span4 = frame.DangerousGetRowSpan(i);
			Span<TPixel> span5 = frame.DangerousGetRowSpan(i + 1);
			PixelOperations<TPixel>.Instance.ToBgra32(configuration, span4, span2);
			PixelOperations<TPixel>.Instance.ToBgra32(configuration, span5, span3);
			int num3;
			if (WebpCommonUtils.CheckNonOpaque(span2))
			{
				num3 = (WebpCommonUtils.CheckNonOpaque(span3) ? 1 : 0);
				if (num3 != 0)
				{
					result = true;
				}
			}
			else
			{
				num3 = 0;
			}
			if (num3 == 0)
			{
				AccumulateRgb(span2, span3, span, width);
			}
			else
			{
				AccumulateRgba(span2, span3, span, width);
			}
			ref Span<byte> reference = ref u;
			int num4 = num2 * num;
			Span<byte> u2 = reference.Slice(num4, reference.Length - num4);
			reference = ref v;
			num4 = num2 * num;
			ConvertRgbaToUv(span, u2, reference.Slice(num4, reference.Length - num4), num);
			num2++;
			reference = ref y;
			num4 = i * width;
			ConvertRgbaToY(span2, reference.Slice(num4, reference.Length - num4), width);
			reference = ref y;
			num4 = (i + 1) * width;
			ConvertRgbaToY(span3, reference.Slice(num4, reference.Length - num4), width);
		}
		if ((height & 1) != 0)
		{
			Span<TPixel> span6 = frame.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToBgra32(configuration, span6, span2);
			ref Span<byte> reference = ref y;
			int num4 = i * width;
			ConvertRgbaToY(span2, reference.Slice(num4, reference.Length - num4), width);
			if (!WebpCommonUtils.CheckNonOpaque(span2))
			{
				AccumulateRgb(span2, span2, span, width);
			}
			else
			{
				AccumulateRgba(span2, span2, span, width);
				result = true;
			}
			reference = ref u;
			num4 = num2 * num;
			Span<byte> u3 = reference.Slice(num4, reference.Length - num4);
			reference = ref v;
			num4 = num2 * num;
			ConvertRgbaToUv(span, u3, reference.Slice(num4, reference.Length - num4), num);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ConvertRgbaToY(Span<Bgra32> rowSpan, Span<byte> y, int width)
	{
		for (int i = 0; i < width; i++)
		{
			y[i] = (byte)RgbToY(rowSpan[i].R, rowSpan[i].G, rowSpan[i].B, 32768);
		}
	}

	public static void ConvertRgbaToUv(Span<ushort> rgb, Span<byte> u, Span<byte> v, int width)
	{
		int num = 0;
		int num2 = 0;
		while (num2 < width)
		{
			int r = rgb[num];
			int g = rgb[num + 1];
			int b = rgb[num + 2];
			u[num2] = (byte)RgbToU(r, g, b, 131072);
			v[num2] = (byte)RgbToV(r, g, b, 131072);
			num2++;
			num += 4;
		}
	}

	public static void AccumulateRgb(Span<Bgra32> rowSpan, Span<Bgra32> nextRowSpan, Span<ushort> dst, int width)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		while (num2 < width >> 1)
		{
			Bgra32 bgra = rowSpan[num3];
			Bgra32 bgra2 = rowSpan[num3 + 1];
			Bgra32 bgra3 = nextRowSpan[num3];
			Bgra32 bgra4 = nextRowSpan[num3 + 1];
			dst[num] = (ushort)LinearToGamma(GammaToLinear(bgra.R) + GammaToLinear(bgra2.R) + GammaToLinear(bgra3.R) + GammaToLinear(bgra4.R), 0);
			dst[num + 1] = (ushort)LinearToGamma(GammaToLinear(bgra.G) + GammaToLinear(bgra2.G) + GammaToLinear(bgra3.G) + GammaToLinear(bgra4.G), 0);
			dst[num + 2] = (ushort)LinearToGamma(GammaToLinear(bgra.B) + GammaToLinear(bgra2.B) + GammaToLinear(bgra3.B) + GammaToLinear(bgra4.B), 0);
			num2++;
			num3 += 2;
			num += 4;
		}
		if ((width & 1) != 0)
		{
			Bgra32 bgra = rowSpan[num3];
			Bgra32 bgra2 = nextRowSpan[num3];
			dst[num] = (ushort)LinearToGamma(GammaToLinear(bgra.R) + GammaToLinear(bgra2.R), 1);
			dst[num + 1] = (ushort)LinearToGamma(GammaToLinear(bgra.G) + GammaToLinear(bgra2.G), 1);
			dst[num + 2] = (ushort)LinearToGamma(GammaToLinear(bgra.B) + GammaToLinear(bgra2.B), 1);
		}
	}

	public static void AccumulateRgba(Span<Bgra32> rowSpan, Span<Bgra32> nextRowSpan, Span<ushort> dst, int width)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		while (num2 < width >> 1)
		{
			Bgra32 bgra = rowSpan[num3];
			Bgra32 bgra2 = rowSpan[num3 + 1];
			Bgra32 bgra3 = nextRowSpan[num3];
			Bgra32 bgra4 = nextRowSpan[num3 + 1];
			uint num4 = (uint)(bgra.A + bgra2.A + bgra3.A + bgra4.A);
			int num5;
			int num6;
			int num7;
			if ((num4 == 0 || num4 == 1020) ? true : false)
			{
				num5 = (ushort)LinearToGamma(GammaToLinear(bgra.R) + GammaToLinear(bgra2.R) + GammaToLinear(bgra3.R) + GammaToLinear(bgra4.R), 0);
				num6 = (ushort)LinearToGamma(GammaToLinear(bgra.G) + GammaToLinear(bgra2.G) + GammaToLinear(bgra3.G) + GammaToLinear(bgra4.G), 0);
				num7 = (ushort)LinearToGamma(GammaToLinear(bgra.B) + GammaToLinear(bgra2.B) + GammaToLinear(bgra3.B) + GammaToLinear(bgra4.B), 0);
			}
			else
			{
				num5 = LinearToGammaWeighted(bgra.R, bgra2.R, bgra3.R, bgra4.R, bgra.A, bgra2.A, bgra3.A, bgra4.A, num4);
				num6 = LinearToGammaWeighted(bgra.G, bgra2.G, bgra3.G, bgra4.G, bgra.A, bgra2.A, bgra3.A, bgra4.A, num4);
				num7 = LinearToGammaWeighted(bgra.B, bgra2.B, bgra3.B, bgra4.B, bgra.A, bgra2.A, bgra3.A, bgra4.A, num4);
			}
			dst[num] = (ushort)num5;
			dst[num + 1] = (ushort)num6;
			dst[num + 2] = (ushort)num7;
			dst[num + 3] = (ushort)num4;
			num2++;
			num3 += 2;
			num += 4;
		}
		if ((width & 1) != 0)
		{
			Bgra32 bgra = rowSpan[num3];
			Bgra32 bgra2 = nextRowSpan[num3];
			uint num8 = (uint)(2uL * (ulong)(bgra.A + bgra2.A));
			int num9;
			int num10;
			int num11;
			if ((num8 == 0 || num8 == 1020) ? true : false)
			{
				num9 = (ushort)LinearToGamma(GammaToLinear(bgra.R) + GammaToLinear(bgra2.R), 1);
				num10 = (ushort)LinearToGamma(GammaToLinear(bgra.G) + GammaToLinear(bgra2.G), 1);
				num11 = (ushort)LinearToGamma(GammaToLinear(bgra.B) + GammaToLinear(bgra2.B), 1);
			}
			else
			{
				num9 = LinearToGammaWeighted(bgra.R, bgra2.R, bgra.R, bgra2.R, bgra.A, bgra2.A, bgra.A, bgra2.A, num8);
				num10 = LinearToGammaWeighted(bgra.G, bgra2.G, bgra.G, bgra2.G, bgra.A, bgra2.A, bgra.A, bgra2.A, num8);
				num11 = LinearToGammaWeighted(bgra.B, bgra2.B, bgra.B, bgra2.B, bgra.A, bgra2.A, bgra.A, bgra2.A, num8);
			}
			dst[num] = (ushort)num9;
			dst[num + 1] = (ushort)num10;
			dst[num + 2] = (ushort)num11;
			dst[num + 3] = (ushort)num8;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int LinearToGammaWeighted(byte rgb0, byte rgb1, byte rgb2, byte rgb3, byte a0, byte a1, byte a2, byte a3, uint totalA)
	{
		return LinearToGamma((a0 * GammaToLinear(rgb0) + a1 * GammaToLinear(rgb1) + a2 * GammaToLinear(rgb2) + a3 * GammaToLinear(rgb3)) * WebpLookupTables.InvAlpha[totalA] >> 17, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int LinearToGamma(uint baseValue, int shift)
	{
		return Interpolate((int)(baseValue << shift)) + 64 >> 7;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint GammaToLinear(byte v)
	{
		return WebpLookupTables.GammaToLinearTab[v];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Interpolate(int v)
	{
		int num = v >> 9;
		int num2 = v & 0x1FF;
		int num3 = WebpLookupTables.LinearToGammaTab[num];
		return WebpLookupTables.LinearToGammaTab[num + 1] * num2 + num3 * (512 - num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int RgbToY(byte r, byte g, byte b, int rounding)
	{
		return 16839 * r + 33059 * g + 6420 * b + rounding + 1048576 >> 16;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int RgbToU(int r, int g, int b, int rounding)
	{
		return ClipUv(-9719 * r - 19081 * g + 28800 * b, rounding);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int RgbToV(int r, int g, int b, int rounding)
	{
		return ClipUv(28800 * r - 24116 * g - 4684 * b, rounding);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ClipUv(int uv, int rounding)
	{
		uv = uv + rounding + 33554432 >> 18;
		if ((uv & -256) != 0)
		{
			if (uv >= 0)
			{
				return 255;
			}
			return 0;
		}
		return uv;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint LoadUv(byte u, byte v)
	{
		return (uint)(u | (v << 16));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void YuvToBgr(int y, int u, int v, Span<byte> bgr)
	{
		bgr[2] = (byte)YuvToR(y, v);
		bgr[1] = (byte)YuvToG(y, u, v);
		bgr[0] = (byte)YuvToB(y, u);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ConvertYuvToBgrSse41(Span<byte> topY, Span<byte> topDst, Span<byte> ru, Span<byte> rv, int curX, int step)
	{
		ref Span<byte> reference = ref topY;
		int num = curX;
		Span<byte> y = reference.Slice(num, reference.Length - num);
		reference = ref topDst;
		num = curX * step;
		YuvToBgrSse41(y, ru, rv, reference.Slice(num, reference.Length - num));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ConvertYuvToBgrWithBottomYSse41(Span<byte> topY, Span<byte> bottomY, Span<byte> topDst, Span<byte> bottomDst, Span<byte> ru, Span<byte> rv, int curX, int step)
	{
		ref Span<byte> reference = ref topY;
		int num = curX;
		Span<byte> y = reference.Slice(num, reference.Length - num);
		Span<byte> u = ru;
		Span<byte> v = rv;
		reference = ref topDst;
		num = curX * step;
		YuvToBgrSse41(y, u, v, reference.Slice(num, reference.Length - num));
		reference = ref bottomY;
		num = curX;
		Span<byte> y2 = reference.Slice(num, reference.Length - num);
		reference = ref ru;
		Span<byte> u2 = reference.Slice(64, reference.Length - 64);
		reference = ref rv;
		Span<byte> v2 = reference.Slice(64, reference.Length - 64);
		reference = ref bottomDst;
		num = curX * step;
		YuvToBgrSse41(y2, u2, v2, reference.Slice(num, reference.Length - num));
	}

	private static void YuvToBgrSse41(Span<byte> y, Span<byte> u, Span<byte> v, Span<byte> dst)
	{
		ref byte reference = ref MemoryMarshal.GetReference(y);
		ref byte reference2 = ref MemoryMarshal.GetReference(u);
		ref byte reference3 = ref MemoryMarshal.GetReference(v);
		ConvertYuv444ToBgrSse41(ref reference, ref reference2, ref reference3, out var r, out var g, out var b);
		ConvertYuv444ToBgrSse41(ref Unsafe.Add(ref reference, 8), ref Unsafe.Add(ref reference2, 8), ref Unsafe.Add(ref reference3, 8), out var r2, out var g2, out var b2);
		ConvertYuv444ToBgrSse41(ref Unsafe.Add(ref reference, 16), ref Unsafe.Add(ref reference2, 16), ref Unsafe.Add(ref reference3, 16), out var r3, out var g3, out var b3);
		ConvertYuv444ToBgrSse41(ref Unsafe.Add(ref reference, 24), ref Unsafe.Add(ref reference2, 24), ref Unsafe.Add(ref reference3, 24), out var r4, out var g4, out var b4);
		Vector128<byte> input = Sse2.PackUnsignedSaturate(b, b2);
		Vector128<byte> input2 = Sse2.PackUnsignedSaturate(b3, b4);
		Vector128<byte> input3 = Sse2.PackUnsignedSaturate(g, g2);
		Vector128<byte> input4 = Sse2.PackUnsignedSaturate(g3, g4);
		Vector128<byte> input5 = Sse2.PackUnsignedSaturate(r, r2);
		Vector128<byte> input6 = Sse2.PackUnsignedSaturate(r3, r4);
		PlanarTo24bSse41(input, input2, input3, input4, input5, input6, dst);
	}

	private static void PlanarTo24bSse41(Vector128<byte> input0, Vector128<byte> input1, Vector128<byte> input2, Vector128<byte> input3, Vector128<byte> input4, Vector128<byte> input5, Span<byte> rgb)
	{
		ChannelMixing(input0, input1, Vector128.Create(0, byte.MaxValue, byte.MaxValue, 1, byte.MaxValue, byte.MaxValue, 2, byte.MaxValue, byte.MaxValue, 3, byte.MaxValue, byte.MaxValue, 4, byte.MaxValue, byte.MaxValue, 5), Vector128.Create(byte.MaxValue, byte.MaxValue, 6, byte.MaxValue, byte.MaxValue, 7, byte.MaxValue, byte.MaxValue, 8, byte.MaxValue, byte.MaxValue, 9, byte.MaxValue, byte.MaxValue, 10, byte.MaxValue), Vector128.Create(byte.MaxValue, 11, byte.MaxValue, byte.MaxValue, 12, byte.MaxValue, byte.MaxValue, 13, byte.MaxValue, byte.MaxValue, 14, byte.MaxValue, byte.MaxValue, 15, byte.MaxValue, byte.MaxValue), out var output, out var output2, out var output3, out var output4, out var output5, out var output6);
		ChannelMixing(input2, input3, Vector128.Create(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue, 1, byte.MaxValue, byte.MaxValue, 2, byte.MaxValue, byte.MaxValue, 3, byte.MaxValue, byte.MaxValue, 4, byte.MaxValue, byte.MaxValue), Vector128.Create(5, byte.MaxValue, byte.MaxValue, 6, byte.MaxValue, byte.MaxValue, 7, byte.MaxValue, byte.MaxValue, 8, byte.MaxValue, byte.MaxValue, 9, byte.MaxValue, byte.MaxValue, 10), Vector128.Create(byte.MaxValue, byte.MaxValue, 11, byte.MaxValue, byte.MaxValue, 12, byte.MaxValue, byte.MaxValue, 13, byte.MaxValue, byte.MaxValue, 14, byte.MaxValue, byte.MaxValue, 15, byte.MaxValue), out var output7, out var output8, out var output9, out var output10, out var output11, out var output12);
		ChannelMixing(input4, input5, Vector128.Create(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue, byte.MaxValue, 1, byte.MaxValue, byte.MaxValue, 2, byte.MaxValue, byte.MaxValue, 3, byte.MaxValue, byte.MaxValue, 4, byte.MaxValue), Vector128.Create(byte.MaxValue, 5, byte.MaxValue, byte.MaxValue, 6, byte.MaxValue, byte.MaxValue, 7, byte.MaxValue, byte.MaxValue, 8, byte.MaxValue, byte.MaxValue, 9, byte.MaxValue, byte.MaxValue), Vector128.Create(10, byte.MaxValue, byte.MaxValue, 11, byte.MaxValue, byte.MaxValue, 12, byte.MaxValue, byte.MaxValue, 13, byte.MaxValue, byte.MaxValue, 14, byte.MaxValue, byte.MaxValue, 15), out var output13, out var output14, out var output15, out var output16, out var output17, out var output18);
		Vector128<byte> left = Sse2.Or(output, output7);
		Vector128<byte> left2 = Sse2.Or(output2, output8);
		Vector128<byte> left3 = Sse2.Or(output3, output9);
		Vector128<byte> left4 = Sse2.Or(output4, output10);
		Vector128<byte> left5 = Sse2.Or(output5, output11);
		Vector128<byte> left6 = Sse2.Or(output6, output12);
		ref byte reference = ref MemoryMarshal.GetReference(rgb);
		Unsafe.As<byte, Vector128<byte>>(ref reference) = Sse2.Or(left, output13);
		Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference, 16)) = Sse2.Or(left2, output14);
		Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference, 32)) = Sse2.Or(left3, output15);
		Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference, 48)) = Sse2.Or(left4, output16);
		Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference, 64)) = Sse2.Or(left5, output17);
		Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref reference, 80)) = Sse2.Or(left6, output18);
	}

	private static void ChannelMixing(Vector128<byte> input0, Vector128<byte> input1, Vector128<byte> shuffle0, Vector128<byte> shuffle1, Vector128<byte> shuffle2, out Vector128<byte> output0, out Vector128<byte> output1, out Vector128<byte> output2, out Vector128<byte> output3, out Vector128<byte> output4, out Vector128<byte> output5)
	{
		output0 = Ssse3.Shuffle(input0, shuffle0);
		output1 = Ssse3.Shuffle(input0, shuffle1);
		output2 = Ssse3.Shuffle(input0, shuffle2);
		output3 = Ssse3.Shuffle(input1, shuffle0);
		output4 = Ssse3.Shuffle(input1, shuffle1);
		output5 = Ssse3.Shuffle(input1, shuffle2);
	}

	private static void ConvertYuv444ToBgrSse41(ref byte y, ref byte u, ref byte v, out Vector128<short> r, out Vector128<short> g, out Vector128<short> b)
	{
		Vector128<byte> right = Unsafe.As<byte, Vector128<byte>>(ref y);
		Vector128<byte> right2 = Unsafe.As<byte, Vector128<byte>>(ref u);
		Vector128<byte> right3 = Unsafe.As<byte, Vector128<byte>>(ref v);
		right = Sse2.UnpackLow(Vector128<byte>.Zero, right);
		right2 = Sse2.UnpackLow(Vector128<byte>.Zero, right2);
		right3 = Sse2.UnpackLow(Vector128<byte>.Zero, right3);
		Vector128<ushort> right4 = Vector128.Create((ushort)19077);
		Vector128<ushort> right5 = Vector128.Create((ushort)26149);
		Vector128<ushort> right6 = Vector128.Create((ushort)14234);
		Vector128<ushort> vector = Sse2.MultiplyHigh(right.AsUInt16(), right4);
		Vector128<ushort> right7 = Sse2.MultiplyHigh(right3.AsUInt16(), right5);
		Vector128<ushort> left = Sse2.MultiplyHigh(right2.AsUInt16(), Vector128.Create((ushort)6419));
		Vector128<ushort> right8 = Sse2.MultiplyHigh(right3.AsUInt16(), Vector128.Create((ushort)13320));
		Vector128<ushort> vector2 = Sse2.Add(Sse2.Subtract(vector.AsUInt16(), right6), right7);
		Vector128<ushort> left2 = Sse2.Add(vector.AsUInt16(), Vector128.Create((ushort)8708));
		Vector128<ushort> right9 = Sse2.Add(left, right8);
		Vector128<ushort> vector3 = Sse2.Subtract(left2, right9);
		Vector128<ushort> vector4 = Sse2.SubtractSaturate(Sse2.AddSaturate(Sse2.MultiplyHigh(right2.AsUInt16(), Vector128.Create(26, 129, 26, 129, 26, 129, 26, 129, 26, 129, 26, 129, 26, 129, 26, 129).AsUInt16()), vector), Vector128.Create((ushort)17685));
		r = Sse2.ShiftRightArithmetic(vector2.AsInt16(), 6);
		g = Sse2.ShiftRightArithmetic(vector3.AsInt16(), 6);
		b = Sse2.ShiftRightLogical(vector4.AsInt16(), 6);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int YuvToB(int y, int u)
	{
		return Clip8(MultHi(y, 19077) + MultHi(u, 33050) - 17685);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int YuvToG(int y, int u, int v)
	{
		return Clip8(MultHi(y, 19077) - MultHi(u, 6419) - MultHi(v, 13320) + 8708);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int YuvToR(int y, int v)
	{
		return Clip8(MultHi(y, 19077) + MultHi(v, 26149) - 14234);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int MultHi(int v, int coeff)
	{
		return v * coeff >> 8;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte Clip8(int v)
	{
		return (byte)(((v & -16384) == 0) ? ((uint)(v >> 6)) : ((v >= 0) ? 255u : 0u));
	}
}
