using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIPropertyValueVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTypeDelegate(void* @this, PropertyType* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetIsNumericScalarDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUInt8Delegate(void* @this, byte* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetInt16Delegate(void* @this, short* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUInt16Delegate(void* @this, ushort* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetInt32Delegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUInt32Delegate(void* @this, uint* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetInt64Delegate(void* @this, long* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUInt64Delegate(void* @this, ulong* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSingleDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDoubleDelegate(void* @this, double* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetChar16Delegate(void* @this, char* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBooleanDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStringDelegate(void* @this, IntPtr* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetGuidDelegate(void* @this, Guid* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDateTimeDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTimeSpanDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPointDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSizeDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRectDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUInt8ArrayDelegate(void* @this, uint* __valueSize, byte** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetInt16ArrayDelegate(void* @this, uint* __valueSize, short** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUInt16ArrayDelegate(void* @this, uint* __valueSize, ushort** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetInt32ArrayDelegate(void* @this, uint* __valueSize, int** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUInt32ArrayDelegate(void* @this, uint* __valueSize, uint** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetInt64ArrayDelegate(void* @this, uint* __valueSize, long** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetUInt64ArrayDelegate(void* @this, uint* __valueSize, ulong** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSingleArrayDelegate(void* @this, uint* __valueSize, float** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDoubleArrayDelegate(void* @this, uint* __valueSize, double** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetChar16ArrayDelegate(void* @this, uint* __valueSize, char** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBooleanArrayDelegate(void* @this, uint* __valueSize, int** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStringArrayDelegate(void* @this, uint* __valueSize, IntPtr** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetInspectableArrayDelegate(void* @this, uint* __valueSize, void*** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetGuidArrayDelegate(void* @this, uint* __valueSize, Guid** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDateTimeArrayDelegate(void* @this, uint* __valueSize, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTimeSpanArrayDelegate(void* @this, uint* __valueSize, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPointArrayDelegate(void* @this, uint* __valueSize, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSizeArrayDelegate(void* @this, uint* __valueSize, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRectArrayDelegate(void* @this, uint* __valueSize, void** value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetType(void* @this, PropertyType* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			PropertyType type = propertyValue.Type;
			*value = type;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetIsNumericScalar(void* @this, int* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int isNumericScalar = propertyValue.IsNumericScalar;
			*value = isNumericScalar;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUInt8(void* @this, byte* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			byte uInt = propertyValue.UInt8;
			*value = uInt;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetInt16(void* @this, short* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			short @int = propertyValue.Int16;
			*value = @int;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUInt16(void* @this, ushort* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ushort uInt = propertyValue.UInt16;
			*value = uInt;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetInt32(void* @this, int* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int @int = propertyValue.Int32;
			*value = @int;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUInt32(void* @this, uint* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			uint uInt = propertyValue.UInt32;
			*value = uInt;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetInt64(void* @this, long* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			long @int = propertyValue.Int64;
			*value = @int;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUInt64(void* @this, ulong* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ulong uInt = propertyValue.UInt64;
			*value = uInt;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSingle(void* @this, float* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float single = propertyValue.Single;
			*value = single;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDouble(void* @this, double* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			double @double = propertyValue.Double;
			*value = @double;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetChar16(void* @this, char* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			char @char = propertyValue.Char16;
			*value = @char;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetBoolean(void* @this, int* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int boolean = propertyValue.Boolean;
			*value = boolean;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetString(void* @this, IntPtr* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr @string = propertyValue.String;
			*value = @string;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetGuid(void* @this, Guid* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Guid guid = propertyValue.Guid;
			*value = guid;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDateTime(void* @this, void* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			propertyValue.GetDateTime(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetTimeSpan(void* @this, void* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			propertyValue.GetTimeSpan(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPoint(void* @this, void* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			propertyValue.GetPoint(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSize(void* @this, void* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			propertyValue.GetSize(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRect(void* @this, void* value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			propertyValue.GetRect(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUInt8Array(void* @this, uint* __valueSize, byte** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			byte* uInt8Array = propertyValue.GetUInt8Array(__valueSize);
			*value = uInt8Array;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetInt16Array(void* @this, uint* __valueSize, short** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			short* int16Array = propertyValue.GetInt16Array(__valueSize);
			*value = int16Array;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUInt16Array(void* @this, uint* __valueSize, ushort** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ushort* uInt16Array = propertyValue.GetUInt16Array(__valueSize);
			*value = uInt16Array;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetInt32Array(void* @this, uint* __valueSize, int** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int* int32Array = propertyValue.GetInt32Array(__valueSize);
			*value = int32Array;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUInt32Array(void* @this, uint* __valueSize, uint** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			uint* uInt32Array = propertyValue.GetUInt32Array(__valueSize);
			*value = uInt32Array;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetInt64Array(void* @this, uint* __valueSize, long** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			long* int64Array = propertyValue.GetInt64Array(__valueSize);
			*value = int64Array;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetUInt64Array(void* @this, uint* __valueSize, ulong** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ulong* uInt64Array = propertyValue.GetUInt64Array(__valueSize);
			*value = uInt64Array;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSingleArray(void* @this, uint* __valueSize, float** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float* singleArray = propertyValue.GetSingleArray(__valueSize);
			*value = singleArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDoubleArray(void* @this, uint* __valueSize, double** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			double* doubleArray = propertyValue.GetDoubleArray(__valueSize);
			*value = doubleArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetChar16Array(void* @this, uint* __valueSize, char** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			char* char16Array = propertyValue.GetChar16Array(__valueSize);
			*value = char16Array;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetBooleanArray(void* @this, uint* __valueSize, int** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int* booleanArray = propertyValue.GetBooleanArray(__valueSize);
			*value = booleanArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStringArray(void* @this, uint* __valueSize, IntPtr** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr* stringArray = propertyValue.GetStringArray(__valueSize);
			*value = stringArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetInspectableArray(void* @this, uint* __valueSize, void*** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void** inspectableArray = propertyValue.GetInspectableArray(__valueSize);
			*value = inspectableArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetGuidArray(void* @this, uint* __valueSize, Guid** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Guid* guidArray = propertyValue.GetGuidArray(__valueSize);
			*value = guidArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDateTimeArray(void* @this, uint* __valueSize, void** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* dateTimeArray = propertyValue.GetDateTimeArray(__valueSize);
			*value = dateTimeArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetTimeSpanArray(void* @this, uint* __valueSize, void** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* timeSpanArray = propertyValue.GetTimeSpanArray(__valueSize);
			*value = timeSpanArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPointArray(void* @this, uint* __valueSize, void** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* pointArray = propertyValue.GetPointArray(__valueSize);
			*value = pointArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSizeArray(void* @this, uint* __valueSize, void** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* sizeArray = propertyValue.GetSizeArray(__valueSize);
			*value = sizeArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRectArray(void* @this, uint* __valueSize, void** value)
	{
		IPropertyValue propertyValue = null;
		try
		{
			propertyValue = (IPropertyValue)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			void* rectArray = propertyValue.GetRectArray(__valueSize);
			*value = rectArray;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(propertyValue, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIPropertyValueVTable()
	{
		AddMethod((delegate*<void*, PropertyType*, int>)(&GetType));
		AddMethod((delegate*<void*, int*, int>)(&GetIsNumericScalar));
		AddMethod((delegate*<void*, byte*, int>)(&GetUInt8));
		AddMethod((delegate*<void*, short*, int>)(&GetInt16));
		AddMethod((delegate*<void*, ushort*, int>)(&GetUInt16));
		AddMethod((delegate*<void*, int*, int>)(&GetInt32));
		AddMethod((delegate*<void*, uint*, int>)(&GetUInt32));
		AddMethod((delegate*<void*, long*, int>)(&GetInt64));
		AddMethod((delegate*<void*, ulong*, int>)(&GetUInt64));
		AddMethod((delegate*<void*, float*, int>)(&GetSingle));
		AddMethod((delegate*<void*, double*, int>)(&GetDouble));
		AddMethod((delegate*<void*, char*, int>)(&GetChar16));
		AddMethod((delegate*<void*, int*, int>)(&GetBoolean));
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetString));
		AddMethod((delegate*<void*, Guid*, int>)(&GetGuid));
		AddMethod((delegate*<void*, void*, int>)(&GetDateTime));
		AddMethod((delegate*<void*, void*, int>)(&GetTimeSpan));
		AddMethod((delegate*<void*, void*, int>)(&GetPoint));
		AddMethod((delegate*<void*, void*, int>)(&GetSize));
		AddMethod((delegate*<void*, void*, int>)(&GetRect));
		AddMethod((delegate*<void*, uint*, byte**, int>)(&GetUInt8Array));
		AddMethod((delegate*<void*, uint*, short**, int>)(&GetInt16Array));
		AddMethod((delegate*<void*, uint*, ushort**, int>)(&GetUInt16Array));
		AddMethod((delegate*<void*, uint*, int**, int>)(&GetInt32Array));
		AddMethod((delegate*<void*, uint*, uint**, int>)(&GetUInt32Array));
		AddMethod((delegate*<void*, uint*, long**, int>)(&GetInt64Array));
		AddMethod((delegate*<void*, uint*, ulong**, int>)(&GetUInt64Array));
		AddMethod((delegate*<void*, uint*, float**, int>)(&GetSingleArray));
		AddMethod((delegate*<void*, uint*, double**, int>)(&GetDoubleArray));
		AddMethod((delegate*<void*, uint*, char**, int>)(&GetChar16Array));
		AddMethod((delegate*<void*, uint*, int**, int>)(&GetBooleanArray));
		AddMethod((delegate*<void*, uint*, IntPtr**, int>)(&GetStringArray));
		AddMethod((delegate*<void*, uint*, void***, int>)(&GetInspectableArray));
		AddMethod((delegate*<void*, uint*, Guid**, int>)(&GetGuidArray));
		AddMethod((delegate*<void*, uint*, void**, int>)(&GetDateTimeArray));
		AddMethod((delegate*<void*, uint*, void**, int>)(&GetTimeSpanArray));
		AddMethod((delegate*<void*, uint*, void**, int>)(&GetPointArray));
		AddMethod((delegate*<void*, uint*, void**, int>)(&GetSizeArray));
		AddMethod((delegate*<void*, uint*, void**, int>)(&GetRectArray));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IPropertyValue), new __MicroComIPropertyValueVTable().CreateVTable());
	}
}
