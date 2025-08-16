using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.X11;

internal class X11PlatformLifetimeEvents : IDisposable, IPlatformLifetimeEventsImpl
{
	private readonly AvaloniaX11Platform _platform;

	private const nuint SmcSaveYourselfProcMask = 1u;

	private const nuint SmcDieProcMask = 2u;

	private const nuint SmcSaveCompleteProcMask = 4u;

	private const nuint SmcShutdownCancelledProcMask = 8u;

	private static readonly ConcurrentDictionary<IntPtr, X11PlatformLifetimeEvents> s_nativeToManagedMapper = new ConcurrentDictionary<IntPtr, X11PlatformLifetimeEvents>();

	private static readonly SMLib.SmcSaveYourselfProc s_saveYourselfProcDelegate = SmcSaveYourselfHandler;

	private static readonly SMLib.SmcDieProc s_dieDelegate = SmcDieHandler;

	private static readonly SMLib.SmcShutdownCancelledProc s_shutdownCancelledDelegate = SmcShutdownCancelledHandler;

	private static readonly SMLib.SmcSaveCompleteProc s_saveCompleteDelegate = SmcSaveCompleteHandler;

	private static readonly SMLib.SmcInteractProc s_smcInteractDelegate = StaticInteractHandler;

	private static readonly SMLib.SmcErrorHandler s_smcErrorHandlerDelegate = StaticErrorHandler;

	private static readonly ICELib.IceErrorHandler s_iceErrorHandlerDelegate = StaticErrorHandler;

	private static readonly ICELib.IceIOErrorHandler s_iceIoErrorHandlerDelegate = StaticIceIOErrorHandler;

	private unsafe static readonly SMLib.IceWatchProc s_iceWatchProcDelegate = IceWatchHandler;

	private static SMLib.SmcCallbacks s_callbacks = new SMLib.SmcCallbacks
	{
		ShutdownCancelled = Marshal.GetFunctionPointerForDelegate(s_shutdownCancelledDelegate),
		Die = Marshal.GetFunctionPointerForDelegate(s_dieDelegate),
		SaveYourself = Marshal.GetFunctionPointerForDelegate(s_saveYourselfProcDelegate),
		SaveComplete = Marshal.GetFunctionPointerForDelegate(s_saveCompleteDelegate)
	};

	private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

	private readonly IntPtr _currentIceConn;

	private readonly IntPtr _currentSmcConn;

	private bool _saveYourselfPhase;

	public event EventHandler<ShutdownRequestedEventArgs>? ShutdownRequested;

	internal X11PlatformLifetimeEvents(AvaloniaX11Platform platform)
	{
		_platform = platform;
		if (ICELib.IceAddConnectionWatch(Marshal.GetFunctionPointerForDelegate(s_iceWatchProcDelegate), IntPtr.Zero) == 0)
		{
			Logger.TryGet(LogEventLevel.Warning, "X11Platform")?.Log(this, "SMLib was unable to add an ICE connection watcher.");
			return;
		}
		byte[] array = new byte[255];
		IntPtr clientIdRet = IntPtr.Zero;
		IntPtr intPtr = SMLib.SmcOpenConnection(null, IntPtr.Zero, 1, 0, 15u, ref s_callbacks, null, ref clientIdRet, array.Length, array);
		if (intPtr == IntPtr.Zero)
		{
			Logger.TryGet(LogEventLevel.Warning, "X11Platform")?.Log(this, "SMLib/ICELib reported a new error: " + Encoding.ASCII.GetString(array));
			return;
		}
		if (!s_nativeToManagedMapper.TryAdd(intPtr, this))
		{
			Logger.TryGet(LogEventLevel.Warning, "X11Platform")?.Log(this, "SMLib was unable to add this instance to the native to managed map.");
			return;
		}
		SMLib.SmcSetErrorHandler(Marshal.GetFunctionPointerForDelegate(s_smcErrorHandlerDelegate));
		ICELib.IceSetErrorHandler(Marshal.GetFunctionPointerForDelegate(s_iceErrorHandlerDelegate));
		ICELib.IceSetIOErrorHandler(Marshal.GetFunctionPointerForDelegate(s_iceIoErrorHandlerDelegate));
		_currentSmcConn = intPtr;
		_currentIceConn = SMLib.SmcGetIceConnection(intPtr);
		Task.Run(delegate
		{
			CancellationToken token = _cancellationTokenSource.Token;
			while (!token.IsCancellationRequested)
			{
				HandleRequests();
			}
		}, _cancellationTokenSource.Token);
	}

	public void Dispose()
	{
		if (!(_currentSmcConn == IntPtr.Zero))
		{
			s_nativeToManagedMapper.TryRemove(_currentSmcConn, out X11PlatformLifetimeEvents _);
			SMLib.SmcCloseConnection(_currentSmcConn, 1, new string[1] { "X11PlatformLifetimeEvents was disposed in managed code." });
		}
	}

	private static void SmcSaveCompleteHandler(IntPtr smcConn, IntPtr clientData)
	{
		GetInstance(smcConn)?.SaveCompleteHandler();
	}

	private static X11PlatformLifetimeEvents? GetInstance(IntPtr smcConn)
	{
		if (!s_nativeToManagedMapper.TryGetValue(smcConn, out X11PlatformLifetimeEvents value))
		{
			return null;
		}
		return value;
	}

	private static void SmcShutdownCancelledHandler(IntPtr smcConn, IntPtr clientData)
	{
		GetInstance(smcConn)?.ShutdownCancelledHandler();
	}

	private static void SmcDieHandler(IntPtr smcConn, IntPtr clientData)
	{
		GetInstance(smcConn)?.DieHandler();
	}

	private static void SmcSaveYourselfHandler(IntPtr smcConn, IntPtr clientData, int saveType, bool shutdown, int interactStyle, bool fast)
	{
		GetInstance(smcConn)?.SaveYourselfHandler(smcConn, clientData, shutdown, fast);
	}

	private static void StaticInteractHandler(IntPtr smcConn, IntPtr clientData)
	{
		GetInstance(smcConn)?.InteractHandler(smcConn);
	}

	private static void StaticIceIOErrorHandler(IntPtr iceConn)
	{
		Logger.TryGet(LogEventLevel.Warning, "X11Platform")?.Log(null, "ICELib reported an unknown IO Error.");
	}

	private static void StaticErrorHandler(IntPtr smcConn, bool swap, int offendingMinorOpcode, nuint offendingSequence, int errorClass, int severity, IntPtr values)
	{
		GetInstance(smcConn)?.ErrorHandler(swap, offendingMinorOpcode, offendingSequence, errorClass, severity, values);
	}

	private void ErrorHandler(bool swap, int offendingMinorOpcode, nuint offendingSequence, int errorClass, int severity, IntPtr values)
	{
		Logger.TryGet(LogEventLevel.Warning, "X11Platform")?.Log(this, "SMLib reported an error:" + $" severity {severity:X}" + $" mOpcode {offendingMinorOpcode:X}" + $" mSeq {offendingSequence:X}" + $" errClass {errorClass:X}.");
	}

	private void HandleRequests()
	{
		if (ICELib.IceProcessMessages(_currentIceConn, out var _, out var _) == ICELib.IceProcessMessagesStatus.IceProcessMessagesIoError)
		{
			Logger.TryGet(LogEventLevel.Warning, "X11Platform")?.Log(this, "SMLib lost its underlying ICE connection.");
			Dispose();
		}
	}

	private void SaveCompleteHandler()
	{
		_saveYourselfPhase = false;
	}

	private void ShutdownCancelledHandler()
	{
		if (_saveYourselfPhase)
		{
			SMLib.SmcSaveYourselfDone(_currentSmcConn, success: true);
		}
		_saveYourselfPhase = false;
	}

	private void DieHandler()
	{
		Dispose();
	}

	private void SaveYourselfHandler(IntPtr smcConn, IntPtr clientData, bool shutdown, bool fast)
	{
		if (_saveYourselfPhase)
		{
			SMLib.SmcSaveYourselfDone(smcConn, success: true);
		}
		_saveYourselfPhase = true;
		if (shutdown && !fast)
		{
			SMLib.SmcInteractRequest(smcConn, SMLib.SmDialogValue.SmDialogError, Marshal.GetFunctionPointerForDelegate(s_smcInteractDelegate), clientData);
			return;
		}
		SMLib.SmcSaveYourselfDone(smcConn, success: true);
		_saveYourselfPhase = false;
	}

	private void InteractHandler(IntPtr smcConn)
	{
		Dispatcher.UIThread.Post(delegate
		{
			ActualInteractHandler(smcConn);
		});
	}

	private void ActualInteractHandler(IntPtr smcConn)
	{
		ShutdownRequestedEventArgs shutdownRequestedEventArgs = new ShutdownRequestedEventArgs();
		X11PlatformOptions options = _platform.Options;
		if (options != null && options.EnableSessionManagement)
		{
			this.ShutdownRequested?.Invoke(this, shutdownRequestedEventArgs);
		}
		SMLib.SmcInteractDone(smcConn, shutdownRequestedEventArgs.Cancel);
		if (!shutdownRequestedEventArgs.Cancel)
		{
			_saveYourselfPhase = false;
			SMLib.SmcSaveYourselfDone(smcConn, success: true);
		}
	}

	private unsafe static void IceWatchHandler(IntPtr iceConn, IntPtr clientData, bool opening, IntPtr* watchData)
	{
		if (opening)
		{
			ICELib.IceRemoveConnectionWatch(Marshal.GetFunctionPointerForDelegate(s_iceWatchProcDelegate), IntPtr.Zero);
		}
	}
}
