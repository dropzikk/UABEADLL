using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnStringArrayVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint GetCountDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDelegate(void* @this, uint index, void** ppv);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint GetCount(void* @this)
	{
		IAvnStringArray avnStringArray = null;
		try
		{
			avnStringArray = (IAvnStringArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return avnStringArray.Count;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnStringArray, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Get(void* @this, uint index, void** ppv)
	{
		IAvnStringArray avnStringArray = null;
		try
		{
			avnStringArray = (IAvnStringArray)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnString obj = avnStringArray.Get(index);
			*ppv = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnStringArray, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnStringArrayVTable()
	{
		AddMethod((delegate*<void*, uint>)(&GetCount));
		AddMethod((delegate*<void*, uint, void**, int>)(&Get));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnStringArray), new __MicroComIAvnStringArrayVTable().CreateVTable());
	}
}
