using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public class ResizeOptions
{
	public ResizeMode Mode { get; set; }

	public AnchorPositionMode Position { get; set; }

	public PointF? CenterCoordinates { get; set; }

	public Size Size { get; set; }

	public IResampler Sampler { get; set; } = KnownResamplers.Bicubic;

	public bool Compand { get; set; }

	public Rectangle? TargetRectangle { get; set; }

	public bool PremultiplyAlpha { get; set; } = true;

	public Color PadColor { get; set; }
}
