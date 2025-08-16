using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnTextInputMethodClientVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SetPreeditTextDelegate(void* @this, byte* preeditText);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SelectInSurroundingTextDelegate(void* @this, int start, int length);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SetPreeditText(void* @this, byte* preeditText)
	{
		IAvnTextInputMethodClient avnTextInputMethodClient = null;
		try
		{
			avnTextInputMethodClient = (IAvnTextInputMethodClient)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTextInputMethodClient.SetPreeditText((preeditText == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(preeditText)));
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTextInputMethodClient, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SelectInSurroundingText(void* @this, int start, int length)
	{
		IAvnTextInputMethodClient avnTextInputMethodClient = null;
		try
		{
			avnTextInputMethodClient = (IAvnTextInputMethodClient)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTextInputMethodClient.SelectInSurroundingText(start, length);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTextInputMethodClient, e);
		}
	}

	protected unsafe __MicroComIAvnTextInputMethodClientVTable()
	{
		AddMethod((delegate*<void*, byte*, void>)(&SetPreeditText));
		AddMethod((delegate*<void*, int, int, void>)(&SelectInSurroundingText));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnTextInputMethodClient), new __MicroComIAvnTextInputMethodClientVTable().CreateVTable());
	}
}
