using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

public class InlineObjectRun : DrawableTextRun
{
	internal Size DesiredSize;

	public Control Element { get; }

	public VisualLine? VisualLine { get; internal set; }

	public override TextRunProperties? Properties { get; }

	public override int Length { get; }

	public override double Baseline
	{
		get
		{
			double num = TextBlock.GetBaselineOffset(Element);
			if (double.IsNaN(num))
			{
				num = DesiredSize.Height;
			}
			return num;
		}
	}

	public override Size Size => DesiredSize;

	public InlineObjectRun(int length, TextRunProperties? properties, Control element)
	{
		if (length <= 0)
		{
			throw new ArgumentOutOfRangeException("length", length, "Value must be positive");
		}
		Length = length;
		Properties = properties ?? throw new ArgumentNullException("properties");
		Element = element ?? throw new ArgumentNullException("element");
		DesiredSize = element.DesiredSize;
	}

	public override void Draw(DrawingContext drawingContext, Point origin)
	{
	}
}
