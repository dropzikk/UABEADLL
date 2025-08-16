using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class AvaloniaNativePlatform : IWindowingPlatform
{
	private delegate IntPtr CreateAvaloniaNativeDelegate();

	private class GCHandleDeallocator : NativeCallbackBase, IAvnGCHandleDeallocatorCallback, IUnknown, IDisposable
	{
		public void FreeGCHandle(IntPtr handle)
		{
			GCHandle.FromIntPtr(handle).Free();
		}
	}

	private readonly IAvaloniaNativeFactory _factory;

	private AvaloniaNativePlatformOptions? _options;

	private IPlatformGraphics? _platformGraphics;

	internal static readonly KeyboardDevice KeyboardDevice = new KeyboardDevice();

	internal static Compositor Compositor { get; private set; } = null;

	[DllImport("libAvaloniaNative")]
	private static extern IntPtr CreateAvaloniaNative();

	public static AvaloniaNativePlatform Initialize(IntPtr factory, AvaloniaNativePlatformOptions options)
	{
		AvaloniaNativePlatform avaloniaNativePlatform = new AvaloniaNativePlatform(MicroComRuntime.CreateProxyFor<IAvaloniaNativeFactory>(factory, ownsHandle: true));
		avaloniaNativePlatform.DoInitialize(options);
		return avaloniaNativePlatform;
	}

	public static AvaloniaNativePlatform Initialize(AvaloniaNativePlatformOptions options)
	{
		if (options.AvaloniaNativeLibraryPath != null)
		{
			IDynLoader dynLoader2;
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				IDynLoader dynLoader = new UnixLoader();
				dynLoader2 = dynLoader;
			}
			else
			{
				IDynLoader dynLoader = new Win32Loader();
				dynLoader2 = dynLoader;
			}
			IntPtr dll = dynLoader2.LoadLibrary(options.AvaloniaNativeLibraryPath);
			return Initialize(Marshal.GetDelegateForFunctionPointer<CreateAvaloniaNativeDelegate>(dynLoader2.GetProcAddress(dll, "CreateAvaloniaNative", optional: false))(), options);
		}
		return Initialize(CreateAvaloniaNative(), options);
	}

	public void SetupApplicationMenuExporter()
	{
		new AvaloniaNativeMenuExporter(_factory);
	}

	public void SetupApplicationName()
	{
		if (!string.IsNullOrWhiteSpace(Application.Current.Name))
		{
			_factory.MacOptions.SetApplicationTitle(Application.Current.Name);
		}
	}

	private AvaloniaNativePlatform(IAvaloniaNativeFactory factory)
	{
		_factory = factory;
	}

	private void DoInitialize(AvaloniaNativePlatformOptions options)
	{
		_options = options;
		AvaloniaNativeApplicationPlatform avaloniaNativeApplicationPlatform = new AvaloniaNativeApplicationPlatform();
		MacOSPlatformOptions macOSPlatformOptions = AvaloniaLocator.Current.GetService<MacOSPlatformOptions>() ?? new MacOSPlatformOptions();
		if (_factory.MacOptions != null)
		{
			_factory.MacOptions.SetDisableAppDelegate(macOSPlatformOptions.DisableAvaloniaAppDelegate ? 1 : 0);
		}
		_factory.Initialize(new GCHandleDeallocator(), avaloniaNativeApplicationPlatform, new AvnDispatcher());
		if (_factory.MacOptions != null)
		{
			_factory.MacOptions.SetShowInDock(macOSPlatformOptions.ShowInDock ? 1 : 0);
			_factory.MacOptions.SetDisableSetProcessName(macOSPlatformOptions.DisableSetProcessName ? 1 : 0);
		}
		AvaloniaLocator.CurrentMutable.Bind<IDispatcherImpl>().ToConstant(new DispatcherImpl(_factory.CreatePlatformThreadingInterface())).Bind<ICursorFactory>()
			.ToConstant(new CursorFactory(_factory.CreateCursorFactory()))
			.Bind<IPlatformIconLoader>()
			.ToSingleton<IconLoader>()
			.Bind<IKeyboardDevice>()
			.ToConstant(KeyboardDevice)
			.Bind<IPlatformSettings>()
			.ToConstant(new NativePlatformSettings(_factory.CreatePlatformSettings()))
			.Bind<IWindowingPlatform>()
			.ToConstant(this)
			.Bind<IClipboard>()
			.ToConstant(new ClipboardImpl(_factory.CreateClipboard()))
			.Bind<IRenderTimer>()
			.ToConstant(new DefaultRenderTimer(60))
			.Bind<IMountedVolumeInfoProvider>()
			.ToConstant(new MacOSMountedVolumeInfoProvider())
			.Bind<IPlatformDragSource>()
			.ToConstant(new AvaloniaNativeDragSource(_factory))
			.Bind<IPlatformLifetimeEventsImpl>()
			.ToConstant(avaloniaNativeApplicationPlatform)
			.Bind<INativeApplicationCommands>()
			.ToConstant(new MacOSNativeMenuCommands(_factory.CreateApplicationCommands()));
		PlatformHotkeyConfiguration platformHotkeyConfiguration = new PlatformHotkeyConfiguration(KeyModifiers.Meta, KeyModifiers.Shift, KeyModifiers.Alt);
		platformHotkeyConfiguration.MoveCursorToTheStartOfLine.Add(new KeyGesture(Key.Left, platformHotkeyConfiguration.CommandModifiers));
		platformHotkeyConfiguration.MoveCursorToTheStartOfLineWithSelection.Add(new KeyGesture(Key.Left, platformHotkeyConfiguration.CommandModifiers | platformHotkeyConfiguration.SelectionModifiers));
		platformHotkeyConfiguration.MoveCursorToTheEndOfLine.Add(new KeyGesture(Key.Right, platformHotkeyConfiguration.CommandModifiers));
		platformHotkeyConfiguration.MoveCursorToTheEndOfLineWithSelection.Add(new KeyGesture(Key.Right, platformHotkeyConfiguration.CommandModifiers | platformHotkeyConfiguration.SelectionModifiers));
		AvaloniaLocator.CurrentMutable.Bind<PlatformHotkeyConfiguration>().ToConstant(platformHotkeyConfiguration);
		using (IEnumerator<AvaloniaNativeRenderingMode> enumerator = _options.RenderingMode.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case AvaloniaNativeRenderingMode.OpenGl:
					try
					{
						_platformGraphics = new AvaloniaNativeGlPlatformGraphics(_factory.ObtainGlDisplay());
					}
					catch (Exception)
					{
						continue;
					}
					break;
				case AvaloniaNativeRenderingMode.Metal:
					try
					{
						MetalPlatformGraphics metalPlatformGraphics = new MetalPlatformGraphics(_factory);
						metalPlatformGraphics.CreateContext().Dispose();
						_platformGraphics = metalPlatformGraphics;
					}
					catch
					{
					}
					continue;
				case AvaloniaNativeRenderingMode.Software:
					break;
				default:
					continue;
				}
				break;
			}
		}
		if (_platformGraphics != null)
		{
			AvaloniaLocator.CurrentMutable.Bind<IPlatformGraphics>().ToConstant(_platformGraphics);
		}
		Compositor = new Compositor(_platformGraphics, useUiThreadForSynchronousCommits: true);
		AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
	}

	private void OnProcessExit(object? sender, EventArgs e)
	{
		AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
		_factory.Dispose();
	}

	public ITrayIconImpl CreateTrayIcon()
	{
		return new TrayIconImpl(_factory);
	}

	public IWindowImpl CreateWindow()
	{
		return new WindowImpl(_factory, _options);
	}

	public IWindowImpl CreateEmbeddableWindow()
	{
		throw new NotImplementedException();
	}
}
