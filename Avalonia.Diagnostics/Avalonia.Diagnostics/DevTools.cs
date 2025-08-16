using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Diagnostics.Controls;
using Avalonia.Diagnostics.Views;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Avalonia.Diagnostics;

internal static class DevTools
{
	private static readonly Dictionary<IDevToolsTopLevelGroup, MainWindow> s_open = new Dictionary<IDevToolsTopLevelGroup, MainWindow>();

	public static IDisposable Attach(TopLevel root, KeyGesture gesture)
	{
		return Attach(root, new DevToolsOptions
		{
			Gesture = gesture
		});
	}

	public static IDisposable Attach(TopLevel root, DevToolsOptions options)
	{
		return (root ?? throw new ArgumentNullException("root")).AddDisposableHandler(InputElement.KeyDownEvent, PreviewKeyDown, RoutingStrategies.Tunnel);
		void PreviewKeyDown(object? sender, KeyEventArgs e)
		{
			if (options.Gesture.Matches(e))
			{
				Open(root, options);
			}
		}
	}

	public static IDisposable Open(TopLevel root, DevToolsOptions options)
	{
		return Open(new SingleViewTopLevelGroup(root), options, root as Window, null);
	}

	internal static IDisposable Open(IDevToolsTopLevelGroup group, DevToolsOptions options)
	{
		return Open(group, options, null, null);
	}

	internal static IDisposable Attach(Application application, DevToolsOptions options)
	{
		SerialDisposableValue openedDisposable = new SerialDisposableValue();
		CompositeDisposable compositeDisposable = new CompositeDisposable(2);
		compositeDisposable.Add(openedDisposable);
		if (!Design.IsDesignMode)
		{
			IClassicDesktopStyleApplicationLifetime lifeTime = application.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
			if (lifeTime == null)
			{
				throw new ArgumentNullException("application", "DevTools can only attach to applications that support IClassicDesktopStyleApplicationLifetime.");
			}
			if (application.InputManager != null)
			{
				compositeDisposable.Add(application.InputManager.PreProcess.Subscribe(delegate(RawInputEventArgs e)
				{
					Window mainWindow = lifeTime.MainWindow;
					if (e is RawKeyEventArgs { Type: RawKeyEventType.KeyUp } rawKeyEventArgs && options.Gesture.Matches(rawKeyEventArgs))
					{
						openedDisposable.Disposable = Open(new ClassicDesktopStyleApplicationLifetimeTopLevelGroup(lifeTime), options, mainWindow, application);
						e.Handled = true;
					}
				}));
			}
		}
		return compositeDisposable;
	}

	private static IDisposable Open(IDevToolsTopLevelGroup topLevelGroup, DevToolsOptions options, Window? owner, Application? app)
	{
		Control control = owner?.FocusManager?.GetFocusedElement() as Control;
		AvaloniaObject avaloniaObject = ((topLevelGroup is ClassicDesktopStyleApplicationLifetimeTopLevelGroup group) ? new Avalonia.Diagnostics.Controls.Application(group, app ?? Application.Current) : ((!(topLevelGroup is SingleViewTopLevelGroup singleViewTopLevelGroup)) ? ((AvaloniaObject)new TopLevelGroup(topLevelGroup)) : ((AvaloniaObject)singleViewTopLevelGroup.Items.First())));
		AvaloniaObject root = avaloniaObject;
		if (s_open.TryGetValue(topLevelGroup, out MainWindow value))
		{
			value.Activate();
			value.SelectedControl(control);
			return Disposable.Empty;
		}
		if (topLevelGroup.Items.Count == 1 && !(topLevelGroup.Items is INotifyCollectionChanged))
		{
			TopLevel value2 = topLevelGroup.Items.First();
			foreach (KeyValuePair<IDevToolsTopLevelGroup, MainWindow> item in s_open)
			{
				if (item.Key.Items.Contains(value2))
				{
					item.Value.Activate();
					item.Value.SelectedControl(control);
					return Disposable.Empty;
				}
			}
		}
		MainWindow window = new MainWindow
		{
			Root = root,
			Width = options.Size.Width,
			Height = options.Size.Height,
			Tag = topLevelGroup
		};
		window.SetOptions(options);
		window.SelectedControl(control);
		window.Closed += DevToolsClosed;
		s_open.Add(topLevelGroup, window);
		if (options.ShowAsChildWindow && owner != null)
		{
			window.Show(owner);
		}
		else
		{
			window.Show();
		}
		return Disposable.Create(delegate
		{
			window.Close();
		});
	}

	private static void DevToolsClosed(object? sender, EventArgs e)
	{
		MainWindow mainWindow = (MainWindow)sender;
		mainWindow.Closed -= DevToolsClosed;
		s_open.Remove((IDevToolsTopLevelGroup)mainWindow.Tag);
	}
}
