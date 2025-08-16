using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace Avalonia.Input;

[PrivateApi]
public class TouchDevice : IPointerDevice, IInputDevice, IDisposable
{
	private readonly Dictionary<long, Pointer> _pointers = new Dictionary<long, Pointer>();

	private bool _disposed;

	private int _clickCount;

	private Rect _lastClickRect;

	private ulong _lastClickTime;

	private static RawInputModifiers GetModifiers(RawInputModifiers modifiers, bool isLeftButtonDown)
	{
		RawInputModifiers rawInputModifiers = (modifiers &= RawInputModifiers.KeyboardMask);
		if (isLeftButtonDown)
		{
			rawInputModifiers |= RawInputModifiers.LeftMouseButton;
		}
		return rawInputModifiers;
	}

	public void ProcessRawEvent(RawInputEventArgs ev)
	{
		if (ev.Handled || _disposed)
		{
			return;
		}
		RawPointerEventArgs rawPointerEventArgs = (RawPointerEventArgs)ev;
		if (!_pointers.TryGetValue(rawPointerEventArgs.RawPointerId, out Pointer value))
		{
			if (rawPointerEventArgs.Type == RawPointerEventType.TouchEnd)
			{
				return;
			}
			IInputElement inputHitTestResult = rawPointerEventArgs.InputHitTestResult;
			value = (_pointers[rawPointerEventArgs.RawPointerId] = new Pointer(Pointer.GetNextFreeId(), PointerType.Touch, _pointers.Count == 0));
			value.Capture(inputHitTestResult);
		}
		IInputElement inputElement = value.Captured ?? rawPointerEventArgs.InputHitTestResult ?? rawPointerEventArgs.Root;
		IInputElement inputElement2 = value.CapturedGestureRecognizer?.Target;
		PointerUpdateKind kind = rawPointerEventArgs.Type.ToUpdateKind();
		KeyModifiers modifiers = rawPointerEventArgs.InputModifiers.ToKeyModifiers();
		if (rawPointerEventArgs.Type == RawPointerEventType.TouchBegin)
		{
			if (_pointers.Count > 1)
			{
				_clickCount = 1;
				_lastClickTime = 0uL;
				_lastClickRect = default(Rect);
			}
			else
			{
				IPlatformSettings platformSettings = ((IInputRoot)((inputElement as Interactive)?.GetVisualRoot()))?.PlatformSettings;
				if (platformSettings != null)
				{
					double totalMilliseconds = platformSettings.GetDoubleTapTime(PointerType.Touch).TotalMilliseconds;
					Size doubleTapSize = platformSettings.GetDoubleTapSize(PointerType.Touch);
					if (!_lastClickRect.Contains(rawPointerEventArgs.Position) || (double)(ev.Timestamp - _lastClickTime) > totalMilliseconds)
					{
						_clickCount = 0;
					}
					_clickCount++;
					_lastClickTime = ev.Timestamp;
					_lastClickRect = new Rect(rawPointerEventArgs.Position, default(Size)).Inflate(new Thickness(doubleTapSize.Width / 2.0, doubleTapSize.Height / 2.0));
				}
			}
			inputElement.RaiseEvent(new PointerPressedEventArgs(inputElement, value, (Visual)rawPointerEventArgs.Root, rawPointerEventArgs.Position, ev.Timestamp, new PointerPointProperties(GetModifiers(rawPointerEventArgs.InputModifiers, isLeftButtonDown: true), kind), modifiers, _clickCount));
		}
		if (rawPointerEventArgs.Type == RawPointerEventType.TouchEnd)
		{
			_pointers.Remove(rawPointerEventArgs.RawPointerId);
			using (value)
			{
				inputElement = inputElement2 ?? inputElement;
				PointerReleasedEventArgs e = new PointerReleasedEventArgs(inputElement, value, (Visual)rawPointerEventArgs.Root, rawPointerEventArgs.Position, ev.Timestamp, new PointerPointProperties(GetModifiers(rawPointerEventArgs.InputModifiers, isLeftButtonDown: false), kind), modifiers, MouseButton.Left);
				if (inputElement2 != null)
				{
					value?.CapturedGestureRecognizer?.PointerReleasedInternal(e);
				}
				else
				{
					inputElement.RaiseEvent(e);
				}
			}
		}
		if (rawPointerEventArgs.Type == RawPointerEventType.TouchCancel)
		{
			_pointers.Remove(rawPointerEventArgs.RawPointerId);
			using (value)
			{
				value?.Capture(null);
				value?.CaptureGestureRecognizer(null);
			}
		}
		if (rawPointerEventArgs.Type == RawPointerEventType.TouchUpdate)
		{
			inputElement = inputElement2 ?? inputElement;
			PointerEventArgs e2 = new PointerEventArgs(InputElement.PointerMovedEvent, inputElement, value, (Visual)rawPointerEventArgs.Root, rawPointerEventArgs.Position, ev.Timestamp, new PointerPointProperties(GetModifiers(rawPointerEventArgs.InputModifiers, isLeftButtonDown: true), kind), modifiers, rawPointerEventArgs.IntermediatePoints);
			if (inputElement2 != null)
			{
				value?.CapturedGestureRecognizer?.PointerMovedInternal(e2);
			}
			else
			{
				inputElement.RaiseEvent(e2);
			}
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			Pointer[] array = _pointers.Values.ToArray();
			_pointers.Clear();
			_disposed = true;
			Pointer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Dispose();
			}
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
