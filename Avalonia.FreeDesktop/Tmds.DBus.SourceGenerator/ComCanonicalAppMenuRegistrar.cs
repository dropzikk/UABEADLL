using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class ComCanonicalAppMenuRegistrar
{
	private const string Interface = "com.canonical.AppMenu.Registrar";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public ComCanonicalAppMenuRegistrar(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task RegisterWindowAsync(uint windowId, ObjectPath menuObjectPath)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "com.canonical.AppMenu.Registrar", "RegisterWindow", "uo");
			messageWriter.WriteUInt32(windowId);
			messageWriter.WriteObjectPath(menuObjectPath);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task UnregisterWindowAsync(uint windowId)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "com.canonical.AppMenu.Registrar", "UnregisterWindow", "u");
			messageWriter.WriteUInt32(windowId);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<(string service, ObjectPath menuObjectPath)> GetMenuForWindowAsync(uint windowId)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_so);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "com.canonical.AppMenu.Registrar", "GetMenuForWindow", "u");
			messageWriter.WriteUInt32(windowId);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}
}
