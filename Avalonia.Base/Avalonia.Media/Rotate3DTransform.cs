using System;
using System.Numerics;

namespace Avalonia.Media;

public sealed class Rotate3DTransform : Transform
{
	private readonly bool _isInitializing;

	public static readonly StyledProperty<double> AngleXProperty = AvaloniaProperty.Register<Rotate3DTransform, double>("AngleX", 0.0);

	public static readonly StyledProperty<double> AngleYProperty = AvaloniaProperty.Register<Rotate3DTransform, double>("AngleY", 0.0);

	public static readonly StyledProperty<double> AngleZProperty = AvaloniaProperty.Register<Rotate3DTransform, double>("AngleZ", 0.0);

	public static readonly StyledProperty<double> CenterXProperty = AvaloniaProperty.Register<Rotate3DTransform, double>("CenterX", 0.0);

	public static readonly StyledProperty<double> CenterYProperty = AvaloniaProperty.Register<Rotate3DTransform, double>("CenterY", 0.0);

	public static readonly StyledProperty<double> CenterZProperty = AvaloniaProperty.Register<Rotate3DTransform, double>("CenterZ", 0.0);

	public static readonly StyledProperty<double> DepthProperty = AvaloniaProperty.Register<Rotate3DTransform, double>("Depth", 0.0);

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

	public double AngleZ
	{
		get
		{
			return GetValue(AngleZProperty);
		}
		set
		{
			SetValue(AngleZProperty, value);
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

	public double CenterZ
	{
		get
		{
			return GetValue(CenterZProperty);
		}
		set
		{
			SetValue(CenterZProperty, value);
		}
	}

	public double Depth
	{
		get
		{
			return GetValue(DepthProperty);
		}
		set
		{
			SetValue(DepthProperty, value);
		}
	}

	public override Matrix Value
	{
		get
		{
			Matrix4x4 identity = Matrix4x4.Identity;
			double centerX = CenterX;
			double centerY = CenterY;
			double centerZ = CenterZ;
			double angleX = AngleX;
			double angleY = AngleY;
			double angleZ = AngleZ;
			double depth = Depth;
			double num = centerX;
			double num2 = centerY;
			double num3 = centerZ;
			double num4 = angleX;
			double num5 = angleY;
			double num6 = angleZ;
			double num7 = depth;
			double value = num + num2 + num3;
			if (Math.Abs(value) > double.Epsilon)
			{
				identity *= Matrix4x4.CreateTranslation(0f - (float)num, 0f - (float)num2, 0f - (float)num3);
			}
			if (num4 != 0.0)
			{
				identity *= Matrix4x4.CreateRotationX((float)Matrix.ToRadians(num4));
			}
			if (num5 != 0.0)
			{
				identity *= Matrix4x4.CreateRotationY((float)Matrix.ToRadians(num5));
			}
			if (num6 != 0.0)
			{
				identity *= Matrix4x4.CreateRotationZ((float)Matrix.ToRadians(num6));
			}
			if (Math.Abs(value) > double.Epsilon)
			{
				identity *= Matrix4x4.CreateTranslation((float)num, (float)num2, (float)num3);
			}
			if (num7 != 0.0)
			{
				Matrix4x4 identity2 = Matrix4x4.Identity;
				identity2.M34 = -1f / (float)num7;
				identity *= identity2;
			}
			return new Matrix(identity.M11, identity.M12, identity.M14, identity.M21, identity.M22, identity.M24, identity.M41, identity.M42, identity.M44);
		}
	}

	public Rotate3DTransform()
	{
	}

	public Rotate3DTransform(double angleX, double angleY, double angleZ, double centerX, double centerY, double centerZ, double depth)
		: this()
	{
		_isInitializing = true;
		AngleX = angleX;
		AngleY = angleY;
		AngleZ = angleZ;
		CenterX = centerX;
		CenterY = centerY;
		CenterZ = centerZ;
		Depth = depth;
		_isInitializing = false;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (!_isInitializing)
		{
			RaiseChanged();
		}
	}
}
