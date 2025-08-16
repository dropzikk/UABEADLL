using System;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal static class SignalHelper
{
	public static ValueTask<IDisposable> WatchSignalAsync(Connection connection, MatchRule rule, Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		return connection.AddMatchAsync(rule, (Message _, object? _) => (object)null, delegate(Exception e, object _, object? _, object? handlerState)
		{
			((Action<Exception>)handlerState)(e);
		}, null, handler, emitOnCapturedContext);
	}

	public static ValueTask<IDisposable> WatchSignalAsync<T>(Connection connection, MatchRule rule, MessageValueReader<T> reader, Action<Exception?, T> handler, bool emitOnCapturedContext = true)
	{
		return connection.AddMatchAsync(rule, reader, delegate(Exception e, T arg, object? readerState, object? handlerState)
		{
			((Action<Exception, T>)handlerState)(e, arg);
		}, null, handler, emitOnCapturedContext);
	}

	public static ValueTask<IDisposable> WatchPropertiesChangedAsync<T>(Connection connection, string destination, string path, string @interface, MessageValueReader<PropertyChanges<T>> reader, Action<Exception?, PropertyChanges<T>> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = destination,
			Path = path,
			Member = "PropertiesChanged",
			Interface = "org.freedesktop.DBus.Properties",
			Arg0 = @interface
		};
		return WatchSignalAsync(connection, rule, reader, handler, emitOnCapturedContext);
	}
}
