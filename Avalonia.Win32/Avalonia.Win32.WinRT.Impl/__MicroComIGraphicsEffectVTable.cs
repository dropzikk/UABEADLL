using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIGraphicsEffectVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetNameDelegate(void* @this, IntPtr* name);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetNameDelegate(void* @this, IntPtr name);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetName(void* @this, IntPtr* name)
	{
		IGraphicsEffect graphicsEffect = null;
		try
		{
			graphicsEffect = (IGraphicsEffect)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr name2 = graphicsEffect.Name;
			*name = name2;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(graphicsEffect, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetName(void* @this, IntPtr name)
	{
		IGraphicsEffect graphicsEffect = null;
		try
		{
			graphicsEffect = (IGraphicsEffect)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			graphicsEffect.SetName(name);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(graphicsEffect, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIGraphicsEffectVTable()
	{
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetName));
		AddMethod((delegate*<void*, IntPtr, int>)(&SetName));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IGraphicsEffect), new __MicroComIGraphicsEffectVTable().CreateVTable());
	}
}
