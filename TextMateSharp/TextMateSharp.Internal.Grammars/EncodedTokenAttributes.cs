using System;
using System.Collections.Generic;

namespace TextMateSharp.Internal.Grammars;

public static class EncodedTokenAttributes
{
	public static string ToBinaryStr(int metadata)
	{
		List<char> builder = new List<char>(Convert.ToString((uint)metadata, 2));
		while (builder.Count < 32)
		{
			builder.Insert(0, '0');
		}
		return new string(builder.ToArray());
	}

	public static int GetLanguageId(int metadata)
	{
		return metadata & (int)MetadataConsts.LANGUAGEID_MASK;
	}

	public static int GetTokenType(int metadata)
	{
		return (int)(((uint)metadata & MetadataConsts.TOKEN_TYPE_MASK) >> 8);
	}

	public static bool ContainsBalancedBrackets(int metadata)
	{
		return ((uint)metadata & MetadataConsts.BALANCED_BRACKETS_MASK) != 0;
	}

	public static int GetFontStyle(int metadata)
	{
		return (int)(((uint)metadata & MetadataConsts.FONT_STYLE_MASK) >> 11);
	}

	public static int GetForeground(int metadata)
	{
		return (int)(((uint)metadata & MetadataConsts.FOREGROUND_MASK) >> 15);
	}

	public static int GetBackground(int metadata)
	{
		return (int)((metadata & MetadataConsts.BACKGROUND_MASK) >>> 24);
	}

	public static int Set(int metadata, int languageId, int tokenType, bool? containsBalancedBrackets, int fontStyle, int foreground, int background)
	{
		languageId = ((languageId == 0) ? GetLanguageId(metadata) : languageId);
		tokenType = ((tokenType == OptionalStandardTokenType.NotSet) ? GetTokenType(metadata) : tokenType);
		int containsBalancedBracketsBit = (((!containsBalancedBrackets.HasValue) ? ContainsBalancedBrackets(metadata) : containsBalancedBrackets.Value) ? 1 : 0);
		fontStyle = ((fontStyle == -1) ? GetFontStyle(metadata) : fontStyle);
		foreground = ((foreground == 0) ? GetForeground(metadata) : foreground);
		background = ((background == 0) ? GetBackground(metadata) : background);
		return languageId | (tokenType << 8) | (containsBalancedBracketsBit << 10) | (fontStyle << 11) | (foreground << 15) | (background << 24);
	}
}
