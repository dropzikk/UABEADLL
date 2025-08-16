using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIDropTargetVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int DragEnterDelegate(void* @this, void* pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int DragOverDelegate(void* @this, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int DragLeaveDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int DropDelegate(void* @this, void* pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int DragEnter(void* @this, void* pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		IDropTarget dropTarget = null;
		try
		{
			dropTarget = (IDropTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			dropTarget.DragEnter(MicroComRuntime.CreateProxyOrNullFor<IDataObject>(pDataObj, ownsHandle: false), grfKeyState, pt, pdwEffect);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dropTarget, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int DragOver(void* @this, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		IDropTarget dropTarget = null;
		try
		{
			dropTarget = (IDropTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			dropTarget.DragOver(grfKeyState, pt, pdwEffect);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dropTarget, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int DragLeave(void* @this)
	{
		IDropTarget dropTarget = null;
		try
		{
			dropTarget = (IDropTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			dropTarget.DragLeave();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dropTarget, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Drop(void* @this, void* pDataObj, int grfKeyState, UnmanagedMethods.POINT pt, DropEffect* pdwEffect)
	{
		IDropTarget dropTarget = null;
		try
		{
			dropTarget = (IDropTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			dropTarget.Drop(MicroComRuntime.CreateProxyOrNullFor<IDataObject>(pDataObj, ownsHandle: false), grfKeyState, pt, pdwEffect);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dropTarget, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDropTargetVTable()
	{
		AddMethod((delegate*<void*, void*, int, UnmanagedMethods.POINT, DropEffect*, int>)(&DragEnter));
		AddMethod((delegate*<void*, int, UnmanagedMethods.POINT, DropEffect*, int>)(&DragOver));
		AddMethod((delegate*<void*, int>)(&DragLeave));
		AddMethod((delegate*<void*, void*, int, UnmanagedMethods.POINT, DropEffect*, int>)(&Drop));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDropTarget), new __MicroComIDropTargetVTable().CreateVTable());
	}
}
