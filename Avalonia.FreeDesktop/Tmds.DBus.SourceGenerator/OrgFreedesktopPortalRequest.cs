using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFreedesktopPortalRequest
{
	private const string Interface = "org.freedesktop.portal.Request";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFreedesktopPortalRequest(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task CloseAsync()
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.portal.Request", "Close");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public ValueTask<IDisposable> WatchResponseAsync(Action<Exception?, (uint response, Dictionary<string, DBusVariantItem> results)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "Response",
			Interface = "org.freedesktop.portal.Request"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_uaesv, handler, emitOnCapturedContext);
	}
}
