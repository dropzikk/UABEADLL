using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace Avalonia.Input;

[PrivateApi]
public class PenDevice : IPenDevice, IPointerDevice, IInputDevice, IDisposable
{
	private readonly Dictionary<long, Pointer> _pointers = new Dictionary<long, Pointer>();

	private int _clickCount;

	private Rect _lastClickRect;

	private ulong _lastClickTime;

	private MouseButton _lastMouseDownButton;

	private bool _disposed;

	public void ProcessRawEvent(RawInputEventArgs e)
	{
		if (!e.Handled && e is RawPointerEventArgs e2)
		{
			ProcessRawEvent(e2);
		}
	}

	private void ProcessRawEvent(RawPointerEventArgs e)
	{
		e = e ?? throw new ArgumentNullException("e");
		if (!_pointers.TryGetValue(e.RawPointerId, out Pointer value))
		{
			if (e.Type == RawPointerEventType.LeftButtonUp || e.Type == RawPointerEventType.TouchEnd)
			{
				return;
			}
			value = (_pointers[e.RawPointerId] = new Pointer(Pointer.GetNextFreeId(), PointerType.Pen, _pointers.Count == 0));
		}
		PointerPointProperties properties = new PointerPointProperties(e.InputModifiers, e.Type.ToUpdateKind(), e.Point.Twist, e.Point.Pressure, e.Point.XTilt, e.Point.YTilt);
		KeyModifiers inputModifiers = e.InputModifiers.ToKeyModifiers();
		bool flag = false;
		switch (e.Type)
		{
		case RawPointerEventType.LeaveWindow:
			flag = true;
			break;
		case RawPointerEventType.LeftButtonDown:
			e.Handled = PenDown(value, e.Timestamp, e.Root, e.Position, properties, inputModifiers, e.InputHitTestResult);
			break;
		case RawPointerEventType.LeftButtonUp:
			e.Handled = PenUp(value, e.Timestamp, e.Root, e.Position, properties, inputModifiers, e.InputHitTestResult);
			break;
		case RawPointerEventType.Move:
			e.Handled = PenMove(value, e.Timestamp, e.Root, e.Position, properties, inputModifiers, e.InputHitTestResult, e.IntermediatePoints);
			break;
		}
		if (flag)
		{
			value.Dispose();
			_pointers.Remove(e.RawPointerId);
		}
	}

	private bool PenDown(Pointer pointer, ulong timestamp, IInputElement root, Point p, PointerPointProperties properties, KeyModifiers inputModifiers, IInputElement? hitTest)
	{
		IInputElement inputElement = pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			pointer.Capture(inputElement);
			IPlatformSettings platformSettings = ((IInputRoot)((inputElement as Interactive)?.GetVisualRoot()))?.PlatformSettings;
			if (platformSettings != null)
			{
				double totalMilliseconds = platformSettings.GetDoubleTapTime(PointerType.Pen).TotalMilliseconds;
				Size doubleTapSize = platformSettings.GetDoubleTapSize(PointerType.Pen);
				if (!_lastClickRect.Contains(p) || (double)(timestamp - _lastClickTime) > totalMilliseconds)
				{
					_clickCount = 0;
				}
				_clickCount++;
				_lastClickTime = timestamp;
				_lastClickRect = new Rect(p, default(Size)).Inflate(new Thickness(doubleTapSize.Width / 2.0, doubleTapSize.Height / 2.0));
			}
			_lastMouseDownButton = properties.PointerUpdateKind.GetMouseButton();
			PointerPressedEventArgs pointerPressedEventArgs = new PointerPressedEventArgs(inputElement, pointer, (Visual)root, p, timestamp, properties, inputModifiers, _clickCount);
			inputElement.RaiseEvent(pointerPressedEventArgs);
			return pointerPressedEventArgs.Handled;
		}
		return false;
	}

	private static bool PenMove(Pointer pointer, ulong timestamp, IInputRoot root, Point p, PointerPointProperties properties, KeyModifiers inputModifiers, IInputElement? hitTest, Lazy<IReadOnlyList<RawPointerPoint>?>? intermediatePoints)
	{
		IInputElement inputElement = pointer.CapturedGestureRecognizer?.Target ?? pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			PointerEventArgs pointerEventArgs = new PointerEventArgs(InputElement.PointerMovedEvent, inputElement, pointer, (Visual)root, p, timestamp, properties, inputModifiers, intermediatePoints);
			GestureRecognizer capturedGestureRecognizer = pointer.CapturedGestureRecognizer;
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

	private bool PenUp(Pointer pointer, ulong timestamp, IInputElement root, Point p, PointerPointProperties properties, KeyModifiers inputModifiers, IInputElement? hitTest)
	{
		IInputElement inputElement = pointer.CapturedGestureRecognizer?.Target ?? pointer.Captured ?? hitTest;
		if (inputElement != null)
		{
			PointerReleasedEventArgs pointerReleasedEventArgs = new PointerReleasedEventArgs(inputElement, pointer, (Visual)root, p, timestamp, properties, inputModifiers, _lastMouseDownButton);
			GestureRecognizer capturedGestureRecognizer = pointer.CapturedGestureRecognizer;
			if (capturedGestureRecognizer != null)
			{
				capturedGestureRecognizer.PointerReleasedInternal(pointerReleasedEventArgs);
			}
			else
			{
				inputElement.RaiseEvent(pointerReleasedEventArgs);
			}
			pointer.Capture(null);
			pointer.CaptureGestureRecognizer(null);
			_lastMouseDownButton = MouseButton.None;
			return pointerReleasedEventArgs.Handled;
		}
		return false;
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}
		List<Pointer> list = _pointers.Values.ToList();
		_pointers.Clear();
		_disposed = true;
		foreach (Pointer item in list)
		{
			item.Dispose();
		}
	}

	public IPointer? TryGetPointer(RawPointerEventArgs ev)
	{
		if (!_pointers.TryGetValue(ev.RawPointerId, out Pointer value))
		{
			return null;
		}
		return value;
	}
}
