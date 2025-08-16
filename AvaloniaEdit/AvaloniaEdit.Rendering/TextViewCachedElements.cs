using System.Collections.Generic;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

internal sealed class TextViewCachedElements
{
	private Dictionary<string, TextLine> _nonPrintableCharacterTexts;

	public TextLine GetTextForNonPrintableCharacter(string text, TextRunProperties properties)
	{
		if (_nonPrintableCharacterTexts == null)
		{
			_nonPrintableCharacterTexts = new Dictionary<string, TextLine>();
		}
		if (!_nonPrintableCharacterTexts.TryGetValue(text, out var value))
		{
			value = FormattedTextElement.PrepareText(TextFormatter.Current, text, properties);
			_nonPrintableCharacterTexts[text] = value;
		}
		return value;
	}
}
