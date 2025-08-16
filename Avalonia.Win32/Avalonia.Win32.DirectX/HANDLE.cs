using System;

namespace Avalonia.Win32.DirectX;

internal struct HANDLE
{
	public unsafe readonly void* Value;

	public unsafe static HANDLE INVALID_VALUE => new HANDLE((void*)(-1));

	public static HANDLE NULL => new HANDLE(null);

	public unsafe HANDLE(void* value)
	{
		Value = value;
	}

	public unsafe static bool operator ==(HANDLE left, HANDLE right)
	{
		return left.Value == right.Value;
	}

	public unsafe static bool operator !=(HANDLE left, HANDLE right)
	{
		return left.Value != right.Value;
	}

	public override bool Equals(object? obj)
	{
		if (obj is HANDLE other)
		{
			return Equals(other);
		}
		return false;
	}

	public unsafe bool Equals(HANDLE other)
	{
		UIntPtr value = (UIntPtr)Value;
		return value.Equals((UIntPtr)other.Value);
	}

	public unsafe override int GetHashCode()
	{
		UIntPtr value = (UIntPtr)Value;
		return value.GetHashCode();
	}

	public unsafe override string ToString()
	{
		return ((IntPtr)Value).ToString();
	}
}
