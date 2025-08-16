using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMetalDeviceProxy : MicroComProxyBase, IAvnMetalDevice, IUnknown, IDisposable
{
	public unsafe IntPtr Device => ((delegate* unmanaged[Stdcall]<void*, IntPtr>)(*base.PPV)[base.VTableSize])(base.PPV);

	public unsafe IntPtr Queue => ((delegate* unmanaged[Stdcall]<void*, IntPtr>)(*base.PPV)[base.VTableSize + 1])(base.PPV);

	protected override int VTableSize => base.VTableSize + 2;

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnMetalDevice), new Guid("969fa914-b74a-4c9f-8725-5160dc63579e"), (IntPtr p, bool owns) => new __MicroComIAvnMetalDeviceProxy(p, owns));
	}

	protected __MicroComIAvnMetalDeviceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
