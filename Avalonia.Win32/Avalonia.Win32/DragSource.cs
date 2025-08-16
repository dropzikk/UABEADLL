using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using Avalonia.Win32.Interop;
using Avalonia.Win32.Win32Com;
using MicroCom.Runtime;

namespace Avalonia.Win32;

internal class DragSource : IPlatformDragSource
{
	public Task<DragDropEffects> DoDragDrop(PointerEventArgs triggerEvent, Avalonia.Input.IDataObject data, DragDropEffects allowedEffects)
	{
		Dispatcher.UIThread.VerifyAccess();
		triggerEvent.Pointer.Capture(null);
		using DataObject dataObject = new DataObject(data);
		using OleDragSource obj = new OleDragSource();
		DropEffect allowedEffects2 = OleDropTarget.ConvertDropEffect(allowedEffects);
		IntPtr nativeIntPtr = ((Avalonia.Win32.Win32Com.IDataObject)dataObject).GetNativeIntPtr(owned: false);
		IntPtr nativeIntPtr2 = ((IDropSource)obj).GetNativeIntPtr(owned: false);
		UnmanagedMethods.DoDragDrop(nativeIntPtr, nativeIntPtr2, (int)allowedEffects2, out var finalEffect);
		dataObject.ReleaseWrapped();
		return Task.FromResult(OleDropTarget.ConvertDropEffect((DropEffect)finalEffect));
	}
}
