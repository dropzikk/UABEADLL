using System;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

internal sealed class SimpleTextSource : ITextSource
{
	private readonly string _text;

	private readonly TextRunProperties _properties;

	public SimpleTextSource(string text, TextRunProperties properties)
	{
		_text = text;
		_properties = properties;
	}

	public TextRun GetTextRun(int textSourceCharacterIndex)
	{
		if (textSourceCharacterIndex < _text.Length)
		{
			return new TextCharacters(_text.AsMemory().Slice(textSourceCharacterIndex, _text.Length - textSourceCharacterIndex), _properties);
		}
		return new TextEndOfParagraph(1);
	}
}
