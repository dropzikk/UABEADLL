using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.TextMate;

public class DocumentSnapshot
{
	private struct LineRange
	{
		public int Offset;

		public int Length;

		public int TotalLength;
	}

	private LineRange[] _lineRanges;

	private TextDocument _document;

	private ITextSource _textSource;

	private object _lock = new object();

	private int _lineCount;

	public int LineCount
	{
		get
		{
			lock (_lock)
			{
				return _lineCount;
			}
		}
	}

	public DocumentSnapshot(TextDocument document)
	{
		_document = document;
		_lineRanges = new LineRange[document.LineCount];
		Update(null);
	}

	public void RemoveLines(int startLine, int endLine)
	{
		lock (_lock)
		{
			List<LineRange> list = _lineRanges.ToList();
			list.RemoveRange(startLine, endLine - startLine + 1);
			_lineRanges = list.ToArray();
			_lineCount = _lineRanges.Length;
		}
	}

	public string GetLineText(int lineIndex)
	{
		lock (_lock)
		{
			LineRange lineRange = _lineRanges[lineIndex];
			return _textSource.GetText(lineRange.Offset, lineRange.Length);
		}
	}

	public string GetLineTextIncludingTerminator(int lineIndex)
	{
		lock (_lock)
		{
			LineRange lineRange = _lineRanges[lineIndex];
			return _textSource.GetText(lineRange.Offset, lineRange.TotalLength);
		}
	}

	public string GetLineTerminator(int lineIndex)
	{
		lock (_lock)
		{
			LineRange lineRange = _lineRanges[lineIndex];
			return _textSource.GetText(lineRange.Offset + lineRange.Length, lineRange.TotalLength - lineRange.Length);
		}
	}

	public int GetLineLength(int lineIndex)
	{
		lock (_lock)
		{
			return _lineRanges[lineIndex].Length;
		}
	}

	public int GetTotalLineLength(int lineIndex)
	{
		lock (_lock)
		{
			return _lineRanges[lineIndex].TotalLength;
		}
	}

	public string GetText()
	{
		lock (_lock)
		{
			return _textSource.Text;
		}
	}

	public void Update(DocumentChangeEventArgs e)
	{
		lock (_lock)
		{
			_lineCount = _document.Lines.Count;
			if (e != null && e.OffsetChangeMap != null && _lineRanges != null && _lineCount == _lineRanges.Length)
			{
				RecalculateOffsets(e);
			}
			else
			{
				RecomputeAllLineRanges(e);
			}
			_textSource = _document.CreateSnapshot();
		}
	}

	private void RecalculateOffsets(DocumentChangeEventArgs e)
	{
		DocumentLine lineByOffset = _document.GetLineByOffset(e.Offset);
		int num = lineByOffset.LineNumber - 1;
		_lineRanges[num].Offset = lineByOffset.Offset;
		_lineRanges[num].Length = lineByOffset.Length;
		_lineRanges[num].TotalLength = lineByOffset.TotalLength;
		for (int i = num + 1; i < _lineCount; i++)
		{
			_lineRanges[i].Offset = e.OffsetChangeMap.GetNewOffset(_lineRanges[i].Offset);
		}
	}

	private void RecomputeAllLineRanges(DocumentChangeEventArgs e)
	{
		Array.Resize(ref _lineRanges, _lineCount);
		int num = ((e != null) ? (_document.GetLineByOffset(e.Offset).LineNumber - 1) : 0);
		DocumentLine documentLine = _document.GetLineByNumber(num + 1);
		while (documentLine != null)
		{
			_lineRanges[num].Offset = documentLine.Offset;
			_lineRanges[num].Length = documentLine.Length;
			_lineRanges[num].TotalLength = documentLine.TotalLength;
			documentLine = documentLine.NextLine;
			num++;
		}
	}
}
