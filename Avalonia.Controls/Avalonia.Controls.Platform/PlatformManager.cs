using System;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Reactive;

namespace Avalonia.Controls.Platform;

[Unstable]
public static class PlatformManager
{
	private static bool s_designerMode;

	public static IDisposable DesignerMode()
	{
		s_designerMode = true;
		return Disposable.Create(delegate
		{
			s_designerMode = false;
		});
	}

	public static void SetDesignerScalingFactor(double factor)
	{
	}

	public static ITrayIconImpl? CreateTrayIcon()
	{
		if (!s_designerMode)
		{
			return AvaloniaLocator.Current.GetService<IWindowingPlatform>()?.CreateTrayIcon();
		}
		return null;
	}

	public static IWindowImpl CreateWindow()
	{
		IWindowingPlatform requiredService = AvaloniaLocator.Current.GetRequiredService<IWindowingPlatform>();
		if (!s_designerMode)
		{
			return requiredService.CreateWindow();
		}
		return requiredService.CreateEmbeddableWindow();
	}

	public static IWindowImpl CreateEmbeddableWindow()
	{
		return AvaloniaLocator.Current.GetRequiredService<IWindowingPlatform>().CreateEmbeddableWindow();
	}
}
