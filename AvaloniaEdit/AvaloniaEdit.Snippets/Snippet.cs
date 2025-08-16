using System;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;

namespace AvaloniaEdit.Snippets;

public class Snippet : SnippetContainerElement
{
	public InsertionContext Insert(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
		int num = textArea.Caret.Offset;
		if (surroundingSegment != null)
		{
			num = surroundingSegment.Offset + TextUtilities.GetWhitespaceAfter(textArea.Document, surroundingSegment.Offset).Length;
		}
		InsertionContext insertionContext = new InsertionContext(textArea, num);
		using (insertionContext.Document.RunUpdate())
		{
			if (surroundingSegment != null)
			{
				textArea.Document.Remove(num, surroundingSegment.EndOffset - num);
			}
			Insert(insertionContext);
			insertionContext.RaiseInsertionCompleted(EventArgs.Empty);
			return insertionContext;
		}
	}
}
