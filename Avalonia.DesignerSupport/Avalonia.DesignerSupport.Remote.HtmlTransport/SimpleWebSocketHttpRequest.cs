using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.DesignerSupport.Remote.HtmlTransport;

public class SimpleWebSocketHttpRequest : IDisposable
{
	private NetworkStream _stream;

	private string _websocketKey;

	public Dictionary<string, string> Headers { get; }

	public string Path { get; }

	public bool IsWebsocketRequest { get; }

	public IReadOnlyList<string> WebSocketProtocols { get; }

	public SimpleWebSocketHttpRequest(NetworkStream stream, string path, Dictionary<string, string> headers)
	{
		Path = path;
		Headers = headers;
		_stream = stream;
		if (!headers.TryGetValue("Connection", out var value) || !value.Contains("Upgrade") || !headers.TryGetValue("Upgrade", out value) || !(value == "websocket") || !headers.TryGetValue("Sec-WebSocket-Key", out _websocketKey))
		{
			return;
		}
		IsWebsocketRequest = true;
		if (headers.TryGetValue("Sec-WebSocket-Protocol", out value))
		{
			WebSocketProtocols = (from x in value.Split(',')
				select x.Trim()).ToArray();
		}
		else
		{
			WebSocketProtocols = Array.Empty<string>();
		}
	}

	public async Task RespondAsync(int code, byte[] data, string contentType)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(FormattableString.Invariant($"HTTP/1.1 {code} {(HttpStatusCode)code}\r\nConnection: close\r\nContent-Type: {contentType}\r\nContent-Length: {data.Length}\r\n\r\n"));
		await _stream.WriteAsync(bytes, 0, bytes.Length);
		await _stream.WriteAsync(data, 0, data.Length);
		_stream.Dispose();
		_stream = null;
	}

	public async Task<SimpleWebSocket> AcceptWebSocket(string protocol = null)
	{
		string s = _websocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
		string text;
		using (SHA1 sHA = SHA1.Create())
		{
			text = Convert.ToBase64String(sHA.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}
		string text2 = "HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Accept: " + text + "\r\n";
		if (protocol != null)
		{
			text2 = text2 + protocol + "\r\n";
		}
		text2 += "\r\n";
		byte[] bytes = Encoding.UTF8.GetBytes(text2);
		await _stream.WriteAsync(bytes, 0, bytes.Length);
		NetworkStream stream = _stream;
		_stream = null;
		return new SimpleWebSocket(stream);
	}

	public void Dispose()
	{
		_stream?.Dispose();
	}
}
