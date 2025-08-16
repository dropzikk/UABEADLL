using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Platform;
using Avalonia.FreeDesktop;
using Avalonia.FreeDesktop.DBusIme;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.X11.Glx;

namespace Avalonia.X11;

internal class AvaloniaX11Platform : IWindowingPlatform
{
	private Lazy<KeyboardDevice> _keyboardDevice = new Lazy<KeyboardDevice>(() => new KeyboardDevice());

	public Dictionary<IntPtr, X11PlatformThreading.EventHandler> Windows = new Dictionary<IntPtr, X11PlatformThreading.EventHandler>();

	public XI2Manager XI2;

	public KeyboardDevice KeyboardDevice => _keyboardDevice.Value;

	public X11Info Info { get; private set; }

	public IX11Screens X11Screens { get; private set; }

	public Compositor Compositor { get; private set; }

	public IScreenImpl Screens { get; private set; }

	public X11PlatformOptions Options { get; private set; }

	public IntPtr OrphanedWindow { get; private set; }

	public X11Globals Globals { get; private set; }

	public ManualRawEventGrouperDispatchQueue EventGrouperDispatchQueue { get; } = new ManualRawEventGrouperDispatchQueue();

	public IntPtr DeferredDisplay { get; set; }

	public IntPtr Display { get; set; }

	public void Initialize(X11PlatformOptions options)
	{
		Options = options;
		bool useXim = false;
		if (EnableIme(options) && !X11DBusImeHelper.DetectAndRegister() && ShouldUseXim())
		{
			useXim = true;
		}
		XLib.XInitThreads();
		Display = XLib.XOpenDisplay(IntPtr.Zero);
		if (Display == IntPtr.Zero)
		{
			throw new Exception("XOpenDisplay failed");
		}
		DeferredDisplay = XLib.XOpenDisplay(IntPtr.Zero);
		if (DeferredDisplay == IntPtr.Zero)
		{
			throw new Exception("XOpenDisplay failed");
		}
		OrphanedWindow = XLib.XCreateSimpleWindow(Display, XLib.XDefaultRootWindow(Display), 0, 0, 1, 1, 0, IntPtr.Zero, IntPtr.Zero);
		XError.Init();
		Info = new X11Info(Display, DeferredDisplay, useXim);
		Globals = new X11Globals(this);
		if (options.UseDBusMenu)
		{
			DBusHelper.TryInitialize();
		}
		AvaloniaLocator.CurrentMutable.BindToSelf(this).Bind<IWindowingPlatform>().ToConstant(this)
			.Bind<IDispatcherImpl>()
			.ToConstant(new X11PlatformThreading(this))
			.Bind<IRenderTimer>()
			.ToConstant(new SleepLoopRenderTimer(60))
			.Bind<PlatformHotkeyConfiguration>()
			.ToConstant(new PlatformHotkeyConfiguration(KeyModifiers.Control))
			.Bind<IKeyboardDevice>()
			.ToFunc(() => KeyboardDevice)
			.Bind<ICursorFactory>()
			.ToConstant(new X11CursorFactory(Display))
			.Bind<IClipboard>()
			.ToConstant(new X11Clipboard(this))
			.Bind<IPlatformSettings>()
			.ToSingleton<DBusPlatformSettings>()
			.Bind<IPlatformIconLoader>()
			.ToConstant(new X11IconLoader())
			.Bind<IMountedVolumeInfoProvider>()
			.ToConstant(new LinuxMountedVolumeInfoProvider())
			.Bind<IPlatformLifetimeEventsImpl>()
			.ToConstant(new X11PlatformLifetimeEvents(this));
		X11Screens = Avalonia.X11.X11Screens.Init(this);
		Screens = new X11Screens(X11Screens);
		if (Info.XInputVersion != null)
		{
			XI2Manager xI2Manager = new XI2Manager();
			if (xI2Manager.Init(this))
			{
				XI2 = xI2Manager;
			}
		}
		IPlatformGraphics platformGraphics = InitializeGraphics(options, Info);
		if (platformGraphics != null)
		{
			AvaloniaLocator.CurrentMutable.Bind<IPlatformGraphics>().ToConstant(platformGraphics);
		}
		Compositor = new Compositor(platformGraphics);
	}

	private static uint[] X11IconConverter(IWindowIconImpl icon)
	{
		if (!(icon is X11IconData x11IconData))
		{
			return Array.Empty<uint>();
		}
		return x11IconData.Data.Select((UIntPtr x) => x.ToUInt32()).ToArray();
	}

	public ITrayIconImpl CreateTrayIcon()
	{
		DBusTrayIconImpl dBusTrayIconImpl = new DBusTrayIconImpl();
		if (!dBusTrayIconImpl.IsActive)
		{
			return new XEmbedTrayIconImpl();
		}
		dBusTrayIconImpl.IconConverterDelegate = X11IconConverter;
		return dBusTrayIconImpl;
	}

	public IWindowImpl CreateWindow()
	{
		return new X11Window(this, null);
	}

	public IWindowImpl CreateEmbeddableWindow()
	{
		throw new NotSupportedException();
	}

	private static bool EnableIme(X11PlatformOptions options)
	{
		if (Environment.GetEnvironmentVariable("AVALONIA_IM_MODULE") == "none")
		{
			return false;
		}
		if (options.EnableIme.HasValue)
		{
			return options.EnableIme.Value;
		}
		string environmentVariable = Environment.GetEnvironmentVariable("LANG");
		if (environmentVariable != null)
		{
			if (!environmentVariable.Contains("zh") && !environmentVariable.Contains("ja") && !environmentVariable.Contains("vi"))
			{
				return environmentVariable.Contains("ko");
			}
			return true;
		}
		return false;
	}

	private static bool ShouldUseXim()
	{
		if (Environment.GetEnvironmentVariable("AVALONIA_IM_MODULE") == "none" || Environment.GetEnvironmentVariable("GTK_IM_MODULE") == "none" || Environment.GetEnvironmentVariable("QT_IM_MODULE") == "none")
		{
			return true;
		}
		string environmentVariable = Environment.GetEnvironmentVariable("XMODIFIERS");
		if (environmentVariable == null)
		{
			return false;
		}
		if (environmentVariable.Contains("@im=none") || environmentVariable.Contains("@im=null"))
		{
			return false;
		}
		if (!environmentVariable.Contains("@im="))
		{
			return false;
		}
		if (Environment.GetEnvironmentVariable("GTK_IM_MODULE") == "xim" || Environment.GetEnvironmentVariable("QT_IM_MODULE") == "xim" || Environment.GetEnvironmentVariable("AVALONIA_IM_MODULE") == "xim")
		{
			return true;
		}
		return false;
	}

	private static IPlatformGraphics InitializeGraphics(X11PlatformOptions opts, X11Info info)
	{
		if (opts.RenderingMode == null || !opts.RenderingMode.Any())
		{
			throw new InvalidOperationException("X11PlatformOptions.RenderingMode must not be empty or null");
		}
		foreach (X11RenderingMode item in opts.RenderingMode)
		{
			switch (item)
			{
			case X11RenderingMode.Software:
				return null;
			case X11RenderingMode.Glx:
			{
				GlxPlatformGraphics glxPlatformGraphics = GlxPlatformGraphics.TryCreate(info, opts.GlProfiles);
				if (glxPlatformGraphics != null)
				{
					return glxPlatformGraphics;
				}
				break;
			}
			}
			if (item == X11RenderingMode.Egl)
			{
				EglPlatformGraphics eglPlatformGraphics = EglPlatformGraphics.TryCreate();
				if (eglPlatformGraphics != null)
				{
					return eglPlatformGraphics;
				}
			}
		}
		throw new InvalidOperationException($"{"X11PlatformOptions"}.{"RenderingMode"} has a value of \"{string.Join(", ", opts.RenderingMode)}\", but no options were applied.");
	}
}
