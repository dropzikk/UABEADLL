using System;
using System.Runtime.CompilerServices;

namespace Tmds.DBus.Protocol;

internal static class ProtocolConstants
{
	public static ReadOnlySpan<byte> ByteSignature => "y"u8;

	public static ReadOnlySpan<byte> BooleanSignature => "b"u8;

	public static ReadOnlySpan<byte> Int16Signature => "n"u8;

	public static ReadOnlySpan<byte> UInt16Signature => "q"u8;

	public static ReadOnlySpan<byte> Int32Signature => "i"u8;

	public static ReadOnlySpan<byte> UInt32Signature => "u"u8;

	public static ReadOnlySpan<byte> Int64Signature => "x"u8;

	public static ReadOnlySpan<byte> UInt64Signature => "t"u8;

	public static ReadOnlySpan<byte> DoubleSignature => "d"u8;

	public static ReadOnlySpan<byte> UnixFdSignature => "h"u8;

	public static ReadOnlySpan<byte> StringSignature => "s"u8;

	public static ReadOnlySpan<byte> ObjectPathSignature => "o"u8;

	public static ReadOnlySpan<byte> SignatureSignature => "g"u8;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetTypeAlignment(DBusType type)
	{
		return type switch
		{
			DBusType.Byte => 1, 
			DBusType.Bool => 4, 
			DBusType.Int16 => 2, 
			DBusType.UInt16 => 2, 
			DBusType.Int32 => 4, 
			DBusType.UInt32 => 4, 
			DBusType.Int64 => 8, 
			DBusType.UInt64 => 8, 
			DBusType.Double => 8, 
			DBusType.String => 4, 
			DBusType.ObjectPath => 4, 
			DBusType.Signature => 4, 
			DBusType.Array => 4, 
			DBusType.Struct => 8, 
			DBusType.Variant => 1, 
			DBusType.DictEntry => 8, 
			DBusType.UnixFd => 4, 
			_ => 1, 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetFixedTypeLength(DBusType type)
	{
		return type switch
		{
			DBusType.Byte => 1, 
			DBusType.Bool => 4, 
			DBusType.Int16 => 2, 
			DBusType.UInt16 => 2, 
			DBusType.Int32 => 4, 
			DBusType.UInt32 => 4, 
			DBusType.Int64 => 8, 
			DBusType.UInt64 => 8, 
			DBusType.Double => 8, 
			DBusType.UnixFd => 4, 
			_ => 0, 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Align(int offset, DBusType type)
	{
		return offset + GetPadding(offset, type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetPadding(int offset, DBusType type)
	{
		int typeAlignment = GetTypeAlignment(type);
		return (~offset + 1) & (typeAlignment - 1);
	}
}
