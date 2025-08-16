using System;
using System.Globalization;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

public class VisualLineElementTextRunProperties : TextRunProperties, ICloneable
{
	private IBrush _backgroundBrush;

	private BaselineAlignment _baselineAlignment;

	private CultureInfo _cultureInfo;

	private double _fontRenderingEmSize;

	private IBrush _foregroundBrush;

	private Typeface _typeface;

	private TextDecorationCollection _textDecorations;

	public override IBrush BackgroundBrush => _backgroundBrush;

	public override BaselineAlignment BaselineAlignment => _baselineAlignment;

	public override CultureInfo CultureInfo => _cultureInfo;

	public override double FontRenderingEmSize => _fontRenderingEmSize;

	public override IBrush ForegroundBrush => _foregroundBrush;

	public override Typeface Typeface => _typeface;

	public override TextDecorationCollection TextDecorations => _textDecorations;

	public VisualLineElementTextRunProperties(TextRunProperties textRunProperties)
	{
		if (textRunProperties == null)
		{
			throw new ArgumentNullException("textRunProperties");
		}
		_backgroundBrush = textRunProperties.BackgroundBrush;
		_baselineAlignment = textRunProperties.BaselineAlignment;
		_cultureInfo = textRunProperties.CultureInfo;
		_fontRenderingEmSize = textRunProperties.FontRenderingEmSize;
		_foregroundBrush = textRunProperties.ForegroundBrush;
		_typeface = textRunProperties.Typeface;
		_textDecorations = textRunProperties.TextDecorations;
	}

	public virtual VisualLineElementTextRunProperties Clone()
	{
		return new VisualLineElementTextRunProperties(this);
	}

	object ICloneable.Clone()
	{
		return Clone();
	}

	public void SetBackgroundBrush(IBrush value)
	{
		_backgroundBrush = value?.ToImmutable();
	}

	public void SetBaselineAlignment(BaselineAlignment value)
	{
		_baselineAlignment = value;
	}

	public void SetCultureInfo(CultureInfo value)
	{
		_cultureInfo = value ?? throw new ArgumentNullException("value");
	}

	public void SetFontRenderingEmSize(double value)
	{
		_fontRenderingEmSize = value;
	}

	public void SetForegroundBrush(IBrush value)
	{
		_foregroundBrush = value?.ToImmutable();
	}

	public void SetTypeface(Typeface value)
	{
		_typeface = value;
	}

	public void SetTextDecorations(TextDecorationCollection value)
	{
		if (_textDecorations == null)
		{
			_textDecorations = value;
		}
		else
		{
			_textDecorations = new TextDecorationCollection(_textDecorations.Union(value));
		}
	}
}
