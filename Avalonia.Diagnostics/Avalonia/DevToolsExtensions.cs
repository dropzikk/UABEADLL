using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Input;

namespace Avalonia;

public static class DevToolsExtensions
{
	public static void AttachDevTools(this TopLevel root)
	{
		DevTools.Attach(root, new DevToolsOptions());
	}

	public static void AttachDevTools(this TopLevel root, KeyGesture gesture)
	{
		DevTools.Attach(root, gesture);
	}

	public static void AttachDevTools(this TopLevel root, DevToolsOptions options)
	{
		DevTools.Attach(root, options);
	}

	public static void AttachDevTools(this Application application)
	{
		DevTools.Attach(application, new DevToolsOptions());
	}

	public static void AttachDevTools(this Application application, DevToolsOptions options)
	{
		DevTools.Attach(application, options);
	}
}
