using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGlSurfaceRenderTargetVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int BeginDrawingDelegate(void* @this, void** ret);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int BeginDrawing(void* @this, void** ret)
	{
		IAvnGlSurfaceRenderTarget avnGlSurfaceRenderTarget = null;
		try
		{
			avnGlSurfaceRenderTarget = (IAvnGlSurfaceRenderTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnGlSurfaceRenderingSession obj = avnGlSurfaceRenderTarget.BeginDrawing();
			*ret = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlSurfaceRenderTarget, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnGlSurfaceRenderTargetVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&BeginDrawing));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnGlSurfaceRenderTarget), new __MicroComIAvnGlSurfaceRenderTargetVTable().CreateVTable());
	}
}
