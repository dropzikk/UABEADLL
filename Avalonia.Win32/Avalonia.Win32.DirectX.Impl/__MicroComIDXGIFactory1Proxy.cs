using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIFactory1Proxy : __MicroComIDXGIFactoryProxy, IDXGIFactory1, IDXGIFactory, IDXGIObject, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe int EnumAdapters1(ushort Adapter, void** ppAdapter)
	{
		return ((delegate* unmanaged[Stdcall]<void*, ushort, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, Adapter, ppAdapter);
	}

	public unsafe int IsCurrent()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIFactory1), new Guid("770aae78-f26f-4dba-a829-253c83d1b387"), (IntPtr p, bool owns) => new __MicroComIDXGIFactory1Proxy(p, owns));
	}

	protected __MicroComIDXGIFactory1Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
