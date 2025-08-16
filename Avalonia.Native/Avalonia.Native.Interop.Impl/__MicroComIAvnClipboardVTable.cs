using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnClipboardVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTextDelegate(void* @this, byte* type, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTextDelegate(void* @this, byte* type, byte* utf8Text);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ObtainFormatsDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStringsDelegate(void* @this, byte* type, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetBytesDelegate(void* @this, byte* type, void* utf8Text, int len);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBytesDelegate(void* @this, byte* type, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ClearDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetText(void* @this, byte* type, void** ppv)
	{
		IAvnClipboard avnClipboard = null;
		try
		{
			avnClipboard = (IAvnClipboard)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnString text = avnClipboard.GetText((type == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(type)));
			*ppv = MicroComRuntime.GetNativePointer(text, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnClipboard, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetText(void* @this, byte* type, byte* utf8Text)
	{
		IAvnClipboard avnClipboard = null;
		try
		{
			avnClipboard = (IAvnClipboard)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnClipboard.SetText((type == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(type)), (utf8Text == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(utf8Text)));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnClipboard, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ObtainFormats(void* @this, void** ppv)
	{
		IAvnClipboard avnClipboard = null;
		try
		{
			avnClipboard = (IAvnClipboard)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnStringArray obj = avnClipboard.ObtainFormats();
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnClipboard, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrings(void* @this, byte* type, void** ppv)
	{
		IAvnClipboard avnClipboard = null;
		try
		{
			avnClipboard = (IAvnClipboard)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnStringArray strings = avnClipboard.GetStrings((type == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(type)));
			*ppv = MicroComRuntime.GetNativePointer(strings, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnClipboard, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetBytes(void* @this, byte* type, void* utf8Text, int len)
	{
		IAvnClipboard avnClipboard = null;
		try
		{
			avnClipboard = (IAvnClipboard)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnClipboard.SetBytes((type == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(type)), utf8Text, len);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnClipboard, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetBytes(void* @this, byte* type, void** ppv)
	{
		IAvnClipboard avnClipboard = null;
		try
		{
			avnClipboard = (IAvnClipboard)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnString bytes = avnClipboard.GetBytes((type == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(type)));
			*ppv = MicroComRuntime.GetNativePointer(bytes, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnClipboard, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Clear(void* @this)
	{
		IAvnClipboard avnClipboard = null;
		try
		{
			avnClipboard = (IAvnClipboard)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnClipboard.Clear();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnClipboard, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnClipboardVTable()
	{
		AddMethod((delegate*<void*, byte*, void**, int>)(&GetText));
		AddMethod((delegate*<void*, byte*, byte*, int>)(&SetText));
		AddMethod((delegate*<void*, void**, int>)(&ObtainFormats));
		AddMethod((delegate*<void*, byte*, void**, int>)(&GetStrings));
		AddMethod((delegate*<void*, byte*, void*, int, int>)(&SetBytes));
		AddMethod((delegate*<void*, byte*, void**, int>)(&GetBytes));
		AddMethod((delegate*<void*, int>)(&Clear));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnClipboard), new __MicroComIAvnClipboardVTable().CreateVTable());
	}
}
