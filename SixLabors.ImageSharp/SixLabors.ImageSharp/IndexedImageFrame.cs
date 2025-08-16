using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public sealed class IndexedImageFrame<TPixel> : IPixelSource, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly Buffer2D<byte> pixelBuffer;

	private readonly IMemoryOwner<TPixel> paletteOwner;

	private bool isDisposed;

	public Configuration Configuration { get; }

	public int Width { get; }

	public int Height { get; }

	public ReadOnlyMemory<TPixel> Palette { get; }

	Buffer2D<byte> IPixelSource.PixelBuffer => pixelBuffer;

	internal IndexedImageFrame(Configuration configuration, int width, int height, ReadOnlyMemory<TPixel> palette)
	{
		Guard.NotNull(configuration, "configuration");
		Guard.MustBeLessThanOrEqualTo(palette.Length, 256, "palette");
		Guard.MustBeGreaterThan(width, 0, "width");
		Guard.MustBeGreaterThan(height, 0, "height");
		Configuration = configuration;
		Width = width;
		Height = height;
		pixelBuffer = configuration.MemoryAllocator.Allocate2D<byte>(width, height);
		paletteOwner = configuration.MemoryAllocator.Allocate<TPixel>(palette.Length);
		palette.Span.CopyTo(paletteOwner.GetSpan());
		Palette = paletteOwner.Memory.Slice(0, palette.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<byte> DangerousGetRowSpan(int rowIndex)
	{
		return GetWritablePixelRowSpanUnsafe(rowIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<byte> GetWritablePixelRowSpanUnsafe(int rowIndex)
	{
		return pixelBuffer.DangerousGetRowSpan(rowIndex);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			pixelBuffer.Dispose();
			paletteOwner.Dispose();
		}
	}
}
