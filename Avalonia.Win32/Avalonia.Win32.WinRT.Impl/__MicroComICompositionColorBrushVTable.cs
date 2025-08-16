using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionColorBrushVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetColorDelegate(void* @this, WinRTColor* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetColorDelegate(void* @this, WinRTColor value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetColor(void* @this, WinRTColor* value)
	{
		ICompositionColorBrush compositionColorBrush = null;
		try
		{
			compositionColorBrush = (ICompositionColorBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			WinRTColor color = compositionColorBrush.Color;
			*value = color;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionColorBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetColor(void* @this, WinRTColor value)
	{
		ICompositionColorBrush compositionColorBrush = null;
		try
		{
			compositionColorBrush = (ICompositionColorBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionColorBrush.SetColor(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionColorBrush, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionColorBrushVTable()
	{
		AddMethod((delegate*<void*, WinRTColor*, int>)(&GetColor));
		AddMethod((delegate*<void*, WinRTColor, int>)(&SetColor));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionColorBrush), new __MicroComICompositionColorBrushVTable().CreateVTable());
	}
}
