using System.Collections.Generic;
using System.Linq;

namespace AvaloniaEdit.Document;

internal sealed class LineManager
{
	private readonly TextDocument _document;

	private readonly DocumentLineTree _documentLineTree;

	private ILineTracker[] _lineTrackers;

	internal void UpdateListOfLineTrackers()
	{
		_lineTrackers = _document.LineTrackers.ToArray();
	}

	public LineManager(DocumentLineTree documentLineTree, TextDocument document)
	{
		_document = document;
		_documentLineTree = documentLineTree;
		UpdateListOfLineTrackers();
		Rebuild();
	}

	public void Rebuild()
	{
		DocumentLine documentLine = _documentLineTree.GetByNumber(1);
		for (DocumentLine nextLine = documentLine.NextLine; nextLine != null; nextLine = nextLine.NextLine)
		{
			nextLine.IsDeleted = true;
			DocumentLine documentLine2 = nextLine;
			DocumentLine documentLine3 = nextLine;
			DocumentLine documentLine5 = (nextLine.Right = null);
			DocumentLine parent = (documentLine3.Left = documentLine5);
			documentLine2.Parent = parent;
		}
		documentLine.ResetLine();
		SimpleSegment simpleSegment = NewLineFinder.NextNewLine(_document, 0);
		List<DocumentLine> list = new List<DocumentLine>();
		int num = 0;
		while (simpleSegment != SimpleSegment.Invalid)
		{
			documentLine.TotalLength = simpleSegment.Offset + simpleSegment.Length - num;
			documentLine.DelimiterLength = simpleSegment.Length;
			num = simpleSegment.Offset + simpleSegment.Length;
			list.Add(documentLine);
			documentLine = new DocumentLine(_document);
			simpleSegment = NewLineFinder.NextNewLine(_document, num);
		}
		documentLine.TotalLength = _document.TextLength - num;
		list.Add(documentLine);
		_documentLineTree.RebuildTree(list);
		ILineTracker[] lineTrackers = _lineTrackers;
		for (int i = 0; i < lineTrackers.Length; i++)
		{
			lineTrackers[i].RebuildDocument();
		}
	}

	public void Remove(int offset, int length)
	{
		if (length == 0)
		{
			return;
		}
		DocumentLine byOffset = _documentLineTree.GetByOffset(offset);
		int offset2 = byOffset.Offset;
		if (offset > offset2 + byOffset.Length)
		{
			SetLineLength(byOffset, byOffset.TotalLength - 1);
			Remove(offset, length - 1);
			return;
		}
		if (offset + length < offset2 + byOffset.TotalLength)
		{
			SetLineLength(byOffset, byOffset.TotalLength - length);
			return;
		}
		int num = offset2 + byOffset.TotalLength - offset;
		DocumentLine byOffset2 = _documentLineTree.GetByOffset(offset + length);
		if (byOffset2 == byOffset)
		{
			SetLineLength(byOffset, byOffset.TotalLength - length);
			return;
		}
		int num2 = byOffset2.Offset + byOffset2.TotalLength - (offset + length);
		DocumentLine nextLine = byOffset.NextLine;
		DocumentLine documentLine;
		do
		{
			documentLine = nextLine;
			nextLine = nextLine.NextLine;
			RemoveLine(documentLine);
		}
		while (documentLine != byOffset2);
		SetLineLength(byOffset, byOffset.TotalLength - num + num2);
	}

	private void RemoveLine(DocumentLine lineToRemove)
	{
		ILineTracker[] lineTrackers = _lineTrackers;
		for (int i = 0; i < lineTrackers.Length; i++)
		{
			lineTrackers[i].BeforeRemoveLine(lineToRemove);
		}
		_documentLineTree.RemoveLine(lineToRemove);
	}

	public void Insert(int offset, ITextSource text)
	{
		DocumentLine documentLine = _documentLineTree.GetByOffset(offset);
		int offset2 = documentLine.Offset;
		if (offset > offset2 + documentLine.Length)
		{
			SetLineLength(documentLine, documentLine.TotalLength - 1);
			documentLine = InsertLineAfter(documentLine, 1);
			documentLine = SetLineLength(documentLine, 1);
		}
		SimpleSegment simpleSegment = NewLineFinder.NextNewLine(text, 0);
		if (simpleSegment == SimpleSegment.Invalid)
		{
			SetLineLength(documentLine, documentLine.TotalLength + text.TextLength);
			return;
		}
		int num = 0;
		while (simpleSegment != SimpleSegment.Invalid)
		{
			int num2 = offset + simpleSegment.Offset + simpleSegment.Length;
			offset2 = documentLine.Offset;
			int num3 = offset2 + documentLine.TotalLength - (offset + num);
			documentLine = SetLineLength(documentLine, num2 - offset2);
			DocumentLine line = InsertLineAfter(documentLine, num3);
			line = SetLineLength(line, num3);
			documentLine = line;
			num = simpleSegment.Offset + simpleSegment.Length;
			simpleSegment = NewLineFinder.NextNewLine(text, num);
		}
		if (num != text.TextLength)
		{
			SetLineLength(documentLine, documentLine.TotalLength + text.TextLength - num);
		}
	}

	private DocumentLine InsertLineAfter(DocumentLine line, int length)
	{
		DocumentLine documentLine = _documentLineTree.InsertLineAfter(line, length);
		ILineTracker[] lineTrackers = _lineTrackers;
		for (int i = 0; i < lineTrackers.Length; i++)
		{
			lineTrackers[i].LineInserted(line, documentLine);
		}
		return documentLine;
	}

	private DocumentLine SetLineLength(DocumentLine line, int newTotalLength)
	{
		if (newTotalLength - line.TotalLength != 0)
		{
			ILineTracker[] lineTrackers = _lineTrackers;
			for (int i = 0; i < lineTrackers.Length; i++)
			{
				lineTrackers[i].SetLineLength(line, newTotalLength);
			}
			line.TotalLength = newTotalLength;
			DocumentLineTree.UpdateAfterChildrenChange(line);
		}
		if (newTotalLength == 0)
		{
			line.DelimiterLength = 0;
		}
		else
		{
			int offset = line.Offset;
			switch (_document.GetCharAt(offset + newTotalLength - 1))
			{
			case '\r':
				line.DelimiterLength = 1;
				break;
			case '\n':
				if (newTotalLength >= 2 && _document.GetCharAt(offset + newTotalLength - 2) == '\r')
				{
					line.DelimiterLength = 2;
					break;
				}
				if (newTotalLength == 1 && offset > 0 && _document.GetCharAt(offset - 1) == '\r')
				{
					DocumentLine previousLine = line.PreviousLine;
					RemoveLine(line);
					return SetLineLength(previousLine, previousLine.TotalLength + 1);
				}
				line.DelimiterLength = 1;
				break;
			default:
				line.DelimiterLength = 0;
				break;
			}
		}
		return line;
	}

	public void ChangeComplete(DocumentChangeEventArgs e)
	{
		ILineTracker[] lineTrackers = _lineTrackers;
		for (int i = 0; i < lineTrackers.Length; i++)
		{
			lineTrackers[i].ChangeComplete(e);
		}
	}
}
