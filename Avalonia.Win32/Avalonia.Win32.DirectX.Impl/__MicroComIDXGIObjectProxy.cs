using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIObjectProxy : MicroComProxyBase, IDXGIObject, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 4;

	public unsafe void SetPrivateData(Guid* Name, ushort DataSize, void** pData)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, ushort, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, Name, DataSize, pData);
		if (num != 0)
		{
			throw new COMException("SetPrivateData failed", num);
		}
	}

	public unsafe void SetPrivateDataInterface(Guid* Name, IUnknown pUnknown)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, Name, MicroComRuntime.GetNativePointer(pUnknown));
		if (num != 0)
		{
			throw new COMException("SetPrivateDataInterface failed", num);
		}
	}

	public unsafe void* GetPrivateData(Guid* Name, ushort* pDataSize)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, Name, pDataSize, &result);
		if (num != 0)
		{
			throw new COMException("GetPrivateData failed", num);
		}
		return result;
	}

	public unsafe void* GetParent(Guid* riid)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, riid, &result);
		if (num != 0)
		{
			throw new COMException("GetParent failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIObject), new Guid("aec22fb8-76f3-4639-9be0-28eb43a67a2e"), (IntPtr p, bool owns) => new __MicroComIDXGIObjectProxy(p, owns));
	}

	protected __MicroComIDXGIObjectProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
