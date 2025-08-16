using System;
using Avalonia.Media.TextFormatting.Unicode;

namespace Avalonia.Controls.Utils;

internal static class StringUtils
{
	private enum CharClass
	{
		CharClassUnknown,
		CharClassWhitespace,
		CharClassAlphaNumeric
	}

	public static bool IsEol(char c)
	{
		if (c != '\r')
		{
			return c == '\n';
		}
		return true;
	}

	public static bool IsStartOfWord(string text, int index)
	{
		if (index >= text.Length)
		{
			return false;
		}
		Codepoint codepoint = new Codepoint(text[index]);
		if (index > 0)
		{
			Codepoint codepoint2 = new Codepoint(text[index - 1]);
			if (!codepoint2.IsWhiteSpace)
			{
				return false;
			}
			if (codepoint2.IsBreakChar)
			{
				return true;
			}
		}
		switch (codepoint.GeneralCategory)
		{
		case GeneralCategory.LowercaseLetter:
		case GeneralCategory.TitlecaseLetter:
		case GeneralCategory.UppercaseLetter:
		case GeneralCategory.DecimalNumber:
		case GeneralCategory.LetterNumber:
		case GeneralCategory.OtherNumber:
		case GeneralCategory.DashPunctuation:
		case GeneralCategory.InitialPunctuation:
		case GeneralCategory.OpenPunctuation:
		case GeneralCategory.CurrencySymbol:
		case GeneralCategory.MathSymbol:
			return true;
		default:
			return false;
		}
	}

	public static bool IsEndOfWord(string text, int index)
	{
		if (index >= text.Length)
		{
			return true;
		}
		Codepoint codepoint = new Codepoint(text[index]);
		if (!codepoint.IsWhiteSpace)
		{
			return false;
		}
		if (index > 0)
		{
			if (index + 1 >= text.Length)
			{
				return true;
			}
			if (new Codepoint(text[index + 1]).IsBreakChar)
			{
				return true;
			}
		}
		switch (codepoint.GeneralCategory)
		{
		case GeneralCategory.LowercaseLetter:
		case GeneralCategory.TitlecaseLetter:
		case GeneralCategory.UppercaseLetter:
		case GeneralCategory.DecimalNumber:
		case GeneralCategory.LetterNumber:
		case GeneralCategory.OtherNumber:
		case GeneralCategory.DashPunctuation:
		case GeneralCategory.InitialPunctuation:
		case GeneralCategory.OpenPunctuation:
		case GeneralCategory.CurrencySymbol:
		case GeneralCategory.MathSymbol:
			return false;
		default:
			return true;
		}
	}

	public static int PreviousWord(string text, int cursor)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		cursor = Math.Min(cursor, text.Length);
		int num = LineBegin(text, cursor) - 1;
		int num2 = ((num <= 0 || text[num] != '\n' || text[num - 1] != '\r') ? num : (num - 1));
		if (cursor - 1 == num)
		{
			if (num2 <= 0)
			{
				return 0;
			}
			return num2;
		}
		CharClass charClass = GetCharClass(text[cursor - 1]);
		int num3 = num + 1;
		int num4 = cursor;
		while (num4 > num3 && GetCharClass(text[num4 - 1]) == charClass)
		{
			num4--;
		}
		if (charClass == CharClass.CharClassWhitespace && num4 > num3)
		{
			charClass = GetCharClass(text[num4 - 1]);
			while (num4 > num3 && GetCharClass(text[num4 - 1]) == charClass)
			{
				num4--;
			}
		}
		return num4;
	}

	public static int NextWord(string text, int cursor)
	{
		int num = LineEnd(text, cursor);
		if (cursor >= text.Length)
		{
			return cursor;
		}
		int num2 = ((num >= text.Length || text[num] != '\r' || num + 1 >= text.Length || text[num + 1] != '\n') ? num : (num + 1));
		if (cursor == num || cursor == num2)
		{
			if (num2 < text.Length)
			{
				return num2 + 1;
			}
			return cursor;
		}
		int i;
		for (i = cursor; i < num && char.IsWhiteSpace(text[i]); i++)
		{
		}
		if (i >= num)
		{
			return i;
		}
		for (CharClass charClass = GetCharClass(text[i]); i < num && GetCharClass(text[i]) == charClass; i++)
		{
		}
		return i;
	}

	private static CharClass GetCharClass(char c)
	{
		if (char.IsWhiteSpace(c))
		{
			return CharClass.CharClassWhitespace;
		}
		if (char.IsLetterOrDigit(c))
		{
			return CharClass.CharClassAlphaNumeric;
		}
		return CharClass.CharClassUnknown;
	}

	private static int LineBegin(string text, int pos)
	{
		while (pos > 0 && !IsEol(text[pos - 1]))
		{
			pos--;
		}
		return pos;
	}

	private static int LineEnd(string text, int cursor, bool include = false)
	{
		while (cursor < text.Length && !IsEol(text[cursor]))
		{
			cursor++;
		}
		if (include && cursor < text.Length)
		{
			cursor = ((text[cursor] != '\r' || text[cursor + 1] != '\n') ? (cursor + 1) : (cursor + 2));
		}
		return cursor;
	}
}
