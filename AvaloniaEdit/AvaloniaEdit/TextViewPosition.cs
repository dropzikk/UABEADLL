using System;
using System.Globalization;
using AvaloniaEdit.Document;

namespace AvaloniaEdit;

public struct TextViewPosition : IEquatable<TextViewPosition>, IComparable<TextViewPosition>
{
	public TextLocation Location
	{
		get
		{
			return new TextLocation(Line, Column);
		}
		set
		{
			Line = value.Line;
			Column = value.Column;
		}
	}

	public int Line { get; set; }

	public int Column { get; set; }

	public int VisualColumn { get; set; }

	public bool IsAtEndOfLine { get; set; }

	public TextViewPosition(int line, int column, int visualColumn)
	{
		Line = line;
		Column = column;
		VisualColumn = visualColumn;
		IsAtEndOfLine = false;
	}

	public TextViewPosition(int line, int column)
		: this(line, column, -1)
	{
	}

	public TextViewPosition(TextLocation location, int visualColumn)
	{
		Line = location.Line;
		Column = location.Column;
		VisualColumn = visualColumn;
		IsAtEndOfLine = false;
	}

	public TextViewPosition(TextLocation location)
		: this(location, -1)
	{
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "[TextViewPosition Line={0} Column={1} VisualColumn={2} IsAtEndOfLine={3}]", Line, Column, VisualColumn, IsAtEndOfLine);
	}

	public override bool Equals(object obj)
	{
		if (obj is TextViewPosition)
		{
			return Equals((TextViewPosition)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (IsAtEndOfLine ? 115817 : 0) + 1000000007 * Line.GetHashCode() + 1000000009 * Column.GetHashCode() + 1000000021 * VisualColumn.GetHashCode();
	}

	public bool Equals(TextViewPosition other)
	{
		if (Line == other.Line && Column == other.Column && VisualColumn == other.VisualColumn)
		{
			return IsAtEndOfLine == other.IsAtEndOfLine;
		}
		return false;
	}

	public static bool operator ==(TextViewPosition left, TextViewPosition right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TextViewPosition left, TextViewPosition right)
	{
		return !left.Equals(right);
	}

	public int CompareTo(TextViewPosition other)
	{
		int num = Location.CompareTo(other.Location);
		if (num != 0)
		{
			return num;
		}
		num = VisualColumn.CompareTo(other.VisualColumn);
		if (num != 0)
		{
			return num;
		}
		if (IsAtEndOfLine && !other.IsAtEndOfLine)
		{
			return -1;
		}
		if (!IsAtEndOfLine && other.IsAtEndOfLine)
		{
			return 1;
		}
		return 0;
	}
}
