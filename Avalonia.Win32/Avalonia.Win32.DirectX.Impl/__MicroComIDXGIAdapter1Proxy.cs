using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIAdapter1Proxy : __MicroComIDXGIAdapterProxy, IDXGIAdapter1, IDXGIAdapter, IDXGIObject, IUnknown, IDisposable
{
	public unsafe DXGI_ADAPTER_DESC1 Desc1
	{
		get
		{
			DXGI_ADAPTER_DESC1 result = default(DXGI_ADAPTER_DESC1);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetDesc1 failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 1;

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIAdapter1), new Guid("29038f61-3839-4626-91fd-086879011a05"), (IntPtr p, bool owns) => new __MicroComIDXGIAdapter1Proxy(p, owns));
	}

	protected __MicroComIDXGIAdapter1Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
