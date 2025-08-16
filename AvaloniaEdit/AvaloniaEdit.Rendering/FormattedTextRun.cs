using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

public class FormattedTextRun : DrawableTextRun
{
	public FormattedTextElement Element { get; }

	public override ReadOnlyMemory<char> Text { get; }

	public override TextRunProperties Properties { get; }

	public override double Baseline => Element.FormattedText?.Baseline ?? Element.TextLine.Baseline;

	public override Size Size
	{
		get
		{
			FormattedText formattedText = Element.FormattedText;
			if (formattedText != null)
			{
				return new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
			}
			TextLine textLine = Element.TextLine;
			return new Size(textLine.WidthIncludingTrailingWhitespace, textLine.Height);
		}
	}

	public FormattedTextRun(FormattedTextElement element, TextRunProperties properties)
	{
		if (properties == null)
		{
			throw new ArgumentNullException("properties");
		}
		Properties = properties;
		Element = element ?? throw new ArgumentNullException("element");
	}

	public override void Draw(DrawingContext drawingContext, Point origin)
	{
		if (Element.FormattedText != null)
		{
			drawingContext.DrawText(Element.FormattedText, origin);
		}
		else
		{
			Element.TextLine.Draw(drawingContext, origin);
		}
	}
}
