using System;
using System.IO;
using System.Text;

namespace AvaloniaEdit.Utils;

public static class FileReader
{
	private static readonly Encoding UTF8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

	public static bool IsUnicode(Encoding encoding)
	{
		if (encoding == null)
		{
			throw new ArgumentNullException("encoding");
		}
		if (encoding is UnicodeEncoding || encoding is UTF8Encoding)
		{
			return true;
		}
		return false;
	}

	private static bool IsAsciiCompatible(Encoding encoding)
	{
		byte[] bytes = encoding.GetBytes("Az");
		if (bytes.Length == 2 && bytes[0] == 65)
		{
			return bytes[1] == 122;
		}
		return false;
	}

	private static Encoding RemoveBom(Encoding encoding)
	{
		if (encoding is UTF8Encoding)
		{
			return UTF8NoBOM;
		}
		return encoding;
	}

	public static string ReadFileContent(Stream stream, Encoding defaultEncoding)
	{
		using StreamReader streamReader = OpenStream(stream, defaultEncoding);
		return streamReader.ReadToEnd();
	}

	public static string ReadFileContent(string fileName, Encoding defaultEncoding)
	{
		throw new NotImplementedException();
	}

	public static StreamReader OpenFile(string fileName, Encoding defaultEncoding)
	{
		throw new NotImplementedException();
	}

	public static StreamReader OpenStream(Stream stream, Encoding defaultEncoding)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (stream.Position != 0L)
		{
			throw new ArgumentException("stream is not positioned at beginning.", "stream");
		}
		if (defaultEncoding == null)
		{
			throw new ArgumentNullException("defaultEncoding");
		}
		if (stream.Length >= 2)
		{
			int num = stream.ReadByte();
			int num2 = stream.ReadByte();
			switch ((num << 8) | num2)
			{
			case 0:
			case 61371:
			case 65279:
			case 65534:
				stream.Position = 0L;
				return new StreamReader(stream);
			default:
				return AutoDetect(stream, (byte)num, (byte)num2, defaultEncoding);
			}
		}
		return new StreamReader(stream, defaultEncoding);
	}

	private static StreamReader AutoDetect(Stream fs, byte firstByte, byte secondByte, Encoding defaultEncoding)
	{
		int num = (int)Math.Min(fs.Length, 500000L);
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			byte b = i switch
			{
				0 => firstByte, 
				1 => secondByte, 
				_ => (byte)fs.ReadByte(), 
			};
			if (b < 128)
			{
				if (num2 == 3)
				{
					num2 = 1;
					break;
				}
			}
			else if (b < 192)
			{
				if (num2 != 3)
				{
					num2 = 1;
					break;
				}
				num3--;
				if (num3 < 0)
				{
					num2 = 1;
					break;
				}
				if (num3 == 0)
				{
					num2 = 2;
				}
			}
			else
			{
				if (b < 194 || b >= 245)
				{
					num2 = 1;
					break;
				}
				if (num2 != 2 && num2 != 0)
				{
					num2 = 1;
					break;
				}
				num2 = 3;
				num3 = ((b < 224) ? 1 : ((b >= 240) ? 3 : 2));
			}
		}
		fs.Position = 0L;
		switch (num2)
		{
		case 0:
			return new StreamReader(fs, IsAsciiCompatible(defaultEncoding) ? RemoveBom(defaultEncoding) : Encoding.UTF8);
		case 1:
			if (IsUnicode(defaultEncoding))
			{
				defaultEncoding = Encoding.UTF8;
			}
			return new StreamReader(fs, RemoveBom(defaultEncoding));
		default:
			return new StreamReader(fs, UTF8NoBOM);
		}
	}
}
