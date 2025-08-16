using System;
using System.IO;

namespace AssetsTools.NET.Extra.Decompressors.LZ4;

public class Lz4DecoderStream : Stream
{
	private enum DecodePhase
	{
		ReadToken,
		ReadExLiteralLength,
		CopyLiteral,
		ReadOffset,
		ReadExMatchLength,
		CopyMatch
	}

	private long inputLength;

	private Stream input;

	private const int DecBufLen = 65536;

	private const int DecBufMask = 65535;

	private const int InBufLen = 128;

	private byte[] decodeBuffer = new byte[65664];

	private int decodeBufferPos;

	private int inBufPos;

	private int inBufEnd;

	private DecodePhase phase;

	private int litLen;

	private int matLen;

	private int matDst;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public Lz4DecoderStream(Stream input, long inputLength = long.MaxValue)
	{
		Reset(input, inputLength);
	}

	public void Reset(Stream input, long inputLength = long.MaxValue)
	{
		this.inputLength = inputLength;
		this.input = input;
		phase = DecodePhase.ReadToken;
		decodeBufferPos = 0;
		litLen = 0;
		matLen = 0;
		matDst = 0;
		inBufPos = 65536;
		inBufEnd = 65536;
	}

	public override void Close()
	{
		input = null;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0 || count < 0 || buffer.Length - count < offset)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (input == null)
		{
			throw new InvalidOperationException();
		}
		int num = count;
		byte[] array = decodeBuffer;
		int num8;
		switch (phase)
		{
		default:
		{
			int num2;
			if (inBufPos < inBufEnd)
			{
				num2 = array[inBufPos++];
			}
			else
			{
				num2 = ReadByteCore();
				if (num2 == -1)
				{
					break;
				}
			}
			litLen = num2 >> 4;
			matLen = (num2 & 0xF) + 4;
			int num3 = litLen;
			int num4 = num3;
			if (num4 != 0)
			{
				if (num4 == 15)
				{
					phase = DecodePhase.ReadExLiteralLength;
					goto case DecodePhase.ReadExLiteralLength;
				}
				phase = DecodePhase.CopyLiteral;
				goto case DecodePhase.CopyLiteral;
			}
			phase = DecodePhase.ReadOffset;
			goto case DecodePhase.ReadOffset;
		}
		case DecodePhase.ReadExLiteralLength:
			while (true)
			{
				int num16;
				if (inBufPos < inBufEnd)
				{
					num16 = array[inBufPos++];
				}
				else
				{
					num16 = ReadByteCore();
					if (num16 == -1)
					{
						break;
					}
				}
				litLen += num16;
				if (num16 == 255)
				{
					continue;
				}
				goto IL_019b;
			}
			break;
		case DecodePhase.CopyLiteral:
			do
			{
				int num5 = ((litLen < num) ? litLen : num);
				if (num5 == 0)
				{
					break;
				}
				if (inBufPos + num5 <= inBufEnd)
				{
					int num6 = offset;
					int num7 = num5;
					while (num7-- != 0)
					{
						buffer[num6++] = array[inBufPos++];
					}
					num8 = num5;
				}
				else
				{
					num8 = ReadCore(buffer, offset, num5);
					if (num8 == 0)
					{
						goto end_IL_0061;
					}
				}
				offset += num8;
				num -= num8;
				litLen -= num8;
			}
			while (litLen != 0);
			if (num == 0)
			{
				break;
			}
			phase = DecodePhase.ReadOffset;
			goto case DecodePhase.ReadOffset;
		case DecodePhase.ReadOffset:
			if (inBufPos + 1 < inBufEnd)
			{
				matDst = (array[inBufPos + 1] << 8) | array[inBufPos];
				inBufPos += 2;
			}
			else
			{
				matDst = ReadOffsetCore();
				if (matDst == -1)
				{
					break;
				}
			}
			if (matLen == 19)
			{
				phase = DecodePhase.ReadExMatchLength;
				goto case DecodePhase.ReadExMatchLength;
			}
			phase = DecodePhase.CopyMatch;
			goto case DecodePhase.CopyMatch;
		case DecodePhase.ReadExMatchLength:
			while (true)
			{
				int num15;
				if (inBufPos < inBufEnd)
				{
					num15 = array[inBufPos++];
				}
				else
				{
					num15 = ReadByteCore();
					if (num15 == -1)
					{
						break;
					}
				}
				matLen += num15;
				if (num15 == 255)
				{
					continue;
				}
				goto IL_0376;
			}
			break;
		case DecodePhase.CopyMatch:
			{
				int num9 = ((matLen < num) ? matLen : num);
				if (num9 != 0)
				{
					num8 = count - num;
					int num10 = matDst - num8;
					if (num10 > 0)
					{
						int num11 = decodeBufferPos - num10;
						if (num11 < 0)
						{
							num11 += 65536;
						}
						int num12 = ((num10 < num9) ? num10 : num9);
						int num13 = num12;
						while (num13-- != 0)
						{
							buffer[offset++] = array[num11++ & 0xFFFF];
						}
					}
					else
					{
						num10 = 0;
					}
					int num14 = offset - matDst;
					for (int i = num10; i < num9; i++)
					{
						buffer[offset++] = buffer[num14++];
					}
					num -= num9;
					matLen -= num9;
				}
				if (num == 0)
				{
					break;
				}
				phase = DecodePhase.ReadToken;
				goto default;
			}
			IL_0376:
			phase = DecodePhase.CopyMatch;
			goto case DecodePhase.CopyMatch;
			IL_019b:
			phase = DecodePhase.CopyLiteral;
			goto case DecodePhase.CopyLiteral;
			end_IL_0061:
			break;
		}
		num8 = count - num;
		int num17 = ((num8 < 65536) ? num8 : 65536);
		int srcOffset = offset - num17;
		if (num17 == 65536)
		{
			Buffer.BlockCopy(buffer, srcOffset, array, 0, 65536);
			decodeBufferPos = 0;
		}
		else
		{
			int num18 = decodeBufferPos;
			while (num17-- != 0)
			{
				array[num18++ & 0xFFFF] = buffer[srcOffset++];
			}
			decodeBufferPos = num18 & 0xFFFF;
		}
		return num8;
	}

	private int ReadByteCore()
	{
		byte[] array = decodeBuffer;
		if (inBufPos == inBufEnd)
		{
			int num = input.Read(array, 65536, (int)((128 < inputLength) ? 128 : inputLength));
			if (num == 0)
			{
				return -1;
			}
			inputLength -= num;
			inBufPos = 65536;
			inBufEnd = 65536 + num;
		}
		return array[inBufPos++];
	}

	private int ReadOffsetCore()
	{
		byte[] array = decodeBuffer;
		if (inBufPos == inBufEnd)
		{
			int num = input.Read(array, 65536, (int)((128 < inputLength) ? 128 : inputLength));
			if (num == 0)
			{
				return -1;
			}
			inputLength -= num;
			inBufPos = 65536;
			inBufEnd = 65536 + num;
		}
		if (inBufEnd - inBufPos == 1)
		{
			array[65536] = array[inBufPos];
			int num2 = input.Read(array, 65537, (int)((127 < inputLength) ? 127 : inputLength));
			if (num2 == 0)
			{
				inBufPos = 65536;
				inBufEnd = 65537;
				return -1;
			}
			inputLength -= num2;
			inBufPos = 65536;
			inBufEnd = 65536 + num2 + 1;
		}
		int result = (array[inBufPos + 1] << 8) | array[inBufPos];
		inBufPos += 2;
		return result;
	}

	private int ReadCore(byte[] buffer, int offset, int count)
	{
		int num = count;
		byte[] array = decodeBuffer;
		int num2 = inBufEnd - inBufPos;
		int num3 = ((num < num2) ? num : num2);
		if (num3 != 0)
		{
			int num4 = inBufPos;
			int num5 = num3;
			while (num5-- != 0)
			{
				buffer[offset++] = array[num4++];
			}
			inBufPos = num4;
			num -= num3;
		}
		if (num != 0)
		{
			int num6;
			if (num >= 128)
			{
				num6 = input.Read(buffer, offset, (int)((num < inputLength) ? num : inputLength));
				num -= num6;
			}
			else
			{
				num6 = input.Read(array, 65536, (int)((128 < inputLength) ? 128 : inputLength));
				inBufPos = 65536;
				inBufEnd = 65536 + num6;
				num3 = ((num < num6) ? num : num6);
				int num7 = inBufPos;
				int num8 = num3;
				while (num8-- != 0)
				{
					buffer[offset++] = array[num7++];
				}
				inBufPos = num7;
				num -= num3;
			}
			inputLength -= num6;
		}
		return count - num;
	}

	public override void Flush()
	{
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}
}
