using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGeometricClipVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetGeometryDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetGeometryDelegate(void* @this, void* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetGeometry(void* @this, void** value)
	{
		ICompositionGeometricClip compositionGeometricClip = null;
		try
		{
			compositionGeometricClip = (ICompositionGeometricClip)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionGeometry geometry = compositionGeometricClip.Geometry;
			*value = MicroComRuntime.GetNativePointer(geometry, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGeometricClip, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetGeometry(void* @this, void* value)
	{
		ICompositionGeometricClip compositionGeometricClip = null;
		try
		{
			compositionGeometricClip = (ICompositionGeometricClip)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionGeometricClip.SetGeometry(MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGeometricClip, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionGeometricClipVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetGeometry));
		AddMethod((delegate*<void*, void*, int>)(&SetGeometry));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionGeometricClip), new __MicroComICompositionGeometricClipVTable().CreateVTable());
	}
}
