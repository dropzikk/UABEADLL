using System;
using System.Collections.Generic;
using Avalonia.Media.TextFormatting.Unicode;
using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting;

internal readonly struct FormattedTextSource : ITextSource
{
	private readonly string _text;

	private readonly TextRunProperties _defaultProperties;

	private readonly IReadOnlyList<ValueSpan<TextRunProperties>>? _textModifier;

	public FormattedTextSource(string text, TextRunProperties defaultProperties, IReadOnlyList<ValueSpan<TextRunProperties>>? textModifier)
	{
		_text = text;
		_defaultProperties = defaultProperties;
		_textModifier = textModifier;
	}

	public TextRun? GetTextRun(int textSourceIndex)
	{
		if (textSourceIndex > _text.Length)
		{
			return null;
		}
		ReadOnlySpan<char> text = _text.AsSpan(textSourceIndex);
		if (text.IsEmpty)
		{
			return null;
		}
		ValueSpan<TextRunProperties> valueSpan = CreateTextStyleRun(text, textSourceIndex, _defaultProperties, _textModifier);
		return new TextCharacters(_text.AsMemory(textSourceIndex, valueSpan.Length), valueSpan.Value);
	}

	private static ValueSpan<TextRunProperties> CreateTextStyleRun(ReadOnlySpan<char> text, int firstTextSourceIndex, TextRunProperties defaultProperties, IReadOnlyList<ValueSpan<TextRunProperties>>? textModifier)
	{
		if (textModifier == null || textModifier.Count == 0)
		{
			return new ValueSpan<TextRunProperties>(firstTextSourceIndex, text.Length, defaultProperties);
		}
		TextRunProperties textRunProperties = defaultProperties;
		bool flag = false;
		int i = 0;
		int num = 0;
		for (; i < textModifier.Count; i++)
		{
			ValueSpan<TextRunProperties> valueSpan = textModifier[i];
			TextRange textRange = new TextRange(valueSpan.Start, valueSpan.Length);
			if (textRange.Start + textRange.Length > firstTextSourceIndex)
			{
				if (textRange.Start > firstTextSourceIndex + text.Length)
				{
					num = text.Length;
					break;
				}
				if (textRange.Start > firstTextSourceIndex && valueSpan.Value != textRunProperties)
				{
					num = Math.Min(Math.Abs(textRange.Start - firstTextSourceIndex), text.Length);
					break;
				}
				num = Math.Max(0, textRange.Start + textRange.Length - firstTextSourceIndex);
				if (!flag)
				{
					flag = true;
					textRunProperties = valueSpan.Value;
				}
			}
		}
		if (num < text.Length && i == textModifier.Count && textRunProperties == defaultProperties)
		{
			num = text.Length;
		}
		if (num == 0 && textRunProperties != defaultProperties)
		{
			textRunProperties = defaultProperties;
			num = text.Length;
		}
		num = CoerceLength(text, num);
		return new ValueSpan<TextRunProperties>(firstTextSourceIndex, num, textRunProperties);
	}

	private static int CoerceLength(ReadOnlySpan<char> text, int length)
	{
		int num = 0;
		GraphemeEnumerator graphemeEnumerator = new GraphemeEnumerator(text);
		Grapheme grapheme;
		while (graphemeEnumerator.MoveNext(out grapheme))
		{
			num += grapheme.Length;
			if (num >= length)
			{
				return num;
			}
		}
		return Math.Min(length, text.Length);
	}
}
