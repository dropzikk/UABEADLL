using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Media;

public readonly record struct UnicodeRange
{
	internal UnicodeRangeSegment Single => _single;

	internal IReadOnlyList<UnicodeRangeSegment>? Segments => _segments;

	public static readonly UnicodeRange Default = Parse("0-10FFFD");

	private readonly UnicodeRangeSegment _single;

	private readonly IReadOnlyList<UnicodeRangeSegment>? _segments;

	public UnicodeRange(int start, int end)
	{
		_segments = null;
		_single = new UnicodeRangeSegment(start, end);
	}

	public UnicodeRange(UnicodeRangeSegment single)
	{
		_segments = null;
		_single = single;
	}

	public UnicodeRange(IReadOnlyList<UnicodeRangeSegment> segments)
	{
		_segments = null;
		if (segments == null || segments.Count == 0)
		{
			throw new ArgumentException("segments");
		}
		_single = segments[0];
		_segments = segments;
	}

	public bool IsInRange(int value)
	{
		if (_segments == null)
		{
			return _single.IsInRange(value);
		}
		foreach (UnicodeRangeSegment segment in _segments)
		{
			if (segment.IsInRange(value))
			{
				return true;
			}
		}
		return false;
	}

	public static UnicodeRange Parse(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			throw new FormatException("Could not parse specified Unicode range.");
		}
		string[] array = s.Split(',');
		int num = array.Length;
		switch (num)
		{
		case 0:
			throw new FormatException("Could not parse specified Unicode range.");
		case 1:
			return new UnicodeRange(UnicodeRangeSegment.Parse(array[0]));
		default:
		{
			UnicodeRangeSegment[] array2 = new UnicodeRangeSegment[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = UnicodeRangeSegment.Parse(array[i].Trim());
			}
			return new UnicodeRange(array2);
		}
		}
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		return false;
	}
}
