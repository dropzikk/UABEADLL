using Avalonia.X11;

namespace Avalonia;

public static class AvaloniaX11PlatformExtensions
{
	public static AppBuilder UseX11(this AppBuilder builder)
	{
		builder.UseStandardRuntimePlatformSubsystem().UseWindowingSubsystem(delegate
		{
			new AvaloniaX11Platform().Initialize(AvaloniaLocator.Current.GetService<X11PlatformOptions>() ?? new X11PlatformOptions());
		});
		return builder;
	}

	public static void InitializeX11Platform(X11PlatformOptions options = null)
	{
		new AvaloniaX11Platform().Initialize(options ?? new X11PlatformOptions());
	}
}
