using System;
using Avalonia.Reactive;

namespace Avalonia.Media;

public class ExperimentalAcrylicMaterial : AvaloniaObject, IMutableExperimentalAcrylicMaterial, IExperimentalAcrylicMaterial
{
	private Color _effectiveTintColor;

	private Color _effectiveLuminosityColor;

	public static readonly StyledProperty<Color> TintColorProperty;

	public static readonly StyledProperty<AcrylicBackgroundSource> BackgroundSourceProperty;

	public static readonly StyledProperty<double> TintOpacityProperty;

	public static readonly StyledProperty<double> MaterialOpacityProperty;

	public static readonly StyledProperty<double> PlatformTransparencyCompensationLevelProperty;

	public static readonly StyledProperty<Color> FallbackColorProperty;

	public AcrylicBackgroundSource BackgroundSource
	{
		get
		{
			return GetValue(BackgroundSourceProperty);
		}
		set
		{
			SetValue(BackgroundSourceProperty, value);
		}
	}

	public Color TintColor
	{
		get
		{
			return GetValue(TintColorProperty);
		}
		set
		{
			SetValue(TintColorProperty, value);
		}
	}

	public double TintOpacity
	{
		get
		{
			return GetValue(TintOpacityProperty);
		}
		set
		{
			SetValue(TintOpacityProperty, value);
		}
	}

	public Color FallbackColor
	{
		get
		{
			return GetValue(FallbackColorProperty);
		}
		set
		{
			SetValue(FallbackColorProperty, value);
		}
	}

	public double MaterialOpacity
	{
		get
		{
			return GetValue(MaterialOpacityProperty);
		}
		set
		{
			SetValue(MaterialOpacityProperty, value);
		}
	}

	public double PlatformTransparencyCompensationLevel
	{
		get
		{
			return GetValue(PlatformTransparencyCompensationLevelProperty);
		}
		set
		{
			SetValue(PlatformTransparencyCompensationLevelProperty, value);
		}
	}

	Color IExperimentalAcrylicMaterial.MaterialColor => _effectiveLuminosityColor;

	Color IExperimentalAcrylicMaterial.TintColor => _effectiveTintColor;

	public event EventHandler? Invalidated;

	static ExperimentalAcrylicMaterial()
	{
		TintColorProperty = AvaloniaProperty.Register<ExperimentalAcrylicMaterial, Color>("TintColor");
		BackgroundSourceProperty = AvaloniaProperty.Register<ExperimentalAcrylicMaterial, AcrylicBackgroundSource>("BackgroundSource", AcrylicBackgroundSource.None);
		TintOpacityProperty = AvaloniaProperty.Register<ExperimentalAcrylicMaterial, double>("TintOpacity", 0.8);
		MaterialOpacityProperty = AvaloniaProperty.Register<ExperimentalAcrylicMaterial, double>("MaterialOpacity", 0.5);
		PlatformTransparencyCompensationLevelProperty = AvaloniaProperty.Register<ExperimentalAcrylicMaterial, double>("PlatformTransparencyCompensationLevel", 0.0);
		FallbackColorProperty = AvaloniaProperty.Register<ExperimentalAcrylicMaterial, Color>("FallbackColor");
		AffectsRender<ExperimentalAcrylicMaterial>(new AvaloniaProperty[5] { TintColorProperty, BackgroundSourceProperty, TintOpacityProperty, MaterialOpacityProperty, PlatformTransparencyCompensationLevelProperty });
		TintColorProperty.Changed.AddClassHandler(delegate(ExperimentalAcrylicMaterial b, AvaloniaPropertyChangedEventArgs e)
		{
			b._effectiveTintColor = GetEffectiveTintColor(b.TintColor, b.TintOpacity);
			b._effectiveLuminosityColor = b.GetEffectiveLuminosityColor();
		});
		TintOpacityProperty.Changed.AddClassHandler(delegate(ExperimentalAcrylicMaterial b, AvaloniaPropertyChangedEventArgs e)
		{
			b._effectiveTintColor = GetEffectiveTintColor(b.TintColor, b.TintOpacity);
			b._effectiveLuminosityColor = b.GetEffectiveLuminosityColor();
		});
		MaterialOpacityProperty.Changed.AddClassHandler(delegate(ExperimentalAcrylicMaterial b, AvaloniaPropertyChangedEventArgs e)
		{
			b._effectiveTintColor = GetEffectiveTintColor(b.TintColor, b.TintOpacity);
			b._effectiveLuminosityColor = b.GetEffectiveLuminosityColor();
		});
		PlatformTransparencyCompensationLevelProperty.Changed.AddClassHandler(delegate(ExperimentalAcrylicMaterial b, AvaloniaPropertyChangedEventArgs e)
		{
			b._effectiveTintColor = GetEffectiveTintColor(b.TintColor, b.TintOpacity);
			b._effectiveLuminosityColor = b.GetEffectiveLuminosityColor();
		});
	}

	private static Color GetEffectiveTintColor(Color tintColor, double tintOpacity)
	{
		double tintOpacityModifier = GetTintOpacityModifier(tintColor);
		return new Color((byte)(255.0 * (255.0 / (double)(int)tintColor.A * tintOpacity) * tintOpacityModifier), tintColor.R, tintColor.G, tintColor.B);
	}

	private static double GetTintOpacityModifier(Color tintColor)
	{
		HsvColor hsvColor = tintColor.ToHsv();
		double result = 0.45;
		if (hsvColor.V != 0.5)
		{
			double num = 0.45;
			double num2 = 0.5;
			if (hsvColor.V > 0.5)
			{
				num = 0.2;
				num2 = 1.0 - num2;
			}
			else if (hsvColor.V < 0.5)
			{
				num = 0.45;
			}
			double num3 = 0.45 - num;
			double num4 = Math.Abs(hsvColor.V - 0.5) / num2;
			if (hsvColor.S > 0.0)
			{
				num3 *= Math.Max(1.0 - hsvColor.S * 2.0, 0.0);
			}
			double num5 = num3 * num4;
			result = 0.45 - num5;
		}
		return result;
	}

	private Color GetEffectiveLuminosityColor()
	{
		double? luminosityOpacity = MaterialOpacity;
		return GetLuminosityColor(luminosityOpacity);
	}

	private static byte Trim(double value)
	{
		value = Math.Min(Math.Floor(value * 256.0), 255.0);
		if (value < 0.0)
		{
			return 0;
		}
		if (value > 255.0)
		{
			return byte.MaxValue;
		}
		return (byte)value;
	}

	private static float RGBMax(Color color)
	{
		if (color.R > color.G)
		{
			return (int)((color.R > color.B) ? color.R : color.B);
		}
		return (int)((color.G > color.B) ? color.G : color.B);
	}

	private static float RGBMin(Color color)
	{
		if (color.R < color.G)
		{
			return (int)((color.R < color.B) ? color.R : color.B);
		}
		return (int)((color.G < color.B) ? color.G : color.B);
	}

	private Color GetLuminosityColor(double? luminosityOpacity)
	{
		float num = RGBMax(TintColor) / 255f;
		float num2 = RGBMin(TintColor) / 255f;
		double num3 = (double)(num + num2) / 2.0;
		num3 = 1.0 - (1.0 - num3) * (luminosityOpacity ?? 1.0);
		num3 = 0.13 + num3 * 0.74;
		Color color = new Color(byte.MaxValue, Trim(num3), Trim(num3), Trim(num3));
		double num4 = 1.0 - PlatformTransparencyCompensationLevel;
		return new Color((byte)(255.0 * Math.Max(Math.Min(PlatformTransparencyCompensationLevel + (luminosityOpacity ?? 1.0) * num4, 1.0), 0.0)), color.R, color.G, color.B);
	}

	protected static void AffectsRender<T>(params AvaloniaProperty[] properties) where T : ExperimentalAcrylicMaterial
	{
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			(e.Sender as T)?.RaiseInvalidated(EventArgs.Empty);
		});
		for (int i = 0; i < properties.Length; i++)
		{
			properties[i].Changed.Subscribe(observer);
		}
	}

	protected void RaiseInvalidated(EventArgs e)
	{
		this.Invalidated?.Invoke(this, e);
	}

	public IExperimentalAcrylicMaterial ToImmutable()
	{
		return new ImmutableExperimentalAcrylicMaterial(this);
	}
}
