using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgKdeStatusNotifierWatcher
{
	public class Properties
	{
		public string[] RegisteredStatusNotifierItems { get; set; }

		public bool IsStatusNotifierHostRegistered { get; set; }

		public int ProtocolVersion { get; set; }
	}

	private const string Interface = "org.kde.StatusNotifierWatcher";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgKdeStatusNotifierWatcher(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task RegisterStatusNotifierItemAsync(string service)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.kde.StatusNotifierWatcher", "RegisterStatusNotifierItem", "s");
			messageWriter.WriteString(service);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task RegisterStatusNotifierHostAsync(string service)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.kde.StatusNotifierWatcher", "RegisterStatusNotifierHost", "s");
			messageWriter.WriteString(service);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public ValueTask<IDisposable> WatchStatusNotifierItemRegisteredAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "StatusNotifierItemRegistered",
			Interface = "org.kde.StatusNotifierWatcher"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_s, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchStatusNotifierItemUnregisteredAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "StatusNotifierItemUnregistered",
			Interface = "org.kde.StatusNotifierWatcher"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_s, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchStatusNotifierHostRegisteredAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "StatusNotifierHostRegistered",
			Interface = "org.kde.StatusNotifierWatcher"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchStatusNotifierHostUnregisteredAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "StatusNotifierHostUnregistered",
			Interface = "org.kde.StatusNotifierWatcher"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public Task<string[]> GetRegisteredStatusNotifierItemsPropertyAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_as);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "Get", "ss");
			messageWriter.WriteString("org.kde.StatusNotifierWatcher");
			messageWriter.WriteString("RegisteredStatusNotifierItems");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<bool> GetIsStatusNotifierHostRegisteredPropertyAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_b);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "Get", "ss");
			messageWriter.WriteString("org.kde.StatusNotifierWatcher");
			messageWriter.WriteString("IsStatusNotifierHostRegistered");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<int> GetProtocolVersionPropertyAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_i);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "Get", "ss");
			messageWriter.WriteString("org.kde.StatusNotifierWatcher");
			messageWriter.WriteString("ProtocolVersion");
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
			messageWriter.WriteString("org.kde.StatusNotifierWatcher");
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
			switch (reader.ReadString())
			{
			case "RegisteredStatusNotifierItems":
				reader.ReadSignature("as");
				properties.RegisteredStatusNotifierItems = reader.ReadArray_as();
				changed?.Add("RegisteredStatusNotifierItems");
				break;
			case "IsStatusNotifierHostRegistered":
				reader.ReadSignature("b");
				properties.IsStatusNotifierHostRegistered = reader.ReadBool();
				changed?.Add("IsStatusNotifierHostRegistered");
				break;
			case "ProtocolVersion":
				reader.ReadSignature("i");
				properties.ProtocolVersion = reader.ReadInt32();
				changed?.Add("ProtocolVersion");
				break;
			}
		}
		return properties;
	}

	public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<Properties>> handler, bool emitOnCapturedContext = true)
	{
		return SignalHelper.WatchPropertiesChangedAsync(_connection, _destination, _path, "org.kde.StatusNotifierWatcher", ReadMessage, handler, emitOnCapturedContext);
		static PropertyChanges<Properties> ReadMessage(Message message, object? _)
		{
			Reader reader = message.GetBodyReader();
			reader.ReadString();
			List<string> list = new List<string>();
			return new PropertyChanges<Properties>(ReadProperties(ref reader, list), list.ToArray(), reader.ReadArray_as());
		}
	}
}
