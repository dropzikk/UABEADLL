using System;

namespace Avalonia.Compatibility;

internal sealed class OperatingSystemEx
{
	public static bool IsWindows()
	{
		return OperatingSystem.IsWindows();
	}

	public static bool IsMacOS()
	{
		return OperatingSystem.IsMacOS();
	}

	public static bool IsLinux()
	{
		return OperatingSystem.IsLinux();
	}

	public static bool IsAndroid()
	{
		return OperatingSystem.IsAndroid();
	}

	public static bool IsIOS()
	{
		return OperatingSystem.IsIOS();
	}

	public static bool IsBrowser()
	{
		return OperatingSystem.IsBrowser();
	}

	public static bool IsOSPlatform(string platform)
	{
		return OperatingSystem.IsOSPlatform(platform);
	}
}
