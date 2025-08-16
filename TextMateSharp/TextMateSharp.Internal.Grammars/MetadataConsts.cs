namespace TextMateSharp.Internal.Grammars;

public class MetadataConsts
{
	public static uint LANGUAGEID_MASK = 255u;

	public static uint TOKEN_TYPE_MASK = 768u;

	public static uint BALANCED_BRACKETS_MASK = 1024u;

	public static uint FONT_STYLE_MASK = 30720u;

	public static uint FOREGROUND_MASK = 16744448u;

	public static uint BACKGROUND_MASK = 4278190080u;

	public const int LANGUAGEID_OFFSET = 0;

	public const int TOKEN_TYPE_OFFSET = 8;

	public const int BALANCED_BRACKETS_OFFSET = 10;

	public const int FONT_STYLE_OFFSET = 11;

	public const int FOREGROUND_OFFSET = 15;

	public const int BACKGROUND_OFFSET = 24;
}
