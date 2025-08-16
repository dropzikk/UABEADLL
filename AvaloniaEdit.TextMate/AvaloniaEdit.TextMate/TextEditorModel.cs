using System;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using TextMateSharp.Model;

namespace AvaloniaEdit.TextMate;

public class TextEditorModel : AbstractLineList, IDisposable
{
	internal class InvalidLineRange
	{
		internal int StartLine { get; private set; }

		internal int EndLine { get; private set; }

		internal InvalidLineRange(int startLine, int endLine)
		{
			StartLine = startLine;
			EndLine = endLine;
		}

		internal void SetInvalidRange(int startLine, int endLine)
		{
			if (startLine < StartLine)
			{
				StartLine = startLine;
			}
			if (endLine > EndLine)
			{
				EndLine = endLine;
			}
		}
	}

	private readonly TextDocument _document;

	private readonly TextView _textView;

	private DocumentSnapshot _documentSnapshot;

	private Action<Exception> _exceptionHandler;

	private InvalidLineRange _invalidRange;

	public DocumentSnapshot DocumentSnapshot => _documentSnapshot;

	internal InvalidLineRange InvalidRange => _invalidRange;

	public TextEditorModel(TextView textView, TextDocument document, Action<Exception> exceptionHandler)
	{
		_textView = textView;
		_document = document;
		_exceptionHandler = exceptionHandler;
		_documentSnapshot = new DocumentSnapshot(_document);
		for (int i = 0; i < _document.LineCount; i++)
		{
			AddLine(i);
		}
		_document.Changing += DocumentOnChanging;
		_document.Changed += DocumentOnChanged;
		_document.UpdateFinished += DocumentOnUpdateFinished;
		_textView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
	}

	public override void Dispose()
	{
		_document.Changing -= DocumentOnChanging;
		_document.Changed -= DocumentOnChanged;
		_document.UpdateFinished -= DocumentOnUpdateFinished;
		_textView.ScrollOffsetChanged -= TextView_ScrollOffsetChanged;
	}

	public override void UpdateLine(int lineIndex)
	{
	}

	public void InvalidateViewPortLines()
	{
		if (_textView.VisualLinesValid && _textView.VisualLines.Count != 0)
		{
			InvalidateLineRange(_textView.VisualLines[0].FirstDocumentLine.LineNumber - 1, _textView.VisualLines[_textView.VisualLines.Count - 1].LastDocumentLine.LineNumber - 1);
		}
	}

	public override int GetNumberOfLines()
	{
		return _documentSnapshot.LineCount;
	}

	public override string GetLineText(int lineIndex)
	{
		return _documentSnapshot.GetLineText(lineIndex);
	}

	public override int GetLineLength(int lineIndex)
	{
		return _documentSnapshot.GetLineLength(lineIndex);
	}

	private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
	{
		try
		{
			TokenizeViewPort();
		}
		catch (Exception obj)
		{
			_exceptionHandler?.Invoke(obj);
		}
	}

	private void DocumentOnChanging(object sender, DocumentChangeEventArgs e)
	{
		try
		{
			if (e.RemovalLength > 0)
			{
				int num = _document.GetLineByOffset(e.Offset).LineNumber - 1;
				int num2 = _document.GetLineByOffset(e.Offset + e.RemovalLength).LineNumber - 1;
				for (int num3 = num2; num3 > num; num3--)
				{
					RemoveLine(num3);
				}
				_documentSnapshot.RemoveLines(num, num2);
			}
		}
		catch (Exception obj)
		{
			_exceptionHandler?.Invoke(obj);
		}
	}

	private void DocumentOnChanged(object sender, DocumentChangeEventArgs e)
	{
		try
		{
			int num = _document.GetLineByOffset(e.Offset).LineNumber - 1;
			int num2 = num;
			if (e.InsertionLength > 0)
			{
				num2 = _document.GetLineByOffset(e.Offset + e.InsertionLength).LineNumber - 1;
				for (int i = num; i < num2; i++)
				{
					AddLine(i);
				}
			}
			_documentSnapshot.Update(e);
			if (num == 0)
			{
				SetInvalidRange(num, num2);
			}
			else
			{
				SetInvalidRange(num - 1, num2);
			}
		}
		catch (Exception obj)
		{
			_exceptionHandler?.Invoke(obj);
		}
	}

	private void SetInvalidRange(int startLine, int endLine)
	{
		if (!_document.IsInUpdate)
		{
			InvalidateLineRange(startLine, endLine);
		}
		else if (_invalidRange == null)
		{
			_invalidRange = new InvalidLineRange(startLine, endLine);
		}
		else
		{
			_invalidRange.SetInvalidRange(startLine, endLine);
		}
	}

	private void DocumentOnUpdateFinished(object sender, EventArgs e)
	{
		if (_invalidRange == null)
		{
			return;
		}
		try
		{
			InvalidateLineRange(_invalidRange.StartLine, _invalidRange.EndLine);
		}
		finally
		{
			_invalidRange = null;
		}
	}

	private void TokenizeViewPort()
	{
		Dispatcher.UIThread.InvokeAsync(delegate
		{
			if (_textView.VisualLinesValid && _textView.VisualLines.Count != 0)
			{
				ForceTokenization(_textView.VisualLines[0].FirstDocumentLine.LineNumber - 1, _textView.VisualLines[_textView.VisualLines.Count - 1].LastDocumentLine.LineNumber - 1);
			}
		}, DispatcherPriority.Default);
	}
}
