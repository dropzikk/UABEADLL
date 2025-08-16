using System;
using System.Threading;
using Avalonia.Input;
using Avalonia.Threading;

namespace Avalonia.Controls;

public static class DesktopApplicationExtensions
{
	public static void Run(this Application app, ICloseable closable)
	{
		CancellationTokenSource cts = new CancellationTokenSource();
		closable.Closed += delegate
		{
			cts.Cancel();
		};
		app.Run(cts.Token);
	}

	public static void Run(this Application app, Window mainWindow)
	{
		if (mainWindow == null)
		{
			throw new ArgumentNullException("mainWindow");
		}
		CancellationTokenSource cts = new CancellationTokenSource();
		mainWindow.Closed += delegate
		{
			cts.Cancel();
		};
		if (!mainWindow.IsVisible)
		{
			mainWindow.Show();
		}
		app.Run(cts.Token);
	}

	public static void Run(this Application app, CancellationToken token)
	{
		Dispatcher.UIThread.MainLoop(token);
	}

	public static void RunWithMainWindow<TWindow>(this Application app) where TWindow : Window, new()
	{
		TWindow val = new TWindow();
		val.Show();
		app.Run(val);
	}
}
