using System;
using SixLabors.ImageSharp.Formats.Webp.BitReader;
using SixLabors.ImageSharp.Formats.Webp.Lossy;

namespace SixLabors.ImageSharp.Formats.Webp;

internal class WebpImageInfo : IDisposable
{
	public uint Width { get; set; }

	public uint Height { get; set; }

	public sbyte XScale { get; set; }

	public sbyte YScale { get; set; }

	public WebpBitsPerPixel BitsPerPixel { get; set; }

	public bool IsLossless { get; set; }

	public WebpFeatures? Features { get; set; }

	public int Vp8Profile { get; set; } = -1;

	public Vp8FrameHeader? Vp8FrameHeader { get; set; }

	public Vp8LBitReader? Vp8LBitReader { get; set; }

	public Vp8BitReader? Vp8BitReader { get; set; }

	public void Dispose()
	{
		Vp8BitReader?.Dispose();
		Vp8LBitReader?.Dispose();
	}
}
