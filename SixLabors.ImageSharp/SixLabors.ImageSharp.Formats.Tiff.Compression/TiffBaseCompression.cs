using System;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

internal abstract class TiffBaseCompression : IDisposable
{
	private bool isDisposed;

	public int Width { get; }

	public int BitsPerPixel { get; }

	public int BytesPerRow { get; }

	public TiffPredictor Predictor { get; }

	protected MemoryAllocator Allocator { get; }

	protected TiffBaseCompression(MemoryAllocator allocator, int width, int bitsPerPixel, TiffPredictor predictor = TiffPredictor.None)
	{
		Allocator = allocator;
		Width = width;
		BitsPerPixel = bitsPerPixel;
		Predictor = predictor;
		BytesPerRow = (width * bitsPerPixel + 7) / 8;
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			Dispose(disposing: true);
		}
	}

	protected abstract void Dispose(bool disposing);
}
