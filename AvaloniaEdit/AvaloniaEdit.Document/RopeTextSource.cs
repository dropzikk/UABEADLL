using System;
using System.IO;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public sealed class RopeTextSource : ITextSource
{
	private readonly Rope<char> _rope;

	public string Text => _rope.ToString();

	public int TextLength => _rope.Length;

	public ITextSourceVersion Version { get; }

	public RopeTextSource(Rope<char> rope)
	{
		_rope = rope?.Clone() ?? throw new ArgumentNullException("rope");
	}

	public RopeTextSource(Rope<char> rope, ITextSourceVersion version)
	{
		_rope = rope?.Clone() ?? throw new ArgumentNullException("rope");
		Version = version;
	}

	public Rope<char> GetRope()
	{
		return _rope.Clone();
	}

	public char GetCharAt(int offset)
	{
		return _rope[offset];
	}

	public string GetText(int offset, int length)
	{
		return _rope.ToString(offset, length);
	}

	public string GetText(ISegment segment)
	{
		return _rope.ToString(segment.Offset, segment.Length);
	}

	public TextReader CreateReader()
	{
		return new RopeTextReader(_rope);
	}

	public TextReader CreateReader(int offset, int length)
	{
		return new RopeTextReader(_rope.GetRange(offset, length));
	}

	public ITextSource CreateSnapshot()
	{
		return this;
	}

	public ITextSource CreateSnapshot(int offset, int length)
	{
		return new RopeTextSource(_rope.GetRange(offset, length));
	}

	public int IndexOf(char c, int startIndex, int count)
	{
		return _rope.IndexOf(c, startIndex, count);
	}

	public int IndexOfAny(char[] anyOf, int startIndex, int count)
	{
		return _rope.IndexOfAny(anyOf, startIndex, count);
	}

	public int LastIndexOf(char c, int startIndex, int count)
	{
		return _rope.LastIndexOf(c, startIndex, count);
	}

	public int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return _rope.IndexOf(searchText, startIndex, count, comparisonType);
	}

	public int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return _rope.LastIndexOf(searchText, startIndex, count, comparisonType);
	}

	public void WriteTextTo(TextWriter writer)
	{
		_rope.WriteTo(writer, 0, _rope.Length);
	}

	public void WriteTextTo(TextWriter writer, int offset, int length)
	{
		_rope.WriteTo(writer, offset, length);
	}
}
