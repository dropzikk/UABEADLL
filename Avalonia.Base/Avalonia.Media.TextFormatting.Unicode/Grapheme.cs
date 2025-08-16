namespace Avalonia.Media.TextFormatting.Unicode;

public readonly ref struct Grapheme
{
	public Codepoint FirstCodepoint { get; }

	public int Offset { get; }

	public int Length { get; }

	public Grapheme(Codepoint firstCodepoint, int offset, int length)
	{
		FirstCodepoint = firstCodepoint;
		Offset = offset;
		Length = length;
	}
}
