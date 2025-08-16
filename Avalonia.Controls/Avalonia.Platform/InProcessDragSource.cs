using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Platform;

internal class InProcessDragSource : IPlatformDragSource
{
	private const RawInputModifiers MOUSE_INPUTMODIFIERS = RawInputModifiers.LeftMouseButton | RawInputModifiers.RightMouseButton | RawInputModifiers.MiddleMouseButton;

	private readonly IDragDropDevice _dragDrop;

	private readonly IInputManager _inputManager;

	private readonly LightweightSubject<DragDropEffects> _result = new LightweightSubject<DragDropEffects>();

	private DragDropEffects _allowedEffects;

	private IDataObject? _draggedData;

	private IInputRoot? _lastRoot;

	private Point _lastPosition;

	private StandardCursorType _lastCursorType;

	private object? _originalCursor;

	private RawInputModifiers? _initialInputModifiers;

	public InProcessDragSource()
	{
		_inputManager = AvaloniaLocator.Current.GetRequiredService<IInputManager>();
		_dragDrop = AvaloniaLocator.Current.GetRequiredService<IDragDropDevice>();
	}

	public async Task<DragDropEffects> DoDragDrop(PointerEventArgs triggerEvent, IDataObject data, DragDropEffects allowedEffects)
	{
		Dispatcher.UIThread.VerifyAccess();
		triggerEvent.Pointer.Capture(null);
		if (_draggedData == null)
		{
			_draggedData = data;
			_lastRoot = null;
			_lastPosition = default(Point);
			_allowedEffects = allowedEffects;
			AnonymousObserver<RawInputEventArgs> observer = new AnonymousObserver<RawInputEventArgs>(delegate(RawInputEventArgs arg)
			{
				if (!(arg is RawPointerEventArgs e))
				{
					if (arg is RawKeyEventArgs e2)
					{
						ProcessKeyEvents(e2);
					}
				}
				else
				{
					ProcessMouseEvents(e);
				}
			});
			using (_inputManager.PreProcess.Subscribe(observer))
			{
				TaskCompletionSource<DragDropEffects> taskCompletionSource = new TaskCompletionSource<DragDropEffects>();
				using (_result.Subscribe(new AnonymousObserver<DragDropEffects>(taskCompletionSource)))
				{
					return await taskCompletionSource.Task;
				}
			}
		}
		return DragDropEffects.None;
	}

	private DragDropEffects RaiseEventAndUpdateCursor(RawDragEventType type, IInputRoot root, Point pt, RawInputModifiers modifiers)
	{
		_lastPosition = pt;
		RawDragEvent rawDragEvent = new RawDragEvent(_dragDrop, type, root, pt, _draggedData, _allowedEffects, modifiers);
		((root as Visual)?.GetSelfAndVisualAncestors().OfType<TopLevel>().FirstOrDefault())?.PlatformImpl?.Input?.Invoke(rawDragEvent);
		DragDropEffects preferredEffect = GetPreferredEffect(rawDragEvent.Effects & _allowedEffects, modifiers);
		UpdateCursor(root, preferredEffect);
		return preferredEffect;
	}

	private static DragDropEffects GetPreferredEffect(DragDropEffects effect, RawInputModifiers modifiers)
	{
		if (effect == DragDropEffects.Copy || effect == DragDropEffects.Move || effect == DragDropEffects.Link || effect == DragDropEffects.None)
		{
			return effect;
		}
		if (effect.HasAllFlags(DragDropEffects.Link) && modifiers.HasAllFlags(RawInputModifiers.Alt))
		{
			return DragDropEffects.Link;
		}
		if (effect.HasAllFlags(DragDropEffects.Copy) && modifiers.HasAllFlags(RawInputModifiers.Control))
		{
			return DragDropEffects.Copy;
		}
		return DragDropEffects.Move;
	}

	private static StandardCursorType GetCursorForDropEffect(DragDropEffects effects)
	{
		if (effects.HasAllFlags(DragDropEffects.Copy))
		{
			return StandardCursorType.DragCopy;
		}
		if (effects.HasAllFlags(DragDropEffects.Move))
		{
			return StandardCursorType.DragMove;
		}
		if (effects.HasAllFlags(DragDropEffects.Link))
		{
			return StandardCursorType.DragLink;
		}
		return StandardCursorType.No;
	}

	private void UpdateCursor(IInputRoot? root, DragDropEffects effect)
	{
		if (_lastRoot != root)
		{
			if (_lastRoot is InputElement inputElement)
			{
				if (_originalCursor == AvaloniaProperty.UnsetValue)
				{
					inputElement.ClearValue(InputElement.CursorProperty);
				}
				else
				{
					inputElement.Cursor = _originalCursor as Cursor;
				}
			}
			if (root is InputElement inputElement2)
			{
				if (!inputElement2.IsSet(InputElement.CursorProperty))
				{
					_originalCursor = AvaloniaProperty.UnsetValue;
				}
				else
				{
					_originalCursor = root.Cursor;
				}
			}
			else
			{
				_originalCursor = null;
			}
			_lastCursorType = StandardCursorType.Arrow;
			_lastRoot = root;
		}
		if (root is InputElement inputElement3)
		{
			StandardCursorType cursorForDropEffect = GetCursorForDropEffect(effect);
			if (cursorForDropEffect != _lastCursorType)
			{
				_lastCursorType = cursorForDropEffect;
				inputElement3.Cursor = new Cursor(cursorForDropEffect);
			}
		}
	}

	private void CancelDragging()
	{
		if (_lastRoot != null)
		{
			RaiseEventAndUpdateCursor(RawDragEventType.DragLeave, _lastRoot, _lastPosition, RawInputModifiers.None);
		}
		UpdateCursor(null, DragDropEffects.None);
		_result.OnNext(DragDropEffects.None);
	}

	private void ProcessKeyEvents(RawKeyEventArgs e)
	{
		if (e.Type == RawKeyEventType.KeyDown && e.Key == Key.Escape)
		{
			if (_lastRoot != null)
			{
				RaiseEventAndUpdateCursor(RawDragEventType.DragLeave, _lastRoot, _lastPosition, e.Modifiers);
			}
			UpdateCursor(null, DragDropEffects.None);
			_result.OnNext(DragDropEffects.None);
			e.Handled = true;
		}
		else if ((e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.LeftAlt || e.Key == Key.RightAlt) && _lastRoot != null)
		{
			RaiseEventAndUpdateCursor(RawDragEventType.DragOver, _lastRoot, _lastPosition, e.Modifiers);
		}
	}

	private void ProcessMouseEvents(RawPointerEventArgs e)
	{
		if (!_initialInputModifiers.HasValue)
		{
			_initialInputModifiers = e.InputModifiers & (RawInputModifiers.LeftMouseButton | RawInputModifiers.RightMouseButton | RawInputModifiers.MiddleMouseButton);
		}
		switch (e.Type)
		{
		case RawPointerEventType.LeftButtonDown:
		case RawPointerEventType.RightButtonDown:
		case RawPointerEventType.MiddleButtonDown:
		case RawPointerEventType.NonClientLeftButtonDown:
			CancelDragging();
			e.Handled = true;
			break;
		case RawPointerEventType.LeaveWindow:
			RaiseEventAndUpdateCursor(RawDragEventType.DragLeave, e.Root, e.Position, e.InputModifiers);
			break;
		case RawPointerEventType.LeftButtonUp:
			CheckDraggingAccepted(RawInputModifiers.LeftMouseButton);
			break;
		case RawPointerEventType.MiddleButtonUp:
			CheckDraggingAccepted(RawInputModifiers.MiddleMouseButton);
			break;
		case RawPointerEventType.RightButtonUp:
			CheckDraggingAccepted(RawInputModifiers.RightMouseButton);
			break;
		case RawPointerEventType.Move:
		{
			RawInputModifiers rawInputModifiers = e.InputModifiers & (RawInputModifiers.LeftMouseButton | RawInputModifiers.RightMouseButton | RawInputModifiers.MiddleMouseButton);
			if (_initialInputModifiers.Value != rawInputModifiers)
			{
				CancelDragging();
				e.Handled = true;
			}
			else if (e.Root != _lastRoot)
			{
				if (_lastRoot is Visual visual && e.Root is Visual visual2)
				{
					RaiseEventAndUpdateCursor(RawDragEventType.DragLeave, _lastRoot, visual.PointToClient(visual2.PointToScreen(e.Position)), e.InputModifiers);
				}
				RaiseEventAndUpdateCursor(RawDragEventType.DragEnter, e.Root, e.Position, e.InputModifiers);
			}
			else
			{
				RaiseEventAndUpdateCursor(RawDragEventType.DragOver, e.Root, e.Position, e.InputModifiers);
			}
			break;
		}
		case RawPointerEventType.XButton1Down:
		case RawPointerEventType.XButton1Up:
		case RawPointerEventType.XButton2Down:
		case RawPointerEventType.XButton2Up:
		case RawPointerEventType.Wheel:
			break;
		}
		void CheckDraggingAccepted(RawInputModifiers changedMouseButton)
		{
			if (_initialInputModifiers.Value.HasAllFlags(changedMouseButton))
			{
				DragDropEffects value = RaiseEventAndUpdateCursor(RawDragEventType.Drop, e.Root, e.Position, e.InputModifiers);
				UpdateCursor(null, DragDropEffects.None);
				_result.OnNext(value);
			}
			else
			{
				CancelDragging();
			}
			e.Handled = true;
		}
	}
}
