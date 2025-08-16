using System;

namespace AvaloniaEdit.Document;

public sealed class WeakLineTracker : ILineTracker
{
	private TextDocument _textDocument;

	private readonly WeakReference _targetObject;

	private WeakLineTracker(TextDocument textDocument, ILineTracker targetTracker)
	{
		_textDocument = textDocument;
		_targetObject = new WeakReference(targetTracker);
	}

	public static WeakLineTracker Register(TextDocument textDocument, ILineTracker targetTracker)
	{
		if (textDocument == null)
		{
			throw new ArgumentNullException("textDocument");
		}
		if (targetTracker == null)
		{
			throw new ArgumentNullException("targetTracker");
		}
		WeakLineTracker weakLineTracker = new WeakLineTracker(textDocument, targetTracker);
		textDocument.LineTrackers.Add(weakLineTracker);
		return weakLineTracker;
	}

	public void Deregister()
	{
		if (_textDocument != null)
		{
			_textDocument.LineTrackers.Remove(this);
			_textDocument = null;
		}
	}

	void ILineTracker.BeforeRemoveLine(DocumentLine line)
	{
		if (_targetObject.Target is ILineTracker lineTracker)
		{
			lineTracker.BeforeRemoveLine(line);
		}
		else
		{
			Deregister();
		}
	}

	void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
	{
		if (_targetObject.Target is ILineTracker lineTracker)
		{
			lineTracker.SetLineLength(line, newTotalLength);
		}
		else
		{
			Deregister();
		}
	}

	void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
	{
		if (_targetObject.Target is ILineTracker lineTracker)
		{
			lineTracker.LineInserted(insertionPos, newLine);
		}
		else
		{
			Deregister();
		}
	}

	void ILineTracker.RebuildDocument()
	{
		if (_targetObject.Target is ILineTracker lineTracker)
		{
			lineTracker.RebuildDocument();
		}
		else
		{
			Deregister();
		}
	}

	void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
	{
		if (_targetObject.Target is ILineTracker lineTracker)
		{
			lineTracker.ChangeComplete(e);
		}
		else
		{
			Deregister();
		}
	}
}
