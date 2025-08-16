using Avalonia.Compatibility;
using Avalonia.Logging;

namespace Avalonia;

public static class AppBuilderDesktopExtensions
{
	public static AppBuilder UsePlatformDetect(this AppBuilder builder)
	{
		if (OperatingSystemEx.IsWindows())
		{
			LoadWin32(builder);
			LoadSkia(builder);
		}
		else if (OperatingSystemEx.IsMacOS())
		{
			LoadAvaloniaNative(builder);
			LoadSkia(builder);
		}
		else if (OperatingSystemEx.IsLinux())
		{
			LoadX11(builder);
			LoadSkia(builder);
		}
		else
		{
			Logger.TryGet(LogEventLevel.Warning, "Platform")?.Log(builder, "Avalonia.Desktop package was referenced on non-desktop platform or it isn't supported");
		}
		return builder;
	}

	private static void LoadAvaloniaNative(AppBuilder builder)
	{
		builder.UseAvaloniaNative();
	}

	private static void LoadWin32(AppBuilder builder)
	{
		builder.UseWin32();
	}

	private static void LoadX11(AppBuilder builder)
	{
		builder.UseX11();
	}

	private static void LoadSkia(AppBuilder builder)
	{
		builder.UseSkia();
	}
}
