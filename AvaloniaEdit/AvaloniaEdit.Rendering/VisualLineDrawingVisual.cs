using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

internal sealed class VisualLineDrawingVisual : Control
{
	public VisualLine VisualLine { get; }

	public double LineHeight { get; }

	internal bool IsAdded { get; set; }

	public VisualLineDrawingVisual(VisualLine visualLine)
	{
		VisualLine = visualLine;
		LineHeight = VisualLine.TextLines.Sum((TextLine textLine) => textLine.Height);
	}

	public override void Render(DrawingContext context)
	{
		double num = 0.0;
		foreach (TextLine textLine in VisualLine.TextLines)
		{
			textLine.Draw(context, new Point(0.0, num));
			num += textLine.Height;
		}
	}
}
