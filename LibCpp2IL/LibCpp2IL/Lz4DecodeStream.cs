using System;
using System.IO;

namespace LibCpp2IL;

public class Lz4DecodeStream : Stream
{
	private enum DecodePhase
	{
		ReadToken,
		ReadExLiteralLength,
		CopyLiteral,
		ReadMatch,
		ReadExMatchLength,
		CopyMatch,
		Finish
	}

	private const int InputBufferCapacity = 4096;

	private const int DecodeBufferCapacity = 65536;

	private const int DecodeBufferMask = 65535;

	private readonly byte[] m_inputBuffer = new byte[4096];

	private readonly byte[] m_decodeBuffer = new byte[65536];

	private readonly Stream m_baseStream;

	private readonly bool m_leaveOpen;

	private long m_position;

	private long m_inputLeft;

	private int m_inputBufferPosition = 4096;

	private int m_decodeBufferPosition;

	private DecodePhase m_phase;

	private int m_literalLength;

	private int m_matchLength;

	private int m_matchDestination;

	private int m_decodeBufferStart;

	public bool IsDataLeft
	{
		get
		{
			if (m_phase != DecodePhase.CopyLiteral)
			{
				return m_matchLength != 0;
			}
			return m_literalLength != 0;
		}
	}

	public override bool CanSeek => false;

	public override bool CanRead => true;

	public override bool CanWrite => false;

	public override long Length { get; }

	public override long Position
	{
		get
		{
			return m_position;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public Lz4DecodeStream(byte[] buffer, int offset, int length)
		: this(new MemoryStream(buffer, offset, length), length, leaveOpen: false)
	{
	}

	public Lz4DecodeStream(Stream baseStream, bool leaveOpen = true)
		: this(baseStream, baseStream.Length, leaveOpen)
	{
	}

	public Lz4DecodeStream(Stream baseStream, long compressedSize, bool leaveOpen = true)
	{
		if (compressedSize <= 0)
		{
			throw new ArgumentException($"Compressed size {compressedSize} must be greater then 0");
		}
		m_baseStream = baseStream ?? throw new ArgumentNullException("baseStream");
		Length = compressedSize;
		m_inputLeft = compressedSize;
		m_phase = DecodePhase.ReadToken;
		m_leaveOpen = leaveOpen;
	}

	~Lz4DecodeStream()
	{
		Dispose(disposing: false);
	}

	public override void Flush()
	{
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		using MemoryStream stream = new MemoryStream(buffer, offset, count);
		return (int)Read(stream, count);
	}

	public long Read(Stream stream, long count)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (count <= 0)
		{
			throw new ArgumentException("count");
		}
		long num = count;
		switch (m_phase)
		{
		case DecodePhase.ReadToken:
		{
			int num2 = ReadInputByte();
			m_literalLength = num2 >> 4;
			m_matchLength = (num2 & 0xF) + 4;
			if (m_literalLength != 0)
			{
				if (m_literalLength == 15)
				{
					goto case DecodePhase.ReadExLiteralLength;
				}
				goto case DecodePhase.CopyLiteral;
			}
			goto case DecodePhase.ReadMatch;
		}
		case DecodePhase.ReadExLiteralLength:
		{
			int num4;
			do
			{
				num4 = ReadInputByte();
				m_literalLength += num4;
			}
			while (num4 == 255);
			goto case DecodePhase.CopyLiteral;
		}
		case DecodePhase.CopyLiteral:
			if (m_literalLength >= num)
			{
				Write(stream, (int)num);
				m_literalLength -= (int)num;
				num = 0L;
				m_phase = DecodePhase.CopyLiteral;
				goto case DecodePhase.Finish;
			}
			Write(stream, m_literalLength);
			num -= m_literalLength;
			goto case DecodePhase.ReadMatch;
		case DecodePhase.ReadMatch:
			m_matchDestination = ReadInputInt16();
			if (m_matchLength == 19)
			{
				goto case DecodePhase.ReadExMatchLength;
			}
			goto case DecodePhase.CopyMatch;
		case DecodePhase.ReadExMatchLength:
		{
			int num3;
			do
			{
				num3 = ReadInputByte();
				m_matchLength += num3;
			}
			while (num3 == 255);
			goto case DecodePhase.CopyMatch;
		}
		case DecodePhase.CopyMatch:
		{
			int num5 = (int)((m_matchLength < num) ? m_matchLength : num);
			while (num5 > 0)
			{
				int num6 = (m_decodeBufferPosition - m_matchDestination) & 0xFFFF;
				int num7 = 65536 - num6;
				int num8 = 65536 - m_decodeBufferPosition;
				int num9 = ((num7 < num8) ? num7 : num8);
				int num10 = ((num5 < num9) ? num5 : num9);
				int num11 = m_decodeBufferPosition - num6;
				if (num11 > 0 && num11 < num10)
				{
					for (int i = 0; i < num10; i++)
					{
						m_decodeBuffer[m_decodeBufferPosition++] = m_decodeBuffer[num6++];
					}
				}
				else
				{
					Buffer.BlockCopy(m_decodeBuffer, num6, m_decodeBuffer, m_decodeBufferPosition, num10);
					m_decodeBufferPosition += num10;
				}
				num5 -= num10;
				m_matchLength -= num10;
				num -= num10;
				if (m_decodeBufferPosition == 65536)
				{
					FillOutputStream(stream);
				}
			}
			if (num != 0L)
			{
				goto case DecodePhase.ReadToken;
			}
			m_phase = DecodePhase.CopyMatch;
			goto case DecodePhase.Finish;
		}
		case DecodePhase.Finish:
			FillOutputStream(stream);
			return count - num;
		default:
			throw new Exception($"Unknonw decode phase {m_phase}");
		}
	}

	public void ReadBuffer(byte[] buffer, int offset, int count)
	{
		using MemoryStream stream = new MemoryStream(buffer, offset, count);
		ReadBuffer(stream, count);
	}

	public void ReadBuffer(Stream stream, long count)
	{
		int num = (int)Read(stream, count);
		if (num != count)
		{
			throw new Exception($"Unexpected end of input stream. Read {num} but expected {count}");
		}
		if (IsDataLeft)
		{
			throw new Exception("Some data left");
		}
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	protected override void Dispose(bool disposing)
	{
		if (!m_leaveOpen)
		{
			m_baseStream.Dispose();
		}
		base.Dispose(disposing);
	}

	private int ReadInputByte()
	{
		if (m_inputBufferPosition == 4096)
		{
			FillInputBuffer();
		}
		return m_inputBuffer[m_inputBufferPosition++];
	}

	private int ReadInputInt16()
	{
		switch (4096 - m_inputBufferPosition)
		{
		case 0:
			FillInputBuffer();
			break;
		case 1:
			m_inputBuffer[0] = m_inputBuffer[m_inputBufferPosition];
			FillInputBuffer(1);
			break;
		}
		return m_inputBuffer[m_inputBufferPosition++] | (m_inputBuffer[m_inputBufferPosition++] << 8);
	}

	private void Write(Stream stream, int count)
	{
		while (count > 0)
		{
			if (m_inputBufferPosition == 4096)
			{
				FillInputBuffer();
			}
			int num = 4096 - m_inputBufferPosition;
			int num2 = 65536 - m_decodeBufferPosition;
			int num3 = ((num < num2) ? num : num2);
			int num4 = ((count < num3) ? count : num3);
			Buffer.BlockCopy(m_inputBuffer, m_inputBufferPosition, m_decodeBuffer, m_decodeBufferPosition, num4);
			count -= num4;
			m_inputBufferPosition += num4;
			m_decodeBufferPosition += num4;
			if (m_decodeBufferPosition == 65536)
			{
				FillOutputStream(stream);
			}
		}
	}

	private void FillInputBuffer(int offset = 0)
	{
		int num = 4096 - offset;
		int num2 = (int)((num < m_inputLeft) ? num : m_inputLeft);
		m_inputBufferPosition = 0;
		while (num2 > 0)
		{
			int num3 = m_baseStream.Read(m_inputBuffer, offset, num2);
			if (num3 == 0)
			{
				throw new Exception("No data left");
			}
			m_position += num3;
			offset += num3;
			num2 -= num3;
			m_inputLeft -= num3;
		}
	}

	private void FillOutputStream(Stream stream)
	{
		int num = m_decodeBufferPosition - m_decodeBufferStart;
		int num2 = 65536 - m_decodeBufferStart;
		int num3 = ((num2 < num) ? num2 : num);
		stream.Write(m_decodeBuffer, m_decodeBufferStart, num3);
		stream.Write(m_decodeBuffer, 0, num - num3);
		m_decodeBufferPosition &= 65535;
		m_decodeBufferStart = m_decodeBufferPosition;
	}
}
