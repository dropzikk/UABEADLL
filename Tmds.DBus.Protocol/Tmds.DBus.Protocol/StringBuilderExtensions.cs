using System;
using System.Buffers;
using System.Text;

namespace Tmds.DBus.Protocol;

internal static class StringBuilderExtensions
{
	public static void AppendUTF8(this StringBuilder sb, ReadOnlySpan<byte> value)
	{
		char[] array = null;
		int charCount = Encoding.UTF8.GetCharCount(value);
		Span<char> span = ((charCount > 256) ? ((Span<char>)(array = ArrayPool<char>.Shared.Rent(charCount))) : stackalloc char[charCount]);
		Span<char> chars = span;
		sb.Append(chars[..Encoding.UTF8.GetChars(value, chars)]);
		if (array != null)
		{
			ArrayPool<char>.Shared.Return(array);
		}
	}
}
