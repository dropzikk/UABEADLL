using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIDropTargetProxy : MicroComProxyBase, IDropTarget, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 4;

	public unsafe void DragEnter(IDataObject pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int, UnmanagedMethods.POINT, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(pDataObj), grfKeyState, pt, pdwEffect);
		if (num != 0)
		{
			throw new COMException("DragEnter failed", num);
		}
	}

	public unsafe void DragOver(int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, UnmanagedMethods.POINT, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, grfKeyState, pt, pdwEffect);
		if (num != 0)
		{
			throw new COMException("DragOver failed", num);
		}
	}

	public unsafe void DragLeave()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
		if (num != 0)
		{
			throw new COMException("DragLeave failed", num);
		}
	}

	public unsafe void Drop(IDataObject pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int, UnmanagedMethods.POINT, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, MicroComRuntime.GetNativePointer(pDataObj), grfKeyState, pt, pdwEffect);
		if (num != 0)
		{
			throw new COMException("Drop failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDropTarget), new Guid("00000122-0000-0000-C000-000000000046"), (IntPtr p, bool owns) => new __MicroComIDropTargetProxy(p, owns));
	}

	protected __MicroComIDropTargetProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
