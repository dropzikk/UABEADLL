using System.Runtime.CompilerServices;

namespace Avalonia.Media.TextFormatting.Unicode;

internal static class UnicodeData
{
	internal const int CATEGORY_BITS = 6;

	internal const int SCRIPT_BITS = 8;

	internal const int LINEBREAK_BITS = 6;

	internal const int BIDIPAIREDBRACKED_BITS = 16;

	internal const int BIDIPAIREDBRACKEDTYPE_BITS = 2;

	internal const int BIDICLASS_BITS = 5;

	internal const int SCRIPT_SHIFT = 6;

	internal const int LINEBREAK_SHIFT = 14;

	internal const int BIDIPAIREDBRACKEDTYPE_SHIFT = 16;

	internal const int BIDICLASS_SHIFT = 18;

	internal const int CATEGORY_MASK = 63;

	internal const int SCRIPT_MASK = 255;

	internal const int LINEBREAK_MASK = 63;

	internal const int BIDIPAIREDBRACKED_MASK = 65535;

	internal const int BIDIPAIREDBRACKEDTYPE_MASK = 3;

	internal const int BIDICLASS_MASK = 31;

	private static readonly UnicodeTrie s_unicodeDataTrie;

	private static readonly UnicodeTrie s_graphemeBreakTrie;

	private static readonly UnicodeTrie s_biDiTrie;

	static UnicodeData()
	{
		s_unicodeDataTrie = new UnicodeTrie(UnicodeDataTrie.Data);
		s_graphemeBreakTrie = new UnicodeTrie(GraphemeBreakTrie.Data);
		s_biDiTrie = new UnicodeTrie(BidiTrie.Data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GeneralCategory GetGeneralCategory(uint codepoint)
	{
		return (GeneralCategory)(s_unicodeDataTrie.Get(codepoint) & 0x3F);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Script GetScript(uint codepoint)
	{
		return (Script)((s_unicodeDataTrie.Get(codepoint) >> 6) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BidiClass GetBiDiClass(uint codepoint)
	{
		return (BidiClass)((s_biDiTrie.Get(codepoint) >> 18) & 0x1F);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BidiPairedBracketType GetBiDiPairedBracketType(uint codepoint)
	{
		return (BidiPairedBracketType)((s_biDiTrie.Get(codepoint) >> 16) & 3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Codepoint GetBiDiPairedBracket(uint codepoint)
	{
		return new Codepoint(s_biDiTrie.Get(codepoint) & 0xFFFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LineBreakClass GetLineBreakClass(uint codepoint)
	{
		return (LineBreakClass)((s_unicodeDataTrie.Get(codepoint) >> 14) & 0x3F);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GraphemeBreakClass GetGraphemeClusterBreak(uint codepoint)
	{
		return (GraphemeBreakClass)s_graphemeBreakTrie.Get(codepoint);
	}
}
