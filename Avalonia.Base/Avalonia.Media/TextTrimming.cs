using System;
using Avalonia.Media.TextFormatting;

namespace Avalonia.Media;

public abstract class TextTrimming
{
	internal const string DefaultEllipsisChar = "…";

	public static TextTrimming None { get; } = new TextNoneTrimming();

	public static TextTrimming CharacterEllipsis { get; } = new TextTrailingTrimming("…", isWordBased: false);

	public static TextTrimming WordEllipsis { get; } = new TextTrailingTrimming("…", isWordBased: true);

	public static TextTrimming PrefixCharacterEllipsis { get; } = new TextLeadingPrefixTrimming("…", 8);

	public static TextTrimming LeadingCharacterEllipsis { get; } = new TextLeadingPrefixTrimming("…", 0);

	public abstract TextCollapsingProperties CreateCollapsingProperties(TextCollapsingCreateInfo createInfo);

	public static TextTrimming Parse(string s)
	{
		if (Matches("None"))
		{
			return None;
		}
		if (Matches("CharacterEllipsis"))
		{
			return CharacterEllipsis;
		}
		if (Matches("WordEllipsis"))
		{
			return WordEllipsis;
		}
		if (Matches("PrefixCharacterEllipsis"))
		{
			return PrefixCharacterEllipsis;
		}
		throw new FormatException("Invalid text trimming string: '" + s + "'.");
		bool Matches(string name)
		{
			return name.Equals(s, StringComparison.OrdinalIgnoreCase);
		}
	}
}
