using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Metsys.Bson;

namespace Avalonia.Remote.Protocol;

[RequiresUnreferencedCode("Bson uses reflection")]
internal class BsonStreamTransportConnection : IAvaloniaRemoteTransportConnection, IDisposable
{
	private readonly IMessageTypeResolver _resolver;

	private readonly Stream _inputStream;

	private readonly Stream _outputStream;

	private readonly Action _disposeCallback;

	private readonly CancellationToken _cancel;

	private readonly CancellationTokenSource _cancelSource;

	private readonly MemoryStream _outputBlock = new MemoryStream();

	private readonly object _lock = new object();

	private bool _writeOperationPending;

	private bool _readingAlreadyStarted;

	private bool _writerIsBroken;

	private static readonly byte[] ZeroLength = new byte[4];

	public event Action<IAvaloniaRemoteTransportConnection, object> OnMessage;

	public event Action<IAvaloniaRemoteTransportConnection, Exception> OnException;

	public BsonStreamTransportConnection(IMessageTypeResolver resolver, Stream inputStream, Stream outputStream, Action disposeCallback)
	{
		_resolver = resolver;
		_inputStream = inputStream;
		_outputStream = outputStream;
		_disposeCallback = disposeCallback;
		_cancelSource = new CancellationTokenSource();
		_cancel = _cancelSource.Token;
	}

	public void Dispose()
	{
		_cancelSource.Cancel();
		_disposeCallback?.Invoke();
	}

	public void StartReading()
	{
		lock (_lock)
		{
			if (_readingAlreadyStarted)
			{
				throw new InvalidOperationException("Reading has already started");
			}
			_readingAlreadyStarted = true;
			Task.Run((Func<Task?>)Reader, _cancel);
		}
	}

	private async Task ReadExact(byte[] buffer)
	{
		int num;
		for (int read = 0; read != buffer.Length; read += num)
		{
			num = await _inputStream.ReadAsync(buffer, read, buffer.Length - read, _cancel).ConfigureAwait(continueOnCapturedContext: false);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
		}
	}

	private async Task Reader()
	{
		_ = 1;
		try
		{
			while (true)
			{
				byte[] infoBlock = new byte[20];
				await ReadExact(infoBlock).ConfigureAwait(continueOnCapturedContext: false);
				int num = BitConverter.ToInt32(infoBlock, 0);
				byte[] array = new byte[16];
				Buffer.BlockCopy(infoBlock, 4, array, 0, 16);
				Guid guid = new Guid(array);
				byte[] buffer = new byte[num];
				await ReadExact(buffer).ConfigureAwait(continueOnCapturedContext: false);
				object arg = Deserializer.Deserialize(new BinaryReader(new MemoryStream(buffer)), _resolver.GetByGuid(guid));
				this.OnMessage?.Invoke(this, arg);
			}
		}
		catch (Exception e)
		{
			FireException(e);
		}
	}

	public async Task Send(object data)
	{
		lock (_lock)
		{
			if (_writerIsBroken)
			{
				return;
			}
			if (_writeOperationPending)
			{
				throw new InvalidOperationException("Previous send operation was not finished");
			}
			_writeOperationPending = true;
		}
		try
		{
			byte[] array = _resolver.GetGuid(data.GetType()).ToByteArray();
			_outputBlock.Seek(0L, SeekOrigin.Begin);
			_outputBlock.SetLength(0L);
			_outputBlock.Write(ZeroLength, 0, 4);
			_outputBlock.Write(array, 0, array.Length);
			byte[] array2 = Serializer.Serialize(data);
			_outputBlock.Write(array2, 0, array2.Length);
			_outputBlock.Seek(0L, SeekOrigin.Begin);
			byte[] bytes = BitConverter.GetBytes((int)_outputBlock.Length - 20);
			_outputBlock.Write(bytes, 0, bytes.Length);
			_outputBlock.Seek(0L, SeekOrigin.Begin);
			try
			{
				await _outputBlock.CopyToAsync(_outputStream, 4096, _cancel).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception e)
			{
				lock (_lock)
				{
					_writerIsBroken = true;
				}
				FireException(e);
			}
		}
		finally
		{
			lock (_lock)
			{
				_writeOperationPending = false;
			}
		}
	}

	private void FireException(Exception e)
	{
		if (!((e as OperationCanceledException)?.CancellationToken == _cancel))
		{
			this.OnException?.Invoke(this, e);
		}
	}

	public void Start()
	{
	}
}
