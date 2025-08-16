using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnCursorFactoryProxy : MicroComProxyBase, IAvnCursorFactory, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe IAvnCursor GetCursor(AvnStandardCursorType cursorType)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnStandardCursorType, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, cursorType, &pObject);
		if (num != 0)
		{
			throw new COMException("GetCursor failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnCursor>(pObject, ownsHandle: true);
	}

	public unsafe IAvnCursor CreateCustomCursor(void* bitmapData, IntPtr length, AvnPixelSize hotPixel)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, AvnPixelSize, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, bitmapData, length, hotPixel, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateCustomCursor failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnCursor>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnCursorFactory), new Guid("51ecfb12-c427-4757-a2c9-1596bfce53ef"), (IntPtr p, bool owns) => new __MicroComIAvnCursorFactoryProxy(p, owns));
	}

	protected __MicroComIAvnCursorFactoryProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
