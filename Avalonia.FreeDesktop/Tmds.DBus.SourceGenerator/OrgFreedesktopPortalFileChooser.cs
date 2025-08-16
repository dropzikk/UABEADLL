using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFreedesktopPortalFileChooser
{
	public class Properties
	{
		public uint version { get; set; }
	}

	private const string Interface = "org.freedesktop.portal.FileChooser";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFreedesktopPortalFileChooser(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task<ObjectPath> OpenFileAsync(string parent_window, string title, Dictionary<string, DBusVariantItem> options)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_o);
		MessageBuffer CreateMessage()
		{
			MessageWriter writer = _connection.GetMessageWriter();
			writer.WriteMethodCallHeader(_destination, _path, "org.freedesktop.portal.FileChooser", "OpenFile", "ssa{sv}");
			writer.WriteString(parent_window);
			writer.WriteString(title);
			writer.WriteDictionary_aesv(options);
			MessageBuffer result = writer.CreateMessage();
			writer.Dispose();
			return result;
		}
	}

	public Task<ObjectPath> SaveFileAsync(string parent_window, string title, Dictionary<string, DBusVariantItem> options)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_o);
		MessageBuffer CreateMessage()
		{
			MessageWriter writer = _connection.GetMessageWriter();
			writer.WriteMethodCallHeader(_destination, _path, "org.freedesktop.portal.FileChooser", "SaveFile", "ssa{sv}");
			writer.WriteString(parent_window);
			writer.WriteString(title);
			writer.WriteDictionary_aesv(options);
			MessageBuffer result = writer.CreateMessage();
			writer.Dispose();
			return result;
		}
	}

	public Task<ObjectPath> SaveFilesAsync(string parent_window, string title, Dictionary<string, DBusVariantItem> options)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_o);
		MessageBuffer CreateMessage()
		{
			MessageWriter writer = _connection.GetMessageWriter();
			writer.WriteMethodCallHeader(_destination, _path, "org.freedesktop.portal.FileChooser", "SaveFiles", "ssa{sv}");
			writer.WriteString(parent_window);
			writer.WriteString(title);
			writer.WriteDictionary_aesv(options);
			MessageBuffer result = writer.CreateMessage();
			writer.Dispose();
			return result;
		}
	}

	public Task<uint> GetVersionPropertyAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_u);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "Get", "ss");
			messageWriter.WriteString("org.freedesktop.portal.FileChooser");
			messageWriter.WriteString("version");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<Properties> GetAllPropertiesAsync()
	{
		return _connection.CallMethodAsync(CreateGetAllMessage(), delegate(Message message, object? state)
		{
			Reader reader = message.GetBodyReader();
			return ReadProperties(ref reader);
		});
		MessageBuffer CreateGetAllMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "GetAll", "s");
			messageWriter.WriteString("org.freedesktop.portal.FileChooser");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	private static Properties ReadProperties(ref Reader reader, List<string>? changed = null)
	{
		Properties properties = new Properties();
		ArrayEnd iterator = reader.ReadArrayStart(DBusType.Struct);
		while (reader.HasNext(iterator))
		{
			if (reader.ReadString() == "version")
			{
				reader.ReadSignature("u");
				properties.version = reader.ReadUInt32();
				changed?.Add("version");
			}
		}
		return properties;
	}

	public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<Properties>> handler, bool emitOnCapturedContext = true)
	{
		return SignalHelper.WatchPropertiesChangedAsync(_connection, _destination, _path, "org.freedesktop.portal.FileChooser", ReadMessage, handler, emitOnCapturedContext);
		static PropertyChanges<Properties> ReadMessage(Message message, object? _)
		{
			Reader reader = message.GetBodyReader();
			reader.ReadString();
			List<string> list = new List<string>();
			return new PropertyChanges<Properties>(ReadProperties(ref reader, list), list.ToArray(), reader.ReadArray_as());
		}
	}
}
