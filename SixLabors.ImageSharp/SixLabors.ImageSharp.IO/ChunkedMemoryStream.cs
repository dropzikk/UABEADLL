using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.IO;

internal sealed class ChunkedMemoryStream : Stream
{
	private sealed class MemoryChunk : IDisposable
	{
		private bool isDisposed;

		public IMemoryOwner<byte> Buffer { get; }

		public MemoryChunk? Next { get; set; }

		public int Length { get; init; }

		public MemoryChunk(IMemoryOwner<byte> buffer)
		{
			Buffer = buffer;
		}

		private void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{
					Buffer.Dispose();
				}
				isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

	private readonly MemoryAllocator allocator;

	private MemoryChunk? memoryChunk;

	private int chunkCount;

	private readonly int allocatorCapacity;

	private bool isDisposed;

	private MemoryChunk? writeChunk;

	private int writeOffset;

	private MemoryChunk? readChunk;

	private int readOffset;

	public override bool CanRead => !isDisposed;

	public override bool CanSeek => !isDisposed;

	public override bool CanWrite => !isDisposed;

	public override long Length
	{
		get
		{
			EnsureNotDisposed();
			int num = 0;
			MemoryChunk memoryChunk = this.memoryChunk;
			while (memoryChunk != null)
			{
				MemoryChunk? next = memoryChunk.Next;
				num = ((next == null) ? (num + writeOffset) : (num + memoryChunk.Length));
				memoryChunk = next;
			}
			return num;
		}
	}

	public override long Position
	{
		get
		{
			EnsureNotDisposed();
			if (readChunk == null)
			{
				return 0L;
			}
			int num = 0;
			MemoryChunk next = memoryChunk;
			while (next != readChunk && next != null)
			{
				num += next.Length;
				next = next.Next;
			}
			num += readOffset;
			return num;
		}
		set
		{
			EnsureNotDisposed();
			if (value < 0)
			{
				ThrowArgumentOutOfRange("value");
			}
			MemoryChunk memoryChunk = readChunk;
			int num = readOffset;
			readChunk = null;
			readOffset = 0;
			int num2 = (int)value;
			for (MemoryChunk next = this.memoryChunk; next != null; next = next.Next)
			{
				if (num2 < next.Length || (num2 == next.Length && next.Next == null))
				{
					readChunk = next;
					readOffset = num2;
					break;
				}
				num2 -= next.Length;
			}
			if (readChunk == null)
			{
				readChunk = memoryChunk;
				readOffset = num;
			}
		}
	}

	public ChunkedMemoryStream(MemoryAllocator allocator)
	{
		allocatorCapacity = allocator.GetBufferCapacityInBytes();
		this.allocator = allocator;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override long Seek(long offset, SeekOrigin origin)
	{
		EnsureNotDisposed();
		switch (origin)
		{
		case SeekOrigin.Begin:
			Position = offset;
			break;
		case SeekOrigin.Current:
			Position += offset;
			break;
		case SeekOrigin.End:
			Position = Length + offset;
			break;
		default:
			ThrowInvalidSeek();
			break;
		}
		return Position;
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	protected override void Dispose(bool disposing)
	{
		if (isDisposed)
		{
			return;
		}
		try
		{
			isDisposed = true;
			if (disposing)
			{
				ReleaseMemoryChunks(memoryChunk);
			}
			memoryChunk = null;
			writeChunk = null;
			readChunk = null;
			chunkCount = 0;
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	public override void Flush()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int Read(byte[] buffer, int offset, int count)
	{
		Guard.NotNull(buffer, "buffer");
		Guard.MustBeGreaterThanOrEqualTo(offset, 0, "offset");
		Guard.MustBeGreaterThanOrEqualTo(count, 0, "count");
		Guard.IsFalse(buffer.Length - offset < count, "buffer", "Offset subtracted from the buffer length is less than count.");
		return ReadImpl(buffer.AsSpan(offset, count));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int Read(Span<byte> buffer)
	{
		return ReadImpl(buffer);
	}

	private int ReadImpl(Span<byte> buffer)
	{
		EnsureNotDisposed();
		if (readChunk == null)
		{
			if (memoryChunk == null)
			{
				return 0;
			}
			readChunk = memoryChunk;
			readOffset = 0;
		}
		IMemoryOwner<byte> buffer2 = readChunk.Buffer;
		int length = readChunk.Length;
		if (readChunk.Next == null)
		{
			length = writeOffset;
		}
		int num = 0;
		int num2 = 0;
		int num3 = buffer.Length;
		while (num3 > 0)
		{
			if (readOffset == length)
			{
				if (readChunk.Next == null)
				{
					break;
				}
				readChunk = readChunk.Next;
				readOffset = 0;
				buffer2 = readChunk.Buffer;
				length = readChunk.Length;
				if (readChunk.Next == null)
				{
					length = writeOffset;
				}
			}
			int num4 = Math.Min(num3, length - readOffset);
			Span<byte> span = buffer2.Slice(readOffset, num4);
			int num5 = num2;
			span.CopyTo(buffer.Slice(num5, buffer.Length - num5));
			num2 += num4;
			num3 -= num4;
			readOffset += num4;
			num += num4;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int ReadByte()
	{
		EnsureNotDisposed();
		if (readChunk == null)
		{
			if (memoryChunk == null)
			{
				return 0;
			}
			readChunk = memoryChunk;
			readOffset = 0;
		}
		IMemoryOwner<byte> buffer = readChunk.Buffer;
		int length = readChunk.Length;
		if (readChunk.Next == null)
		{
			length = writeOffset;
		}
		if (readOffset == length)
		{
			if (readChunk.Next == null)
			{
				return -1;
			}
			readChunk = readChunk.Next;
			readOffset = 0;
			buffer = readChunk.Buffer;
		}
		return buffer.GetSpan()[readOffset++];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(byte[] buffer, int offset, int count)
	{
		Guard.NotNull(buffer, "buffer");
		Guard.MustBeGreaterThanOrEqualTo(offset, 0, "offset");
		Guard.MustBeGreaterThanOrEqualTo(count, 0, "count");
		Guard.IsFalse(buffer.Length - offset < count, "buffer", "Offset subtracted from the buffer length is less than count.");
		WriteImpl(buffer.AsSpan(offset, count));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(ReadOnlySpan<byte> buffer)
	{
		WriteImpl(buffer);
	}

	private void WriteImpl(ReadOnlySpan<byte> buffer)
	{
		EnsureNotDisposed();
		if (memoryChunk == null)
		{
			memoryChunk = AllocateMemoryChunk();
			writeChunk = memoryChunk;
			writeOffset = 0;
		}
		Guard.NotNull(writeChunk, "this.writeChunk");
		Span<byte> span = writeChunk.Buffer.GetSpan();
		int length = writeChunk.Length;
		int num = buffer.Length;
		int num2 = 0;
		while (num > 0)
		{
			if (writeOffset == length)
			{
				writeChunk.Next = AllocateMemoryChunk();
				writeChunk = writeChunk.Next;
				writeOffset = 0;
				span = writeChunk.Buffer.GetSpan();
				length = writeChunk.Length;
			}
			int num3 = Math.Min(num, length - writeOffset);
			ReadOnlySpan<byte> readOnlySpan = buffer.Slice(num2, num3);
			int num4 = writeOffset;
			readOnlySpan.CopyTo(span.Slice(num4, span.Length - num4));
			num2 += num3;
			num -= num3;
			writeOffset += num3;
		}
	}

	public override void WriteByte(byte value)
	{
		EnsureNotDisposed();
		if (memoryChunk == null)
		{
			memoryChunk = AllocateMemoryChunk();
			writeChunk = memoryChunk;
			writeOffset = 0;
		}
		Guard.NotNull(writeChunk, "this.writeChunk");
		IMemoryOwner<byte> buffer = writeChunk.Buffer;
		int length = writeChunk.Length;
		if (writeOffset == length)
		{
			writeChunk.Next = AllocateMemoryChunk();
			writeChunk = writeChunk.Next;
			writeOffset = 0;
			buffer = writeChunk.Buffer;
		}
		buffer.GetSpan()[writeOffset++] = value;
	}

	public byte[] ToArray()
	{
		int count = (int)Length;
		byte[] array = new byte[Length];
		MemoryChunk memoryChunk = readChunk;
		int num = readOffset;
		readChunk = this.memoryChunk;
		readOffset = 0;
		Read(array, 0, count);
		readChunk = memoryChunk;
		readOffset = num;
		return array;
	}

	public void WriteTo(Stream stream)
	{
		EnsureNotDisposed();
		Guard.NotNull(stream, "stream");
		if (readChunk == null)
		{
			if (memoryChunk == null)
			{
				return;
			}
			readChunk = memoryChunk;
			readOffset = 0;
		}
		IMemoryOwner<byte> buffer = readChunk.Buffer;
		int length = readChunk.Length;
		if (readChunk.Next == null)
		{
			length = writeOffset;
		}
		while (true)
		{
			if (readOffset == length)
			{
				if (readChunk.Next == null)
				{
					break;
				}
				readChunk = readChunk.Next;
				readOffset = 0;
				buffer = readChunk.Buffer;
				length = readChunk.Length;
				if (readChunk.Next == null)
				{
					length = writeOffset;
				}
			}
			int count = length - readOffset;
			stream.Write(buffer.GetSpan(), readOffset, count);
			readOffset = length;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnsureNotDisposed()
	{
		if (isDisposed)
		{
			ThrowDisposed();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowDisposed()
	{
		throw new ObjectDisposedException(null, "The stream is closed.");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowArgumentOutOfRange(string value)
	{
		throw new ArgumentOutOfRangeException(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowInvalidSeek()
	{
		throw new ArgumentException("Invalid seek origin.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private MemoryChunk AllocateMemoryChunk()
	{
		IMemoryOwner<byte> buffer = allocator.Allocate<byte>(Math.Min(allocatorCapacity, GetChunkSize(chunkCount++)));
		return new MemoryChunk(buffer)
		{
			Next = null,
			Length = buffer.Length()
		};
	}

	private static void ReleaseMemoryChunks(MemoryChunk? chunk)
	{
		while (chunk != null)
		{
			chunk.Dispose();
			chunk = chunk.Next;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetChunkSize(int i)
	{
		if (i >= 16)
		{
			return 4194304;
		}
		return 131072 * (1 << (int)((uint)i / 4u));
	}
}
