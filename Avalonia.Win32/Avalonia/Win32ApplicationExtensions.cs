using Avalonia.Win32;

namespace Avalonia;

public static class Win32ApplicationExtensions
{
	public static AppBuilder UseWin32(this AppBuilder builder)
	{
		return builder.UseStandardRuntimePlatformSubsystem().UseWindowingSubsystem(delegate
		{
			Win32Platform.Initialize(AvaloniaLocator.Current.GetService<Win32PlatformOptions>() ?? new Win32PlatformOptions());
		}, "Win32");
	}
}
