using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComISwapChainInteropVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetSwapChainDelegate(void* @this, void* swapChain);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetSwapChain(void* @this, void* swapChain)
	{
		ISwapChainInterop swapChainInterop = null;
		try
		{
			swapChainInterop = (ISwapChainInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			swapChainInterop.SetSwapChain(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(swapChain, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(swapChainInterop, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComISwapChainInteropVTable()
	{
		AddMethod((delegate*<void*, void*, int>)(&SetSwapChain));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ISwapChainInterop), new __MicroComISwapChainInteropVTable().CreateVTable());
	}
}
