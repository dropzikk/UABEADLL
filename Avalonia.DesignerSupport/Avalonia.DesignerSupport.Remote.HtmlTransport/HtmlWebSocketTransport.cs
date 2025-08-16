using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Remote.Protocol;
using Avalonia.Remote.Protocol.Input;
using Avalonia.Remote.Protocol.Viewport;
using Avalonia.Utilities;

namespace Avalonia.DesignerSupport.Remote.HtmlTransport;

public class HtmlWebSocketTransport : IAvaloniaRemoteTransportConnection, IDisposable
{
	private readonly IAvaloniaRemoteTransportConnection _signalTransport;

	private readonly SimpleWebSocketHttpServer _simpleServer;

	private readonly Dictionary<string, byte[]> _resources;

	private SimpleWebSocket _pendingSocket;

	private bool _disposed;

	private object _lock = new object();

	private AutoResetEvent _wakeup = new AutoResetEvent(initialState: false);

	private FrameMessage _lastFrameMessage;

	private FrameMessage _lastSentFrameMessage;

	private Action<IAvaloniaRemoteTransportConnection, object> _onMessage;

	private Action<IAvaloniaRemoteTransportConnection, Exception> _onException;

	private static readonly Dictionary<string, string> Mime = new Dictionary<string, string>
	{
		["html"] = "text/html",
		["htm"] = "text/html",
		["js"] = "text/javascript",
		["css"] = "text/css"
	};

	private static readonly byte[] NotFound = Encoding.UTF8.GetBytes("404 - Not Found");

	public event Action<IAvaloniaRemoteTransportConnection, object> OnMessage
	{
		add
		{
			bool flag;
			lock (_lock)
			{
				flag = _onMessage == null;
				_onMessage = (Action<IAvaloniaRemoteTransportConnection, object>)Delegate.Combine(_onMessage, value);
			}
			if (flag)
			{
				_signalTransport.OnMessage += OnSignalTransportMessage;
			}
		}
		remove
		{
			lock (_lock)
			{
				_onMessage = (Action<IAvaloniaRemoteTransportConnection, object>)Delegate.Remove(_onMessage, value);
				if (_onMessage == null)
				{
					_signalTransport.OnMessage -= OnSignalTransportMessage;
				}
			}
		}
	}

	public event Action<IAvaloniaRemoteTransportConnection, Exception> OnException
	{
		add
		{
			lock (_lock)
			{
				bool num = _onException == null;
				_onException = (Action<IAvaloniaRemoteTransportConnection, Exception>)Delegate.Combine(_onException, value);
				if (num)
				{
					_signalTransport.OnException += OnSignalTransportException;
				}
			}
		}
		remove
		{
			lock (_lock)
			{
				_onException = (Action<IAvaloniaRemoteTransportConnection, Exception>)Delegate.Remove(_onException, value);
				if (_onException == null)
				{
					_signalTransport.OnException -= OnSignalTransportException;
				}
			}
		}
	}

	public HtmlWebSocketTransport(IAvaloniaRemoteTransportConnection signalTransport, Uri listenUri)
	{
		if (listenUri.Scheme != "http")
		{
			throw new ArgumentException("URI scheme is not HTTP.", "listenUri");
		}
		string resourcePrefix = "Avalonia.DesignerSupport.Remote.HtmlTransport.webapp.build.";
		_resources = (from r in typeof(HtmlWebSocketTransport).Assembly.GetManifestResourceNames()
			where r.StartsWith(resourcePrefix, StringComparison.OrdinalIgnoreCase) && r.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
			select r).ToDictionary((string r) => r.Substring(resourcePrefix.Length).Substring(0, r.Length - resourcePrefix.Length - 3), delegate(string r)
		{
			using GZipStream gZipStream = new GZipStream(typeof(HtmlWebSocketTransport).Assembly.GetManifestResourceStream(r), CompressionMode.Decompress);
			MemoryStream memoryStream = new MemoryStream();
			gZipStream.CopyTo(memoryStream);
			return memoryStream.ToArray();
		});
		_signalTransport = signalTransport;
		IPAddress iPAddress = IPAddress.Parse(listenUri.Host);
		_simpleServer = new SimpleWebSocketHttpServer(iPAddress, listenUri.Port);
		_simpleServer.Listen();
		Task.Run((Action)AcceptWorker);
		Task.Run((Action)SocketWorker);
		_signalTransport.Send(new HtmlTransportStartedMessage
		{
			Uri = "http://" + iPAddress?.ToString() + ":" + listenUri.Port + "/"
		});
	}

	private async void AcceptWorker()
	{
		while (true)
		{
			using SimpleWebSocketHttpRequest req = await _simpleServer.AcceptAsync();
			if (!req.IsWebsocketRequest)
			{
				string text = ((req.Path == "/") ? "index.html" : req.Path.TrimStart('/').Replace('/', '.'));
				if (!_resources.TryGetValue(text, out var value))
				{
					await req.RespondAsync(404, NotFound, "text/plain");
					continue;
				}
				string text2 = Path.GetExtension(text).Substring(1);
				string value2 = null;
				if (text2 == null || !Mime.TryGetValue(text2, out value2))
				{
					value2 = "application/octet-stream";
				}
				await req.RespondAsync(200, value, value2);
			}
			else
			{
				SimpleWebSocket simpleWebSocket = await req.AcceptWebSocket();
				SocketReceiveWorker(simpleWebSocket);
				lock (_lock)
				{
					_pendingSocket?.Dispose();
					_pendingSocket = simpleWebSocket;
				}
			}
		}
	}

	private async void SocketReceiveWorker(SimpleWebSocket socket)
	{
		try
		{
			while (true)
			{
				SimpleWebSocketMessage simpleWebSocketMessage = await socket.ReceiveMessage().ConfigureAwait(continueOnCapturedContext: false);
				if (simpleWebSocketMessage != null && simpleWebSocketMessage.IsText)
				{
					object obj = ParseMessage(simpleWebSocketMessage.AsString());
					if (obj != null)
					{
						_onMessage?.Invoke(this, obj);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex.ToString());
		}
	}

	private async void SocketWorker()
	{
		_ = 1;
		try
		{
			SimpleWebSocket socket = null;
			while (!_disposed)
			{
				FrameMessage sendNow = null;
				lock (_lock)
				{
					if (_pendingSocket != null)
					{
						socket?.Dispose();
						socket = _pendingSocket;
						_pendingSocket = null;
						_lastSentFrameMessage = null;
					}
					if (_lastFrameMessage != _lastSentFrameMessage)
					{
						HtmlWebSocketTransport htmlWebSocketTransport = this;
						FrameMessage lastFrameMessage;
						sendNow = (lastFrameMessage = _lastFrameMessage);
						htmlWebSocketTransport._lastSentFrameMessage = lastFrameMessage;
					}
				}
				if (sendNow != null && socket != null)
				{
					await socket.SendMessage(FormattableString.Invariant($"frame:{sendNow.SequenceId}:{sendNow.Width}:{sendNow.Height}:{sendNow.Stride}:{sendNow.DpiX}:{sendNow.DpiY}"));
					await socket.SendMessage(isText: false, sendNow.Data);
				}
				_wakeup.WaitOne(TimeSpan.FromSeconds(1.0));
			}
			socket?.Dispose();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex.ToString());
		}
	}

	public void Dispose()
	{
		_disposed = true;
		_pendingSocket?.Dispose();
		_simpleServer.Dispose();
	}

	public Task Send(object data)
	{
		if (data is FrameMessage lastFrameMessage)
		{
			_lastFrameMessage = lastFrameMessage;
			_wakeup.Set();
			return Task.CompletedTask;
		}
		if (data is RequestViewportResizeMessage)
		{
			return Task.CompletedTask;
		}
		return _signalTransport.Send(data);
	}

	public void Start()
	{
		_onMessage?.Invoke(this, new ClientSupportedPixelFormatsMessage
		{
			Formats = new PixelFormat[1] { PixelFormat.Rgba8888 }
		});
		_signalTransport.Start();
	}

	private void OnSignalTransportMessage(IAvaloniaRemoteTransportConnection signal, object message)
	{
		_onMessage?.Invoke(this, message);
	}

	private void OnSignalTransportException(IAvaloniaRemoteTransportConnection arg1, Exception ex)
	{
		_onException?.Invoke(this, ex);
	}

	private static object ParseMessage(string message)
	{
		string[] array = message.Split(':');
		string text = array[0];
		if (text.Equals("frame-received", StringComparison.OrdinalIgnoreCase))
		{
			return new FrameReceivedMessage
			{
				SequenceId = long.Parse(array[1])
			};
		}
		if (text.Equals("pointer-released", StringComparison.OrdinalIgnoreCase))
		{
			return new PointerReleasedEventMessage
			{
				Modifiers = ParseInputModifiers(array[1]),
				X = ParseDouble(array[2]),
				Y = ParseDouble(array[3]),
				Button = ParseMouseButton(array[4])
			};
		}
		if (text.Equals("pointer-pressed", StringComparison.OrdinalIgnoreCase))
		{
			return new PointerPressedEventMessage
			{
				Modifiers = ParseInputModifiers(array[1]),
				X = ParseDouble(array[2]),
				Y = ParseDouble(array[3]),
				Button = ParseMouseButton(array[4])
			};
		}
		if (text.Equals("pointer-moved", StringComparison.OrdinalIgnoreCase))
		{
			return new PointerMovedEventMessage
			{
				Modifiers = ParseInputModifiers(array[1]),
				X = ParseDouble(array[2]),
				Y = ParseDouble(array[3])
			};
		}
		if (text.Equals("scroll", StringComparison.OrdinalIgnoreCase))
		{
			return new ScrollEventMessage
			{
				Modifiers = ParseInputModifiers(array[1]),
				X = ParseDouble(array[2]),
				Y = ParseDouble(array[3]),
				DeltaX = ParseDouble(array[4]),
				DeltaY = ParseDouble(array[5])
			};
		}
		return null;
	}

	private static InputModifiers[] ParseInputModifiers(string modifiersText)
	{
		if (!string.IsNullOrWhiteSpace(modifiersText))
		{
			return (from x in modifiersText.Split(',')
				select EnumHelper.Parse<InputModifiers>(x, ignoreCase: true)).ToArray();
		}
		return null;
	}

	private static MouseButton ParseMouseButton(string buttonText)
	{
		if (!string.IsNullOrWhiteSpace(buttonText))
		{
			return EnumHelper.Parse<MouseButton>(buttonText, ignoreCase: true);
		}
		return MouseButton.None;
	}

	private static double ParseDouble(string text)
	{
		return double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
	}
}
