using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMenuItemVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetSubMenuDelegate(void* @this, void* menu);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTitleDelegate(void* @this, byte* utf8String);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetGestureDelegate(void* @this, AvnKey key, AvnInputModifiers modifiers);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetActionDelegate(void* @this, void* predicate, void* callback);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetIsCheckedDelegate(void* @this, int isChecked);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetToggleTypeDelegate(void* @this, AvnMenuItemToggleType toggleType);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetIconDelegate(void* @this, void* data, IntPtr length);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetSubMenu(void* @this, void* menu)
	{
		IAvnMenuItem avnMenuItem = null;
		try
		{
			avnMenuItem = (IAvnMenuItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuItem.SetSubMenu(MicroComRuntime.CreateProxyOrNullFor<IAvnMenu>(menu, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTitle(void* @this, byte* utf8String)
	{
		IAvnMenuItem avnMenuItem = null;
		try
		{
			avnMenuItem = (IAvnMenuItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuItem.SetTitle((utf8String == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(utf8String)));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetGesture(void* @this, AvnKey key, AvnInputModifiers modifiers)
	{
		IAvnMenuItem avnMenuItem = null;
		try
		{
			avnMenuItem = (IAvnMenuItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuItem.SetGesture(key, modifiers);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetAction(void* @this, void* predicate, void* callback)
	{
		IAvnMenuItem avnMenuItem = null;
		try
		{
			avnMenuItem = (IAvnMenuItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuItem.SetAction(MicroComRuntime.CreateProxyOrNullFor<IAvnPredicateCallback>(predicate, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IAvnActionCallback>(callback, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetIsChecked(void* @this, int isChecked)
	{
		IAvnMenuItem avnMenuItem = null;
		try
		{
			avnMenuItem = (IAvnMenuItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuItem.SetIsChecked(isChecked);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetToggleType(void* @this, AvnMenuItemToggleType toggleType)
	{
		IAvnMenuItem avnMenuItem = null;
		try
		{
			avnMenuItem = (IAvnMenuItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuItem.SetToggleType(toggleType);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuItem, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetIcon(void* @this, void* data, IntPtr length)
	{
		IAvnMenuItem avnMenuItem = null;
		try
		{
			avnMenuItem = (IAvnMenuItem)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnMenuItem.SetIcon(data, length);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnMenuItem, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnMenuItemVTable()
	{
		AddMethod((delegate*<void*, void*, int>)(&SetSubMenu));
		AddMethod((delegate*<void*, byte*, int>)(&SetTitle));
		AddMethod((delegate*<void*, AvnKey, AvnInputModifiers, int>)(&SetGesture));
		AddMethod((delegate*<void*, void*, void*, int>)(&SetAction));
		AddMethod((delegate*<void*, int, int>)(&SetIsChecked));
		AddMethod((delegate*<void*, AvnMenuItemToggleType, int>)(&SetToggleType));
		AddMethod((delegate*<void*, void*, IntPtr, int>)(&SetIcon));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnMenuItem), new __MicroComIAvnMenuItemVTable().CreateVTable());
	}
}
