using System;
using System.Collections.Generic;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace Avalonia.Controls;

[TemplatePart("PART_HexTextBox", typeof(TextBox))]
[TemplatePart("PART_TabControl", typeof(TabControl))]
public class ColorView : TemplatedControl
{
	private TextBox? _hexTextBox;

	private TabControl? _tabControl;

	protected bool _ignorePropertyChanged;

	public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<ColorView, Color>("Color", Colors.White, inherits: false, BindingMode.TwoWay, null, CoerceColor);

	public static readonly StyledProperty<ColorModel> ColorModelProperty = AvaloniaProperty.Register<ColorView, ColorModel>("ColorModel", ColorModel.Rgba);

	public static readonly StyledProperty<ColorSpectrumComponents> ColorSpectrumComponentsProperty = AvaloniaProperty.Register<ColorView, ColorSpectrumComponents>("ColorSpectrumComponents", ColorSpectrumComponents.HueSaturation);

	public static readonly StyledProperty<ColorSpectrumShape> ColorSpectrumShapeProperty = AvaloniaProperty.Register<ColorView, ColorSpectrumShape>("ColorSpectrumShape", ColorSpectrumShape.Box);

	public static readonly StyledProperty<AlphaComponentPosition> HexInputAlphaPositionProperty = AvaloniaProperty.Register<ColorView, AlphaComponentPosition>("HexInputAlphaPosition", AlphaComponentPosition.Leading);

	public static readonly StyledProperty<HsvColor> HsvColorProperty = AvaloniaProperty.Register<ColorView, HsvColor>("HsvColor", Colors.White.ToHsv(), inherits: false, BindingMode.TwoWay, null, CoerceHsvColor);

	public static readonly StyledProperty<bool> IsAccentColorsVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsAccentColorsVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsAlphaEnabledProperty = AvaloniaProperty.Register<ColorView, bool>("IsAlphaEnabled", defaultValue: true);

	public static readonly StyledProperty<bool> IsAlphaVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsAlphaVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsColorComponentsVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsColorComponentsVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsColorModelVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsColorModelVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsColorPaletteVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsColorPaletteVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsColorPreviewVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsColorPreviewVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsColorSpectrumVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsColorSpectrumVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsColorSpectrumSliderVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsColorSpectrumSliderVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsComponentSliderVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsComponentSliderVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsComponentTextInputVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsComponentTextInputVisible", defaultValue: true);

	public static readonly StyledProperty<bool> IsHexInputVisibleProperty = AvaloniaProperty.Register<ColorView, bool>("IsHexInputVisible", defaultValue: true);

	public static readonly StyledProperty<int> MaxHueProperty = AvaloniaProperty.Register<ColorView, int>("MaxHue", 359);

	public static readonly StyledProperty<int> MaxSaturationProperty = AvaloniaProperty.Register<ColorView, int>("MaxSaturation", 100);

	public static readonly StyledProperty<int> MaxValueProperty = AvaloniaProperty.Register<ColorView, int>("MaxValue", 100);

	public static readonly StyledProperty<int> MinHueProperty = AvaloniaProperty.Register<ColorView, int>("MinHue", 0);

	public static readonly StyledProperty<int> MinSaturationProperty = AvaloniaProperty.Register<ColorView, int>("MinSaturation", 0);

	public static readonly StyledProperty<int> MinValueProperty = AvaloniaProperty.Register<ColorView, int>("MinValue", 0);

	public static readonly StyledProperty<IEnumerable<Color>?> PaletteColorsProperty = AvaloniaProperty.Register<ColorView, IEnumerable<Color>>("PaletteColors");

	public static readonly StyledProperty<int> PaletteColumnCountProperty = AvaloniaProperty.Register<ColorView, int>("PaletteColumnCount", 4);

	public static readonly StyledProperty<IColorPalette?> PaletteProperty = AvaloniaProperty.Register<ColorView, IColorPalette>("Palette");

	public static readonly StyledProperty<int> SelectedIndexProperty = AvaloniaProperty.Register<ColorView, int>("SelectedIndex", 0);

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

	public ColorSpectrumComponents ColorSpectrumComponents
	{
		get
		{
			return GetValue(ColorSpectrumComponentsProperty);
		}
		set
		{
			SetValue(ColorSpectrumComponentsProperty, value);
		}
	}

	public ColorSpectrumShape ColorSpectrumShape
	{
		get
		{
			return GetValue(ColorSpectrumShapeProperty);
		}
		set
		{
			SetValue(ColorSpectrumShapeProperty, value);
		}
	}

	public AlphaComponentPosition HexInputAlphaPosition
	{
		get
		{
			return GetValue(HexInputAlphaPositionProperty);
		}
		set
		{
			SetValue(HexInputAlphaPositionProperty, value);
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

	public bool IsAccentColorsVisible
	{
		get
		{
			return GetValue(IsAccentColorsVisibleProperty);
		}
		set
		{
			SetValue(IsAccentColorsVisibleProperty, value);
		}
	}

	public bool IsAlphaEnabled
	{
		get
		{
			return GetValue(IsAlphaEnabledProperty);
		}
		set
		{
			SetValue(IsAlphaEnabledProperty, value);
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

	public bool IsColorComponentsVisible
	{
		get
		{
			return GetValue(IsColorComponentsVisibleProperty);
		}
		set
		{
			SetValue(IsColorComponentsVisibleProperty, value);
		}
	}

	public bool IsColorModelVisible
	{
		get
		{
			return GetValue(IsColorModelVisibleProperty);
		}
		set
		{
			SetValue(IsColorModelVisibleProperty, value);
		}
	}

	public bool IsColorPaletteVisible
	{
		get
		{
			return GetValue(IsColorPaletteVisibleProperty);
		}
		set
		{
			SetValue(IsColorPaletteVisibleProperty, value);
		}
	}

	public bool IsColorPreviewVisible
	{
		get
		{
			return GetValue(IsColorPreviewVisibleProperty);
		}
		set
		{
			SetValue(IsColorPreviewVisibleProperty, value);
		}
	}

	public bool IsColorSpectrumVisible
	{
		get
		{
			return GetValue(IsColorSpectrumVisibleProperty);
		}
		set
		{
			SetValue(IsColorSpectrumVisibleProperty, value);
		}
	}

	public bool IsColorSpectrumSliderVisible
	{
		get
		{
			return GetValue(IsColorSpectrumSliderVisibleProperty);
		}
		set
		{
			SetValue(IsColorSpectrumSliderVisibleProperty, value);
		}
	}

	public bool IsComponentSliderVisible
	{
		get
		{
			return GetValue(IsComponentSliderVisibleProperty);
		}
		set
		{
			SetValue(IsComponentSliderVisibleProperty, value);
		}
	}

	public bool IsComponentTextInputVisible
	{
		get
		{
			return GetValue(IsComponentTextInputVisibleProperty);
		}
		set
		{
			SetValue(IsComponentTextInputVisibleProperty, value);
		}
	}

	public bool IsHexInputVisible
	{
		get
		{
			return GetValue(IsHexInputVisibleProperty);
		}
		set
		{
			SetValue(IsHexInputVisibleProperty, value);
		}
	}

	public int MaxHue
	{
		get
		{
			return GetValue(MaxHueProperty);
		}
		set
		{
			SetValue(MaxHueProperty, value);
		}
	}

	public int MaxSaturation
	{
		get
		{
			return GetValue(MaxSaturationProperty);
		}
		set
		{
			SetValue(MaxSaturationProperty, value);
		}
	}

	public int MaxValue
	{
		get
		{
			return GetValue(MaxValueProperty);
		}
		set
		{
			SetValue(MaxValueProperty, value);
		}
	}

	public int MinHue
	{
		get
		{
			return GetValue(MinHueProperty);
		}
		set
		{
			SetValue(MinHueProperty, value);
		}
	}

	public int MinSaturation
	{
		get
		{
			return GetValue(MinSaturationProperty);
		}
		set
		{
			SetValue(MinSaturationProperty, value);
		}
	}

	public int MinValue
	{
		get
		{
			return GetValue(MinValueProperty);
		}
		set
		{
			SetValue(MinValueProperty, value);
		}
	}

	public IEnumerable<Color>? PaletteColors
	{
		get
		{
			return GetValue(PaletteColorsProperty);
		}
		set
		{
			SetValue(PaletteColorsProperty, value);
		}
	}

	public int PaletteColumnCount
	{
		get
		{
			return GetValue(PaletteColumnCountProperty);
		}
		set
		{
			SetValue(PaletteColumnCountProperty, value);
		}
	}

	public IColorPalette? Palette
	{
		get
		{
			return GetValue(PaletteProperty);
		}
		set
		{
			SetValue(PaletteProperty, value);
		}
	}

	public int SelectedIndex
	{
		get
		{
			return GetValue(SelectedIndexProperty);
		}
		set
		{
			SetValue(SelectedIndexProperty, value);
		}
	}

	public event EventHandler<ColorChangedEventArgs>? ColorChanged;

	private void GetColorFromHexTextBox()
	{
		if (_hexTextBox != null)
		{
			Color? color = ColorToHexConverter.ParseHexString(_hexTextBox.Text ?? string.Empty, HexInputAlphaPosition);
			if (color.HasValue)
			{
				Color valueOrDefault = color.GetValueOrDefault();
				SetCurrentValue(ColorProperty, valueOrDefault);
			}
			SetColorToHexTextBox();
		}
	}

	private void SetColorToHexTextBox()
	{
		if (_hexTextBox != null)
		{
			_hexTextBox.Text = ColorToHexConverter.ToHexString(Color, HexInputAlphaPosition, IsAlphaEnabled && IsAlphaVisible);
		}
	}

	protected virtual void ValidateSelection()
	{
		if (_tabControl == null || _tabControl.Items == null)
		{
			return;
		}
		int num = 0;
		foreach (object item in _tabControl.Items)
		{
			if (item is Control { IsVisible: not false })
			{
				num++;
			}
		}
		if (num > 0)
		{
			object obj = null;
			if (_tabControl.SelectedItem == null && _tabControl.ItemCount > 0)
			{
				using IEnumerator<object> enumerator = _tabControl.Items.GetEnumerator();
				if (enumerator.MoveNext())
				{
					obj = enumerator.Current;
				}
			}
			else
			{
				obj = _tabControl.SelectedItem;
			}
			if (obj is Control { IsVisible: false })
			{
				foreach (object item2 in _tabControl.Items)
				{
					if (item2 is Control { IsVisible: not false })
					{
						obj = item2;
						break;
					}
				}
			}
			_tabControl.SelectedItem = obj;
			_tabControl.IsVisible = true;
		}
		else
		{
			_tabControl.SelectedItem = null;
			_tabControl.IsVisible = false;
		}
		SetCurrentValue(SelectedIndexProperty, _tabControl.SelectedIndex);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		if (_hexTextBox != null)
		{
			_hexTextBox.KeyDown -= HexTextBox_KeyDown;
			_hexTextBox.LostFocus -= HexTextBox_LostFocus;
		}
		_hexTextBox = e.NameScope.Find<TextBox>("PART_HexTextBox");
		_tabControl = e.NameScope.Find<TabControl>("PART_TabControl");
		SetColorToHexTextBox();
		if (_hexTextBox != null)
		{
			_hexTextBox.KeyDown += HexTextBox_KeyDown;
			_hexTextBox.LostFocus += HexTextBox_LostFocus;
		}
		base.OnApplyTemplate(e);
		ValidateSelection();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (_ignorePropertyChanged)
		{
			base.OnPropertyChanged(change);
			return;
		}
		if (change.Property == ColorProperty)
		{
			_ignorePropertyChanged = true;
			SetCurrentValue(HsvColorProperty, Color.ToHsv());
			SetColorToHexTextBox();
			OnColorChanged(new ColorChangedEventArgs(change.GetOldValue<Color>(), change.GetNewValue<Color>()));
			_ignorePropertyChanged = false;
		}
		else if (change.Property == HsvColorProperty)
		{
			_ignorePropertyChanged = true;
			SetCurrentValue(ColorProperty, HsvColor.ToRgb());
			SetColorToHexTextBox();
			OnColorChanged(new ColorChangedEventArgs(change.GetOldValue<HsvColor>().ToRgb(), change.GetNewValue<HsvColor>().ToRgb()));
			_ignorePropertyChanged = false;
		}
		else if (change.Property == PaletteProperty)
		{
			IColorPalette palette = Palette;
			if (palette != null)
			{
				SetCurrentValue(PaletteColumnCountProperty, palette.ColorCount);
				List<Color> list = new List<Color>();
				for (int i = 0; i < palette.ShadeCount; i++)
				{
					for (int j = 0; j < palette.ColorCount; j++)
					{
						list.Add(palette.GetColor(j, i));
					}
				}
				SetCurrentValue(PaletteColorsProperty, list);
			}
		}
		else if (change.Property == IsAlphaEnabledProperty)
		{
			SetCurrentValue(HsvColorProperty, OnCoerceHsvColor(HsvColor));
		}
		else if (change.Property == IsColorComponentsVisibleProperty || change.Property == IsColorPaletteVisibleProperty || change.Property == IsColorSpectrumVisibleProperty)
		{
			Dispatcher.UIThread.Post(delegate
			{
				ValidateSelection();
			}, DispatcherPriority.Background);
		}
		else if (change.Property == SelectedIndexProperty)
		{
			Dispatcher.UIThread.Post(delegate
			{
				ValidateSelection();
			}, DispatcherPriority.Background);
		}
		base.OnPropertyChanged(change);
	}

	protected virtual void OnColorChanged(ColorChangedEventArgs e)
	{
		this.ColorChanged?.Invoke(this, e);
	}

	protected virtual Color OnCoerceColor(Color value)
	{
		if (!IsAlphaEnabled)
		{
			return new Color(byte.MaxValue, value.R, value.G, value.B);
		}
		return value;
	}

	protected virtual HsvColor OnCoerceHsvColor(HsvColor value)
	{
		if (!IsAlphaEnabled)
		{
			return new HsvColor(1.0, value.H, value.S, value.V);
		}
		return value;
	}

	private static Color CoerceColor(AvaloniaObject instance, Color value)
	{
		if (instance is ColorView colorView)
		{
			return colorView.OnCoerceColor(value);
		}
		return value;
	}

	private static HsvColor CoerceHsvColor(AvaloniaObject instance, HsvColor value)
	{
		if (instance is ColorView colorView)
		{
			return colorView.OnCoerceHsvColor(value);
		}
		return value;
	}

	private void HexTextBox_KeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			GetColorFromHexTextBox();
		}
	}

	private void HexTextBox_LostFocus(object? sender, RoutedEventArgs e)
	{
		GetColorFromHexTextBox();
	}
}
