using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public sealed class TextDocument : IDocument, ITextSource, IServiceProvider, INotifyPropertyChanged
{
	private readonly object _lockObject = new object();

	private readonly Rope<char> _rope;

	private readonly DocumentLineTree _lineTree;

	private readonly LineManager _lineManager;

	private readonly TextAnchorTree _anchorTree;

	private readonly TextSourceVersionProvider _versionProvider = new TextSourceVersionProvider();

	private Thread ownerThread = Thread.CurrentThread;

	private WeakReference _cachedText;

	private int _beginUpdateCount;

	private int _oldTextLength;

	private int _oldLineCount;

	private bool _fireTextChanged;

	internal bool InDocumentChanging;

	private readonly ObservableCollection<ILineTracker> _lineTrackers = new ObservableCollection<ILineTracker>();

	public UndoStack _undoStack;

	private IServiceProvider _serviceProvider;

	private string _fileName;

	public string Text
	{
		get
		{
			VerifyAccess();
			string text = _cachedText?.Target as string;
			if (text == null)
			{
				text = _rope.ToString();
				_cachedText = new WeakReference(text);
			}
			return text;
		}
		set
		{
			VerifyAccess();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Replace(0, _rope.Length, value);
		}
	}

	public int TextLength
	{
		get
		{
			VerifyAccess();
			return _rope.Length;
		}
	}

	public ITextSourceVersion Version => _versionProvider.CurrentVersion;

	public bool IsInUpdate
	{
		get
		{
			VerifyAccess();
			return _beginUpdateCount > 0;
		}
	}

	public IList<DocumentLine> Lines => _lineTree;

	public IList<ILineTracker> LineTrackers
	{
		get
		{
			VerifyAccess();
			return _lineTrackers;
		}
	}

	public UndoStack UndoStack
	{
		get
		{
			return _undoStack;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (value != _undoStack)
			{
				_undoStack.ClearAll();
				_undoStack = value;
				OnPropertyChanged("UndoStack");
			}
		}
	}

	public int LineCount
	{
		get
		{
			VerifyAccess();
			return _lineTree.LineCount;
		}
	}

	internal IServiceProvider ServiceProvider
	{
		get
		{
			VerifyAccess();
			if (_serviceProvider == null)
			{
				ServiceContainer serviceContainer = new ServiceContainer();
				serviceContainer.AddService(this);
				((IServiceContainer)serviceContainer).AddService((IDocument)this);
				_serviceProvider = serviceContainer;
			}
			return _serviceProvider;
		}
		set
		{
			VerifyAccess();
			_serviceProvider = value ?? throw new ArgumentNullException("value");
		}
	}

	public string FileName
	{
		get
		{
			return _fileName;
		}
		set
		{
			if (_fileName != value)
			{
				_fileName = value;
				OnFileNameChanged(EventArgs.Empty);
			}
		}
	}

	public event EventHandler TextChanged;

	event EventHandler IDocument.ChangeCompleted
	{
		add
		{
			TextChanged += value;
		}
		remove
		{
			TextChanged -= value;
		}
	}

	public event EventHandler TextLengthChanged;

	public event EventHandler<DocumentChangeEventArgs> Changing;

	private event EventHandler<TextChangeEventArgs> TextChangingInternal;

	event EventHandler<TextChangeEventArgs> IDocument.TextChanging
	{
		add
		{
			TextChangingInternal += value;
		}
		remove
		{
			TextChangingInternal -= value;
		}
	}

	public event EventHandler<DocumentChangeEventArgs> Changed;

	private event EventHandler<TextChangeEventArgs> TextChangedInternal;

	event EventHandler<TextChangeEventArgs> IDocument.TextChanged
	{
		add
		{
			TextChangedInternal += value;
		}
		remove
		{
			TextChangedInternal -= value;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public event EventHandler UpdateStarted;

	public event EventHandler UpdateFinished;

	public event EventHandler LineCountChanged;

	public event EventHandler FileNameChanged;

	public TextDocument()
		: this(string.Empty.ToCharArray())
	{
	}

	public TextDocument(IEnumerable<char> initialText)
	{
		if (initialText == null)
		{
			throw new ArgumentNullException("initialText");
		}
		_rope = new Rope<char>(initialText);
		_lineTree = new DocumentLineTree(this);
		_lineManager = new LineManager(_lineTree, this);
		_lineTrackers.CollectionChanged += delegate
		{
			_lineManager.UpdateListOfLineTrackers();
		};
		_anchorTree = new TextAnchorTree(this);
		_undoStack = new UndoStack();
		FireChangeEvents();
	}

	public TextDocument(ITextSource initialText)
		: this(GetTextFromTextSource(initialText))
	{
	}

	private static IEnumerable<char> GetTextFromTextSource(ITextSource textSource)
	{
		if (textSource == null)
		{
			throw new ArgumentNullException("textSource");
		}
		if (textSource is RopeTextSource ropeTextSource)
		{
			return ropeTextSource.GetRope();
		}
		if (textSource is TextDocument textDocument)
		{
			return textDocument._rope;
		}
		return textSource.Text.ToCharArray();
	}

	private void ThrowIfRangeInvalid(int offset, int length)
	{
		if (offset < 0 || offset > _rope.Length)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + _rope.Length.ToString(CultureInfo.InvariantCulture));
		}
		if (length < 0 || offset + length > _rope.Length)
		{
			throw new ArgumentOutOfRangeException("length", length, "0 <= length, offset(" + offset + ")+length <= " + _rope.Length.ToString(CultureInfo.InvariantCulture));
		}
	}

	public string GetText(int offset, int length)
	{
		VerifyAccess();
		return _rope.ToString(offset, length);
	}

	public void SetOwnerThread(Thread newOwner)
	{
		lock (_lockObject)
		{
			if (ownerThread != null)
			{
				VerifyAccess();
			}
			ownerThread = newOwner;
		}
	}

	private void VerifyAccess()
	{
		if (Thread.CurrentThread != ownerThread)
		{
			throw new InvalidOperationException("Call from invalid thread.");
		}
	}

	public string GetText(ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return GetText(segment.Offset, segment.Length);
	}

	public int IndexOf(char c, int startIndex, int count)
	{
		return _rope.IndexOf(c, startIndex, count);
	}

	public int LastIndexOf(char c, int startIndex, int count)
	{
		return _rope.LastIndexOf(c, startIndex, count);
	}

	public int IndexOfAny(char[] anyOf, int startIndex, int count)
	{
		return _rope.IndexOfAny(anyOf, startIndex, count);
	}

	public int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return _rope.IndexOf(searchText, startIndex, count, comparisonType);
	}

	public int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return _rope.LastIndexOf(searchText, startIndex, count, comparisonType);
	}

	public char GetCharAt(int offset)
	{
		return _rope[offset];
	}

	public ITextSource CreateSnapshot()
	{
		lock (_lockObject)
		{
			return new RopeTextSource(_rope, _versionProvider.CurrentVersion);
		}
	}

	public ITextSource CreateSnapshot(int offset, int length)
	{
		lock (_lockObject)
		{
			return new RopeTextSource(_rope.GetRange(offset, length));
		}
	}

	public TextReader CreateReader()
	{
		lock (_lockObject)
		{
			return new RopeTextReader(_rope);
		}
	}

	public TextReader CreateReader(int offset, int length)
	{
		lock (_lockObject)
		{
			return new RopeTextReader(_rope.GetRange(offset, length));
		}
	}

	public void WriteTextTo(TextWriter writer)
	{
		VerifyAccess();
		_rope.WriteTo(writer, 0, _rope.Length);
	}

	public void WriteTextTo(TextWriter writer, int offset, int length)
	{
		VerifyAccess();
		_rope.WriteTo(writer, offset, length);
	}

	private void OnPropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public IDisposable RunUpdate()
	{
		BeginUpdate();
		return new CallbackOnDispose(EndUpdate);
	}

	public void BeginUpdate()
	{
		VerifyAccess();
		if (InDocumentChanging)
		{
			throw new InvalidOperationException("Cannot change document within another document change.");
		}
		_beginUpdateCount++;
		if (_beginUpdateCount == 1)
		{
			_undoStack.StartUndoGroup();
			this.UpdateStarted?.Invoke(this, EventArgs.Empty);
		}
	}

	public void EndUpdate()
	{
		VerifyAccess();
		if (InDocumentChanging)
		{
			throw new InvalidOperationException("Cannot end update within document change.");
		}
		if (_beginUpdateCount == 0)
		{
			throw new InvalidOperationException("No update is active.");
		}
		if (_beginUpdateCount == 1)
		{
			FireChangeEvents();
			_undoStack.EndUndoGroup();
			_beginUpdateCount = 0;
			this.UpdateFinished?.Invoke(this, EventArgs.Empty);
		}
		else
		{
			_beginUpdateCount--;
		}
	}

	void IDocument.StartUndoableAction()
	{
		BeginUpdate();
	}

	void IDocument.EndUndoableAction()
	{
		EndUpdate();
	}

	IDisposable IDocument.OpenUndoGroup()
	{
		return RunUpdate();
	}

	internal void FireChangeEvents()
	{
		while (_fireTextChanged)
		{
			_fireTextChanged = false;
			this.TextChanged?.Invoke(this, EventArgs.Empty);
			OnPropertyChanged("Text");
			int length = _rope.Length;
			if (length != _oldTextLength)
			{
				_oldTextLength = length;
				this.TextLengthChanged?.Invoke(this, EventArgs.Empty);
				OnPropertyChanged("TextLength");
			}
			int lineCount = _lineTree.LineCount;
			if (lineCount != _oldLineCount)
			{
				_oldLineCount = lineCount;
				this.LineCountChanged?.Invoke(this, EventArgs.Empty);
				OnPropertyChanged("LineCount");
			}
		}
	}

	public void Insert(int offset, string text)
	{
		Replace(offset, 0, new StringTextSource(text), null);
	}

	public void Insert(int offset, ITextSource text)
	{
		Replace(offset, 0, text, null);
	}

	public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
	{
		if (defaultAnchorMovementType == AnchorMovementType.BeforeInsertion)
		{
			Replace(offset, 0, new StringTextSource(text), OffsetChangeMappingType.KeepAnchorBeforeInsertion);
		}
		else
		{
			Replace(offset, 0, new StringTextSource(text), null);
		}
	}

	public void Insert(int offset, ITextSource text, AnchorMovementType defaultAnchorMovementType)
	{
		if (defaultAnchorMovementType == AnchorMovementType.BeforeInsertion)
		{
			Replace(offset, 0, text, OffsetChangeMappingType.KeepAnchorBeforeInsertion);
		}
		else
		{
			Replace(offset, 0, text, null);
		}
	}

	public void Remove(ISegment segment)
	{
		Replace(segment, string.Empty);
	}

	public void Remove(int offset, int length)
	{
		Replace(offset, length, StringTextSource.Empty);
	}

	public void Replace(ISegment segment, string text)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		Replace(segment.Offset, segment.Length, new StringTextSource(text), null);
	}

	public void Replace(ISegment segment, ITextSource text)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		Replace(segment.Offset, segment.Length, text, null);
	}

	public void Replace(int offset, int length, string text)
	{
		Replace(offset, length, new StringTextSource(text), null);
	}

	public void Replace(int offset, int length, ITextSource text)
	{
		Replace(offset, length, text, null);
	}

	public void Replace(int offset, int length, string text, OffsetChangeMappingType offsetChangeMappingType)
	{
		Replace(offset, length, new StringTextSource(text), offsetChangeMappingType);
	}

	public void Replace(int offset, int length, ITextSource text, OffsetChangeMappingType offsetChangeMappingType)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		switch (offsetChangeMappingType)
		{
		case OffsetChangeMappingType.Normal:
			Replace(offset, length, text, null);
			break;
		case OffsetChangeMappingType.KeepAnchorBeforeInsertion:
			Replace(offset, length, text, OffsetChangeMap.FromSingleElement(new OffsetChangeMapEntry(offset, length, text.TextLength, removalNeverCausesAnchorDeletion: false, defaultAnchorMovementIsBeforeInsertion: true)));
			break;
		case OffsetChangeMappingType.RemoveAndInsert:
		{
			if (length == 0 || text.TextLength == 0)
			{
				Replace(offset, length, text, null);
				break;
			}
			OffsetChangeMap offsetChangeMap = new OffsetChangeMap(2)
			{
				new OffsetChangeMapEntry(offset, length, 0),
				new OffsetChangeMapEntry(offset, 0, text.TextLength)
			};
			offsetChangeMap.Freeze();
			Replace(offset, length, text, offsetChangeMap);
			break;
		}
		case OffsetChangeMappingType.CharacterReplace:
			if (length == 0 || text.TextLength == 0)
			{
				Replace(offset, length, text, null);
			}
			else if (text.TextLength > length)
			{
				OffsetChangeMapEntry entry = new OffsetChangeMapEntry(offset + length - 1, 1, 1 + text.TextLength - length);
				Replace(offset, length, text, OffsetChangeMap.FromSingleElement(entry));
			}
			else if (text.TextLength < length)
			{
				OffsetChangeMapEntry entry2 = new OffsetChangeMapEntry(offset + text.TextLength, length - text.TextLength, 0, removalNeverCausesAnchorDeletion: true, defaultAnchorMovementIsBeforeInsertion: false);
				Replace(offset, length, text, OffsetChangeMap.FromSingleElement(entry2));
			}
			else
			{
				Replace(offset, length, text, OffsetChangeMap.Empty);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("offsetChangeMappingType", offsetChangeMappingType, "Invalid enum value");
		}
	}

	public void Replace(int offset, int length, string text, OffsetChangeMap offsetChangeMap)
	{
		Replace(offset, length, new StringTextSource(text), offsetChangeMap);
	}

	public void Replace(int offset, int length, ITextSource text, OffsetChangeMap offsetChangeMap)
	{
		text = text?.CreateSnapshot() ?? throw new ArgumentNullException("text");
		offsetChangeMap?.Freeze();
		BeginUpdate();
		try
		{
			InDocumentChanging = true;
			try
			{
				ThrowIfRangeInvalid(offset, length);
				DoReplace(offset, length, text, offsetChangeMap);
			}
			finally
			{
				InDocumentChanging = false;
			}
		}
		finally
		{
			EndUpdate();
		}
	}

	private void DoReplace(int offset, int length, ITextSource newText, OffsetChangeMap offsetChangeMap)
	{
		if (length == 0 && newText.TextLength == 0)
		{
			return;
		}
		if (length == 1 && newText.TextLength == 1 && offsetChangeMap == null)
		{
			offsetChangeMap = OffsetChangeMap.Empty;
		}
		ITextSource removedText = ((length == 0) ? StringTextSource.Empty : ((length >= 100) ? ((ITextSource)new RopeTextSource(_rope.GetRange(offset, length))) : ((ITextSource)new StringTextSource(_rope.ToString(offset, length)))));
		DocumentChangeEventArgs documentChangeEventArgs = new DocumentChangeEventArgs(offset, removedText, newText, offsetChangeMap);
		this.Changing?.Invoke(this, documentChangeEventArgs);
		this.TextChangingInternal?.Invoke(this, documentChangeEventArgs);
		_undoStack.Push(this, documentChangeEventArgs);
		_cachedText = null;
		_fireTextChanged = true;
		DelayedEvents delayedEvents = new DelayedEvents();
		lock (_lockObject)
		{
			_versionProvider.AppendChange(documentChangeEventArgs);
			if (offset == 0 && length == _rope.Length)
			{
				_rope.Clear();
				if (newText is RopeTextSource ropeTextSource)
				{
					_rope.InsertRange(0, ropeTextSource.GetRope());
				}
				else
				{
					_rope.InsertText(0, newText.Text);
				}
				_lineManager.Rebuild();
			}
			else
			{
				_rope.RemoveRange(offset, length);
				_lineManager.Remove(offset, length);
				if (newText is RopeTextSource ropeTextSource2)
				{
					_rope.InsertRange(offset, ropeTextSource2.GetRope());
				}
				else
				{
					_rope.InsertText(offset, newText.Text);
				}
				_lineManager.Insert(offset, newText);
			}
		}
		if (offsetChangeMap == null)
		{
			_anchorTree.HandleTextChange(documentChangeEventArgs.CreateSingleChangeMapEntry(), delayedEvents);
		}
		else
		{
			foreach (OffsetChangeMapEntry item in offsetChangeMap)
			{
				_anchorTree.HandleTextChange(item, delayedEvents);
			}
		}
		_lineManager.ChangeComplete(documentChangeEventArgs);
		delayedEvents.RaiseEvents();
		this.Changed?.Invoke(this, documentChangeEventArgs);
		this.TextChangedInternal?.Invoke(this, documentChangeEventArgs);
	}

	public DocumentLine GetLineByNumber(int number)
	{
		VerifyAccess();
		if (number < 1 || number > _lineTree.LineCount)
		{
			throw new ArgumentOutOfRangeException("number", number, "Value must be between 1 and " + _lineTree.LineCount);
		}
		return _lineTree.GetByNumber(number);
	}

	IDocumentLine IDocument.GetLineByNumber(int lineNumber)
	{
		return GetLineByNumber(lineNumber);
	}

	public DocumentLine GetLineByOffset(int offset)
	{
		VerifyAccess();
		if (offset < 0 || offset > _rope.Length)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + _rope.Length);
		}
		return _lineTree.GetByOffset(offset);
	}

	IDocumentLine IDocument.GetLineByOffset(int offset)
	{
		return GetLineByOffset(offset);
	}

	public int GetOffset(TextLocation location)
	{
		return GetOffset(location.Line, location.Column);
	}

	public int GetOffset(int line, int column)
	{
		DocumentLine lineByNumber = GetLineByNumber(line);
		if (column <= 0)
		{
			return lineByNumber.Offset;
		}
		if (column > lineByNumber.Length)
		{
			return lineByNumber.EndOffset;
		}
		return lineByNumber.Offset + column - 1;
	}

	public TextLocation GetLocation(int offset)
	{
		DocumentLine lineByOffset = GetLineByOffset(offset);
		return new TextLocation(lineByOffset.LineNumber, offset - lineByOffset.Offset + 1);
	}

	public TextAnchor CreateAnchor(int offset)
	{
		VerifyAccess();
		if (offset < 0 || offset > _rope.Length)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + _rope.Length.ToString(CultureInfo.InvariantCulture));
		}
		return _anchorTree.CreateAnchor(offset);
	}

	ITextAnchor IDocument.CreateAnchor(int offset)
	{
		return CreateAnchor(offset);
	}

	[Conditional("DEBUG")]
	internal void DebugVerifyAccess()
	{
		VerifyAccess();
	}

	internal string GetLineTreeAsString()
	{
		return "Not available in release build.";
	}

	internal string GetTextAnchorTreeAsString()
	{
		return "Not available in release build.";
	}

	object IServiceProvider.GetService(Type serviceType)
	{
		return ServiceProvider.GetService(serviceType);
	}

	private void OnFileNameChanged(EventArgs e)
	{
		this.FileNameChanged?.Invoke(this, e);
	}
}
