using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal class WinRTPropertyValue : WinRTInspectable, IPropertyValue, IInspectable, IUnknown, IDisposable
{
	private readonly float[]? _singleArray;

	public PropertyType Type { get; }

	public int IsNumericScalar { get; }

	public byte UInt8 { get; }

	public short Int16 { get; }

	public ushort UInt16 { get; }

	public int Int32 { get; }

	public uint UInt32 { get; }

	public long Int64 { get; }

	public ulong UInt64 { get; }

	public float Single { get; }

	public double Double { get; }

	public char Char16 { get; }

	public int Boolean { get; }

	public IntPtr String { get; }

	public Guid Guid { get; }

	private static COMException NotImplemented => new COMException("Not supported", -2147467263);

	public WinRTPropertyValue(float f)
	{
		Type = PropertyType.Single;
		Single = f;
	}

	public WinRTPropertyValue(uint u)
	{
		UInt32 = u;
		Type = PropertyType.UInt32;
	}

	public WinRTPropertyValue(float[] uiColor)
	{
		Type = PropertyType.SingleArray;
		_singleArray = uiColor;
	}

	public unsafe void GetDateTime(void* value)
	{
		throw NotImplemented;
	}

	public unsafe void GetTimeSpan(void* value)
	{
		throw NotImplemented;
	}

	public unsafe void GetPoint(void* value)
	{
		throw NotImplemented;
	}

	public unsafe void GetSize(void* value)
	{
		throw NotImplemented;
	}

	public unsafe void GetRect(void* value)
	{
		throw NotImplemented;
	}

	public unsafe byte* GetUInt8Array(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe short* GetInt16Array(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe ushort* GetUInt16Array(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe int* GetInt32Array(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe uint* GetUInt32Array(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe long* GetInt64Array(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe ulong* GetUInt64Array(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe float* GetSingleArray(uint* __valueSize)
	{
		if (_singleArray == null)
		{
			throw NotImplemented;
		}
		*__valueSize = (uint)_singleArray.Length;
		IntPtr intPtr = Marshal.AllocCoTaskMem(_singleArray.Length * Unsafe.SizeOf<float>());
		Marshal.Copy(_singleArray, 0, intPtr, _singleArray.Length);
		return (float*)(void*)intPtr;
	}

	public unsafe double* GetDoubleArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe char* GetChar16Array(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe int* GetBooleanArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe IntPtr* GetStringArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe void** GetInspectableArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe Guid* GetGuidArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe void* GetDateTimeArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe void* GetTimeSpanArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe void* GetPointArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe void* GetSizeArray(uint* __valueSize)
	{
		throw NotImplemented;
	}

	public unsafe void* GetRectArray(uint* __valueSize)
	{
		throw NotImplemented;
	}
}
