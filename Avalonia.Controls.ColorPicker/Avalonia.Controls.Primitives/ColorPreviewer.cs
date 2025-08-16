using System;
using System.Globalization;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives.Converters;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives;

[TemplatePart("PART_AccentDecrement1Border", typeof(Border))]
[TemplatePart("PART_AccentDecrement2Border", typeof(Border))]
[TemplatePart("PART_AccentIncrement1Border", typeof(Border))]
[TemplatePart("PART_AccentIncrement2Border", typeof(Border))]
public class ColorPreviewer : TemplatedControl
{
	private bool eventsConnected;

	private Border? _accentDecrement1Border;

	private Border? _accentDecrement2Border;

	private Border? _accentIncrement1Border;

	private Border? _accentIncrement2Border;

	public static readonly StyledProperty<HsvColor> HsvColorProperty = AvaloniaProperty.Register<ColorPreviewer, HsvColor>("HsvColor", Colors.Transparent.ToHsv(), inherits: false, BindingMode.TwoWay);

	public static readonly StyledProperty<bool> IsAccentColorsVisibleProperty = AvaloniaProperty.Register<ColorPreviewer, bool>("IsAccentColorsVisible", defaultValue: true);

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

	public event EventHandler<ColorChangedEventArgs>? ColorChanged;

	private void ConnectEvents(bool connected)
	{
		if (connected && !eventsConnected)
		{
			if (_accentDecrement1Border != null)
			{
				_accentDecrement1Border.PointerPressed += AccentBorder_PointerPressed;
			}
			if (_accentDecrement2Border != null)
			{
				_accentDecrement2Border.PointerPressed += AccentBorder_PointerPressed;
			}
			if (_accentIncrement1Border != null)
			{
				_accentIncrement1Border.PointerPressed += AccentBorder_PointerPressed;
			}
			if (_accentIncrement2Border != null)
			{
				_accentIncrement2Border.PointerPressed += AccentBorder_PointerPressed;
			}
			eventsConnected = true;
		}
		else if (!connected && eventsConnected)
		{
			if (_accentDecrement1Border != null)
			{
				_accentDecrement1Border.PointerPressed -= AccentBorder_PointerPressed;
			}
			if (_accentDecrement2Border != null)
			{
				_accentDecrement2Border.PointerPressed -= AccentBorder_PointerPressed;
			}
			if (_accentIncrement1Border != null)
			{
				_accentIncrement1Border.PointerPressed -= AccentBorder_PointerPressed;
			}
			if (_accentIncrement2Border != null)
			{
				_accentIncrement2Border.PointerPressed -= AccentBorder_PointerPressed;
			}
			eventsConnected = false;
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		ConnectEvents(connected: false);
		_accentDecrement1Border = e.NameScope.Find<Border>("PART_AccentDecrement1Border");
		_accentDecrement2Border = e.NameScope.Find<Border>("PART_AccentDecrement2Border");
		_accentIncrement1Border = e.NameScope.Find<Border>("PART_AccentIncrement1Border");
		_accentIncrement2Border = e.NameScope.Find<Border>("PART_AccentIncrement2Border");
		ConnectEvents(connected: true);
		base.OnApplyTemplate(e);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == HsvColorProperty)
		{
			OnColorChanged(new ColorChangedEventArgs(change.GetOldValue<HsvColor>().ToRgb(), change.GetNewValue<HsvColor>().ToRgb()));
		}
		base.OnPropertyChanged(change);
	}

	protected virtual void OnColorChanged(ColorChangedEventArgs e)
	{
		this.ColorChanged?.Invoke(this, e);
	}

	private void AccentBorder_PointerPressed(object? sender, PointerPressedEventArgs e)
	{
		Border border = sender as Border;
		int num = 0;
		HsvColor hsvColor = HsvColor;
		try
		{
			num = int.Parse(border?.Tag?.ToString() ?? "0", CultureInfo.InvariantCulture);
		}
		catch
		{
		}
		if (num != 0)
		{
			SetCurrentValue(HsvColorProperty, AccentColorConverter.GetAccent(hsvColor, num));
		}
	}
}
