using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnSoftwareRenderTargetVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFrameDelegate(void* @this, AvnFramebuffer* fb);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFrame(void* @this, AvnFramebuffer* fb)
	{
		IAvnSoftwareRenderTarget avnSoftwareRenderTarget = null;
		try
		{
			avnSoftwareRenderTarget = (IAvnSoftwareRenderTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnSoftwareRenderTarget.SetFrame(fb);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnSoftwareRenderTarget, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnSoftwareRenderTargetVTable()
	{
		AddMethod((delegate*<void*, AvnFramebuffer*, int>)(&SetFrame));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnSoftwareRenderTarget), new __MicroComIAvnSoftwareRenderTargetVTable().CreateVTable());
	}
}
