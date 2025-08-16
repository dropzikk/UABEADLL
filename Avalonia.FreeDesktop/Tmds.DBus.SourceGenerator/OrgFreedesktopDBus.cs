using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFreedesktopDBus
{
	public class Properties
	{
		public string[] Features { get; set; }

		public string[] Interfaces { get; set; }
	}

	private const string Interface = "org.freedesktop.DBus";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFreedesktopDBus(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task<string> HelloAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_s);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "Hello");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<uint> RequestNameAsync(string arg0, uint arg1)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_u);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "RequestName", "su");
			messageWriter.WriteString(arg0);
			messageWriter.WriteUInt32(arg1);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<uint> ReleaseNameAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_u);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "ReleaseName", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<uint> StartServiceByNameAsync(string arg0, uint arg1)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_u);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "StartServiceByName", "su");
			messageWriter.WriteString(arg0);
			messageWriter.WriteUInt32(arg1);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task UpdateActivationEnvironmentAsync(Dictionary<string, string> arg0)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter writer = _connection.GetMessageWriter();
			writer.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "UpdateActivationEnvironment", "a{ss}");
			writer.WriteDictionary_aess(arg0);
			MessageBuffer result = writer.CreateMessage();
			writer.Dispose();
			return result;
		}
	}

	public Task<bool> NameHasOwnerAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_b);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "NameHasOwner", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<string[]> ListNamesAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_as);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "ListNames");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<string[]> ListActivatableNamesAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_as);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "ListActivatableNames");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task AddMatchAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "AddMatch", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task RemoveMatchAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "RemoveMatch", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<string> GetNameOwnerAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_s);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "GetNameOwner", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<string[]> ListQueuedOwnersAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_as);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "ListQueuedOwners", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<uint> GetConnectionUnixUserAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_u);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "GetConnectionUnixUser", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<uint> GetConnectionUnixProcessIDAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_u);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "GetConnectionUnixProcessID", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<byte[]> GetAdtAuditSessionDataAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_ay);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "GetAdtAuditSessionData", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<byte[]> GetConnectionSELinuxSecurityContextAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_ay);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "GetConnectionSELinuxSecurityContext", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task ReloadConfigAsync()
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "ReloadConfig");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<string> GetIdAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_s);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "GetId");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<Dictionary<string, DBusVariantItem>> GetConnectionCredentialsAsync(string arg0)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_aesv);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus", "GetConnectionCredentials", "s");
			messageWriter.WriteString(arg0);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public ValueTask<IDisposable> WatchNameOwnerChangedAsync(Action<Exception?, (string Item1, string Item2, string Item3)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "NameOwnerChanged",
			Interface = "org.freedesktop.DBus"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_sss, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchNameLostAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "NameLost",
			Interface = "org.freedesktop.DBus"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_s, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchNameAcquiredAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "NameAcquired",
			Interface = "org.freedesktop.DBus"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_s, handler, emitOnCapturedContext);
	}

	public Task<string[]> GetFeaturesPropertyAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_as);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "Get", "ss");
			messageWriter.WriteString("org.freedesktop.DBus");
			messageWriter.WriteString("Features");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<string[]> GetInterfacesPropertyAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_as);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "Get", "ss");
			messageWriter.WriteString("org.freedesktop.DBus");
			messageWriter.WriteString("Interfaces");
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
			messageWriter.WriteString("org.freedesktop.DBus");
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
			string text = reader.ReadString();
			if (!(text == "Features"))
			{
				if (text == "Interfaces")
				{
					reader.ReadSignature("as");
					properties.Interfaces = reader.ReadArray_as();
					changed?.Add("Interfaces");
				}
			}
			else
			{
				reader.ReadSignature("as");
				properties.Features = reader.ReadArray_as();
				changed?.Add("Features");
			}
		}
		return properties;
	}

	public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<Properties>> handler, bool emitOnCapturedContext = true)
	{
		return SignalHelper.WatchPropertiesChangedAsync(_connection, _destination, _path, "org.freedesktop.DBus", ReadMessage, handler, emitOnCapturedContext);
		static PropertyChanges<Properties> ReadMessage(Message message, object? _)
		{
			Reader reader = message.GetBodyReader();
			reader.ReadString();
			List<string> list = new List<string>();
			return new PropertyChanges<Properties>(ReadProperties(ref reader, list), list.ToArray(), reader.ReadArray_as());
		}
	}
}
