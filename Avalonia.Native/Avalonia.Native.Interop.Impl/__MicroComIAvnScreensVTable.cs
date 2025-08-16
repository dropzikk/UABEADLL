using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnScreensVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetScreenCountDelegate(void* @this, int* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetScreenDelegate(void* @this, int index, AvnScreen* ret);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetScreenCount(void* @this, int* ret)
	{
		IAvnScreens avnScreens = null;
		try
		{
			avnScreens = (IAvnScreens)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int screenCount = avnScreens.ScreenCount;
			*ret = screenCount;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnScreens, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetScreen(void* @this, int index, AvnScreen* ret)
	{
		IAvnScreens avnScreens = null;
		try
		{
			avnScreens = (IAvnScreens)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			AvnScreen screen = avnScreens.GetScreen(index);
			*ret = screen;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnScreens, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnScreensVTable()
	{
		AddMethod((delegate*<void*, int*, int>)(&GetScreenCount));
		AddMethod((delegate*<void*, int, AvnScreen*, int>)(&GetScreen));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnScreens), new __MicroComIAvnScreensVTable().CreateVTable());
	}
}
