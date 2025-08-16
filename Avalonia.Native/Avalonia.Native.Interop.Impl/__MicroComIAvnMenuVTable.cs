using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMenuVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InsertItemDelegate(void* @this, int index, void* item);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RemoveItemDelegate(void* @this, void* item);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTitleDelegate(void* @this, byte* utf8String);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ClearDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int InsertItem(void* @this, int index, void* item)
	{
		IAvnMenu avnMenu = null;
		try
		{
			avnMenu = (IAvnMenu)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenu.InsertItem(index, MicroComRuntime.CreateProxyOrNullFor<IAvnMenuItem>(item, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenu, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RemoveItem(void* @this, void* item)
	{
		IAvnMenu avnMenu = null;
		try
		{
			avnMenu = (IAvnMenu)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenu.RemoveItem(MicroComRuntime.CreateProxyOrNullFor<IAvnMenuItem>(item, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenu, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTitle(void* @this, byte* utf8String)
	{
		IAvnMenu avnMenu = null;
		try
		{
			avnMenu = (IAvnMenu)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenu.SetTitle((utf8String == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(utf8String)));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenu, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Clear(void* @this)
	{
		IAvnMenu avnMenu = null;
		try
		{
			avnMenu = (IAvnMenu)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenu.Clear();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenu, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnMenuVTable()
	{
		AddMethod((delegate*<void*, int, void*, int>)(&InsertItem));
		AddMethod((delegate*<void*, void*, int>)(&RemoveItem));
		AddMethod((delegate*<void*, byte*, int>)(&SetTitle));
		AddMethod((delegate*<void*, int>)(&Clear));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnMenu), new __MicroComIAvnMenuVTable().CreateVTable());
	}
}
