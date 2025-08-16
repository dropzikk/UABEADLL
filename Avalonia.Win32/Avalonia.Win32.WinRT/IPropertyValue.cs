using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IPropertyValue : IInspectable, IUnknown, IDisposable
{
	PropertyType Type { get; }

	int IsNumericScalar { get; }

	byte UInt8 { get; }

	short Int16 { get; }

	ushort UInt16 { get; }

	int Int32 { get; }

	uint UInt32 { get; }

	long Int64 { get; }

	ulong UInt64 { get; }

	float Single { get; }

	double Double { get; }

	char Char16 { get; }

	int Boolean { get; }

	IntPtr String { get; }

	Guid Guid { get; }

	unsafe void GetDateTime(void* value);

	unsafe void GetTimeSpan(void* value);

	unsafe void GetPoint(void* value);

	unsafe void GetSize(void* value);

	unsafe void GetRect(void* value);

	unsafe byte* GetUInt8Array(uint* __valueSize);

	unsafe short* GetInt16Array(uint* __valueSize);

	unsafe ushort* GetUInt16Array(uint* __valueSize);

	unsafe int* GetInt32Array(uint* __valueSize);

	unsafe uint* GetUInt32Array(uint* __valueSize);

	unsafe long* GetInt64Array(uint* __valueSize);

	unsafe ulong* GetUInt64Array(uint* __valueSize);

	unsafe float* GetSingleArray(uint* __valueSize);

	unsafe double* GetDoubleArray(uint* __valueSize);

	unsafe char* GetChar16Array(uint* __valueSize);

	unsafe int* GetBooleanArray(uint* __valueSize);

	unsafe IntPtr* GetStringArray(uint* __valueSize);

	unsafe void** GetInspectableArray(uint* __valueSize);

	unsafe Guid* GetGuidArray(uint* __valueSize);

	unsafe void* GetDateTimeArray(uint* __valueSize);

	unsafe void* GetTimeSpanArray(uint* __valueSize);

	unsafe void* GetPointArray(uint* __valueSize);

	unsafe void* GetSizeArray(uint* __valueSize);

	unsafe void* GetRectArray(uint* __valueSize);
}
