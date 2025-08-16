using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIFactory1VTable : __MicroComIDXGIFactoryVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EnumAdapters1Delegate(void* @this, ushort Adapter, void** ppAdapter);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsCurrentDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EnumAdapters1(void* @this, ushort Adapter, void** ppAdapter)
	{
		IDXGIFactory1 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return iDXGIFactory.EnumAdapters1(Adapter, ppAdapter);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsCurrent(void* @this)
	{
		IDXGIFactory1 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory1)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return iDXGIFactory.IsCurrent();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return 0;
		}
	}

	protected unsafe __MicroComIDXGIFactory1VTable()
	{
		AddMethod((delegate*<void*, ushort, void**, int>)(&EnumAdapters1));
		AddMethod((delegate*<void*, int>)(&IsCurrent));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIFactory1), new __MicroComIDXGIFactory1VTable().CreateVTable());
	}
}
