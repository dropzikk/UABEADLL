namespace Avalonia.Media;

public sealed class GradientStop : AvaloniaObject, IGradientStop
{
	public static readonly StyledProperty<double> OffsetProperty = AvaloniaProperty.Register<GradientStop, double>("Offset", 0.0);

	public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<GradientStop, Color>("Color");

	public double Offset
	{
		get
		{
			return GetValue(OffsetProperty);
		}
		set
		{
			SetValue(OffsetProperty, value);
		}
	}

	public Color Color
	{
		get
		{
			return GetValue(ColorProperty);
		}
		set
		{
			SetValue(ColorProperty, value);
		}
	}

	public GradientStop()
	{
	}

	public GradientStop(Color color, double offset)
	{
		Color = color;
		Offset = offset;
	}
}
