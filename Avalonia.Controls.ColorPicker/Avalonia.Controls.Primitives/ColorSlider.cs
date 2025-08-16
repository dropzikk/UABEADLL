using System;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Utilities;

namespace Avalonia.Controls.Primitives;

[PseudoClasses(new string[] { ":dark-selector", ":light-selector" })]
public class ColorSlider : Slider
{
	protected const string pcDarkSelector = ":dark-selector";

	protected const string pcLightSelector = ":light-selector";

	private const double MaxHue = 359.0;

	protected bool ignorePropertyChanged;

	private WriteableBitmap? _backgroundBitmap;

	public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<ColorSlider, Color>("Color", Colors.White, inherits: false, BindingMode.TwoWay);

	public static readonly StyledProperty<ColorComponent> ColorComponentProperty = AvaloniaProperty.Register<ColorSlider, ColorComponent>("ColorComponent", ColorComponent.Component1);

	public static readonly StyledProperty<ColorModel> ColorModelProperty = AvaloniaProperty.Register<ColorSlider, ColorModel>("ColorModel", ColorModel.Rgba);

	public static readonly StyledProperty<HsvColor> HsvColorProperty = AvaloniaProperty.Register<ColorSlider, HsvColor>("HsvColor", Colors.White.ToHsv(), inherits: false, BindingMode.TwoWay);

	public static readonly StyledProperty<bool> IsAlphaVisibleProperty = AvaloniaProperty.Register<ColorSlider, bool>("IsAlphaVisible", defaultValue: false);

	public static readonly StyledProperty<bool> IsPerceptiveProperty = AvaloniaProperty.Register<ColorSlider, bool>("IsPerceptive", defaultValue: true);

	public static readonly StyledProperty<bool> IsRoundingEnabledProperty = AvaloniaProperty.Register<ColorSlider, bool>("IsRoundingEnabled", defaultValue: false);

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

	public ColorComponent ColorComponent
	{
		get
		{
			return GetValue(ColorComponentProperty);
		}
		set
		{
			SetValue(ColorComponentProperty, value);
		}
	}

	public ColorModel ColorModel
	{
		get
		{
			return GetValue(ColorModelProperty);
		}
		set
		{
			SetValue(ColorModelProperty, value);
		}
	}

	public HsvColor HsvColor
	{
		get
		{
			return GetValue(HsvColorProperty);
		}
		set
		{
			SetValue(HsvColorProperty, value);
		}
	}

	public bool IsAlphaVisible
	{
		get
		{
			return GetValue(IsAlphaVisibleProperty);
		}
		set
		{
			SetValue(IsAlphaVisibleProperty, value);
		}
	}

	public bool IsPerceptive
	{
		get
		{
			return GetValue(IsPerceptiveProperty);
		}
		set
		{
			SetValue(IsPerceptiveProperty, value);
		}
	}

	public bool IsRoundingEnabled
	{
		get
		{
			return GetValue(IsRoundingEnabledProperty);
		}
		set
		{
			SetValue(IsRoundingEnabledProperty, value);
		}
	}

	public event EventHandler<ColorChangedEventArgs>? ColorChanged;

	private void UpdatePseudoClasses()
	{
		if (Color.A < 128 && (IsAlphaVisible || ColorComponent == ColorComponent.Alpha))
		{
			base.PseudoClasses.Set(":dark-selector", value: false);
			base.PseudoClasses.Set(":light-selector", value: false);
			return;
		}
		Color color = ((ColorModel != 0) ? GetPerceptiveBackgroundColor(Color) : GetPerceptiveBackgroundColor(HsvColor).ToRgb());
		if (ColorHelper.GetRelativeLuminance(color) <= 0.5)
		{
			base.PseudoClasses.Set(":dark-selector", value: false);
			base.PseudoClasses.Set(":light-selector", value: true);
		}
		else
		{
			base.PseudoClasses.Set(":dark-selector", value: true);
			base.PseudoClasses.Set(":light-selector", value: false);
		}
	}

	private async void UpdateBackground()
	{
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		int pixelWidth;
		int pixelHeight;
		if (base.Track != null)
		{
			pixelWidth = Convert.ToInt32(base.Track.Bounds.Width * layoutScale);
			pixelHeight = Convert.ToInt32(base.Track.Bounds.Height * layoutScale);
		}
		else
		{
			pixelWidth = Convert.ToInt32(base.Bounds.Width * layoutScale);
			pixelHeight = Convert.ToInt32(base.Bounds.Height * layoutScale);
		}
		if (pixelWidth != 0 && pixelHeight != 0)
		{
			ArrayList<byte> bgraPixelData = await ColorPickerHelpers.CreateComponentBitmapAsync(pixelWidth, pixelHeight, base.Orientation, ColorModel, ColorComponent, HsvColor, IsAlphaVisible, IsPerceptive);
			if (_backgroundBitmap != null)
			{
				_backgroundBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraPixelData, pixelWidth, pixelHeight);
			}
			else
			{
				_backgroundBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraPixelData, pixelWidth, pixelHeight);
			}
			base.Background = new ImageBrush(_backgroundBitmap);
		}
	}

	private static HsvColor RoundComponentValues(HsvColor hsvColor)
	{
		return new HsvColor(Math.Round(hsvColor.A, 2, MidpointRounding.AwayFromZero), Math.Round(hsvColor.H, 0, MidpointRounding.AwayFromZero), Math.Round(hsvColor.S, 2, MidpointRounding.AwayFromZero), Math.Round(hsvColor.V, 2, MidpointRounding.AwayFromZero));
	}

	private void SetColorToSliderValues()
	{
		ColorComponent colorComponent = ColorComponent;
		if (ColorModel == ColorModel.Hsva)
		{
			HsvColor hsvColor = HsvColor;
			if (IsRoundingEnabled)
			{
				hsvColor = RoundComponentValues(hsvColor);
			}
			switch (colorComponent)
			{
			case ColorComponent.Alpha:
				base.Minimum = 0.0;
				base.Maximum = 100.0;
				base.Value = hsvColor.A * 100.0;
				break;
			case ColorComponent.Component1:
				base.Minimum = 0.0;
				base.Maximum = 359.0;
				base.Value = hsvColor.H;
				break;
			case ColorComponent.Component2:
				base.Minimum = 0.0;
				base.Maximum = 100.0;
				base.Value = hsvColor.S * 100.0;
				break;
			case ColorComponent.Component3:
				base.Minimum = 0.0;
				base.Maximum = 100.0;
				base.Value = hsvColor.V * 100.0;
				break;
			}
		}
		else
		{
			Color color = Color;
			switch (colorComponent)
			{
			case ColorComponent.Alpha:
				base.Minimum = 0.0;
				base.Maximum = 255.0;
				base.Value = Convert.ToDouble(color.A);
				break;
			case ColorComponent.Component1:
				base.Minimum = 0.0;
				base.Maximum = 255.0;
				base.Value = Convert.ToDouble(color.R);
				break;
			case ColorComponent.Component2:
				base.Minimum = 0.0;
				base.Maximum = 255.0;
				base.Value = Convert.ToDouble(color.G);
				break;
			case ColorComponent.Component3:
				base.Minimum = 0.0;
				base.Maximum = 255.0;
				base.Value = Convert.ToDouble(color.B);
				break;
			}
		}
	}

	private (Color, HsvColor) GetColorFromSliderValues()
	{
		HsvColor hsvColor = default(HsvColor);
		Color item = default(Color);
		double num = base.Value / (base.Maximum - base.Minimum);
		ColorComponent colorComponent = ColorComponent;
		if (ColorModel == ColorModel.Hsva)
		{
			HsvColor hsvColor2 = HsvColor;
			switch (colorComponent)
			{
			case ColorComponent.Alpha:
				hsvColor = new HsvColor(num, hsvColor2.H, hsvColor2.S, hsvColor2.V);
				break;
			case ColorComponent.Component1:
				hsvColor = new HsvColor(hsvColor2.A, num * 359.0, hsvColor2.S, hsvColor2.V);
				break;
			case ColorComponent.Component2:
				hsvColor = new HsvColor(hsvColor2.A, hsvColor2.H, num, hsvColor2.V);
				break;
			case ColorComponent.Component3:
				hsvColor = new HsvColor(hsvColor2.A, hsvColor2.H, hsvColor2.S, num);
				break;
			}
			item = hsvColor.ToRgb();
		}
		else
		{
			Color color = Color;
			byte b = Convert.ToByte(MathUtilities.Clamp(num * 255.0, 0.0, 255.0));
			switch (colorComponent)
			{
			case ColorComponent.Alpha:
				item = new Color(b, color.R, color.G, color.B);
				break;
			case ColorComponent.Component1:
				item = new Color(color.A, b, color.G, color.B);
				break;
			case ColorComponent.Component2:
				item = new Color(color.A, color.R, b, color.B);
				break;
			case ColorComponent.Component3:
				item = new Color(color.A, color.R, color.G, b);
				break;
			}
			hsvColor = item.ToHsv();
		}
		if (IsRoundingEnabled)
		{
			hsvColor = RoundComponentValues(hsvColor);
		}
		return (item, hsvColor);
	}

	private HsvColor GetPerceptiveBackgroundColor(HsvColor hsvColor)
	{
		ColorComponent colorComponent = ColorComponent;
		bool isAlphaVisible = IsAlphaVisible;
		bool isPerceptive = IsPerceptive;
		if (!isAlphaVisible && colorComponent != 0)
		{
			hsvColor = new HsvColor(1.0, hsvColor.H, hsvColor.S, hsvColor.V);
		}
		if (isPerceptive)
		{
			return colorComponent switch
			{
				ColorComponent.Component1 => new HsvColor(hsvColor.A, hsvColor.H, 1.0, 1.0), 
				ColorComponent.Component2 => new HsvColor(hsvColor.A, hsvColor.H, hsvColor.S, 1.0), 
				ColorComponent.Component3 => new HsvColor(hsvColor.A, hsvColor.H, 1.0, hsvColor.V), 
				_ => hsvColor, 
			};
		}
		return hsvColor;
	}

	private Color GetPerceptiveBackgroundColor(Color rgbColor)
	{
		ColorComponent colorComponent = ColorComponent;
		bool isAlphaVisible = IsAlphaVisible;
		bool isPerceptive = IsPerceptive;
		if (!isAlphaVisible && colorComponent != 0)
		{
			rgbColor = new Color(byte.MaxValue, rgbColor.R, rgbColor.G, rgbColor.B);
		}
		if (isPerceptive)
		{
			return colorComponent switch
			{
				ColorComponent.Component1 => new Color(rgbColor.A, rgbColor.R, 0, 0), 
				ColorComponent.Component2 => new Color(rgbColor.A, 0, rgbColor.G, 0), 
				ColorComponent.Component3 => new Color(rgbColor.A, 0, 0, rgbColor.B), 
				_ => rgbColor, 
			};
		}
		return rgbColor;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (ignorePropertyChanged)
		{
			base.OnPropertyChanged(change);
			return;
		}
		if (change.Property == ColorProperty)
		{
			ignorePropertyChanged = true;
			SetCurrentValue(HsvColorProperty, Color.ToHsv());
			SetColorToSliderValues();
			UpdateBackground();
			UpdatePseudoClasses();
			OnColorChanged(new ColorChangedEventArgs(change.GetOldValue<Color>(), change.GetNewValue<Color>()));
			ignorePropertyChanged = false;
		}
		else if (change.Property == ColorComponentProperty || change.Property == ColorModelProperty || change.Property == IsAlphaVisibleProperty || change.Property == IsPerceptiveProperty)
		{
			ignorePropertyChanged = true;
			SetColorToSliderValues();
			UpdateBackground();
			UpdatePseudoClasses();
			ignorePropertyChanged = false;
		}
		else if (change.Property == HsvColorProperty)
		{
			ignorePropertyChanged = true;
			SetCurrentValue(ColorProperty, HsvColor.ToRgb());
			SetColorToSliderValues();
			UpdateBackground();
			UpdatePseudoClasses();
			OnColorChanged(new ColorChangedEventArgs(change.GetOldValue<HsvColor>().ToRgb(), change.GetNewValue<HsvColor>().ToRgb()));
			ignorePropertyChanged = false;
		}
		else if (change.Property == IsRoundingEnabledProperty)
		{
			SetColorToSliderValues();
		}
		else if (change.Property == Visual.BoundsProperty)
		{
			_backgroundBitmap?.Dispose();
			_backgroundBitmap = null;
			UpdateBackground();
			UpdatePseudoClasses();
		}
		else if (change.Property == RangeBase.ValueProperty || change.Property == RangeBase.MinimumProperty || change.Property == RangeBase.MaximumProperty)
		{
			ignorePropertyChanged = true;
			Color color = Color;
			var (value, value2) = GetColorFromSliderValues();
			if (ColorModel == ColorModel.Hsva)
			{
				SetCurrentValue(HsvColorProperty, value2);
				SetCurrentValue(ColorProperty, value2.ToRgb());
			}
			else
			{
				SetCurrentValue(ColorProperty, value);
				SetCurrentValue(HsvColorProperty, value.ToHsv());
			}
			UpdatePseudoClasses();
			OnColorChanged(new ColorChangedEventArgs(color, Color));
			ignorePropertyChanged = false;
		}
		base.OnPropertyChanged(change);
	}

	protected virtual void OnColorChanged(ColorChangedEventArgs e)
	{
		this.ColorChanged?.Invoke(this, e);
	}
}
