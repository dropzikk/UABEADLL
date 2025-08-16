using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.PixelFormats.Utils;

internal static class PixelConverter
{
	public static class FromRgba32
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToArgb32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(WXYZShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgra32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(ZYXWShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToAbgr32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(WZYXShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgb24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4Slice3(source, dest, default(XYZWShuffle4Slice3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgr24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4Slice3(source, dest, new DefaultShuffle4Slice3(198));
		}
	}

	public static class FromArgb32
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgba32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(YZWXShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgra32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(WZYXShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToAbgr32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(XWZYShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgb24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4Slice3(source, dest, new DefaultShuffle4Slice3(57));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgr24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4Slice3(source, dest, new DefaultShuffle4Slice3(27));
		}
	}

	public static class FromBgra32
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToArgb32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(WZYXShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgba32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(ZYXWShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToAbgr32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(WXYZShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgb24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4Slice3(source, dest, new DefaultShuffle4Slice3(198));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgr24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4Slice3(source, dest, default(XYZWShuffle4Slice3));
		}
	}

	public static class FromAbgr32
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToArgb32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(XWZYShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgba32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(WZYXShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgra32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4(source, dest, default(YZWXShuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgb24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4Slice3(source, dest, new DefaultShuffle4Slice3(27));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgr24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle4Slice3(source, dest, new DefaultShuffle4Slice3(57));
		}
	}

	public static class FromRgb24
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgba32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Pad3Shuffle4(source, dest, default(XYZWPad3Shuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToArgb32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Pad3Shuffle4(source, dest, new DefaultPad3Shuffle4(147));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgra32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Pad3Shuffle4(source, dest, new DefaultPad3Shuffle4(198));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToAbgr32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Pad3Shuffle4(source, dest, new DefaultPad3Shuffle4(27));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgr24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle3(source, dest, new DefaultShuffle3(198));
		}
	}

	public static class FromBgr24
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToArgb32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Pad3Shuffle4(source, dest, new DefaultPad3Shuffle4(27));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgba32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Pad3Shuffle4(source, dest, new DefaultPad3Shuffle4(198));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToBgra32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Pad3Shuffle4(source, dest, default(XYZWPad3Shuffle4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToAbgr32(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Pad3Shuffle4(source, dest, new DefaultPad3Shuffle4(147));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToRgb24(ReadOnlySpan<byte> source, Span<byte> dest)
		{
			SimdUtils.Shuffle3(source, dest, new DefaultShuffle3(198));
		}
	}
}
