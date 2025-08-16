using System;
using System.Text;

namespace Tmds.DBus.Protocol;

public ref struct Utf8Span
{
	private ReadOnlySpan<byte> _buffer;

	public ReadOnlySpan<byte> Span => _buffer;

	public bool IsEmpty => _buffer.IsEmpty;

	public Utf8Span(ReadOnlySpan<byte> value)
	{
		_buffer = value;
	}

	public static implicit operator Utf8Span(ReadOnlySpan<byte> value)
	{
		return new Utf8Span(value);
	}

	public static implicit operator Utf8Span(Span<byte> value)
	{
		return new Utf8Span(value);
	}

	public static implicit operator ReadOnlySpan<byte>(Utf8Span value)
	{
		return value._buffer;
	}

	public override string ToString()
	{
		return Encoding.UTF8.GetString(_buffer);
	}
}
