using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Tmds.DBus.Protocol;

internal class MessageStream : IMessageStream
{
	private struct AuthenticationResult
	{
		public bool IsAuthenticated;

		public bool SupportsFdPassing;

		public Guid Guid;
	}

	private static readonly ReadOnlyMemory<byte> OneByteArray = new byte[1];

	private readonly Socket _socket;

	private readonly UnixFdCollection? _fdCollection;

	private bool _supportsFdPassing;

	private readonly MessagePool _messagePool;

	private readonly ChannelReader<MessageBuffer> _messageReader;

	private readonly ChannelWriter<MessageBuffer> _messageWriter;

	private readonly PipeWriter _pipeWriter;

	private readonly PipeReader _pipeReader;

	private Exception? _completionException;

	public MessageStream(Socket socket)
	{
		_socket = socket;
		Channel<MessageBuffer> channel = Channel.CreateUnbounded<MessageBuffer>(new UnboundedChannelOptions
		{
			AllowSynchronousContinuations = true,
			SingleReader = true,
			SingleWriter = false
		});
		_messageReader = channel.Reader;
		_messageWriter = channel.Writer;
		Pipe pipe = new Pipe(new PipeOptions(null, null, null, -1L, -1L, -1, useSynchronizationContext: false));
		_pipeReader = pipe.Reader;
		_pipeWriter = pipe.Writer;
		if (_supportsFdPassing)
		{
			_fdCollection = new UnixFdCollection();
		}
		_messagePool = new MessagePool();
	}

	private async void ReadFromSocketIntoPipe()
	{
		PipeWriter writer = _pipeWriter;
		Exception exception;
		try
		{
			while (true)
			{
				Memory<byte> memory = writer.GetMemory(1024);
				int num = await _socket.ReceiveAsync(memory, _fdCollection).ConfigureAwait(continueOnCapturedContext: false);
				if (num == 0)
				{
					break;
				}
				writer.Advance(num);
				await writer.FlushAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			throw new IOException("Connection closed by peer");
		}
		catch (Exception ex)
		{
			exception = ex;
		}
		writer.Complete(exception);
	}

	private async void ReadMessagesIntoSocket()
	{
		while (await _messageReader.WaitToReadAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			MessageBuffer message = await _messageReader.ReadAsync().ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				IReadOnlyList<SafeHandle> handles = (_supportsFdPassing ? message.Handles : null);
				ReadOnlySequence<byte> buffer = message.AsReadOnlySequence();
				if (buffer.IsSingleSegment)
				{
					await _socket.SendAsync(buffer.First, handles).ConfigureAwait(continueOnCapturedContext: false);
					continue;
				}
				SequencePosition position = buffer.Start;
				ReadOnlyMemory<byte> memory;
				while (buffer.TryGet(ref position, out memory))
				{
					await _socket.SendAsync(memory, handles).ConfigureAwait(continueOnCapturedContext: false);
					handles = null;
				}
			}
			catch (Exception closeReason)
			{
				Close(closeReason);
				break;
			}
			finally
			{
				message.ReturnToPool();
			}
		}
	}

	public async void ReceiveMessages<T>(IMessageStream.MessageReceivedHandler<T> handler, T state)
	{
		PipeReader reader = _pipeReader;
		try
		{
			while (true)
			{
				ReadOnlySequence<byte> buffer2 = (await reader.ReadAsync().ConfigureAwait(continueOnCapturedContext: false)).Buffer;
				ReadMessages<T>(ref buffer2, _fdCollection, _messagePool, handler, state);
				reader.AdvanceTo(buffer2.Start, buffer2.End);
			}
		}
		catch (Exception closeReason)
		{
			Exception exception2 = CloseCore(closeReason);
			OnException(exception2, handler, state);
		}
		finally
		{
			_fdCollection?.Dispose();
		}
		static void OnException(Exception exception, IMessageStream.MessageReceivedHandler<T> handler, T state)
		{
			handler(exception, null, state);
		}
		static void ReadMessages<TState>(ref ReadOnlySequence<byte> buffer, UnixFdCollection? fdCollection, MessagePool messagePool, IMessageStream.MessageReceivedHandler<TState> handler, TState state)
		{
			Message message;
			while ((message = Message.TryReadMessage(messagePool, ref buffer, fdCollection)) != null)
			{
				handler(null, message, state);
			}
		}
	}

	public async ValueTask DoClientAuthAsync(Guid guid, string? userId, bool supportsFdPassing)
	{
		ReadFromSocketIntoPipe();
		await _socket.SendAsync(OneByteArray, SocketFlags.None, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
		AuthenticationResult authenticationResult = await SendAuthCommandsAsync(userId, supportsFdPassing).ConfigureAwait(continueOnCapturedContext: false);
		_supportsFdPassing = authenticationResult.SupportsFdPassing;
		if (guid != Guid.Empty && guid != authenticationResult.Guid)
		{
			throw new ConnectException("Authentication failure: Unexpected GUID");
		}
		ReadMessagesIntoSocket();
	}

	private async ValueTask<AuthenticationResult> SendAuthCommandsAsync(string? userId, bool supportsFdPassing)
	{
		AuthenticationResult result;
		if (userId != null)
		{
			string command = CreateAuthExternalCommand(userId);
			result = await SendAuthCommandAsync(command, supportsFdPassing).ConfigureAwait(continueOnCapturedContext: false);
			if (result.IsAuthenticated)
			{
				return result;
			}
		}
		result = await SendAuthCommandAsync("AUTH ANONYMOUS\r\n", supportsFdPassing).ConfigureAwait(continueOnCapturedContext: false);
		if (result.IsAuthenticated)
		{
			return result;
		}
		throw new ConnectException("Authentication failure");
	}

	private static string CreateAuthExternalCommand(string userId)
	{
		return string.Create("AUTH EXTERNAL ".Length + userId.Length * 2 + 2, userId, delegate(Span<char> span, string userId)
		{
			"AUTH EXTERNAL ".AsSpan().CopyTo(span);
			span = span.Slice("AUTH EXTERNAL ".Length);
			for (int i = 0; i < userId.Length; i++)
			{
				byte b = (byte)userId[i];
				span[i * 2] = "0123456789abcdef"[b >> 4];
				span[i * 2 + 1] = "0123456789abcdef"[b & 0xF];
			}
			span = span.Slice(userId.Length * 2);
			span[0] = '\r';
			span[1] = '\n';
		});
	}

	private async ValueTask<AuthenticationResult> SendAuthCommandAsync(string command, bool supportsFdPassing)
	{
		byte[] lineBuffer = ArrayPool<byte>.Shared.Rent(512);
		try
		{
			AuthenticationResult result = default(AuthenticationResult);
			await WriteAsync(command, lineBuffer).ConfigureAwait(continueOnCapturedContext: false);
			int length2 = await ReadLineAsync(lineBuffer).ConfigureAwait(continueOnCapturedContext: false);
			if (StartsWithAscii(lineBuffer, length2, "OK"))
			{
				result.IsAuthenticated = true;
				result.Guid = ParseGuid(lineBuffer, length2);
				if (supportsFdPassing)
				{
					await WriteAsync("NEGOTIATE_UNIX_FD\r\n", lineBuffer).ConfigureAwait(continueOnCapturedContext: false);
					result.SupportsFdPassing = StartsWithAscii(lineBuffer, await ReadLineAsync(lineBuffer).ConfigureAwait(continueOnCapturedContext: false), "AGREE_UNIX_FD");
				}
				await WriteAsync("BEGIN\r\n", lineBuffer).ConfigureAwait(continueOnCapturedContext: false);
				return result;
			}
			if (StartsWithAscii(lineBuffer, length2, "REJECTED"))
			{
				return result;
			}
			await WriteAsync("ERROR\r\n", lineBuffer).ConfigureAwait(continueOnCapturedContext: false);
			return result;
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(lineBuffer);
		}
		static Guid ParseGuid(byte[] line, int length)
		{
			Span<byte> span = new Span<byte>(line, 0, length);
			int num = span.IndexOf<byte>(32);
			if (num == -1)
			{
				return Guid.Empty;
			}
			span = span.Slice(num + 1);
			num = span.IndexOf<byte>(32);
			if (num != -1)
			{
				span = span.Slice(0, num);
			}
			Span<char> span2 = stackalloc char[span.Length];
			for (int j = 0; j < span.Length; j++)
			{
				span2[j] = (char)span[j];
			}
			return Guid.ParseExact(span2, "N");
		}
		static bool StartsWithAscii(byte[] line, int length, string expected)
		{
			if (length < expected.Length)
			{
				return false;
			}
			for (int i = 0; i < expected.Length; i++)
			{
				if (line[i] != expected[i])
				{
					return false;
				}
			}
			return true;
		}
	}

	private async ValueTask WriteAsync(string message, Memory<byte> lineBuffer)
	{
		lineBuffer = lineBuffer[..Encoding.ASCII.GetBytes(message.AsSpan(), lineBuffer.Span)];
		await _socket.SendAsync(lineBuffer, SocketFlags.None, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
	}

	private async ValueTask<int> ReadLineAsync(Memory<byte> lineBuffer)
	{
		PipeReader reader = _pipeReader;
		ReadOnlySequence<byte> source;
		SequencePosition? sequencePosition;
		while (true)
		{
			source = (await reader.ReadAsync().ConfigureAwait(continueOnCapturedContext: false)).Buffer;
			sequencePosition = source.PositionOf<byte>(10);
			if (sequencePosition.HasValue)
			{
				break;
			}
			reader.AdvanceTo(source.Start, source.End);
		}
		int result = CopyBuffer(source.Slice(0, sequencePosition.Value), lineBuffer);
		reader.AdvanceTo(source.GetPosition(1L, sequencePosition.Value));
		return result;
		static int CopyBuffer(ReadOnlySequence<byte> src, Memory<byte> dst)
		{
			Span<byte> span = dst.Span;
			src.CopyTo(span);
			span = span.Slice(0, (int)src.Length);
			if (!span.EndsWith("\r"u8))
			{
				throw new ProtocolException("Authentication messages from server must end with '\\r\\n'.");
			}
			if (span.Length == 1)
			{
				throw new ProtocolException("Received empty authentication message from server.");
			}
			return span.Length - 1;
		}
	}

	public async ValueTask<bool> TrySendMessageAsync(MessageBuffer message)
	{
		while (await _messageWriter.WaitToWriteAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			if (_messageWriter.TryWrite(message))
			{
				return true;
			}
		}
		return false;
	}

	public void Close(Exception closeReason)
	{
		CloseCore(closeReason);
	}

	private Exception CloseCore(Exception closeReason)
	{
		Exception ex = Interlocked.CompareExchange(ref _completionException, closeReason, null);
		if (ex == null)
		{
			_socket?.Dispose();
			_messageWriter.Complete();
		}
		if (ex == null)
		{
			ex = closeReason;
		}
		return ex;
	}
}
