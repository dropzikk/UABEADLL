using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFreedesktopIBusService
{
	private const string Interface = "org.freedesktop.IBus.Service";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFreedesktopIBusService(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task DestroyAsync()
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.Service", "Destroy");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}
}
