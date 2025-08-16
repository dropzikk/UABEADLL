using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMetalRenderTargetProxy : MicroComProxyBase, IAvnMetalRenderTarget, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 1;

	public unsafe IAvnMetalRenderingSession BeginDrawing()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("BeginDrawing failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnMetalRenderingSession>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnMetalRenderTarget), new Guid("f1306b71-eca0-426e-8700-105192693b1a"), (IntPtr p, bool owns) => new __MicroComIAvnMetalRenderTargetProxy(p, owns));
	}

	protected __MicroComIAvnMetalRenderTargetProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
