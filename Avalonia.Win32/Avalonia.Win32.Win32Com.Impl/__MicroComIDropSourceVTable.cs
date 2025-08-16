using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIDropSourceVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int QueryContinueDragDelegate(void* @this, int fEscapePressed, int grfKeyState);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GiveFeedbackDelegate(void* @this, DropEffect dwEffect);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int QueryContinueDrag(void* @this, int fEscapePressed, int grfKeyState)
	{
		IDropSource dropSource = null;
		try
		{
			dropSource = (IDropSource)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return dropSource.QueryContinueDrag(fEscapePressed, grfKeyState);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dropSource, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GiveFeedback(void* @this, DropEffect dwEffect)
	{
		IDropSource dropSource = null;
		try
		{
			dropSource = (IDropSource)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return dropSource.GiveFeedback(dwEffect);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dropSource, e);
			return 0;
		}
	}

	protected unsafe __MicroComIDropSourceVTable()
	{
		AddMethod((delegate*<void*, int, int, int>)(&QueryContinueDrag));
		AddMethod((delegate*<void*, DropEffect, int>)(&GiveFeedback));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDropSource), new __MicroComIDropSourceVTable().CreateVTable());
	}
}
