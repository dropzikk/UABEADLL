using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnSoftwareRenderTargetProxy : MicroComProxyBase, IAvnSoftwareRenderTarget, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe void SetFrame(AvnFramebuffer* fb)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, fb);
		if (num != 0)
		{
			throw new COMException("SetFrame failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnSoftwareRenderTarget), new Guid("931062d2-5bc8-4062-8588-83dd8deb99c2"), (IntPtr p, bool owns) => new __MicroComIAvnSoftwareRenderTargetProxy(p, owns));
	}

	protected __MicroComIAvnSoftwareRenderTargetProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
