using System;

namespace Avalonia.Media.TextFormatting;

public sealed class UnshapedTextRun : TextRun
{
	public override int Length => Text.Length;

	public override ReadOnlyMemory<char> Text { get; }

	public override TextRunProperties Properties { get; }

	public sbyte BidiLevel { get; }

	public UnshapedTextRun(ReadOnlyMemory<char> text, TextRunProperties properties, sbyte biDiLevel)
	{
		Text = text;
		Properties = properties;
		BidiLevel = biDiLevel;
	}
}
