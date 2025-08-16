using System;
using System.Collections.Generic;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace Avalonia.Input;

[PrivateApi]
public class MouseDevice : IMouseDevice, IPointerDevice, IInputDevice, IDisposable
{
	private int _clickCount;

	private Rect _lastClickRect;

	private ulong _lastClickTime;

	private readonly Pointer _pointer;

	private bool _disposed;

	private MouseButton _lastMouseDownButton;

	public MouseDevice(Pointer? pointer = null)
	{
		_pointer = pointer ?? new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, isPrimary: true);
	}

	public void ProcessRawEvent(RawInputEventArgs e)
	{
		if (!e.Handled && e is RawPointerEventArgs e2)
		{
			ProcessRawEvent(e2);
		}
	}

	private static int ButtonCount(PointerPointProperties props)
	{
		int num = 0;
		if (props.IsLeftButtonPressed)
		{
			num++;
		}
		if (props.IsMiddleButtonPressed)
		{
			num++;
		}
		if (props.IsRightButtonPressed)
		{
			num++;
		}
		if (props.IsXButton1Pressed)
		{
			num++;
		}
		if (props.IsXButton2Pressed)
		{
			num++;
		}
		return num;
	}

	private void ProcessRawEvent(RawPointerEventArgs e)
	{
		e = e ?? throw new ArgumentNullException("e");
		MouseDevice mouseDevice = (MouseDevice)e.Device;
		if (mouseDevice._disposed)
		{
			return;
		}
		PointerPointProperties pointerPointProperties = CreateProperties(e);
		KeyModifiers inputModifiers = e.InputModifiers.ToKeyModifiers();
		switch (e.Type)
		{
		case RawPointerEventType.LeaveWindow:
		case RawPointerEventType.NonClientLeftButtonDown:
			LeaveWindow();
			break;
		case RawPointerEventType.LeftButtonDown:
		case RawPointerEventType.RightButtonDown:
		case RawPointerEventType.MiddleButtonDown:
		case RawPointerEventType.XButton1Down:
		case RawPointerEventType.XButton2Down:
			if (ButtonCount(pointerPointProperties) > 1)
			{
				e.Handled = MouseMove(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, inputModifiers, e.IntermediatePoints, e.InputHitTestResult);
			}
			else
			{
				e.Handled = MouseDown(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, inputModifiers, e.InputHitTestResult);
			}
			break;
		case RawPointerEventType.LeftButtonUp:
		case RawPointerEventType.RightButtonUp:
		case RawPointerEventType.MiddleButtonUp:
		case RawPointerEventType.XButton1Up:
		case RawPointerEventType.XButton2Up:
			if (ButtonCount(pointerPointProperties) != 0)
			{
				e.Handled = MouseMove(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, inputModifiers, e.IntermediatePoints, e.InputHitTestResult);
			}
			else
			{
				e.Handled = MouseUp(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, inputModifiers, e.InputHitTestResult);
			}
			break;
		case RawPointerEventType.Move:
			e.Handled = MouseMove(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, inputModifiers, e.IntermediatePoints, e.InputHitTestResult);
			break;
		case RawPointerEventType.Wheel:
			e.Handled = MouseWheel(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, ((RawMouseWheelEventArgs)e).Delta, inputModifiers, e.InputHitTestResult);
			break;
		case RawPointerEventType.Magnify:
			e.Handled = GestureMagnify(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, ((RawPointerGestureEventArgs)e).Delta, inputModifiers, e.InputHitTestResult);
			break;
		case RawPointerEventType.Rotate:
			e.Handled = GestureRotate(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, ((RawPointerGestureEventArgs)e).Delta, inputModifiers, e.InputHitTestResult);
			break;
		case RawPointerEventType.Swipe:
			e.Handled = GestureSwipe(mouseDevice, e.Timestamp, e.Root, e.Position, pointerPointProperties, ((RawPointerGestureEventArgs)e).Delta, inputModifiers, e.InputHitTestResult);
			break;
		case RawPointerEventType.TouchBegin:
		case RawPointerEventType.TouchUpdate:
		case RawPointerEventType.TouchEnd:
		case RawPointerEventType.TouchCancel:
			break;
		}
	}

	private void LeaveWindow()
	{
	}

	private static PointerPointProperties CreateProperties(RawPointerEventArgs args)
	{
		return new PointerPointProperties(args.InputModifiers, args.Type.ToUpdateKind());
	}

	private bool MouseDown(IMouseDevice device, ulong timestamp, IInputElement root, Point p, PointerPointProperties properties, KeyModifiers inputModifiers, IInputElement? hitTest)
	{
		device = device ?? throw new ArgumentNullException("device");
		root = root ?? throw new ArgumentNullException("root");
		IInputElement inputElement = _pointer.Captured ?? root.InputHitTest(p);
		if (inputElement != null)
		{
			_pointer.Capture(inputElement);
			IPlatformSettings platformSettings = ((IInputRoot)((inputElement as Interactive)?.GetVisualRoot()))?.PlatformSettings;
			if (platformSettings != null)
			{
				double totalMilliseconds = platformSettings.GetDoubleTapTime(PointerType.Mouse).TotalMilliseconds;
				Size doubleTapSize = platformSettings.GetDoubleTapSize(PointerType.Mouse);
				if (!_lastClickRect.Contains(p) || (double)(timestamp - _lastClickTime) > totalMilliseconds)
				{
					_clickCount = 0;
				}
				_clickCount++;
				_lastClickTime = timestamp;
				_lastClickRect = new Rect(p, default(Size)).Inflate(new Thickness(doubleTapSize.Width / 2.0, doubleTapSize.Height / 2.0));
			}
			_lastMouseDownButton = properties.PointerUpdateKind.GetMouseButton();
			PointerPressedEventArgs pointerPressedEventArgs = new PointerPressedEventArgs(inputElement, _pointer, (Visual)root, p, timestamp, properties, inputModifiers, _clickCount);
			inputElement.RaiseEvent(pointerPressedEventArgs);
			return pointerPressedEventArgs.Handled;
		}
		return false;
	}

	private bool MouseMove(IMouseDevice device, ulong timestamp, IInputRoot root, Point p, PointerPointProperties properties, KeyModifiers inputModifiers, Lazy<IReadOnlyList<RawPointerPoint>?>? intermediatePoints, IInputElement? hitTest)
	{
		device = device ?? throw new ArgumentNullException("device");
		root = root ?? throw new ArgumentNullException("root");
		IInputElement inputElement = _pointer.CapturedGestureRecognizer?.Target ?? _pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			PointerEventArgs pointerEventArgs = new PointerEventArgs(InputElement.PointerMovedEvent, inputElement, _pointer, (Visual)root, p, timestamp, properties, inputModifiers, intermediatePoints);
			GestureRecognizer capturedGestureRecognizer = _pointer.CapturedGestureRecognizer;
			if (capturedGestureRecognizer != null)
			{
				capturedGestureRecognizer.PointerMovedInternal(pointerEventArgs);
			}
			else
			{
				inputElement.RaiseEvent(pointerEventArgs);
			}
			return pointerEventArgs.Handled;
		}
		return false;
	}

	private bool MouseUp(IMouseDevice device, ulong timestamp, IInputRoot root, Point p, PointerPointProperties props, KeyModifiers inputModifiers, IInputElement? hitTest)
	{
		device = device ?? throw new ArgumentNullException("device");
		root = root ?? throw new ArgumentNullException("root");
		IInputElement inputElement = _pointer.CapturedGestureRecognizer?.Target ?? _pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			PointerReleasedEventArgs pointerReleasedEventArgs = new PointerReleasedEventArgs(inputElement, _pointer, (Visual)root, p, timestamp, props, inputModifiers, _lastMouseDownButton);
			GestureRecognizer capturedGestureRecognizer = _pointer.CapturedGestureRecognizer;
			if (capturedGestureRecognizer != null)
			{
				capturedGestureRecognizer.PointerReleasedInternal(pointerReleasedEventArgs);
			}
			else
			{
				inputElement?.RaiseEvent(pointerReleasedEventArgs);
			}
			_pointer.Capture(null);
			_pointer.CaptureGestureRecognizer(null);
			_lastMouseDownButton = MouseButton.None;
			return pointerReleasedEventArgs.Handled;
		}
		return false;
	}

	private bool MouseWheel(IMouseDevice device, ulong timestamp, IInputRoot root, Point p, PointerPointProperties props, Vector delta, KeyModifiers inputModifiers, IInputElement? hitTest)
	{
		device = device ?? throw new ArgumentNullException("device");
		root = root ?? throw new ArgumentNullException("root");
		IInputElement inputElement = _pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			PointerWheelEventArgs pointerWheelEventArgs = new PointerWheelEventArgs(inputElement, _pointer, (Visual)root, p, timestamp, props, inputModifiers, delta);
			inputElement?.RaiseEvent(pointerWheelEventArgs);
			return pointerWheelEventArgs.Handled;
		}
		return false;
	}

	private bool GestureMagnify(IMouseDevice device, ulong timestamp, IInputRoot root, Point p, PointerPointProperties props, Vector delta, KeyModifiers inputModifiers, IInputElement? hitTest)
	{
		device = device ?? throw new ArgumentNullException("device");
		root = root ?? throw new ArgumentNullException("root");
		IInputElement inputElement = _pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			PointerDeltaEventArgs pointerDeltaEventArgs = new PointerDeltaEventArgs(Gestures.PointerTouchPadGestureMagnifyEvent, inputElement, _pointer, (Visual)root, p, timestamp, props, inputModifiers, delta);
			inputElement?.RaiseEvent(pointerDeltaEventArgs);
			return pointerDeltaEventArgs.Handled;
		}
		return false;
	}

	private bool GestureRotate(IMouseDevice device, ulong timestamp, IInputRoot root, Point p, PointerPointProperties props, Vector delta, KeyModifiers inputModifiers, IInputElement? hitTest)
	{
		device = device ?? throw new ArgumentNullException("device");
		root = root ?? throw new ArgumentNullException("root");
		IInputElement inputElement = _pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			PointerDeltaEventArgs pointerDeltaEventArgs = new PointerDeltaEventArgs(Gestures.PointerTouchPadGestureRotateEvent, inputElement, _pointer, (Visual)root, p, timestamp, props, inputModifiers, delta);
			inputElement?.RaiseEvent(pointerDeltaEventArgs);
			return pointerDeltaEventArgs.Handled;
		}
		return false;
	}

	private bool GestureSwipe(IMouseDevice device, ulong timestamp, IInputRoot root, Point p, PointerPointProperties props, Vector delta, KeyModifiers inputModifiers, IInputElement? hitTest)
	{
		device = device ?? throw new ArgumentNullException("device");
		root = root ?? throw new ArgumentNullException("root");
		IInputElement inputElement = _pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			PointerDeltaEventArgs pointerDeltaEventArgs = new PointerDeltaEventArgs(Gestures.PointerTouchPadGestureSwipeEvent, inputElement, _pointer, (Visual)root, p, timestamp, props, inputModifiers, delta);
			inputElement?.RaiseEvent(pointerDeltaEventArgs);
			return pointerDeltaEventArgs.Handled;
		}
		return false;
	}

	public void Dispose()
	{
		_disposed = true;
		_pointer?.Dispose();
	}

	public IPointer? TryGetPointer(RawPointerEventArgs ev)
	{
		return _pointer;
	}
}
