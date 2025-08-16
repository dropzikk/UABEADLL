using System;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal class OrgFcitxFcitxInputContext1
{
	private const string Interface = "org.fcitx.Fcitx.InputContext1";

	private readonly Connection _connection;

	private readonly string _destination;

	private readonly string _path;

	public OrgFcitxFcitxInputContext1(Connection connection, string destination, string path)
	{
		_connection = connection;
		_destination = destination;
		_path = path;
	}

	public Task FocusInAsync()
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "FocusIn");
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
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "FocusOut");
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
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "Reset");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetCursorRectAsync(int x, int y, int w, int h)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "SetCursorRect", "iiii");
			messageWriter.WriteInt32(x);
			messageWriter.WriteInt32(y);
			messageWriter.WriteInt32(w);
			messageWriter.WriteInt32(h);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetCapabilityAsync(ulong caps)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "SetCapability", "t");
			messageWriter.WriteUInt64(caps);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetSurroundingTextAsync(string text, uint cursor, uint anchor)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "SetSurroundingText", "suu");
			messageWriter.WriteString(text);
			messageWriter.WriteUInt32(cursor);
			messageWriter.WriteUInt32(anchor);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task SetSurroundingTextPositionAsync(uint cursor, uint anchor)
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "SetSurroundingTextPosition", "uu");
			messageWriter.WriteUInt32(cursor);
			messageWriter.WriteUInt32(anchor);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task DestroyICAsync()
	{
		return _connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "DestroyIC");
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public Task<bool> ProcessKeyEventAsync(uint keyval, uint keycode, uint state, bool type, uint time)
	{
		return _connection.CallMethodAsync(CreateMessage(), ReaderExtensions.ReadMessage_b);
		MessageBuffer CreateMessage()
		{
			MessageWriter messageWriter = _connection.GetMessageWriter();
			messageWriter.WriteMethodCallHeader(_destination, _path, "org.fcitx.Fcitx.InputContext1", "ProcessKeyEvent", "uuubu");
			messageWriter.WriteUInt32(keyval);
			messageWriter.WriteUInt32(keycode);
			messageWriter.WriteUInt32(state);
			messageWriter.WriteBool(type);
			messageWriter.WriteUInt32(time);
			MessageBuffer result = messageWriter.CreateMessage();
			messageWriter.Dispose();
			return result;
		}
	}

	public ValueTask<IDisposable> WatchCommitStringAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "CommitString",
			Interface = "org.fcitx.Fcitx.InputContext1"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_s, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchCurrentIMAsync(Action<Exception?, (string name, string uniqueName, string langCode)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "CurrentIM",
			Interface = "org.fcitx.Fcitx.InputContext1"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_sss, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchUpdateFormattedPreeditAsync(Action<Exception?, ((string, int)[] str, int cursorpos)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "UpdateFormattedPreedit",
			Interface = "org.fcitx.Fcitx.InputContext1"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_arsizi, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchForwardKeyAsync(Action<Exception?, (uint keyval, uint state, bool type)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "ForwardKey",
			Interface = "org.fcitx.Fcitx.InputContext1"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_uub, handler, emitOnCapturedContext);
	}

	public ValueTask<IDisposable> WatchDeleteSurroundingTextAsync(Action<Exception?, (int offset, uint nchar)> handler, bool emitOnCapturedContext = true)
	{
		MatchRule rule = new MatchRule
		{
			Type = MessageType.Signal,
			Sender = _destination,
			Path = _path,
			Member = "DeleteSurroundingText",
			Interface = "org.fcitx.Fcitx.InputContext1"
		};
		return SignalHelper.WatchSignalAsync(_connection, rule, ReaderExtensions.ReadMessage_iu, handler, emitOnCapturedContext);
	}
}
