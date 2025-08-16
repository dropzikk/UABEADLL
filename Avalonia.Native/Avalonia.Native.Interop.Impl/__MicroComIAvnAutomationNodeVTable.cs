using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnAutomationNodeVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void DisposeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void ChildrenChangedDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void PropertyChangedDelegate(void* @this, AvnAutomationProperty property);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void FocusChangedDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void Dispose(void* @this)
	{
		IAvnAutomationNode avnAutomationNode = null;
		try
		{
			avnAutomationNode = (IAvnAutomationNode)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationNode.Dispose();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationNode, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void ChildrenChanged(void* @this)
	{
		IAvnAutomationNode avnAutomationNode = null;
		try
		{
			avnAutomationNode = (IAvnAutomationNode)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationNode.ChildrenChanged();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationNode, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void PropertyChanged(void* @this, AvnAutomationProperty property)
	{
		IAvnAutomationNode avnAutomationNode = null;
		try
		{
			avnAutomationNode = (IAvnAutomationNode)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationNode.PropertyChanged(property);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationNode, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void FocusChanged(void* @this)
	{
		IAvnAutomationNode avnAutomationNode = null;
		try
		{
			avnAutomationNode = (IAvnAutomationNode)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnAutomationNode.FocusChanged();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationNode, e);
		}
	}

	protected unsafe __MicroComIAvnAutomationNodeVTable()
	{
		AddMethod((delegate*<void*, void>)(&Dispose));
		AddMethod((delegate*<void*, void>)(&ChildrenChanged));
		AddMethod((delegate*<void*, AvnAutomationProperty, void>)(&PropertyChanged));
		AddMethod((delegate*<void*, void>)(&FocusChanged));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnAutomationNode), new __MicroComIAvnAutomationNodeVTable().CreateVTable());
	}
}
