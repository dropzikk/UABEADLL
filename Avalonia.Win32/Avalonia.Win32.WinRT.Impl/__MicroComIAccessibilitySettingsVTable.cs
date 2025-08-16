using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIAccessibilitySettingsVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetHighContrastDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetHighContrastSchemeDelegate(void* @this, IntPtr* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetHighContrast(void* @this, int* value)
	{
		IAccessibilitySettings accessibilitySettings = null;
		try
		{
			accessibilitySettings = (IAccessibilitySettings)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int highContrast = accessibilitySettings.HighContrast;
			*value = highContrast;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(accessibilitySettings, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetHighContrastScheme(void* @this, IntPtr* value)
	{
		IAccessibilitySettings accessibilitySettings = null;
		try
		{
			accessibilitySettings = (IAccessibilitySettings)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr highContrastScheme = accessibilitySettings.HighContrastScheme;
			*value = highContrastScheme;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(accessibilitySettings, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAccessibilitySettingsVTable()
	{
		AddMethod((delegate*<void*, int*, int>)(&GetHighContrast));
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetHighContrastScheme));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAccessibilitySettings), new __MicroComIAccessibilitySettingsVTable().CreateVTable());
	}
}
