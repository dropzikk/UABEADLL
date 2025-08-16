using System;
using System.Runtime.InteropServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal class AvnString : NativeCallbackBase, IAvnString, IUnknown, IDisposable
{
	private IntPtr _native;

	private int _nativeLen;

	public string String { get; }

	public byte[] Bytes => Encoding.UTF8.GetBytes(String);

	public AvnString(string s)
	{
		String = s;
	}

	public unsafe void* Pointer()
	{
		EnsureNative();
		return _native.ToPointer();
	}

	public int Length()
	{
		EnsureNative();
		return _nativeLen;
	}

	protected override void Destroyed()
	{
		if (_native != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(_native);
			_native = IntPtr.Zero;
		}
	}

	private unsafe void EnsureNative()
	{
		if (!string.IsNullOrEmpty(String) && _native == IntPtr.Zero)
		{
			_nativeLen = Encoding.UTF8.GetByteCount(String);
			_native = Marshal.AllocHGlobal(_nativeLen + 1);
			byte* ptr = (byte*)_native.ToPointer();
			fixed (char* @string = String)
			{
				Encoding.UTF8.GetBytes(@string, String.Length, ptr, _nativeLen);
			}
			ptr[_nativeLen] = 0;
		}
	}
}
