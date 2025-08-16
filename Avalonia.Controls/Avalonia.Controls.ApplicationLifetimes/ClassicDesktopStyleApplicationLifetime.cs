using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Avalonia.Collections;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Controls.ApplicationLifetimes;

public class ClassicDesktopStyleApplicationLifetime : IClassicDesktopStyleApplicationLifetime, IControlledApplicationLifetime, IApplicationLifetime, IDisposable
{
	private int _exitCode;

	private CancellationTokenSource? _cts;

	private bool _isShuttingDown;

	private readonly AvaloniaList<Window> _windows = new AvaloniaList<Window>();

	private static ClassicDesktopStyleApplicationLifetime? s_activeLifetime;

	public string[]? Args { get; set; }

	public ShutdownMode ShutdownMode { get; set; }

	public Window? MainWindow { get; set; }

	public IReadOnlyList<Window> Windows => _windows;

	public event EventHandler<ControlledApplicationLifetimeStartupEventArgs>? Startup;

	public event EventHandler<ShutdownRequestedEventArgs>? ShutdownRequested;

	public event EventHandler<ControlledApplicationLifetimeExitEventArgs>? Exit;

	static ClassicDesktopStyleApplicationLifetime()
	{
		Window.WindowOpenedEvent.AddClassHandler(typeof(Window), OnWindowOpened);
		Window.WindowClosedEvent.AddClassHandler(typeof(Window), OnWindowClosed);
	}

	private static void OnWindowClosed(object? sender, RoutedEventArgs e)
	{
		Window window = (Window)sender;
		s_activeLifetime?._windows.Remove(window);
		s_activeLifetime?.HandleWindowClosed(window);
	}

	private static void OnWindowOpened(object? sender, RoutedEventArgs e)
	{
		Window item = (Window)sender;
		if (s_activeLifetime != null && !s_activeLifetime._windows.Contains(item))
		{
			s_activeLifetime._windows.Add(item);
		}
	}

	public ClassicDesktopStyleApplicationLifetime()
	{
		if (s_activeLifetime != null)
		{
			throw new InvalidOperationException("Can not have multiple active ClassicDesktopStyleApplicationLifetime instances and the previously created one was not disposed");
		}
		s_activeLifetime = this;
	}

	private void HandleWindowClosed(Window? window)
	{
		if (window != null && !_isShuttingDown)
		{
			if (ShutdownMode == ShutdownMode.OnLastWindowClose && _windows.Count == 0)
			{
				TryShutdown();
			}
			else if (ShutdownMode == ShutdownMode.OnMainWindowClose && window == MainWindow)
			{
				TryShutdown();
			}
		}
	}

	public void Shutdown(int exitCode = 0)
	{
		DoShutdown(new ShutdownRequestedEventArgs(), isProgrammatic: true, force: true, exitCode);
	}

	public bool TryShutdown(int exitCode = 0)
	{
		return DoShutdown(new ShutdownRequestedEventArgs(), isProgrammatic: true, force: false, exitCode);
	}

	public int Start(string[] args)
	{
		this.Startup?.Invoke(this, new ControlledApplicationLifetimeStartupEventArgs(args));
		ClassicDesktopStyleApplicationLifetimeOptions service = AvaloniaLocator.Current.GetService<ClassicDesktopStyleApplicationLifetimeOptions>();
		if (service != null && service.ProcessUrlActivationCommandLine && args.Length != 0)
		{
			((IApplicationPlatformEvents)Application.Current)?.RaiseUrlsOpened(args);
		}
		IPlatformLifetimeEventsImpl service2 = AvaloniaLocator.Current.GetService<IPlatformLifetimeEventsImpl>();
		if (service2 != null)
		{
			service2.ShutdownRequested += OnShutdownRequested;
		}
		_cts = new CancellationTokenSource();
		ShowMainWindow();
		Dispatcher.UIThread.MainLoop(_cts.Token);
		Environment.ExitCode = _exitCode;
		return _exitCode;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ShowMainWindow()
	{
		MainWindow?.Show();
	}

	public void Dispose()
	{
		if (s_activeLifetime == this)
		{
			s_activeLifetime = null;
		}
	}

	private bool DoShutdown(ShutdownRequestedEventArgs e, bool isProgrammatic, bool force = false, int exitCode = 0)
	{
		if (!force)
		{
			this.ShutdownRequested?.Invoke(this, e);
			if (e.Cancel)
			{
				return false;
			}
			if (_isShuttingDown)
			{
				throw new InvalidOperationException("Application is already shutting down.");
			}
		}
		_exitCode = exitCode;
		_isShuttingDown = true;
		try
		{
			Window[] array = Windows.ToArray();
			foreach (Window window in array)
			{
				if (window.Owner == null)
				{
					window.CloseCore(WindowCloseReason.ApplicationShutdown, isProgrammatic);
				}
			}
			if (!force && Windows.Count > 0)
			{
				e.Cancel = true;
				return false;
			}
			ControlledApplicationLifetimeExitEventArgs controlledApplicationLifetimeExitEventArgs = new ControlledApplicationLifetimeExitEventArgs(exitCode);
			this.Exit?.Invoke(this, controlledApplicationLifetimeExitEventArgs);
			_exitCode = controlledApplicationLifetimeExitEventArgs.ApplicationExitCode;
		}
		finally
		{
			_cts?.Cancel();
			_cts = null;
			_isShuttingDown = false;
			Dispatcher.UIThread.InvokeShutdown();
		}
		return true;
	}

	private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
	{
		DoShutdown(e, isProgrammatic: false);
	}
}
