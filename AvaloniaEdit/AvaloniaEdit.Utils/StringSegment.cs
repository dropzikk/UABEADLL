using System;

namespace AvaloniaEdit.Utils;

public struct StringSegment : IEquatable<StringSegment>
{
	public string Text { get; }

	public int Offset { get; }

	public int Count { get; }

	public StringSegment(string text, int offset, int count)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (offset < 0 || offset > text.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (offset + count > text.Length)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		Text = text;
		Offset = offset;
		Count = count;
	}

	public StringSegment(string text)
	{
		Text = text ?? throw new ArgumentNullException("text");
		Offset = 0;
		Count = text.Length;
	}

	public override bool Equals(object obj)
	{
		if (obj is StringSegment)
		{
			return Equals((StringSegment)obj);
		}
		return false;
	}

	public bool Equals(StringSegment other)
	{
		if ((object)Text == other.Text && Offset == other.Offset)
		{
			return Count == other.Count;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Text.GetHashCode() ^ Offset ^ Count;
	}

	public static bool operator ==(StringSegment left, StringSegment right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(StringSegment left, StringSegment right)
	{
		return !left.Equals(right);
	}
}
