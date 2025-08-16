using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIResourceProxy : __MicroComIDXGIDeviceSubObjectProxy, IDXGIResource, IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	public unsafe IntPtr SharedHandle
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetSharedHandle failed", num);
			}
			return result;
		}
	}

	public unsafe uint Usage
	{
		get
		{
			uint result = 0u;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetUsage failed", num);
			}
			return result;
		}
	}

	public unsafe ushort EvictionPriority
	{
		get
		{
			ushort result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetEvictionPriority failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 4;

	public unsafe void SetEvictionPriority(ushort EvictionPriority)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, EvictionPriority);
		if (num != 0)
		{
			throw new COMException("SetEvictionPriority failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIResource), new Guid(" 035f3ab4-482e-4e50-b41f-8a7f8bd8960b"), (IntPtr p, bool owns) => new __MicroComIDXGIResourceProxy(p, owns));
	}

	protected __MicroComIDXGIResourceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
