using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIUISettings3VTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetColorValueDelegate(void* @this, UIColorType desiredColor, WinRTColor* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetColorValue(void* @this, UIColorType desiredColor, WinRTColor* value)
	{
		IUISettings3 iUISettings = null;
		try
		{
			iUISettings = (IUISettings3)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			WinRTColor colorValue = iUISettings.GetColorValue(desiredColor);
			*value = colorValue;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iUISettings, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIUISettings3VTable()
	{
		AddMethod((delegate*<void*, UIColorType, WinRTColor*, int>)(&GetColorValue));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IUISettings3), new __MicroComIUISettings3VTable().CreateVTable());
	}
}
