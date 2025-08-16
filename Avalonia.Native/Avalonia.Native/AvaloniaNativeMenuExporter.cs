using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Dialogs;
using Avalonia.Input;
using Avalonia.Native.Interop;
using Avalonia.Native.Interop.Impl;
using Avalonia.Threading;

namespace Avalonia.Native;

internal class AvaloniaNativeMenuExporter : ITopLevelNativeMenuExporter, INativeMenuExporter
{
	private readonly IAvaloniaNativeFactory _factory;

	private bool _resetQueued = true;

	private bool _exported;

	private readonly IAvnWindow _nativeWindow;

	private NativeMenu _menu;

	private __MicroComIAvnMenuProxy _nativeMenu;

	private readonly IAvnTrayIcon _trayIcon;

	private readonly IAvnApplicationCommands _applicationCommands;

	public bool IsNativeMenuExported => _exported;

	public event EventHandler OnIsNativeMenuExportedChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	public AvaloniaNativeMenuExporter(IAvnWindow nativeWindow, IAvaloniaNativeFactory factory)
	{
		_factory = factory;
		_nativeWindow = nativeWindow;
		_applicationCommands = _factory.CreateApplicationCommands();
		DoLayoutReset();
	}

	public AvaloniaNativeMenuExporter(IAvaloniaNativeFactory factory)
	{
		_factory = factory;
		_applicationCommands = _factory.CreateApplicationCommands();
		DoLayoutReset();
	}

	public AvaloniaNativeMenuExporter(IAvnTrayIcon trayIcon, IAvaloniaNativeFactory factory)
	{
		_factory = factory;
		_trayIcon = trayIcon;
		_applicationCommands = _factory.CreateApplicationCommands();
		DoLayoutReset();
	}

	public void SetNativeMenu(NativeMenu menu)
	{
		_menu = menu ?? new NativeMenu();
		DoLayoutReset(forceUpdate: true);
	}

	internal void UpdateIfNeeded()
	{
		if (_resetQueued)
		{
			DoLayoutReset();
		}
	}

	private static NativeMenu CreateDefaultAppMenu()
	{
		NativeMenu nativeMenu = new NativeMenu();
		NativeMenuItem nativeMenuItem = new NativeMenuItem("About Avalonia");
		nativeMenuItem.Click += async delegate
		{
			AboutAvaloniaDialog aboutAvaloniaDialog = new AboutAvaloniaDialog();
			Application current = Application.Current;
			if (current != null && current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime)
			{
				Window mainWindow = classicDesktopStyleApplicationLifetime.MainWindow;
				if (mainWindow != null)
				{
					await aboutAvaloniaDialog.ShowDialog(mainWindow);
				}
			}
		};
		nativeMenu.Add(nativeMenuItem);
		return nativeMenu;
	}

	private void PopulateStandardOSXMenuItems(NativeMenu appMenu)
	{
		appMenu.Add(new NativeMenuItemSeparator());
		NativeMenuItem nativeMenuItem = new NativeMenuItem("Services");
		nativeMenuItem.Menu = new NativeMenu { [MacOSNativeMenuCommands.IsServicesSubmenuProperty] = true };
		appMenu.Add(nativeMenuItem);
		appMenu.Add(new NativeMenuItemSeparator());
		NativeMenuItem nativeMenuItem2 = new NativeMenuItem("Hide " + (Application.Current?.Name ?? "Application"))
		{
			Gesture = new KeyGesture(Key.H, KeyModifiers.Meta)
		};
		nativeMenuItem2.Click += delegate
		{
			_applicationCommands.HideApp();
		};
		appMenu.Add(nativeMenuItem2);
		NativeMenuItem nativeMenuItem3 = new NativeMenuItem("Hide Others")
		{
			Gesture = new KeyGesture(Key.Q, KeyModifiers.Alt | KeyModifiers.Meta)
		};
		nativeMenuItem3.Click += delegate
		{
			_applicationCommands.HideOthers();
		};
		appMenu.Add(nativeMenuItem3);
		NativeMenuItem nativeMenuItem4 = new NativeMenuItem("Show All");
		nativeMenuItem4.Click += delegate
		{
			_applicationCommands.ShowAll();
		};
		appMenu.Add(nativeMenuItem4);
		appMenu.Add(new NativeMenuItemSeparator());
		NativeMenuItem nativeMenuItem5 = new NativeMenuItem("Quit")
		{
			Gesture = new KeyGesture(Key.Q, KeyModifiers.Meta)
		};
		nativeMenuItem5.Click += delegate
		{
			Application current = Application.Current;
			if (current != null && current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime)
			{
				classicDesktopStyleApplicationLifetime.TryShutdown();
			}
			else
			{
				current = Application.Current;
				if (current != null && current.ApplicationLifetime is IControlledApplicationLifetime controlledApplicationLifetime)
				{
					controlledApplicationLifetime.Shutdown();
				}
			}
		};
		appMenu.Add(nativeMenuItem5);
	}

	private void DoLayoutReset(bool forceUpdate = false)
	{
		if ((AvaloniaLocator.Current.GetService<MacOSPlatformOptions>() ?? new MacOSPlatformOptions()).DisableNativeMenus || !(_resetQueued || forceUpdate))
		{
			return;
		}
		_resetQueued = false;
		if (_nativeWindow == null)
		{
			if (_trayIcon == null)
			{
				NativeMenu nativeMenu = NativeMenu.GetMenu(Application.Current);
				if (nativeMenu == null)
				{
					nativeMenu = CreateDefaultAppMenu();
					NativeMenu.SetMenu(Application.Current, nativeMenu);
				}
				SetMenu(nativeMenu);
			}
			else if (_menu != null)
			{
				SetMenu(_trayIcon, _menu);
			}
		}
		else if (_menu != null)
		{
			SetMenu(_nativeWindow, _menu);
		}
		_exported = true;
	}

	internal void QueueReset()
	{
		if (!_resetQueued)
		{
			_resetQueued = true;
			Dispatcher.UIThread.Post(delegate
			{
				DoLayoutReset();
			}, DispatcherPriority.Background);
		}
	}

	private void SetMenu(NativeMenu menu)
	{
		NativeMenuItem nativeMenuItem = menu.Parent;
		NativeMenu nativeMenu = nativeMenuItem?.Parent;
		if (nativeMenuItem == null)
		{
			nativeMenuItem = new NativeMenuItem();
		}
		if (nativeMenu == null)
		{
			nativeMenu = new NativeMenu();
			nativeMenu.Add(nativeMenuItem);
		}
		nativeMenuItem.Menu = menu;
		bool flag = false;
		if (_nativeMenu == null)
		{
			_nativeMenu = __MicroComIAvnMenuProxy.Create(_factory);
			_nativeMenu.Initialize(this, nativeMenu, "");
			MacOSPlatformOptions service = AvaloniaLocator.Current.GetService<MacOSPlatformOptions>();
			if (service == null || !service.DisableDefaultApplicationMenuItems)
			{
				PopulateStandardOSXMenuItems(menu);
			}
			flag = true;
		}
		_nativeMenu.Update(_factory, nativeMenu);
		if (flag)
		{
			_factory.SetAppMenu(_nativeMenu);
		}
	}

	private void SetMenu(IAvnWindow avnWindow, NativeMenu menu)
	{
		bool flag = false;
		if (_nativeMenu == null)
		{
			_nativeMenu = __MicroComIAvnMenuProxy.Create(_factory);
			_nativeMenu.Initialize(this, menu, "");
			flag = true;
		}
		_nativeMenu.Update(_factory, menu);
		if (flag)
		{
			avnWindow.SetMainMenu(_nativeMenu);
		}
	}

	private void SetMenu(IAvnTrayIcon trayIcon, NativeMenu menu)
	{
		bool flag = false;
		if (_nativeMenu == null)
		{
			_nativeMenu = __MicroComIAvnMenuProxy.Create(_factory);
			_nativeMenu.Initialize(this, menu, "");
			flag = true;
		}
		_nativeMenu.Update(_factory, menu);
		if (flag)
		{
			trayIcon.SetMenu(_nativeMenu);
		}
	}
}
