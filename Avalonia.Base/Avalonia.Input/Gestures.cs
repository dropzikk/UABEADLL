using System;
using System.Threading;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Input;

public static class Gestures
{
	private static bool s_isDoubleTapped;

	private static bool s_isHolding;

	private static CancellationTokenSource? s_holdCancellationToken;

	public static readonly AttachedProperty<bool> IsHoldingEnabledProperty;

	public static readonly AttachedProperty<bool> IsHoldWithMouseEnabledProperty;

	public static readonly RoutedEvent<TappedEventArgs> TappedEvent;

	public static readonly RoutedEvent<TappedEventArgs> DoubleTappedEvent;

	public static readonly RoutedEvent<TappedEventArgs> RightTappedEvent;

	public static readonly RoutedEvent<ScrollGestureEventArgs> ScrollGestureEvent;

	public static readonly RoutedEvent<ScrollGestureInertiaStartingEventArgs> ScrollGestureInertiaStartingEvent;

	public static readonly RoutedEvent<ScrollGestureEndedEventArgs> ScrollGestureEndedEvent;

	public static readonly RoutedEvent<PointerDeltaEventArgs> PointerTouchPadGestureMagnifyEvent;

	public static readonly RoutedEvent<PointerDeltaEventArgs> PointerTouchPadGestureRotateEvent;

	public static readonly RoutedEvent<PointerDeltaEventArgs> PointerTouchPadGestureSwipeEvent;

	private static readonly WeakReference<object?> s_lastPress;

	private static Point s_lastPressPoint;

	private static IPointer? s_lastPointer;

	public static readonly RoutedEvent<PinchEventArgs> PinchEvent;

	public static readonly RoutedEvent<PinchEndedEventArgs> PinchEndedEvent;

	public static readonly RoutedEvent<PullGestureEventArgs> PullGestureEvent;

	public static readonly RoutedEvent<HoldingRoutedEventArgs> HoldingEvent;

	public static readonly RoutedEvent<PullGestureEndedEventArgs> PullGestureEndedEvent;

	public static bool GetIsHoldingEnabled(StyledElement element)
	{
		return element.GetValue(IsHoldingEnabledProperty);
	}

	public static void SetIsHoldingEnabled(StyledElement element, bool value)
	{
		element.SetValue(IsHoldingEnabledProperty, value);
	}

	public static bool GetIsHoldWithMouseEnabled(StyledElement element)
	{
		return element.GetValue(IsHoldWithMouseEnabledProperty);
	}

	public static void SetIsHoldWithMouseEnabled(StyledElement element, bool value)
	{
		element.SetValue(IsHoldWithMouseEnabledProperty, value);
	}

	static Gestures()
	{
		s_isDoubleTapped = false;
		IsHoldingEnabledProperty = AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsHoldingEnabled", typeof(Gestures), defaultValue: true);
		IsHoldWithMouseEnabledProperty = AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsHoldWithMouseEnabled", typeof(Gestures), defaultValue: false);
		TappedEvent = RoutedEvent.Register<TappedEventArgs>("Tapped", RoutingStrategies.Bubble, typeof(Gestures));
		DoubleTappedEvent = RoutedEvent.Register<TappedEventArgs>("DoubleTapped", RoutingStrategies.Bubble, typeof(Gestures));
		RightTappedEvent = RoutedEvent.Register<TappedEventArgs>("RightTapped", RoutingStrategies.Bubble, typeof(Gestures));
		ScrollGestureEvent = RoutedEvent.Register<ScrollGestureEventArgs>("ScrollGesture", RoutingStrategies.Bubble, typeof(Gestures));
		ScrollGestureInertiaStartingEvent = RoutedEvent.Register<ScrollGestureInertiaStartingEventArgs>("ScrollGestureInertiaStarting", RoutingStrategies.Bubble, typeof(Gestures));
		ScrollGestureEndedEvent = RoutedEvent.Register<ScrollGestureEndedEventArgs>("ScrollGestureEnded", RoutingStrategies.Bubble, typeof(Gestures));
		PointerTouchPadGestureMagnifyEvent = RoutedEvent.Register<PointerDeltaEventArgs>("PointerMagnifyGesture", RoutingStrategies.Bubble, typeof(Gestures));
		PointerTouchPadGestureRotateEvent = RoutedEvent.Register<PointerDeltaEventArgs>("PointerRotateGesture", RoutingStrategies.Bubble, typeof(Gestures));
		PointerTouchPadGestureSwipeEvent = RoutedEvent.Register<PointerDeltaEventArgs>("PointerSwipeGesture", RoutingStrategies.Bubble, typeof(Gestures));
		s_lastPress = new WeakReference<object>(null);
		PinchEvent = RoutedEvent.Register<PinchEventArgs>("PinchEvent", RoutingStrategies.Bubble, typeof(Gestures));
		PinchEndedEvent = RoutedEvent.Register<PinchEndedEventArgs>("PinchEndedEvent", RoutingStrategies.Bubble, typeof(Gestures));
		PullGestureEvent = RoutedEvent.Register<PullGestureEventArgs>("PullGesture", RoutingStrategies.Bubble, typeof(Gestures));
		HoldingEvent = RoutedEvent.Register<HoldingRoutedEventArgs>("Holding", RoutingStrategies.Bubble, typeof(Gestures));
		PullGestureEndedEvent = RoutedEvent.Register<PullGestureEndedEventArgs>("PullGestureEnded", RoutingStrategies.Bubble, typeof(Gestures));
		InputElement.PointerPressedEvent.RouteFinished.Subscribe(PointerPressed);
		InputElement.PointerReleasedEvent.RouteFinished.Subscribe(PointerReleased);
		InputElement.PointerMovedEvent.RouteFinished.Subscribe(PointerMoved);
	}

	public static void AddTappedHandler(Interactive element, EventHandler<RoutedEventArgs> handler)
	{
		element.AddHandler(TappedEvent, handler);
	}

	public static void AddDoubleTappedHandler(Interactive element, EventHandler<RoutedEventArgs> handler)
	{
		element.AddHandler(DoubleTappedEvent, handler);
	}

	public static void AddRightTappedHandler(Interactive element, EventHandler<RoutedEventArgs> handler)
	{
		element.AddHandler(RightTappedEvent, handler);
	}

	public static void RemoveTappedHandler(Interactive element, EventHandler<RoutedEventArgs> handler)
	{
		element.RemoveHandler(TappedEvent, handler);
	}

	public static void RemoveDoubleTappedHandler(Interactive element, EventHandler<RoutedEventArgs> handler)
	{
		element.RemoveHandler(DoubleTappedEvent, handler);
	}

	public static void RemoveRightTappedHandler(Interactive element, EventHandler<RoutedEventArgs> handler)
	{
		element.RemoveHandler(RightTappedEvent, handler);
	}

	private static void PointerPressed(RoutedEventArgs ev)
	{
		if (ev.Source == null || ev.Route != RoutingStrategies.Bubble)
		{
			return;
		}
		PointerPressedEventArgs e = (PointerPressedEventArgs)ev;
		Visual visual = (Visual)ev.Source;
		if (s_lastPointer != null)
		{
			if (s_isHolding && ev.Source is Interactive interactive)
			{
				interactive.RaiseEvent(new HoldingRoutedEventArgs(HoldingState.Cancelled, s_lastPressPoint, s_lastPointer.Type));
			}
			s_holdCancellationToken?.Cancel();
			s_holdCancellationToken?.Dispose();
			s_holdCancellationToken = null;
			s_lastPointer = null;
		}
		s_isHolding = false;
		object target;
		if (e.ClickCount % 2 == 1)
		{
			s_isDoubleTapped = false;
			s_lastPress.SetTarget(ev.Source);
			s_lastPointer = e.Pointer;
			s_lastPressPoint = e.GetPosition((Visual)ev.Source);
			s_holdCancellationToken = new CancellationTokenSource();
			CancellationToken token = s_holdCancellationToken.Token;
			IPlatformSettings platformSettings = ((IInputRoot)visual.GetVisualRoot())?.PlatformSettings;
			if (platformSettings == null)
			{
				return;
			}
			DispatcherTimer.RunOnce(delegate
			{
				if (!token.IsCancellationRequested && e.Source is InputElement inputElement && GetIsHoldingEnabled(inputElement) && (e.Pointer.Type != 0 || GetIsHoldWithMouseEnabled(inputElement)))
				{
					s_isHolding = true;
					inputElement.RaiseEvent(new HoldingRoutedEventArgs(HoldingState.Started, s_lastPressPoint, s_lastPointer.Type));
				}
			}, platformSettings.HoldWaitDuration);
		}
		else if (e.ClickCount % 2 == 0 && e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed && s_lastPress.TryGetTarget(out target) && target == e.Source && e.Source is Interactive interactive2)
		{
			s_isDoubleTapped = true;
			interactive2.RaiseEvent(new TappedEventArgs(DoubleTappedEvent, e));
		}
	}

	private static void PointerReleased(RoutedEventArgs ev)
	{
		if (ev.Route != RoutingStrategies.Bubble)
		{
			return;
		}
		PointerReleasedEventArgs pointerReleasedEventArgs = (PointerReleasedEventArgs)ev;
		if (s_lastPress.TryGetTarget(out object target) && target == pointerReleasedEventArgs.Source)
		{
			MouseButton initialPressMouseButton = pointerReleasedEventArgs.InitialPressMouseButton;
			if ((initialPressMouseButton == MouseButton.Left || initialPressMouseButton == MouseButton.Right) && pointerReleasedEventArgs.Source is Interactive interactive)
			{
				PointerPoint currentPoint = pointerReleasedEventArgs.GetCurrentPoint((Visual)target);
				Size size = (((IInputRoot)interactive.GetVisualRoot())?.PlatformSettings)?.GetTapSize(currentPoint.Pointer.Type) ?? new Size(4.0, 4.0);
				if (new Rect(s_lastPressPoint, default(Size)).Inflate(new Thickness(size.Width, size.Height)).ContainsExclusive(currentPoint.Position))
				{
					if (s_isHolding)
					{
						s_isHolding = false;
						interactive.RaiseEvent(new HoldingRoutedEventArgs(HoldingState.Completed, s_lastPressPoint, s_lastPointer.Type));
					}
					else if (pointerReleasedEventArgs.InitialPressMouseButton == MouseButton.Right)
					{
						interactive.RaiseEvent(new TappedEventArgs(RightTappedEvent, pointerReleasedEventArgs));
					}
					else if (!s_isDoubleTapped)
					{
						interactive.RaiseEvent(new TappedEventArgs(TappedEvent, pointerReleasedEventArgs));
					}
				}
			}
		}
		s_holdCancellationToken?.Cancel();
		s_holdCancellationToken?.Dispose();
		s_holdCancellationToken = null;
		s_lastPointer = null;
	}

	private static void PointerMoved(RoutedEventArgs ev)
	{
		if (ev.Route != RoutingStrategies.Bubble)
		{
			return;
		}
		PointerEventArgs pointerEventArgs = (PointerEventArgs)ev;
		if (s_lastPress.TryGetTarget(out object target) && pointerEventArgs.Pointer == s_lastPointer && ev.Source is Interactive interactive)
		{
			PointerPoint currentPoint = pointerEventArgs.GetCurrentPoint((Visual)target);
			Size size = (((IInputRoot)interactive.GetVisualRoot())?.PlatformSettings)?.GetTapSize(currentPoint.Pointer.Type) ?? new Size(4.0, 4.0);
			if (new Rect(s_lastPressPoint, default(Size)).Inflate(new Thickness(size.Width, size.Height)).ContainsExclusive(currentPoint.Position))
			{
				return;
			}
			if (s_isHolding)
			{
				interactive.RaiseEvent(new HoldingRoutedEventArgs(HoldingState.Cancelled, s_lastPressPoint, s_lastPointer.Type));
			}
		}
		s_holdCancellationToken?.Cancel();
		s_holdCancellationToken?.Dispose();
		s_holdCancellationToken = null;
		s_isHolding = false;
	}
}
