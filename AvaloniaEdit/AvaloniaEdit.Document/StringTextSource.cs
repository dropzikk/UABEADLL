using System;
using System.IO;

namespace AvaloniaEdit.Document;

public class StringTextSource : ITextSource
{
	public static readonly StringTextSource Empty = new StringTextSource(string.Empty);

	public ITextSourceVersion Version { get; }

	public int TextLength => Text.Length;

	public string Text { get; }

	public StringTextSource(string text)
	{
		Text = text ?? throw new ArgumentNullException("text");
	}

	public StringTextSource(string text, ITextSourceVersion version)
	{
		Text = text ?? throw new ArgumentNullException("text");
		Version = version;
	}

	public ITextSource CreateSnapshot()
	{
		return this;
	}

	public ITextSource CreateSnapshot(int offset, int length)
	{
		return new StringTextSource(Text.Substring(offset, length));
	}

	public TextReader CreateReader()
	{
		return new StringReader(Text);
	}

	public TextReader CreateReader(int offset, int length)
	{
		return new StringReader(Text.Substring(offset, length));
	}

	public void WriteTextTo(TextWriter writer)
	{
		writer.Write(Text);
	}

	public void WriteTextTo(TextWriter writer, int offset, int length)
	{
		writer.Write(Text.Substring(offset, length));
	}

	public char GetCharAt(int offset)
	{
		return Text[offset];
	}

	public string GetText(int offset, int length)
	{
		return Text.Substring(offset, length);
	}

	public string GetText(ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return Text.Substring(segment.Offset, segment.Length);
	}

	public int IndexOf(char c, int startIndex, int count)
	{
		return Text.IndexOf(c, startIndex, count);
	}

	public int IndexOfAny(char[] anyOf, int startIndex, int count)
	{
		return Text.IndexOfAny(anyOf, startIndex, count);
	}

	public int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return Text.IndexOf(searchText, startIndex, count, comparisonType);
	}

	public int LastIndexOf(char c, int startIndex, int count)
	{
		return Text.LastIndexOf(c, startIndex + count - 1, count);
	}

	public int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return Text.LastIndexOf(searchText, startIndex + count - 1, count, comparisonType);
	}
}
