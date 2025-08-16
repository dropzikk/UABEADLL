using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Tmds.DBus.Protocol;

internal static class DBusEnvironment
{
	public static string? UserId
	{
		get
		{
			if (PlatformDetection.IsWindows())
			{
				return WindowsIdentity.GetCurrent().User?.Value;
			}
			return geteuid().ToString();
		}
	}

	public static string MachineId
	{
		get
		{
			if (File.Exists("/var/lib/dbus/machine-id"))
			{
				return Guid.Parse(File.ReadAllText("/var/lib/dbus/machine-id").Substring(0, 32)).ToString();
			}
			return Guid.Empty.ToString();
		}
	}

	[DllImport("libc")]
	internal static extern uint geteuid();
}
