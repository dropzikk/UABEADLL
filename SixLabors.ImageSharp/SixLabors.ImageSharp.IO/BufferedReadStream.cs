using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SixLabors.ImageSharp.IO;

internal sealed class BufferedReadStream : Stream
{
	private readonly CancellationToken cancellationToken;

	private readonly int maxBufferIndex;

	private readonly byte[] readBuffer;

	private MemoryHandle readBufferHandle;

	private unsafe readonly byte* pinnedReadBuffer;

	private int readBufferIndex;

	private long readerPosition;

	private bool isDisposed;

	public int EofHitCount { get; private set; }

	public int BufferSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public override long Length { get; }

	public override long Position
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return readerPosition;
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		set
		{
			Guard.MustBeGreaterThanOrEqualTo(value, 0L, "Position");
			cancellationToken.ThrowIfCancellationRequested();
			if (IsInReadBuffer(value, out var index))
			{
				readBufferIndex = (int)index;
				readerPosition = value;
			}
			else
			{
				BaseStream.Seek(value, SeekOrigin.Begin);
				readerPosition = value;
				readBufferIndex = int.MinValue;
			}
		}
	}

	public override bool CanRead { get; } = true;

	public override bool CanSeek { get; } = true;

	public override bool CanWrite { get; }

	public long RemainingBytes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Length - Position;
		}
	}

	public Stream BaseStream
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get;
	}

	public unsafe BufferedReadStream(Configuration configuration, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		Guard.NotNull(configuration, "configuration");
		Guard.IsTrue(stream.CanRead, "stream", "Stream must be readable.");
		Guard.IsTrue(stream.CanSeek, "stream", "Stream must be seekable.");
		this.cancellationToken = cancellationToken;
		if (stream.CanWrite)
		{
			stream.Flush();
		}
		BaseStream = stream;
		Length = stream.Length;
		readerPosition = stream.Position;
		BufferSize = configuration.StreamProcessingBufferSize;
		maxBufferIndex = BufferSize - 1;
		readBuffer = ArrayPool<byte>.Shared.Rent(BufferSize);
		readBufferHandle = new Memory<byte>(readBuffer).Pin();
		pinnedReadBuffer = (byte*)readBufferHandle.Pointer;
		readBufferIndex = int.MinValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe override int ReadByte()
	{
		if (readerPosition >= Length)
		{
			EofHitCount++;
			return -1;
		}
		if ((uint)readBufferIndex > (uint)maxBufferIndex)
		{
			FillReadBuffer();
		}
		readerPosition++;
		return pinnedReadBuffer[readBufferIndex++];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int Read(byte[] buffer, int offset, int count)
	{
		return Read(buffer.AsSpan(offset, count));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int Read(Span<byte> buffer)
	{
		cancellationToken.ThrowIfCancellationRequested();
		int length = buffer.Length;
		if (length > BufferSize)
		{
			return ReadToBufferDirectSlow(buffer);
		}
		if ((uint)readBufferIndex > (uint)(BufferSize - length))
		{
			return ReadToBufferViaCopySlow(buffer);
		}
		return ReadToBufferViaCopyFast(buffer);
	}

	public override void Flush()
	{
		Stream baseStream = BaseStream;
		if (readerPosition != baseStream.Position)
		{
			baseStream.Seek(readerPosition, SeekOrigin.Begin);
			readerPosition = baseStream.Position;
		}
		readBufferIndex = int.MinValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override long Seek(long offset, SeekOrigin origin)
	{
		switch (origin)
		{
		case SeekOrigin.Begin:
			Position = offset;
			break;
		case SeekOrigin.Current:
			Position += offset;
			break;
		case SeekOrigin.End:
			Position = Length - offset;
			break;
		}
		return readerPosition;
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}

	protected override void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			isDisposed = true;
			readBufferHandle.Dispose();
			ArrayPool<byte>.Shared.Return(readBuffer);
			Flush();
			base.Dispose(disposing: true);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsInReadBuffer(long newPosition, out long index)
	{
		index = newPosition - readerPosition + readBufferIndex;
		if (index > -1)
		{
			return index < BufferSize;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void FillReadBuffer()
	{
		cancellationToken.ThrowIfCancellationRequested();
		Stream baseStream = BaseStream;
		if (readerPosition != baseStream.Position)
		{
			baseStream.Seek(readerPosition, SeekOrigin.Begin);
		}
		int num = 0;
		int num2;
		do
		{
			num2 = baseStream.Read(readBuffer, num, BufferSize - num);
			num += num2;
		}
		while (num < BufferSize && num2 > 0);
		readBufferIndex = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int ReadToBufferViaCopyFast(Span<byte> buffer)
	{
		int copyCount = GetCopyCount(buffer.Length);
		readBuffer.AsSpan(readBufferIndex, copyCount).CopyTo(buffer);
		readerPosition += copyCount;
		readBufferIndex += copyCount;
		CheckEof(copyCount);
		return copyCount;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int ReadToBufferViaCopyFast(byte[] buffer, int offset, int count)
	{
		int copyCount = GetCopyCount(count);
		CopyBytes(buffer, offset, copyCount);
		readerPosition += copyCount;
		readBufferIndex += copyCount;
		return copyCount;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int ReadToBufferViaCopySlow(Span<byte> buffer)
	{
		FillReadBuffer();
		return ReadToBufferViaCopyFast(buffer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int ReadToBufferViaCopySlow(byte[] buffer, int offset, int count)
	{
		FillReadBuffer();
		return ReadToBufferViaCopyFast(buffer, offset, count);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int ReadToBufferDirectSlow(Span<byte> buffer)
	{
		Stream baseStream = BaseStream;
		if (readerPosition != baseStream.Position)
		{
			baseStream.Seek(readerPosition, SeekOrigin.Begin);
		}
		int length = buffer.Length;
		int num = 0;
		int num3;
		do
		{
			int num2 = num;
			num3 = baseStream.Read(buffer.Slice(num2, length - num2));
			num += num3;
		}
		while (num < length && num3 > 0);
		Position += num;
		CheckEof(num);
		return num;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int ReadToBufferDirectSlow(byte[] buffer, int offset, int count)
	{
		Stream baseStream = BaseStream;
		if (readerPosition != baseStream.Position)
		{
			baseStream.Seek(readerPosition, SeekOrigin.Begin);
		}
		int num = 0;
		int num2;
		do
		{
			num2 = baseStream.Read(buffer, num + offset, count - num);
			num += num2;
		}
		while (num < count && num2 > 0);
		Position += num;
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetCopyCount(int count)
	{
		long num = Length - readerPosition;
		if (num > count)
		{
			return count;
		}
		if (num < 0)
		{
			return 0;
		}
		return (int)num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe void CopyBytes(byte[] buffer, int offset, int count)
	{
		if (count < 9)
		{
			int num = count;
			int num2 = readBufferIndex;
			byte* ptr = pinnedReadBuffer;
			while (--num > -1)
			{
				buffer[offset + num] = ptr[num2 + num];
			}
		}
		else
		{
			Buffer.BlockCopy(readBuffer, readBufferIndex, buffer, offset, count);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckEof(int read)
	{
		if (read == 0)
		{
			EofHitCount++;
		}
	}
}
