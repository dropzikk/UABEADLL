using System;
using System.Runtime.InteropServices;

namespace Avalonia.X11;

internal static class ICELib
{
	public enum IceProcessMessagesStatus
	{
		IceProcessMessagesIoError = 1
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void IceErrorHandler(IntPtr iceConn, bool swap, int offendingMinorOpcode, nuint offendingSequence, int errorClass, int severity, IntPtr values);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void IceIOErrorHandler(IntPtr iceConn);

	private const string LibIce = "libICE.so.6";

	[DllImport("libICE.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern int IceAddConnectionWatch(IntPtr watchProc, IntPtr clientData);

	[DllImport("libICE.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern void IceRemoveConnectionWatch(IntPtr watchProc, IntPtr clientData);

	[DllImport("libICE.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern IceProcessMessagesStatus IceProcessMessages(IntPtr iceConn, out IntPtr replyWait, out bool replyReadyRet);

	[DllImport("libICE.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern IntPtr IceSetErrorHandler(IntPtr handler);

	[DllImport("libICE.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern IntPtr IceSetIOErrorHandler(IntPtr handler);
}
