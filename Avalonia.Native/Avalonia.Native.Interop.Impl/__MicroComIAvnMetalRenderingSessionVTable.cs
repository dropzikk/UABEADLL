using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMetalRenderingSessionVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPixelSizeDelegate(void* @this, AvnPixelSize* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate double GetScalingDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate IntPtr GetTextureDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPixelSize(void* @this, AvnPixelSize* ret)
	{
		IAvnMetalRenderingSession avnMetalRenderingSession = null;
		try
		{
			avnMetalRenderingSession = (IAvnMetalRenderingSession)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			AvnPixelSize pixelSize = avnMetalRenderingSession.PixelSize;
			*ret = pixelSize;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMetalRenderingSession, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static double GetScaling(void* @this)
	{
		IAvnMetalRenderingSession avnMetalRenderingSession = null;
		try
		{
			avnMetalRenderingSession = (IAvnMetalRenderingSession)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnMetalRenderingSession.Scaling;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMetalRenderingSession, e);
			return 0.0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static IntPtr GetTexture(void* @this)
	{
		IAvnMetalRenderingSession avnMetalRenderingSession = null;
		try
		{
			avnMetalRenderingSession = (IAvnMetalRenderingSession)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnMetalRenderingSession.Texture;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMetalRenderingSession, e);
			return (IntPtr)0;
		}
	}

	protected unsafe __MicroComIAvnMetalRenderingSessionVTable()
	{
		AddMethod((delegate*<void*, AvnPixelSize*, int>)(&GetPixelSize));
		AddMethod((delegate*<void*, double>)(&GetScaling));
		AddMethod((delegate*<void*, IntPtr>)(&GetTexture));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnMetalRenderingSession), new __MicroComIAvnMetalRenderingSessionVTable().CreateVTable());
	}
}
