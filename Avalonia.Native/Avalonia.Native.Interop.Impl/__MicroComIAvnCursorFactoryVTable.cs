using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnCursorFactoryVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCursorDelegate(void* @this, AvnStandardCursorType cursorType, void** retOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateCustomCursorDelegate(void* @this, void* bitmapData, IntPtr length, AvnPixelSize hotPixel, void** retOut);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCursor(void* @this, AvnStandardCursorType cursorType, void** retOut)
	{
		IAvnCursorFactory avnCursorFactory = null;
		try
		{
			avnCursorFactory = (IAvnCursorFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnCursor cursor = avnCursorFactory.GetCursor(cursorType);
			*retOut = MicroComRuntime.GetNativePointer(cursor, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnCursorFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateCustomCursor(void* @this, void* bitmapData, IntPtr length, AvnPixelSize hotPixel, void** retOut)
	{
		IAvnCursorFactory avnCursorFactory = null;
		try
		{
			avnCursorFactory = (IAvnCursorFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAvnCursor obj = avnCursorFactory.CreateCustomCursor(bitmapData, length, hotPixel);
			*retOut = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnCursorFactory, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnCursorFactoryVTable()
	{
		AddMethod((delegate*<void*, AvnStandardCursorType, void**, int>)(&GetCursor));
		AddMethod((delegate*<void*, void*, IntPtr, AvnPixelSize, void**, int>)(&CreateCustomCursor));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnCursorFactory), new __MicroComIAvnCursorFactoryVTable().CreateVTable());
	}
}
