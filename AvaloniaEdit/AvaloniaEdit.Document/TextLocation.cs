using System;
using System.ComponentModel;
using System.Globalization;

namespace AvaloniaEdit.Document;

[TypeConverter(typeof(TextLocationConverter))]
public struct TextLocation : IComparable<TextLocation>, IEquatable<TextLocation>
{
	public static readonly TextLocation Empty = new TextLocation(0, 0);

	public int Line { get; }

	public int Column { get; }

	public bool IsEmpty
	{
		get
		{
			if (Column <= 0)
			{
				return Line <= 0;
			}
			return false;
		}
	}

	public TextLocation(int line, int column)
	{
		Line = line;
		Column = column;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "(Line {1}, Col {0})", Column, Line);
	}

	public override int GetHashCode()
	{
		return (191 * Column.GetHashCode()) ^ Line.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is TextLocation))
		{
			return false;
		}
		return (TextLocation)obj == this;
	}

	public bool Equals(TextLocation other)
	{
		return this == other;
	}

	public static bool operator ==(TextLocation left, TextLocation right)
	{
		if (left.Column == right.Column)
		{
			return left.Line == right.Line;
		}
		return false;
	}

	public static bool operator !=(TextLocation left, TextLocation right)
	{
		if (left.Column == right.Column)
		{
			return left.Line != right.Line;
		}
		return true;
	}

	public static bool operator <(TextLocation left, TextLocation right)
	{
		if (left.Line < right.Line)
		{
			return true;
		}
		if (left.Line == right.Line)
		{
			return left.Column < right.Column;
		}
		return false;
	}

	public static bool operator >(TextLocation left, TextLocation right)
	{
		if (left.Line > right.Line)
		{
			return true;
		}
		if (left.Line == right.Line)
		{
			return left.Column > right.Column;
		}
		return false;
	}

	public static bool operator <=(TextLocation left, TextLocation right)
	{
		return !(left > right);
	}

	public static bool operator >=(TextLocation left, TextLocation right)
	{
		return !(left < right);
	}

	public int CompareTo(TextLocation other)
	{
		if (this == other)
		{
			return 0;
		}
		if (this < other)
		{
			return -1;
		}
		return 1;
	}
}
