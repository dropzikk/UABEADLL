using Avalonia.Metadata;

namespace Avalonia.Input.Raw;

[PrivateApi]
public record struct RawPointerPoint
{
	public Point Position { get; set; }

	public float Twist { get; set; }

	public float Pressure { get; set; }

	public float XTilt { get; set; }

	public float YTilt { get; set; }

	public RawPointerPoint()
	{
		this = default(RawPointerPoint);
		Pressure = 0.5f;
	}
}
