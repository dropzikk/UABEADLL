using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Memory;

public readonly struct Buffer2DRegion<T> where T : unmanaged
{
	public Rectangle Rectangle { get; }

	public Buffer2D<T> Buffer { get; }

	public int Width => Rectangle.Width;

	public int Height => Rectangle.Height;

	public int Stride => Buffer.Width;

	internal Size Size => Rectangle.Size;

	internal bool IsFullBufferArea => Size == Buffer.Size();

	internal ref T this[int x, int y] => ref Buffer[x + Rectangle.X, y + Rectangle.Y];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Buffer2DRegion(Buffer2D<T> buffer, Rectangle rectangle)
	{
		Buffer = buffer;
		Rectangle = rectangle;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Buffer2DRegion(Buffer2D<T> buffer)
		: this(buffer, buffer.FullRectangle())
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T> DangerousGetRowSpan(int y)
	{
		int y2 = Rectangle.Y + y;
		int x = Rectangle.X;
		int width = Rectangle.Width;
		return Buffer.DangerousGetRowSpan(y2).Slice(x, width);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Buffer2DRegion<T> GetSubRegion(int x, int y, int width, int height)
	{
		Rectangle rectangle = new Rectangle(x, y, width, height);
		return GetSubRegion(rectangle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Buffer2DRegion<T> GetSubRegion(Rectangle rectangle)
	{
		int x = Rectangle.X + rectangle.X;
		int y = Rectangle.Y + rectangle.Y;
		rectangle = new Rectangle(x, y, rectangle.Width, rectangle.Height);
		return new Buffer2DRegion<T>(Buffer, rectangle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref T GetReferenceToOrigin()
	{
		int y = Rectangle.Y;
		int x = Rectangle.X;
		return ref Buffer.DangerousGetRowSpan(y)[x];
	}

	internal void Clear()
	{
		if (IsFullBufferArea)
		{
			Buffer.FastMemoryGroup.Clear();
			return;
		}
		for (int i = 0; i < Rectangle.Height; i++)
		{
			DangerousGetRowSpan(i).Clear();
		}
	}

	internal void Fill(T value)
	{
		if (IsFullBufferArea)
		{
			Buffer.FastMemoryGroup.Fill(value);
			return;
		}
		for (int i = 0; i < Rectangle.Height; i++)
		{
			DangerousGetRowSpan(i).Fill(value);
		}
	}
}
