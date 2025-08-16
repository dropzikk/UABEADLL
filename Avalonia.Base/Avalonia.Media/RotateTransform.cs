using Avalonia.Reactive;

namespace Avalonia.Media;

public sealed class RotateTransform : Transform
{
	public static readonly StyledProperty<double> AngleProperty = AvaloniaProperty.Register<RotateTransform, double>("Angle", 0.0);

	public static readonly StyledProperty<double> CenterXProperty = AvaloniaProperty.Register<RotateTransform, double>("CenterX", 0.0);

	public static readonly StyledProperty<double> CenterYProperty = AvaloniaProperty.Register<RotateTransform, double>("CenterY", 0.0);

	public double Angle
	{
		get
		{
			return GetValue(AngleProperty);
		}
		set
		{
			SetValue(AngleProperty, value);
		}
	}

	public double CenterX
	{
		get
		{
			return GetValue(CenterXProperty);
		}
		set
		{
			SetValue(CenterXProperty, value);
		}
	}

	public double CenterY
	{
		get
		{
			return GetValue(CenterYProperty);
		}
		set
		{
			SetValue(CenterYProperty, value);
		}
	}

	public override Matrix Value => Matrix.CreateTranslation(0.0 - CenterX, 0.0 - CenterY) * Matrix.CreateRotation(Matrix.ToRadians(Angle)) * Matrix.CreateTranslation(CenterX, CenterY);

	public RotateTransform()
	{
		this.GetObservable(AngleProperty).Subscribe(delegate
		{
			RaiseChanged();
		});
	}

	public RotateTransform(double angle)
		: this()
	{
		Angle = angle;
	}

	public RotateTransform(double angle, double centerX, double centerY)
		: this()
	{
		Angle = angle;
		CenterX = centerX;
		CenterY = centerY;
	}
}
