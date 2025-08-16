namespace Avalonia.Media;

public sealed class TranslateTransform : Transform
{
	public static readonly StyledProperty<double> XProperty = AvaloniaProperty.Register<TranslateTransform, double>("X", 0.0);

	public static readonly StyledProperty<double> YProperty = AvaloniaProperty.Register<TranslateTransform, double>("Y", 0.0);

	public double X
	{
		get
		{
			return GetValue(XProperty);
		}
		set
		{
			SetValue(XProperty, value);
		}
	}

	public double Y
	{
		get
		{
			return GetValue(YProperty);
		}
		set
		{
			SetValue(YProperty, value);
		}
	}

	public override Matrix Value => Matrix.CreateTranslation(X, Y);

	public TranslateTransform()
	{
	}

	public TranslateTransform(double x, double y)
		: this()
	{
		X = x;
		Y = y;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == XProperty || change.Property == YProperty)
		{
			RaiseChanged();
		}
	}
}
