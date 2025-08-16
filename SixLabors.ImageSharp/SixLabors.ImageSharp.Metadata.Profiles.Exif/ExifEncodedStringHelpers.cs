using System;
using System.Buffers.Binary;
using System.Text;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal static class ExifEncodedStringHelpers
{
	public const int CharacterCodeBytesLength = 8;

	private const ulong AsciiCode = 314761761601uL;

	private const ulong JISCode = 5458250uL;

	private const ulong UnicodeCode = 19496880615018069uL;

	private const ulong UndefinedCode = 0uL;

	private static ReadOnlySpan<byte> AsciiCodeBytes => new byte[8] { 65, 83, 67, 73, 73, 0, 0, 0 };

	private static ReadOnlySpan<byte> JISCodeBytes => new byte[8] { 74, 73, 83, 0, 0, 0, 0, 0 };

	private static ReadOnlySpan<byte> UnicodeCodeBytes => new byte[8] { 85, 78, 73, 67, 79, 68, 69, 0 };

	private static ReadOnlySpan<byte> UndefinedCodeBytes => new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

	private static Encoding JIS0208Encoding
	{
		get
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			return Encoding.GetEncoding(20932);
		}
	}

	public static bool IsEncodedString(ExifTagValue tag)
	{
		if ((uint)(tag - 27) <= 1u || tag == ExifTagValue.UserComment)
		{
			return true;
		}
		return false;
	}

	public static ReadOnlySpan<byte> GetCodeBytes(EncodedString.CharacterCode code)
	{
		return code switch
		{
			EncodedString.CharacterCode.ASCII => AsciiCodeBytes, 
			EncodedString.CharacterCode.JIS => JISCodeBytes, 
			EncodedString.CharacterCode.Unicode => UnicodeCodeBytes, 
			EncodedString.CharacterCode.Undefined => UndefinedCodeBytes, 
			_ => UndefinedCodeBytes, 
		};
	}

	public static Encoding GetEncoding(EncodedString.CharacterCode code)
	{
		return code switch
		{
			EncodedString.CharacterCode.ASCII => Encoding.ASCII, 
			EncodedString.CharacterCode.JIS => JIS0208Encoding, 
			EncodedString.CharacterCode.Unicode => Encoding.Unicode, 
			EncodedString.CharacterCode.Undefined => Encoding.UTF8, 
			_ => Encoding.UTF8, 
		};
	}

	public static bool TryParse(ReadOnlySpan<byte> buffer, out EncodedString encodedString)
	{
		if (TryDetect(buffer, out var code))
		{
			string @string = GetEncoding(code).GetString(buffer.Slice(8, buffer.Length - 8));
			encodedString = new EncodedString(code, @string);
			return true;
		}
		encodedString = default(EncodedString);
		return false;
	}

	public static uint GetDataLength(EncodedString encodedString)
	{
		return (uint)(GetEncoding(encodedString.Code).GetByteCount(encodedString.Text) + 8);
	}

	public static int Write(EncodedString encodedString, Span<byte> destination)
	{
		GetCodeBytes(encodedString.Code).CopyTo(destination);
		string text = encodedString.Text;
		int num = Write(GetEncoding(encodedString.Code), text, destination.Slice(8, destination.Length - 8));
		return 8 + num;
	}

	public static int Write(Encoding encoding, string value, Span<byte> destination)
	{
		return encoding.GetBytes(value.AsSpan(), destination);
	}

	private static bool TryDetect(ReadOnlySpan<byte> buffer, out EncodedString.CharacterCode code)
	{
		if (buffer.Length >= 8)
		{
			switch (BinaryPrimitives.ReadUInt64LittleEndian(buffer))
			{
			case 314761761601uL:
				code = EncodedString.CharacterCode.ASCII;
				return true;
			case 5458250uL:
				code = EncodedString.CharacterCode.JIS;
				return true;
			case 19496880615018069uL:
				code = EncodedString.CharacterCode.Unicode;
				return true;
			case 0uL:
				code = EncodedString.CharacterCode.Undefined;
				return true;
			}
		}
		code = EncodedString.CharacterCode.ASCII;
		return false;
	}
}
