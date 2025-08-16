using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tmds.DBus.Protocol;

public class Connection : IDisposable
{
	private enum ConnectionState
	{
		Created,
		Connecting,
		Connected,
		Disconnected
	}

	private static readonly Exception s_disposedSentinel = new ObjectDisposedException(typeof(Connection).FullName);

	private static Connection? s_systemConnection;

	private static Connection? s_sessionConnection;

	private readonly object _gate = new object();

	private readonly ClientConnectionOptions _connectionOptions;

	private DBusConnection? _connection;

	private CancellationTokenSource? _connectCts;

	private Task<DBusConnection>? _connectingTask;

	private ClientSetupResult? _setupResult;

	private ConnectionState _state;

	private bool _disposed;

	private int _nextSerial;

	public const string DBusObjectPath = "/org/freedesktop/DBus";

	public const string DBusServiceName = "org.freedesktop.DBus";

	public const string DBusInterface = "org.freedesktop.DBus";

	public static Connection System => s_systemConnection ?? CreateConnection(ref s_systemConnection, Address.System);

	public static Connection Session => s_sessionConnection ?? CreateConnection(ref s_sessionConnection, Address.Session);

	public string? UniqueName => GetConnection().UniqueName;

	public Connection(string address)
		: this(new ClientConnectionOptions(address))
	{
	}

	public Connection(ConnectionOptions connectionOptions)
	{
		if (connectionOptions == null)
		{
			throw new ArgumentNullException("connectionOptions");
		}
		_connectionOptions = (ClientConnectionOptions)connectionOptions;
	}

	internal void Connect(IMessageStream stream)
	{
		_connection = new DBusConnection(this);
		_connection.Connect(stream);
		_state = ConnectionState.Connected;
	}

	public async ValueTask ConnectAsync()
	{
		await ConnectCoreAsync(autoConnect: false).ConfigureAwait(continueOnCapturedContext: false);
	}

	private ValueTask<DBusConnection> ConnectCoreAsync(bool autoConnect = true)
	{
		lock (_gate)
		{
			ThrowHelper.ThrowIfDisposed(_disposed, this);
			ConnectionState state = _state;
			if (state == ConnectionState.Connected)
			{
				return new ValueTask<DBusConnection>(_connection);
			}
			if (!_connectionOptions.AutoConnect && (autoConnect || _state != 0))
			{
				throw new InvalidOperationException("Can only connect once using an explicit call.");
			}
			if (state == ConnectionState.Connecting)
			{
				return new ValueTask<DBusConnection>(_connectingTask);
			}
			_state = ConnectionState.Connecting;
			_connectingTask = DoConnectAsync();
			return new ValueTask<DBusConnection>(_connectingTask);
		}
	}

	private async Task<DBusConnection> DoConnectAsync()
	{
		DBusConnection connection = null;
		try
		{
			_connectCts = new CancellationTokenSource();
			_setupResult = await _connectionOptions.SetupAsync(_connectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			connection = (_connection = new DBusConnection(this));
			await connection.ConnectAsync(_setupResult.ConnectionAddress, _setupResult.UserId, _setupResult.SupportsFdPassing, _connectCts.Token).ConfigureAwait(continueOnCapturedContext: false);
			lock (_gate)
			{
				ThrowHelper.ThrowIfDisposed(_disposed, this);
				if (_connection == connection && _state == ConnectionState.Connecting)
				{
					_connectingTask = null;
					_connectCts = null;
					_state = ConnectionState.Connected;
					return connection;
				}
				throw new DisconnectedException(connection.DisconnectReason);
			}
		}
		catch (Exception ex)
		{
			Disconnect(ex, connection);
			ThrowHelper.ThrowIfDisposed(_disposed, this);
			if (ex is DisconnectedException || ex is ConnectException)
			{
				throw;
			}
			throw new ConnectException(ex.Message, ex);
		}
	}

	public void Dispose()
	{
		lock (_gate)
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
		}
		Disconnect(s_disposedSentinel);
	}

	internal void Disconnect(Exception disconnectReason, DBusConnection? trigger = null)
	{
		DBusConnection connection;
		ClientSetupResult setupResult;
		CancellationTokenSource connectCts;
		lock (_gate)
		{
			if ((trigger != null && trigger != _connection) || _state == ConnectionState.Disconnected)
			{
				return;
			}
			_state = ConnectionState.Disconnected;
			connection = _connection;
			setupResult = _setupResult;
			connectCts = _connectCts;
			_connectingTask = null;
			_setupResult = null;
			_connectCts = null;
			if (connection != null)
			{
				connection.DisconnectReason = disconnectReason;
			}
		}
		connectCts?.Cancel();
		connection?.Dispose();
		if (setupResult != null)
		{
			_connectionOptions.Teardown(setupResult.TeardownToken);
		}
	}

	public async Task CallMethodAsync(MessageBuffer message)
	{
		DBusConnection dBusConnection;
		try
		{
			dBusConnection = await ConnectCoreAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			message.ReturnToPool();
			throw;
		}
		await dBusConnection.CallMethodAsync(message).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<T> CallMethodAsync<T>(MessageBuffer message, MessageValueReader<T> reader, object? readerState = null)
	{
		DBusConnection dBusConnection;
		try
		{
			dBusConnection = await ConnectCoreAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			message.ReturnToPool();
			throw;
		}
		return await dBusConnection.CallMethodAsync(message, reader, readerState).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async ValueTask<IDisposable> AddMatchAsync<T>(MatchRule rule, MessageValueReader<T> reader, Action<Exception?, T, object?, object?> handler, object? readerState = null, object? handlerState = null, bool emitOnCapturedContext = true, bool subscribe = true)
	{
		SynchronizationContext synchronizationContext = (emitOnCapturedContext ? SynchronizationContext.Current : null);
		return await (await ConnectCoreAsync().ConfigureAwait(continueOnCapturedContext: false)).AddMatchAsync(synchronizationContext, rule, reader, handler, readerState, handlerState, subscribe).ConfigureAwait(continueOnCapturedContext: false);
	}

	public void AddMethodHandler(IMethodHandler methodHandler)
	{
		AddMethodHandlers(new IMethodHandler[1] { methodHandler });
	}

	public void AddMethodHandlers(IList<IMethodHandler> methodHandlers)
	{
		GetConnection().AddMethodHandlers(methodHandlers);
	}

	private static Connection CreateConnection(ref Connection? field, string? address)
	{
		address = address ?? "unix:";
		Connection connection = Volatile.Read(ref field);
		if (connection != null)
		{
			return connection;
		}
		Connection connection2 = new Connection(new ClientConnectionOptions(address)
		{
			AutoConnect = true
		});
		connection = Interlocked.CompareExchange(ref field, connection2, null);
		if (connection != null)
		{
			connection2.Dispose();
			return connection;
		}
		return connection2;
	}

	public MessageWriter GetMessageWriter()
	{
		return new MessageWriter(MessageBufferPool.Shared, GetNextSerial());
	}

	public bool TrySendMessage(MessageBuffer message)
	{
		DBusConnection connection = GetConnection(ifConnected: true);
		if (connection == null)
		{
			message.ReturnToPool();
			return false;
		}
		connection.SendMessage(message);
		return true;
	}

	public Task<Exception?> DisconnectedAsync()
	{
		return GetConnection().DisconnectedAsync();
	}

	private DBusConnection GetConnection()
	{
		return GetConnection(ifConnected: false);
	}

	private DBusConnection? GetConnection(bool ifConnected)
	{
		lock (_gate)
		{
			ThrowHelper.ThrowIfDisposed(_disposed, this);
			if (_connectionOptions.AutoConnect)
			{
				throw new InvalidOperationException("Method cannot be used on autoconnect connections.");
			}
			ConnectionState state = _state;
			if (state == ConnectionState.Created || state == ConnectionState.Connecting)
			{
				throw new InvalidOperationException("Connect before using this method.");
			}
			if (ifConnected && state != ConnectionState.Connected)
			{
				return null;
			}
			return _connection;
		}
	}

	internal uint GetNextSerial()
	{
		return (uint)Interlocked.Increment(ref _nextSerial);
	}

	public Task<string[]> ListServicesAsync()
	{
		return CallMethodAsync(CreateMessage(), (Message m, object? s) => m.GetBodyReader().ReadArray<string>());
		MessageBuffer CreateMessage()
		{
			using MessageWriter messageWriter = GetMessageWriter();
			messageWriter.WriteMethodCallHeader("org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus", "ListNames");
			return messageWriter.CreateMessage();
		}
	}
}
