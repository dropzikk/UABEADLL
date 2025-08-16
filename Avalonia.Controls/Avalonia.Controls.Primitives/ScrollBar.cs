using System;
using System.Linq;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

[TemplatePart("PART_LineDownButton", typeof(Button))]
[TemplatePart("PART_LineUpButton", typeof(Button))]
[TemplatePart("PART_PageDownButton", typeof(Button))]
[TemplatePart("PART_PageUpButton", typeof(Button))]
[PseudoClasses(new string[] { ":vertical", ":horizontal" })]
public class ScrollBar : RangeBase
{
	public static readonly StyledProperty<double> ViewportSizeProperty;

	public static readonly StyledProperty<ScrollBarVisibility> VisibilityProperty;

	public static readonly StyledProperty<Orientation> OrientationProperty;

	public static readonly DirectProperty<ScrollBar, bool> IsExpandedProperty;

	public static readonly StyledProperty<bool> AllowAutoHideProperty;

	public static readonly StyledProperty<TimeSpan> HideDelayProperty;

	public static readonly StyledProperty<TimeSpan> ShowDelayProperty;

	private Button? _lineUpButton;

	private Button? _lineDownButton;

	private Button? _pageUpButton;

	private Button? _pageDownButton;

	private DispatcherTimer? _timer;

	private bool _isExpanded;

	private CompositeDisposable? _ownerSubscriptions;

	private ScrollViewer? _owner;

	public double ViewportSize
	{
		get
		{
			return GetValue(ViewportSizeProperty);
		}
		set
		{
			SetValue(ViewportSizeProperty, value);
		}
	}

	public ScrollBarVisibility Visibility
	{
		get
		{
			return GetValue(VisibilityProperty);
		}
		set
		{
			SetValue(VisibilityProperty, value);
		}
	}

	public Orientation Orientation
	{
		get
		{
			return GetValue(OrientationProperty);
		}
		set
		{
			SetValue(OrientationProperty, value);
		}
	}

	public bool IsExpanded
	{
		get
		{
			return _isExpanded;
		}
		private set
		{
			SetAndRaise(IsExpandedProperty, ref _isExpanded, value);
		}
	}

	public bool AllowAutoHide
	{
		get
		{
			return GetValue(AllowAutoHideProperty);
		}
		set
		{
			SetValue(AllowAutoHideProperty, value);
		}
	}

	public TimeSpan HideDelay
	{
		get
		{
			return GetValue(HideDelayProperty);
		}
		set
		{
			SetValue(HideDelayProperty, value);
		}
	}

	public TimeSpan ShowDelay
	{
		get
		{
			return GetValue(ShowDelayProperty);
		}
		set
		{
			SetValue(ShowDelayProperty, value);
		}
	}

	public event EventHandler<ScrollEventArgs>? Scroll;

	static ScrollBar()
	{
		ViewportSizeProperty = AvaloniaProperty.Register<ScrollBar, double>("ViewportSize", double.NaN);
		VisibilityProperty = AvaloniaProperty.Register<ScrollBar, ScrollBarVisibility>("Visibility", ScrollBarVisibility.Visible);
		OrientationProperty = AvaloniaProperty.Register<ScrollBar, Orientation>("Orientation", Orientation.Vertical);
		IsExpandedProperty = AvaloniaProperty.RegisterDirect("IsExpanded", (ScrollBar o) => o.IsExpanded, null, unsetValue: false);
		AllowAutoHideProperty = AvaloniaProperty.Register<ScrollBar, bool>("AllowAutoHide", defaultValue: true);
		HideDelayProperty = AvaloniaProperty.Register<ScrollBar, TimeSpan>("HideDelay", TimeSpan.FromSeconds(2.0));
		ShowDelayProperty = AvaloniaProperty.Register<ScrollBar, TimeSpan>("ShowDelay", TimeSpan.FromSeconds(0.5));
		Thumb.DragDeltaEvent.AddClassHandler(delegate(ScrollBar x, VectorEventArgs e)
		{
			x.OnThumbDragDelta(e);
		}, RoutingStrategies.Bubble);
		Thumb.DragCompletedEvent.AddClassHandler(delegate(ScrollBar x, VectorEventArgs e)
		{
			x.OnThumbDragComplete(e);
		}, RoutingStrategies.Bubble);
		InputElement.FocusableProperty.OverrideMetadata<ScrollBar>(new StyledPropertyMetadata<bool>(false));
	}

	public ScrollBar()
	{
		UpdatePseudoClasses(Orientation);
	}

	private void UpdateIsVisible()
	{
		bool value = Visibility switch
		{
			ScrollBarVisibility.Visible => true, 
			ScrollBarVisibility.Disabled => false, 
			ScrollBarVisibility.Hidden => false, 
			ScrollBarVisibility.Auto => double.IsNaN(ViewportSize) || base.Maximum > 0.0, 
			_ => throw new InvalidOperationException("Invalid value for ScrollBar.Visibility."), 
		};
		SetCurrentValue(Visual.IsVisibleProperty, value);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		AttachToScrollViewer();
	}

	internal void AttachToScrollViewer()
	{
		ScrollViewer owner = this.FindAncestorOfType<ScrollViewer>();
		if (owner == null)
		{
			_owner = null;
			_ownerSubscriptions?.Dispose();
			_ownerSubscriptions = null;
		}
		else if (owner != _owner)
		{
			_ownerSubscriptions?.Dispose();
			AttachedProperty<ScrollBarVisibility> visibilitySource = ((Orientation == Orientation.Horizontal) ? ScrollViewer.HorizontalScrollBarVisibilityProperty : ScrollViewer.VerticalScrollBarVisibilityProperty);
			IDisposable[] disposables = new IDisposable[7]
			{
				IfUnset<StyledProperty<double>>(RangeBase.MaximumProperty, (StyledProperty<double> p) => Bind(p, owner.GetObservable(ScrollViewer.ScrollBarMaximumProperty, ExtractOrdinate), BindingPriority.Template)),
				IfUnset<StyledProperty<double>>(RangeBase.ValueProperty, (StyledProperty<double> p) => Bind(p, owner.GetObservable(ScrollViewer.OffsetProperty, ExtractOrdinate), BindingPriority.Template)),
				IfUnset<StyledProperty<double>>(ViewportSizeProperty, (StyledProperty<double> p) => Bind(p, owner.GetObservable(ScrollViewer.ViewportProperty, ExtractOrdinate), BindingPriority.Template)),
				IfUnset<StyledProperty<ScrollBarVisibility>>(VisibilityProperty, (StyledProperty<ScrollBarVisibility> p) => Bind(p, owner.GetObservable(visibilitySource), BindingPriority.Template)),
				IfUnset<StyledProperty<bool>>(AllowAutoHideProperty, (StyledProperty<bool> p) => Bind(p, owner.GetObservable(ScrollViewer.AllowAutoHideProperty), BindingPriority.Template)),
				IfUnset<StyledProperty<double>>(RangeBase.LargeChangeProperty, (StyledProperty<double> p) => Bind(p, owner.GetObservable(ScrollViewer.LargeChangeProperty).Select(ExtractOrdinate), BindingPriority.Template)),
				IfUnset<StyledProperty<double>>(RangeBase.SmallChangeProperty, (StyledProperty<double> p) => Bind(p, owner.GetObservable(ScrollViewer.SmallChangeProperty).Select(ExtractOrdinate), BindingPriority.Template))
			}.Where((IDisposable d) => d != null).Cast<IDisposable>().ToArray();
			_owner = owner;
			_ownerSubscriptions = new CompositeDisposable(disposables);
		}
		IDisposable? IfUnset<T>(T property, Func<T, IDisposable> func) where T : notnull, AvaloniaProperty
		{
			if (!IsSet(property))
			{
				return func(property);
			}
			return null;
		}
	}

	private double ExtractOrdinate(Vector v)
	{
		if (Orientation != 0)
		{
			return v.Y;
		}
		return v.X;
	}

	private double ExtractOrdinate(Size v)
	{
		if (Orientation != 0)
		{
			return v.Height;
		}
		return v.Width;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.Key == Key.PageUp)
		{
			LargeDecrement();
			e.Handled = true;
		}
		else if (e.Key == Key.PageDown)
		{
			LargeIncrement();
			e.Handled = true;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == OrientationProperty)
		{
			UpdatePseudoClasses(change.GetNewValue<Orientation>());
			if (base.IsAttachedToVisualTree)
			{
				AttachToScrollViewer();
			}
		}
		else if (change.Property == AllowAutoHideProperty)
		{
			UpdateIsExpandedState();
		}
		else if (change.Property == RangeBase.ValueProperty)
		{
			double newValue = change.GetNewValue<double>();
			_owner?.SetCurrentValue(ScrollViewer.OffsetProperty, (Orientation == Orientation.Horizontal) ? _owner.Offset.WithX(newValue) : _owner.Offset.WithY(newValue));
		}
		else if (change.Property == RangeBase.MinimumProperty || change.Property == RangeBase.MaximumProperty || change.Property == ViewportSizeProperty || change.Property == VisibilityProperty)
		{
			UpdateIsVisible();
		}
	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		base.OnPointerEntered(e);
		if (AllowAutoHide)
		{
			ExpandAfterDelay();
		}
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		base.OnPointerExited(e);
		if (AllowAutoHide)
		{
			CollapseAfterDelay();
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		if (_lineUpButton != null)
		{
			_lineUpButton.Click -= LineUpClick;
		}
		if (_lineDownButton != null)
		{
			_lineDownButton.Click -= LineDownClick;
		}
		if (_pageUpButton != null)
		{
			_pageUpButton.Click -= PageUpClick;
		}
		if (_pageDownButton != null)
		{
			_pageDownButton.Click -= PageDownClick;
		}
		_lineUpButton = e.NameScope.Find<Button>("PART_LineUpButton");
		_lineDownButton = e.NameScope.Find<Button>("PART_LineDownButton");
		_pageUpButton = e.NameScope.Find<Button>("PART_PageUpButton");
		_pageDownButton = e.NameScope.Find<Button>("PART_PageDownButton");
		if (_lineUpButton != null)
		{
			_lineUpButton.Click += LineUpClick;
		}
		if (_lineDownButton != null)
		{
			_lineDownButton.Click += LineDownClick;
		}
		if (_pageUpButton != null)
		{
			_pageUpButton.Click += PageUpClick;
		}
		if (_pageDownButton != null)
		{
			_pageDownButton.Click += PageDownClick;
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ScrollBarAutomationPeer(this);
	}

	private void InvokeAfterDelay(Action handler, TimeSpan delay)
	{
		if (_timer != null)
		{
			_timer.Stop();
		}
		else
		{
			_timer = new DispatcherTimer(DispatcherPriority.Normal);
			_timer.Tick += delegate(object? sender, EventArgs args)
			{
				DispatcherTimer obj = (DispatcherTimer)sender;
				if (obj.Tag is Action action)
				{
					action();
				}
				obj.Stop();
			};
		}
		_timer.Tag = handler;
		_timer.Interval = delay;
		_timer.Start();
	}

	private void UpdateIsExpandedState()
	{
		if (!AllowAutoHide)
		{
			_timer?.Stop();
			IsExpanded = true;
		}
		else
		{
			IsExpanded = base.IsPointerOver;
		}
	}

	private void CollapseAfterDelay()
	{
		InvokeAfterDelay(Collapse, HideDelay);
	}

	private void ExpandAfterDelay()
	{
		InvokeAfterDelay(Expand, ShowDelay);
	}

	private void Collapse()
	{
		IsExpanded = false;
	}

	private void Expand()
	{
		IsExpanded = true;
	}

	private void LineUpClick(object? sender, RoutedEventArgs e)
	{
		SmallDecrement();
	}

	private void LineDownClick(object? sender, RoutedEventArgs e)
	{
		SmallIncrement();
	}

	private void PageUpClick(object? sender, RoutedEventArgs e)
	{
		LargeDecrement();
	}

	private void PageDownClick(object? sender, RoutedEventArgs e)
	{
		LargeIncrement();
	}

	private void SmallDecrement()
	{
		SetCurrentValue(RangeBase.ValueProperty, Math.Max(base.Value - base.SmallChange, base.Minimum));
		OnScroll(ScrollEventType.SmallDecrement);
	}

	private void SmallIncrement()
	{
		SetCurrentValue(RangeBase.ValueProperty, Math.Min(base.Value + base.SmallChange, base.Maximum));
		OnScroll(ScrollEventType.SmallIncrement);
	}

	private void LargeDecrement()
	{
		SetCurrentValue(RangeBase.ValueProperty, Math.Max(base.Value - base.LargeChange, base.Minimum));
		OnScroll(ScrollEventType.LargeDecrement);
	}

	private void LargeIncrement()
	{
		SetCurrentValue(RangeBase.ValueProperty, Math.Min(base.Value + base.LargeChange, base.Maximum));
		OnScroll(ScrollEventType.LargeIncrement);
	}

	private void OnThumbDragDelta(VectorEventArgs e)
	{
		OnScroll(ScrollEventType.ThumbTrack);
	}

	private void OnThumbDragComplete(VectorEventArgs e)
	{
		OnScroll(ScrollEventType.EndScroll);
	}

	protected void OnScroll(ScrollEventType scrollEventType)
	{
		this.Scroll?.Invoke(this, new ScrollEventArgs(scrollEventType, base.Value));
	}

	private void UpdatePseudoClasses(Orientation o)
	{
		base.PseudoClasses.Set(":vertical", o == Orientation.Vertical);
		base.PseudoClasses.Set(":horizontal", o == Orientation.Horizontal);
	}
}
