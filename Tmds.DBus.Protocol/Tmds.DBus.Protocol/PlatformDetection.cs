using System;
using System.Runtime.Versioning;

namespace Tmds.DBus.Protocol;

internal static class PlatformDetection
{
	[SupportedOSPlatformGuard("windows")]
	public static bool IsWindows()
	{
		return OperatingSystem.IsWindows();
	}
}
