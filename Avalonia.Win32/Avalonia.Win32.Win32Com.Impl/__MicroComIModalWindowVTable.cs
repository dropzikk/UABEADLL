using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIModalWindowVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ShowDelegate(void* @this, IntPtr hwndOwner);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Show(void* @this, IntPtr hwndOwner)
	{
		IModalWindow modalWindow = null;
		try
		{
			modalWindow = (IModalWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return modalWindow.Show(hwndOwner);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(modalWindow, e);
			return 0;
		}
	}

	protected unsafe __MicroComIModalWindowVTable()
	{
		AddMethod((delegate*<void*, IntPtr, int>)(&Show));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IModalWindow), new __MicroComIModalWindowVTable().CreateVTable());
	}
}
