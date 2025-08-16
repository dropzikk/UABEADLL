using System;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal ref struct Vp8Io
{
	public int Width { get; set; }

	public int Height { get; set; }

	public int MbY { get; set; }

	public int MbW { get; set; }

	public int MbH { get; set; }

	public Span<byte> Y { get; set; }

	public Span<byte> U { get; set; }

	public Span<byte> V { get; set; }

	public int YStride { get; set; }

	public int UvStride { get; set; }

	public bool UseScaling { get; set; }

	public int ScaledWidth { get; set; }

	public int ScaledHeight { get; set; }
}
