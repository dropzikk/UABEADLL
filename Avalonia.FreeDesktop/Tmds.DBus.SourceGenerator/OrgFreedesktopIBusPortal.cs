using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFreedesktopIBusPortal
{
	private const string Interface = "org.freedesktop.IBus.Portal";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFreedesktopIBusPortal(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task<ObjectPath> CreateInputContextAsync(string client_name)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_o);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.Portal", "CreateInputContext", "s");
			messageWriter.WriteString(client_name);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}
}
