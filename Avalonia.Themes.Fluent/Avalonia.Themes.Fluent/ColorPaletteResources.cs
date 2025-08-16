using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent.Accents;

namespace Avalonia.Themes.Fluent;

public class ColorPaletteResources : AvaloniaObject, IResourceNode
{
	private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>(StringComparer.InvariantCulture);

	private bool _hasAccentColor;

	private Color _accentColor;

	private Color _accentColorDark1;

	private Color _accentColorDark2;

	private Color _accentColorDark3;

	private Color _accentColorLight1;

	private Color _accentColorLight2;

	private Color _accentColorLight3;

	public static readonly DirectProperty<ColorPaletteResources, Color> AccentProperty = AvaloniaProperty.RegisterDirect("Accent", (ColorPaletteResources r) => r.Accent, delegate(ColorPaletteResources r, Color v)
	{
		r.Accent = v;
	});

	public bool HasResources
	{
		get
		{
			if (!_hasAccentColor)
			{
				return _colors.Count > 0;
			}
			return true;
		}
	}

	public Color Accent
	{
		get
		{
			return _accentColor;
		}
		set
		{
			SetAndRaise(AccentProperty, ref _accentColor, value);
		}
	}

	public Color AltHigh
	{
		get
		{
			return GetColor("SystemAltHighColor");
		}
		set
		{
			SetColor("SystemAltHighColor", value);
		}
	}

	public Color AltLow
	{
		get
		{
			return GetColor("SystemAltLowColor");
		}
		set
		{
			SetColor("SystemAltLowColor", value);
		}
	}

	public Color AltMedium
	{
		get
		{
			return GetColor("SystemAltMediumColor");
		}
		set
		{
			SetColor("SystemAltMediumColor", value);
		}
	}

	public Color AltMediumHigh
	{
		get
		{
			return GetColor("SystemAltMediumHighColor");
		}
		set
		{
			SetColor("SystemAltMediumHighColor", value);
		}
	}

	public Color AltMediumLow
	{
		get
		{
			return GetColor("SystemAltMediumLowColor");
		}
		set
		{
			SetColor("SystemAltMediumLowColor", value);
		}
	}

	public Color BaseHigh
	{
		get
		{
			return GetColor("SystemBaseHighColor");
		}
		set
		{
			SetColor("SystemBaseHighColor", value);
		}
	}

	public Color BaseLow
	{
		get
		{
			return GetColor("SystemBaseLowColor");
		}
		set
		{
			SetColor("SystemBaseLowColor", value);
		}
	}

	public Color BaseMedium
	{
		get
		{
			return GetColor("SystemBaseMediumColor");
		}
		set
		{
			SetColor("SystemBaseMediumColor", value);
		}
	}

	public Color BaseMediumHigh
	{
		get
		{
			return GetColor("SystemBaseMediumHighColor");
		}
		set
		{
			SetColor("SystemBaseMediumHighColor", value);
		}
	}

	public Color BaseMediumLow
	{
		get
		{
			return GetColor("SystemBaseMediumLowColor");
		}
		set
		{
			SetColor("SystemBaseMediumLowColor", value);
		}
	}

	public Color ChromeAltLow
	{
		get
		{
			return GetColor("SystemChromeAltLowColor");
		}
		set
		{
			SetColor("SystemChromeAltLowColor", value);
		}
	}

	public Color ChromeBlackHigh
	{
		get
		{
			return GetColor("SystemChromeBlackHighColor");
		}
		set
		{
			SetColor("SystemChromeBlackHighColor", value);
		}
	}

	public Color ChromeBlackLow
	{
		get
		{
			return GetColor("SystemChromeBlackLowColor");
		}
		set
		{
			SetColor("SystemChromeBlackLowColor", value);
		}
	}

	public Color ChromeBlackMedium
	{
		get
		{
			return GetColor("SystemChromeBlackMediumColor");
		}
		set
		{
			SetColor("SystemChromeBlackMediumColor", value);
		}
	}

	public Color ChromeBlackMediumLow
	{
		get
		{
			return GetColor("SystemChromeBlackMediumLowColor");
		}
		set
		{
			SetColor("SystemChromeBlackMediumLowColor", value);
		}
	}

	public Color ChromeDisabledHigh
	{
		get
		{
			return GetColor("SystemChromeDisabledHighColor");
		}
		set
		{
			SetColor("SystemChromeDisabledHighColor", value);
		}
	}

	public Color ChromeDisabledLow
	{
		get
		{
			return GetColor("SystemChromeDisabledLowColor");
		}
		set
		{
			SetColor("SystemChromeDisabledLowColor", value);
		}
	}

	public Color ChromeGray
	{
		get
		{
			return GetColor("SystemChromeGrayColor");
		}
		set
		{
			SetColor("SystemChromeGrayColor", value);
		}
	}

	public Color ChromeHigh
	{
		get
		{
			return GetColor("SystemChromeHighColor");
		}
		set
		{
			SetColor("SystemChromeHighColor", value);
		}
	}

	public Color ChromeLow
	{
		get
		{
			return GetColor("SystemChromeLowColor");
		}
		set
		{
			SetColor("SystemChromeLowColor", value);
		}
	}

	public Color ChromeMedium
	{
		get
		{
			return GetColor("SystemChromeMediumColor");
		}
		set
		{
			SetColor("SystemChromeMediumColor", value);
		}
	}

	public Color ChromeMediumLow
	{
		get
		{
			return GetColor("SystemChromeMediumLowColor");
		}
		set
		{
			SetColor("SystemChromeMediumLowColor", value);
		}
	}

	public Color ChromeWhite
	{
		get
		{
			return GetColor("SystemChromeWhiteColor");
		}
		set
		{
			SetColor("SystemChromeWhiteColor", value);
		}
	}

	public Color ErrorText
	{
		get
		{
			return GetColor("SystemErrorTextColor");
		}
		set
		{
			SetColor("SystemErrorTextColor", value);
		}
	}

	public Color ListLow
	{
		get
		{
			return GetColor("SystemListLowColor");
		}
		set
		{
			SetColor("SystemListLowColor", value);
		}
	}

	public Color ListMedium
	{
		get
		{
			return GetColor("SystemListMediumColor");
		}
		set
		{
			SetColor("SystemListMediumColor", value);
		}
	}

	public Color RegionColor
	{
		get
		{
			return GetColor("SystemRegionColor");
		}
		set
		{
			SetColor("SystemRegionColor", value);
		}
	}

	public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
	{
		if (key is string text)
		{
			if (text.Equals("SystemAccentColor", StringComparison.InvariantCulture))
			{
				value = _accentColor;
				return _hasAccentColor;
			}
			if (text.Equals("SystemAccentColorDark1", StringComparison.InvariantCulture))
			{
				value = _accentColorDark1;
				return _hasAccentColor;
			}
			if (text.Equals("SystemAccentColorDark2", StringComparison.InvariantCulture))
			{
				value = _accentColorDark2;
				return _hasAccentColor;
			}
			if (text.Equals("SystemAccentColorDark3", StringComparison.InvariantCulture))
			{
				value = _accentColorDark3;
				return _hasAccentColor;
			}
			if (text.Equals("SystemAccentColorLight1", StringComparison.InvariantCulture))
			{
				value = _accentColorLight1;
				return _hasAccentColor;
			}
			if (text.Equals("SystemAccentColorLight2", StringComparison.InvariantCulture))
			{
				value = _accentColorLight2;
				return _hasAccentColor;
			}
			if (text.Equals("SystemAccentColorLight3", StringComparison.InvariantCulture))
			{
				value = _accentColorLight3;
				return _hasAccentColor;
			}
			if (_colors.TryGetValue(text, out var value2))
			{
				value = value2;
				return true;
			}
		}
		value = null;
		return false;
	}

	private Color GetColor(string key)
	{
		if (_colors.TryGetValue(key, out var value))
		{
			return value;
		}
		return default(Color);
	}

	private void SetColor(string key, Color value)
	{
		if (value == default(Color))
		{
			_colors.Remove(key);
		}
		else
		{
			_colors[key] = value;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == AccentProperty)
		{
			_hasAccentColor = _accentColor != default(Color);
			if (_hasAccentColor)
			{
				(_accentColorDark1, _accentColorDark2, _accentColorDark3, _accentColorLight1, _accentColorLight2, _accentColorLight3) = SystemAccentColors.CalculateAccentShades(_accentColor);
			}
		}
	}
}
