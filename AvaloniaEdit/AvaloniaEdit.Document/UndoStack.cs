using System;
using System.Collections.Generic;
using System.ComponentModel;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public sealed class UndoStack : INotifyPropertyChanged
{
	internal const int StateListen = 0;

	internal const int StatePlayback = 1;

	internal const int StatePlaybackModifyDocument = 2;

	private readonly Deque<IUndoableOperation> _undostack = new Deque<IUndoableOperation>();

	private readonly Deque<IUndoableOperation> _redostack = new Deque<IUndoableOperation>();

	private int _sizeLimit = int.MaxValue;

	private int _undoGroupDepth;

	private int _actionCountInUndoGroup;

	private int _optionalActionCount;

	private bool _allowContinue;

	private int _elementsOnUndoUntilOriginalFile;

	private List<TextDocument> _affectedDocuments;

	internal int State { get; set; }

	public bool IsOriginalFile { get; private set; } = true;

	public bool AcceptChanges => State == 0;

	public bool CanUndo => _undostack.Count > 0;

	public bool CanRedo => _redostack.Count > 0;

	public int SizeLimit
	{
		get
		{
			return _sizeLimit;
		}
		set
		{
			if (value < 0)
			{
				ThrowUtil.CheckNotNegative(value, "value");
			}
			if (_sizeLimit != value)
			{
				_sizeLimit = value;
				NotifyPropertyChanged("SizeLimit");
				if (_undoGroupDepth == 0)
				{
					EnforceSizeLimit();
				}
			}
		}
	}

	public object LastGroupDescriptor { get; private set; }

	public event PropertyChangedEventHandler PropertyChanged;

	private void RecalcIsOriginalFile()
	{
		bool flag = _elementsOnUndoUntilOriginalFile == 0;
		if (flag != IsOriginalFile)
		{
			IsOriginalFile = flag;
			NotifyPropertyChanged("IsOriginalFile");
		}
	}

	public void MarkAsOriginalFile()
	{
		_elementsOnUndoUntilOriginalFile = 0;
		RecalcIsOriginalFile();
	}

	public void DiscardOriginalFileMarker()
	{
		_elementsOnUndoUntilOriginalFile = int.MinValue;
		RecalcIsOriginalFile();
	}

	private void FileModified(int newElementsOnUndoStack)
	{
		if (_elementsOnUndoUntilOriginalFile != int.MinValue)
		{
			_elementsOnUndoUntilOriginalFile += newElementsOnUndoStack;
			if (_elementsOnUndoUntilOriginalFile > _undostack.Count)
			{
				_elementsOnUndoUntilOriginalFile = int.MinValue;
			}
		}
	}

	private void EnforceSizeLimit()
	{
		while (_undostack.Count > _sizeLimit)
		{
			_undostack.PopFront();
		}
		while (_redostack.Count > _sizeLimit)
		{
			_redostack.PopFront();
		}
	}

	public void StartUndoGroup()
	{
		StartUndoGroup(null);
	}

	public void StartUndoGroup(object groupDescriptor)
	{
		if (_undoGroupDepth == 0)
		{
			_actionCountInUndoGroup = 0;
			_optionalActionCount = 0;
			LastGroupDescriptor = groupDescriptor;
		}
		_undoGroupDepth++;
	}

	public void StartContinuedUndoGroup(object groupDescriptor = null)
	{
		if (_undoGroupDepth == 0)
		{
			_actionCountInUndoGroup = ((_allowContinue && _undostack.Count > 0) ? 1 : 0);
			_optionalActionCount = 0;
			LastGroupDescriptor = groupDescriptor;
		}
		_undoGroupDepth++;
	}

	public void EndUndoGroup()
	{
		if (_undoGroupDepth == 0)
		{
			throw new InvalidOperationException("There are no open undo groups");
		}
		_undoGroupDepth--;
		if (_undoGroupDepth != 0)
		{
			return;
		}
		_allowContinue = true;
		if (_actionCountInUndoGroup == _optionalActionCount)
		{
			for (int i = 0; i < _optionalActionCount; i++)
			{
				_undostack.PopBack();
			}
			_allowContinue = false;
		}
		else if (_actionCountInUndoGroup > 1)
		{
			_undostack.PushBack(new UndoOperationGroup(_undostack, _actionCountInUndoGroup));
			FileModified(-_actionCountInUndoGroup + 1 + _optionalActionCount);
		}
		EnforceSizeLimit();
		RecalcIsOriginalFile();
	}

	private void ThrowIfUndoGroupOpen()
	{
		if (_undoGroupDepth != 0)
		{
			_undoGroupDepth = 0;
			throw new InvalidOperationException("No undo group should be open at this point");
		}
		if (State != 0)
		{
			throw new InvalidOperationException("This method cannot be called while an undo operation is being performed");
		}
	}

	internal void RegisterAffectedDocument(TextDocument document)
	{
		if (_affectedDocuments == null)
		{
			_affectedDocuments = new List<TextDocument>();
		}
		if (!_affectedDocuments.Contains(document))
		{
			_affectedDocuments.Add(document);
			document.BeginUpdate();
		}
	}

	private void CallEndUpdateOnAffectedDocuments()
	{
		if (_affectedDocuments == null)
		{
			return;
		}
		foreach (TextDocument affectedDocument in _affectedDocuments)
		{
			affectedDocument.EndUpdate();
		}
		_affectedDocuments = null;
	}

	public void Undo()
	{
		ThrowIfUndoGroupOpen();
		if (_undostack.Count > 0)
		{
			LastGroupDescriptor = null;
			_allowContinue = false;
			IUndoableOperation undoableOperation = _undostack.PopBack();
			_redostack.PushBack(undoableOperation);
			State = 1;
			try
			{
				RunUndo(undoableOperation);
			}
			finally
			{
				State = 0;
				FileModified(-1);
				CallEndUpdateOnAffectedDocuments();
			}
			RecalcIsOriginalFile();
			if (_undostack.Count == 0)
			{
				NotifyPropertyChanged("CanUndo");
			}
			if (_redostack.Count == 1)
			{
				NotifyPropertyChanged("CanRedo");
			}
		}
	}

	internal void RunUndo(IUndoableOperation op)
	{
		if (op is IUndoableOperationWithContext undoableOperationWithContext)
		{
			undoableOperationWithContext.Undo(this);
		}
		else
		{
			op.Undo();
		}
	}

	public void Redo()
	{
		ThrowIfUndoGroupOpen();
		if (_redostack.Count > 0)
		{
			LastGroupDescriptor = null;
			_allowContinue = false;
			IUndoableOperation undoableOperation = _redostack.PopBack();
			_undostack.PushBack(undoableOperation);
			State = 1;
			try
			{
				RunRedo(undoableOperation);
			}
			finally
			{
				State = 0;
				FileModified(1);
				CallEndUpdateOnAffectedDocuments();
			}
			RecalcIsOriginalFile();
			if (_redostack.Count == 0)
			{
				NotifyPropertyChanged("CanRedo");
			}
			if (_undostack.Count == 1)
			{
				NotifyPropertyChanged("CanUndo");
			}
		}
	}

	internal void RunRedo(IUndoableOperation op)
	{
		if (op is IUndoableOperationWithContext undoableOperationWithContext)
		{
			undoableOperationWithContext.Redo(this);
		}
		else
		{
			op.Redo();
		}
	}

	public void Push(IUndoableOperation operation)
	{
		Push(operation, isOptional: false);
	}

	public void PushOptional(IUndoableOperation operation)
	{
		if (_undoGroupDepth == 0)
		{
			throw new InvalidOperationException("Cannot use PushOptional outside of undo group");
		}
		Push(operation, isOptional: true);
	}

	private void Push(IUndoableOperation operation, bool isOptional)
	{
		if (operation == null)
		{
			throw new ArgumentNullException("operation");
		}
		if (State == 0 && _sizeLimit > 0)
		{
			bool num = _undostack.Count == 0;
			bool num2 = _undoGroupDepth == 0;
			if (num2)
			{
				StartUndoGroup();
			}
			_undostack.PushBack(operation);
			_actionCountInUndoGroup++;
			if (isOptional)
			{
				_optionalActionCount++;
			}
			else
			{
				FileModified(1);
			}
			if (num2)
			{
				EndUndoGroup();
			}
			if (num)
			{
				NotifyPropertyChanged("CanUndo");
			}
			ClearRedoStack();
		}
	}

	public void ClearRedoStack()
	{
		if (_redostack.Count != 0)
		{
			_redostack.Clear();
			NotifyPropertyChanged("CanRedo");
			if (_elementsOnUndoUntilOriginalFile < 0)
			{
				_elementsOnUndoUntilOriginalFile = int.MinValue;
			}
		}
	}

	public void ClearAll()
	{
		ThrowIfUndoGroupOpen();
		_actionCountInUndoGroup = 0;
		_optionalActionCount = 0;
		if (_undostack.Count != 0)
		{
			LastGroupDescriptor = null;
			_allowContinue = false;
			_undostack.Clear();
			NotifyPropertyChanged("CanUndo");
		}
		ClearRedoStack();
	}

	internal void Push(TextDocument document, DocumentChangeEventArgs e)
	{
		if (State == 1)
		{
			throw new InvalidOperationException("Document changes during undo/redo operations are not allowed.");
		}
		if (State == 2)
		{
			State = 1;
		}
		else
		{
			Push(new DocumentChangeOperation(document, e));
		}
	}

	private void NotifyPropertyChanged(string propertyName)
	{
		PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
		this.PropertyChanged?.Invoke(this, e);
	}
}
