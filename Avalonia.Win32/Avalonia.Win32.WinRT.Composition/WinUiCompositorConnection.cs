using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Logging;
using Avalonia.MicroCom;
using Avalonia.OpenGL.Egl;
using Avalonia.Rendering;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Composition;

internal class WinUiCompositorConnection : IRenderTimer
{
	private class RunLoopHandler : CallbackBase, IAsyncActionCompletedHandler, IUnknown, IDisposable
	{
		private readonly WinUiCompositorConnection _parent;

		private readonly Stopwatch _st = Stopwatch.StartNew();

		public RunLoopHandler(WinUiCompositorConnection parent)
		{
			_parent = parent;
		}

		public void Invoke(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
		{
			_parent.Tick?.Invoke(_st.Elapsed);
			using IAsyncAction asyncAction = _parent._shared.Compositor5.RequestCommitAsync();
			asyncAction.SetCompleted(this);
		}
	}

	private readonly WinUiCompositionShared _shared;

	public bool RunsInBackground => true;

	public event Action<TimeSpan>? Tick;

	public WinUiCompositorConnection()
	{
		using ICompositor compositor = NativeWinRTMethods.CreateInstance<ICompositor>("Windows.UI.Composition.Compositor");
		_shared = new WinUiCompositionShared(compositor);
	}

	private static bool TryCreateAndRegisterCore()
	{
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		Thread thread = new Thread((ThreadStart)delegate
		{
			WinUiCompositorConnection winUiCompositorConnection;
			try
			{
				NativeWinRTMethods.DispatcherQueueOptions options = default(NativeWinRTMethods.DispatcherQueueOptions);
				options.apartmentType = NativeWinRTMethods.DISPATCHERQUEUE_THREAD_APARTMENTTYPE.DQTAT_COM_NONE;
				options.dwSize = Marshal.SizeOf<NativeWinRTMethods.DispatcherQueueOptions>();
				options.threadType = NativeWinRTMethods.DISPATCHERQUEUE_THREAD_TYPE.DQTYPE_THREAD_CURRENT;
				NativeWinRTMethods.CreateDispatcherQueueController(options);
				winUiCompositorConnection = new WinUiCompositorConnection();
				AvaloniaLocator.CurrentMutable.BindToSelf(winUiCompositorConnection);
				AvaloniaLocator.CurrentMutable.Bind<IRenderTimer>().ToConstant(winUiCompositorConnection);
				tcs.SetResult(result: true);
			}
			catch (Exception exception)
			{
				tcs.SetException(exception);
				return;
			}
			winUiCompositorConnection.RunLoop();
		});
		thread.IsBackground = true;
		thread.Name = "DwmRenderTimerLoop";
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		return tcs.Task.Result;
	}

	private void RunLoop()
	{
		CancellationTokenSource cts = new CancellationTokenSource();
		AppDomain.CurrentDomain.ProcessExit += delegate
		{
			cts.Cancel();
		};
		lock (_shared.SyncRoot)
		{
			using IAsyncAction asyncAction = _shared.Compositor5.RequestCommitAsync();
			asyncAction.SetCompleted(new RunLoopHandler(this));
		}
		while (!cts.IsCancellationRequested)
		{
			UnmanagedMethods.GetMessage(out var lpMsg, IntPtr.Zero, 0u, 0u);
			lock (_shared.SyncRoot)
			{
				UnmanagedMethods.DispatchMessage(ref lpMsg);
			}
		}
	}

	public static bool IsSupported()
	{
		return Win32Platform.WindowsVersion >= WinUiCompositionShared.MinWinCompositionVersion;
	}

	public static bool TryCreateAndRegister()
	{
		if (IsSupported())
		{
			try
			{
				TryCreateAndRegisterCore();
				return true;
			}
			catch (Exception propertyValue)
			{
				Logger.TryGet(LogEventLevel.Error, "WinUIComposition")?.Log(null, "Unable to initialize WinUI compositor: {0}", propertyValue);
			}
		}
		else
		{
			string text = $"Windows {WinUiCompositionShared.MinWinCompositionVersion} is required. Your machine has Windows {Win32Platform.WindowsVersion} installed.";
			Logger.TryGet(LogEventLevel.Warning, "WinUIComposition")?.Log(null, "Unable to initialize WinUI compositor: " + text);
		}
		return false;
	}

	public WinUiCompositedWindowSurface CreateSurface(EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info)
	{
		return new WinUiCompositedWindowSurface(_shared, info);
	}
}
