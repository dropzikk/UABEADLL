using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIVectorOfCompositionShapeVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetAtDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSizeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetViewDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IndexOfDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetAtDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InsertAtDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RemoveAtDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int AppendDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RemoveAtEndDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ClearDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetAt(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.GetAt();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSize(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.GetSize();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetView(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.GetView();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IndexOf(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.IndexOf();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetAt(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.SetAt();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int InsertAt(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.InsertAt();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RemoveAt(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.RemoveAt();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Append(void* @this, void* value)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.Append(MicroComRuntime.CreateProxyOrNullFor<ICompositionShape>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RemoveAtEnd(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.RemoveAtEnd();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Clear(void* @this)
	{
		IVectorOfCompositionShape vectorOfCompositionShape = null;
		try
		{
			vectorOfCompositionShape = (IVectorOfCompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			vectorOfCompositionShape.Clear();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(vectorOfCompositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIVectorOfCompositionShapeVTable()
	{
		AddMethod((delegate*<void*, int>)(&GetAt));
		AddMethod((delegate*<void*, int>)(&GetSize));
		AddMethod((delegate*<void*, int>)(&GetView));
		AddMethod((delegate*<void*, int>)(&IndexOf));
		AddMethod((delegate*<void*, int>)(&SetAt));
		AddMethod((delegate*<void*, int>)(&InsertAt));
		AddMethod((delegate*<void*, int>)(&RemoveAt));
		AddMethod((delegate*<void*, void*, int>)(&Append));
		AddMethod((delegate*<void*, int>)(&RemoveAtEnd));
		AddMethod((delegate*<void*, int>)(&Clear));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IVectorOfCompositionShape), new __MicroComIVectorOfCompositionShapeVTable().CreateVTable());
	}
}
