using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositor6VTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateGeometricClipDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateGeometricClipWithGeometryDelegate(void* @this, void* geometry, void** result);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateGeometricClip(void* @this, void** result)
	{
		ICompositor6 compositor = null;
		try
		{
			compositor = (ICompositor6)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionGeometricClip obj = compositor.CreateGeometricClip();
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateGeometricClipWithGeometry(void* @this, void* geometry, void** result)
	{
		ICompositor6 compositor = null;
		try
		{
			compositor = (ICompositor6)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionGeometricClip obj = compositor.CreateGeometricClipWithGeometry(MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(geometry, ownsHandle: false));
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositor6VTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&CreateGeometricClip));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateGeometricClipWithGeometry));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositor6), new __MicroComICompositor6VTable().CreateVTable());
	}
}
