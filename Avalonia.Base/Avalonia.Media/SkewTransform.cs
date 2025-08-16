namespace Avalonia.Media;

public sealed class SkewTransform : Transform
{
	public static readonly StyledProperty<double> AngleXProperty = AvaloniaProperty.Register<SkewTransform, double>("AngleX", 0.0);

	public static readonly StyledProperty<double> AngleYProperty = AvaloniaProperty.Register<SkewTransform, double>("AngleY", 0.0);

	public double AngleX
	{
		get
		{
			return GetValue(AngleXProperty);
		}
		set
		{
			SetValue(AngleXProperty, value);
		}
	}

	public double AngleY
	{
		get
		{
			return GetValue(AngleYProperty);
		}
		set
		{
			SetValue(AngleYProperty, value);
		}
	}

	public override Matrix Value => Matrix.CreateSkew(Matrix.ToRadians(AngleX), Matrix.ToRadians(AngleY));

	public SkewTransform()
	{
	}

	public SkewTransform(double angleX, double angleY)
		: this()
	{
		AngleX = angleX;
		AngleY = angleY;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == AngleXProperty || change.Property == AngleYProperty)
		{
			RaiseChanged();
		}
	}
}
