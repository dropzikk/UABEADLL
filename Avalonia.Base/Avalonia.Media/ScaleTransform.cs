namespace Avalonia.Media;

public sealed class ScaleTransform : Transform
{
	public static readonly StyledProperty<double> ScaleXProperty = AvaloniaProperty.Register<ScaleTransform, double>("ScaleX", 1.0);

	public static readonly StyledProperty<double> ScaleYProperty = AvaloniaProperty.Register<ScaleTransform, double>("ScaleY", 1.0);

	public double ScaleX
	{
		get
		{
			return GetValue(ScaleXProperty);
		}
		set
		{
			SetValue(ScaleXProperty, value);
		}
	}

	public double ScaleY
	{
		get
		{
			return GetValue(ScaleYProperty);
		}
		set
		{
			SetValue(ScaleYProperty, value);
		}
	}

	public override Matrix Value => Matrix.CreateScale(ScaleX, ScaleY);

	public ScaleTransform()
	{
	}

	public ScaleTransform(double scaleX, double scaleY)
		: this()
	{
		ScaleX = scaleX;
		ScaleY = scaleY;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ScaleXProperty || change.Property == ScaleYProperty)
		{
			RaiseChanged();
		}
	}
}
