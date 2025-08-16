using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com.Impl;

internal class __MicroComIEnumFORMATETCVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint NextDelegate(void* @this, uint celt, FORMATETC* rgelt, uint* pceltFetched);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate uint SkipDelegate(void* @this, uint celt);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ResetDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CloneDelegate(void* @this, void** ppenum);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint Next(void* @this, uint celt, FORMATETC* rgelt, uint* pceltFetched)
	{
		IEnumFORMATETC enumFORMATETC = null;
		try
		{
			enumFORMATETC = (IEnumFORMATETC)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return enumFORMATETC.Next(celt, rgelt, pceltFetched);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(enumFORMATETC, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static uint Skip(void* @this, uint celt)
	{
		IEnumFORMATETC enumFORMATETC = null;
		try
		{
			enumFORMATETC = (IEnumFORMATETC)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return enumFORMATETC.Skip(celt);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(enumFORMATETC, e);
			return 0u;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Reset(void* @this)
	{
		IEnumFORMATETC enumFORMATETC = null;
		try
		{
			enumFORMATETC = (IEnumFORMATETC)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			enumFORMATETC.Reset();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(enumFORMATETC, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Clone(void* @this, void** ppenum)
	{
		IEnumFORMATETC enumFORMATETC = null;
		try
		{
			enumFORMATETC = (IEnumFORMATETC)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IEnumFORMATETC obj = enumFORMATETC.Clone();
			*ppenum = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(enumFORMATETC, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIEnumFORMATETCVTable()
	{
		AddMethod((delegate*<void*, uint, FORMATETC*, uint*, uint>)(&Next));
		AddMethod((delegate*<void*, uint, uint>)(&Skip));
		AddMethod((delegate*<void*, int>)(&Reset));
		AddMethod((delegate*<void*, void**, int>)(&Clone));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IEnumFORMATETC), new __MicroComIEnumFORMATETCVTable().CreateVTable());
	}
}
