using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIVisualCollectionVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCountDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InsertAboveDelegate(void* @this, void* newChild, void* sibling);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InsertAtBottomDelegate(void* @this, void* newChild);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InsertAtTopDelegate(void* @this, void* newChild);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InsertBelowDelegate(void* @this, void* newChild, void* sibling);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RemoveDelegate(void* @this, void* child);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RemoveAllDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCount(void* @this, int* value)
	{
		IVisualCollection visualCollection = null;
		try
		{
			visualCollection = (IVisualCollection)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int count = visualCollection.Count;
			*value = count;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visualCollection, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int InsertAbove(void* @this, void* newChild, void* sibling)
	{
		IVisualCollection visualCollection = null;
		try
		{
			visualCollection = (IVisualCollection)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visualCollection.InsertAbove(MicroComRuntime.CreateProxyOrNullFor<IVisual>(newChild, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IVisual>(sibling, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visualCollection, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int InsertAtBottom(void* @this, void* newChild)
	{
		IVisualCollection visualCollection = null;
		try
		{
			visualCollection = (IVisualCollection)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visualCollection.InsertAtBottom(MicroComRuntime.CreateProxyOrNullFor<IVisual>(newChild, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visualCollection, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int InsertAtTop(void* @this, void* newChild)
	{
		IVisualCollection visualCollection = null;
		try
		{
			visualCollection = (IVisualCollection)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visualCollection.InsertAtTop(MicroComRuntime.CreateProxyOrNullFor<IVisual>(newChild, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visualCollection, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int InsertBelow(void* @this, void* newChild, void* sibling)
	{
		IVisualCollection visualCollection = null;
		try
		{
			visualCollection = (IVisualCollection)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visualCollection.InsertBelow(MicroComRuntime.CreateProxyOrNullFor<IVisual>(newChild, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IVisual>(sibling, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visualCollection, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Remove(void* @this, void* child)
	{
		IVisualCollection visualCollection = null;
		try
		{
			visualCollection = (IVisualCollection)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visualCollection.Remove(MicroComRuntime.CreateProxyOrNullFor<IVisual>(child, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visualCollection, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RemoveAll(void* @this)
	{
		IVisualCollection visualCollection = null;
		try
		{
			visualCollection = (IVisualCollection)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visualCollection.RemoveAll();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visualCollection, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIVisualCollectionVTable()
	{
		AddMethod((delegate*<void*, int*, int>)(&GetCount));
		AddMethod((delegate*<void*, void*, void*, int>)(&InsertAbove));
		AddMethod((delegate*<void*, void*, int>)(&InsertAtBottom));
		AddMethod((delegate*<void*, void*, int>)(&InsertAtTop));
		AddMethod((delegate*<void*, void*, void*, int>)(&InsertBelow));
		AddMethod((delegate*<void*, void*, int>)(&Remove));
		AddMethod((delegate*<void*, int>)(&RemoveAll));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IVisualCollection), new __MicroComIVisualCollectionVTable().CreateVTable());
	}
}
