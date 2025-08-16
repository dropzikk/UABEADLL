using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Tmds.DBus.Protocol;

internal class DBusConnection : IDisposable
{
	private delegate void MessageReceivedHandler(Exception? exception, Message message, object? state);

	private class MyValueTaskSource<T> : IValueTaskSource<T>, IValueTaskSource
	{
		private ManualResetValueTaskSourceCore<T> _core;

		private volatile bool _continuationSet;

		public void SetResult(T result)
		{
			SpinWait spinWait = default(SpinWait);
			while (!_continuationSet)
			{
				spinWait.SpinOnce();
			}
			_core.SetResult(result);
		}

		public void SetException(Exception exception)
		{
			_core.SetException(exception);
		}

		public ValueTaskSourceStatus GetStatus(short token)
		{
			return _core.GetStatus(token);
		}

		public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
		{
			_core.OnCompleted(continuation, state, token, flags);
			_continuationSet = true;
		}

		T IValueTaskSource<T>.GetResult(short token)
		{
			return _core.GetResult(token);
		}

		void IValueTaskSource.GetResult(short token)
		{
			_core.GetResult(token);
		}
	}

	private enum ConnectionState
	{
		Created,
		Connecting,
		Connected,
		Disconnected
	}

	private delegate void MessageHandlerDelegate(Exception? exception, Message message, object? state1, object? state2, object? state3);

	private readonly struct MessageHandler
	{
		private readonly MessageHandlerDelegate _delegate;

		private readonly object? _state1;

		private readonly object? _state2;

		private readonly object? _state3;

		public bool HasValue => _delegate != null;

		public MessageHandler(MessageHandlerDelegate handler, object? state1 = null, object? state2 = null, object? state3 = null)
		{
			_delegate = handler;
			_state1 = state1;
			_state2 = state2;
			_state3 = state3;
		}

		public void Invoke(Exception? exception, Message message)
		{
			_delegate(exception, message, _state1, _state2, _state3);
		}
	}

	private delegate void MessageHandlerDelegate4(Exception? exception, Message message, object? state1, object? state2, object? state3, object? state4);

	private readonly struct MessageHandler4
	{
		private readonly MessageHandlerDelegate4 _delegate;

		private readonly object? _state1;

		private readonly object? _state2;

		private readonly object? _state3;

		private readonly object? _state4;

		public bool HasValue => _delegate != null;

		public MessageHandler4(MessageHandlerDelegate4 handler, object? state1 = null, object? state2 = null, object? state3 = null, object? state4 = null)
		{
			_delegate = handler;
			_state1 = state1;
			_state2 = state2;
			_state3 = state3;
			_state4 = state4;
		}

		public void Invoke(Exception? exception, Message message)
		{
			_delegate(exception, message, _state1, _state2, _state3, _state4);
		}
	}

	private sealed class Observer : IDisposable
	{
		private static readonly ObjectDisposedException s_objectDisposedException = new ObjectDisposedException(typeof(Observer).FullName);

		private readonly object _gate = new object();

		private readonly SynchronizationContext? _synchronizationContext;

		private readonly MatchMaker _matchMaker;

		private readonly MessageHandler4 _messageHandler;

		private bool _disposed;

		public bool Subscribes { get; }

		public Observer(SynchronizationContext? synchronizationContext, MatchMaker matchMaker, in MessageHandler4 messageHandler, bool subscribes)
		{
			_synchronizationContext = synchronizationContext;
			_matchMaker = matchMaker;
			_messageHandler = messageHandler;
			Subscribes = subscribes;
		}

		public void Dispose()
		{
			Dispose(s_objectDisposedException);
		}

		public void Dispose(Exception? exception, bool removeObserver = true)
		{
			lock (_gate)
			{
				if (_disposed)
				{
					return;
				}
				_disposed = true;
			}
			if (exception != null)
			{
				Emit(exception);
			}
			if (removeObserver)
			{
				_matchMaker.Connection.RemoveObserver(_matchMaker, this);
			}
		}

		public void Emit(Message message)
		{
			if (_synchronizationContext == null)
			{
				InvokeHandler(message);
			}
			else
			{
				_matchMaker.Connection.EmitOnSynchronizationContextHelper(this, _synchronizationContext, message);
			}
		}

		private void Emit(Exception exception)
		{
			if (_synchronizationContext == null || SynchronizationContext.Current == _synchronizationContext)
			{
				_messageHandler.Invoke(exception, null);
				return;
			}
			_synchronizationContext.Send(delegate
			{
				SynchronizationContext current = SynchronizationContext.Current;
				try
				{
					SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
					_messageHandler.Invoke(exception, null);
				}
				finally
				{
					SynchronizationContext.SetSynchronizationContext(current);
				}
			}, null);
		}

		internal void InvokeHandler(Message message)
		{
			if (Subscribes && !_matchMaker.HasSubscribed)
			{
				return;
			}
			lock (_gate)
			{
				if (!_disposed)
				{
					_messageHandler.Invoke(null, message);
				}
			}
		}
	}

	private sealed class MatchMaker
	{
		private readonly MessageType? _type;

		private readonly byte[]? _sender;

		private readonly byte[]? _interface;

		private readonly byte[]? _member;

		private readonly byte[]? _path;

		private readonly byte[]? _pathNamespace;

		private readonly byte[]? _destination;

		private readonly byte[]? _arg0;

		private readonly byte[]? _arg0Path;

		private readonly byte[]? _arg0Namespace;

		private readonly string _rule;

		private MyValueTaskSource<object?>? _vts;

		public List<Observer> Observers { get; } = new List<Observer>();

		public MyValueTaskSource<object?>? AddMatchTcs
		{
			get
			{
				return _vts;
			}
			set
			{
				_vts = value;
				if (value != null)
				{
					AddMatchTask = new ValueTask<object>(value, 0).AsTask();
				}
			}
		}

		public Task<object?>? AddMatchTask { get; private set; }

		public bool HasSubscribed { get; set; }

		public DBusConnection Connection { get; }

		public string RuleString => _rule;

		public bool HasSubscribers
		{
			get
			{
				if (Observers.Count == 0)
				{
					return false;
				}
				foreach (Observer observer in Observers)
				{
					if (observer.Subscribes)
					{
						return true;
					}
				}
				return false;
			}
		}

		public MatchMaker(DBusConnection connection, string rule, in MatchRuleData data)
		{
			Connection = connection;
			_rule = rule;
			_type = data.MessageType;
			if (data.Sender != null && data.Sender.StartsWith(":"))
			{
				_sender = Encoding.UTF8.GetBytes(data.Sender);
			}
			if (data.Interface != null)
			{
				_interface = Encoding.UTF8.GetBytes(data.Interface);
			}
			if (data.Member != null)
			{
				_member = Encoding.UTF8.GetBytes(data.Member);
			}
			if (data.Path != null)
			{
				_path = Encoding.UTF8.GetBytes(data.Path);
			}
			if (data.PathNamespace != null)
			{
				_pathNamespace = Encoding.UTF8.GetBytes(data.PathNamespace);
			}
			if (data.Destination != null)
			{
				_destination = Encoding.UTF8.GetBytes(data.Destination);
			}
			if (data.Arg0 != null)
			{
				_arg0 = Encoding.UTF8.GetBytes(data.Arg0);
			}
			if (data.Arg0Path != null)
			{
				_arg0Path = Encoding.UTF8.GetBytes(data.Arg0Path);
			}
			if (data.Arg0Namespace != null)
			{
				_arg0Namespace = Encoding.UTF8.GetBytes(data.Arg0Namespace);
			}
		}

		public override string ToString()
		{
			return _rule;
		}

		internal bool Matches(Message message)
		{
			if (_type.HasValue && _type != message.MessageType)
			{
				return false;
			}
			if (_sender != null && !IsEqual(_sender, message.Sender))
			{
				return false;
			}
			if (_interface != null && !IsEqual(_interface, message.Interface))
			{
				return false;
			}
			if (_member != null && !IsEqual(_member, message.Member))
			{
				return false;
			}
			if (_path != null && !IsEqual(_path, message.Path))
			{
				return false;
			}
			if (_destination != null && !IsEqual(_destination, message.Destination))
			{
				return false;
			}
			if (_pathNamespace != null && (!message.PathIsSet || !IsEqualOrChildOfPath(message.Path, _pathNamespace)))
			{
				return false;
			}
			if (_arg0Namespace != null || _arg0 != null || _arg0Path != null)
			{
				if (message.Signature.Length == 0)
				{
					return false;
				}
				DBusType dBusType = (DBusType)message.Signature[0];
				if (dBusType != DBusType.String && dBusType != DBusType.ObjectPath)
				{
					return false;
				}
				ReadOnlySpan<byte> readOnlySpan = message.GetBodyReader().ReadStringAsSpan();
				if (_arg0Path != null && !IsEqualParentOrChildOfPath(readOnlySpan, _arg0Path))
				{
					return false;
				}
				if (dBusType != DBusType.String)
				{
					return false;
				}
				if (_arg0 != null && !IsEqual(_arg0, readOnlySpan))
				{
					return false;
				}
				if (_arg0Namespace != null && !IsEqualOrChildOfName(readOnlySpan, _arg0Namespace))
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsEqualOrChildOfName(ReadOnlySpan<byte> lhs, ReadOnlySpan<byte> rhs)
		{
			if (lhs.StartsWith(rhs))
			{
				if (lhs.Length != rhs.Length)
				{
					return lhs[rhs.Length] == 46;
				}
				return true;
			}
			return false;
		}

		private static bool IsEqualOrChildOfPath(ReadOnlySpan<byte> lhs, ReadOnlySpan<byte> rhs)
		{
			if (lhs.StartsWith(rhs))
			{
				if (lhs.Length != rhs.Length)
				{
					return lhs[rhs.Length] == 47;
				}
				return true;
			}
			return false;
		}

		private static bool IsEqualParentOrChildOfPath(ReadOnlySpan<byte> lhs, ReadOnlySpan<byte> rhs)
		{
			if (rhs.Length < lhs.Length)
			{
				if (rhs[rhs.Length - 1] == 47)
				{
					return lhs.StartsWith(rhs);
				}
				return false;
			}
			if (lhs.Length < rhs.Length)
			{
				if (lhs[lhs.Length - 1] == 47)
				{
					return rhs.StartsWith(lhs);
				}
				return false;
			}
			return IsEqual(lhs, rhs);
		}

		private static bool IsEqual(ReadOnlySpan<byte> lhs, ReadOnlySpan<byte> rhs)
		{
			return lhs.SequenceEqual(rhs);
		}
	}

	private readonly object _gate = new object();

	private readonly Connection _parentConnection;

	private readonly Dictionary<uint, MessageHandler> _pendingCalls;

	private readonly CancellationTokenSource _connectCts;

	private readonly Dictionary<string, MatchMaker> _matchMakers;

	private readonly List<Observer> _matchedObservers;

	private readonly Dictionary<string, IMethodHandler> _pathHandlers;

	private IMessageStream? _messageStream;

	private ConnectionState _state;

	private Exception? _disconnectReason;

	private string? _localName;

	private Message? _currentMessage;

	private Observer? _currentObserver;

	private SynchronizationContext? _currentSynchronizationContext;

	private TaskCompletionSource<Exception?>? _disconnectedTcs;

	public string? UniqueName => _localName;

	public Exception DisconnectReason
	{
		get
		{
			return _disconnectReason ?? new ObjectDisposedException(GetType().FullName);
		}
		set
		{
			Interlocked.CompareExchange(ref _disconnectReason, value, null);
		}
	}

	public bool RemoteIsBus => _localName != null;

	public DBusConnection(Connection parent)
	{
		_parentConnection = parent;
		_connectCts = new CancellationTokenSource();
		_pendingCalls = new Dictionary<uint, MessageHandler>();
		_matchMakers = new Dictionary<string, MatchMaker>();
		_matchedObservers = new List<Observer>();
		_pathHandlers = new Dictionary<string, IMethodHandler>();
	}

	internal void Connect(IMessageStream stream)
	{
		_messageStream = stream;
		stream.ReceiveMessages(delegate(Exception? exception, Message message, DBusConnection connection)
		{
			connection.HandleMessages(exception, message);
		}, this);
		_state = ConnectionState.Connected;
	}

	public async ValueTask ConnectAsync(string address, string? userId, bool supportsFdPassing, CancellationToken cancellationToken)
	{
		_state = ConnectionState.Connecting;
		Exception firstException = null;
		AddressParser.AddressEntry addr = default(AddressParser.AddressEntry);
		while (AddressParser.TryGetNextEntry(address, ref addr))
		{
			Socket socket = null;
			EndPoint remoteEP = null;
			Guid guid = default(Guid);
			if (AddressParser.IsType(addr, "unix"))
			{
				AddressParser.ParseUnixProperties(addr, out string path, out guid);
				socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
				remoteEP = new UnixDomainSocketEndPoint(path);
			}
			else if (AddressParser.IsType(addr, "tcp"))
			{
				AddressParser.ParseTcpProperties(addr, out string host, out int? port, out guid);
				if (!port.HasValue)
				{
					throw new ArgumentException("port");
				}
				socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
				remoteEP = new DnsEndPoint(host, port.Value);
			}
			if (socket == null)
			{
				continue;
			}
			try
			{
				await socket.ConnectAsync(remoteEP, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				MessageStream stream;
				lock (_gate)
				{
					if (_state != ConnectionState.Connecting)
					{
						throw new DisconnectedException(DisconnectReason);
					}
					DBusConnection dBusConnection = this;
					MessageStream messageStream;
					stream = (messageStream = new MessageStream(socket));
					dBusConnection._messageStream = messageStream;
				}
				await stream.DoClientAuthAsync(guid, userId, supportsFdPassing).ConfigureAwait(continueOnCapturedContext: false);
				stream.ReceiveMessages(delegate(Exception? exception, Message message, DBusConnection connection)
				{
					connection.HandleMessages(exception, message);
				}, this);
				lock (_gate)
				{
					if (_state != ConnectionState.Connecting)
					{
						throw new DisconnectedException(DisconnectReason);
					}
					_state = ConnectionState.Connected;
				}
				_localName = await GetLocalNameAsync().ConfigureAwait(continueOnCapturedContext: false);
				return;
			}
			catch (Exception ex)
			{
				socket.Dispose();
				if (firstException == null)
				{
					firstException = ex;
				}
			}
		}
		if (firstException != null)
		{
			throw firstException;
		}
		throw new ArgumentException("No addresses were found", "address");
	}

	private async Task<string?> GetLocalNameAsync()
	{
		MyValueTaskSource<string?> vts = new MyValueTaskSource<string>();
		await CallMethodAsync(CreateHelloMessage(), delegate(Exception? exception, Message message, object? state)
		{
			MyValueTaskSource<string> myValueTaskSource = (MyValueTaskSource<string>)state;
			if (exception != null)
			{
				myValueTaskSource.SetException(exception);
			}
			else if (message.MessageType == MessageType.MethodReturn)
			{
				myValueTaskSource.SetResult(message.GetBodyReader().ReadString().ToString());
			}
			else
			{
				myValueTaskSource.SetResult(null);
			}
		}, vts).ConfigureAwait(continueOnCapturedContext: false);
		return await new ValueTask<string>(vts, 0).ConfigureAwait(continueOnCapturedContext: false);
		MessageBuffer CreateHelloMessage()
		{
			using MessageWriter messageWriter = GetMessageWriter();
			messageWriter.WriteMethodCallHeader("org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus", "Hello");
			return messageWriter.CreateMessage();
		}
	}

	private async void HandleMessages(Exception? exception, Message message)
	{
		if (exception != null)
		{
			_parentConnection.Disconnect(exception, this);
			return;
		}
		try
		{
			bool returnMessageToPool = true;
			MessageHandler value = default(MessageHandler);
			IMethodHandler value2 = null;
			bool flag = message.MessageType == MessageType.MethodCall;
			lock (_gate)
			{
				if (_state == ConnectionState.Disconnected)
				{
					return;
				}
				if (message.ReplySerial.HasValue)
				{
					_pendingCalls.Remove(message.ReplySerial.Value, out value);
				}
				foreach (MatchMaker value3 in _matchMakers.Values)
				{
					if (value3.Matches(message))
					{
						_matchedObservers.AddRange(value3.Observers);
					}
				}
				if (flag && message.PathIsSet)
				{
					_pathHandlers.TryGetValue(message.PathAsString, out value2);
				}
			}
			if (_matchedObservers.Count != 0)
			{
				foreach (Observer matchedObserver in _matchedObservers)
				{
					matchedObserver.Emit(message);
				}
				_matchedObservers.Clear();
			}
			if (value.HasValue)
			{
				value.Invoke(null, message);
			}
			if (flag)
			{
				MethodContext context = new MethodContext(_parentConnection, message);
				if (value2 != null)
				{
					if (value2.RunMethodHandlerSynchronously(message))
					{
						await value2.HandleMethodAsync(context);
						SendUnknownMethodErrorIfNoReplySent(context);
					}
					else
					{
						returnMessageToPool = false;
						RunMethodHandler(value2, context);
					}
				}
				else
				{
					SendUnknownMethodErrorIfNoReplySent(context);
				}
			}
			if (returnMessageToPool)
			{
				message.ReturnToPool();
			}
		}
		catch (Exception disconnectReason)
		{
			_parentConnection.Disconnect(disconnectReason, this);
		}
	}

	private void SendUnknownMethodErrorIfNoReplySent(MethodContext context)
	{
		if (!context.ReplySent && !context.NoReplyExpected)
		{
			Message request = context.Request;
			context.ReplyError("org.freedesktop.DBus.Error.UnknownMethod", string.Format("Method \"{0}\" with signature \"{1}\" on interface \"{2}\" doesn't exist", request.MemberAsString ?? "", request.SignatureAsString ?? "", request.InterfaceAsString ?? ""));
		}
	}

	private async void RunMethodHandler(IMethodHandler methodHandler, MethodContext context)
	{
		try
		{
			await methodHandler.HandleMethodAsync(context);
			SendUnknownMethodErrorIfNoReplySent(context);
			context.Request.ReturnToPool();
		}
		catch (Exception disconnectReason)
		{
			_parentConnection.Disconnect(disconnectReason, this);
		}
	}

	private void EmitOnSynchronizationContextHelper(Observer observer, SynchronizationContext synchronizationContext, Message message)
	{
		_currentMessage = message;
		_currentObserver = observer;
		_currentSynchronizationContext = synchronizationContext;
		synchronizationContext.Send(delegate(object? o)
		{
			SynchronizationContext current = SynchronizationContext.Current;
			try
			{
				DBusConnection dBusConnection = (DBusConnection)o;
				SynchronizationContext.SetSynchronizationContext(dBusConnection._currentSynchronizationContext);
				dBusConnection._currentObserver.InvokeHandler(dBusConnection._currentMessage);
			}
			finally
			{
				SynchronizationContext.SetSynchronizationContext(current);
			}
		}, this);
		_currentMessage = null;
		_currentObserver = null;
		_currentSynchronizationContext = null;
	}

	public void AddMethodHandlers(IList<IMethodHandler> methodHandlers)
	{
		lock (_gate)
		{
			if (_state == ConnectionState.Disconnected)
			{
				return;
			}
			int num = 0;
			try
			{
				for (int i = 0; i < methodHandlers.Count; i++)
				{
					IMethodHandler methodHandler = methodHandlers[i];
					_pathHandlers.Add(methodHandler.Path, methodHandler);
					num++;
				}
			}
			catch
			{
				for (int j = 0; j < num; j++)
				{
					IMethodHandler methodHandler2 = methodHandlers[j];
					_pathHandlers.Remove(methodHandler2.Path);
				}
			}
		}
	}

	public void Dispose()
	{
		lock (_gate)
		{
			if (_state == ConnectionState.Disconnected)
			{
				return;
			}
			_state = ConnectionState.Disconnected;
		}
		Exception disconnectReason = DisconnectReason;
		_messageStream?.Close(disconnectReason);
		if (_pendingCalls != null)
		{
			foreach (MessageHandler value in _pendingCalls.Values)
			{
				value.Invoke(new DisconnectedException(disconnectReason), null);
			}
			_pendingCalls.Clear();
		}
		foreach (MatchMaker value2 in _matchMakers.Values)
		{
			foreach (Observer observer in value2.Observers)
			{
				observer.Dispose(new DisconnectedException(disconnectReason), removeObserver: false);
			}
		}
		_matchMakers.Clear();
		_disconnectedTcs?.SetResult(GetWaitForDisconnectException());
	}

	private ValueTask CallMethodAsync(MessageBuffer message, MessageReceivedHandler returnHandler, object? state)
	{
		MessageHandlerDelegate handler = delegate(Exception? exception, Message message, object? state1, object? state2, object? state3)
		{
			((MessageReceivedHandler)state1)(exception, message, state2);
		};
		MessageHandler handler2 = new MessageHandler(handler, returnHandler, state);
		return CallMethodAsync(message, handler2);
	}

	private async ValueTask CallMethodAsync(MessageBuffer message, MessageHandler handler)
	{
		bool messageSent = false;
		try
		{
			lock (_gate)
			{
				if (_state != ConnectionState.Connected)
				{
					throw new DisconnectedException(DisconnectReason);
				}
				if ((message.MessageFlags & MessageFlags.NoReplyExpected) == 0)
				{
					_pendingCalls.Add(message.Serial, handler);
				}
			}
			messageSent = await _messageStream.TrySendMessageAsync(message).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			if (!messageSent)
			{
				message.ReturnToPool();
			}
		}
	}

	public async Task<T> CallMethodAsync<T>(MessageBuffer message, MessageValueReader<T> valueReader, object? state = null)
	{
		MessageHandlerDelegate handler = delegate(Exception? exception, Message message, object? state1, object? state2, object? state3)
		{
			MessageValueReader<T> messageValueReader = (MessageValueReader<T>)state1;
			MyValueTaskSource<T> myValueTaskSource = (MyValueTaskSource<T>)state2;
			if (exception != null)
			{
				myValueTaskSource.SetException(exception);
			}
			else
			{
				if (message.MessageType == MessageType.MethodReturn)
				{
					try
					{
						myValueTaskSource.SetResult(messageValueReader(message, state3));
						return;
					}
					catch (Exception exception2)
					{
						myValueTaskSource.SetException(exception2);
						return;
					}
				}
				if (message.MessageType == MessageType.Error)
				{
					myValueTaskSource.SetException(CreateDBusExceptionForErrorMessage(message));
				}
				else
				{
					myValueTaskSource.SetException(new ProtocolException($"Unexpected reply type: {message.MessageType}."));
				}
			}
		};
		MyValueTaskSource<T> vts = new MyValueTaskSource<T>();
		MessageHandler handler2 = new MessageHandler(handler, valueReader, vts, state);
		await CallMethodAsync(message, handler2).ConfigureAwait(continueOnCapturedContext: false);
		return await new ValueTask<T>(vts, 0).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task CallMethodAsync(MessageBuffer message)
	{
		MyValueTaskSource<object?> vts = new MyValueTaskSource<object>();
		await CallMethodAsync(message, delegate(Exception? exception, Message message, object? state)
		{
			CompleteCallValueTaskSource(exception, message, state);
		}, vts).ConfigureAwait(continueOnCapturedContext: false);
		await new ValueTask(vts, 0).ConfigureAwait(continueOnCapturedContext: false);
	}

	private static void CompleteCallValueTaskSource(Exception? exception, Message message, object? vts)
	{
		MyValueTaskSource<object> myValueTaskSource = (MyValueTaskSource<object>)vts;
		if (exception != null)
		{
			myValueTaskSource.SetException(exception);
			return;
		}
		if (message.MessageType == MessageType.MethodReturn)
		{
			myValueTaskSource.SetResult(null);
			return;
		}
		if (message.MessageType == MessageType.Error)
		{
			myValueTaskSource.SetException(CreateDBusExceptionForErrorMessage(message));
			return;
		}
		myValueTaskSource.SetException(new ProtocolException($"Unexpected reply type: {message.MessageType}."));
	}

	private static DBusException CreateDBusExceptionForErrorMessage(Message message)
	{
		string? obj = message.ErrorNameAsString ?? "<<No ErrorName>>.";
		string errorMessage = obj;
		if (message.SignatureIsSet && message.Signature.Length > 0 && message.Signature[0] == 115)
		{
			errorMessage = message.GetBodyReader().ReadString();
		}
		return new DBusException(obj, errorMessage);
	}

	public ValueTask<IDisposable> AddMatchAsync<T>(SynchronizationContext? synchronizationContext, MatchRule rule, MessageValueReader<T> valueReader, Action<Exception?, T, object?, object?> valueHandler, object? readerState, object? handlerState, bool subscribe)
	{
		MessageHandlerDelegate4 handler2 = delegate(Exception? exception, Message message, object? reader, object? handler, object? rs, object? hs)
		{
			Action<Exception, T, object, object> action = (Action<Exception, T, object, object>)handler;
			if (exception != null)
			{
				action(exception, default(T), rs, hs);
			}
			else
			{
				T arg = ((MessageValueReader<T>)reader)(message, rs);
				action(null, arg, rs, hs);
			}
		};
		return AddMatchAsync(synchronizationContext, rule, new MessageHandler4(handler2, valueReader, valueHandler, readerState, handlerState), subscribe);
	}

	private async ValueTask<IDisposable> AddMatchAsync(SynchronizationContext? synchronizationContext, MatchRule rule, MessageHandler4 handler, bool subscribe)
	{
		MatchRuleData data = rule.Data;
		MessageBuffer addMatchMessage = null;
		MatchMaker matchMaker;
		Observer observer;
		lock (_gate)
		{
			if (_state != ConnectionState.Connected)
			{
				throw new DisconnectedException(DisconnectReason);
			}
			if (!RemoteIsBus)
			{
				subscribe = false;
			}
			string ruleString2 = data.GetRuleString();
			if (!_matchMakers.TryGetValue(ruleString2, out matchMaker))
			{
				matchMaker = new MatchMaker(this, ruleString2, in data);
				_matchMakers.Add(ruleString2, matchMaker);
			}
			observer = new Observer(synchronizationContext, matchMaker, in handler, subscribe);
			matchMaker.Observers.Add(observer);
			if (subscribe && matchMaker.AddMatchTcs == null)
			{
				addMatchMessage = CreateAddMatchMessage(matchMaker.RuleString);
				matchMaker.AddMatchTcs = new MyValueTaskSource<object>();
				MessageHandlerDelegate handler2 = delegate(Exception? exception, Message message, object? state1, object? state2, object? state3)
				{
					MatchMaker matchMaker2 = (MatchMaker)state1;
					if (message.MessageType == MessageType.MethodReturn)
					{
						matchMaker2.HasSubscribed = true;
					}
					CompleteCallValueTaskSource(exception, message, matchMaker2.AddMatchTcs);
				};
				_pendingCalls.Add(addMatchMessage.Serial, new MessageHandler(handler2, matchMaker));
			}
		}
		if (subscribe)
		{
			if (addMatchMessage != null && !(await _messageStream.TrySendMessageAsync(addMatchMessage).ConfigureAwait(continueOnCapturedContext: false)))
			{
				addMatchMessage.ReturnToPool();
			}
			try
			{
				await matchMaker.AddMatchTask.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
				observer.Dispose(null);
				throw;
			}
		}
		return observer;
		MessageBuffer CreateAddMatchMessage(string ruleString)
		{
			using MessageWriter messageWriter = GetMessageWriter();
			messageWriter.WriteMethodCallHeader("org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus", "AddMatch", "s");
			messageWriter.WriteString(ruleString);
			return messageWriter.CreateMessage();
		}
	}

	private async void RemoveObserver(MatchMaker matchMaker, Observer observer)
	{
		string ruleString = matchMaker.RuleString;
		bool flag = false;
		lock (_gate)
		{
			if (_state == ConnectionState.Disconnected)
			{
				return;
			}
			if (_matchMakers.TryGetValue(ruleString, out MatchMaker _))
			{
				matchMaker.Observers.Remove(observer);
				flag = matchMaker.AddMatchTcs != null && matchMaker.HasSubscribers;
				if (flag)
				{
					_matchMakers.Remove(ruleString);
				}
			}
		}
		if (flag)
		{
			MessageBuffer message = CreateRemoveMatchMessage();
			if (!(await _messageStream.TrySendMessageAsync(message).ConfigureAwait(continueOnCapturedContext: false)))
			{
				message.ReturnToPool();
			}
		}
		MessageBuffer CreateRemoveMatchMessage()
		{
			using MessageWriter messageWriter = GetMessageWriter();
			messageWriter.WriteMethodCallHeader("org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus", "RemoveMatch", "s", MessageFlags.NoReplyExpected);
			messageWriter.WriteString(ruleString);
			return messageWriter.CreateMessage();
		}
	}

	public MessageWriter GetMessageWriter()
	{
		return _parentConnection.GetMessageWriter();
	}

	public async void SendMessage(MessageBuffer message)
	{
		if (!(await _messageStream.TrySendMessageAsync(message).ConfigureAwait(continueOnCapturedContext: false)))
		{
			message.ReturnToPool();
		}
	}

	public Task<Exception?> DisconnectedAsync()
	{
		lock (_gate)
		{
			if (_disconnectedTcs == null)
			{
				if (_state == ConnectionState.Disconnected)
				{
					return Task.FromResult(GetWaitForDisconnectException());
				}
				_disconnectedTcs = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
			}
			return _disconnectedTcs.Task;
		}
	}

	private Exception? GetWaitForDisconnectException()
	{
		if (!(_disconnectReason is ObjectDisposedException))
		{
			return _disconnectReason;
		}
		return null;
	}

	private void SendErrorReplyMessage(Message methodCall, string errorName, string errorMsg)
	{
		SendMessage(CreateErrorMessage(methodCall, errorName, errorMsg));
		MessageBuffer CreateErrorMessage(Message methodCall, string errorName, string errorMsg)
		{
			using MessageWriter messageWriter = GetMessageWriter();
			messageWriter.WriteError(methodCall.Serial, methodCall.Sender, errorName, errorMsg);
			return messageWriter.CreateMessage();
		}
	}
}
