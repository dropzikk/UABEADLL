using System;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Indentation.CSharp;

public sealed class TextDocumentAccessor : IDocumentAccessor
{
	private readonly TextDocument _doc;

	private readonly int _minLine;

	private readonly int _maxLine;

	private int _num;

	private string _text;

	private DocumentLine _line;

	private bool _lineDirty;

	public bool IsReadOnly => _num < _minLine;

	public int LineNumber => _num;

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			if (_num >= _minLine)
			{
				_text = value;
				_lineDirty = true;
			}
		}
	}

	public TextDocumentAccessor(TextDocument document)
	{
		_doc = document ?? throw new ArgumentNullException("document");
		_minLine = 1;
		_maxLine = _doc.LineCount;
	}

	public TextDocumentAccessor(TextDocument document, int minLine, int maxLine)
	{
		_doc = document ?? throw new ArgumentNullException("document");
		_minLine = minLine;
		_maxLine = maxLine;
	}

	public bool MoveNext()
	{
		if (_lineDirty)
		{
			_doc.Replace(_line, _text);
			_lineDirty = false;
		}
		_num++;
		if (_num > _maxLine)
		{
			return false;
		}
		_line = _doc.GetLineByNumber(_num);
		_text = _doc.GetText(_line);
		return true;
	}
}
