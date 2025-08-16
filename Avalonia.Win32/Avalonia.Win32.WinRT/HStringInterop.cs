using System;

namespace Avalonia.Win32.WinRT;

internal class HStringInterop : IDisposable
{
	private IntPtr _s;

	private readonly bool _owns;

	public IntPtr Handle => _s;

	public unsafe string? Value
	{
		get
		{
			if (_s == IntPtr.Zero)
			{
				return null;
			}
			uint length = default(uint);
			return new string(NativeWinRTMethods.WindowsGetStringRawBuffer(_s, &length), 0, (int)length);
		}
	}

	public HStringInterop(string? s)
	{
		_s = ((s == null) ? IntPtr.Zero : NativeWinRTMethods.WindowsCreateString(s));
		_owns = true;
	}

	public HStringInterop(IntPtr str, bool owns = false)
	{
		_s = str;
		_owns = owns;
	}

	public void Dispose()
	{
		if (_s != IntPtr.Zero && _owns)
		{
			NativeWinRTMethods.WindowsDeleteString(_s);
			_s = IntPtr.Zero;
		}
	}
}
