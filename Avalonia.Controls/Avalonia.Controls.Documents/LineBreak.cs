using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace Avalonia.Controls.Documents;

[TrimSurroundingWhitespace]
public class LineBreak : Inline
{
	internal override void BuildTextRun(IList<TextRun> textRuns)
	{
		string newLine = Environment.NewLine;
		TextRunProperties textRunProperties = CreateTextRunProperties();
		TextCharacters item = new TextCharacters(newLine, textRunProperties);
		textRuns.Add(item);
	}

	internal override void AppendText(StringBuilder stringBuilder)
	{
		stringBuilder.Append(Environment.NewLine);
	}
}
