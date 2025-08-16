using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Highlighting;

public class DocumentHighlighter : ILineTracker, IHighlighter, IDisposable
{
	private readonly CompressingTreeList<ImmutableStack<HighlightingSpan>> _storedSpanStacks = new CompressingTreeList<ImmutableStack<HighlightingSpan>>(object.ReferenceEquals);

	private readonly CompressingTreeList<bool> _isValid = new CompressingTreeList<bool>((bool a, bool b) => a == b);

	private readonly IHighlightingDefinition _definition;

	private readonly HighlightingEngine _engine;

	private readonly WeakLineTracker _weakLineTracker;

	private bool _isHighlighting;

	private bool _isInHighlightingGroup;

	private bool _isDisposed;

	private ImmutableStack<HighlightingSpan> _initialSpanStack = ImmutableStack<HighlightingSpan>.Empty;

	private int _firstInvalidLine;

	public IDocument Document { get; }

	public ImmutableStack<HighlightingSpan> InitialSpanStack
	{
		get
		{
			return _initialSpanStack;
		}
		set
		{
			_initialSpanStack = value ?? ImmutableStack<HighlightingSpan>.Empty;
			InvalidateHighlighting();
		}
	}

	public HighlightingColor DefaultTextColor => null;

	public event HighlightingStateChangedEventHandler HighlightingStateChanged;

	public DocumentHighlighter(TextDocument document, IHighlightingDefinition definition)
	{
		Document = document ?? throw new ArgumentNullException("document");
		_definition = definition ?? throw new ArgumentNullException("definition");
		_engine = new HighlightingEngine(definition.MainRuleSet);
		Dispatcher.UIThread.VerifyAccess();
		_weakLineTracker = WeakLineTracker.Register(document, this);
		InvalidateSpanStacks();
	}

	public void Dispose()
	{
		_weakLineTracker?.Deregister();
		_isDisposed = true;
	}

	void ILineTracker.BeforeRemoveLine(DocumentLine line)
	{
		CheckIsHighlighting();
		int lineNumber = line.LineNumber;
		_storedSpanStacks.RemoveAt(lineNumber);
		_isValid.RemoveAt(lineNumber);
		if (lineNumber < _isValid.Count)
		{
			_isValid[lineNumber] = false;
			if (lineNumber < _firstInvalidLine)
			{
				_firstInvalidLine = lineNumber;
			}
		}
	}

	void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
	{
		CheckIsHighlighting();
		int lineNumber = line.LineNumber;
		_isValid[lineNumber] = false;
		if (lineNumber < _firstInvalidLine)
		{
			_firstInvalidLine = lineNumber;
		}
	}

	void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
	{
		CheckIsHighlighting();
		int lineNumber = newLine.LineNumber;
		_storedSpanStacks.Insert(lineNumber, null);
		_isValid.Insert(lineNumber, item: false);
		if (lineNumber < _firstInvalidLine)
		{
			_firstInvalidLine = lineNumber;
		}
	}

	void ILineTracker.RebuildDocument()
	{
		InvalidateSpanStacks();
	}

	void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
	{
	}

	public void InvalidateHighlighting()
	{
		InvalidateSpanStacks();
		OnHighlightStateChanged(1, Document.LineCount);
	}

	private void InvalidateSpanStacks()
	{
		CheckIsHighlighting();
		_storedSpanStacks.Clear();
		_storedSpanStacks.Add(_initialSpanStack);
		_storedSpanStacks.InsertRange(1, Document.LineCount, null);
		_isValid.Clear();
		_isValid.Add(item: true);
		_isValid.InsertRange(1, Document.LineCount, item: false);
		_firstInvalidLine = 1;
	}

	public HighlightedLine HighlightLine(int lineNumber)
	{
		ThrowUtil.CheckInRangeInclusive(lineNumber, "lineNumber", 1, Document.LineCount);
		CheckIsHighlighting();
		_isHighlighting = true;
		try
		{
			HighlightUpTo(lineNumber - 1);
			IDocumentLine lineByNumber = Document.GetLineByNumber(lineNumber);
			HighlightedLine result = _engine.HighlightLine(Document, lineByNumber);
			UpdateTreeList(lineNumber);
			return result;
		}
		finally
		{
			_isHighlighting = false;
		}
	}

	public ImmutableStack<HighlightingSpan> GetSpanStack(int lineNumber)
	{
		ThrowUtil.CheckInRangeInclusive(lineNumber, "lineNumber", 0, Document.LineCount);
		if (_firstInvalidLine <= lineNumber)
		{
			UpdateHighlightingState(lineNumber);
		}
		return _storedSpanStacks[lineNumber];
	}

	public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
	{
		return from s in GetSpanStack(lineNumber)
			select s.SpanColor into s
			where s != null
			select s;
	}

	private void CheckIsHighlighting()
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException("DocumentHighlighter");
		}
		if (_isHighlighting)
		{
			throw new InvalidOperationException("Invalid call - a highlighting operation is currently running.");
		}
	}

	public void UpdateHighlightingState(int lineNumber)
	{
		CheckIsHighlighting();
		_isHighlighting = true;
		try
		{
			HighlightUpTo(lineNumber);
		}
		finally
		{
			_isHighlighting = false;
		}
	}

	private void HighlightUpTo(int targetLineNumber)
	{
		for (int i = 0; i <= targetLineNumber; i++)
		{
			if (_firstInvalidLine > i)
			{
				if (_firstInvalidLine > targetLineNumber)
				{
					_engine.CurrentSpanStack = _storedSpanStacks[targetLineNumber];
					break;
				}
				_engine.CurrentSpanStack = _storedSpanStacks[_firstInvalidLine - 1];
				i = _firstInvalidLine;
			}
			_engine.ScanLine(Document, Document.GetLineByNumber(i));
			UpdateTreeList(i);
		}
	}

	private void UpdateTreeList(int lineNumber)
	{
		if (!EqualSpanStacks(_engine.CurrentSpanStack, _storedSpanStacks[lineNumber]))
		{
			_isValid[lineNumber] = true;
			_storedSpanStacks[lineNumber] = _engine.CurrentSpanStack;
			if (lineNumber + 1 < _isValid.Count)
			{
				_isValid[lineNumber + 1] = false;
				_firstInvalidLine = lineNumber + 1;
			}
			else
			{
				_firstInvalidLine = int.MaxValue;
			}
			if (lineNumber + 1 < Document.LineCount)
			{
				OnHighlightStateChanged(lineNumber + 1, lineNumber + 1);
			}
		}
		else if (_firstInvalidLine == lineNumber)
		{
			_isValid[lineNumber] = true;
			_firstInvalidLine = _isValid.IndexOf(item: false);
			if (_firstInvalidLine < 0)
			{
				_firstInvalidLine = int.MaxValue;
			}
		}
	}

	private static bool EqualSpanStacks(ImmutableStack<HighlightingSpan> a, ImmutableStack<HighlightingSpan> b)
	{
		if (a == b)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		while (!a.IsEmpty && !b.IsEmpty)
		{
			if (a.Peek() != b.Peek())
			{
				return false;
			}
			a = a.Pop();
			b = b.Pop();
			if (a == b)
			{
				return true;
			}
		}
		if (a.IsEmpty)
		{
			return b.IsEmpty;
		}
		return false;
	}

	protected virtual void OnHighlightStateChanged(int fromLineNumber, int toLineNumber)
	{
		this.HighlightingStateChanged?.Invoke(fromLineNumber, toLineNumber);
	}

	public void BeginHighlighting()
	{
		if (_isInHighlightingGroup)
		{
			throw new InvalidOperationException("Highlighting group is already open");
		}
		_isInHighlightingGroup = true;
	}

	public void EndHighlighting()
	{
		if (!_isInHighlightingGroup)
		{
			throw new InvalidOperationException("Highlighting group is not open");
		}
		_isInHighlightingGroup = false;
	}

	public HighlightingColor GetNamedColor(string name)
	{
		return _definition.GetNamedColor(name);
	}
}
