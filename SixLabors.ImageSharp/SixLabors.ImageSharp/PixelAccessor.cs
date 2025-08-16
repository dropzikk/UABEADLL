using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public ref struct PixelAccessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private Buffer2D<TPixel> buffer;

	public int Width => buffer.Width;

	public int Height => buffer.Height;

	internal PixelAccessor(Buffer2D<TPixel> buffer)
	{
		this.buffer = buffer;
	}

	public Span<TPixel> GetRowSpan(int rowIndex)
	{
		return buffer.DangerousGetRowSpan(rowIndex);
	}
}
