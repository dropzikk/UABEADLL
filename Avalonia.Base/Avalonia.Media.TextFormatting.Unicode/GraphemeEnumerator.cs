using System;

namespace Avalonia.Media.TextFormatting.Unicode;

public ref struct GraphemeEnumerator
{
	private readonly ReadOnlySpan<char> _text;

	private int _currentCodeUnitOffset;

	private int _codeUnitLengthOfCurrentCodepoint;

	private Codepoint _currentCodepoint;

	private GraphemeBreakClass _currentType;

	public GraphemeEnumerator(ReadOnlySpan<char> text)
	{
		_text = text;
		_currentCodeUnitOffset = 0;
		_codeUnitLengthOfCurrentCodepoint = 0;
		_currentCodepoint = Codepoint.ReplacementCodepoint;
		_currentType = GraphemeBreakClass.Other;
	}

	public bool MoveNext(out Grapheme grapheme)
	{
		int currentCodeUnitOffset = _currentCodeUnitOffset;
		if ((uint)currentCodeUnitOffset >= (uint)_text.Length)
		{
			grapheme = default(Grapheme);
			return false;
		}
		if (currentCodeUnitOffset == 0)
		{
			ReadNextCodepoint();
		}
		Codepoint currentCodepoint = _currentCodepoint;
		if (_currentType == GraphemeBreakClass.Prepend)
		{
			do
			{
				ReadNextCodepoint();
			}
			while (_currentType == GraphemeBreakClass.Prepend);
			if ((uint)_currentCodeUnitOffset >= (uint)_text.Length)
			{
				goto IL_01d1;
			}
		}
		if (_currentCodeUnitOffset <= currentCodeUnitOffset || ((1 << (int)_currentType) & 0x206) == 0)
		{
			GraphemeBreakClass currentType = _currentType;
			ReadNextCodepoint();
			switch (currentType)
			{
			case GraphemeBreakClass.CR:
				if (_currentType == GraphemeBreakClass.LF)
				{
					ReadNextCodepoint();
				}
				break;
			case GraphemeBreakClass.L:
				while (_currentType == GraphemeBreakClass.L)
				{
					ReadNextCodepoint();
				}
				if (_currentType == GraphemeBreakClass.V)
				{
					ReadNextCodepoint();
				}
				else
				{
					if (_currentType != GraphemeBreakClass.LV)
					{
						if (_currentType == GraphemeBreakClass.LVT)
						{
							ReadNextCodepoint();
							goto case GraphemeBreakClass.LVT;
						}
						goto default;
					}
					ReadNextCodepoint();
				}
				goto case GraphemeBreakClass.LV;
			case GraphemeBreakClass.LV:
			case GraphemeBreakClass.V:
				while (_currentType == GraphemeBreakClass.V)
				{
					ReadNextCodepoint();
				}
				if (_currentType == GraphemeBreakClass.T)
				{
					ReadNextCodepoint();
					goto case GraphemeBreakClass.LVT;
				}
				goto default;
			case GraphemeBreakClass.LVT:
			case GraphemeBreakClass.T:
				while (_currentType == GraphemeBreakClass.T)
				{
					ReadNextCodepoint();
				}
				goto default;
			case GraphemeBreakClass.ExtendedPictographic:
				while (true)
				{
					if (_currentType == GraphemeBreakClass.Extend)
					{
						ReadNextCodepoint();
						continue;
					}
					if (_currentType != GraphemeBreakClass.ZWJ)
					{
						break;
					}
					ReadNextCodepoint();
					if (_currentType != GraphemeBreakClass.ExtendedPictographic)
					{
						break;
					}
					ReadNextCodepoint();
				}
				goto default;
			case GraphemeBreakClass.RegionalIndicator:
				if (_currentType == GraphemeBreakClass.RegionalIndicator)
				{
					ReadNextCodepoint();
				}
				goto default;
			default:
				while (((1 << (int)_currentType) & 0x24040) != 0)
				{
					ReadNextCodepoint();
				}
				break;
			case GraphemeBreakClass.Control:
			case GraphemeBreakClass.LF:
				break;
			}
		}
		goto IL_01d1;
		IL_01d1:
		int length = _currentCodeUnitOffset - currentCodeUnitOffset;
		grapheme = new Grapheme(currentCodepoint, currentCodeUnitOffset, length);
		return true;
	}

	private void ReadNextCodepoint()
	{
		_currentCodeUnitOffset += _codeUnitLengthOfCurrentCodepoint;
		_currentCodepoint = Codepoint.ReadAt(_text, _currentCodeUnitOffset, out _codeUnitLengthOfCurrentCodepoint);
		_currentType = _currentCodepoint.GraphemeBreakClass;
	}
}
