using System;
using System.Runtime.CompilerServices;

namespace Avalonia.Media.TextFormatting.Unicode;

public ref struct LineBreakEnumerator
{
	private readonly ReadOnlySpan<char> _text;

	private int _position;

	private int _lastPosition;

	private LineBreakClass _currentClass;

	private LineBreakClass _nextClass;

	private bool _first;

	private int _alphaNumericCount;

	private bool _lb8a;

	private bool _lb21a;

	private bool _lb22ex;

	private bool _lb24ex;

	private bool _lb25ex;

	private bool _lb30;

	private int _lb30a;

	private bool _lb31;

	public LineBreakEnumerator(ReadOnlySpan<char> text)
	{
		this = default(LineBreakEnumerator);
		_text = text;
		_position = 0;
		_currentClass = LineBreakClass.Unknown;
		_nextClass = LineBreakClass.Unknown;
		_first = true;
		_lb8a = false;
		_lb21a = false;
		_lb22ex = false;
		_lb24ex = false;
		_lb25ex = false;
		_alphaNumericCount = 0;
		_lb31 = false;
		_lb30 = false;
		_lb30a = 0;
	}

	public bool MoveNext(out LineBreak lineBreak)
	{
		if (_first)
		{
			LineBreakClass lineBreakClass = NextCharClass();
			_first = false;
			_currentClass = MapFirst(lineBreakClass);
			_nextClass = lineBreakClass;
			_lb8a = lineBreakClass == LineBreakClass.ZWJ;
			_lb30a = 0;
		}
		while (_position < _text.Length)
		{
			_lastPosition = _position;
			LineBreakClass nextClass = _nextClass;
			_nextClass = NextCharClass();
			LineBreakClass currentClass = _currentClass;
			if (currentClass == LineBreakClass.MandatoryBreak || (currentClass == LineBreakClass.CarriageReturn && _nextClass != LineBreakClass.LineFeed))
			{
				_currentClass = MapFirst(_nextClass);
				lineBreak = new LineBreak(FindPriorNonWhitespace(_lastPosition), _lastPosition, required: true);
				return true;
			}
			bool num = GetSimpleBreak() ?? GetPairTableBreak(nextClass);
			_lb8a = _nextClass == LineBreakClass.ZWJ;
			if (num)
			{
				lineBreak = new LineBreak(FindPriorNonWhitespace(_lastPosition), _lastPosition);
				return true;
			}
		}
		if (_position >= _text.Length && _lastPosition < _text.Length)
		{
			_lastPosition = _text.Length;
			bool required = false;
			LineBreakClass currentClass = _currentClass;
			if (currentClass == LineBreakClass.MandatoryBreak || (currentClass == LineBreakClass.CarriageReturn && _nextClass != LineBreakClass.LineFeed))
			{
				required = true;
			}
			lineBreak = new LineBreak(FindPriorNonWhitespace(_lastPosition), _lastPosition, required);
			return true;
		}
		lineBreak = default(LineBreak);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static LineBreakClass MapClass(Codepoint cp)
	{
		if (cp.Value == 327685)
		{
			return LineBreakClass.Alphabetic;
		}
		LineBreakClass lineBreakClass = cp.LineBreakClass;
		if (((1L << (int)lineBreakClass) & 0x31600000000L) != 0L)
		{
			switch (lineBreakClass)
			{
			case LineBreakClass.Unknown:
			case LineBreakClass.Ambiguous:
			case LineBreakClass.Surrogate:
				return LineBreakClass.Alphabetic;
			case LineBreakClass.ComplexContext:
			{
				GeneralCategory generalCategory = cp.GeneralCategory;
				if (generalCategory != GeneralCategory.NonspacingMark && generalCategory != GeneralCategory.SpacingMark)
				{
					return LineBreakClass.Alphabetic;
				}
				return LineBreakClass.CombiningMark;
			}
			case LineBreakClass.ConditionalJapaneseStarter:
				return LineBreakClass.Nonstarter;
			}
		}
		return lineBreakClass;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static LineBreakClass MapFirst(LineBreakClass c)
	{
		switch (c)
		{
		case LineBreakClass.LineFeed:
		case LineBreakClass.NextLine:
			return LineBreakClass.MandatoryBreak;
		case LineBreakClass.Space:
			return LineBreakClass.WordJoiner;
		default:
			return c;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsAlphaNumeric(LineBreakClass cls)
	{
		return ((1L << (int)cls) & 0x3800) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsPrefixPostfixNumericOrSpace(LineBreakClass cls)
	{
		return ((1L << (int)cls) & 0x40000000600L) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsPrefixPostfixNumeric(LineBreakClass cls)
	{
		return ((1L << (int)cls) & 0x600) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsClosePunctuationOrParenthesis(LineBreakClass cls)
	{
		return ((1L << (int)cls) & 6) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsClosePunctuationOrInfixNumericOrBreakSymbols(LineBreakClass cls)
	{
		return ((1L << (int)cls) & 0x182) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsSpaceOrWordJoinerOrAlphabetic(LineBreakClass cls)
	{
		return ((1L << (int)cls) & 0x40000401000L) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsMandatoryBreakOrLineFeedOrCarriageReturn(LineBreakClass cls)
	{
		return ((1L << (int)cls) & 0x6800000000L) != 0;
	}

	private LineBreakClass PeekNextCharClass()
	{
		int count;
		return MapClass(Codepoint.ReadAt(_text, _position, out count));
	}

	private LineBreakClass NextCharClass()
	{
		int count;
		Codepoint cp = Codepoint.ReadAt(_text, _position, out count);
		LineBreakClass lineBreakClass = MapClass(cp);
		_position += count;
		if (IsAlphaNumeric(_currentClass) || (_alphaNumericCount > 0 && lineBreakClass == LineBreakClass.CombiningMark))
		{
			_alphaNumericCount++;
		}
		if (lineBreakClass == LineBreakClass.CombiningMark)
		{
			if (((1L << (int)_currentClass) & 0x4E900100040L) != 0L)
			{
				_lb22ex = true;
			}
			if (_first || ((1L << (int)_currentClass) & 0x4E980100040L) != 0L)
			{
				_lb31 = true;
			}
		}
		if (_first)
		{
			if (IsClosePunctuationOrParenthesis(lineBreakClass))
			{
				_lb24ex = true;
			}
			if (IsClosePunctuationOrInfixNumericOrBreakSymbols(lineBreakClass))
			{
				_lb25ex = true;
			}
			if (IsPrefixPostfixNumericOrSpace(lineBreakClass))
			{
				_lb31 = true;
			}
		}
		if (_currentClass == LineBreakClass.Alphabetic && IsPrefixPostfixNumericOrSpace(lineBreakClass))
		{
			_lb31 = true;
		}
		if (_lb31 && !IsPrefixPostfixNumeric(_currentClass) && lineBreakClass == LineBreakClass.OpenPunctuation && cp.Value == 40)
		{
			_lb31 = false;
		}
		if (IsSpaceOrWordJoinerOrAlphabetic(lineBreakClass) && IsClosePunctuationOrInfixNumericOrBreakSymbols(PeekNextCharClass()))
		{
			_lb25ex = true;
		}
		_lb30 = _alphaNumericCount > 0 && lineBreakClass == LineBreakClass.OpenPunctuation && cp.Value != 40 && cp.Value != 91 && cp.Value != 123;
		return lineBreakClass;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool? GetSimpleBreak()
	{
		switch (_nextClass)
		{
		case LineBreakClass.Space:
			return false;
		case LineBreakClass.MandatoryBreak:
		case LineBreakClass.LineFeed:
		case LineBreakClass.NextLine:
			_currentClass = LineBreakClass.MandatoryBreak;
			return false;
		case LineBreakClass.CarriageReturn:
			_currentClass = LineBreakClass.CarriageReturn;
			return false;
		default:
			return null;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool GetPairTableBreak(LineBreakClass lastClass)
	{
		bool flag = false;
		switch (LineBreakPairTable.Table[(int)_currentClass][(int)_nextClass])
		{
		case 0:
			flag = true;
			break;
		case 1:
			if (_lb31 && _nextClass == LineBreakClass.OpenPunctuation)
			{
				flag = true;
				_lb31 = false;
			}
			else if (_lb30)
			{
				flag = true;
				_lb30 = false;
				_alphaNumericCount = 0;
			}
			else if (_lb25ex && (_nextClass == LineBreakClass.PrefixNumeric || _nextClass == LineBreakClass.Numeric))
			{
				flag = true;
				_lb25ex = false;
			}
			else if (_lb24ex && (_nextClass == LineBreakClass.PostfixNumeric || _nextClass == LineBreakClass.PrefixNumeric))
			{
				flag = true;
				_lb24ex = false;
			}
			else
			{
				flag = lastClass == LineBreakClass.Space;
			}
			break;
		case 2:
			flag = lastClass == LineBreakClass.Space;
			if (!flag)
			{
				return false;
			}
			break;
		case 3:
			if (lastClass != LineBreakClass.Space)
			{
				return false;
			}
			break;
		}
		if (_nextClass == LineBreakClass.Inseparable)
		{
			switch (lastClass)
			{
			case LineBreakClass.CombiningMark:
				if (_lb22ex)
				{
					_lb22ex = false;
				}
				else
				{
					flag = false;
				}
				break;
			default:
				flag = false;
				break;
			case LineBreakClass.Exclamation:
			case LineBreakClass.ZWSpace:
			case LineBreakClass.ContingentBreak:
			case LineBreakClass.MandatoryBreak:
			case LineBreakClass.LineFeed:
			case LineBreakClass.NextLine:
			case LineBreakClass.Space:
				break;
			}
		}
		if (_lb8a)
		{
			flag = false;
		}
		if (_lb21a && (_currentClass == LineBreakClass.Hyphen || _currentClass == LineBreakClass.BreakAfter))
		{
			flag = false;
			_lb21a = false;
		}
		else
		{
			_lb21a = _currentClass == LineBreakClass.HebrewLetter;
		}
		if (_currentClass == LineBreakClass.RegionalIndicator)
		{
			_lb30a++;
			if (_lb30a == 2 && _nextClass == LineBreakClass.RegionalIndicator)
			{
				flag = true;
				_lb30a = 0;
			}
		}
		else
		{
			_lb30a = 0;
		}
		if (_nextClass == LineBreakClass.EModifier && _lastPosition > 0 && Codepoint.IsInRangeInclusive(Codepoint.ReadAt(_text, _lastPosition - 1, out var _), 126976u, 127023u))
		{
			flag = false;
		}
		_currentClass = _nextClass;
		return flag;
	}

	private int FindPriorNonWhitespace(int from)
	{
		if (from > 0 && IsMandatoryBreakOrLineFeedOrCarriageReturn(Codepoint.ReadAt(_text, from - 1, out var count).LineBreakClass))
		{
			from -= count;
		}
		int count2;
		while (from > 0 && Codepoint.ReadAt(_text, from - 1, out count2).LineBreakClass == LineBreakClass.Space)
		{
			from -= count2;
		}
		return from;
	}
}
