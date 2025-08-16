using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.Platform.Interop;

internal class Utf8Buffer : SafeHandle
{
	private GCHandle _gcHandle;

	private byte[]? _data;

	public int ByteLen
	{
		get
		{
			byte[]? data = _data;
			if (data == null)
			{
				return 0;
			}
			return data.Length;
		}
	}

	public override bool IsInvalid => handle == IntPtr.Zero;

	public Utf8Buffer(string? s)
		: base(IntPtr.Zero, ownsHandle: true)
	{
		if (s != null)
		{
			_data = Encoding.UTF8.GetBytes(s);
			_gcHandle = GCHandle.Alloc(_data, GCHandleType.Pinned);
			handle = _gcHandle.AddrOfPinnedObject();
		}
	}

	protected override bool ReleaseHandle()
	{
		if (handle != IntPtr.Zero)
		{
			handle = IntPtr.Zero;
			_data = null;
			_gcHandle.Free();
		}
		return true;
	}

	public unsafe static string? StringFromPtr(IntPtr s)
	{
		byte* ptr = (byte*)(void*)s;
		if (ptr == null)
		{
			return null;
		}
		int i;
		for (i = 0; ptr[i] != 0; i++)
		{
		}
		byte[] array = ArrayPool<byte>.Shared.Rent(i);
		try
		{
			Marshal.Copy(s, array, 0, i);
			return Encoding.UTF8.GetString(array, 0, i);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(array);
		}
	}
}
