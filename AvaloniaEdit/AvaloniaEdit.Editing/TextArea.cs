using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using AvaloniaEdit.Indentation;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

public class TextArea : TemplatedControl, ITextEditorComponent, IServiceProvider, IRoutedCommandBindable, ILogicalScrollable, IScrollable
{
	private sealed class RestoreCaretAndSelectionUndoAction : IUndoableOperation
	{
		private readonly WeakReference _textAreaReference;

		private readonly TextViewPosition _caretPosition;

		private readonly Selection _selection;

		public RestoreCaretAndSelectionUndoAction(TextArea textArea)
		{
			_textAreaReference = new WeakReference(textArea);
			_caretPosition = textArea.Caret.NonValidatedPosition;
			_selection = textArea.Selection;
		}

		public void Undo()
		{
			TextArea textArea = (TextArea)_textAreaReference.Target;
			if (textArea != null)
			{
				textArea.Caret.Position = _caretPosition;
				textArea.Selection = _selection;
			}
		}

		public void Redo()
		{
			Undo();
		}
	}

	private class TextAreaTextInputMethodClient : TextInputMethodClient
	{
		private TextArea _textArea;

		public override Rect CursorRectangle
		{
			get
			{
				if (_textArea == null)
				{
					return default(Rect);
				}
				Matrix? matrix = _textArea.TextView.TransformToVisual(_textArea);
				if (!matrix.HasValue)
				{
					return default(Rect);
				}
				return _textArea.Caret.CalculateCaretRectangle().TransformToAABB(matrix.Value);
			}
		}

		public override Visual TextViewVisual => _textArea;

		public override bool SupportsPreedit => false;

		public override bool SupportsSurroundingText => true;

		public override string SurroundingText
		{
			get
			{
				if (_textArea == null)
				{
					return null;
				}
				int line = _textArea.Caret.Line;
				DocumentLine lineByNumber = _textArea.Document.GetLineByNumber(line);
				return _textArea.Document.GetText(lineByNumber.Offset, lineByNumber.Length);
			}
		}

		public override TextSelection Selection
		{
			get
			{
				if (_textArea == null)
				{
					return new TextSelection(0, 0);
				}
				return new TextSelection(_textArea.Caret.Position.Column, _textArea.Caret.Position.Column + _textArea.Selection.Length);
			}
			set
			{
				Selection selection = _textArea.Selection;
				_textArea.Selection = selection.StartSelectionOrSetEndpoint(new TextViewPosition(selection.StartPosition.Line, value.Start), new TextViewPosition(selection.StartPosition.Line, value.End));
			}
		}

		public void SetTextArea(TextArea textArea)
		{
			if (_textArea != null)
			{
				_textArea.Caret.PositionChanged -= Caret_PositionChanged;
			}
			_textArea = textArea;
			if (_textArea != null)
			{
				_textArea.Caret.PositionChanged += Caret_PositionChanged;
			}
			RaiseTextViewVisualChanged();
			RaiseCursorRectangleChanged();
			RaiseSurroundingTextChanged();
		}

		private void Caret_PositionChanged(object sender, EventArgs e)
		{
			RaiseCursorRectangleChanged();
			RaiseSurroundingTextChanged();
			RaiseSelectionChanged();
		}

		public override void SetPreeditText(string text)
		{
		}
	}

	private const int AdditionalVerticalScrollAmount = 2;

	private ILogicalScrollable _logicalScrollable;

	private readonly TextAreaTextInputMethodClient _imClient = new TextAreaTextInputMethodClient();

	public static readonly DirectProperty<TextArea, Vector> OffsetProperty;

	private ITextAreaInputHandler _activeInputHandler;

	private bool _isChangingInputHandler;

	public static readonly StyledProperty<TextDocument> DocumentProperty;

	public static readonly StyledProperty<TextEditorOptions> OptionsProperty;

	internal readonly Selection EmptySelection;

	private Selection _selection;

	public static readonly StyledProperty<IBrush> SelectionBrushProperty;

	public static readonly StyledProperty<IBrush> SelectionForegroundProperty;

	public static readonly StyledProperty<Pen> SelectionBorderProperty;

	public static readonly StyledProperty<double> SelectionCornerRadiusProperty;

	private bool _ensureSelectionValidRequested;

	private int _allowCaretOutsideSelection;

	public static readonly DirectProperty<TextArea, ObservableCollection<Control>> LeftMarginsProperty;

	private IReadOnlySectionProvider _readOnlySectionProvider = NoReadOnlySections.Instance;

	public static readonly StyledProperty<bool> RightClickMovesCaretProperty;

	public static readonly StyledProperty<IIndentationStrategy> IndentationStrategyProperty;

	private bool _isMouseCursorHidden;

	public static readonly StyledProperty<bool> OverstrikeModeProperty;

	public TextAreaDefaultInputHandler DefaultInputHandler { get; }

	public ITextAreaInputHandler ActiveInputHandler
	{
		get
		{
			return _activeInputHandler;
		}
		set
		{
			if (value != null && value.TextArea != this)
			{
				throw new ArgumentException("The input handler was created for a different text area than this one.");
			}
			if (_isChangingInputHandler)
			{
				throw new InvalidOperationException("Cannot set ActiveInputHandler recursively");
			}
			if (_activeInputHandler != value)
			{
				_isChangingInputHandler = true;
				try
				{
					PopStackedInputHandler(StackedInputHandlers.LastOrDefault());
					_activeInputHandler?.Detach();
					_activeInputHandler = value;
					value?.Attach();
				}
				finally
				{
					_isChangingInputHandler = false;
				}
				this.ActiveInputHandlerChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public ImmutableStack<TextAreaStackedInputHandler> StackedInputHandlers { get; private set; } = ImmutableStack<TextAreaStackedInputHandler>.Empty;

	public TextDocument Document
	{
		get
		{
			return GetValue(DocumentProperty);
		}
		set
		{
			SetValue(DocumentProperty, value);
		}
	}

	public bool IsReadOnly => ReadOnlySectionProvider == ReadOnlySectionDocument.Instance;

	public TextEditorOptions Options
	{
		get
		{
			return GetValue(OptionsProperty);
		}
		set
		{
			SetValue(OptionsProperty, value);
		}
	}

	public TextView TextView { get; }

	public Selection Selection
	{
		get
		{
			return _selection;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.TextArea != this)
			{
				throw new ArgumentException("Cannot use a Selection instance that belongs to another text area.");
			}
			if (object.Equals(_selection, value))
			{
				return;
			}
			if (TextView != null)
			{
				ISegment surroundingSegment = _selection.SurroundingSegment;
				ISegment surroundingSegment2 = value.SurroundingSegment;
				if (!Selection.EnableVirtualSpace && _selection is SimpleSelection && value is SimpleSelection && surroundingSegment != null && surroundingSegment2 != null)
				{
					int offset = surroundingSegment.Offset;
					int offset2 = surroundingSegment2.Offset;
					if (offset != offset2)
					{
						TextView.Redraw(Math.Min(offset, offset2), Math.Abs(offset - offset2));
					}
					int endOffset = surroundingSegment.EndOffset;
					int endOffset2 = surroundingSegment2.EndOffset;
					if (endOffset != endOffset2)
					{
						TextView.Redraw(Math.Min(endOffset, endOffset2), Math.Abs(endOffset - endOffset2));
					}
				}
				else
				{
					TextView.Redraw(surroundingSegment);
					TextView.Redraw(surroundingSegment2);
				}
			}
			_selection = value;
			this.SelectionChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public IBrush SelectionBrush
	{
		get
		{
			return GetValue(SelectionBrushProperty);
		}
		set
		{
			SetValue(SelectionBrushProperty, value);
		}
	}

	public IBrush SelectionForeground
	{
		get
		{
			return GetValue(SelectionForegroundProperty);
		}
		set
		{
			SetValue(SelectionForegroundProperty, value);
		}
	}

	public Pen SelectionBorder
	{
		get
		{
			return GetValue(SelectionBorderProperty);
		}
		set
		{
			SetValue(SelectionBorderProperty, value);
		}
	}

	public double SelectionCornerRadius
	{
		get
		{
			return GetValue(SelectionCornerRadiusProperty);
		}
		set
		{
			SetValue(SelectionCornerRadiusProperty, value);
		}
	}

	public Caret Caret { get; }

	public ObservableCollection<Control> LeftMargins { get; } = new ObservableCollection<Control>();

	public IReadOnlySectionProvider ReadOnlySectionProvider
	{
		get
		{
			return _readOnlySectionProvider;
		}
		set
		{
			_readOnlySectionProvider = value ?? throw new ArgumentNullException("value");
		}
	}

	public bool RightClickMovesCaret
	{
		get
		{
			return GetValue(RightClickMovesCaretProperty);
		}
		set
		{
			SetValue(RightClickMovesCaretProperty, value);
		}
	}

	public IIndentationStrategy IndentationStrategy
	{
		get
		{
			return GetValue(IndentationStrategyProperty);
		}
		set
		{
			SetValue(IndentationStrategyProperty, value);
		}
	}

	public bool OverstrikeMode
	{
		get
		{
			return GetValue(OverstrikeModeProperty);
		}
		set
		{
			SetValue(OverstrikeModeProperty, value);
		}
	}

	public IList<RoutedCommandBinding> CommandBindings { get; } = new List<RoutedCommandBinding>();

	bool ILogicalScrollable.IsLogicalScrollEnabled => _logicalScrollable?.IsLogicalScrollEnabled ?? false;

	Size ILogicalScrollable.ScrollSize => _logicalScrollable?.ScrollSize ?? default(Size);

	Size ILogicalScrollable.PageScrollSize => _logicalScrollable?.PageScrollSize ?? default(Size);

	Size IScrollable.Extent => _logicalScrollable?.Extent ?? default(Size);

	Vector IScrollable.Offset
	{
		get
		{
			return _logicalScrollable?.Offset ?? default(Vector);
		}
		set
		{
			if (_logicalScrollable != null)
			{
				_logicalScrollable.Offset = value;
			}
		}
	}

	Size IScrollable.Viewport => _logicalScrollable?.Viewport ?? default(Size);

	bool ILogicalScrollable.CanHorizontallyScroll
	{
		get
		{
			return _logicalScrollable?.CanHorizontallyScroll ?? false;
		}
		set
		{
			if (_logicalScrollable != null)
			{
				_logicalScrollable.CanHorizontallyScroll = value;
			}
		}
	}

	bool ILogicalScrollable.CanVerticallyScroll
	{
		get
		{
			return _logicalScrollable?.CanVerticallyScroll ?? false;
		}
		set
		{
			if (_logicalScrollable != null)
			{
				_logicalScrollable.CanVerticallyScroll = value;
			}
		}
	}

	public event EventHandler ActiveInputHandlerChanged;

	public event EventHandler<DocumentChangedEventArgs> DocumentChanged;

	public event PropertyChangedEventHandler OptionChanged;

	public event EventHandler SelectionChanged;

	public event EventHandler<TextInputEventArgs> TextEntering;

	public event EventHandler<TextInputEventArgs> TextEntered;

	public event EventHandler<TextEventArgs> TextCopied;

	event EventHandler ILogicalScrollable.ScrollInvalidated
	{
		add
		{
			if (_logicalScrollable != null)
			{
				_logicalScrollable.ScrollInvalidated += value;
			}
		}
		remove
		{
			if (_logicalScrollable != null)
			{
				_logicalScrollable.ScrollInvalidated -= value;
			}
		}
	}

	static TextArea()
	{
		OffsetProperty = AvaloniaProperty.RegisterDirect("Offset", (TextArea o) => ((IScrollable)o).Offset, delegate(TextArea o, Vector v)
		{
			((IScrollable)o).Offset = v;
		});
		DocumentProperty = TextView.DocumentProperty.AddOwner<TextArea>();
		OptionsProperty = TextView.OptionsProperty.AddOwner<TextArea>();
		SelectionBrushProperty = AvaloniaProperty.Register<TextArea, IBrush>("SelectionBrush");
		SelectionForegroundProperty = AvaloniaProperty.Register<TextArea, IBrush>("SelectionForeground");
		SelectionBorderProperty = AvaloniaProperty.Register<TextArea, Pen>("SelectionBorder");
		SelectionCornerRadiusProperty = AvaloniaProperty.Register<TextArea, double>("SelectionCornerRadius", 3.0);
		LeftMarginsProperty = AvaloniaProperty.RegisterDirect("LeftMargins", (TextArea c) => c.LeftMargins);
		RightClickMovesCaretProperty = AvaloniaProperty.Register<TextArea, bool>("RightClickMovesCaret", defaultValue: false);
		IndentationStrategyProperty = AvaloniaProperty.Register<TextArea, IIndentationStrategy>("IndentationStrategy", new DefaultIndentationStrategy());
		OverstrikeModeProperty = AvaloniaProperty.Register<TextArea, bool>("OverstrikeMode", defaultValue: false);
		KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<TextArea>(KeyboardNavigationMode.None);
		InputElement.FocusableProperty.OverrideDefaultValue<TextArea>(defaultValue: true);
		DocumentProperty.Changed.Subscribe(OnDocumentChanged);
		OptionsProperty.Changed.Subscribe(OnOptionsChanged);
		Layoutable.AffectsArrange<TextArea>(new AvaloniaProperty[1] { OffsetProperty });
		Visual.AffectsRender<TextArea>(new AvaloniaProperty[1] { OffsetProperty });
		InputElement.TextInputMethodClientRequestedEvent.AddClassHandler(delegate(TextArea ta, TextInputMethodClientRequestedEventArgs e)
		{
			if (!ta.IsReadOnly)
			{
				e.Client = ta._imClient;
			}
		});
	}

	public TextArea()
		: this(new TextView())
	{
		AddHandler(InputElement.KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
		AddHandler(InputElement.KeyUpEvent, OnPreviewKeyUp, RoutingStrategies.Tunnel);
	}

	protected TextArea(TextView textView)
	{
		TextView = textView ?? throw new ArgumentNullException("textView");
		_logicalScrollable = textView;
		Options = textView.Options;
		_selection = (EmptySelection = new EmptySelection(this));
		textView.Services.AddService(this);
		textView.LineTransformers.Add(new SelectionColorizer(this));
		textView.InsertLayer(new SelectionLayer(this), KnownLayer.Selection, LayerInsertionPosition.Replace);
		Caret = new Caret(this);
		Caret.PositionChanged += delegate
		{
			RequestSelectionValidation();
		};
		Caret.PositionChanged += CaretPositionChanged;
		AttachTypingEvents();
		LeftMargins.CollectionChanged += LeftMargins_CollectionChanged;
		DefaultInputHandler = new TextAreaDefaultInputHandler(this);
		ActiveInputHandler = DefaultInputHandler;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		if (e.NameScope.Find("PART_CP") is ContentPresenter contentPresenter)
		{
			contentPresenter.Content = TextView;
		}
	}

	internal void AddChild(Visual visual)
	{
		base.VisualChildren.Add(visual);
		InvalidateArrange();
	}

	internal void RemoveChild(Visual visual)
	{
		base.VisualChildren.Remove(visual);
	}

	public void PushStackedInputHandler(TextAreaStackedInputHandler inputHandler)
	{
		if (inputHandler == null)
		{
			throw new ArgumentNullException("inputHandler");
		}
		StackedInputHandlers = StackedInputHandlers.Push(inputHandler);
		inputHandler.Attach();
	}

	public void PopStackedInputHandler(TextAreaStackedInputHandler inputHandler)
	{
		if (StackedInputHandlers.Any((TextAreaStackedInputHandler i) => i == inputHandler))
		{
			ITextAreaInputHandler textAreaInputHandler;
			do
			{
				textAreaInputHandler = StackedInputHandlers.Peek();
				StackedInputHandlers = StackedInputHandlers.Pop();
				textAreaInputHandler.Detach();
			}
			while (textAreaInputHandler != inputHandler);
		}
	}

	private static void OnDocumentChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextArea)?.OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
	}

	private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.RemoveHandler(oldValue, OnDocumentChanging);
			WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.RemoveHandler(oldValue, OnDocumentChanged);
			WeakEventManagerBase<TextDocumentWeakEventManager.UpdateStarted, TextDocument, EventHandler, EventArgs>.RemoveHandler(oldValue, OnUpdateStarted);
			WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument, EventHandler, EventArgs>.RemoveHandler(oldValue, OnUpdateFinished);
		}
		TextView.Document = newValue;
		if (newValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.AddHandler(newValue, OnDocumentChanging);
			WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.AddHandler(newValue, OnDocumentChanged);
			WeakEventManagerBase<TextDocumentWeakEventManager.UpdateStarted, TextDocument, EventHandler, EventArgs>.AddHandler(newValue, OnUpdateStarted);
			WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument, EventHandler, EventArgs>.AddHandler(newValue, OnUpdateFinished);
			InvalidateArrange();
		}
		Caret.Location = new TextLocation(1, 1);
		ClearSelection();
		this.DocumentChanged?.Invoke(this, new DocumentChangedEventArgs(oldValue, newValue));
	}

	private void OnOptionChanged(object sender, PropertyChangedEventArgs e)
	{
		OnOptionChanged(e);
	}

	protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
	{
		this.OptionChanged?.Invoke(this, e);
	}

	private static void OnOptionsChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextArea)?.OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
	}

	private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.RemoveHandler(oldValue, OnOptionChanged);
		}
		TextView.Options = newValue;
		if (newValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.AddHandler(newValue, OnOptionChanged);
		}
		OnOptionChanged(new PropertyChangedEventArgs(null));
	}

	private void OnDocumentChanging(object sender, DocumentChangeEventArgs e)
	{
		Caret.OnDocumentChanging();
	}

	private void OnDocumentChanged(object sender, DocumentChangeEventArgs e)
	{
		Caret.OnDocumentChanged(e);
		Selection = _selection.UpdateOnDocumentChange(e);
	}

	private void OnUpdateStarted(object sender, EventArgs e)
	{
		Document.UndoStack.PushOptional(new RestoreCaretAndSelectionUndoAction(this));
	}

	private void OnUpdateFinished(object sender, EventArgs e)
	{
		Caret.OnDocumentUpdateFinished();
	}

	public void ClearSelection()
	{
		Selection = EmptySelection;
	}

	private void RequestSelectionValidation()
	{
		if (!_ensureSelectionValidRequested && _allowCaretOutsideSelection == 0)
		{
			_ensureSelectionValidRequested = true;
			Dispatcher.UIThread.Post(EnsureSelectionValid);
		}
	}

	private void EnsureSelectionValid()
	{
		_ensureSelectionValidRequested = false;
		if (_allowCaretOutsideSelection == 0 && !_selection.IsEmpty && !_selection.Contains(Caret.Offset))
		{
			ClearSelection();
		}
	}

	public IDisposable AllowCaretOutsideSelection()
	{
		VerifyAccess();
		_allowCaretOutsideSelection++;
		return new CallbackOnDispose(delegate
		{
			VerifyAccess();
			_allowCaretOutsideSelection--;
			RequestSelectionValidation();
		});
	}

	public void ScrollToLine(int line)
	{
		int num = (int)((IScrollable)this).Viewport.Height;
		if (num < Document.LineCount)
		{
			ScrollToLine(line, 2, num / 2);
		}
	}

	public void ScrollToLine(int line, int linesEitherSide)
	{
		ScrollToLine(line, linesEitherSide, linesEitherSide);
	}

	public void ScrollToLine(int line, int linesAbove, int linesBelow)
	{
		int num = line - linesAbove;
		if (num < 0)
		{
			num = 0;
		}
		this.BringIntoView(new Rect(1.0, num, 0.0, 1.0));
		num = line + linesBelow;
		if (num >= 0)
		{
			this.BringIntoView(new Rect(1.0, num, 0.0, 1.0));
		}
	}

	private void CaretPositionChanged(object sender, EventArgs e)
	{
		if (TextView != null)
		{
			TextView.HighlightedLine = Caret.Line;
			ScrollToLine(Caret.Line, 2);
			Dispatcher.UIThread.InvokeAsync(delegate
			{
				((ILogicalScrollable)this).RaiseScrollInvalidated(EventArgs.Empty);
			});
		}
	}

	private void LeftMargins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems != null)
		{
			foreach (ITextViewConnect item in e.OldItems.OfType<ITextViewConnect>())
			{
				item.RemoveFromTextView(TextView);
			}
		}
		if (e.NewItems == null)
		{
			return;
		}
		foreach (ITextViewConnect item2 in e.NewItems.OfType<ITextViewConnect>())
		{
			item2.AddToTextView(TextView);
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		Focus();
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		Caret.Show();
		_imClient.SetTextArea(this);
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		Caret.Hide();
		_imClient.SetTextArea(null);
	}

	protected virtual void OnTextEntering(TextInputEventArgs e)
	{
		this.TextEntering?.Invoke(this, e);
	}

	protected virtual void OnTextEntered(TextInputEventArgs e)
	{
		this.TextEntered?.Invoke(this, e);
	}

	protected override void OnTextInput(TextInputEventArgs e)
	{
		base.OnTextInput(e);
		if (!e.Handled && Document != null && !string.IsNullOrEmpty(e.Text) && !(e.Text == "\u001b") && !(e.Text == "\b") && !(e.Text == "\u007f"))
		{
			HideMouseCursor();
			PerformTextInput(e);
			e.Handled = true;
		}
	}

	public void PerformTextInput(string text)
	{
		TextInputEventArgs e = new TextInputEventArgs
		{
			Text = text,
			RoutedEvent = InputElement.TextInputEvent
		};
		PerformTextInput(e);
	}

	public void PerformTextInput(TextInputEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		OnTextEntering(e);
		if (!e.Handled)
		{
			if (e.Text == "\n" || e.Text == "\r" || e.Text == "\r\n")
			{
				ReplaceSelectionWithNewLine();
			}
			else
			{
				ReplaceSelectionWithText(e.Text);
			}
			OnTextEntered(e);
			Caret.BringCaretToView();
		}
	}

	private void ReplaceSelectionWithNewLine()
	{
		string newLineFromDocument = TextUtilities.GetNewLineFromDocument(Document, Caret.Line);
		using (Document.RunUpdate())
		{
			ReplaceSelectionWithText(newLineFromDocument);
			if (IndentationStrategy != null)
			{
				DocumentLine lineByNumber = Document.GetLineByNumber(Caret.Line);
				ISegment[] deletableSegments = GetDeletableSegments(lineByNumber);
				if (deletableSegments.Length == 1 && deletableSegments[0].Offset == lineByNumber.Offset && deletableSegments[0].Length == lineByNumber.Length)
				{
					IndentationStrategy.IndentLine(Document, lineByNumber);
				}
			}
		}
	}

	internal void RemoveSelectedText()
	{
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		_selection.ReplaceSelectionWithText(string.Empty);
	}

	internal void ReplaceSelectionWithText(string newText)
	{
		if (newText == null)
		{
			throw new ArgumentNullException("newText");
		}
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		_selection.ReplaceSelectionWithText(newText);
	}

	internal ISegment[] GetDeletableSegments(ISegment segment)
	{
		ISegment[] array = (ReadOnlySectionProvider.GetDeletableSegments(segment) ?? throw new InvalidOperationException("ReadOnlySectionProvider.GetDeletableSegments returned null")).ToArray();
		int num = segment.Offset;
		ISegment[] array2 = array;
		foreach (ISegment obj in array2)
		{
			if (obj.Offset < num)
			{
				throw new InvalidOperationException("ReadOnlySectionProvider returned incorrect segments (outside of input segment / wrong order)");
			}
			num = obj.EndOffset;
		}
		if (num > segment.EndOffset)
		{
			throw new InvalidOperationException("ReadOnlySectionProvider returned incorrect segments (outside of input segment / wrong order)");
		}
		return array;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		TextView.InvalidateCursorIfPointerWithinTextView();
	}

	private void OnPreviewKeyDown(object sender, KeyEventArgs e)
	{
		ImmutableStack<TextAreaStackedInputHandler>.Enumerator enumerator = StackedInputHandlers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TextAreaStackedInputHandler current = enumerator.Current;
			if (!e.Handled)
			{
				current.OnPreviewKeyDown(e);
				continue;
			}
			break;
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		base.OnKeyUp(e);
		TextView.InvalidateCursorIfPointerWithinTextView();
	}

	private void OnPreviewKeyUp(object sender, KeyEventArgs e)
	{
		ImmutableStack<TextAreaStackedInputHandler>.Enumerator enumerator = StackedInputHandlers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TextAreaStackedInputHandler current = enumerator.Current;
			if (!e.Handled)
			{
				current.OnPreviewKeyUp(e);
				continue;
			}
			break;
		}
	}

	private void AttachTypingEvents()
	{
		base.PointerEntered += delegate
		{
			ShowMouseCursor();
		};
		base.PointerExited += delegate
		{
			ShowMouseCursor();
		};
	}

	private void ShowMouseCursor()
	{
		if (_isMouseCursorHidden)
		{
			_isMouseCursorHidden = false;
		}
	}

	private void HideMouseCursor()
	{
		if (Options.HideCursorWhileTyping && !_isMouseCursorHidden && base.IsPointerOver)
		{
			_isMouseCursorHidden = true;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == SelectionBrushProperty || change.Property == SelectionBorderProperty || change.Property == SelectionForegroundProperty || change.Property == SelectionCornerRadiusProperty)
		{
			TextView.Redraw();
		}
		else if (change.Property == OverstrikeModeProperty)
		{
			Caret.UpdateIfVisible();
		}
	}

	public virtual object GetService(Type serviceType)
	{
		return TextView.GetService(serviceType);
	}

	internal void OnTextCopied(TextEventArgs e)
	{
		this.TextCopied?.Invoke(this, e);
	}

	public bool BringIntoView(Control target, Rect targetRect)
	{
		return _logicalScrollable?.BringIntoView(target, targetRect) ?? false;
	}

	Control ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control from)
	{
		return _logicalScrollable?.GetControlInDirection(direction, from);
	}

	public void RaiseScrollInvalidated(EventArgs e)
	{
		_logicalScrollable?.RaiseScrollInvalidated(e);
	}
}
