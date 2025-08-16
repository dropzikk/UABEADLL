using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace AvaloniaEdit.Editing;

public static class DottedLineMargin
{
	private static readonly object Tag = new object();

	public static Control Create()
	{
		return new Line
		{
			StartPoint = new Point(0.0, 0.0),
			EndPoint = new Point(0.0, 1.0),
			StrokeDashArray = new AvaloniaList<double> { 0.0, 2.0 },
			Stretch = Stretch.Fill,
			StrokeThickness = 1.0,
			StrokeLineCap = PenLineCap.Round,
			Margin = new Thickness(2.0, 0.0, 2.0, 0.0),
			Tag = Tag
		};
	}

	public static bool IsDottedLineMargin(Control element)
	{
		if (element is Line line)
		{
			return line.Tag == Tag;
		}
		return false;
	}
}
