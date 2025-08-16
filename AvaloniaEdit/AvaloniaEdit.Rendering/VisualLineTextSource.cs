using System;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

internal sealed class VisualLineTextSource : Avalonia.Media.TextFormatting.ITextSource, ITextRunConstructionContext
{
	private string _cachedString;

	private int _cachedStringOffset;

	public VisualLine VisualLine { get; private set; }

	public TextView TextView { get; set; }

	public TextDocument Document { get; set; }

	public TextRunProperties GlobalTextRunProperties { get; set; }

	public VisualLineTextSource(VisualLine visualLine)
	{
		VisualLine = visualLine;
	}

	public TextRun GetTextRun(int textSourceCharacterIndex)
	{
		try
		{
			foreach (VisualLineElement element in VisualLine.Elements)
			{
				if (textSourceCharacterIndex >= element.VisualColumn && textSourceCharacterIndex < element.VisualColumn + element.VisualLength)
				{
					int num = textSourceCharacterIndex - element.VisualColumn;
					TextRun textRun = element.CreateTextRun(textSourceCharacterIndex, this);
					if (textRun == null)
					{
						throw new ArgumentNullException(element.GetType().Name + ".CreateTextRun");
					}
					if (textRun.Length == 0)
					{
						throw new ArgumentException("The returned TextRun must not have length 0.", element.GetType().Name + ".Length");
					}
					if (num + textRun.Length > element.VisualLength)
					{
						throw new ArgumentException("The returned TextRun is too long.", element.GetType().Name + ".CreateTextRun");
					}
					if (textRun is InlineObjectRun inlineObjectRun)
					{
						inlineObjectRun.VisualLine = VisualLine;
						VisualLine.HasInlineObjects = true;
						TextView.AddInlineObject(inlineObjectRun);
					}
					return textRun;
				}
			}
			if (TextView.Options.ShowEndOfLine && textSourceCharacterIndex == VisualLine.VisualLength)
			{
				return CreateTextRunForNewLine();
			}
			return new TextEndOfParagraph(1);
		}
		catch (Exception)
		{
			throw;
		}
	}

	private TextRun CreateTextRunForNewLine()
	{
		string text = "";
		DocumentLine lastDocumentLine = VisualLine.LastDocumentLine;
		if (lastDocumentLine.DelimiterLength == 2)
		{
			text = TextView.Options.EndOfLineCRLFGlyph;
		}
		else if (lastDocumentLine.DelimiterLength == 1)
		{
			text = Document.GetCharAt(lastDocumentLine.Offset + lastDocumentLine.Length) switch
			{
				'\r' => TextView.Options.EndOfLineCRGlyph, 
				'\n' => TextView.Options.EndOfLineLFGlyph, 
				_ => "?", 
			};
		}
		VisualLineElementTextRunProperties visualLineElementTextRunProperties = new VisualLineElementTextRunProperties(GlobalTextRunProperties);
		visualLineElementTextRunProperties.SetForegroundBrush(TextView.NonPrintableCharacterBrush);
		visualLineElementTextRunProperties.SetFontRenderingEmSize(GlobalTextRunProperties.FontRenderingEmSize - 2.0);
		return new FormattedTextRun(new FormattedTextElement(TextView.CachedElements.GetTextForNonPrintableCharacter(text, visualLineElementTextRunProperties), 0)
		{
			RelativeTextOffset = lastDocumentLine.Offset + lastDocumentLine.Length
		}, GlobalTextRunProperties);
	}

	public ReadOnlyMemory<char> GetPrecedingText(int textSourceCharacterIndexLimit)
	{
		try
		{
			foreach (VisualLineElement element in VisualLine.Elements)
			{
				if (textSourceCharacterIndexLimit > element.VisualColumn && textSourceCharacterIndexLimit <= element.VisualColumn + element.VisualLength)
				{
					ReadOnlyMemory<char> precedingText = element.GetPrecedingText(textSourceCharacterIndexLimit, this);
					if (!precedingText.IsEmpty)
					{
						int num = textSourceCharacterIndexLimit - element.VisualColumn;
						if (precedingText.Length > num)
						{
							throw new ArgumentException("The returned TextSpan is too long.", element.GetType().Name + ".GetPrecedingText");
						}
						return precedingText;
					}
					break;
				}
			}
			return ReadOnlyMemory<char>.Empty;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public StringSegment GetText(int offset, int length)
	{
		if (_cachedString != null && offset >= _cachedStringOffset && offset + length <= _cachedStringOffset + _cachedString.Length)
		{
			return new StringSegment(_cachedString, offset - _cachedStringOffset, length);
		}
		_cachedStringOffset = offset;
		return new StringSegment(_cachedString = Document.GetText(offset, length));
	}
}
