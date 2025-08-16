using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMetalRenderTargetVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int BeginDrawingDelegate(void* @this, void** ret);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int BeginDrawing(void* @this, void** ret)
	{
		IAvnMetalRenderTarget avnMetalRenderTarget = null;
		try
		{
			avnMetalRenderTarget = (IAvnMetalRenderTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnMetalRenderingSession obj = avnMetalRenderTarget.BeginDrawing();
			*ret = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMetalRenderTarget, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnMetalRenderTargetVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&BeginDrawing));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnMetalRenderTarget), new __MicroComIAvnMetalRenderTargetVTable().CreateVTable());
	}
}
