using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFcitxFcitxInputMethod1
{
	private const string Interface = "org.fcitx.Fcitx.InputMethod1";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFcitxFcitxInputMethod1(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task<(ObjectPath Item1, byte[] Item2)> CreateInputContextAsync((string, string)[] arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_oay);
		MessageBuffer CreateMessage()
		{
			MessageWriter writer = _connection.GetMessageWriter();
			writer.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputMethod1", "CreateInputContext", "a(ss)");
			writer.WriteArray_arssz(arg0);
			MessageBuffer result = writer.CreateMessage();
			writer.Dispose();
			return result;
		}
	}
}
