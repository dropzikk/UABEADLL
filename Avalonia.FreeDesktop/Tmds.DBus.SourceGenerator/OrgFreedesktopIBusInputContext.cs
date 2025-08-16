using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFreedesktopIBusInputContext
{
	public class Properties
	{
		public (uint, uint) ContentType { get; set; }

		public ValueTuple<bool> ClientCommitPreedit { get; set; }
	}

	private const string Interface = "org.freedesktop.IBus.InputContext";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFreedesktopIBusInputContext(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task<bool> ProcessKeyEventAsync(uint keyval, uint keycode, uint state)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_b);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "ProcessKeyEvent", "uuu");
			messageWriter.WriteUInt32(keyval);
			messageWriter.WriteUInt32(keycode);
			messageWriter.WriteUInt32(state);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetCursorLocationAsync(int x, int y, int w, int h)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "SetCursorLocation", "iiii");
			messageWriter.WriteInt32(x);
			messageWriter.WriteInt32(y);
			messageWriter.WriteInt32(w);
			messageWriter.WriteInt32(h);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetCursorLocationRelativeAsync(int x, int y, int w, int h)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "SetCursorLocationRelative", "iiii");
			messageWriter.WriteInt32(x);
			messageWriter.WriteInt32(y);
			messageWriter.WriteInt32(w);
			messageWriter.WriteInt32(h);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task ProcessHandWritingEventAsync(double[] coordinates)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter writer = _connection.GetMessageWriter();
			writer.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "ProcessHandWritingEvent", "ad");
			writer.WriteArray_ad(coordinates);
			MessageBuffer result = writer.CreateMessage();
			writer.Dispose();
			return result;
		}
	}

	public Task CancelHandWritingAsync(uint n_strokes)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "CancelHandWriting", "u");
			messageWriter.WriteUInt32(n_strokes);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task FocusInAsync()
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "FocusIn");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task FocusOutAsync()
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "FocusOut");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task ResetAsync()
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "Reset");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetCapabilitiesAsync(uint caps)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "SetCapabilities", "u");
			messageWriter.WriteUInt32(caps);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task PropertyActivateAsync(string name, uint state)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "PropertyActivate", "su");
			messageWriter.WriteString(name);
			messageWriter.WriteUInt32(state);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetEngineAsync(string name)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "SetEngine", "s");
			messageWriter.WriteString(name);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<DBusVariantItem> GetEngineAsync()
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_v);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "GetEngine");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetSurroundingTextAsync(DBusVariantItem text, uint cursor_pos, uint anchor_pos)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter writer = _connection.GetMessageWriter();
			writer.WriteMethodCallHeader(_destination, _path, "org.freedesktop.IBus.InputContext", "SetSurroundingText", "vuu");
			writer.WriteDBusVariant(text);
			writer.WriteUInt32(cursor_pos);
			writer.WriteUInt32(anchor_pos);
			MessageBuffer result = writer.CreateMessage();
			writer.Dispose();
			return result;
		}
	}

	public ValueTask<IDisposable> WatchCommitTextAsync(Action<Exception?, DBusVariantItem> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "CommitText",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_v, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchForwardKeyEventAsync(Action<Exception?, (uint keyval, uint keycode, uint state)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "ForwardKeyEvent",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_uuu, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchUpdatePreeditTextAsync(Action<Exception?, (DBusVariantItem text, uint cursor_pos, bool visible)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "UpdatePreeditText",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_vub, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchUpdatePreeditTextWithModeAsync(Action<Exception?, (DBusVariantItem text, uint cursor_pos, bool visible, uint mode)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "UpdatePreeditTextWithMode",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_vubu, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchShowPreeditTextAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "ShowPreeditText",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchHidePreeditTextAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "HidePreeditText",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchUpdateAuxiliaryTextAsync(Action<Exception?, (DBusVariantItem text, bool visible)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "UpdateAuxiliaryText",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_vb, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchShowAuxiliaryTextAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "ShowAuxiliaryText",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchHideAuxiliaryTextAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "HideAuxiliaryText",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchUpdateLookupTableAsync(Action<Exception?, (DBusVariantItem table, bool visible)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "UpdateLookupTable",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_vb, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchShowLookupTableAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "ShowLookupTable",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchHideLookupTableAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "HideLookupTable",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchPageUpLookupTableAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "PageUpLookupTable",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchPageDownLookupTableAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "PageDownLookupTable",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchCursorUpLookupTableAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "CursorUpLookupTable",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchCursorDownLookupTableAsync(Action<Exception?> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "CursorDownLookupTable",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchRegisterPropertiesAsync(Action<Exception?, DBusVariantItem> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "RegisterProperties",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_v, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchUpdatePropertyAsync(Action<Exception?, DBusVariantItem> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "UpdateProperty",
			Interface = "org.freedesktop.IBus.InputContext"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_v, handler, emitOnCapturedContext);
	}

	public Task SetContentTypePropertyAsync((uint, uint) value)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "Set", "ssv");
			messageWriter.WriteString("org.freedesktop.IBus.InputContext");
			messageWriter.WriteString("ContentType");
			messageWriter.WriteVariant(value);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetClientCommitPreeditPropertyAsync(ValueTuple<bool> value)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.freedesktop.DBus.Properties", "Set", "ssv");
			messageWriter.WriteString("org.freedesktop.IBus.InputContext");
			messageWriter.WriteString("ClientCommitPreedit");
			messageWriter.WriteVariant(value);
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
			messageWriter.WriteString("org.freedesktop.IBus.InputContext");
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
			if (!(text == "ContentType"))
			{
				if (text == "ClientCommitPreedit")
				{
					reader.ReadSignature("(b)");
					properties.ClientCommitPreedit = reader.ReadStruct_rbz();
					changed?.Add("ClientCommitPreedit");
				}
			}
			else
			{
				reader.ReadSignature("(uu)");
				properties.ContentType = reader.ReadStruct_ruuz();
				changed?.Add("ContentType");
			}
		}
		return properties;
	}

	public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<Properties>> handler, bool emitOnCapturedContext = true)
	{
		return SignalHelper.WatchPropertiesChangedAsync(_connection, _destination, _path, "org.freedesktop.IBus.InputContext", ReadMessage, handler, emitOnCapturedContext);
		static PropertyChanges<Properties> ReadMessage(Message message, object? _)
		{
			Reader reader = message.GetBodyReader();
			reader.ReadString();
			List<string> list = new List<string>();
			return new PropertyChanges<Properties>(ReadProperties(ref reader, list), list.ToArray(), reader.ReadArray_as());
		}
	}
}
