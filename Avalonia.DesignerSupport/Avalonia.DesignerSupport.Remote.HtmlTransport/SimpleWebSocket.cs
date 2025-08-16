using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.DesignerSupport.Remote.HtmlTransport;

public class SimpleWebSocket : IDisposable
{
	private class AsyncLock
	{
		private class Lock : IDisposable
		{
			private AsyncLock _parent;

			private object _syncRoot = new object();

			public Lock(AsyncLock parent)
			{
				_parent = parent;
			}

			public void Dispose()
			{
				lock (_syncRoot)
				{
					if (_parent != null)
					{
						AsyncLock parent = _parent;
						_parent = null;
						parent.Unlock();
					}
				}
			}
		}

		private object _syncRoot = new object();

		private Queue<TaskCompletionSource<IDisposable>> _queue = new Queue<TaskCompletionSource<IDisposable>>();

		private bool _locked;

		public Task<IDisposable> LockAsync()
		{
			lock (_syncRoot)
			{
				if (!_locked)
				{
					_locked = true;
					return Task.FromResult((IDisposable)new Lock(this));
				}
				TaskCompletionSource<IDisposable> taskCompletionSource = new TaskCompletionSource<IDisposable>();
				_queue.Enqueue(taskCompletionSource);
				return taskCompletionSource.Task;
			}
		}

		private void Unlock()
		{
			lock (_syncRoot)
			{
				if (_queue.Count != 0)
				{
					_queue.Dequeue().SetResult(new Lock(this));
				}
				else
				{
					_locked = false;
				}
			}
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct WebSocketHeader
	{
		[FieldOffset(0)]
		public byte Mask;

		[FieldOffset(1)]
		public byte Length8;

		[FieldOffset(2)]
		public ushort Length16;

		[FieldOffset(2)]
		public ulong Length64;
	}

	private enum FrameType
	{
		Continue = 0,
		Text = 1,
		Binary = 2,
		Close = 8,
		Ping = 9,
		Pong = 10
	}

	private struct Frame
	{
		public byte[] Data;

		public bool EndOfMessage;

		public FrameType FrameType;
	}

	private Stream _stream;

	private AsyncLock _sendLock = new AsyncLock();

	private AsyncLock _recvLock = new AsyncLock();

	private const int WebsocketInitialHeaderLength = 2;

	private const int WebsocketLen16Length = 4;

	private const int WebsocketLen64Length = 10;

	private const int WebsocketLen16Code = 126;

	private const int WebsocketLen64Code = 127;

	private readonly byte[] _sendHeaderBuffer = new byte[10];

	private readonly MemoryStream _receiveFrameStream = new MemoryStream();

	private readonly MemoryStream _receiveMessageStream = new MemoryStream();

	private FrameType _currentMessageFrameType;

	private byte[] _recvHeaderBuffer = new byte[8];

	private byte[] _maskBuffer = new byte[4];

	private byte[] _readExactBuffer = new byte[4096];

	internal SimpleWebSocket(Stream stream)
	{
		_stream = stream;
	}

	public void Dispose()
	{
		_stream?.Dispose();
		_stream = null;
	}

	public Task SendMessage(string text)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		return SendMessage(isText: true, bytes);
	}

	public Task SendMessage(bool isText, byte[] data)
	{
		return SendMessage(isText, data, 0, data.Length);
	}

	public Task SendMessage(bool isText, byte[] data, int offset, int length)
	{
		return SendFrame(isText ? FrameType.Text : FrameType.Binary, data, offset, length);
	}

	private unsafe async Task SendFrame(FrameType type, byte[] data, int offset, int length)
	{
		using (await _sendLock.LockAsync())
		{
			WebSocketHeader webSocketHeader = default(WebSocketHeader);
			int num;
			if (data.Length <= 125)
			{
				num = 2;
				webSocketHeader.Length8 = (byte)length;
			}
			else if (length <= 65535)
			{
				num = 4;
				webSocketHeader.Length8 = 126;
				webSocketHeader.Length16 = (ushort)IPAddress.HostToNetworkOrder((short)(ushort)length);
			}
			else
			{
				num = 10;
				webSocketHeader.Length8 = 127;
				webSocketHeader.Length64 = (ulong)IPAddress.HostToNetworkOrder((long)length);
			}
			if (true)
			{
			}
			webSocketHeader.Mask = (byte)(0x80u | ((byte)type & 0xF));
			Marshal.Copy(new IntPtr(&webSocketHeader), _sendHeaderBuffer, 0, num);
			await _stream.WriteAsync(_sendHeaderBuffer, 0, num);
			await _stream.WriteAsync(data, offset, length);
		}
	}

	private async Task<Frame> ReadFrame()
	{
		_receiveFrameStream.Position = 0L;
		_receiveFrameStream.SetLength(0L);
		await ReadExact(_stream, _recvHeaderBuffer, 0, 2);
		bool masked = (_recvHeaderBuffer[1] & 0x80) != 0;
		int num = _recvHeaderBuffer[1] & 0x7F;
		bool endOfMessage = (_recvHeaderBuffer[0] & 0x80) != 0;
		FrameType frameType = (FrameType)(_recvHeaderBuffer[0] & 0xF);
		int length;
		if (num <= 125)
		{
			length = num;
		}
		else if (num == 126)
		{
			await ReadExact(_stream, _recvHeaderBuffer, 0, 2);
			length = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_recvHeaderBuffer, 0));
		}
		else
		{
			await ReadExact(_stream, _recvHeaderBuffer, 0, 8);
			length = (int)IPAddress.NetworkToHostOrder((long)BitConverter.ToUInt64(_recvHeaderBuffer, 0));
		}
		if (masked)
		{
			await ReadExact(_stream, _maskBuffer, 0, 4);
		}
		await ReadExact(_stream, _receiveFrameStream, length);
		byte[] array = _receiveFrameStream.ToArray();
		if (masked)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] ^= _maskBuffer[i % 4];
			}
		}
		Frame result = default(Frame);
		result.Data = array;
		result.EndOfMessage = endOfMessage;
		result.FrameType = frameType;
		return result;
	}

	public async Task<SimpleWebSocketMessage> ReceiveMessage()
	{
		using (await _recvLock.LockAsync())
		{
			Frame frame;
			while (true)
			{
				frame = await ReadFrame();
				if (frame.FrameType == FrameType.Close)
				{
					return null;
				}
				if (frame.FrameType == FrameType.Ping)
				{
					await SendFrame(FrameType.Pong, frame.Data, 0, frame.Data.Length);
				}
				if (frame.FrameType == FrameType.Text || frame.FrameType == FrameType.Binary)
				{
					bool isText = frame.FrameType == FrameType.Text;
					if (_receiveMessageStream.Length == 0L && frame.EndOfMessage)
					{
						return new SimpleWebSocketMessage
						{
							IsText = isText,
							Data = frame.Data
						};
					}
					_receiveMessageStream.Write(frame.Data, 0, frame.Data.Length);
					_currentMessageFrameType = frame.FrameType;
				}
				if (frame.FrameType == FrameType.Continue)
				{
					frame.FrameType = _currentMessageFrameType;
					_receiveMessageStream.Write(frame.Data, 0, frame.Data.Length);
					if (frame.EndOfMessage)
					{
						break;
					}
				}
			}
			bool isText2 = frame.FrameType == FrameType.Text;
			byte[] data = _receiveMessageStream.ToArray();
			_receiveMessageStream.Position = 0L;
			_receiveMessageStream.SetLength(0L);
			return new SimpleWebSocketMessage
			{
				IsText = isText2,
				Data = data
			};
		}
	}

	private async Task ReadExact(Stream from, MemoryStream to, int length)
	{
		while (length > 0)
		{
			int count = Math.Min(length, _readExactBuffer.Length);
			int num = await from.ReadAsync(_readExactBuffer, 0, count);
			to.Write(_readExactBuffer, 0, num);
			if (num <= 0)
			{
				throw new EndOfStreamException();
			}
			length -= num;
		}
	}

	private async Task ReadExact(Stream from, byte[] to, int offset, int length)
	{
		while (length > 0)
		{
			int num = await from.ReadAsync(to, offset, length);
			if (num <= 0)
			{
				throw new EndOfStreamException();
			}
			length -= num;
			offset += num;
		}
	}
}
