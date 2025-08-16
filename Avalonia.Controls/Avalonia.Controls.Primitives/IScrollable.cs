namespace Avalonia.Controls.Primitives;

public interface IScrollable
{
	Size Extent { get; }

	Vector Offset { get; set; }

	Size Viewport { get; }
}
