using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Logging;
using Avalonia.Threading;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class Win32DispatcherImpl : IControlledDispatcherImpl, IDispatcherImplWithPendingInput, IDispatcherImpl
{
	private readonly IntPtr _messageWindow;

	private static Thread? s_uiThread;

	private readonly Stopwatch _clock = Stopwatch.StartNew();

	internal const int SignalW = -559038801;

	internal const int SignalL = 305419896;

	public bool CurrentThreadIsLoopThread => s_uiThread == Thread.CurrentThread;

	public bool CanQueryPendingInput => true;

	public bool HasPendingInput => UnmanagedMethods.MsgWaitForMultipleObjectsEx(0, null, 0, UnmanagedMethods.QueueStatusFlags.QS_INPUT | UnmanagedMethods.QueueStatusFlags.QS_POSTMESSAGE | UnmanagedMethods.QueueStatusFlags.QS_EVENT, UnmanagedMethods.MsgWaitForMultipleObjectsFlags.MWMO_INPUTAVAILABLE) == 0;

	public long Now => _clock.ElapsedMilliseconds;

	public event Action? Signaled;

	public event Action? Timer;

	public Win32DispatcherImpl(IntPtr messageWindow)
	{
		_messageWindow = messageWindow;
		s_uiThread = Thread.CurrentThread;
	}

	public void Signal()
	{
		UnmanagedMethods.PostMessage(_messageWindow, 1024u, new IntPtr(-559038801), new IntPtr(305419896));
	}

	public void DispatchWorkItem()
	{
		this.Signaled?.Invoke();
	}

	public void FireTimer()
	{
		this.Timer?.Invoke();
	}

	public void UpdateTimer(long? dueTimeInMs)
	{
		if (!dueTimeInMs.HasValue)
		{
			UnmanagedMethods.KillTimer(_messageWindow, (IntPtr)1);
			return;
		}
		uint uElapse = (uint)Math.Min(2147483637L, Math.Max(1L, Now - dueTimeInMs.Value));
		UnmanagedMethods.SetTimer(_messageWindow, (IntPtr)1, uElapse, null);
	}

	public void RunLoop(CancellationToken cancellationToken)
	{
		int num = 0;
		UnmanagedMethods.MSG lpmsg;
		while (!cancellationToken.IsCancellationRequested && (num = UnmanagedMethods.GetMessage(out lpmsg, IntPtr.Zero, 0u, 0u)) > 0)
		{
			UnmanagedMethods.TranslateMessage(ref lpmsg);
			UnmanagedMethods.DispatchMessage(ref lpmsg);
		}
		if (num < 0)
		{
			Logger.TryGet(LogEventLevel.Error, "Win32Platform")?.Log(this, "Unmanaged error in {0}. Error Code: {1}", "RunLoop", Marshal.GetLastWin32Error());
		}
	}
}
