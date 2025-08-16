using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace Avalonia.Controls.Documents;

internal class EmbeddedControlRun : DrawableTextRun
{
	public Control Control { get; }

	public override TextRunProperties? Properties { get; }

	public override Size Size => Control.DesiredSize;

	public override double Baseline
	{
		get
		{
			double num = Size.Height;
			double value = Control.GetValue(TextBlock.BaselineOffsetProperty);
			if (!MathUtilities.IsZero(value))
			{
				num = value;
			}
			return 0.0 - num;
		}
	}

	public EmbeddedControlRun(Control control, TextRunProperties properties)
	{
		Control = control;
		Properties = properties;
	}

	public override void Draw(DrawingContext drawingContext, Point origin)
	{
	}
}
