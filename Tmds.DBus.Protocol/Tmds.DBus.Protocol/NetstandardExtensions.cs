using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Tmds.DBus.Protocol;

internal static class NetstandardExtensions
{
	public static string AsString(this ReadOnlySpan<char> chars)
	{
		return new string(chars);
	}

	public static string AsString(this Span<char> chars)
	{
		return ((ReadOnlySpan<char>)chars).AsString();
	}

	public static SafeHandle GetSafeHandle(this Socket socket)
	{
		return socket.SafeHandle;
	}
}
