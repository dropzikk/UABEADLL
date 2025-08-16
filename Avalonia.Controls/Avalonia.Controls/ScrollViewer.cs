using System;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[TemplatePart("PART_HorizontalScrollBar", typeof(ScrollBar))]
[TemplatePart("PART_VerticalScrollBar", typeof(ScrollBar))]
public class ScrollViewer : ContentControl, IScrollable, IScrollAnchorProvider
{
	public static readonly AttachedProperty<bool> BringIntoViewOnFocusChangeProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, bool>("BringIntoViewOnFocusChange", defaultValue: true);

	public static readonly DirectProperty<ScrollViewer, Size> ExtentProperty = AvaloniaProperty.RegisterDirect("Extent", (ScrollViewer o) => o.Extent);

	public static readonly StyledProperty<Vector> OffsetProperty;

	public static readonly DirectProperty<ScrollViewer, Size> ViewportProperty;

	public static readonly DirectProperty<ScrollViewer, Size> LargeChangeProperty;

	public static readonly DirectProperty<ScrollViewer, Size> SmallChangeProperty;

	public static readonly DirectProperty<ScrollViewer, Vector> ScrollBarMaximumProperty;

	public static readonly AttachedProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty;

	public static readonly AttachedProperty<SnapPointsType> HorizontalSnapPointsTypeProperty;

	public static readonly AttachedProperty<SnapPointsType> VerticalSnapPointsTypeProperty;

	public static readonly AttachedProperty<SnapPointsAlignment> HorizontalSnapPointsAlignmentProperty;

	public static readonly AttachedProperty<SnapPointsAlignment> VerticalSnapPointsAlignmentProperty;

	public static readonly AttachedProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty;

	public static readonly DirectProperty<ScrollViewer, bool> IsExpandedProperty;

	public static readonly AttachedProperty<bool> AllowAutoHideProperty;

	public static readonly AttachedProperty<bool> IsScrollChainingEnabledProperty;

	public static readonly AttachedProperty<bool> IsScrollInertiaEnabledProperty;

	public static readonly RoutedEvent<ScrollChangedEventArgs> ScrollChangedEvent;

	internal const double DefaultSmallChange = 16.0;

	private IDisposable? _childSubscription;

	private ILogicalScrollable? _logicalScrollable;

	private Size _extent;

	private Size _viewport;

	private Size _oldExtent;

	private Vector _oldOffset;

	private Vector _oldMaximum;

	private Size _oldViewport;

	private Size _largeChange;

	private Size _smallChange = new Size(16.0, 16.0);

	private bool _isExpanded;

	private IDisposable? _scrollBarExpandSubscription;

	public bool BringIntoViewOnFocusChange
	{
		get
		{
			return GetValue(BringIntoViewOnFocusChangeProperty);
		}
		set
		{
			SetValue(BringIntoViewOnFocusChangeProperty, value);
		}
	}

	public Size Extent
	{
		get
		{
			return _extent;
		}
		internal set
		{
			if (SetAndRaise(ExtentProperty, ref _extent, value))
			{
				CalculatedPropertiesChanged();
			}
		}
	}

	public Vector Offset
	{
		get
		{
			return GetValue(OffsetProperty);
		}
		set
		{
			SetValue(OffsetProperty, value);
		}
	}

	public Size Viewport
	{
		get
		{
			return _viewport;
		}
		internal set
		{
			if (SetAndRaise(ViewportProperty, ref _viewport, value))
			{
				CalculatedPropertiesChanged();
			}
		}
	}

	public Size LargeChange => _largeChange;

	public Size SmallChange => _smallChange;

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

	protected bool CanHorizontallyScroll => HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;

	protected bool CanVerticallyScroll => VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;

	public Control? CurrentAnchor => (base.Presenter as IScrollAnchorProvider)?.CurrentAnchor;

	public Vector ScrollBarMaximum => new Vector(Max(_extent.Width - _viewport.Width, 0.0), Max(_extent.Height - _viewport.Height, 0.0));

	public bool IsExpanded
	{
		get
		{
			return _isExpanded;
		}
		private set
		{
			SetAndRaise(ScrollBar.IsExpandedProperty, ref _isExpanded, value);
		}
	}

	public SnapPointsType HorizontalSnapPointsType
	{
		get
		{
			return GetValue(HorizontalSnapPointsTypeProperty);
		}
		set
		{
			SetValue(HorizontalSnapPointsTypeProperty, value);
		}
	}

	public SnapPointsType VerticalSnapPointsType
	{
		get
		{
			return GetValue(VerticalSnapPointsTypeProperty);
		}
		set
		{
			SetValue(VerticalSnapPointsTypeProperty, value);
		}
	}

	public SnapPointsAlignment HorizontalSnapPointsAlignment
	{
		get
		{
			return GetValue(HorizontalSnapPointsAlignmentProperty);
		}
		set
		{
			SetValue(HorizontalSnapPointsAlignmentProperty, value);
		}
	}

	public SnapPointsAlignment VerticalSnapPointsAlignment
	{
		get
		{
			return GetValue(VerticalSnapPointsAlignmentProperty);
		}
		set
		{
			SetValue(VerticalSnapPointsAlignmentProperty, value);
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

	public bool IsScrollChainingEnabled
	{
		get
		{
			return GetValue(IsScrollChainingEnabledProperty);
		}
		set
		{
			SetValue(IsScrollChainingEnabledProperty, value);
		}
	}

	public bool IsScrollInertiaEnabled
	{
		get
		{
			return GetValue(IsScrollInertiaEnabledProperty);
		}
		set
		{
			SetValue(IsScrollInertiaEnabledProperty, value);
		}
	}

	public event EventHandler<ScrollChangedEventArgs>? ScrollChanged
	{
		add
		{
			AddHandler(ScrollChangedEvent, value);
		}
		remove
		{
			RemoveHandler(ScrollChangedEvent, value);
		}
	}

	public ScrollViewer()
	{
		base.LayoutUpdated += OnLayoutUpdated;
	}

	public void LineUp()
	{
		SetCurrentValue(OffsetProperty, Offset - new Vector(0.0, _smallChange.Height));
	}

	public void LineDown()
	{
		SetCurrentValue(OffsetProperty, Offset + new Vector(0.0, _smallChange.Height));
	}

	public void LineLeft()
	{
		SetCurrentValue(OffsetProperty, Offset - new Vector(_smallChange.Width, 0.0));
	}

	public void LineRight()
	{
		SetCurrentValue(OffsetProperty, Offset + new Vector(_smallChange.Width, 0.0));
	}

	public void PageUp()
	{
		SetCurrentValue(OffsetProperty, Offset.WithY(Math.Max(Offset.Y - _viewport.Height, 0.0)));
	}

	public void PageDown()
	{
		SetCurrentValue(OffsetProperty, Offset.WithY(Math.Min(Offset.Y + _viewport.Height, ScrollBarMaximum.Y)));
	}

	public void PageLeft()
	{
		SetCurrentValue(OffsetProperty, Offset.WithX(Math.Max(Offset.X - _viewport.Width, 0.0)));
	}

	public void PageRight()
	{
		SetCurrentValue(OffsetProperty, Offset.WithX(Math.Min(Offset.X + _viewport.Width, ScrollBarMaximum.X)));
	}

	public void ScrollToHome()
	{
		SetCurrentValue(OffsetProperty, new Vector(double.NegativeInfinity, double.NegativeInfinity));
	}

	public void ScrollToEnd()
	{
		SetCurrentValue(OffsetProperty, new Vector(double.NegativeInfinity, double.PositiveInfinity));
	}

	public static bool GetBringIntoViewOnFocusChange(Control control)
	{
		return control.GetValue(BringIntoViewOnFocusChangeProperty);
	}

	public static void SetBringIntoViewOnFocusChange(Control control, bool value)
	{
		control.SetValue(BringIntoViewOnFocusChangeProperty, value);
	}

	public static ScrollBarVisibility GetHorizontalScrollBarVisibility(Control control)
	{
		return control.GetValue(HorizontalScrollBarVisibilityProperty);
	}

	public static void SetHorizontalScrollBarVisibility(Control control, ScrollBarVisibility value)
	{
		control.SetValue(HorizontalScrollBarVisibilityProperty, value);
	}

	public static SnapPointsType GetHorizontalSnapPointsType(Control control)
	{
		return control.GetValue(HorizontalSnapPointsTypeProperty);
	}

	public static void SetHorizontalSnapPointsType(Control control, SnapPointsType value)
	{
		control.SetValue(HorizontalSnapPointsTypeProperty, value);
	}

	public static SnapPointsType GetVerticalSnapPointsType(Control control)
	{
		return control.GetValue(VerticalSnapPointsTypeProperty);
	}

	public static void SetVerticalSnapPointsType(Control control, SnapPointsType value)
	{
		control.SetValue(VerticalSnapPointsTypeProperty, value);
	}

	public static SnapPointsAlignment GetHorizontalSnapPointsAlignment(Control control)
	{
		return control.GetValue(HorizontalSnapPointsAlignmentProperty);
	}

	public static void SetHorizontalSnapPointsAlignment(Control control, SnapPointsAlignment value)
	{
		control.SetValue(HorizontalSnapPointsAlignmentProperty, value);
	}

	public static SnapPointsAlignment GetVerticalSnapPointsAlignment(Control control)
	{
		return control.GetValue(VerticalSnapPointsAlignmentProperty);
	}

	public static void SetVerticalSnapPointsAlignment(Control control, SnapPointsAlignment value)
	{
		control.SetValue(VerticalSnapPointsAlignmentProperty, value);
	}

	public static ScrollBarVisibility GetVerticalScrollBarVisibility(Control control)
	{
		return control.GetValue(VerticalScrollBarVisibilityProperty);
	}

	public static void SetAllowAutoHide(Control control, bool value)
	{
		control.SetValue(AllowAutoHideProperty, value);
	}

	public static bool GetAllowAutoHide(Control control)
	{
		return control.GetValue(AllowAutoHideProperty);
	}

	public static void SetIsScrollChainingEnabled(Control control, bool value)
	{
		control.SetValue(IsScrollChainingEnabledProperty, value);
	}

	public static bool GetIsScrollChainingEnabled(Control control)
	{
		return control.GetValue(IsScrollChainingEnabledProperty);
	}

	public static void SetVerticalScrollBarVisibility(Control control, ScrollBarVisibility value)
	{
		control.SetValue(VerticalScrollBarVisibilityProperty, value);
	}

	public static bool GetIsScrollInertiaEnabled(Control control)
	{
		return control.GetValue(IsScrollInertiaEnabledProperty);
	}

	public static void SetIsScrollInertiaEnabled(Control control, bool value)
	{
		control.SetValue(IsScrollInertiaEnabledProperty, value);
	}

	public void RegisterAnchorCandidate(Control element)
	{
		(base.Presenter as IScrollAnchorProvider)?.RegisterAnchorCandidate(element);
	}

	public void UnregisterAnchorCandidate(Control element)
	{
		(base.Presenter as IScrollAnchorProvider)?.UnregisterAnchorCandidate(element);
	}

	protected override bool RegisterContentPresenter(ContentPresenter presenter)
	{
		_childSubscription?.Dispose();
		_childSubscription = null;
		if (base.RegisterContentPresenter(presenter))
		{
			_childSubscription = base.Presenter?.GetObservable(ContentPresenter.ChildProperty).Subscribe(ChildChanged);
			return true;
		}
		return false;
	}

	internal static Vector CoerceOffset(AvaloniaObject sender, Vector value)
	{
		Size value2 = sender.GetValue(ExtentProperty);
		Size value3 = sender.GetValue(ViewportProperty);
		double max = Math.Max(value2.Width - value3.Width, 0.0);
		double max2 = Math.Max(value2.Height - value3.Height, 0.0);
		return new Vector(Clamp(value.X, 0.0, max), Clamp(value.Y, 0.0, max2));
	}

	private static double Clamp(double value, double min, double max)
	{
		if (!(value < min))
		{
			if (!(value > max))
			{
				return value;
			}
			return max;
		}
		return min;
	}

	private static double Max(double x, double y)
	{
		double num = Math.Max(x, y);
		if (!double.IsNaN(num))
		{
			return num;
		}
		return 0.0;
	}

	private void ChildChanged(Control? child)
	{
		if (_logicalScrollable != null)
		{
			_logicalScrollable.ScrollInvalidated -= LogicalScrollInvalidated;
			_logicalScrollable = null;
		}
		if (child is ILogicalScrollable logicalScrollable)
		{
			_logicalScrollable = logicalScrollable;
			logicalScrollable.ScrollInvalidated += LogicalScrollInvalidated;
		}
		CalculatedPropertiesChanged();
	}

	private void LogicalScrollInvalidated(object? sender, EventArgs e)
	{
		CalculatedPropertiesChanged();
	}

	private void CalculatedPropertiesChanged()
	{
		Vector scrollBarMaximum = ScrollBarMaximum;
		if (scrollBarMaximum != _oldMaximum)
		{
			RaisePropertyChanged(ScrollBarMaximumProperty, _oldMaximum, scrollBarMaximum);
			_oldMaximum = scrollBarMaximum;
		}
		ILogicalScrollable? logicalScrollable = _logicalScrollable;
		if (logicalScrollable != null && logicalScrollable.IsLogicalScrollEnabled)
		{
			SetAndRaise(SmallChangeProperty, ref _smallChange, _logicalScrollable.ScrollSize);
			SetAndRaise(LargeChangeProperty, ref _largeChange, _logicalScrollable.PageScrollSize);
		}
		else
		{
			SetAndRaise(SmallChangeProperty, ref _smallChange, new Size(16.0, 16.0));
			SetAndRaise(LargeChangeProperty, ref _largeChange, Viewport);
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == OffsetProperty)
		{
			CalculatedPropertiesChanged();
		}
		else if (change.Property == ExtentProperty)
		{
			CoerceValue(OffsetProperty);
		}
		else if (change.Property == ViewportProperty)
		{
			CoerceValue(OffsetProperty);
		}
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		base.OnGotFocus(e);
		if (e.Source != this && e.Source is Control control && BringIntoViewOnFocusChange)
		{
			control.BringIntoView();
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.Key == Key.PageUp)
		{
			PageUp();
			e.Handled = true;
		}
		else if (e.Key == Key.PageDown)
		{
			PageDown();
			e.Handled = true;
		}
	}

	protected virtual void OnScrollChanged(ScrollChangedEventArgs e)
	{
		RaiseEvent(e);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_scrollBarExpandSubscription?.Dispose();
		_scrollBarExpandSubscription = SubscribeToScrollBars(e);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ScrollViewerAutomationPeer(this);
	}

	private IDisposable? SubscribeToScrollBars(TemplateAppliedEventArgs e)
	{
		ScrollBar scrollBar2 = e.NameScope.Find<ScrollBar>("PART_HorizontalScrollBar");
		ScrollBar? scrollBar3 = e.NameScope.Find<ScrollBar>("PART_VerticalScrollBar");
		IObservable<bool> observable = GetExpandedObservable(scrollBar2);
		IObservable<bool> observable2 = GetExpandedObservable(scrollBar3);
		IObservable<bool> observable3 = null;
		if (observable != null && observable2 != null)
		{
			observable3 = observable.CombineLatest(observable2, (bool h, bool v) => h || v);
		}
		else if (observable != null)
		{
			observable3 = observable;
		}
		else if (observable2 != null)
		{
			observable3 = observable2;
		}
		return observable3?.Subscribe(OnScrollBarExpandedChanged);
		static IObservable<bool>? GetExpandedObservable(ScrollBar? scrollBar)
		{
			return scrollBar?.GetObservable(ScrollBar.IsExpandedProperty);
		}
	}

	private void OnScrollBarExpandedChanged(bool isExpanded)
	{
		IsExpanded = isExpanded;
	}

	private void OnLayoutUpdated(object? sender, EventArgs e)
	{
		RaiseScrollChanged();
	}

	private void RaiseScrollChanged()
	{
		Vector extentDelta = new Vector(Extent.Width - _oldExtent.Width, Extent.Height - _oldExtent.Height);
		Vector offsetDelta = Offset - _oldOffset;
		Vector viewportDelta = new Vector(Viewport.Width - _oldViewport.Width, Viewport.Height - _oldViewport.Height);
		if (!extentDelta.NearlyEquals(default(Vector)) || !offsetDelta.NearlyEquals(default(Vector)) || !viewportDelta.NearlyEquals(default(Vector)))
		{
			ScrollChangedEventArgs e = new ScrollChangedEventArgs(extentDelta, offsetDelta, viewportDelta);
			OnScrollChanged(e);
			_oldExtent = Extent;
			_oldOffset = Offset;
			_oldViewport = Viewport;
		}
	}

	static ScrollViewer()
	{
		Func<AvaloniaObject, Vector, Vector> coerce = CoerceOffset;
		OffsetProperty = AvaloniaProperty.Register<ScrollViewer, Vector>("Offset", default(Vector), inherits: false, BindingMode.OneWay, null, coerce);
		ViewportProperty = AvaloniaProperty.RegisterDirect("Viewport", (ScrollViewer o) => o.Viewport);
		LargeChangeProperty = AvaloniaProperty.RegisterDirect("LargeChange", (ScrollViewer o) => o.LargeChange);
		SmallChangeProperty = AvaloniaProperty.RegisterDirect("SmallChange", (ScrollViewer o) => o.SmallChange);
		ScrollBarMaximumProperty = AvaloniaProperty.RegisterDirect("ScrollBarMaximum", (ScrollViewer o) => o.ScrollBarMaximum);
		HorizontalScrollBarVisibilityProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, ScrollBarVisibility>("HorizontalScrollBarVisibility", ScrollBarVisibility.Disabled);
		HorizontalSnapPointsTypeProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, SnapPointsType>("HorizontalSnapPointsType", SnapPointsType.None);
		VerticalSnapPointsTypeProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, SnapPointsType>("VerticalSnapPointsType", SnapPointsType.None);
		HorizontalSnapPointsAlignmentProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, SnapPointsAlignment>("HorizontalSnapPointsAlignment", SnapPointsAlignment.Near);
		VerticalSnapPointsAlignmentProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, SnapPointsAlignment>("VerticalSnapPointsAlignment", SnapPointsAlignment.Near);
		VerticalScrollBarVisibilityProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, ScrollBarVisibility>("VerticalScrollBarVisibility", ScrollBarVisibility.Auto);
		IsExpandedProperty = ScrollBar.IsExpandedProperty.AddOwner((ScrollViewer o) => o.IsExpanded, null, unsetValue: false);
		AllowAutoHideProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, bool>("AllowAutoHide", defaultValue: true);
		IsScrollChainingEnabledProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, bool>("IsScrollChainingEnabled", defaultValue: true);
		IsScrollInertiaEnabledProperty = AvaloniaProperty.RegisterAttached<ScrollViewer, Control, bool>("IsScrollInertiaEnabled", defaultValue: true);
		ScrollChangedEvent = RoutedEvent.Register<ScrollViewer, ScrollChangedEventArgs>("ScrollChanged", RoutingStrategies.Bubble);
	}
}
