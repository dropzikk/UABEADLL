using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIPropertyValueProxy : __MicroComIInspectableProxy, IPropertyValue, IInspectable, IUnknown, IDisposable
{
	public unsafe PropertyType Type
	{
		get
		{
			PropertyType result = PropertyType.Empty;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetType failed", num);
			}
			return result;
		}
	}

	public unsafe int IsNumericScalar
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetIsNumericScalar failed", num);
			}
			return result;
		}
	}

	public unsafe byte UInt8
	{
		get
		{
			byte result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetUInt8 failed", num);
			}
			return result;
		}
	}

	public unsafe short Int16
	{
		get
		{
			short result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetInt16 failed", num);
			}
			return result;
		}
	}

	public unsafe ushort UInt16
	{
		get
		{
			ushort result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetUInt16 failed", num);
			}
			return result;
		}
	}

	public unsafe int Int32
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetInt32 failed", num);
			}
			return result;
		}
	}

	public unsafe uint UInt32
	{
		get
		{
			uint result = 0u;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetUInt32 failed", num);
			}
			return result;
		}
	}

	public unsafe long Int64
	{
		get
		{
			long result = 0L;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetInt64 failed", num);
			}
			return result;
		}
	}

	public unsafe ulong UInt64
	{
		get
		{
			ulong result = 0uL;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetUInt64 failed", num);
			}
			return result;
		}
	}

	public unsafe float Single
	{
		get
		{
			float result = 0f;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetSingle failed", num);
			}
			return result;
		}
	}

	public unsafe double Double
	{
		get
		{
			double result = 0.0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetDouble failed", num);
			}
			return result;
		}
	}

	public unsafe char Char16
	{
		get
		{
			char result = '\0';
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetChar16 failed", num);
			}
			return result;
		}
	}

	public unsafe int Boolean
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetBoolean failed", num);
			}
			return result;
		}
	}

	public unsafe IntPtr String
	{
		get
		{
			IntPtr result = default(IntPtr);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetString failed", num);
			}
			return result;
		}
	}

	public unsafe Guid Guid
	{
		get
		{
			Guid result = default(Guid);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetGuid failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 39;

	public unsafe void GetDateTime(void* value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("GetDateTime failed", num);
		}
	}

	public unsafe void GetTimeSpan(void* value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("GetTimeSpan failed", num);
		}
	}

	public unsafe void GetPoint(void* value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("GetPoint failed", num);
		}
	}

	public unsafe void GetSize(void* value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("GetSize failed", num);
		}
	}

	public unsafe void GetRect(void* value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("GetRect failed", num);
		}
	}

	public unsafe byte* GetUInt8Array(uint* __valueSize)
	{
		byte* result = default(byte*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetUInt8Array failed", num);
		}
		return result;
	}

	public unsafe short* GetInt16Array(uint* __valueSize)
	{
		short* result = default(short*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 21])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetInt16Array failed", num);
		}
		return result;
	}

	public unsafe ushort* GetUInt16Array(uint* __valueSize)
	{
		ushort* result = default(ushort*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 22])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetUInt16Array failed", num);
		}
		return result;
	}

	public unsafe int* GetInt32Array(uint* __valueSize)
	{
		int* result = default(int*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 23])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetInt32Array failed", num);
		}
		return result;
	}

	public unsafe uint* GetUInt32Array(uint* __valueSize)
	{
		uint* result = default(uint*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 24])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetUInt32Array failed", num);
		}
		return result;
	}

	public unsafe long* GetInt64Array(uint* __valueSize)
	{
		long* result = default(long*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 25])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetInt64Array failed", num);
		}
		return result;
	}

	public unsafe ulong* GetUInt64Array(uint* __valueSize)
	{
		ulong* result = default(ulong*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 26])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetUInt64Array failed", num);
		}
		return result;
	}

	public unsafe float* GetSingleArray(uint* __valueSize)
	{
		float* result = default(float*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 27])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetSingleArray failed", num);
		}
		return result;
	}

	public unsafe double* GetDoubleArray(uint* __valueSize)
	{
		double* result = default(double*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 28])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetDoubleArray failed", num);
		}
		return result;
	}

	public unsafe char* GetChar16Array(uint* __valueSize)
	{
		char* result = default(char*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 29])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetChar16Array failed", num);
		}
		return result;
	}

	public unsafe int* GetBooleanArray(uint* __valueSize)
	{
		int* result = default(int*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 30])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetBooleanArray failed", num);
		}
		return result;
	}

	public unsafe IntPtr* GetStringArray(uint* __valueSize)
	{
		IntPtr* result = default(IntPtr*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 31])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetStringArray failed", num);
		}
		return result;
	}

	public unsafe void** GetInspectableArray(uint* __valueSize)
	{
		void** result = default(void**);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 32])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetInspectableArray failed", num);
		}
		return result;
	}

	public unsafe Guid* GetGuidArray(uint* __valueSize)
	{
		Guid* result = default(Guid*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 33])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetGuidArray failed", num);
		}
		return result;
	}

	public unsafe void* GetDateTimeArray(uint* __valueSize)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 34])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetDateTimeArray failed", num);
		}
		return result;
	}

	public unsafe void* GetTimeSpanArray(uint* __valueSize)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 35])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetTimeSpanArray failed", num);
		}
		return result;
	}

	public unsafe void* GetPointArray(uint* __valueSize)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 36])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetPointArray failed", num);
		}
		return result;
	}

	public unsafe void* GetSizeArray(uint* __valueSize)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 37])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetSizeArray failed", num);
		}
		return result;
	}

	public unsafe void* GetRectArray(uint* __valueSize)
	{
		void* result = default(void*);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 38])(base.PPV, __valueSize, &result);
		if (num != 0)
		{
			throw new COMException("GetRectArray failed", num);
		}
		return result;
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IPropertyValue), new Guid("4BD682DD-7554-40E9-9A9B-82654EDE7E62"), (IntPtr p, bool owns) => new __MicroComIPropertyValueProxy(p, owns));
	}

	protected __MicroComIPropertyValueProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
