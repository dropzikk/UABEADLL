using System;
using System.Collections.Generic;
using System.Text;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal static class VariantReader
{
	public static DBusVariantItem ReadDBusVariant(this ref Reader reader)
	{
		ReadOnlySpan<byte> signature = reader.ReadSignature();
		if (!new SignatureReader(signature).TryRead(out var type, out var innerSignature))
		{
			throw new InvalidOperationException("Unable to read empty variant");
		}
		return new DBusVariantItem(Encoding.UTF8.GetString(innerSignature.ToArray()), reader.ReadDBusItem(type, innerSignature));
	}

	private static DBusBasicTypeItem ReadDBusBasicTypeItem(this ref Reader reader, DBusType dBusType)
	{
		return dBusType switch
		{
			DBusType.Byte => new DBusByteItem(reader.ReadByte()), 
			DBusType.Bool => new DBusBoolItem(reader.ReadBool()), 
			DBusType.Int16 => new DBusInt16Item(reader.ReadInt16()), 
			DBusType.UInt16 => new DBusUInt16Item(reader.ReadUInt16()), 
			DBusType.Int32 => new DBusInt32Item(reader.ReadInt32()), 
			DBusType.UInt32 => new DBusUInt32Item(reader.ReadUInt32()), 
			DBusType.Int64 => new DBusInt64Item(reader.ReadInt64()), 
			DBusType.UInt64 => new DBusUInt64Item(reader.ReadUInt64()), 
			DBusType.Double => new DBusDoubleItem(reader.ReadDouble()), 
			DBusType.String => new DBusStringItem(reader.ReadString()), 
			DBusType.ObjectPath => new DBusObjectPathItem(reader.ReadObjectPath()), 
			DBusType.Signature => new DBusSignatureItem(new Signature(reader.ReadSignature().ToString())), 
			_ => throw new ArgumentOutOfRangeException("dBusType"), 
		};
	}

	private static DBusItem ReadDBusItem(this ref Reader reader, DBusType dBusType, ReadOnlySpan<byte> innerSignature)
	{
		switch (dBusType)
		{
		case DBusType.Byte:
			return new DBusByteItem(reader.ReadByte());
		case DBusType.Bool:
			return new DBusBoolItem(reader.ReadBool());
		case DBusType.Int16:
			return new DBusInt16Item(reader.ReadInt16());
		case DBusType.UInt16:
			return new DBusUInt16Item(reader.ReadUInt16());
		case DBusType.Int32:
			return new DBusInt32Item(reader.ReadInt32());
		case DBusType.UInt32:
			return new DBusUInt32Item(reader.ReadUInt32());
		case DBusType.Int64:
			return new DBusInt64Item(reader.ReadInt64());
		case DBusType.UInt64:
			return new DBusUInt64Item(reader.ReadUInt64());
		case DBusType.Double:
			return new DBusDoubleItem(reader.ReadDouble());
		case DBusType.String:
			return new DBusStringItem(reader.ReadString());
		case DBusType.ObjectPath:
			return new DBusObjectPathItem(reader.ReadObjectPath());
		case DBusType.Signature:
			return new DBusSignatureItem(new Signature(reader.ReadSignature().ToString()));
		case DBusType.Array:
		{
			if (!new SignatureReader(innerSignature).TryRead(out var type4, out var innerSignature5))
			{
				throw new InvalidOperationException("Failed to deserialize array item");
			}
			List<DBusItem> list2 = new List<DBusItem>();
			ArrayEnd iterator = reader.ReadArrayStart(type4);
			while (reader.HasNext(iterator))
			{
				list2.Add(reader.ReadDBusItem(type4, innerSignature5));
			}
			return new DBusArrayItem(type4, list2);
		}
		case DBusType.DictEntry:
		{
			SignatureReader signatureReader2 = new SignatureReader(innerSignature);
			if (!signatureReader2.TryRead(out var type2, out var _) || !signatureReader2.TryRead(out var type3, out var innerSignature4))
			{
				throw new InvalidOperationException("Expected 2 inner types for DictEntry, got " + Encoding.UTF8.GetString(innerSignature.ToArray()));
			}
			DBusBasicTypeItem key = reader.ReadDBusBasicTypeItem(type2);
			DBusItem value = reader.ReadDBusItem(type3, innerSignature4);
			return new DBusDictEntryItem(key, value);
		}
		case DBusType.Struct:
		{
			reader.AlignStruct();
			List<DBusItem> list = new List<DBusItem>();
			SignatureReader signatureReader = new SignatureReader(innerSignature);
			DBusType type;
			ReadOnlySpan<byte> innerSignature2;
			while (signatureReader.TryRead(out type, out innerSignature2))
			{
				list.Add(reader.ReadDBusItem(type, innerSignature2));
			}
			return new DBusStructItem(list);
		}
		case DBusType.Variant:
			return reader.ReadDBusVariant();
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
