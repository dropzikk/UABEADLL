using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Reactive;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Presenters;

public class ScrollContentPresenter : ContentPresenter, IScrollable, IScrollAnchorProvider
{
	private const double EdgeDetectionTolerance = 0.1;

	public static readonly StyledProperty<bool> CanHorizontallyScrollProperty;

	public static readonly StyledProperty<bool> CanVerticallyScrollProperty;

	public static readonly DirectProperty<ScrollContentPresenter, Size> ExtentProperty;

	public static readonly StyledProperty<Vector> OffsetProperty;

	public static readonly DirectProperty<ScrollContentPresenter, Size> ViewportProperty;

	public static readonly StyledProperty<SnapPointsType> HorizontalSnapPointsTypeProperty;

	public static readonly StyledProperty<SnapPointsType> VerticalSnapPointsTypeProperty;

	public static readonly StyledProperty<SnapPointsAlignment> HorizontalSnapPointsAlignmentProperty;

	public static readonly StyledProperty<SnapPointsAlignment> VerticalSnapPointsAlignmentProperty;

	public static readonly StyledProperty<bool> IsScrollChainingEnabledProperty;

	private bool _arranging;

	private Size _extent;

	private IDisposable? _logicalScrollSubscription;

	private Size _viewport;

	private Dictionary<int, Vector>? _activeLogicalGestureScrolls;

	private Dictionary<int, Vector>? _scrollGestureSnapPoints;

	private HashSet<Control>? _anchorCandidates;

	private Control? _anchorElement;

	private Rect _anchorElementBounds;

	private bool _isAnchorElementDirty;

	private bool _areVerticalSnapPointsRegular;

	private bool _areHorizontalSnapPointsRegular;

	private IReadOnlyList<double>? _horizontalSnapPoints;

	private double _horizontalSnapPoint;

	private IReadOnlyList<double>? _verticalSnapPoints;

	private double _verticalSnapPoint;

	private double _verticalSnapPointOffset;

	private double _horizontalSnapPointOffset;

	private CompositeDisposable? _ownerSubscriptions;

	private ScrollViewer? _owner;

	private IScrollSnapPointsInfo? _scrollSnapPointsInfo;

	private bool _isSnapPointsUpdated;

	public bool CanHorizontallyScroll
	{
		get
		{
			return GetValue(CanHorizontallyScrollProperty);
		}
		set
		{
			SetValue(CanHorizontallyScrollProperty, value);
		}
	}

	public bool CanVerticallyScroll
	{
		get
		{
			return GetValue(CanVerticallyScrollProperty);
		}
		set
		{
			SetValue(CanVerticallyScrollProperty, value);
		}
	}

	public Size Extent
	{
		get
		{
			return _extent;
		}
		private set
		{
			SetAndRaise(ExtentProperty, ref _extent, value);
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
		private set
		{
			SetAndRaise(ViewportProperty, ref _viewport, value);
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

	Control? IScrollAnchorProvider.CurrentAnchor
	{
		get
		{
			EnsureAnchorElementSelection();
			return _anchorElement;
		}
	}

	static ScrollContentPresenter()
	{
		CanHorizontallyScrollProperty = AvaloniaProperty.Register<ScrollContentPresenter, bool>("CanHorizontallyScroll", defaultValue: false);
		CanVerticallyScrollProperty = AvaloniaProperty.Register<ScrollContentPresenter, bool>("CanVerticallyScroll", defaultValue: false);
		ExtentProperty = ScrollViewer.ExtentProperty.AddOwner((ScrollContentPresenter o) => o.Extent);
		StyledProperty<Vector> offsetProperty = ScrollViewer.OffsetProperty;
		Func<AvaloniaObject, Vector, Vector> coerce = ScrollViewer.CoerceOffset;
		OffsetProperty = offsetProperty.AddOwner<ScrollContentPresenter>(new StyledPropertyMetadata<Vector>(default(Optional<Vector>), BindingMode.Default, coerce));
		ViewportProperty = ScrollViewer.ViewportProperty.AddOwner((ScrollContentPresenter o) => o.Viewport);
		HorizontalSnapPointsTypeProperty = ScrollViewer.HorizontalSnapPointsTypeProperty.AddOwner<ScrollContentPresenter>();
		VerticalSnapPointsTypeProperty = ScrollViewer.VerticalSnapPointsTypeProperty.AddOwner<ScrollContentPresenter>();
		HorizontalSnapPointsAlignmentProperty = ScrollViewer.HorizontalSnapPointsAlignmentProperty.AddOwner<ScrollContentPresenter>();
		VerticalSnapPointsAlignmentProperty = ScrollViewer.VerticalSnapPointsAlignmentProperty.AddOwner<ScrollContentPresenter>();
		IsScrollChainingEnabledProperty = ScrollViewer.IsScrollChainingEnabledProperty.AddOwner<ScrollContentPresenter>();
		Visual.ClipToBoundsProperty.OverrideDefaultValue(typeof(ScrollContentPresenter), defaultValue: true);
	}

	public ScrollContentPresenter()
	{
		AddHandler(Control.RequestBringIntoViewEvent, BringIntoViewRequested);
		AddHandler(Gestures.ScrollGestureEvent, OnScrollGesture);
		AddHandler(Gestures.ScrollGestureEndedEvent, OnScrollGestureEnded);
		AddHandler(Gestures.ScrollGestureInertiaStartingEvent, OnScrollGestureInertiaStartingEnded);
		this.GetObservable(ContentPresenter.ChildProperty).Subscribe(UpdateScrollableSubscription);
	}

	public bool BringDescendantIntoView(Visual target, Rect targetRect)
	{
		Control? child = base.Child;
		if (child == null || !child.IsEffectivelyVisible)
		{
			return false;
		}
		ILogicalScrollable logicalScrollable = base.Child as ILogicalScrollable;
		Control control = target as Control;
		if (logicalScrollable != null && logicalScrollable.IsLogicalScrollEnabled && control != null)
		{
			return logicalScrollable.BringIntoView(control, targetRect);
		}
		Matrix? matrix = target.TransformToVisual(base.Child);
		if (!matrix.HasValue)
		{
			return false;
		}
		Rect rect = targetRect.TransformToAABB(matrix.Value);
		Vector value = Offset;
		bool flag = false;
		if (rect.Bottom > value.Y + Viewport.Height)
		{
			value = value.WithY(rect.Bottom - Viewport.Height + base.Child.Margin.Top);
			flag = true;
		}
		if (rect.Y < value.Y)
		{
			value = value.WithY(rect.Y);
			flag = true;
		}
		if (rect.Right > value.X + Viewport.Width)
		{
			value = value.WithX(rect.Right - Viewport.Width + base.Child.Margin.Left);
			flag = true;
		}
		if (rect.X < value.X)
		{
			value = value.WithX(rect.X);
			flag = true;
		}
		if (flag)
		{
			SetCurrentValue(OffsetProperty, value);
		}
		return flag;
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
			IDisposable[] disposables = new IDisposable[5]
			{
				IfUnset<StyledProperty<bool>>(CanHorizontallyScrollProperty, (StyledProperty<bool> p) => Bind(p, owner.GetObservable(ScrollViewer.HorizontalScrollBarVisibilityProperty, NotDisabled), BindingPriority.Template)),
				IfUnset<StyledProperty<bool>>(CanVerticallyScrollProperty, (StyledProperty<bool> p) => Bind(p, owner.GetObservable(ScrollViewer.VerticalScrollBarVisibilityProperty, NotDisabled), BindingPriority.Template)),
				IfUnset<StyledProperty<Vector>>(OffsetProperty, (StyledProperty<Vector> p) => Bind(p, owner.GetBindingObservable(ScrollViewer.OffsetProperty), BindingPriority.Template)),
				IfUnset<StyledProperty<bool>>(IsScrollChainingEnabledProperty, (StyledProperty<bool> p) => Bind(p, owner.GetBindingObservable(ScrollViewer.IsScrollChainingEnabledProperty), BindingPriority.Template)),
				IfUnset<StyledProperty<object>>(ContentPresenter.ContentProperty, (StyledProperty<object> p) => Bind<object>(p, owner.GetBindingObservable(ContentPresenter.ContentProperty), BindingPriority.Template))
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
		static bool NotDisabled(ScrollBarVisibility v)
		{
			return v != ScrollBarVisibility.Disabled;
		}
	}

	void IScrollAnchorProvider.RegisterAnchorCandidate(Control element)
	{
		if (!this.IsVisualAncestorOf(element))
		{
			throw new InvalidOperationException("An anchor control must be a visual descendent of the ScrollContentPresenter.");
		}
		if (_anchorCandidates == null)
		{
			_anchorCandidates = new HashSet<Control>();
		}
		_anchorCandidates.Add(element);
		_isAnchorElementDirty = true;
	}

	void IScrollAnchorProvider.UnregisterAnchorCandidate(Control element)
	{
		_anchorCandidates?.Remove(element);
		_isAnchorElementDirty = true;
		if (_anchorElement == element)
		{
			_anchorElement = null;
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (_logicalScrollSubscription != null || base.Child == null)
		{
			return base.MeasureOverride(availableSize);
		}
		Size availableSize2 = new Size(CanHorizontallyScroll ? double.PositiveInfinity : availableSize.Width, CanVerticallyScroll ? double.PositiveInfinity : availableSize.Height);
		base.Child.Measure(availableSize2);
		if (!_isSnapPointsUpdated)
		{
			_isSnapPointsUpdated = true;
			UpdateSnapPoints();
		}
		return base.Child.DesiredSize.Constrain(availableSize);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (_logicalScrollSubscription != null || base.Child == null)
		{
			return base.ArrangeOverride(finalSize);
		}
		return ArrangeWithAnchoring(finalSize);
	}

	private Size ArrangeWithAnchoring(Size finalSize)
	{
		Size finalSize2 = new Size(CanHorizontallyScroll ? Math.Max(base.Child.DesiredSize.Width, finalSize.Width) : finalSize.Width, CanVerticallyScroll ? Math.Max(base.Child.DesiredSize.Height, finalSize.Height) : finalSize.Height);
		if (Offset.X >= 0.1 || Offset.Y >= 0.1)
		{
			EnsureAnchorElementSelection();
			ArrangeOverrideImpl(finalSize2, -Offset);
			Vector vector = TrackAnchor();
			if (vector != default(Vector))
			{
				Vector value = Offset + vector;
				Size extent = Extent;
				Vector vector2 = new Vector(Extent.Width - Viewport.Width, Extent.Height - Viewport.Height);
				if (value.X > vector2.X)
				{
					extent = extent.WithWidth(value.X + Viewport.Width);
				}
				if (value.Y > vector2.Y)
				{
					extent = extent.WithHeight(value.Y + Viewport.Height);
				}
				Extent = extent;
				try
				{
					_arranging = true;
					SetCurrentValue(OffsetProperty, value);
				}
				finally
				{
					_arranging = false;
				}
				ArrangeOverrideImpl(finalSize2, -Offset);
			}
		}
		else
		{
			ArrangeOverrideImpl(finalSize2, -Offset);
		}
		Viewport = finalSize;
		Extent = ComputeExtent(finalSize);
		_isAnchorElementDirty = true;
		return finalSize;
		Vector TrackAnchor()
		{
			if (_anchorElement != null && TranslateBounds(_anchorElement, base.Child, out var bounds) && bounds.Position != _anchorElementBounds.Position)
			{
				return bounds.Position - _anchorElementBounds.Position;
			}
			return default(Vector);
		}
	}

	private Size ComputeExtent(Size viewportSize)
	{
		Thickness thickness = base.Child.Margin;
		if (base.Child.UseLayoutRounding)
		{
			double layoutScale = LayoutHelper.GetLayoutScale(base.Child);
			thickness = LayoutHelper.RoundLayoutThickness(thickness, layoutScale, layoutScale);
		}
		Size result = base.Child.Bounds.Size.Inflate(thickness);
		if (MathUtilities.AreClose(result.Width, viewportSize.Width, LayoutHelper.LayoutEpsilon))
		{
			result = result.WithWidth(viewportSize.Width);
		}
		if (MathUtilities.AreClose(result.Height, viewportSize.Height, LayoutHelper.LayoutEpsilon))
		{
			result = result.WithHeight(viewportSize.Height);
		}
		return result;
	}

	private void OnScrollGesture(object? sender, ScrollGestureEventArgs e)
	{
		if (!(Extent.Height > Viewport.Height) && !(Extent.Width > Viewport.Width))
		{
			return;
		}
		ILogicalScrollable logicalScrollable = base.Child as ILogicalScrollable;
		bool flag = logicalScrollable?.IsLogicalScrollEnabled ?? false;
		Vector vector = new Vector(1.0, 1.0);
		double num = Offset.X;
		double num2 = Offset.Y;
		Vector value = default(Vector);
		if (flag)
		{
			_activeLogicalGestureScrolls?.TryGetValue(e.Id, out value);
		}
		value += e.Delta;
		if (flag && logicalScrollable != null)
		{
			vector = base.Bounds.Size / logicalScrollable.Viewport;
		}
		if (Extent.Height > Viewport.Height)
		{
			double num4;
			if (flag)
			{
				double num3 = value.Y / vector.Y;
				value = value.WithY(value.Y - num3 * vector.Y);
				num4 = num3;
			}
			else
			{
				num4 = value.Y;
			}
			num2 += num4;
			num2 = Math.Max(num2, 0.0);
			num2 = Math.Min(num2, Extent.Height - Viewport.Height);
		}
		if (Extent.Width > Viewport.Width)
		{
			double num6;
			if (flag)
			{
				double num5 = value.X / vector.X;
				value = value.WithX(value.X - num5 * vector.X);
				num6 = num5;
			}
			else
			{
				num6 = value.X;
			}
			num += num6;
			num = Math.Max(num, 0.0);
			num = Math.Min(num, Extent.Width - Viewport.Width);
		}
		if (flag)
		{
			if (_activeLogicalGestureScrolls == null)
			{
				_activeLogicalGestureScrolls = new Dictionary<int, Vector>();
			}
			_activeLogicalGestureScrolls[e.Id] = value;
		}
		Vector vector2 = new Vector(num, num2);
		Dictionary<int, Vector>? scrollGestureSnapPoints = _scrollGestureSnapPoints;
		if (scrollGestureSnapPoints != null && scrollGestureSnapPoints.TryGetValue(e.Id, out var value2))
		{
			double x = num;
			double y = num2;
			if (HorizontalSnapPointsType != 0)
			{
				x = ((value.X < 0.0) ? Math.Max(value2.X, vector2.X) : Math.Min(value2.X, vector2.X));
			}
			if (VerticalSnapPointsType != 0)
			{
				y = ((value.Y < 0.0) ? Math.Max(value2.Y, vector2.Y) : Math.Min(value2.Y, vector2.Y));
			}
			vector2 = new Vector(x, y);
		}
		bool flag2 = vector2 != Offset;
		SetCurrentValue(OffsetProperty, vector2);
		e.Handled = !IsScrollChainingEnabled || flag2;
		e.ShouldEndScrollGesture = !IsScrollChainingEnabled && !flag2;
	}

	private void OnScrollGestureEnded(object? sender, ScrollGestureEndedEventArgs e)
	{
		_activeLogicalGestureScrolls?.Remove(e.Id);
		_scrollGestureSnapPoints?.Remove(e.Id);
		SetCurrentValue(OffsetProperty, SnapOffset(Offset));
	}

	private void OnScrollGestureInertiaStartingEnded(object? sender, ScrollGestureInertiaStartingEventArgs e)
	{
		object obj = base.Content;
		if (base.Content is ItemsControl itemsControl)
		{
			obj = itemsControl.Presenter?.Panel;
		}
		if (!(obj is IScrollSnapPointsInfo))
		{
			return;
		}
		if (_scrollGestureSnapPoints == null)
		{
			_scrollGestureSnapPoints = new Dictionary<int, Vector>();
		}
		Vector offset = Offset;
		if (HorizontalSnapPointsType == SnapPointsType.None || VerticalSnapPointsType == SnapPointsType.None)
		{
			double num = 0.0;
			double num2 = 0.0;
			if (HorizontalSnapPointsType != 0)
			{
				num = ((HorizontalSnapPointsType == SnapPointsType.Mandatory) ? GetDistance(e.Inertia.X) : 0.0);
			}
			if (VerticalSnapPointsType != 0)
			{
				num2 = ((VerticalSnapPointsType == SnapPointsType.Mandatory) ? GetDistance(e.Inertia.Y) : 0.0);
			}
			offset = new Vector(offset.X + num, offset.Y + num2);
			_scrollGestureSnapPoints.Add(e.Id, SnapOffset(offset));
		}
		static double GetDistance(double speed)
		{
			double num3 = Math.Log(5.0 / Math.Abs(speed)) / Math.Log(0.15);
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			while (num4 <= num3)
			{
				double num7 = speed * Math.Pow(0.15, num4);
				num5 += num7 * num6;
				num4 += 0.01600000075995922;
				num6 = 0.01600000075995922;
			}
			return num5;
		}
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		if (Extent.Height > Viewport.Height || Extent.Width > Viewport.Width)
		{
			ILogicalScrollable logicalScrollable = base.Child as ILogicalScrollable;
			bool flag = logicalScrollable?.IsLogicalScrollEnabled ?? false;
			double num = Offset.X;
			double num2 = Offset.Y;
			Vector vector = e.Delta;
			if (e.KeyModifiers == KeyModifiers.Shift && MathUtilities.IsZero(vector.X))
			{
				vector = new Vector(vector.Y, vector.X);
			}
			if (Extent.Height > Viewport.Height)
			{
				double num3 = (flag ? logicalScrollable.ScrollSize.Height : 50.0);
				num2 += (0.0 - vector.Y) * num3;
				num2 = Math.Max(num2, 0.0);
				num2 = Math.Min(num2, Extent.Height - Viewport.Height);
			}
			if (Extent.Width > Viewport.Width)
			{
				double num4 = (flag ? logicalScrollable.ScrollSize.Width : 50.0);
				num += (0.0 - vector.X) * num4;
				num = Math.Max(num, 0.0);
				num = Math.Min(num, Extent.Width - Viewport.Width);
			}
			Vector vector2 = SnapOffset(new Vector(num, num2));
			bool flag2 = vector2 != Offset;
			SetCurrentValue(OffsetProperty, vector2);
			e.Handled = !IsScrollChainingEnabled || flag2;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == OffsetProperty)
		{
			if (!_arranging)
			{
				InvalidateArrange();
			}
			_owner?.SetCurrentValue(OffsetProperty, change.GetNewValue<Vector>());
		}
		else if (change.Property == ContentPresenter.ChildProperty)
		{
			ChildChanged(change);
		}
		else if (change.Property == HorizontalSnapPointsAlignmentProperty || change.Property == VerticalSnapPointsAlignmentProperty)
		{
			UpdateSnapPoints();
		}
		else if (change.Property == ExtentProperty)
		{
			if (_owner != null)
			{
				_owner.Extent = change.GetNewValue<Size>();
			}
			CoerceValue(OffsetProperty);
		}
		else if (change.Property == ViewportProperty)
		{
			if (_owner != null)
			{
				_owner.Viewport = change.GetNewValue<Size>();
			}
			CoerceValue(OffsetProperty);
		}
		base.OnPropertyChanged(change);
	}

	private void ScrollSnapPointsInfoSnapPointsChanged(object? sender, RoutedEventArgs e)
	{
		UpdateSnapPoints();
	}

	private void BringIntoViewRequested(object? sender, RequestBringIntoViewEventArgs e)
	{
		if (e.TargetObject != null)
		{
			e.Handled = BringDescendantIntoView(e.TargetObject, e.TargetRect);
		}
	}

	private void ChildChanged(AvaloniaPropertyChangedEventArgs e)
	{
		UpdateScrollableSubscription((Control)e.NewValue);
		if (e.OldValue != null)
		{
			SetCurrentValue(OffsetProperty, default(Vector));
		}
	}

	private void UpdateScrollableSubscription(Control? child)
	{
		ILogicalScrollable scrollable = child as ILogicalScrollable;
		_logicalScrollSubscription?.Dispose();
		_logicalScrollSubscription = null;
		if (scrollable == null)
		{
			return;
		}
		scrollable.ScrollInvalidated += ScrollInvalidated;
		if (scrollable.IsLogicalScrollEnabled)
		{
			_logicalScrollSubscription = new CompositeDisposable(this.GetObservable(CanHorizontallyScrollProperty).Subscribe(delegate(bool x)
			{
				scrollable.CanHorizontallyScroll = x;
			}), this.GetObservable(CanVerticallyScrollProperty).Subscribe(delegate(bool x)
			{
				scrollable.CanVerticallyScroll = x;
			}), this.GetObservable(OffsetProperty).Skip(1).Subscribe(delegate(Vector x)
			{
				scrollable.Offset = x;
			}), Disposable.Create(delegate
			{
				scrollable.ScrollInvalidated -= ScrollInvalidated;
			}));
			UpdateFromScrollable(scrollable);
		}
	}

	private void ScrollInvalidated(object? sender, EventArgs e)
	{
		UpdateFromScrollable((ILogicalScrollable)sender);
	}

	private void UpdateFromScrollable(ILogicalScrollable scrollable)
	{
		if (_logicalScrollSubscription != null != scrollable.IsLogicalScrollEnabled)
		{
			UpdateScrollableSubscription(base.Child);
			SetCurrentValue(OffsetProperty, default(Vector));
			InvalidateMeasure();
		}
		else if (scrollable.IsLogicalScrollEnabled)
		{
			Viewport = scrollable.Viewport;
			Extent = scrollable.Extent;
			SetCurrentValue(OffsetProperty, scrollable.Offset);
		}
	}

	private void EnsureAnchorElementSelection()
	{
		if (!_isAnchorElementDirty || _anchorCandidates == null)
		{
			return;
		}
		_anchorElement = null;
		_anchorElementBounds = default(Rect);
		_isAnchorElementDirty = false;
		Control control = null;
		double num = double.MaxValue;
		foreach (Control anchorCandidate in _anchorCandidates)
		{
			if (anchorCandidate.IsVisible && GetViewportBounds(anchorCandidate, out var bounds))
			{
				double num2 = Math.Abs(((Vector)bounds.Position).Length);
				if (num2 < num)
				{
					control = anchorCandidate;
					num = num2;
				}
			}
		}
		if (control != null)
		{
			Rect anchorElementBounds = TranslateBounds(control, base.Child);
			_anchorElement = control;
			_anchorElementBounds = anchorElementBounds;
		}
	}

	private bool GetViewportBounds(Control element, out Rect bounds)
	{
		if (TranslateBounds(element, base.Child, out var bounds2))
		{
			Rect rect = new Rect(base.Bounds.Size);
			bounds = new Rect(bounds2.Position - Offset, bounds2.Size);
			return bounds.Intersects(rect);
		}
		bounds = default(Rect);
		return false;
	}

	private Rect TranslateBounds(Control control, Control to)
	{
		if (TranslateBounds(control, to, out var bounds))
		{
			return bounds;
		}
		throw new InvalidOperationException("The control's bounds could not be translated to the requested control.");
	}

	private bool TranslateBounds(Control control, Control to, out Rect bounds)
	{
		if (!control.IsVisible)
		{
			bounds = default(Rect);
			return false;
		}
		Point? point = control.TranslatePoint(default(Point), to);
		bounds = (point.HasValue ? new Rect(point.Value, control.Bounds.Size) : default(Rect));
		return point.HasValue;
	}

	private void UpdateSnapPoints()
	{
		IScrollSnapPointsInfo scrollSnapPointsInfo = GetScrollSnapPointsInfo(base.Content);
		if (scrollSnapPointsInfo != null)
		{
			IScrollSnapPointsInfo scrollSnapPointsInfo2 = scrollSnapPointsInfo;
			_areVerticalSnapPointsRegular = scrollSnapPointsInfo2.AreVerticalSnapPointsRegular;
			_areHorizontalSnapPointsRegular = scrollSnapPointsInfo2.AreHorizontalSnapPointsRegular;
			if (!_areVerticalSnapPointsRegular)
			{
				_verticalSnapPoints = scrollSnapPointsInfo2.GetIrregularSnapPoints(Orientation.Vertical, VerticalSnapPointsAlignment);
			}
			else
			{
				_verticalSnapPoints = new List<double>();
				_verticalSnapPoint = scrollSnapPointsInfo2.GetRegularSnapPoints(Orientation.Vertical, VerticalSnapPointsAlignment, out _verticalSnapPointOffset);
			}
			if (!_areHorizontalSnapPointsRegular)
			{
				_horizontalSnapPoints = scrollSnapPointsInfo2.GetIrregularSnapPoints(Orientation.Horizontal, HorizontalSnapPointsAlignment);
				return;
			}
			_horizontalSnapPoints = new List<double>();
			_horizontalSnapPoint = scrollSnapPointsInfo2.GetRegularSnapPoints(Orientation.Vertical, VerticalSnapPointsAlignment, out _horizontalSnapPointOffset);
		}
		else
		{
			_horizontalSnapPoints = new List<double>();
			_verticalSnapPoints = new List<double>();
		}
	}

	private Vector SnapOffset(Vector offset)
	{
		if (GetScrollSnapPointsInfo(base.Content) == null)
		{
			return offset;
		}
		Vector vector = GetAlignedDiff();
		if (VerticalSnapPointsType != 0)
		{
			offset = new Vector(offset.X, offset.Y + vector.Y);
			double num = offset.Y;
			if (_areVerticalSnapPointsRegular)
			{
				double num2 = (double)(int)(offset.Y / _verticalSnapPoint) * _verticalSnapPoint + _verticalSnapPointOffset;
				double num3 = num2 + _verticalSnapPoint;
				double num4 = (num2 + num3) / 2.0;
				num = ((offset.Y < num4) ? num2 : num3);
			}
			else if (_verticalSnapPoints != null && _verticalSnapPoints.Count > 0)
			{
				double lowerSnapPoint;
				double num5 = FindNearestSnapPoint(_verticalSnapPoints, offset.Y, out lowerSnapPoint);
				double num6 = (lowerSnapPoint + num5) / 2.0;
				num = ((offset.Y < num6) ? lowerSnapPoint : num5);
			}
			offset = new Vector(offset.X, num - vector.Y);
		}
		if (HorizontalSnapPointsType != 0)
		{
			offset = new Vector(offset.X + vector.X, offset.Y);
			double num7 = offset.X;
			if (_areHorizontalSnapPointsRegular)
			{
				double num8 = (double)(int)(offset.X / _horizontalSnapPoint) * _horizontalSnapPoint + _horizontalSnapPointOffset;
				double num9 = num8 + _horizontalSnapPoint;
				double num10 = (num8 + num9) / 2.0;
				num7 = ((offset.X < num10) ? num8 : num9);
			}
			else if (_horizontalSnapPoints != null && _horizontalSnapPoints.Count > 0)
			{
				double lowerSnapPoint2;
				double num11 = FindNearestSnapPoint(_horizontalSnapPoints, offset.X, out lowerSnapPoint2);
				double num12 = (lowerSnapPoint2 + num11) / 2.0;
				num7 = ((offset.X < num12) ? lowerSnapPoint2 : num11);
			}
			offset = new Vector(num7 - vector.X, offset.Y);
		}
		return offset;
		Vector GetAlignedDiff()
		{
			Vector vector2 = offset;
			switch (VerticalSnapPointsAlignment)
			{
			case SnapPointsAlignment.Center:
				vector2 += new Vector(0.0, Viewport.Height / 2.0);
				break;
			case SnapPointsAlignment.Far:
				vector2 += new Vector(0.0, Viewport.Height);
				break;
			}
			switch (HorizontalSnapPointsAlignment)
			{
			case SnapPointsAlignment.Center:
				vector2 += new Vector(Viewport.Width / 2.0, 0.0);
				break;
			case SnapPointsAlignment.Far:
				vector2 += new Vector(Viewport.Width, 0.0);
				break;
			}
			return vector2 - offset;
		}
	}

	private static double FindNearestSnapPoint(IReadOnlyList<double> snapPoints, double value, out double lowerSnapPoint)
	{
		int num = snapPoints.BinarySearch(value, Comparer<double>.Default);
		if (num < 0)
		{
			num = ~num;
			lowerSnapPoint = snapPoints[Math.Max(0, num - 1)];
		}
		else
		{
			lowerSnapPoint = snapPoints[num];
			num++;
		}
		return snapPoints[Math.Min(num, snapPoints.Count - 1)];
	}

	private IScrollSnapPointsInfo? GetScrollSnapPointsInfo(object? content)
	{
		object obj = content;
		if (base.Content is ItemsControl itemsControl)
		{
			obj = itemsControl.Presenter?.Panel;
		}
		if (base.Content is ItemsPresenter itemsPresenter)
		{
			obj = itemsPresenter.Panel;
		}
		IScrollSnapPointsInfo scrollSnapPointsInfo = obj as IScrollSnapPointsInfo;
		if (scrollSnapPointsInfo != _scrollSnapPointsInfo)
		{
			if (_scrollSnapPointsInfo != null)
			{
				_scrollSnapPointsInfo.VerticalSnapPointsChanged -= ScrollSnapPointsInfoSnapPointsChanged;
				_scrollSnapPointsInfo.HorizontalSnapPointsChanged -= ScrollSnapPointsInfoSnapPointsChanged;
			}
			_scrollSnapPointsInfo = scrollSnapPointsInfo;
			if (_scrollSnapPointsInfo != null)
			{
				_scrollSnapPointsInfo.VerticalSnapPointsChanged += ScrollSnapPointsInfoSnapPointsChanged;
				_scrollSnapPointsInfo.HorizontalSnapPointsChanged += ScrollSnapPointsInfoSnapPointsChanged;
			}
		}
		return scrollSnapPointsInfo;
	}
}
