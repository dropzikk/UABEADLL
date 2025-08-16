using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Snippets;

public class InsertionContext
{
	private enum Status
	{
		Insertion,
		RaisingInsertionCompleted,
		Interactive,
		RaisingDeactivated,
		Deactivated
	}

	private Status _currentStatus;

	private readonly int _startPosition;

	private AnchorSegment _wholeSnippetAnchor;

	private bool _deactivateIfSnippetEmpty;

	private readonly Dictionary<SnippetElement, IActiveElement> _elementMap = new Dictionary<SnippetElement, IActiveElement>();

	private readonly List<IActiveElement> _registeredElements = new List<IActiveElement>();

	private SnippetInputHandler _myInputHandler;

	public TextArea TextArea { get; }

	public TextDocument Document { get; }

	public string SelectedText { get; }

	public string Indentation { get; }

	public string Tab { get; }

	public string LineTerminator { get; }

	public int InsertionPosition { get; set; }

	public int StartPosition
	{
		get
		{
			if (_wholeSnippetAnchor != null)
			{
				return _wholeSnippetAnchor.Offset;
			}
			return _startPosition;
		}
	}

	public IEnumerable<IActiveElement> ActiveElements => _registeredElements;

	public event EventHandler InsertionCompleted;

	public event EventHandler<SnippetEventArgs> Deactivated;

	public InsertionContext(TextArea textArea, int insertionPosition)
	{
		TextArea = textArea ?? throw new ArgumentNullException("textArea");
		Document = textArea.Document;
		SelectedText = textArea.Selection.GetText();
		InsertionPosition = insertionPosition;
		_startPosition = insertionPosition;
		DocumentLine lineByOffset = Document.GetLineByOffset(insertionPosition);
		ISegment whitespaceAfter = TextUtilities.GetWhitespaceAfter(Document, lineByOffset.Offset);
		Indentation = Document.GetText(whitespaceAfter.Offset, Math.Min(whitespaceAfter.EndOffset, insertionPosition) - whitespaceAfter.Offset);
		Tab = textArea.Options.IndentationString;
		LineTerminator = TextUtilities.GetNewLineFromDocument(Document, lineByOffset.LineNumber);
	}

	public void InsertText(string text)
	{
		if (_currentStatus != 0)
		{
			throw new InvalidOperationException();
		}
		text = text?.Replace("\t", Tab) ?? throw new ArgumentNullException("text");
		using (Document.RunUpdate())
		{
			int num = 0;
			SimpleSegment simpleSegment;
			while ((simpleSegment = NewLineFinder.NextNewLine(text, num)) != SimpleSegment.Invalid)
			{
				string text2 = text.Substring(num, simpleSegment.Offset - num) + LineTerminator + Indentation;
				Document.Insert(InsertionPosition, text2);
				InsertionPosition += text2.Length;
				num = simpleSegment.EndOffset;
			}
			string text3 = text.Substring(num);
			Document.Insert(InsertionPosition, text3);
			InsertionPosition += text3.Length;
		}
	}

	public void RegisterActiveElement(SnippetElement owner, IActiveElement element)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (_currentStatus != 0)
		{
			throw new InvalidOperationException();
		}
		_elementMap.Add(owner, element);
		_registeredElements.Add(element);
	}

	public IActiveElement GetActiveElement(SnippetElement owner)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (!_elementMap.TryGetValue(owner, out var value))
		{
			return null;
		}
		return value;
	}

	public void RaiseInsertionCompleted(EventArgs e)
	{
		if (_currentStatus != 0)
		{
			throw new InvalidOperationException();
		}
		if (e == null)
		{
			e = EventArgs.Empty;
		}
		_currentStatus = Status.RaisingInsertionCompleted;
		int insertionPosition = InsertionPosition;
		_wholeSnippetAnchor = new AnchorSegment(Document, _startPosition, insertionPosition - _startPosition);
		WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument, EventHandler, EventArgs>.AddHandler(Document, OnUpdateFinished);
		_deactivateIfSnippetEmpty = insertionPosition != _startPosition;
		foreach (IActiveElement registeredElement in _registeredElements)
		{
			registeredElement.OnInsertionCompleted();
		}
		this.InsertionCompleted?.Invoke(this, e);
		_currentStatus = Status.Interactive;
		if (_registeredElements.Count == 0)
		{
			Deactivate(new SnippetEventArgs(DeactivateReason.NoActiveElements));
			return;
		}
		_myInputHandler = new SnippetInputHandler(this);
		ImmutableStack<TextAreaStackedInputHandler>.Enumerator enumerator2 = TextArea.StackedInputHandlers.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			TextAreaStackedInputHandler current = enumerator2.Current;
			if (current is SnippetInputHandler)
			{
				TextArea.PopStackedInputHandler(current);
			}
		}
		TextArea.PushStackedInputHandler(_myInputHandler);
	}

	public void Deactivate(SnippetEventArgs e)
	{
		if (_currentStatus == Status.Deactivated || _currentStatus == Status.RaisingDeactivated)
		{
			return;
		}
		if (_currentStatus != Status.Interactive)
		{
			throw new InvalidOperationException("Cannot call Deactivate() until RaiseInsertionCompleted() has finished.");
		}
		if (e == null)
		{
			e = new SnippetEventArgs(DeactivateReason.Unknown);
		}
		WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument, EventHandler, EventArgs>.RemoveHandler(Document, OnUpdateFinished);
		_currentStatus = Status.RaisingDeactivated;
		TextArea.PopStackedInputHandler(_myInputHandler);
		foreach (IActiveElement registeredElement in _registeredElements)
		{
			registeredElement.Deactivate(e);
		}
		this.Deactivated?.Invoke(this, e);
		_currentStatus = Status.Deactivated;
	}

	private void OnUpdateFinished(object sender, EventArgs e)
	{
		if (_wholeSnippetAnchor.Length == 0 && _deactivateIfSnippetEmpty)
		{
			Deactivate(new SnippetEventArgs(DeactivateReason.Deleted));
		}
	}

	public void Link(ISegment mainElement, ISegment[] boundElements)
	{
		SnippetReplaceableTextElement snippetReplaceableTextElement = new SnippetReplaceableTextElement
		{
			Text = Document.GetText(mainElement)
		};
		RegisterActiveElement(snippetReplaceableTextElement, new ReplaceableActiveElement(this, mainElement.Offset, mainElement.EndOffset));
		foreach (ISegment segment in boundElements)
		{
			SnippetBoundElement snippetBoundElement = new SnippetBoundElement
			{
				TargetElement = snippetReplaceableTextElement
			};
			TextAnchor textAnchor = Document.CreateAnchor(segment.Offset);
			textAnchor.MovementType = AnchorMovementType.BeforeInsertion;
			textAnchor.SurviveDeletion = true;
			TextAnchor textAnchor2 = Document.CreateAnchor(segment.EndOffset);
			textAnchor2.MovementType = AnchorMovementType.BeforeInsertion;
			textAnchor2.SurviveDeletion = true;
			RegisterActiveElement(snippetBoundElement, new BoundActiveElement(this, snippetReplaceableTextElement, snippetBoundElement, new AnchorSegment(textAnchor, textAnchor2)));
		}
	}
}
