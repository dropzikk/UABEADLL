using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIKeyedMutexVTable : __MicroComIDXGIDeviceSubObjectVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int AcquireSyncDelegate(void* @this, ulong Key, uint dwMilliseconds);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ReleaseSyncDelegate(void* @this, ulong Key);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int AcquireSync(void* @this, ulong Key, uint dwMilliseconds)
	{
		IDXGIKeyedMutex iDXGIKeyedMutex = null;
		try
		{
			iDXGIKeyedMutex = (IDXGIKeyedMutex)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIKeyedMutex.AcquireSync(Key, dwMilliseconds);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIKeyedMutex, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ReleaseSync(void* @this, ulong Key)
	{
		IDXGIKeyedMutex iDXGIKeyedMutex = null;
		try
		{
			iDXGIKeyedMutex = (IDXGIKeyedMutex)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIKeyedMutex.ReleaseSync(Key);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIKeyedMutex, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIKeyedMutexVTable()
	{
		AddMethod((delegate*<void*, ulong, uint, int>)(&AcquireSync));
		AddMethod((delegate*<void*, ulong, int>)(&ReleaseSync));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIKeyedMutex), new __MicroComIDXGIKeyedMutexVTable().CreateVTable());
	}
}
