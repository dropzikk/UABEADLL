using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnTextInputMethodVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetClientDelegate(void* @this, void* client);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ResetDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SetCursorRectDelegate(void* @this, AvnRect rect);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void SetSurroundingTextDelegate(void* @this, byte* text, int anchorOffset, int cursorOffset);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetClient(void* @this, void* client)
	{
		IAvnTextInputMethod avnTextInputMethod = null;
		try
		{
			avnTextInputMethod = (IAvnTextInputMethod)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTextInputMethod.SetClient(MicroComRuntime.CreateProxyOrNullFor<IAvnTextInputMethodClient>(client, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTextInputMethod, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Reset(void* @this)
	{
		IAvnTextInputMethod avnTextInputMethod = null;
		try
		{
			avnTextInputMethod = (IAvnTextInputMethod)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTextInputMethod.Reset();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTextInputMethod, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SetCursorRect(void* @this, AvnRect rect)
	{
		IAvnTextInputMethod avnTextInputMethod = null;
		try
		{
			avnTextInputMethod = (IAvnTextInputMethod)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTextInputMethod.SetCursorRect(rect);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTextInputMethod, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void SetSurroundingText(void* @this, byte* text, int anchorOffset, int cursorOffset)
	{
		IAvnTextInputMethod avnTextInputMethod = null;
		try
		{
			avnTextInputMethod = (IAvnTextInputMethod)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnTextInputMethod.SetSurroundingText((text == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(text)), anchorOffset, cursorOffset);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnTextInputMethod, e);
		}
	}

	protected unsafe __MicroComIAvnTextInputMethodVTable()
	{
		AddMethod((delegate*<void*, void*, int>)(&SetClient));
		AddMethod((delegate*<void*, void>)(&Reset));
		AddMethod((delegate*<void*, AvnRect, void>)(&SetCursorRect));
		AddMethod((delegate*<void*, byte*, int, int, void>)(&SetSurroundingText));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnTextInputMethod), new __MicroComIAvnTextInputMethodVTable().CreateVTable());
	}
}
