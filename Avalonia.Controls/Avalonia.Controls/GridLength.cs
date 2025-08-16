using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public struct GridLength : IEquatable<GridLength>
{
	private readonly GridUnitType _type;

	private readonly double _value;

	public static GridLength Auto => new GridLength(0.0, GridUnitType.Auto);

	public static GridLength Star => new GridLength(1.0, GridUnitType.Star);

	public GridUnitType GridUnitType => _type;

	public bool IsAbsolute => _type == GridUnitType.Pixel;

	public bool IsAuto => _type == GridUnitType.Auto;

	public bool IsStar => _type == GridUnitType.Star;

	public double Value => _value;

	public GridLength(double value)
		: this(value, GridUnitType.Pixel)
	{
	}

	public GridLength(double value, GridUnitType type)
	{
		if (value < 0.0 || double.IsNaN(value) || double.IsInfinity(value))
		{
			throw new ArgumentException("Invalid value", "value");
		}
		if (type < GridUnitType.Auto || type > GridUnitType.Star)
		{
			throw new ArgumentException("Invalid value", "type");
		}
		_type = type;
		_value = value;
	}

	public static bool operator ==(GridLength a, GridLength b)
	{
		if (!a.IsAuto || !b.IsAuto)
		{
			if (a._value == b._value)
			{
				return a._type == b._type;
			}
			return false;
		}
		return true;
	}

	public static bool operator !=(GridLength gl1, GridLength gl2)
	{
		return !(gl1 == gl2);
	}

	public override bool Equals(object? o)
	{
		if (o == null)
		{
			return false;
		}
		if (!(o is GridLength))
		{
			return false;
		}
		return this == (GridLength)o;
	}

	public bool Equals(GridLength gridLength)
	{
		return this == gridLength;
	}

	public override int GetHashCode()
	{
		return _value.GetHashCode() ^ _type.GetHashCode();
	}

	public override string ToString()
	{
		if (IsAuto)
		{
			return "Auto";
		}
		string text = _value.ToString(CultureInfo.InvariantCulture);
		if (!IsStar)
		{
			return text;
		}
		return text + "*";
	}

	public static GridLength Parse(string s)
	{
		s = s.ToUpperInvariant();
		if (s == "AUTO")
		{
			return Auto;
		}
		if (s.EndsWith("*"))
		{
			string text = s.Substring(0, s.Length - 1).Trim();
			return new GridLength((text.Length > 0) ? double.Parse(text, CultureInfo.InvariantCulture) : 1.0, GridUnitType.Star);
		}
		return new GridLength(double.Parse(s, CultureInfo.InvariantCulture), GridUnitType.Pixel);
	}

	public static IEnumerable<GridLength> ParseLengths(string s)
	{
		using StringTokenizer tokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture);
		string result;
		while (tokenizer.TryReadString(out result, null))
		{
			yield return Parse(result);
		}
	}
}
