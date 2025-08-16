using System;
using Avalonia.Input.Raw;

namespace Avalonia.Input;

internal class PointerOverPreProcessor : IObserver<RawInputEventArgs>
{
	private IPointerDevice? _lastActivePointerDevice;

	private (IPointer pointer, PixelPoint position)? _currentPointer;

	private PixelPoint? _lastKnownPosition;

	private readonly IInputRoot _inputRoot;

	public PixelPoint? LastPosition => _lastKnownPosition;

	public PointerOverPreProcessor(IInputRoot inputRoot)
	{
		_inputRoot = inputRoot ?? throw new ArgumentNullException("inputRoot");
	}

	public void OnCompleted()
	{
		ClearPointerOver();
	}

	public void OnError(Exception error)
	{
	}

	public void OnNext(RawInputEventArgs value)
	{
		if (!(value is RawPointerEventArgs rawPointerEventArgs) || rawPointerEventArgs.Root != _inputRoot || !(value.Device is IPointerDevice pointerDevice))
		{
			return;
		}
		if (pointerDevice != _lastActivePointerDevice)
		{
			ClearPointerOver();
			_lastActivePointerDevice = pointerDevice;
		}
		RawPointerEventType type = rawPointerEventArgs.Type;
		if (type == RawPointerEventType.LeaveWindow || type == RawPointerEventType.NonClientLeftButtonDown)
		{
			(IPointer, PixelPoint)? currentPointer = _currentPointer;
			if (currentPointer.HasValue)
			{
				(IPointer, PixelPoint) valueOrDefault = currentPointer.GetValueOrDefault();
				IPointer item = valueOrDefault.Item1;
				PixelPoint item2 = valueOrDefault.Item2;
				_currentPointer = null;
				ClearPointerOver(item, rawPointerEventArgs.Root, 0uL, PointToClient(rawPointerEventArgs.Root, item2), new PointerPointProperties(rawPointerEventArgs.InputModifiers, rawPointerEventArgs.Type.ToUpdateKind()), rawPointerEventArgs.InputModifiers.ToKeyModifiers());
				return;
			}
		}
		IPointer pointer = pointerDevice.TryGetPointer(rawPointerEventArgs);
		if (pointer != null && pointer.Type != PointerType.Touch)
		{
			IInputElement element = pointer.Captured ?? rawPointerEventArgs.InputHitTestResult;
			SetPointerOver(pointer, rawPointerEventArgs.Root, element, rawPointerEventArgs.Timestamp, rawPointerEventArgs.Position, new PointerPointProperties(rawPointerEventArgs.InputModifiers, rawPointerEventArgs.Type.ToUpdateKind()), rawPointerEventArgs.InputModifiers.ToKeyModifiers());
		}
	}

	public void SceneInvalidated(Rect dirtyRect)
	{
		(IPointer, PixelPoint)? currentPointer = _currentPointer;
		if (currentPointer.HasValue)
		{
			(IPointer, PixelPoint) valueOrDefault = currentPointer.GetValueOrDefault();
			IPointer item = valueOrDefault.Item1;
			PixelPoint item2 = valueOrDefault.Item2;
			Point point = PointToClient(_inputRoot, item2);
			if (dirtyRect.Contains(point))
			{
				IInputElement element = item.Captured ?? _inputRoot.InputHitTest(point);
				SetPointerOver(item, _inputRoot, element, 0uL, point, PointerPointProperties.None, KeyModifiers.None);
			}
			else if (!((Visual)_inputRoot).Bounds.Contains(point))
			{
				ClearPointerOver(item, _inputRoot, 0uL, point, PointerPointProperties.None, KeyModifiers.None);
			}
		}
	}

	private void ClearPointerOver()
	{
		(IPointer, PixelPoint)? currentPointer = _currentPointer;
		if (currentPointer.HasValue)
		{
			(IPointer, PixelPoint) valueOrDefault = currentPointer.GetValueOrDefault();
			IPointer item = valueOrDefault.Item1;
			PixelPoint item2 = valueOrDefault.Item2;
			Point value = PointToClient(_inputRoot, item2);
			ClearPointerOver(item, _inputRoot, 0uL, value, PointerPointProperties.None, KeyModifiers.None);
		}
		_currentPointer = null;
		_lastActivePointerDevice = null;
	}

	private void ClearPointerOver(IPointer pointer, IInputRoot root, ulong timestamp, Point? position, PointerPointProperties properties, KeyModifiers inputModifiers)
	{
		IInputElement inputElement = root.PointerOverElement;
		if (inputElement != null)
		{
			PointerEventArgs pointerEventArgs = new PointerEventArgs(InputElement.PointerExitedEvent, inputElement, pointer, position.HasValue ? (root as Visual) : null, position.HasValue ? position.Value : default(Point), timestamp, properties, inputModifiers);
			if (inputElement is Visual { IsAttachedToVisualTree: false } && root.IsPointerOver)
			{
				ClearChildrenPointerOver(pointerEventArgs, root, clearRoot: true);
			}
			while (inputElement != null)
			{
				pointerEventArgs.Source = inputElement;
				pointerEventArgs.Handled = false;
				inputElement.RaiseEvent(pointerEventArgs);
				inputElement = GetVisualParent(inputElement);
			}
			root.PointerOverElement = null;
			_lastActivePointerDevice = null;
			_currentPointer = null;
		}
	}

	private void ClearChildrenPointerOver(PointerEventArgs e, IInputElement element, bool clearRoot)
	{
		if (element is Visual visual)
		{
			foreach (Visual visualChild in visual.VisualChildren)
			{
				if (visualChild is IInputElement { IsPointerOver: not false } inputElement)
				{
					ClearChildrenPointerOver(e, inputElement, clearRoot: true);
					break;
				}
			}
		}
		if (clearRoot)
		{
			e.Source = element;
			e.Handled = false;
			element.RaiseEvent(e);
		}
	}

	private void SetPointerOver(IPointer pointer, IInputRoot root, IInputElement? element, ulong timestamp, Point position, PointerPointProperties properties, KeyModifiers inputModifiers)
	{
		IInputElement pointerOverElement = root.PointerOverElement;
		PixelPoint pixelPoint = ((Visual)root).PointToScreen(position);
		_lastKnownPosition = pixelPoint;
		if (element != pointerOverElement)
		{
			if (element != null)
			{
				SetPointerOverToElement(pointer, root, element, timestamp, position, properties, inputModifiers);
			}
			else
			{
				ClearPointerOver(pointer, root, timestamp, position, properties, inputModifiers);
			}
		}
		_currentPointer = (pointer, pixelPoint);
	}

	private void SetPointerOverToElement(IPointer pointer, IInputRoot root, IInputElement element, ulong timestamp, Point position, PointerPointProperties properties, KeyModifiers inputModifiers)
	{
		IInputElement inputElement = null;
		IInputElement inputElement2;
		for (inputElement2 = element; inputElement2 != null; inputElement2 = GetVisualParent(inputElement2))
		{
			if (inputElement2.IsPointerOver)
			{
				inputElement = inputElement2;
				break;
			}
		}
		inputElement2 = root.PointerOverElement;
		PointerEventArgs pointerEventArgs = new PointerEventArgs(InputElement.PointerExitedEvent, inputElement2, pointer, (Visual)root, position, timestamp, properties, inputModifiers);
		if (inputElement2 is Visual visual && inputElement != null && !visual.IsAttachedToVisualTree)
		{
			ClearChildrenPointerOver(pointerEventArgs, inputElement, clearRoot: false);
		}
		while (inputElement2 != null && inputElement2 != inputElement)
		{
			pointerEventArgs.Source = inputElement2;
			pointerEventArgs.Handled = false;
			inputElement2.RaiseEvent(pointerEventArgs);
			inputElement2 = GetVisualParent(inputElement2);
		}
		IInputElement inputElement4 = (root.PointerOverElement = element);
		inputElement2 = inputElement4;
		pointerEventArgs.RoutedEvent = InputElement.PointerEnteredEvent;
		while (inputElement2 != null && inputElement2 != inputElement)
		{
			pointerEventArgs.Source = inputElement2;
			pointerEventArgs.Handled = false;
			inputElement2.RaiseEvent(pointerEventArgs);
			inputElement2 = GetVisualParent(inputElement2);
		}
	}

	private static IInputElement? GetVisualParent(IInputElement e)
	{
		return (e as Visual)?.VisualParent as IInputElement;
	}

	private static Point PointToClient(IInputRoot root, PixelPoint p)
	{
		return ((Visual)root).PointToClient(p);
	}
}
