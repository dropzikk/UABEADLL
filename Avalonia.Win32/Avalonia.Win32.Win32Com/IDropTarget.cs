using System;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IDropTarget : IUnknown, IDisposable
{
	unsafe void DragEnter(IDataObject pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect);

	unsafe void DragOver(int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect);

	void DragLeave();

	unsafe void Drop(IDataObject pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect);
}
