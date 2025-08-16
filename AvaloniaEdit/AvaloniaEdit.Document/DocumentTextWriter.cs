using System;
using System.IO;
using System.Text;

namespace AvaloniaEdit.Document;

public class DocumentTextWriter : TextWriter
{
	private readonly IDocument _document;

	public int InsertionOffset { get; set; }

	public override Encoding Encoding => Encoding.UTF8;

	public DocumentTextWriter(IDocument document, int insertionOffset)
	{
		InsertionOffset = insertionOffset;
		_document = document ?? throw new ArgumentNullException("document");
		IDocumentLine documentLine = document.GetLineByOffset(insertionOffset);
		if (documentLine.DelimiterLength == 0)
		{
			documentLine = documentLine.PreviousLine;
		}
		if (documentLine != null)
		{
			NewLine = document.GetText(documentLine.EndOffset, documentLine.DelimiterLength);
		}
	}

	public override void Write(char value)
	{
		_document.Insert(InsertionOffset, value.ToString());
		InsertionOffset++;
	}

	public override void Write(char[] buffer, int index, int count)
	{
		_document.Insert(InsertionOffset, new string(buffer, index, count));
		InsertionOffset += count;
	}

	public override void Write(string value)
	{
		_document.Insert(InsertionOffset, value);
		InsertionOffset += value.Length;
	}
}
