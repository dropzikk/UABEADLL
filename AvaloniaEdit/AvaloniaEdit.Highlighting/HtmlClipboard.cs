using System;
using System.Globalization;
using System.Text;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Highlighting;

public static class HtmlClipboard
{
	private static string BuildHeader(int startHTML, int endHTML, int startFragment, int endFragment)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Version:0.9");
		stringBuilder.AppendLine("StartHTML:" + startHTML.ToString("d8", CultureInfo.InvariantCulture));
		stringBuilder.AppendLine("EndHTML:" + endHTML.ToString("d8", CultureInfo.InvariantCulture));
		stringBuilder.AppendLine("StartFragment:" + startFragment.ToString("d8", CultureInfo.InvariantCulture));
		stringBuilder.AppendLine("EndFragment:" + endFragment.ToString("d8", CultureInfo.InvariantCulture));
		return stringBuilder.ToString();
	}

	public static string CreateHtmlFragment(IDocument document, IHighlighter highlighter, ISegment segment, HtmlOptions options)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (highlighter != null && highlighter.Document != document)
		{
			throw new ArgumentException("Highlighter does not belong to the specified document.");
		}
		if (segment == null)
		{
			segment = new SimpleSegment(0, document.TextLength);
		}
		StringBuilder stringBuilder = new StringBuilder();
		int endOffset = segment.EndOffset;
		IDocumentLine documentLine = document.GetLineByOffset(segment.Offset);
		while (documentLine != null && documentLine.Offset < endOffset)
		{
			HighlightedLine highlightedLine = ((highlighter != null) ? highlighter.HighlightLine(documentLine.LineNumber) : new HighlightedLine(document, documentLine));
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine("<br>");
			}
			SimpleSegment overlap = SimpleSegment.GetOverlap(segment, documentLine);
			stringBuilder.Append(highlightedLine.ToHtml(overlap.Offset, overlap.EndOffset, options));
			documentLine = documentLine.NextLine;
		}
		return stringBuilder.ToString();
	}
}
