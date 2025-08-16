using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

public class TextView : Control, ITextEditorComponent, IServiceProvider, ILogicalScrollable, IScrollable
{
	public sealed class LayerCollection : Collection<Control>
	{
		private readonly TextView _textView;

		public LayerCollection(TextView textView)
		{
			_textView = textView;
		}

		protected override void ClearItems()
		{
			foreach (Control item in base.Items)
			{
				_textView.VisualChildren.Remove(item);
			}
			base.ClearItems();
			_textView.LayersChanged();
		}

		protected override void InsertItem(int index, Control item)
		{
			base.InsertItem(index, item);
			_textView.VisualChildren.Insert(index, item);
			_textView.LayersChanged();
		}

		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			_textView.VisualChildren.RemoveAt(index);
			_textView.LayersChanged();
		}

		protected override void SetItem(int index, Control item)
		{
			_textView.VisualChildren.Remove(base.Items[index]);
			base.SetItem(index, item);
			_textView.VisualChildren.Add(item);
			_textView.LayersChanged();
		}
	}

	private EventHandler _scrollInvalidated;

	private readonly ColumnRulerRenderer _columnRulerRenderer;

	private readonly CurrentLineHighlightRenderer _currentLineHighlightRenderer;

	public static readonly StyledProperty<TextDocument> DocumentProperty;

	private TextDocument _document;

	private HeightTree _heightTree;

	public static readonly StyledProperty<TextEditorOptions> OptionsProperty;

	private readonly ObserveAddRemoveCollection<VisualLineElementGenerator> _elementGenerators;

	private readonly ObserveAddRemoveCollection<IVisualLineTransformer> _lineTransformers;

	private SingleCharacterElementGenerator _singleCharacterElementGenerator;

	private LinkElementGenerator _linkElementGenerator;

	private MailLinkElementGenerator _mailLinkElementGenerator;

	internal readonly TextLayer TextLayer;

	private readonly List<InlineObjectRun> _inlineObjects = new List<InlineObjectRun>();

	private readonly List<VisualLine> _visualLinesWithOutstandingInlineObjects = new List<VisualLine>();

	public static readonly StyledProperty<IBrush> NonPrintableCharacterBrushProperty;

	public static readonly StyledProperty<IBrush> LinkTextForegroundBrushProperty;

	public static readonly StyledProperty<IBrush> LinkTextBackgroundBrushProperty;

	public static readonly StyledProperty<bool> LinkTextUnderlineProperty;

	private List<VisualLine> _allVisualLines = new List<VisualLine>();

	private ReadOnlyCollection<VisualLine> _visibleVisualLines;

	private double _clippedPixelsOnTop;

	private List<VisualLine> _newVisualLines;

	private const double AdditionalHorizontalScrollAmount = 3.0;

	private Size _lastAvailableSize;

	private bool _inMeasure;

	private TextFormatter _formatter;

	internal TextViewCachedElements CachedElements;

	private readonly ObserveAddRemoveCollection<IBackgroundRenderer> _backgroundRenderers;

	private Size _scrollExtent;

	private Vector _scrollOffset;

	private Size _scrollViewport;

	private bool _canVerticallyScroll = true;

	private bool _canHorizontallyScroll = true;

	private bool _defaultTextMetricsValid;

	private double _wideSpaceWidth;

	private double _defaultLineHeight;

	private double _defaultBaseline;

	[ThreadStatic]
	private static bool _invalidCursor;

	public static readonly RoutedEvent<PointerEventArgs> PreviewPointerHoverEvent;

	public static readonly RoutedEvent<PointerEventArgs> PointerHoverEvent;

	public static readonly RoutedEvent<PointerEventArgs> PreviewPointerHoverStoppedEvent;

	public static readonly RoutedEvent<PointerEventArgs> PointerHoverStoppedEvent;

	private readonly PointerHoverLogic _hoverLogic;

	public static readonly StyledProperty<IPen> ColumnRulerPenProperty;

	public static readonly StyledProperty<IBrush> CurrentLineBackgroundProperty;

	public static readonly StyledProperty<IPen> CurrentLineBorderProperty;

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

	internal double FontSize
	{
		get
		{
			return GetValue(TemplatedControl.FontSizeProperty);
		}
		set
		{
			SetValue(TemplatedControl.FontSizeProperty, value);
		}
	}

	internal FontFamily FontFamily
	{
		get
		{
			return GetValue(TemplatedControl.FontFamilyProperty);
		}
		set
		{
			SetValue(TemplatedControl.FontFamilyProperty, value);
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

	public IList<VisualLineElementGenerator> ElementGenerators => _elementGenerators;

	public IList<IVisualLineTransformer> LineTransformers => _lineTransformers;

	public LayerCollection Layers { get; }

	public IBrush NonPrintableCharacterBrush
	{
		get
		{
			return GetValue(NonPrintableCharacterBrushProperty);
		}
		set
		{
			SetValue(NonPrintableCharacterBrushProperty, value);
		}
	}

	public IBrush LinkTextForegroundBrush
	{
		get
		{
			return GetValue(LinkTextForegroundBrushProperty);
		}
		set
		{
			SetValue(LinkTextForegroundBrushProperty, value);
		}
	}

	public IBrush LinkTextBackgroundBrush
	{
		get
		{
			return GetValue(LinkTextBackgroundBrushProperty);
		}
		set
		{
			SetValue(LinkTextBackgroundBrushProperty, value);
		}
	}

	public bool LinkTextUnderline
	{
		get
		{
			return GetValue(LinkTextUnderlineProperty);
		}
		set
		{
			SetValue(LinkTextUnderlineProperty, value);
		}
	}

	public ReadOnlyCollection<VisualLine> VisualLines
	{
		get
		{
			if (_visibleVisualLines == null)
			{
				throw new VisualLinesInvalidException();
			}
			return _visibleVisualLines;
		}
	}

	public bool VisualLinesValid => _visibleVisualLines != null;

	public IList<IBackgroundRenderer> BackgroundRenderers => _backgroundRenderers;

	public double HorizontalOffset => _scrollOffset.X;

	public double VerticalOffset => _scrollOffset.Y;

	public Vector ScrollOffset => _scrollOffset;

	public double WideSpaceWidth
	{
		get
		{
			CalculateDefaultTextMetrics();
			return _wideSpaceWidth;
		}
	}

	public double DefaultLineHeight
	{
		get
		{
			CalculateDefaultTextMetrics();
			return _defaultLineHeight;
		}
	}

	public double DefaultBaseline
	{
		get
		{
			CalculateDefaultTextMetrics();
			return _defaultBaseline;
		}
	}

	internal IServiceContainer Services { get; } = new ServiceContainer();

	public double DocumentHeight => _heightTree?.TotalHeight ?? 0.0;

	public IPen ColumnRulerPen
	{
		get
		{
			return GetValue(ColumnRulerPenProperty);
		}
		set
		{
			SetValue(ColumnRulerPenProperty, value);
		}
	}

	public IBrush CurrentLineBackground
	{
		get
		{
			return GetValue(CurrentLineBackgroundProperty);
		}
		set
		{
			SetValue(CurrentLineBackgroundProperty, value);
		}
	}

	public IPen CurrentLineBorder
	{
		get
		{
			return GetValue(CurrentLineBorderProperty);
		}
		set
		{
			SetValue(CurrentLineBorderProperty, value);
		}
	}

	public int HighlightedLine
	{
		get
		{
			return _currentLineHighlightRenderer.Line;
		}
		set
		{
			_currentLineHighlightRenderer.Line = value;
		}
	}

	public virtual double EmptyLineSelectionWidth => 1.0;

	bool ILogicalScrollable.CanHorizontallyScroll
	{
		get
		{
			return _canHorizontallyScroll;
		}
		set
		{
			if (_canHorizontallyScroll != value)
			{
				_canHorizontallyScroll = value;
				ClearVisualLines();
				InvalidateMeasure();
			}
		}
	}

	bool ILogicalScrollable.CanVerticallyScroll
	{
		get
		{
			return _canVerticallyScroll;
		}
		set
		{
			if (_canVerticallyScroll != value)
			{
				_canVerticallyScroll = value;
				ClearVisualLines();
				InvalidateMeasure();
			}
		}
	}

	bool ILogicalScrollable.IsLogicalScrollEnabled => true;

	Size ILogicalScrollable.ScrollSize => new Size(10.0, 50.0);

	Size ILogicalScrollable.PageScrollSize => new Size(10.0, 100.0);

	Size IScrollable.Extent => _scrollExtent;

	Vector IScrollable.Offset
	{
		get
		{
			return _scrollOffset;
		}
		set
		{
			value = new Vector(ValidateVisualOffset(value.X), ValidateVisualOffset(value.Y));
			bool flag = !_scrollOffset.X.IsClose(value.X);
			bool flag2 = !_scrollOffset.Y.IsClose(value.Y);
			if (flag || flag2)
			{
				SetScrollOffset(value);
				if (flag)
				{
					InvalidateVisual();
					TextLayer.InvalidateVisual();
				}
				InvalidateMeasure();
			}
		}
	}

	Size IScrollable.Viewport => _scrollViewport;

	public event EventHandler<DocumentChangedEventArgs> DocumentChanged;

	public event PropertyChangedEventHandler OptionChanged;

	public event EventHandler<VisualLineConstructionStartEventArgs> VisualLineConstructionStarting;

	public event EventHandler VisualLinesChanged;

	public event EventHandler ScrollOffsetChanged;

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

	event EventHandler ILogicalScrollable.ScrollInvalidated
	{
		add
		{
			_scrollInvalidated = (EventHandler)Delegate.Combine(_scrollInvalidated, value);
		}
		remove
		{
			_scrollInvalidated = (EventHandler)Delegate.Remove(_scrollInvalidated, value);
		}
	}

	static TextView()
	{
		DocumentProperty = AvaloniaProperty.Register<TextView, TextDocument>("Document");
		OptionsProperty = AvaloniaProperty.Register<TextView, TextEditorOptions>("Options");
		NonPrintableCharacterBrushProperty = AvaloniaProperty.Register<TextView, IBrush>("NonPrintableCharacterBrush", new SolidColorBrush(Color.FromArgb(145, 128, 128, 128)));
		LinkTextForegroundBrushProperty = AvaloniaProperty.Register<TextView, IBrush>("LinkTextForegroundBrush", Brushes.Blue);
		LinkTextBackgroundBrushProperty = AvaloniaProperty.Register<TextView, IBrush>("LinkTextBackgroundBrush", Brushes.Transparent);
		LinkTextUnderlineProperty = AvaloniaProperty.Register<TextView, bool>("LinkTextUnderline", defaultValue: true);
		PreviewPointerHoverEvent = RoutedEvent.Register<PointerEventArgs>("PreviewPointerHover", RoutingStrategies.Tunnel, typeof(TextView));
		PointerHoverEvent = RoutedEvent.Register<PointerEventArgs>("PointerHover", RoutingStrategies.Bubble, typeof(TextView));
		PreviewPointerHoverStoppedEvent = RoutedEvent.Register<PointerEventArgs>("PreviewPointerHoverStopped", RoutingStrategies.Tunnel, typeof(TextView));
		PointerHoverStoppedEvent = RoutedEvent.Register<PointerEventArgs>("PointerHoverStopped", RoutingStrategies.Bubble, typeof(TextView));
		ColumnRulerPenProperty = AvaloniaProperty.Register<TextView, IPen>("ColumnRulerBrush", CreateFrozenPen(new SolidColorBrush(Color.FromArgb(90, 128, 128, 128))));
		CurrentLineBackgroundProperty = AvaloniaProperty.Register<TextView, IBrush>("CurrentLineBackground");
		CurrentLineBorderProperty = AvaloniaProperty.Register<TextView, IPen>("CurrentLineBorder");
		Visual.ClipToBoundsProperty.OverrideDefaultValue<TextView>(defaultValue: true);
		InputElement.FocusableProperty.OverrideDefaultValue<TextView>(defaultValue: false);
		OptionsProperty.Changed.Subscribe(OnOptionsChanged);
		DocumentProperty.Changed.Subscribe(OnDocumentChanged);
	}

	public TextView()
	{
		Services.AddService(this);
		TextLayer = new TextLayer(this);
		_elementGenerators = new ObserveAddRemoveCollection<VisualLineElementGenerator>(ElementGenerator_Added, ElementGenerator_Removed);
		_lineTransformers = new ObserveAddRemoveCollection<IVisualLineTransformer>(LineTransformer_Added, LineTransformer_Removed);
		_backgroundRenderers = new ObserveAddRemoveCollection<IBackgroundRenderer>(BackgroundRenderer_Added, BackgroundRenderer_Removed);
		_currentLineHighlightRenderer = new CurrentLineHighlightRenderer(this);
		_columnRulerRenderer = new ColumnRulerRenderer(this);
		Options = new TextEditorOptions();
		Layers = new LayerCollection(this);
		InsertLayer(TextLayer, KnownLayer.Text, LayerInsertionPosition.Replace);
		_hoverLogic = new PointerHoverLogic(this);
		_hoverLogic.PointerHover += delegate(object sender, PointerEventArgs e)
		{
			RaiseHoverEventPair(e, PreviewPointerHoverEvent, PointerHoverEvent);
		};
		_hoverLogic.PointerHoverStopped += delegate(object sender, PointerEventArgs e)
		{
			RaiseHoverEventPair(e, PreviewPointerHoverStoppedEvent, PointerHoverStoppedEvent);
		};
	}

	private static void OnDocumentChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextView)?.OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
	}

	private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
	{
		if (oldValue != null)
		{
			_heightTree.Dispose();
			_heightTree = null;
			_formatter = null;
			CachedElements = null;
			WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.RemoveHandler(oldValue, OnChanging);
		}
		_document = newValue;
		ClearScrollData();
		ClearVisualLines();
		if (newValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.AddHandler(newValue, OnChanging);
			_formatter = TextFormatter.Current;
			InvalidateDefaultTextMetrics();
			_heightTree = new HeightTree(newValue, DefaultLineHeight);
			CachedElements = new TextViewCachedElements();
		}
		InvalidateMeasure();
		this.DocumentChanged?.Invoke(this, new DocumentChangedEventArgs(oldValue, newValue));
	}

	private void RecreateCachedElements()
	{
		if (CachedElements != null)
		{
			CachedElements = new TextViewCachedElements();
		}
	}

	private void OnChanging(object sender, DocumentChangeEventArgs e)
	{
		Redraw(e.Offset, e.RemovalLength);
	}

	private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		OnOptionChanged(e);
	}

	protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
	{
		this.OptionChanged?.Invoke(this, e);
		if (Options.ShowColumnRulers)
		{
			_columnRulerRenderer.SetRuler(Options.ColumnRulerPositions, ColumnRulerPen);
		}
		else
		{
			_columnRulerRenderer.SetRuler(null, ColumnRulerPen);
		}
		UpdateBuiltinElementGeneratorsFromOptions();
		Redraw();
	}

	private static void OnOptionsChanged(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as TextView)?.OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
	}

	private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.RemoveHandler(oldValue, OnPropertyChanged);
		}
		if (newValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.AddHandler(newValue, OnPropertyChanged);
		}
		OnOptionChanged(new PropertyChangedEventArgs(null));
	}

	private void ElementGenerator_Added(VisualLineElementGenerator generator)
	{
		ConnectToTextView(generator);
		Redraw();
	}

	private void ElementGenerator_Removed(VisualLineElementGenerator generator)
	{
		DisconnectFromTextView(generator);
		Redraw();
	}

	private void LineTransformer_Added(IVisualLineTransformer lineTransformer)
	{
		ConnectToTextView(lineTransformer);
		Redraw();
	}

	private void LineTransformer_Removed(IVisualLineTransformer lineTransformer)
	{
		DisconnectFromTextView(lineTransformer);
		Redraw();
	}

	private void UpdateBuiltinElementGeneratorsFromOptions()
	{
		TextEditorOptions options = Options;
		AddRemoveDefaultElementGeneratorOnDemand(ref _singleCharacterElementGenerator, options.ShowBoxForControlCharacters || options.ShowSpaces || options.ShowTabs);
		AddRemoveDefaultElementGeneratorOnDemand(ref _linkElementGenerator, options.EnableHyperlinks);
		AddRemoveDefaultElementGeneratorOnDemand(ref _mailLinkElementGenerator, options.EnableEmailHyperlinks);
	}

	private void AddRemoveDefaultElementGeneratorOnDemand<T>(ref T generator, bool demand) where T : VisualLineElementGenerator, IBuiltinElementGenerator, new()
	{
		if (generator != null != demand)
		{
			if (demand)
			{
				generator = new T();
				ElementGenerators.Add(generator);
			}
			else
			{
				ElementGenerators.Remove(generator);
				generator = null;
			}
		}
		generator?.FetchOptions(Options);
	}

	private void LayersChanged()
	{
		TextLayer.Index = Layers.IndexOf(TextLayer);
	}

	public void InsertLayer(Control layer, KnownLayer referencedLayer, LayerInsertionPosition position)
	{
		if (layer == null)
		{
			throw new ArgumentNullException("layer");
		}
		if (!Enum.IsDefined(typeof(KnownLayer), referencedLayer))
		{
			throw new ArgumentOutOfRangeException("referencedLayer", (int)referencedLayer, "KnownLayer");
		}
		if (!Enum.IsDefined(typeof(LayerInsertionPosition), position))
		{
			throw new ArgumentOutOfRangeException("position", (int)position, "LayerInsertionPosition");
		}
		if (referencedLayer == KnownLayer.Background && position != LayerInsertionPosition.Above)
		{
			throw new InvalidOperationException("Cannot replace or insert below the background layer.");
		}
		LayerPosition value = new LayerPosition(referencedLayer, position);
		LayerPosition.SetLayerPosition(layer, value);
		for (int i = 0; i < Layers.Count; i++)
		{
			LayerPosition layerPosition = LayerPosition.GetLayerPosition(Layers[i]);
			if (layerPosition == null)
			{
				continue;
			}
			if (layerPosition.KnownLayer == referencedLayer && layerPosition.Position == LayerInsertionPosition.Replace)
			{
				switch (position)
				{
				case LayerInsertionPosition.Below:
					Layers.Insert(i, layer);
					return;
				case LayerInsertionPosition.Above:
					Layers.Insert(i + 1, layer);
					return;
				case LayerInsertionPosition.Replace:
					Layers[i] = layer;
					return;
				}
			}
			else if ((layerPosition.KnownLayer == referencedLayer && layerPosition.Position == LayerInsertionPosition.Above) || layerPosition.KnownLayer > referencedLayer)
			{
				Layers.Insert(i, layer);
				return;
			}
		}
		Layers.Add(layer);
	}

	internal void AddInlineObject(InlineObjectRun inlineObject)
	{
		bool flag = false;
		for (int i = 0; i < _inlineObjects.Count; i++)
		{
			if (_inlineObjects[i].Element == inlineObject.Element)
			{
				RemoveInlineObjectRun(_inlineObjects[i], keepElement: true);
				_inlineObjects.RemoveAt(i);
				flag = true;
				break;
			}
		}
		_inlineObjects.Add(inlineObject);
		if (!flag)
		{
			base.VisualChildren.Add(inlineObject.Element);
			((ISetLogicalParent)inlineObject.Element).SetParent((ILogical?)this);
		}
		inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		inlineObject.DesiredSize = inlineObject.Element.DesiredSize;
	}

	private void MeasureInlineObjects()
	{
		foreach (InlineObjectRun inlineObject in _inlineObjects)
		{
			if (inlineObject.VisualLine.IsDisposed)
			{
				continue;
			}
			inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			if (!inlineObject.Element.DesiredSize.IsClose(inlineObject.DesiredSize))
			{
				inlineObject.DesiredSize = inlineObject.Element.DesiredSize;
				if (_allVisualLines.Remove(inlineObject.VisualLine))
				{
					DisposeVisualLine(inlineObject.VisualLine);
				}
			}
		}
	}

	private void RemoveInlineObjects(VisualLine visualLine)
	{
		if (visualLine.HasInlineObjects)
		{
			_visualLinesWithOutstandingInlineObjects.Add(visualLine);
		}
	}

	private void RemoveInlineObjectsNow()
	{
		if (_visualLinesWithOutstandingInlineObjects.Count == 0)
		{
			return;
		}
		_inlineObjects.RemoveAll(delegate(InlineObjectRun ior)
		{
			if (_visualLinesWithOutstandingInlineObjects.Contains(ior.VisualLine))
			{
				RemoveInlineObjectRun(ior, keepElement: false);
				return true;
			}
			return false;
		});
		_visualLinesWithOutstandingInlineObjects.Clear();
	}

	private void RemoveInlineObjectRun(InlineObjectRun ior, bool keepElement)
	{
		ior.VisualLine = null;
		if (!keepElement)
		{
			base.VisualChildren.Remove(ior.Element);
		}
	}

	public void Redraw()
	{
		VerifyAccess();
		ClearVisualLines();
		InvalidateMeasure();
	}

	public void Redraw(VisualLine visualLine)
	{
		VerifyAccess();
		if (_allVisualLines.Remove(visualLine))
		{
			DisposeVisualLine(visualLine);
			InvalidateMeasure();
		}
	}

	public void Redraw(int offset, int length)
	{
		VerifyAccess();
		bool flag = false;
		for (int i = 0; i < _allVisualLines.Count; i++)
		{
			VisualLine visualLine = _allVisualLines[i];
			int offset2 = visualLine.FirstDocumentLine.Offset;
			int num = visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.TotalLength;
			if (offset <= num)
			{
				flag = true;
				if (offset + length >= offset2)
				{
					_allVisualLines.RemoveAt(i--);
					DisposeVisualLine(visualLine);
				}
			}
		}
		if (flag)
		{
			InvalidateMeasure();
		}
	}

	public void InvalidateLayer(KnownLayer knownLayer)
	{
		InvalidateMeasure();
	}

	public void Redraw(ISegment segment)
	{
		if (segment != null)
		{
			Redraw(segment.Offset, segment.Length);
		}
	}

	private void ClearVisualLines()
	{
		if (_allVisualLines.Count == 0)
		{
			return;
		}
		foreach (VisualLine allVisualLine in _allVisualLines)
		{
			DisposeVisualLine(allVisualLine);
		}
		_allVisualLines.Clear();
		_visibleVisualLines = new ReadOnlyCollection<VisualLine>(_allVisualLines.ToArray());
	}

	private void DisposeVisualLine(VisualLine visualLine)
	{
		if (_newVisualLines != null && _newVisualLines.Contains(visualLine))
		{
			throw new ArgumentException("Cannot dispose visual line because it is in construction!");
		}
		visualLine.Dispose();
		RemoveInlineObjects(visualLine);
	}

	public VisualLine GetVisualLine(int documentLineNumber)
	{
		foreach (VisualLine allVisualLine in _allVisualLines)
		{
			int lineNumber = allVisualLine.FirstDocumentLine.LineNumber;
			int lineNumber2 = allVisualLine.LastDocumentLine.LineNumber;
			if (documentLineNumber >= lineNumber && documentLineNumber <= lineNumber2)
			{
				return allVisualLine;
			}
		}
		return null;
	}

	public VisualLine GetOrConstructVisualLine(DocumentLine documentLine)
	{
		if (documentLine == null)
		{
			throw new ArgumentNullException("documentLine");
		}
		if (!Document.Lines.Contains(documentLine))
		{
			throw new InvalidOperationException("Line belongs to wrong document");
		}
		VerifyAccess();
		VisualLine visualLine = GetVisualLine(documentLine.LineNumber);
		if (visualLine == null)
		{
			TextRunProperties textRunProperties = CreateGlobalTextRunProperties();
			VisualLineTextParagraphProperties paragraphProperties = CreateParagraphProperties(textRunProperties);
			while (_heightTree.GetIsCollapsed(documentLine.LineNumber))
			{
				documentLine = documentLine.PreviousLine;
			}
			visualLine = BuildVisualLine(documentLine, textRunProperties, paragraphProperties, _elementGenerators.ToArray(), _lineTransformers.ToArray(), _lastAvailableSize);
			_allVisualLines.Add(visualLine);
			foreach (VisualLine allVisualLine in _allVisualLines)
			{
				allVisualLine.VisualTop = _heightTree.GetVisualPosition(allVisualLine.FirstDocumentLine);
			}
		}
		return visualLine;
	}

	public void EnsureVisualLines()
	{
		Dispatcher.UIThread.VerifyAccess();
		if (_inMeasure)
		{
			throw new InvalidOperationException("The visual line build process is already running! Cannot EnsureVisualLines() during Measure!");
		}
		if (!VisualLinesValid)
		{
			InvalidateMeasure();
			InvalidateVisual();
		}
		if (!VisualLinesValid)
		{
			MeasureOverride(_lastAvailableSize);
		}
		if (!VisualLinesValid)
		{
			throw new VisualLinesInvalidException("Internal error: visual lines invalid after EnsureVisualLines call");
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (availableSize.Width > 32000.0)
		{
			availableSize = availableSize.WithWidth(32000.0);
		}
		if (!_canHorizontallyScroll && !availableSize.Width.IsClose(_lastAvailableSize.Width))
		{
			ClearVisualLines();
		}
		_lastAvailableSize = availableSize;
		foreach (Control layer in Layers)
		{
			layer.Measure(availableSize);
		}
		InvalidateVisual();
		MeasureInlineObjects();
		double num;
		if (_document == null)
		{
			_allVisualLines = new List<VisualLine>();
			_visibleVisualLines = new ReadOnlyCollection<VisualLine>(_allVisualLines.ToArray());
			num = 0.0;
		}
		else
		{
			_inMeasure = true;
			try
			{
				num = CreateAndMeasureVisualLines(availableSize);
			}
			finally
			{
				_inMeasure = false;
			}
		}
		RemoveInlineObjectsNow();
		num += 3.0;
		double documentHeight = DocumentHeight;
		TextEditorOptions options = Options;
		double num2 = Math.Min(availableSize.Height, documentHeight);
		double num3 = 0.0;
		if (options.AllowScrollBelowDocument && !double.IsInfinity(_scrollViewport.Height))
		{
			double defaultLineHeight = DefaultLineHeight;
			num3 = num2 - defaultLineHeight;
		}
		TextLayer.SetVisualLines(_visibleVisualLines);
		SetScrollData(availableSize, new Size(num, documentHeight + num3), _scrollOffset);
		this.VisualLinesChanged?.Invoke(this, EventArgs.Empty);
		return new Size(Math.Min(availableSize.Width, num), num2);
	}

	private double CreateAndMeasureVisualLines(Size availableSize)
	{
		TextRunProperties textRunProperties = CreateGlobalTextRunProperties();
		VisualLineTextParagraphProperties paragraphProperties = CreateParagraphProperties(textRunProperties);
		DocumentLine lineByVisualPosition = _heightTree.GetLineByVisualPosition(_scrollOffset.Y);
		_clippedPixelsOnTop = _scrollOffset.Y - _heightTree.GetVisualPosition(lineByVisualPosition);
		_newVisualLines = new List<VisualLine>();
		this.VisualLineConstructionStarting?.Invoke(this, new VisualLineConstructionStartEventArgs(lineByVisualPosition));
		VisualLineElementGenerator[] elementGeneratorsArray = _elementGenerators.ToArray();
		IVisualLineTransformer[] lineTransformersArray = _lineTransformers.ToArray();
		DocumentLine documentLine = lineByVisualPosition;
		double num = 0.0;
		double num2 = 0.0 - _clippedPixelsOnTop;
		while (num2 < availableSize.Height && documentLine != null)
		{
			VisualLine visualLine = GetVisualLine(documentLine.LineNumber) ?? BuildVisualLine(documentLine, textRunProperties, paragraphProperties, elementGeneratorsArray, lineTransformersArray, availableSize);
			visualLine.VisualTop = _scrollOffset.Y + num2;
			documentLine = visualLine.LastDocumentLine.NextLine;
			num2 += visualLine.Height;
			foreach (TextLine textLine in visualLine.TextLines)
			{
				if (textLine.WidthIncludingTrailingWhitespace > num)
				{
					num = textLine.WidthIncludingTrailingWhitespace;
				}
			}
			_newVisualLines.Add(visualLine);
		}
		foreach (VisualLine allVisualLine in _allVisualLines)
		{
			if (!_newVisualLines.Contains(allVisualLine))
			{
				DisposeVisualLine(allVisualLine);
			}
		}
		_allVisualLines = _newVisualLines;
		_visibleVisualLines = new ReadOnlyCollection<VisualLine>(_newVisualLines.ToArray());
		_newVisualLines = null;
		if (_allVisualLines.Any((VisualLine line) => line.IsDisposed))
		{
			throw new InvalidOperationException("A visual line was disposed even though it is still in use.\nThis can happen when Redraw() is called during measure for lines that are already constructed.");
		}
		return num;
	}

	private TextRunProperties CreateGlobalTextRunProperties()
	{
		return new GlobalTextRunProperties
		{
			typeface = this.CreateTypeface(),
			fontRenderingEmSize = FontSize,
			foregroundBrush = GetValue(TextElement.ForegroundProperty),
			cultureInfo = CultureInfo.CurrentCulture
		};
	}

	private VisualLineTextParagraphProperties CreateParagraphProperties(TextRunProperties defaultTextRunProperties)
	{
		return new VisualLineTextParagraphProperties
		{
			defaultTextRunProperties = defaultTextRunProperties,
			textWrapping = ((!_canHorizontallyScroll) ? TextWrapping.Wrap : TextWrapping.NoWrap),
			tabSize = (double)Options.IndentationSize * WideSpaceWidth
		};
	}

	private VisualLine BuildVisualLine(DocumentLine documentLine, TextRunProperties globalTextRunProperties, VisualLineTextParagraphProperties paragraphProperties, IReadOnlyList<VisualLineElementGenerator> elementGeneratorsArray, IReadOnlyList<IVisualLineTransformer> lineTransformersArray, Size availableSize)
	{
		if (_heightTree.GetIsCollapsed(documentLine.LineNumber))
		{
			throw new InvalidOperationException("Trying to build visual line from collapsed line");
		}
		VisualLine visualLine = new VisualLine(this, documentLine);
		VisualLineTextSource visualLineTextSource = new VisualLineTextSource(visualLine)
		{
			Document = _document,
			GlobalTextRunProperties = globalTextRunProperties,
			TextView = this
		};
		visualLine.ConstructVisualElements(visualLineTextSource, elementGeneratorsArray);
		if (visualLine.FirstDocumentLine != visualLine.LastDocumentLine)
		{
			double visualPosition = _heightTree.GetVisualPosition(visualLine.FirstDocumentLine.NextLine);
			double visualPosition2 = _heightTree.GetVisualPosition(visualLine.LastDocumentLine.NextLine ?? visualLine.LastDocumentLine);
			if (!visualPosition.IsClose(visualPosition2))
			{
				for (int i = visualLine.FirstDocumentLine.LineNumber + 1; i <= visualLine.LastDocumentLine.LineNumber; i++)
				{
					if (!_heightTree.GetIsCollapsed(i))
					{
						throw new InvalidOperationException("Line " + i + " was skipped by a VisualLineElementGenerator, but it is not collapsed.");
					}
				}
				throw new InvalidOperationException("All lines collapsed but visual pos different - height tree inconsistency?");
			}
		}
		visualLine.RunTransformers(visualLineTextSource, lineTransformersArray);
		TextLineBreak previousLineBreak = null;
		int num = 0;
		List<TextLine> list = new List<TextLine>();
		while (num <= visualLine.VisualLengthWithEndOfLineMarker)
		{
			TextLine textLine = _formatter.FormatLine(visualLineTextSource, num, availableSize.Width, paragraphProperties, previousLineBreak);
			list.Add(textLine);
			num += textLine.Length;
			if (num >= visualLine.VisualLengthWithEndOfLineMarker)
			{
				break;
			}
			if (paragraphProperties.firstLineInParagraph)
			{
				paragraphProperties.firstLineInParagraph = false;
				TextEditorOptions options = Options;
				double num2 = 0.0;
				if (options.InheritWordWrapIndentation)
				{
					int indentationVisualColumn = GetIndentationVisualColumn(visualLine);
					if (indentationVisualColumn > 0 && indentationVisualColumn < num)
					{
						num2 = textLine.GetDistanceFromCharacterHit(new CharacterHit(indentationVisualColumn));
					}
				}
				num2 += options.WordWrapIndentation;
				if (num2 > 0.0 && num2 * 2.0 < availableSize.Width)
				{
					paragraphProperties.indent = num2;
				}
			}
			previousLineBreak = textLine.TextLineBreak;
		}
		visualLine.SetTextLines(list);
		_heightTree.SetHeight(visualLine.FirstDocumentLine, visualLine.Height);
		return visualLine;
	}

	private static int GetIndentationVisualColumn(VisualLine visualLine)
	{
		if (visualLine.Elements.Count == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = 0;
		VisualLineElement visualLineElement = visualLine.Elements[num2];
		while (visualLineElement.IsWhitespace(num))
		{
			num++;
			if (num == visualLineElement.VisualColumn + visualLineElement.VisualLength)
			{
				num2++;
				if (num2 == visualLine.Elements.Count)
				{
					break;
				}
				visualLineElement = visualLine.Elements[num2];
			}
		}
		return num;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		EnsureVisualLines();
		foreach (Control layer in Layers)
		{
			layer.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
		}
		if (_document == null || _allVisualLines.Count == 0)
		{
			return finalSize;
		}
		double x = _scrollOffset.X;
		double y = _scrollOffset.Y;
		if (_scrollOffset.X + finalSize.Width > _scrollExtent.Width)
		{
			x = Math.Max(0.0, _scrollExtent.Width - finalSize.Width);
		}
		if (_scrollOffset.Y + finalSize.Height > _scrollExtent.Height)
		{
			y = Math.Max(0.0, _scrollExtent.Height - finalSize.Height);
		}
		if (SetScrollData(_scrollViewport, _scrollExtent, new Vector(x, y)))
		{
			InvalidateMeasure();
		}
		if (_visibleVisualLines != null)
		{
			Point point = new Point(0.0 - _scrollOffset.X, 0.0 - _clippedPixelsOnTop);
			foreach (VisualLine visibleVisualLine in _visibleVisualLines)
			{
				int num = 0;
				foreach (TextLine textLine in visibleVisualLine.TextLines)
				{
					foreach (TextRun textRun in textLine.TextRuns)
					{
						InlineObjectRun inlineObjectRun = textRun as InlineObjectRun;
						if (inlineObjectRun?.VisualLine != null)
						{
							double distanceFromCharacterHit = textLine.GetDistanceFromCharacterHit(new CharacterHit(num));
							inlineObjectRun.Element.Arrange(new Rect(new Point(point.X + distanceFromCharacterHit, point.Y), inlineObjectRun.Element.DesiredSize));
						}
						num += textRun.Length;
					}
					point = new Point(point.X, point.Y + textLine.Height);
				}
			}
		}
		InvalidateCursorIfPointerWithinTextView();
		return finalSize;
	}

	private void BackgroundRenderer_Added(IBackgroundRenderer renderer)
	{
		ConnectToTextView(renderer);
		InvalidateLayer(renderer.Layer);
	}

	private void BackgroundRenderer_Removed(IBackgroundRenderer renderer)
	{
		DisconnectFromTextView(renderer);
		InvalidateLayer(renderer.Layer);
	}

	public override void Render(DrawingContext drawingContext)
	{
		if (!VisualLinesValid)
		{
			return;
		}
		RenderBackground(drawingContext, KnownLayer.Background);
		foreach (VisualLine visibleVisualLine in _visibleVisualLines)
		{
			IBrush brush = null;
			int num = 0;
			int num2 = 0;
			foreach (VisualLineElement element in visibleVisualLine.Elements)
			{
				if (brush == null || !brush.Equals(element.BackgroundBrush))
				{
					if (brush != null)
					{
						BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder
						{
							AlignToWholePixels = true,
							CornerRadius = 3.0
						};
						foreach (Rect item in BackgroundGeometryBuilder.GetRectsFromVisualSegment(this, visibleVisualLine, num, num + num2))
						{
							backgroundGeometryBuilder.AddRectangle(this, item);
						}
						Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
						if (geometry != null)
						{
							drawingContext.DrawGeometry(brush, null, geometry);
						}
					}
					num = element.VisualColumn;
					num2 = element.DocumentLength;
					brush = element.BackgroundBrush;
				}
				else
				{
					num2 += element.VisualLength;
				}
			}
			if (brush == null)
			{
				continue;
			}
			BackgroundGeometryBuilder backgroundGeometryBuilder2 = new BackgroundGeometryBuilder
			{
				AlignToWholePixels = true,
				CornerRadius = 3.0
			};
			foreach (Rect item2 in BackgroundGeometryBuilder.GetRectsFromVisualSegment(this, visibleVisualLine, num, num + num2))
			{
				backgroundGeometryBuilder2.AddRectangle(this, item2);
			}
			Geometry geometry2 = backgroundGeometryBuilder2.CreateGeometry();
			if (geometry2 != null)
			{
				drawingContext.DrawGeometry(brush, null, geometry2);
			}
		}
	}

	internal void RenderBackground(DrawingContext drawingContext, KnownLayer layer)
	{
		drawingContext.FillRectangle(Brushes.Transparent, base.Bounds);
		foreach (IBackgroundRenderer backgroundRenderer in _backgroundRenderers)
		{
			if (backgroundRenderer.Layer == layer)
			{
				backgroundRenderer.Draw(this, drawingContext);
			}
		}
	}

	internal void ArrangeTextLayer(IList<VisualLineDrawingVisual> visuals)
	{
		Point point = new Point(0.0 - _scrollOffset.X, 0.0 - _clippedPixelsOnTop);
		foreach (VisualLineDrawingVisual visual in visuals)
		{
			if (!(visual.RenderTransform is TranslateTransform translateTransform) || translateTransform.X != point.X || translateTransform.Y != point.Y)
			{
				visual.RenderTransform = new TranslateTransform(point.X, point.Y);
			}
			point = new Point(point.X, point.Y + visual.LineHeight);
		}
	}

	private void ClearScrollData()
	{
		SetScrollData(default(Size), default(Size), default(Vector));
	}

	private bool SetScrollData(Size viewport, Size extent, Vector offset)
	{
		if (!viewport.IsClose(_scrollViewport) || !extent.IsClose(_scrollExtent) || !offset.IsClose(_scrollOffset))
		{
			_scrollViewport = viewport;
			_scrollExtent = extent;
			SetScrollOffset(offset);
			OnScrollChange();
			return true;
		}
		return false;
	}

	private void OnScrollChange()
	{
		((ILogicalScrollable)this).RaiseScrollInvalidated(EventArgs.Empty);
	}

	private void SetScrollOffset(Vector vector)
	{
		if (!_canHorizontallyScroll)
		{
			vector = new Vector(0.0, vector.Y);
		}
		if (!_canVerticallyScroll)
		{
			vector = new Vector(vector.X, 0.0);
		}
		if (!_scrollOffset.IsClose(vector))
		{
			_scrollOffset = vector;
			this.ScrollOffsetChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void InvalidateDefaultTextMetrics()
	{
		_defaultTextMetricsValid = false;
		if (_heightTree != null)
		{
			CalculateDefaultTextMetrics();
		}
	}

	private void CalculateDefaultTextMetrics()
	{
		if (!_defaultTextMetricsValid)
		{
			_defaultTextMetricsValid = true;
			if (_formatter != null)
			{
				TextRunProperties textRunProperties = CreateGlobalTextRunProperties();
				TextLine textLine = _formatter.FormatLine(new SimpleTextSource("x", textRunProperties), 0, 32000.0, new VisualLineTextParagraphProperties
				{
					defaultTextRunProperties = textRunProperties
				});
				_wideSpaceWidth = Math.Max(1.0, textLine.WidthIncludingTrailingWhitespace);
				_defaultBaseline = Math.Max(1.0, textLine.Baseline);
				_defaultLineHeight = Math.Max(1.0, textLine.Height);
			}
			else
			{
				_wideSpaceWidth = FontSize / 2.0;
				_defaultBaseline = FontSize;
				_defaultLineHeight = FontSize + 3.0;
			}
			if (_heightTree != null)
			{
				_heightTree.DefaultLineHeight = _defaultLineHeight;
			}
		}
	}

	private static double ValidateVisualOffset(double offset)
	{
		if (double.IsNaN(offset))
		{
			throw new ArgumentException("offset must not be NaN");
		}
		if (offset < 0.0)
		{
			return 0.0;
		}
		return offset;
	}

	public virtual void MakeVisible(Rect rectangle)
	{
		Rect rect = new Rect(_scrollOffset.X, _scrollOffset.Y, _scrollViewport.Width, _scrollViewport.Height);
		double offset = _scrollOffset.X;
		double offset2 = _scrollOffset.Y;
		if (rectangle.X < rect.X)
		{
			offset = ((!(rectangle.Right > rect.Right)) ? rectangle.X : (rectangle.X + rectangle.Width / 2.0));
		}
		else if (rectangle.Right > rect.Right)
		{
			offset = rectangle.Right - _scrollViewport.Width;
		}
		if (rectangle.Y < rect.Y)
		{
			offset2 = ((!(rectangle.Bottom > rect.Bottom)) ? rectangle.Y : (rectangle.Y + rectangle.Height / 2.0));
		}
		else if (rectangle.Bottom > rect.Bottom)
		{
			offset2 = rectangle.Bottom - _scrollViewport.Height;
		}
		offset = ValidateVisualOffset(offset);
		offset2 = ValidateVisualOffset(offset2);
		Vector vector = new Vector(offset, offset2);
		if (!_scrollOffset.IsClose(vector))
		{
			SetScrollOffset(vector);
			OnScrollChange();
			InvalidateMeasure();
		}
	}

	public static void InvalidateCursor()
	{
		if (!_invalidCursor)
		{
			_invalidCursor = true;
			Dispatcher.UIThread.InvokeAsync(delegate
			{
				_invalidCursor = false;
			}, DispatcherPriority.Background);
		}
	}

	internal void InvalidateCursorIfPointerWithinTextView()
	{
		if (base.IsPointerOver)
		{
			InvalidateCursor();
		}
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		base.OnPointerMoved(e);
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (!e.Handled)
		{
			EnsureVisualLines();
			GetVisualLineElementFromPosition(e.GetPosition(this) + _scrollOffset)?.OnPointerPressed(e);
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (!e.Handled)
		{
			EnsureVisualLines();
			GetVisualLineElementFromPosition(e.GetPosition(this) + _scrollOffset)?.OnPointerReleased(e);
		}
	}

	public VisualLine GetVisualLineFromVisualTop(double visualTop)
	{
		EnsureVisualLines();
		foreach (VisualLine visualLine in VisualLines)
		{
			if (!(visualTop < visualLine.VisualTop) && visualTop < visualLine.VisualTop + visualLine.Height)
			{
				return visualLine;
			}
		}
		return null;
	}

	public double GetVisualTopByDocumentLine(int line)
	{
		VerifyAccess();
		if (_heightTree == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return _heightTree.GetVisualPosition(_heightTree.GetLineByNumber(line));
	}

	private VisualLineElement GetVisualLineElementFromPosition(Point visualPosition)
	{
		VisualLine visualLineFromVisualTop = GetVisualLineFromVisualTop(visualPosition.Y);
		if (visualLineFromVisualTop != null)
		{
			int visualColumnFloor = visualLineFromVisualTop.GetVisualColumnFloor(visualPosition);
			foreach (VisualLineElement element in visualLineFromVisualTop.Elements)
			{
				if (element.VisualColumn + element.VisualLength > visualColumnFloor)
				{
					return element;
				}
			}
		}
		return null;
	}

	public Point GetVisualPosition(TextViewPosition position, VisualYPosition yPositionMode)
	{
		VerifyAccess();
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		DocumentLine lineByNumber = Document.GetLineByNumber(position.Line);
		VisualLine orConstructVisualLine = GetOrConstructVisualLine(lineByNumber);
		int visualColumn = position.VisualColumn;
		if (visualColumn < 0)
		{
			int num = lineByNumber.Offset + position.Column - 1;
			visualColumn = orConstructVisualLine.GetVisualColumn(num - orConstructVisualLine.FirstDocumentLine.Offset);
		}
		return orConstructVisualLine.GetVisualPosition(visualColumn, position.IsAtEndOfLine, yPositionMode);
	}

	public TextViewPosition? GetPosition(Point visualPosition)
	{
		VerifyAccess();
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return GetVisualLineFromVisualTop(visualPosition.Y)?.GetTextViewPosition(visualPosition, Options.EnableVirtualSpace);
	}

	public TextViewPosition? GetPositionFloor(Point visualPosition)
	{
		VerifyAccess();
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return GetVisualLineFromVisualTop(visualPosition.Y)?.GetTextViewPositionFloor(visualPosition, Options.EnableVirtualSpace);
	}

	public virtual object GetService(Type serviceType)
	{
		object service = Services.GetService(serviceType);
		if (service == null && _document != null)
		{
			service = _document.ServiceProvider.GetService(serviceType);
		}
		return service;
	}

	private void ConnectToTextView(object obj)
	{
		(obj as ITextViewConnect)?.AddToTextView(this);
	}

	private void DisconnectFromTextView(object obj)
	{
		(obj as ITextViewConnect)?.RemoveFromTextView(this);
	}

	private void RaiseHoverEventPair(PointerEventArgs e, RoutedEvent tunnelingEvent, RoutedEvent bubblingEvent)
	{
		e.RoutedEvent = tunnelingEvent;
		RaiseEvent(e);
		e.RoutedEvent = bubblingEvent;
		RaiseEvent(e);
	}

	public CollapsedLineSection CollapseLines(DocumentLine start, DocumentLine end)
	{
		VerifyAccess();
		if (_heightTree == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return _heightTree.CollapseText(start, end);
	}

	public DocumentLine GetDocumentLineByVisualTop(double visualTop)
	{
		VerifyAccess();
		if (_heightTree == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return _heightTree.GetLineByVisualPosition(visualTop);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == TemplatedControl.ForegroundProperty || change.Property == NonPrintableCharacterBrushProperty || change.Property == LinkTextBackgroundBrushProperty || change.Property == LinkTextForegroundBrushProperty || change.Property == LinkTextUnderlineProperty)
		{
			RecreateCachedElements();
			Redraw();
		}
		if (change.Property == TemplatedControl.FontFamilyProperty || change.Property == TemplatedControl.FontSizeProperty || change.Property == TemplatedControl.FontStyleProperty || change.Property == TemplatedControl.FontWeightProperty)
		{
			RecreateCachedElements();
			InvalidateDefaultTextMetrics();
			Redraw();
		}
		if (change.Property == ColumnRulerPenProperty)
		{
			_columnRulerRenderer.SetRuler(Options.ColumnRulerPositions, ColumnRulerPen);
		}
		if (change.Property == CurrentLineBorderProperty)
		{
			_currentLineHighlightRenderer.BorderPen = CurrentLineBorder;
		}
		if (change.Property == CurrentLineBackgroundProperty)
		{
			_currentLineHighlightRenderer.BackgroundBrush = CurrentLineBackground;
		}
	}

	private static ImmutablePen CreateFrozenPen(IBrush brush)
	{
		return new ImmutablePen(brush?.ToImmutable());
	}

	bool ILogicalScrollable.BringIntoView(Control target, Rect rectangle)
	{
		if (rectangle == default(Rect) || target == null || target == this || !this.IsVisualAncestorOf(target))
		{
			return false;
		}
		MakeVisible(rectangle.WithX(rectangle.X + _scrollOffset.X).WithY(rectangle.Y + _scrollOffset.Y));
		return true;
	}

	Control ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control from)
	{
		return null;
	}

	void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
	{
		_scrollInvalidated?.Invoke(this, e);
	}
}
