using System;
using System.Runtime.InteropServices;

namespace Avalonia.X11;

internal static class SMLib
{
	public enum SmDialogValue
	{
		SmDialogError
	}

	public struct SmcCallbacks
	{
		public IntPtr SaveYourself;

		private readonly IntPtr Unused0;

		public IntPtr Die;

		private readonly IntPtr Unused1;

		public IntPtr SaveComplete;

		private readonly IntPtr Unused2;

		public IntPtr ShutdownCancelled;

		private readonly IntPtr Unused3;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void IceWatchProc(IntPtr iceConn, IntPtr clientData, bool opening, IntPtr* watchData);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SmcDieProc(IntPtr smcConn, IntPtr clientData);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SmcInteractProc(IntPtr smcConn, IntPtr clientData);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SmcSaveCompleteProc(IntPtr smcConn, IntPtr clientData);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SmcSaveYourselfProc(IntPtr smcConn, IntPtr clientData, int saveType, bool shutdown, int interactStyle, bool fast);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SmcShutdownCancelledProc(IntPtr smcConn, IntPtr clientData);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SmcErrorHandler(IntPtr smcConn, bool swap, int offendingMinorOpcode, nuint offendingSequence, int errorClass, int severity, IntPtr values);

	private const string LibSm = "libSM.so.6";

	[DllImport("libSM.so.6", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
	public static extern IntPtr SmcOpenConnection([MarshalAs(UnmanagedType.LPStr)] string? networkId, IntPtr content, int xsmpMajorRev, int xsmpMinorRev, nuint mask, ref SmcCallbacks callbacks, [MarshalAs(UnmanagedType.LPStr)] string? previousId, ref IntPtr clientIdRet, int errorLength, [Out] byte[] errorStringRet);

	[DllImport("libSM.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern int SmcCloseConnection(IntPtr smcConn, int count, string[] reasonMsgs);

	[DllImport("libSM.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern void SmcSaveYourselfDone(IntPtr smcConn, bool success);

	[DllImport("libSM.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern int SmcInteractRequest(IntPtr smcConn, SmDialogValue dialogType, IntPtr interactProc, IntPtr clientData);

	[DllImport("libSM.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern void SmcInteractDone(IntPtr smcConn, bool success);

	[DllImport("libSM.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern IntPtr SmcGetIceConnection(IntPtr smcConn);

	[DllImport("libSM.so.6", CallingConvention = CallingConvention.StdCall)]
	public static extern IntPtr SmcSetErrorHandler(IntPtr handler);
}
