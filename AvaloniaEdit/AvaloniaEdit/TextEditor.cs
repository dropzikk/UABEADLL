using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Search;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit;

public class TextEditor : TemplatedControl, ITextEditorComponent, IServiceProvider
{
	public static readonly StyledProperty<TextDocument> DocumentProperty;

	public static readonly StyledProperty<TextEditorOptions> OptionsProperty;

	private readonly TextArea textArea;

	private SearchPanel searchPanel;

	private bool wasSearchPanelOpened;

	public static readonly StyledProperty<IHighlightingDefinition> SyntaxHighlightingProperty;

	private IVisualLineTransformer _colorizer;

	public static readonly StyledProperty<bool> WordWrapProperty;

	public static readonly StyledProperty<bool> IsReadOnlyProperty;

	public static readonly StyledProperty<bool> IsModifiedProperty;

	public static readonly StyledProperty<bool> ShowLineNumbersProperty;

	public static readonly StyledProperty<IBrush> SearchResultsBrushProperty;

	public static readonly StyledProperty<IBrush> LineNumbersForegroundProperty;

	public static readonly StyledProperty<Encoding> EncodingProperty;

	public static readonly RoutedEvent<PointerEventArgs> PreviewPointerHoverEvent;

	public static readonly RoutedEvent<PointerEventArgs> PointerHoverEvent;

	public static readonly RoutedEvent<PointerEventArgs> PreviewPointerHoverStoppedEvent;

	public static readonly RoutedEvent<PointerEventArgs> PointerHoverStoppedEvent;

	public static readonly AttachedProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty;

	private ScrollBarVisibility _horizontalScrollBarVisibilityBck = ScrollBarVisibility.Auto;

	public static readonly AttachedProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty;

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

	public string Text
	{
		get
		{
			TextDocument document = Document;
			if (document == null)
			{
				return string.Empty;
			}
			return document.Text;
		}
		set
		{
			TextDocument document = GetDocument();
			document.Text = value ?? string.Empty;
			CaretOffset = 0;
			document.UndoStack.ClearAll();
		}
	}

	public TextArea TextArea => textArea;

	public SearchPanel SearchPanel => searchPanel;

	internal ScrollViewer ScrollViewer { get; private set; }

	public IHighlightingDefinition SyntaxHighlighting
	{
		get
		{
			return GetValue(SyntaxHighlightingProperty);
		}
		set
		{
			SetValue(SyntaxHighlightingProperty, value);
		}
	}

	public bool WordWrap
	{
		get
		{
			return GetValue(WordWrapProperty);
		}
		set
		{
			SetValue(WordWrapProperty, value);
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return GetValue(IsReadOnlyProperty);
		}
		set
		{
			SetValue(IsReadOnlyProperty, value);
		}
	}

	public bool IsModified
	{
		get
		{
			return GetValue(IsModifiedProperty);
		}
		set
		{
			SetValue(IsModifiedProperty, value);
		}
	}

	public bool ShowLineNumbers
	{
		get
		{
			return GetValue(ShowLineNumbersProperty);
		}
		set
		{
			SetValue(ShowLineNumbersProperty, value);
		}
	}

	public IBrush SearchResultsBrush
	{
		get
		{
			return GetValue(SearchResultsBrushProperty);
		}
		set
		{
			SetValue(SearchResultsBrushProperty, value);
		}
	}

	public IBrush LineNumbersForeground
	{
		get
		{
			return GetValue(LineNumbersForegroundProperty);
		}
		set
		{
			SetValue(LineNumbersForegroundProperty, value);
		}
	}

	public bool CanRedo => ApplicationCommands.Redo.CanExecute(null, TextArea);

	public bool CanUndo => ApplicationCommands.Undo.CanExecute(null, TextArea);

	public bool CanCopy => ApplicationCommands.Copy.CanExecute(null, TextArea);

	public bool CanCut => ApplicationCommands.Cut.CanExecute(null, TextArea);

	public bool CanPaste => ApplicationCommands.Paste.CanExecute(null, TextArea);

	public bool CanDelete => ApplicationCommands.Delete.CanExecute(null, TextArea);

	public bool CanSelectAll => ApplicationCommands.SelectAll.CanExecute(null, TextArea);

	public bool CanSearch => searchPanel != null;

	public double ExtentHeight => ScrollViewer?.Extent.Height ?? 0.0;

	public double ExtentWidth => ScrollViewer?.Extent.Width ?? 0.0;

	public double ViewportHeight => ScrollViewer?.Viewport.Height ?? 0.0;

	public double ViewportWidth => ScrollViewer?.Viewport.Width ?? 0.0;

	public double VerticalOffset => ScrollViewer?.Offset.Y ?? 0.0;

	public double HorizontalOffset => ScrollViewer?.Offset.X ?? 0.0;

	public string SelectedText
	{
		get
		{
			if (textArea.Document != null && !textArea.Selection.IsEmpty)
			{
				return textArea.Document.GetText(textArea.Selection.SurroundingSegment);
			}
			return string.Empty;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			TextArea textArea = TextArea;
			if (textArea.Document != null)
			{
				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;
				textArea.Document.Replace(selectionStart, selectionLength, value);
				textArea.Selection = Selection.Create(textArea, selectionStart, selectionStart + value.Length);
			}
		}
	}

	public int CaretOffset
	{
		get
		{
			return textArea.Caret.Offset;
		}
		set
		{
			textArea.Caret.Offset = value;
		}
	}

	public int SelectionStart
	{
		get
		{
			if (textArea.Selection.IsEmpty)
			{
				return textArea.Caret.Offset;
			}
			return textArea.Selection.SurroundingSegment.Offset;
		}
		set
		{
			Select(value, SelectionLength);
		}
	}

	public int SelectionLength
	{
		get
		{
			if (!textArea.Selection.IsEmpty)
			{
				return textArea.Selection.SurroundingSegment.Length;
			}
			return 0;
		}
		set
		{
			Select(SelectionStart, value);
		}
	}

	public int LineCount => Document?.LineCount ?? 1;

	public Encoding Encoding
	{
		get
		{
			return GetValue(EncodingProperty);
		}
		set
		{
			SetValue(EncodingProperty, value);
		}
	}

	public ScrollBarVisibility HorizontalScrollBarVisibility
	{
		get
		{
			return GetValue(HorizontalScrollBarVisibilityProperty);
		}
		set
		{
			SetValue(HorizontalScrollBarVisibilityProperty, value);
		}
	}

	public ScrollBarVisibility VerticalScrollBarVisibility
	{
		get
		{
			return GetValue(VerticalScrollBarVisibilityProperty);
		}
		set
		{
			SetValue(VerticalScrollBarVisibilityProperty, value);
		}
	}

	public event EventHandler<DocumentChangedEventArgs> DocumentChanged;

	public event PropertyChangedEventHandler OptionChanged;

	public event EventHandler TextChanged;

	public event EventHandler<PointerEventArgs> PreviewPointerHover
	{
		add
		{
			AddHandler(PreviewPointerHoverEvent, value);
		}
		remove
		{
			RemoveHandler(PreviewPointerHoverEvent, value);
		}
	}

	public event EventHandler<PointerEventArgs> PointerHover
	{
		add
		{
			AddHandler(PointerHoverEvent, value);
		}
		remove
		{
			RemoveHandler(PointerHoverEvent, value);
		}
	}

	public event EventHandler<PointerEventArgs> PreviewPointerHoverStopped
	{
		add
		{
			AddHandler(PreviewPointerHoverStoppedEvent, value);
		}
		remove
		{
			RemoveHandler(PreviewPointerHoverStoppedEvent, value);
		}
	}

	public event EventHandler<PointerEventArgs> PointerHoverStopped
	{
		add
		{
			AddHandler(PointerHoverStoppedEvent, value);
		}
		remove
		{
			RemoveHandler(PointerHoverStoppedEvent, value);
		}
	}

	static TextEditor()
	{
		DocumentProperty = TextView.DocumentProperty.AddOwner<TextEditor>();
		OptionsProperty = TextView.OptionsProperty.AddOwner<TextEditor>();
		SyntaxHighlightingProperty = AvaloniaProperty.Register<TextEditor, IHighlightingDefinition>("SyntaxHighlighting");
		WordWrapProperty = AvaloniaProperty.Register<TextEditor, bool>("WordWrap", defaultValue: false);
		IsReadOnlyProperty = AvaloniaProperty.Register<TextEditor, bool>("IsReadOnly", defaultValue: false);
		IsModifiedProperty = AvaloniaProperty.Register<TextEditor, bool>("IsModified", defaultValue: false);
		ShowLineNumbersProperty = AvaloniaProperty.Register<TextEditor, bool>("ShowLineNumbers", defaultValue: false);
		SearchResultsBrushProperty = AvaloniaProperty.Register<TextEditor, IBrush>("SearchResultsBrush", new SolidColorBrush(Color.FromRgb(81, 92, 106)));
		LineNumbersForegroundProperty = AvaloniaProperty.Register<TextEditor, IBrush>("LineNumbersForeground", Brushes.Gray);
		EncodingProperty = AvaloniaProperty.Register<TextEditor, Encoding>("Encoding");
		PreviewPointerHoverEvent = TextView.PreviewPointerHoverEvent;
		PointerHoverEvent = TextView.PointerHoverEvent;
		PreviewPointerHoverStoppedEvent = TextView.PreviewPointerHoverStoppedEvent;
		PointerHoverStoppedEvent = TextView.PointerHoverStoppedEvent;
		HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner<TextEditor>();
		VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner<TextEditor>();
		InputElement.FocusableProperty.OverrideDefaultValue<TextEditor>(defaultValue: true);
		HorizontalScrollBarVisibilityProperty.OverrideDefaultValue<TextEditor>(ScrollBarVisibility.Auto);
		VerticalScrollBarVisibilityProperty.OverrideDefaultValue<TextEditor>(ScrollBarVisibility.Auto);
		OptionsProperty.Changed.Subscribe(OnOptionsChanged);
		DocumentProperty.Changed.Subscribe(OnDocumentChanged);
		SyntaxHighlightingProperty.Changed.Subscribe(OnSyntaxHighlightingChanged);
		IsReadOnlyProperty.Changed.Subscribe(OnIsReadOnlyChanged);
		IsModifiedProperty.Changed.Subscribe(OnIsModifiedChanged);
		ShowLineNumbersProperty.Changed.Subscribe(OnShowLineNumbersChanged);
		LineNumbersForegroundProperty.Changed.Subscribe(OnLineNumbersForegroundChanged);
		TemplatedControl.FontFamilyProperty.Changed.Subscribe(OnFontFamilyPropertyChanged);
		TemplatedControl.FontSizeProperty.Changed.Subscribe(OnFontSizePropertyChanged);
		SearchResultsBrushProperty.Changed.Subscribe(SearchResultsBrushChangedCallback);
	}

	public TextEditor()
		: this(new TextArea())
	{
	}

	protected TextEditor(TextArea textArea)
		: this(textArea, new TextDocument())
	{
	}

	protected TextEditor(TextArea textArea, TextDocument document)
	{
		this.textArea = textArea ?? throw new ArgumentNullException("textArea");
		textArea.TextView.Services.AddService(this);
		SetValue(OptionsProperty, textArea.Options);
		SetValue(DocumentProperty, document);
		textArea[!TemplatedControl.BackgroundProperty] = base[!TemplatedControl.BackgroundProperty];
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		TextArea.Focus();
		e.Handled = true;
	}

	protected virtual void OnDocumentChanged(DocumentChangedEventArgs e)
	{
		this.DocumentChanged?.Invoke(this, e);
	}

	private static void OnDocumentChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextEditor)?.OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
	}

	private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument, EventHandler, EventArgs>.RemoveHandler(oldValue, OnTextChanged);
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.RemoveHandler(oldValue.UndoStack, OnUndoStackPropertyChangedHandler);
		}
		TextArea.Document = newValue;
		if (newValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument, EventHandler, EventArgs>.AddHandler(newValue, OnTextChanged);
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.AddHandler(newValue.UndoStack, OnUndoStackPropertyChangedHandler);
		}
		OnDocumentChanged(new DocumentChangedEventArgs(oldValue, newValue));
		OnTextChanged(EventArgs.Empty);
	}

	protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
	{
		this.OptionChanged?.Invoke(this, e);
	}

	private static void OnOptionsChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextEditor)?.OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
	}

	private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.RemoveHandler(oldValue, OnPropertyChangedHandler);
		}
		TextArea.Options = newValue;
		if (newValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.AddHandler(newValue, OnPropertyChangedHandler);
		}
		OnOptionChanged(new PropertyChangedEventArgs(null));
	}

	private void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
	{
		OnOptionChanged(e);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == WordWrapProperty)
		{
			if (WordWrap)
			{
				_horizontalScrollBarVisibilityBck = HorizontalScrollBarVisibility;
				HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			}
			else
			{
				HorizontalScrollBarVisibility = _horizontalScrollBarVisibilityBck;
			}
		}
	}

	private void OnUndoStackPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "IsOriginalFile")
		{
			HandleIsOriginalChanged(e);
		}
	}

	private void OnTextChanged(object sender, EventArgs e)
	{
		OnTextChanged(e);
	}

	private TextDocument GetDocument()
	{
		return Document ?? throw ThrowUtil.NoDocumentAssigned();
	}

	protected virtual void OnTextChanged(EventArgs e)
	{
		this.TextChanged?.Invoke(this, e);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		ScrollViewer = (ScrollViewer)e.NameScope.Find("PART_ScrollViewer");
		ScrollViewer.Content = TextArea;
		searchPanel = SearchPanel.Install(this);
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnAttachedToLogicalTree(e);
		if (searchPanel != null && wasSearchPanelOpened)
		{
			searchPanel.Open();
		}
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromLogicalTree(e);
		if (searchPanel != null)
		{
			wasSearchPanelOpened = searchPanel.IsOpened;
			if (searchPanel.IsOpened)
			{
				searchPanel.Close();
			}
		}
	}

	private static void OnSyntaxHighlightingChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextEditor)?.OnSyntaxHighlightingChanged(e.NewValue as IHighlightingDefinition);
	}

	private void OnSyntaxHighlightingChanged(IHighlightingDefinition newValue)
	{
		if (_colorizer != null)
		{
			textArea.TextView.LineTransformers.Remove(_colorizer);
			_colorizer = null;
		}
		if (newValue != null)
		{
			_colorizer = CreateColorizer(newValue);
			if (_colorizer != null)
			{
				textArea.TextView.LineTransformers.Insert(0, _colorizer);
			}
		}
	}

	protected virtual IVisualLineTransformer CreateColorizer(IHighlightingDefinition highlightingDefinition)
	{
		if (highlightingDefinition == null)
		{
			throw new ArgumentNullException("highlightingDefinition");
		}
		return new HighlightingColorizer(highlightingDefinition);
	}

	private static void OnIsReadOnlyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is TextEditor textEditor)
		{
			bool newValue = e.GetNewValue<bool>();
			TextArea obj = textEditor.TextArea;
			IReadOnlySectionProvider readOnlySectionProvider;
			if (!newValue)
			{
				IReadOnlySectionProvider instance = NoReadOnlySections.Instance;
				readOnlySectionProvider = instance;
			}
			else
			{
				IReadOnlySectionProvider instance = ReadOnlySectionDocument.Instance;
				readOnlySectionProvider = instance;
			}
			obj.ReadOnlySectionProvider = readOnlySectionProvider;
			if (textEditor.SearchPanel != null)
			{
				textEditor.SearchPanel.IsReplaceMode = !newValue && textEditor.SearchPanel.IsReplaceMode;
			}
		}
	}

	private static void OnIsModifiedChanged(AvaloniaPropertyChangedEventArgs e)
	{
		TextDocument textDocument = (e.Sender as TextEditor)?.Document;
		if (textDocument == null)
		{
			return;
		}
		UndoStack undoStack = textDocument.UndoStack;
		if ((bool)e.NewValue)
		{
			if (undoStack.IsOriginalFile)
			{
				undoStack.DiscardOriginalFileMarker();
			}
		}
		else
		{
			undoStack.MarkAsOriginalFile();
		}
	}

	private void HandleIsOriginalChanged(PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "IsOriginalFile")
		{
			TextDocument document = Document;
			if (document != null)
			{
				SetValue((AvaloniaProperty)IsModifiedProperty, (object?)(!document.UndoStack.IsOriginalFile), BindingPriority.LocalValue);
			}
		}
	}

	private static void OnShowLineNumbersChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (!(e.Sender is TextEditor textEditor))
		{
			return;
		}
		ObservableCollection<Control> leftMargins = textEditor.TextArea.LeftMargins;
		if ((bool)e.NewValue)
		{
			LineNumberMargin lineNumberMargin = new LineNumberMargin();
			Line line = (Line)DottedLineMargin.Create();
			leftMargins.Insert(0, lineNumberMargin);
			leftMargins.Insert(1, line);
			Binding binding = new Binding("LineNumbersForeground")
			{
				Source = textEditor
			};
			line.Bind(Shape.StrokeProperty, binding);
			lineNumberMargin.Bind(TemplatedControl.ForegroundProperty, binding);
			return;
		}
		for (int i = 0; i < leftMargins.Count; i++)
		{
			if (leftMargins[i] is LineNumberMargin)
			{
				leftMargins.RemoveAt(i);
				if (i < leftMargins.Count && DottedLineMargin.IsDottedLineMargin(leftMargins[i]))
				{
					leftMargins.RemoveAt(i);
				}
				break;
			}
		}
	}

	private static void OnLineNumbersForegroundChanged(AvaloniaPropertyChangedEventArgs e)
	{
		((e.Sender as TextEditor)?.TextArea.LeftMargins.FirstOrDefault((Control margin) => margin is LineNumberMargin) as LineNumberMargin)?.SetValue(TemplatedControl.ForegroundProperty, e.NewValue);
	}

	private static void OnFontFamilyPropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextEditor)?.TextArea.TextView.SetValue(TemplatedControl.FontFamilyProperty, e.NewValue);
	}

	private static void OnFontSizePropertyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextEditor)?.TextArea.TextView.SetValue(TemplatedControl.FontSizeProperty, e.NewValue);
	}

	private static void SearchResultsBrushChangedCallback(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextEditor)?.SearchPanel?.SetSearchResultsBrush(e.GetNewValue<IBrush>());
	}

	public void AppendText(string textData)
	{
		TextDocument document = GetDocument();
		document.Insert(document.TextLength, textData);
	}

	public void BeginChange()
	{
		GetDocument().BeginUpdate();
	}

	public void Copy()
	{
		if (CanCopy)
		{
			ApplicationCommands.Copy.Execute(null, TextArea);
		}
	}

	public void Cut()
	{
		if (CanCut)
		{
			ApplicationCommands.Cut.Execute(null, TextArea);
		}
	}

	public IDisposable DeclareChangeBlock()
	{
		return GetDocument().RunUpdate();
	}

	public void Delete()
	{
		if (CanDelete)
		{
			ApplicationCommands.Delete.Execute(null, TextArea);
		}
	}

	public void EndChange()
	{
		GetDocument().EndUpdate();
	}

	public void LineDown()
	{
	}

	public void LineLeft()
	{
	}

	public void LineRight()
	{
	}

	public void LineUp()
	{
	}

	public void PageDown()
	{
	}

	public void PageUp()
	{
	}

	public void PageLeft()
	{
	}

	public void PageRight()
	{
	}

	public void Paste()
	{
		if (CanPaste)
		{
			ApplicationCommands.Paste.Execute(null, TextArea);
		}
	}

	public bool Redo()
	{
		if (CanRedo)
		{
			ApplicationCommands.Redo.Execute(null, TextArea);
			return true;
		}
		return false;
	}

	public void ScrollToEnd()
	{
		ApplyTemplate();
		ScrollViewer?.ScrollToEnd();
	}

	public void ScrollToHome()
	{
		ApplyTemplate();
		ScrollViewer?.ScrollToHome();
	}

	public void ScrollToHorizontalOffset(double offset)
	{
		ApplyTemplate();
	}

	public void ScrollToVerticalOffset(double offset)
	{
		ApplyTemplate();
	}

	public void SelectAll()
	{
		if (CanSelectAll)
		{
			ApplicationCommands.SelectAll.Execute(null, TextArea);
		}
	}

	public bool Undo()
	{
		if (CanUndo)
		{
			ApplicationCommands.Undo.Execute(null, TextArea);
			return true;
		}
		return false;
	}

	public void Select(int start, int length)
	{
		int num = Document?.TextLength ?? 0;
		if (start < 0 || start > num)
		{
			throw new ArgumentOutOfRangeException("start", start, "Value must be between 0 and " + num);
		}
		if (length < 0 || start + length > num)
		{
			throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (num - start));
		}
		TextArea.Selection = Selection.Create(TextArea, start, start + length);
		TextArea.Caret.Offset = start + length;
	}

	public void Clear()
	{
		Text = string.Empty;
	}

	public void Load(Stream stream)
	{
		using (StreamReader streamReader = FileReader.OpenStream(stream, Encoding ?? Encoding.UTF8))
		{
			Text = streamReader.ReadToEnd();
			SetValue((AvaloniaProperty)EncodingProperty, (object?)streamReader.CurrentEncoding, BindingPriority.LocalValue);
		}
		SetValue((AvaloniaProperty)IsModifiedProperty, (object?)false, BindingPriority.LocalValue);
	}

	public void Load(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
	}

	public void Save(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		Encoding encoding = Encoding;
		TextDocument document = Document;
		StreamWriter streamWriter = ((encoding != null) ? new StreamWriter(stream, encoding) : new StreamWriter(stream));
		document?.WriteTextTo(streamWriter);
		streamWriter.Flush();
		SetValue((AvaloniaProperty)IsModifiedProperty, (object?)false, BindingPriority.LocalValue);
	}

	public void Save(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
	}

	object IServiceProvider.GetService(Type serviceType)
	{
		return TextArea.GetService(serviceType);
	}

	public TextViewPosition? GetPositionFromPoint(Point point)
	{
		if (Document == null)
		{
			return null;
		}
		TextView textView = TextArea.TextView;
		Point value = this.TranslatePoint(point + new Point(textView.ScrollOffset.X, Math.Floor(textView.ScrollOffset.Y)), textView).Value;
		return textView.GetPosition(value);
	}

	public void ScrollToLine(int line)
	{
		ScrollTo(line, -1);
	}

	public void ScrollTo(int line, int column)
	{
		ScrollTo(line, column, VisualYPosition.LineMiddle, (ScrollViewer != null) ? (ScrollViewer.Viewport.Height / 2.0) : 0.0, 0.3);
	}

	public void ScrollTo(int line, int column, VisualYPosition yPositionMode, double referencedVerticalViewPortOffset, double minimumScrollFraction)
	{
		TextView textView = textArea.TextView;
		TextDocument document = textView.Document;
		if (ScrollViewer == null || document == null)
		{
			return;
		}
		if (line < 1)
		{
			line = 1;
		}
		if (line > document.LineCount)
		{
			line = document.LineCount;
		}
		if (!((ILogicalScrollable)textView).CanHorizontallyScroll)
		{
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(document.GetLineByNumber(line));
			for (double num = referencedVerticalViewPortOffset; num > 0.0; num -= orConstructVisualLine.Height)
			{
				DocumentLine previousLine = orConstructVisualLine.FirstDocumentLine.PreviousLine;
				if (previousLine == null)
				{
					break;
				}
				orConstructVisualLine = textView.GetOrConstructVisualLine(previousLine);
			}
		}
		Point visualPosition = textArea.TextView.GetVisualPosition(new TextViewPosition(line, Math.Max(1, column)), yPositionMode);
		double num2 = ScrollViewer.Offset.X;
		double num3 = ScrollViewer.Offset.Y;
		double num4 = visualPosition.Y - referencedVerticalViewPortOffset;
		if (Math.Abs(num4 - ScrollViewer.Offset.Y) > minimumScrollFraction * ScrollViewer.Viewport.Height)
		{
			num3 = Math.Max(0.0, num4);
		}
		if (column > 0)
		{
			if (visualPosition.X > ScrollViewer.Viewport.Width - 0.0)
			{
				if (Math.Abs(Math.Max(0.0, visualPosition.X - ScrollViewer.Viewport.Width / 2.0) - ScrollViewer.Offset.X) > minimumScrollFraction * ScrollViewer.Viewport.Width)
				{
					num2 = 0.0;
				}
			}
			else
			{
				num2 = 0.0;
			}
		}
		if (num2 != ScrollViewer.Offset.X || num3 != ScrollViewer.Offset.Y)
		{
			ScrollViewer.Offset = new Vector(num2, num3);
		}
	}
}
