using System;
using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal class NOTIFYICONDATA
{
	public int cbSize = Marshal.SizeOf<NOTIFYICONDATA>();

	public IntPtr hWnd;

	public int uID;

	public NIF uFlags;

	public int uCallbackMessage;

	public IntPtr hIcon;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
	public string? szTip;

	public int dwState;

	public int dwStateMask;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
	public string? szInfo;

	public int uTimeoutOrVersion;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
	public string? szInfoTitle;

	public NIIF dwInfoFlags;
}
