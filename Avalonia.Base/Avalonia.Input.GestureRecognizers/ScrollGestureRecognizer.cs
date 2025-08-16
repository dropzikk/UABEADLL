using System;
using System.Diagnostics;
using Avalonia.Threading;

namespace Avalonia.Input.GestureRecognizers;

public class ScrollGestureRecognizer : GestureRecognizer
{
	internal const double InertialScrollSpeedEnd = 5.0;

	public const double InertialResistance = 0.15;

	private bool _canHorizontallyScroll;

	private bool _canVerticallyScroll;

	private bool _isScrollInertiaEnabled;

	private int _scrollStartDistance = 30;

	private bool _scrolling;

	private Point _trackedRootPoint;

	private IPointer? _tracking;

	private int _gestureId;

	private Point _pointerPressedPoint;

	private VelocityTracker? _velocityTracker;

	private Vector _inertia;

	private ulong? _lastMoveTimestamp;

	public static readonly DirectProperty<ScrollGestureRecognizer, bool> CanHorizontallyScrollProperty = AvaloniaProperty.RegisterDirect("CanHorizontallyScroll", (ScrollGestureRecognizer o) => o.CanHorizontallyScroll, delegate(ScrollGestureRecognizer o, bool v)
	{
		o.CanHorizontallyScroll = v;
	}, unsetValue: false);

	public static readonly DirectProperty<ScrollGestureRecognizer, bool> CanVerticallyScrollProperty = AvaloniaProperty.RegisterDirect("CanVerticallyScroll", (ScrollGestureRecognizer o) => o.CanVerticallyScroll, delegate(ScrollGestureRecognizer o, bool v)
	{
		o.CanVerticallyScroll = v;
	}, unsetValue: false);

	public static readonly DirectProperty<ScrollGestureRecognizer, bool> IsScrollInertiaEnabledProperty = AvaloniaProperty.RegisterDirect("IsScrollInertiaEnabled", (ScrollGestureRecognizer o) => o.IsScrollInertiaEnabled, delegate(ScrollGestureRecognizer o, bool v)
	{
		o.IsScrollInertiaEnabled = v;
	}, unsetValue: false);

	public static readonly DirectProperty<ScrollGestureRecognizer, int> ScrollStartDistanceProperty = AvaloniaProperty.RegisterDirect("ScrollStartDistance", (ScrollGestureRecognizer o) => o.ScrollStartDistance, delegate(ScrollGestureRecognizer o, int v)
	{
		o.ScrollStartDistance = v;
	}, 30);

	public bool CanHorizontallyScroll
	{
		get
		{
			return _canHorizontallyScroll;
		}
		set
		{
			SetAndRaise(CanHorizontallyScrollProperty, ref _canHorizontallyScroll, value);
		}
	}

	public bool CanVerticallyScroll
	{
		get
		{
			return _canVerticallyScroll;
		}
		set
		{
			SetAndRaise(CanVerticallyScrollProperty, ref _canVerticallyScroll, value);
		}
	}

	public bool IsScrollInertiaEnabled
	{
		get
		{
			return _isScrollInertiaEnabled;
		}
		set
		{
			SetAndRaise(IsScrollInertiaEnabledProperty, ref _isScrollInertiaEnabled, value);
		}
	}

	public int ScrollStartDistance
	{
		get
		{
			return _scrollStartDistance;
		}
		set
		{
			SetAndRaise(ScrollStartDistanceProperty, ref _scrollStartDistance, value);
		}
	}

	protected override void PointerPressed(PointerPressedEventArgs e)
	{
		if (e.Pointer.IsPrimary && (e.Pointer.Type == PointerType.Touch || e.Pointer.Type == PointerType.Pen))
		{
			EndGesture();
			_tracking = e.Pointer;
			_gestureId = ScrollGestureEventArgs.GetNextFreeId();
			_trackedRootPoint = (_pointerPressedPoint = e.GetPosition((Visual)base.Target));
		}
	}

	protected override void PointerMoved(PointerEventArgs e)
	{
		if (e.Pointer != _tracking)
		{
			return;
		}
		Point position = e.GetPosition((Visual)base.Target);
		if (!_scrolling)
		{
			if (CanHorizontallyScroll && Math.Abs(_trackedRootPoint.X - position.X) > (double)ScrollStartDistance)
			{
				_scrolling = true;
			}
			if (CanVerticallyScroll && Math.Abs(_trackedRootPoint.Y - position.Y) > (double)ScrollStartDistance)
			{
				_scrolling = true;
			}
			if (_scrolling)
			{
				_velocityTracker = new VelocityTracker();
				_trackedRootPoint = new Point(_trackedRootPoint.X - (double)((_trackedRootPoint.X >= position.X) ? ScrollStartDistance : (-ScrollStartDistance)), _trackedRootPoint.Y - (double)((_trackedRootPoint.Y >= position.Y) ? ScrollStartDistance : (-ScrollStartDistance)));
				Capture(e.Pointer);
				e.PreventGestureRecognition();
			}
		}
		if (_scrolling)
		{
			Point point = _trackedRootPoint - position;
			_velocityTracker?.AddPosition(TimeSpan.FromMilliseconds(e.Timestamp), _pointerPressedPoint - position);
			_lastMoveTimestamp = e.Timestamp;
			_trackedRootPoint = position;
			base.Target.RaiseEvent(new ScrollGestureEventArgs(_gestureId, point));
			e.Handled = true;
		}
	}

	protected override void PointerCaptureLost(IPointer pointer)
	{
		if (pointer == _tracking)
		{
			EndGesture();
		}
	}

	private void EndGesture()
	{
		_tracking = null;
		if (_scrolling)
		{
			_inertia = default(Vector);
			_scrolling = false;
			base.Target.RaiseEvent(new ScrollGestureEndedEventArgs(_gestureId));
			_gestureId = 0;
			_lastMoveTimestamp = null;
		}
	}

	protected override void PointerReleased(PointerReleasedEventArgs e)
	{
		if (e.Pointer != _tracking || !_scrolling)
		{
			return;
		}
		_inertia = _velocityTracker?.GetFlingVelocity().PixelsPerSecond ?? Vector.Zero;
		e.Handled = true;
		if (_inertia == default(Vector) || e.Timestamp == 0L || _lastMoveTimestamp == 0 || e.Timestamp - _lastMoveTimestamp > 200 || !IsScrollInertiaEnabled)
		{
			EndGesture();
			return;
		}
		_tracking = null;
		int savedGestureId = _gestureId;
		Stopwatch st = Stopwatch.StartNew();
		TimeSpan lastTime = TimeSpan.Zero;
		base.Target.RaiseEvent(new ScrollGestureInertiaStartingEventArgs(_gestureId, _inertia));
		DispatcherTimer.Run(delegate
		{
			if (_gestureId != savedGestureId)
			{
				return false;
			}
			TimeSpan timeSpan = st.Elapsed - lastTime;
			lastTime = st.Elapsed;
			Vector vector = _inertia * Math.Pow(0.15, st.Elapsed.TotalSeconds);
			Vector delta = vector * timeSpan.TotalSeconds;
			ScrollGestureEventArgs scrollGestureEventArgs = new ScrollGestureEventArgs(_gestureId, delta);
			base.Target.RaiseEvent(scrollGestureEventArgs);
			if (!scrollGestureEventArgs.Handled || scrollGestureEventArgs.ShouldEndScrollGesture)
			{
				EndGesture();
				return false;
			}
			if (CanVerticallyScroll && CanHorizontallyScroll && Math.Abs(vector.X) < 5.0 && Math.Abs(vector.Y) <= 5.0)
			{
				EndGesture();
				return false;
			}
			if (CanVerticallyScroll && Math.Abs(vector.Y) <= 5.0)
			{
				EndGesture();
				return false;
			}
			if (CanHorizontallyScroll && Math.Abs(vector.X) < 5.0)
			{
				EndGesture();
				return false;
			}
			return true;
		}, TimeSpan.FromMilliseconds(16.0), DispatcherPriority.Background);
	}
}
