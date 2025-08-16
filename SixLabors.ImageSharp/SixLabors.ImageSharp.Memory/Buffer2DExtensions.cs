using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Memory;

public static class Buffer2DExtensions
{
	public static IMemoryGroup<T> GetMemoryGroup<T>(this Buffer2D<T> buffer) where T : struct
	{
		Guard.NotNull(buffer, "buffer");
		return buffer.FastMemoryGroup.View;
	}

	internal unsafe static void DangerousCopyColumns<T>(this Buffer2D<T> buffer, int sourceIndex, int destIndex, int columnCount) where T : struct
	{
		int num = Unsafe.SizeOf<T>();
		int num2 = buffer.Width * num;
		int num3 = sourceIndex * num;
		int num4 = destIndex * num;
		long num5 = columnCount * num;
		fixed (byte* ptr = MemoryMarshal.AsBytes(buffer.DangerousGetSingleMemory().Span))
		{
			byte* ptr2 = ptr;
			for (int i = 0; i < buffer.Height; i++)
			{
				byte* source = ptr2 + num3;
				byte* destination = ptr2 + num4;
				Buffer.MemoryCopy(source, destination, num5, num5);
				ptr2 += num2;
			}
		}
	}

	internal static Rectangle FullRectangle<T>(this Buffer2D<T> buffer) where T : struct
	{
		return new Rectangle(0, 0, buffer.Width, buffer.Height);
	}

	internal static Buffer2DRegion<T> GetRegion<T>(this Buffer2D<T> buffer, Rectangle rectangle) where T : unmanaged
	{
		return new Buffer2DRegion<T>(buffer, rectangle);
	}

	internal static Buffer2DRegion<T> GetRegion<T>(this Buffer2D<T> buffer, int x, int y, int width, int height) where T : unmanaged
	{
		return new Buffer2DRegion<T>(buffer, new Rectangle(x, y, width, height));
	}

	internal static Buffer2DRegion<T> GetRegion<T>(this Buffer2D<T> buffer) where T : unmanaged
	{
		return new Buffer2DRegion<T>(buffer);
	}

	internal static Size Size<T>(this Buffer2D<T> buffer) where T : struct
	{
		return new Size(buffer.Width, buffer.Height);
	}

	internal static Rectangle Bounds<T>(this Buffer2D<T> buffer) where T : struct
	{
		return new Rectangle(0, 0, buffer.Width, buffer.Height);
	}

	[Conditional("DEBUG")]
	private static void CheckColumnRegionsDoNotOverlap<T>(Buffer2D<T> buffer, int sourceIndex, int destIndex, int columnCount) where T : struct
	{
		int num = Math.Min(sourceIndex, destIndex);
		int num2 = Math.Max(sourceIndex, destIndex);
		if (num2 < num + columnCount || num2 > buffer.Width - columnCount)
		{
			throw new InvalidOperationException("Column regions should not overlap!");
		}
	}
}
