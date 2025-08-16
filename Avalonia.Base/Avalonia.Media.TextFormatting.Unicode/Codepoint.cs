using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Media.TextFormatting.Unicode;

public readonly record struct Codepoint(uint value)
{
	public static Codepoint ReplacementCodepoint
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Codepoint(65533u);
		}
	}

	public uint Value => value;

	public GeneralCategory GeneralCategory => UnicodeData.GetGeneralCategory(value);

	public Script Script => UnicodeData.GetScript(value);

	public BidiClass BiDiClass => UnicodeData.GetBiDiClass(value);

	public BidiPairedBracketType PairedBracketType => UnicodeData.GetBiDiPairedBracketType(value);

	public LineBreakClass LineBreakClass => UnicodeData.GetLineBreakClass(value);

	public GraphemeBreakClass GraphemeBreakClass => UnicodeData.GetGraphemeClusterBreak(value);

	public bool IsBreakChar
	{
		get
		{
			uint value = value;
			if (value - 10 <= 3 || value == 133 || value - 8232 <= 1)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsWhiteSpace
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ((1L << (int)GeneralCategory) & 0x2000014006L) != 0;
		}
	}

	private readonly uint _value = value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Codepoint GetCanonicalType(Codepoint codePoint)
	{
		if (codePoint.value == 12296)
		{
			return new Codepoint(9001u);
		}
		if (codePoint.value == 12297)
		{
			return new Codepoint(9002u);
		}
		return codePoint;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetPairedBracket(out Codepoint codepoint)
	{
		if (PairedBracketType == BidiPairedBracketType.None)
		{
			codepoint = default(Codepoint);
			return false;
		}
		codepoint = UnicodeData.GetBiDiPairedBracket(value);
		return true;
	}

	public static implicit operator int(Codepoint codepoint)
	{
		return (int)codepoint.value;
	}

	public static implicit operator uint(Codepoint codepoint)
	{
		return codepoint.value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Codepoint ReadAt(ReadOnlySpan<char> text, int index, out int count)
	{
		count = 1;
		if ((uint)index >= (uint)text.Length)
		{
			return ReplacementCodepoint;
		}
		uint num = text[index];
		if (IsInRangeInclusive(num, 55296u, 57343u))
		{
			if (num <= 56319)
			{
				if ((uint)(index + 1) < (uint)text.Length)
				{
					uint num2 = num;
					uint num3 = text[index + 1];
					if (IsInRangeInclusive(num3, 56320u, 57343u))
					{
						count = 2;
						return new Codepoint((num2 << 10) + num3 - 56613888);
					}
				}
			}
			else if (index > 0)
			{
				uint num3 = num;
				uint num2 = text[index - 1];
				if (IsInRangeInclusive(num2, 55296u, 56319u))
				{
					count = 2;
					return new Codepoint((num2 << 10) + num3 - 56613888);
				}
			}
			return ReplacementCodepoint;
		}
		return new Codepoint(num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsInRangeInclusive(uint value, uint lowerBound, uint upperBound)
	{
		return value - lowerBound <= upperBound - lowerBound;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInRangeInclusive(Codepoint cp, uint lowerBound, uint upperBound)
	{
		return IsInRangeInclusive(cp.value, lowerBound, upperBound);
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("Value = ");
		builder.Append(Value.ToString());
		builder.Append(", GeneralCategory = ");
		builder.Append(GeneralCategory.ToString());
		builder.Append(", Script = ");
		builder.Append(Script.ToString());
		builder.Append(", BiDiClass = ");
		builder.Append(BiDiClass.ToString());
		builder.Append(", PairedBracketType = ");
		builder.Append(PairedBracketType.ToString());
		builder.Append(", LineBreakClass = ");
		builder.Append(LineBreakClass.ToString());
		builder.Append(", GraphemeBreakClass = ");
		builder.Append(GraphemeBreakClass.ToString());
		builder.Append(", IsBreakChar = ");
		builder.Append(IsBreakChar.ToString());
		builder.Append(", IsWhiteSpace = ");
		builder.Append(IsWhiteSpace.ToString());
		return true;
	}
}
