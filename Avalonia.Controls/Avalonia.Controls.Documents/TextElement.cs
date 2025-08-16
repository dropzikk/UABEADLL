using Avalonia.Media;

namespace Avalonia.Controls.Documents;

public abstract class TextElement : StyledElement
{
	public static readonly StyledProperty<IBrush?> BackgroundProperty = Border.BackgroundProperty.AddOwner<TextElement>();

	public static readonly AttachedProperty<FontFamily> FontFamilyProperty = AvaloniaProperty.RegisterAttached<TextElement, TextElement, FontFamily>("FontFamily", Avalonia.Media.FontFamily.Default, inherits: true);

	public static readonly AttachedProperty<double> FontSizeProperty = AvaloniaProperty.RegisterAttached<TextElement, TextElement, double>("FontSize", 12.0, inherits: true);

	public static readonly AttachedProperty<FontStyle> FontStyleProperty = AvaloniaProperty.RegisterAttached<TextElement, TextElement, FontStyle>("FontStyle", FontStyle.Normal, inherits: true);

	public static readonly AttachedProperty<FontWeight> FontWeightProperty = AvaloniaProperty.RegisterAttached<TextElement, TextElement, FontWeight>("FontWeight", FontWeight.Normal, inherits: true);

	public static readonly AttachedProperty<FontStretch> FontStretchProperty = AvaloniaProperty.RegisterAttached<TextElement, TextElement, FontStretch>("FontStretch", FontStretch.Normal, inherits: true);

	public static readonly AttachedProperty<IBrush?> ForegroundProperty = AvaloniaProperty.RegisterAttached<TextElement, TextElement, IBrush>("Foreground", Brushes.Black, inherits: true);

	private IInlineHost? _inlineHost;

	public IBrush? Background
	{
		get
		{
			return GetValue(BackgroundProperty);
		}
		set
		{
			SetValue(BackgroundProperty, value);
		}
	}

	public FontFamily FontFamily
	{
		get
		{
			return GetValue(FontFamilyProperty);
		}
		set
		{
			SetValue(FontFamilyProperty, value);
		}
	}

	public double FontSize
	{
		get
		{
			return GetValue(FontSizeProperty);
		}
		set
		{
			SetValue(FontSizeProperty, value);
		}
	}

	public FontStyle FontStyle
	{
		get
		{
			return GetValue(FontStyleProperty);
		}
		set
		{
			SetValue(FontStyleProperty, value);
		}
	}

	public FontWeight FontWeight
	{
		get
		{
			return GetValue(FontWeightProperty);
		}
		set
		{
			SetValue(FontWeightProperty, value);
		}
	}

	public FontStretch FontStretch
	{
		get
		{
			return GetValue(FontStretchProperty);
		}
		set
		{
			SetValue(FontStretchProperty, value);
		}
	}

	public IBrush? Foreground
	{
		get
		{
			return GetValue(ForegroundProperty);
		}
		set
		{
			SetValue(ForegroundProperty, value);
		}
	}

	internal IInlineHost? InlineHost
	{
		get
		{
			return _inlineHost;
		}
		set
		{
			IInlineHost inlineHost = _inlineHost;
			_inlineHost = value;
			OnInlineHostChanged(inlineHost, value);
		}
	}

	public static FontFamily GetFontFamily(Control control)
	{
		return control.GetValue(FontFamilyProperty);
	}

	public static void SetFontFamily(Control control, FontFamily value)
	{
		control.SetValue(FontFamilyProperty, value);
	}

	public static double GetFontSize(Control control)
	{
		return control.GetValue(FontSizeProperty);
	}

	public static void SetFontSize(Control control, double value)
	{
		control.SetValue(FontSizeProperty, value);
	}

	public static FontStyle GetFontStyle(Control control)
	{
		return control.GetValue(FontStyleProperty);
	}

	public static void SetFontStyle(Control control, FontStyle value)
	{
		control.SetValue(FontStyleProperty, value);
	}

	public static FontWeight GetFontWeight(Control control)
	{
		return control.GetValue(FontWeightProperty);
	}

	public static void SetFontWeight(Control control, FontWeight value)
	{
		control.SetValue(FontWeightProperty, value);
	}

	public static FontStretch GetFontStretch(Control control)
	{
		return control.GetValue(FontStretchProperty);
	}

	public static void SetFontStretch(Control control, FontStretch value)
	{
		control.SetValue(FontStretchProperty, value);
	}

	public static IBrush? GetForeground(Control control)
	{
		return control.GetValue(ForegroundProperty);
	}

	public static void SetForeground(Control control, IBrush? value)
	{
		control.SetValue(ForegroundProperty, value);
	}

	internal virtual void OnInlineHostChanged(IInlineHost? oldValue, IInlineHost? newValue)
	{
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		switch (change.Property.Name)
		{
		case "Background":
		case "FontFamily":
		case "FontSize":
		case "FontStyle":
		case "FontWeight":
		case "FontStretch":
		case "Foreground":
			InlineHost?.Invalidate();
			break;
		}
	}
}
