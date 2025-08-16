using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.MicroCom;
using Avalonia.Platform;
using Avalonia.Win32.Interop;
using Avalonia.Win32.Win32Com;
using MicroCom.Runtime;

namespace Avalonia.Win32;

internal class OleDropTarget : CallbackBase, IDropTarget, IUnknown, IDisposable
{
	private readonly IInputRoot _target;

	private readonly ITopLevelImpl _topLevel;

	private readonly IDragDropDevice _dragDevice;

	private Avalonia.Input.IDataObject? _currentDrag;

	public OleDropTarget(ITopLevelImpl topLevel, IInputRoot target, IDragDropDevice dragDevice)
	{
		_topLevel = topLevel;
		_target = target;
		_dragDevice = dragDevice;
	}

	public static DropEffect ConvertDropEffect(DragDropEffects operation)
	{
		DropEffect dropEffect = DropEffect.None;
		if (operation.HasAllFlags(DragDropEffects.Copy))
		{
			dropEffect |= DropEffect.Copy;
		}
		if (operation.HasAllFlags(DragDropEffects.Move))
		{
			dropEffect |= DropEffect.Move;
		}
		if (operation.HasAllFlags(DragDropEffects.Link))
		{
			dropEffect |= DropEffect.Link;
		}
		return dropEffect;
	}

	public static DragDropEffects ConvertDropEffect(DropEffect effect)
	{
		DragDropEffects dragDropEffects = DragDropEffects.None;
		if (effect.HasAllFlags(DropEffect.Copy))
		{
			dragDropEffects |= DragDropEffects.Copy;
		}
		if (effect.HasAllFlags(DropEffect.Move))
		{
			dragDropEffects |= DragDropEffects.Move;
		}
		if (effect.HasAllFlags(DropEffect.Link))
		{
			dragDropEffects |= DragDropEffects.Link;
		}
		return dragDropEffects;
	}

	private static RawInputModifiers ConvertKeyState(int grfKeyState)
	{
		RawInputModifiers rawInputModifiers = RawInputModifiers.None;
		if (((UnmanagedMethods.ModifierKeys)grfKeyState).HasAllFlags(UnmanagedMethods.ModifierKeys.MK_LBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.LeftMouseButton;
		}
		if (((UnmanagedMethods.ModifierKeys)grfKeyState).HasAllFlags(UnmanagedMethods.ModifierKeys.MK_MBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.MiddleMouseButton;
		}
		if (((UnmanagedMethods.ModifierKeys)grfKeyState).HasAllFlags(UnmanagedMethods.ModifierKeys.MK_RBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.RightMouseButton;
		}
		if (((UnmanagedMethods.ModifierKeys)grfKeyState).HasAllFlags(UnmanagedMethods.ModifierKeys.MK_SHIFT))
		{
			rawInputModifiers |= RawInputModifiers.Shift;
		}
		if (((UnmanagedMethods.ModifierKeys)grfKeyState).HasAllFlags(UnmanagedMethods.ModifierKeys.MK_CONTROL))
		{
			rawInputModifiers |= RawInputModifiers.Control;
		}
		if (((UnmanagedMethods.ModifierKeys)grfKeyState).HasAllFlags(UnmanagedMethods.ModifierKeys.MK_ALT))
		{
			rawInputModifiers |= RawInputModifiers.Alt;
		}
		return rawInputModifiers;
	}

	unsafe void IDropTarget.DragEnter(Avalonia.Win32.Win32Com.IDataObject pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		Action<RawInputEventArgs> input = _topLevel.Input;
		if (input == null)
		{
			*pdwEffect = DropEffect.None;
			return;
		}
		SetDataObject(pDataObj);
		RawDragEvent rawDragEvent = new RawDragEvent(_dragDevice, RawDragEventType.DragEnter, _target, GetDragLocation(pt), _currentDrag, ConvertDropEffect(*pdwEffect), ConvertKeyState(grfKeyState));
		input(rawDragEvent);
		*pdwEffect = ConvertDropEffect(rawDragEvent.Effects);
	}

	unsafe void IDropTarget.DragOver(int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		Action<RawInputEventArgs> input = _topLevel.Input;
		if (input == null || _currentDrag == null)
		{
			*pdwEffect = DropEffect.None;
			return;
		}
		RawDragEvent rawDragEvent = new RawDragEvent(_dragDevice, RawDragEventType.DragOver, _target, GetDragLocation(pt), _currentDrag, ConvertDropEffect(*pdwEffect), ConvertKeyState(grfKeyState));
		input(rawDragEvent);
		*pdwEffect = ConvertDropEffect(rawDragEvent.Effects);
	}

	void IDropTarget.DragLeave()
	{
		Action<RawInputEventArgs> input = _topLevel.Input;
		if (input == null || _currentDrag == null)
		{
			return;
		}
		try
		{
			input(new RawDragEvent(_dragDevice, RawDragEventType.DragLeave, _target, default(Point), _currentDrag, DragDropEffects.None, RawInputModifiers.None));
		}
		finally
		{
			ReleaseDataObject();
		}
	}

	unsafe void IDropTarget.Drop(Avalonia.Win32.Win32Com.IDataObject pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		try
		{
			Action<RawInputEventArgs> input = _topLevel.Input;
			if (input == null)
			{
				*pdwEffect = DropEffect.None;
				return;
			}
			SetDataObject(pDataObj);
			RawDragEvent rawDragEvent = new RawDragEvent(_dragDevice, RawDragEventType.Drop, _target, GetDragLocation(pt), _currentDrag, ConvertDropEffect(*pdwEffect), ConvertKeyState(grfKeyState));
			input(rawDragEvent);
			*pdwEffect = ConvertDropEffect(rawDragEvent.Effects);
		}
		finally
		{
			ReleaseDataObject();
		}
	}

	[MemberNotNull("_currentDrag")]
	private void SetDataObject(Avalonia.Win32.Win32Com.IDataObject pDataObj)
	{
		Avalonia.Input.IDataObject avaloniaObjectFromCOM = GetAvaloniaObjectFromCOM(pDataObj);
		if (_currentDrag != avaloniaObjectFromCOM)
		{
			ReleaseDataObject();
			_currentDrag = avaloniaObjectFromCOM;
		}
	}

	private void ReleaseDataObject()
	{
		if (_currentDrag is OleDataObject oleDataObject)
		{
			oleDataObject.Dispose();
		}
		_currentDrag = null;
	}

	private Point GetDragLocation(UnmanagedMethods.POINT dragPoint)
	{
		return VisualExtensions.PointToClient(point: new PixelPoint(dragPoint.X, dragPoint.Y), visual: (Visual)_target);
	}

	protected override void Destroyed()
	{
		ReleaseDataObject();
	}

	public static Avalonia.Input.IDataObject GetAvaloniaObjectFromCOM(Avalonia.Win32.Win32Com.IDataObject pDataObj)
	{
		if (pDataObj == null)
		{
			throw new ArgumentNullException("pDataObj");
		}
		if (pDataObj is Avalonia.Input.IDataObject result)
		{
			return result;
		}
		if (MicroComRuntime.TryUnwrapManagedObject(pDataObj) is DataObject result2)
		{
			return result2;
		}
		return new OleDataObject(pDataObj);
	}
}
