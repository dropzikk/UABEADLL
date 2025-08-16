using System;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

public class FormattedTextElement : VisualLineElement
{
	internal readonly FormattedText FormattedText;

	internal string Text;

	internal TextLine TextLine;

	public FormattedTextElement(string text, int documentLength)
		: base(1, documentLength)
	{
		Text = text ?? throw new ArgumentNullException("text");
	}

	public FormattedTextElement(TextLine text, int documentLength)
		: base(1, documentLength)
	{
		TextLine = text ?? throw new ArgumentNullException("text");
	}

	public FormattedTextElement(FormattedText text, int documentLength)
		: base(1, documentLength)
	{
		FormattedText = text ?? throw new ArgumentNullException("text");
	}

	public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
	{
		if (TextLine == null)
		{
			TextFormatter formatter = TextFormatterFactory.Create(context.TextView);
			TextLine = PrepareText(formatter, Text, base.TextRunProperties);
			Text = null;
		}
		return new FormattedTextRun(this, base.TextRunProperties);
	}

	public static TextLine PrepareText(TextFormatter formatter, string text, TextRunProperties properties)
	{
		if (formatter == null)
		{
			throw new ArgumentNullException("formatter");
		}
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (properties == null)
		{
			throw new ArgumentNullException("properties");
		}
		return formatter.FormatLine(new SimpleTextSource(text, properties), 0, 32000.0, new VisualLineTextParagraphProperties
		{
			defaultTextRunProperties = properties,
			textWrapping = TextWrapping.NoWrap,
			tabSize = 40.0
		});
	}
}
