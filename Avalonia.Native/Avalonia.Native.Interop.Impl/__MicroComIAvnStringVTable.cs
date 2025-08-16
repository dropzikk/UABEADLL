using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnStringVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int PointerDelegate(void* @this, void** retOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int LengthDelegate(void* @this, int* ret);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Pointer(void* @this, void** retOut)
	{
		IAvnString avnString = null;
		try
		{
			avnString = (IAvnString)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* ptr = avnString.Pointer();
			*retOut = ptr;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnString, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Length(void* @this, int* ret)
	{
		IAvnString avnString = null;
		try
		{
			avnString = (IAvnString)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = avnString.Length();
			*ret = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnString, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnStringVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&Pointer));
		AddMethod((delegate*<void*, int*, int>)(&Length));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnString), new __MicroComIAvnStringVTable().CreateVTable());
	}
}
