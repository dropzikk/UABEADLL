using System;

namespace SixLabors.ImageSharp.Formats;

internal class AnimatedImageMetadata
{
	public ReadOnlyMemory<Color>? ColorTable { get; set; }

	public FrameColorTableMode ColorTableMode { get; set; }

	public Color BackgroundColor { get; set; }

	public ushort RepeatCount { get; set; }
}
