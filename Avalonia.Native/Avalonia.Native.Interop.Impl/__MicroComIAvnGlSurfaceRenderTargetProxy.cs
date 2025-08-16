using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGlSurfaceRenderTargetProxy : MicroComProxyBase, IAvnGlSurfaceRenderTarget, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe IAvnGlSurfaceRenderingSession BeginDrawing()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("BeginDrawing failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnGlSurfaceRenderingSession>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnGlSurfaceRenderTarget), new Guid("931062d2-5bc8-4062-8588-83dd8deb99c2"), (IntPtr p, bool owns) => new __MicroComIAvnGlSurfaceRenderTargetProxy(p, owns));
	}

	protected __MicroComIAvnGlSurfaceRenderTargetProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
