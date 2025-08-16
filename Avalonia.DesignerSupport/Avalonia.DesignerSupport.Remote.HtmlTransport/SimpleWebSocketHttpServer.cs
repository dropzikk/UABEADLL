using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.DesignerSupport.Remote.HtmlTransport;

public class SimpleWebSocketHttpServer : IDisposable
{
	private readonly IPAddress _address;

	private readonly int _port;

	private TcpListener _listener;

	public async Task<SimpleWebSocketHttpRequest> AcceptAsync()
	{
		SimpleWebSocketHttpRequest simpleWebSocketHttpRequest;
		do
		{
			simpleWebSocketHttpRequest = await AcceptAsyncImpl();
		}
		while (simpleWebSocketHttpRequest == null);
		return simpleWebSocketHttpRequest;
	}

	private async Task<SimpleWebSocketHttpRequest> AcceptAsyncImpl()
	{
		if (_listener == null)
		{
			throw new InvalidOperationException("Currently not listening");
		}
		NetworkStream stream = new NetworkStream(await _listener.AcceptSocketAsync());
		bool error = true;
		Dictionary<string, string> headers = new Dictionary<string, string>();
		try
		{
			string[] array = (await ReadLineAsync()).Split(' ');
			if (array.Length != 3 || !array[2].StartsWith("HTTP") || array[0] != "GET")
			{
				return null;
			}
			string path = array[1];
			while (true)
			{
				string text = await ReadLineAsync();
				if (string.IsNullOrEmpty(text))
				{
					break;
				}
				array = text.Split(new char[1] { ':' }, 2);
				headers[array[0]] = array[1].TrimStart();
			}
			error = false;
			return new SimpleWebSocketHttpRequest(stream, path, headers);
		}
		catch
		{
			error = true;
			return null;
		}
		finally
		{
			if (error)
			{
				stream.Dispose();
			}
		}
		async Task<string> ReadLineAsync()
		{
			byte[] readBuffer = new byte[1];
			byte[] lineBuffer = new byte[1024];
			for (int c = 0; c < 1024; c++)
			{
				if (await stream.ReadAsync(readBuffer, 0, 1) == 0)
				{
					throw new EndOfStreamException();
				}
				if (readBuffer[0] == 10)
				{
					if (c == 0)
					{
						return "";
					}
					if (lineBuffer[c - 1] == 13)
					{
						c--;
					}
					if (c == 0)
					{
						return "";
					}
					return Encoding.UTF8.GetString(lineBuffer, 0, c);
				}
				lineBuffer[c] = readBuffer[0];
			}
			throw new InvalidDataException("Header is too large");
		}
	}

	public void Listen()
	{
		TcpListener tcpListener = new TcpListener(_address, _port);
		tcpListener.Start();
		_listener = tcpListener;
	}

	public SimpleWebSocketHttpServer(IPAddress address, int port)
	{
		_address = address;
		_port = port;
	}

	public void Dispose()
	{
		_listener?.Stop();
		_listener = null;
	}
}
