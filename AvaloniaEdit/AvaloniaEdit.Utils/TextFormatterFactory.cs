using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Utils;

internal static class TextFormatterFactory
{
	public static TextFormatter Create(Control owner)
	{
		return TextFormatter.Current;
	}

	public static FormattedText CreateFormattedText(Control element, string text, Typeface typeface, double? emSize, IBrush foreground)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (typeface == default(Typeface))
		{
			typeface = element.CreateTypeface();
		}
		if (!emSize.HasValue)
		{
			emSize = TextElement.GetFontSize(element);
		}
		if (foreground == null)
		{
			foreground = TextElement.GetForeground(element);
		}
		return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, emSize.Value, foreground);
	}
}
