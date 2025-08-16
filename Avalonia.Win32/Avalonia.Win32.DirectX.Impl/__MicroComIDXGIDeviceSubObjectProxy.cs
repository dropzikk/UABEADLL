using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIDeviceSubObjectProxy : __MicroComIDXGIObjectProxy, IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void* GetDevice(Guid* riid)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, riid, &result);
		if (num != 0)
		{
			throw new COMException("GetDevice failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIDeviceSubObject), new Guid("3d3e0379-f9de-4d58-bb6c-18d62992f1a6"), (IntPtr p, bool owns) => new __MicroComIDXGIDeviceSubObjectProxy(p, owns));
	}

	protected __MicroComIDXGIDeviceSubObjectProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
