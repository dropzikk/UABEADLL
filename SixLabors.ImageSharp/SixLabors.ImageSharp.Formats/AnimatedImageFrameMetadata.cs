using System;

namespace SixLabors.ImageSharp.Formats;

internal class AnimatedImageFrameMetadata
{
	public ReadOnlyMemory<Color>? ColorTable { get; set; }

	public FrameColorTableMode ColorTableMode { get; set; }

	public TimeSpan Duration { get; set; }

	public FrameBlendMode BlendMode { get; set; }

	public FrameDisposalMode DisposalMode { get; set; }
}
