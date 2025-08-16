using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIActivationFactoryVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ActivateInstanceDelegate(void* @this, IntPtr* instance);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ActivateInstance(void* @this, IntPtr* instance)
	{
		IActivationFactory activationFactory = null;
		try
		{
			activationFactory = (IActivationFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr intPtr = activationFactory.ActivateInstance();
			*instance = intPtr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(activationFactory, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIActivationFactoryVTable()
	{
		AddMethod((delegate*<void*, IntPtr*, int>)(&ActivateInstance));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IActivationFactory), new __MicroComIActivationFactoryVTable().CreateVTable());
	}
}
