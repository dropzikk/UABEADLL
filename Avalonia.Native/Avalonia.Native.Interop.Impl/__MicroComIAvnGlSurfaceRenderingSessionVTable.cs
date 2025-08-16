using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGlSurfaceRenderingSessionVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPixelSizeDelegate(void* @this, AvnPixelSize* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetScalingDelegate(void* @this, double* ret);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPixelSize(void* @this, AvnPixelSize* ret)
	{
		IAvnGlSurfaceRenderingSession avnGlSurfaceRenderingSession = null;
		try
		{
			avnGlSurfaceRenderingSession = (IAvnGlSurfaceRenderingSession)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			AvnPixelSize pixelSize = avnGlSurfaceRenderingSession.PixelSize;
			*ret = pixelSize;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlSurfaceRenderingSession, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetScaling(void* @this, double* ret)
	{
		IAvnGlSurfaceRenderingSession avnGlSurfaceRenderingSession = null;
		try
		{
			avnGlSurfaceRenderingSession = (IAvnGlSurfaceRenderingSession)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			double scaling = avnGlSurfaceRenderingSession.Scaling;
			*ret = scaling;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGlSurfaceRenderingSession, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnGlSurfaceRenderingSessionVTable()
	{
		AddMethod((delegate*<void*, AvnPixelSize*, int>)(&GetPixelSize));
		AddMethod((delegate*<void*, double*, int>)(&GetScaling));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnGlSurfaceRenderingSession), new __MicroComIAvnGlSurfaceRenderingSessionVTable().CreateVTable());
	}
}
