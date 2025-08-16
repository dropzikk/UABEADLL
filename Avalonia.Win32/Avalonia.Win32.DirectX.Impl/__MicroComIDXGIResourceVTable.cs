using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIResourceVTable : __MicroComIDXGIDeviceSubObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSharedHandleDelegate(void* @this, IntPtr* pSharedHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUsageDelegate(void* @this, uint* pUsage);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetEvictionPriorityDelegate(void* @this, ushort EvictionPriority);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetEvictionPriorityDelegate(void* @this, ushort* pEvictionPriority);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSharedHandle(void* @this, IntPtr* pSharedHandle)
	{
		IDXGIResource iDXGIResource = null;
		try
		{
			iDXGIResource = (IDXGIResource)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr sharedHandle = iDXGIResource.SharedHandle;
			*pSharedHandle = sharedHandle;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIResource, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUsage(void* @this, uint* pUsage)
	{
		IDXGIResource iDXGIResource = null;
		try
		{
			iDXGIResource = (IDXGIResource)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			uint usage = iDXGIResource.Usage;
			*pUsage = usage;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIResource, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetEvictionPriority(void* @this, ushort EvictionPriority)
	{
		IDXGIResource iDXGIResource = null;
		try
		{
			iDXGIResource = (IDXGIResource)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIResource.SetEvictionPriority(EvictionPriority);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIResource, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetEvictionPriority(void* @this, ushort* pEvictionPriority)
	{
		IDXGIResource iDXGIResource = null;
		try
		{
			iDXGIResource = (IDXGIResource)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ushort evictionPriority = iDXGIResource.EvictionPriority;
			*pEvictionPriority = evictionPriority;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIResource, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIResourceVTable()
	{
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetSharedHandle));
		AddMethod((delegate*<void*, uint*, int>)(&GetUsage));
		AddMethod((delegate*<void*, ushort, int>)(&SetEvictionPriority));
		AddMethod((delegate*<void*, ushort*, int>)(&GetEvictionPriority));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIResource), new __MicroComIDXGIResourceVTable().CreateVTable());
	}
}
