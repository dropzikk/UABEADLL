using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFcitxFcitxInputMethod
{
	private const string Interface = "org.fcitx.Fcitx.InputMethod";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFcitxFcitxInputMethod(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task<(int icid, bool enable, uint keyval1, uint state1, uint keyval2, uint state2)> CreateICv3Async(string appname, int pid)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_ibuuuu);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputMethod", "CreateICv3", "si");
			messageWriter.WriteString(appname);
			messageWriter.WriteInt32(pid);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}
}
