using System;

namespace Tmds.DBus.Protocol;

internal static class AddressParser
{
	public struct AddressEntry
	{
		internal string String { get; }

		internal int Offset { get; }

		internal int Count { get; }

		internal AddressEntry(string s, int offset, int count)
		{
			String = s;
			Offset = offset;
			Count = count;
		}

		internal ReadOnlySpan<char> AsSpan()
		{
			return String.AsSpan(Offset, Count);
		}

		public override string ToString()
		{
			return AsSpan().AsString();
		}
	}

	public static bool TryGetNextEntry(string addresses, ref AddressEntry address)
	{
		int num = ((address.String != null) ? (address.Offset + address.Count + 1) : 0);
		if (num >= addresses.Length - 1)
		{
			return false;
		}
		ReadOnlySpan<char> span = addresses.AsSpan().Slice(num);
		int num2 = span.IndexOf(';');
		if (num2 == -1)
		{
			num2 = span.Length;
		}
		address = new AddressEntry(addresses, num, num2);
		return true;
	}

	public static bool IsType(AddressEntry address, string type)
	{
		ReadOnlySpan<char> span = address.AsSpan();
		if (span.Length > type.Length && span[type.Length] == ':')
		{
			return span.StartsWith(type.AsSpan());
		}
		return false;
	}

	public static void ParseTcpProperties(AddressEntry address, out string host, out int? port, out Guid guid)
	{
		host = null;
		port = null;
		guid = default(Guid);
		ReadOnlySpan<char> properties = GetProperties(address);
		ReadOnlySpan<char> key;
		ReadOnlySpan<char> value;
		while (TryParseProperty(ref properties, out key, out value))
		{
			switch (key)
			{
			case "host":
				host = Unescape(value);
				break;
			case "port":
				port = int.Parse(Unescape(value));
				break;
			case "guid":
				guid = Guid.ParseExact(Unescape(value), "N");
				break;
			}
		}
		if (host == null)
		{
			host = "localhost";
		}
	}

	public static void ParseUnixProperties(AddressEntry address, out string path, out Guid guid)
	{
		path = null;
		bool flag = false;
		guid = default(Guid);
		ReadOnlySpan<char> properties = GetProperties(address);
		ReadOnlySpan<char> key;
		ReadOnlySpan<char> value;
		while (TryParseProperty(ref properties, out key, out value))
		{
			switch (key)
			{
			case "path":
				path = Unescape(value);
				break;
			case "abstract":
				flag = true;
				path = Unescape(value);
				break;
			case "guid":
				guid = Guid.ParseExact(Unescape(value), "N");
				break;
			}
		}
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException("path");
		}
		if (flag)
		{
			path = "\0" + path;
		}
	}

	private static ReadOnlySpan<char> GetProperties(AddressEntry address)
	{
		ReadOnlySpan<char> span = address.AsSpan();
		int num = span.IndexOf(':');
		if (num == -1)
		{
			throw new FormatException("No colon found.");
		}
		return span.Slice(num + 1);
	}

	public static bool TryParseProperty(ref ReadOnlySpan<char> properties, out ReadOnlySpan<char> key, out ReadOnlySpan<char> value)
	{
		if (properties.Length == 0)
		{
			key = default(ReadOnlySpan<char>);
			value = default(ReadOnlySpan<char>);
			return false;
		}
		int num = properties.IndexOf(',');
		ReadOnlySpan<char> span;
		if (num == -1)
		{
			span = properties;
			properties = default(ReadOnlySpan<char>);
		}
		else
		{
			span = properties.Slice(0, num);
			properties = properties.Slice(num + 1);
		}
		int num2 = span.IndexOf('=');
		if (num2 == -1)
		{
			throw new FormatException("No equals sign found.");
		}
		key = span.Slice(0, num2);
		value = span.Slice(num2 + 1);
		return true;
	}

	private static string Unescape(ReadOnlySpan<char> value)
	{
		if (!value.Contains("%".AsSpan(), StringComparison.Ordinal))
		{
			return value.AsString();
		}
		Span<char> span = stackalloc char[256];
		int length = 0;
		int num = 0;
		while (num < value.Length)
		{
			char c2 = value[num++];
			if (c2 != '%')
			{
				span[length++] = c2;
				continue;
			}
			if (num + 2 < value.Length)
			{
				int num2 = FromHexChar(value[num++]);
				int num3 = FromHexChar(value[num++]);
				if (num2 == -1 || num3 == -1)
				{
					throw new FormatException("Invalid hex char.");
				}
				span[length++] = (char)((num2 << 4) + num3);
				continue;
			}
			throw new FormatException("Escape sequence is too short.");
		}
		return span.Slice(0, length).AsString();
		static int FromHexChar(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - 48;
			}
			if (c >= 'A' && c <= 'F')
			{
				return c - 65 + 10;
			}
			if (c >= 'a' && c <= 'f')
			{
				return c - 97 + 10;
			}
			return -1;
		}
	}
}
