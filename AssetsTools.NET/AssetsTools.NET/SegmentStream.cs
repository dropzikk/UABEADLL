using System;
using System.IO;

namespace AssetsTools.NET;

public class SegmentStream : Stream
{
	private readonly long length;

	public Stream BaseStream { get; }

	public long BaseOffset { get; }

	public override long Position { get; set; }

	public override long Length => (length >= 0) ? length : (BaseStream.Length - BaseOffset);

	public override bool CanRead => BaseStream.CanRead;

	public override bool CanSeek => BaseStream.CanSeek;

	public override bool CanWrite => BaseStream.CanWrite;

	public SegmentStream(Stream baseStream, long baseOffset)
		: this(baseStream, baseOffset, -1L)
	{
	}

	public SegmentStream(Stream baseStream, long baseOffset, long length)
	{
		if (baseOffset < 0 || baseOffset > baseStream.Length)
		{
			throw new ArgumentOutOfRangeException("baseOffset");
		}
		if (length >= 0 && baseOffset + length > baseStream.Length)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		BaseStream = baseStream;
		BaseOffset = baseOffset;
		this.length = length;
	}

	public override void Flush()
	{
		BaseStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		BaseStream.Position = BaseOffset + Position;
		count = BaseStream.Read(buffer, offset, (int)Math.Min(count, Length - Position));
		Position += count;
		return count;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		long num = origin switch
		{
			SeekOrigin.Begin => offset, 
			SeekOrigin.Current => Position + offset, 
			SeekOrigin.End => Position + Length + offset, 
			_ => throw new ArgumentException(), 
		};
		if (num < 0 || num > Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		Position = num;
		return Position;
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (length >= 0 && count > Length - Position)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		BaseStream.Position = BaseOffset + Position;
		BaseStream.Write(buffer, offset, count);
		Position += count;
	}
}
