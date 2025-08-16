using System;
using System.Runtime.CompilerServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPlatformBehaviorInhibitionProxy : MicroComProxyBase, IAvnPlatformBehaviorInhibition, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void SetInhibitAppSleep(int inhibitAppSleep, string reason)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(reason) + 1];
		Encoding.UTF8.GetBytes(reason, 0, reason.Length, array, 0);
		fixed (byte* ptr = array)
		{
			((delegate* unmanaged[Stdcall]<void*, int, void*, void>)(*base.PPV)[base.VTableSize])(base.PPV, inhibitAppSleep, ptr);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnPlatformBehaviorInhibition), new Guid("12edf00d-5803-4d3f-9947-b4840e5e9372"), (IntPtr p, bool owns) => new __MicroComIAvnPlatformBehaviorInhibitionProxy(p, owns));
	}

	protected __MicroComIAvnPlatformBehaviorInhibitionProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
