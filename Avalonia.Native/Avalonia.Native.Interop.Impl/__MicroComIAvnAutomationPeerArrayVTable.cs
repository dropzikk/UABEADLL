using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnAutomationPeerArrayVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint GetCountDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDelegate(void* @this, uint index, void** ppv);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint GetCount(void* @this)
	{
		IAvnAutomationPeerArray avnAutomationPeerArray = null;
		try
		{
			avnAutomationPeerArray = (IAvnAutomationPeerArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnAutomationPeerArray.Count;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeerArray, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Get(void* @this, uint index, void** ppv)
	{
		IAvnAutomationPeerArray avnAutomationPeerArray = null;
		try
		{
			avnAutomationPeerArray = (IAvnAutomationPeerArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnAutomationPeer obj = avnAutomationPeerArray.Get(index);
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnAutomationPeerArray, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnAutomationPeerArrayVTable()
	{
		AddMethod((delegate*<void*, uint>)(&GetCount));
		AddMethod((delegate*<void*, uint, void**, int>)(&Get));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnAutomationPeerArray), new __MicroComIAvnAutomationPeerArrayVTable().CreateVTable());
	}
}
