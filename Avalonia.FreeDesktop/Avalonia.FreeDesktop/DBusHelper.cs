using System;
using System.Threading;
using Avalonia.Logging;
using Tmds.DBus.Protocol;

namespace Avalonia.FreeDesktop;

internal static class DBusHelper
{
	public static Connection? Connection { get; private set; }

	public static Connection? TryInitialize(string? dbusAddress = null)
	{
		return Connection ?? TryCreateNewConnection(dbusAddress);
	}

	public static Connection? TryCreateNewConnection(string? dbusAddress = null)
	{
		SynchronizationContext current = SynchronizationContext.Current;
		try
		{
			Connection connection = new Connection(new ClientConnectionOptions(dbusAddress ?? Address.Session)
			{
				AutoConnect = false
			});
			connection.ConnectAsync().GetAwaiter().GetResult();
			Connection = connection;
		}
		catch (Exception ex)
		{
			Logger.TryGet(LogEventLevel.Error, "DBUS")?.Log(null, "Unable to connect to DBus: " + ex);
		}
		finally
		{
			SynchronizationContext.SetSynchronizationContext(current);
		}
		return Connection;
	}
}
