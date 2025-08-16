using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIAdapterProxy : __MicroComIDXGIObjectProxy, IDXGIAdapter, IDXGIObject, IUnknown, IDisposable
{
	public unsafe DXGI_ADAPTER_DESC Desc
	{
		get
		{
			DXGI_ADAPTER_DESC result = default(DXGI_ADAPTER_DESC);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetDesc failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 3;

	public unsafe int EnumOutputs(ushort Output, void* ppOutput)
	{
		return ((delegate* unmanaged[Stdcall]<void*, ushort, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, Output, ppOutput);
	}

	public unsafe ulong CheckInterfaceSupport(Guid* InterfaceName)
	{
		ulong result = 0uL;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, InterfaceName, &result);
		if (num != 0)
		{
			throw new COMException("CheckInterfaceSupport failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIAdapter), new Guid("2411e7e1-12ac-4ccf-bd14-9798e8534dc0"), (IntPtr p, bool owns) => new __MicroComIDXGIAdapterProxy(p, owns));
	}

	protected __MicroComIDXGIAdapterProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
