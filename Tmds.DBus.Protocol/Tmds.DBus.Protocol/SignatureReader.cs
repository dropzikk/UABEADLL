using System;

namespace Tmds.DBus.Protocol;

public ref struct SignatureReader
{
	private ReadOnlySpan<byte> _signature;

	public ReadOnlySpan<byte> Signature => _signature;

	private static ReadOnlySpan<byte> BasicTypes => "ybnqiuxtdsogh"u8;

	public SignatureReader(ReadOnlySpan<byte> signature)
	{
		_signature = signature;
	}

	public bool TryRead(out DBusType type, out ReadOnlySpan<byte> innerSignature)
	{
		innerSignature = default(ReadOnlySpan<byte>);
		if (_signature.IsEmpty)
		{
			type = DBusType.Invalid;
			return false;
		}
		type = ReadSingleType(_signature, out var length);
		if (length > 1)
		{
			switch (type)
			{
			case DBusType.Array:
				innerSignature = _signature.Slice(1, length - 1);
				break;
			case DBusType.Struct:
			case DBusType.DictEntry:
				innerSignature = _signature.Slice(1, length - 2);
				break;
			}
		}
		_signature = _signature.Slice(length);
		return true;
	}

	private static DBusType ReadSingleType(ReadOnlySpan<byte> signature, out int length)
	{
		length = 0;
		if (signature.IsEmpty)
		{
			return DBusType.Invalid;
		}
		DBusType dBusType = (DBusType)signature[0];
		if (IsBasicType(dBusType))
		{
			length = 1;
		}
		else
		{
			switch (dBusType)
			{
			case DBusType.Variant:
				length = 1;
				break;
			case DBusType.Array:
			{
				if (ReadSingleType(signature.Slice(1), out var length3) != 0)
				{
					dBusType = DBusType.Array;
					length = length3 + 1;
				}
				else
				{
					dBusType = DBusType.Invalid;
				}
				break;
			}
			case DBusType.Struct:
				length = DetermineLength(signature.Slice(1), 40, 41);
				if (length == 0)
				{
					dBusType = DBusType.Invalid;
				}
				break;
			case DBusType.DictEntry:
			{
				length = DetermineLength(signature.Slice(1), 123, 125);
				if (length < 4 || !IsBasicType((DBusType)signature[1]) || ReadSingleType(signature.Slice(2), out var length2) == DBusType.Invalid || length != length2 + 3)
				{
					dBusType = DBusType.Invalid;
				}
				break;
			}
			default:
				dBusType = DBusType.Invalid;
				break;
			}
		}
		return dBusType;
	}

	private static int DetermineLength(ReadOnlySpan<byte> span, byte startChar, byte endChar)
	{
		int num = 1;
		int num2 = 1;
		do
		{
			int num3 = span.IndexOfAny(startChar, endChar);
			if (num3 == -1)
			{
				return 0;
			}
			num2 = ((span[num3] != startChar) ? (num2 - 1) : (num2 + 1));
			num += num3 + 1;
			span = span.Slice(num3 + 1);
		}
		while (num2 > 0);
		return num;
	}

	private static bool IsBasicType(DBusType type)
	{
		return BasicTypes.IndexOf((byte)type) != -1;
	}

	private static ReadOnlySpan<byte> ReadSingleType(ref ReadOnlySpan<byte> signature)
	{
		if (signature.Length == 0)
		{
			return default(ReadOnlySpan<byte>);
		}
		int num;
		switch ((DBusType)signature[0])
		{
		case DBusType.Struct:
			num = DetermineLength(signature.Slice(1), 40, 41);
			break;
		case DBusType.DictEntry:
			num = DetermineLength(signature.Slice(1), 123, 125);
			break;
		case DBusType.Array:
		{
			ReadOnlySpan<byte> signature2 = signature.Slice(1);
			num = 1 + ReadSingleType(ref signature2).Length;
			break;
		}
		default:
			num = 1;
			break;
		}
		ReadOnlySpan<byte> result = signature.Slice(0, num);
		signature = signature.Slice(num);
		return result;
	}

	internal static T Transform<T>(ReadOnlySpan<byte> signature, Func<DBusType, T[], T> map)
	{
		DBusType dBusType = (DBusType)((signature.Length != 0) ? signature[0] : 0);
		switch (dBusType)
		{
		case DBusType.Array:
		{
			if (signature[1] == 123)
			{
				signature = signature.Slice(2);
				ReadOnlySpan<byte> signature3 = ReadSingleType(ref signature);
				ReadOnlySpan<byte> signature4 = ReadSingleType(ref signature);
				signature = signature.Slice(1);
				T val = Transform(signature3, map);
				T val2 = Transform(signature4, map);
				return map(DBusType.DictEntry, new T[2] { val, val2 });
			}
			signature = signature.Slice(1);
			T val3 = Transform(signature, map);
			signature = signature.Slice(1);
			return map(DBusType.Array, new T[1] { val3 });
		}
		case DBusType.Struct:
		{
			signature = signature.Slice(1, signature.Length - 2);
			T[] array = new T[CountTypes(signature)];
			for (int i = 0; i < array.Length; i++)
			{
				ReadOnlySpan<byte> signature2 = ReadSingleType(ref signature);
				array[i] = Transform(signature2, map);
			}
			return map(DBusType.Struct, array);
		}
		default:
			return map(dBusType, Array.Empty<T>());
		}
	}

	private static int CountTypes(ReadOnlySpan<byte> signature)
	{
		if (signature.Length == 0)
		{
			return 0;
		}
		if (signature.Length == 1)
		{
			return 1;
		}
		DBusType dBusType = (DBusType)signature[0];
		signature = signature.Slice(1);
		switch (dBusType)
		{
		case DBusType.Struct:
			ReadToEnd(ref signature, 40, 41);
			break;
		case DBusType.DictEntry:
			ReadToEnd(ref signature, 123, 125);
			break;
		}
		return ((dBusType != DBusType.Array) ? 1 : 0) + CountTypes(signature);
		static void ReadToEnd(ref ReadOnlySpan<byte> span, byte startChar, byte endChar)
		{
			int num = 1;
			do
			{
				int num2 = span.IndexOfAny(startChar, endChar);
				num = ((span[num2] != startChar) ? (num - 1) : (num + 1));
				span = span.Slice(num2 + 1);
			}
			while (num > 0);
		}
	}
}
