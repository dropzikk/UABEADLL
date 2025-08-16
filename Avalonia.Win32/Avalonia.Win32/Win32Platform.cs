using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.Utilities;
using Avalonia.Win32.Input;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class Win32Platform : IWindowingPlatform, IPlatformIconLoader, IPlatformLifetimeEventsImpl
{
	private static readonly Win32Platform s_instance = new Win32Platform();

	private static Win32PlatformOptions? s_options;

	private static Compositor? s_compositor;

	internal const int TIMERID_DISPATCHER = 1;

	private UnmanagedMethods.WndProc? _wndProcDelegate;

	private IntPtr _hwnd;

	private Win32DispatcherImpl _dispatcher;

	internal static Win32Platform Instance => s_instance;

	internal static IPlatformSettings PlatformSettings => AvaloniaLocator.Current.GetRequiredService<IPlatformSettings>();

	internal IntPtr Handle => _hwnd;

	public static Version WindowsVersion { get; } = UnmanagedMethods.RtlGetVersion();

	internal static bool UseOverlayPopups => Options.OverlayPopups;

	public static Win32PlatformOptions Options => s_options ?? throw new InvalidOperationException("Win32Platform hasn't been initialized");

	internal static Compositor Compositor => s_compositor ?? throw new InvalidOperationException("Win32Platform hasn't been initialized");

	public event EventHandler<ShutdownRequestedEventArgs>? ShutdownRequested;

	public Win32Platform()
	{
		SetDpiAwareness();
		CreateMessageWindow();
		_dispatcher = new Win32DispatcherImpl(_hwnd);
	}

	public static void Initialize()
	{
		Initialize(new Win32PlatformOptions());
	}

	public static void Initialize(Win32PlatformOptions options)
	{
		s_options = options;
		DefaultRenderTimer constant = (options.ShouldRenderOnUIThread ? new UiThreadRenderTimer(60) : new DefaultRenderTimer(60));
		AvaloniaLocator.CurrentMutable.Bind<IClipboard>().ToSingleton<ClipboardImpl>().Bind<ICursorFactory>()
			.ToConstant(CursorFactory.Instance)
			.Bind<IKeyboardDevice>()
			.ToConstant(WindowsKeyboardDevice.Instance)
			.Bind<IPlatformSettings>()
			.ToSingleton<Win32PlatformSettings>()
			.Bind<IDispatcherImpl>()
			.ToConstant(s_instance._dispatcher)
			.Bind<IRenderTimer>()
			.ToConstant(constant)
			.Bind<IWindowingPlatform>()
			.ToConstant(s_instance)
			.Bind<PlatformHotkeyConfiguration>()
			.ToConstant(new PlatformHotkeyConfiguration(KeyModifiers.Control)
			{
				OpenContextMenu = 
				{
					new KeyGesture(Key.F10, KeyModifiers.Shift)
				}
			})
			.Bind<IPlatformIconLoader>()
			.ToConstant(s_instance)
			.Bind<NonPumpingLockHelper.IHelperImpl>()
			.ToConstant(NonPumpingWaitHelperImpl.Instance)
			.Bind<IMountedVolumeInfoProvider>()
			.ToConstant(new WindowsMountedVolumeInfoProvider())
			.Bind<IPlatformLifetimeEventsImpl>()
			.ToConstant(s_instance);
		IPlatformGraphics gpu;
		if (options.CustomPlatformGraphics != null)
		{
			IReadOnlyList<Win32CompositionMode> compositionMode = options.CompositionMode;
			if (compositionMode != null && !compositionMode.Contains(Win32CompositionMode.RedirectionSurface))
			{
				throw new InvalidOperationException("Win32PlatformOptions.CustomPlatformGraphics is only compatible with Win32CompositionMode.RedirectionSurface");
			}
			gpu = options.CustomPlatformGraphics;
		}
		else
		{
			gpu = Win32GlManager.Initialize();
		}
		if (OleContext.Current != null)
		{
			AvaloniaLocator.CurrentMutable.Bind<IPlatformDragSource>().ToSingleton<DragSource>();
		}
		s_compositor = new Compositor(gpu);
	}

	private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		if (msg == 1024 && wParam.ToInt64() == -559038801 && lParam.ToInt64() == 305419896)
		{
			_dispatcher?.DispatchWorkItem();
		}
		if (msg == 17 && this.ShutdownRequested != null)
		{
			ShutdownRequestedEventArgs shutdownRequestedEventArgs = new ShutdownRequestedEventArgs();
			this.ShutdownRequested(this, shutdownRequestedEventArgs);
			if (shutdownRequestedEventArgs.Cancel)
			{
				return IntPtr.Zero;
			}
		}
		if (msg == 26 && PlatformSettings is Win32PlatformSettings win32PlatformSettings)
		{
			string text = Marshal.PtrToStringAuto(lParam);
			if (text == "ImmersiveColorSet" || text == "WindowsThemeElement")
			{
				win32PlatformSettings.OnColorValuesChanged();
			}
		}
		if (msg == 275 && wParam == (IntPtr)1)
		{
			_dispatcher?.FireTimer();
		}
		TrayIconImpl.ProcWnd(hWnd, msg, wParam, lParam);
		return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
	}

	private void CreateMessageWindow()
	{
		_wndProcDelegate = WndProc;
		UnmanagedMethods.WNDCLASSEX wNDCLASSEX = default(UnmanagedMethods.WNDCLASSEX);
		wNDCLASSEX.cbSize = Marshal.SizeOf<UnmanagedMethods.WNDCLASSEX>();
		wNDCLASSEX.lpfnWndProc = _wndProcDelegate;
		wNDCLASSEX.hInstance = UnmanagedMethods.GetModuleHandle(null);
		wNDCLASSEX.lpszClassName = "AvaloniaMessageWindow " + Guid.NewGuid();
		UnmanagedMethods.WNDCLASSEX lpwcx = wNDCLASSEX;
		ushort num = UnmanagedMethods.RegisterClassEx(ref lpwcx);
		if (num == 0)
		{
			throw new Win32Exception();
		}
		_hwnd = UnmanagedMethods.CreateWindowEx(0, num, null, 0u, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		if (_hwnd == IntPtr.Zero)
		{
			throw new Win32Exception();
		}
	}

	public ITrayIconImpl CreateTrayIcon()
	{
		return new TrayIconImpl();
	}

	public IWindowImpl CreateWindow()
	{
		return new WindowImpl();
	}

	public IWindowImpl CreateEmbeddableWindow()
	{
		EmbeddedWindowImpl embeddedWindowImpl = new EmbeddedWindowImpl();
		embeddedWindowImpl.Show(activate: true, isDialog: false);
		return embeddedWindowImpl;
	}

	public IWindowIconImpl LoadIcon(string fileName)
	{
		using FileStream stream = File.OpenRead(fileName);
		return CreateIconImpl(stream);
	}

	public IWindowIconImpl LoadIcon(Stream stream)
	{
		return CreateIconImpl(stream);
	}

	public IWindowIconImpl LoadIcon(IBitmapImpl bitmap)
	{
		using MemoryStream stream = new MemoryStream();
		bitmap.Save(stream, null);
		return CreateIconImpl(stream);
	}

	private static IconImpl CreateIconImpl(Stream stream)
	{
		try
		{
			return new IconImpl(new Icon(stream));
		}
		catch (ArgumentException)
		{
			using Bitmap bitmap = new Bitmap(stream);
			return new IconImpl(Icon.FromHandle(bitmap.GetHicon()));
		}
	}

	private static void SetDpiAwareness()
	{
		if (!(UnmanagedMethods.GetProcAddress(UnmanagedMethods.LoadLibrary("user32.dll"), "SetProcessDpiAwarenessContext") != IntPtr.Zero) || (!UnmanagedMethods.SetProcessDpiAwarenessContext(UnmanagedMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2) && !UnmanagedMethods.SetProcessDpiAwarenessContext(UnmanagedMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE)))
		{
			if (UnmanagedMethods.GetProcAddress(UnmanagedMethods.LoadLibrary("shcore.dll"), "SetProcessDpiAwareness") != IntPtr.Zero)
			{
				UnmanagedMethods.SetProcessDpiAwareness(UnmanagedMethods.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);
			}
			else
			{
				UnmanagedMethods.SetProcessDPIAware();
			}
		}
	}
}
