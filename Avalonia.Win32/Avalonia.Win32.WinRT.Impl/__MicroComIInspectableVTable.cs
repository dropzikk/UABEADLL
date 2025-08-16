using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIInspectableVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetIidsDelegate(void* @this, ulong* iidCount, Guid** iids);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRuntimeClassNameDelegate(void* @this, IntPtr* className);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTrustLevelDelegate(void* @this, TrustLevel* trustLevel);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetIids(void* @this, ulong* iidCount, Guid** iids)
	{
		IInspectable inspectable = null;
		try
		{
			inspectable = (IInspectable)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			inspectable.GetIids(iidCount, iids);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(inspectable, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRuntimeClassName(void* @this, IntPtr* className)
	{
		IInspectable inspectable = null;
		try
		{
			inspectable = (IInspectable)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr runtimeClassName = inspectable.RuntimeClassName;
			*className = runtimeClassName;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(inspectable, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetTrustLevel(void* @this, TrustLevel* trustLevel)
	{
		IInspectable inspectable = null;
		try
		{
			inspectable = (IInspectable)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			TrustLevel trustLevel2 = inspectable.TrustLevel;
			*trustLevel = trustLevel2;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(inspectable, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIInspectableVTable()
	{
		AddMethod((delegate*<void*, ulong*, Guid**, int>)(&GetIids));
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetRuntimeClassName));
		AddMethod((delegate*<void*, TrustLevel*, int>)(&GetTrustLevel));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IInspectable), new __MicroComIInspectableVTable().CreateVTable());
	}
}
