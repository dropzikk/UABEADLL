using System;
using System.Collections.Generic;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal static class ReaderExtensions
{
	public static string ReadMessage_s(Message message, object? _)
	{
		return message.GetBodyReader().ReadString();
	}

	public static uint ReadMessage_u(Message message, object? _)
	{
		return message.GetBodyReader().ReadUInt32();
	}

	public static bool ReadMessage_b(Message message, object? _)
	{
		return message.GetBodyReader().ReadBool();
	}

	public static string[] ReadArray_as(this ref Reader reader)
	{
		List<string> list = new List<string>();
		ArrayEnd iterator = reader.ReadArrayStart(DBusType.String);
		while (reader.HasNext(iterator))
		{
			list.Add(reader.ReadString());
		}
		return list.ToArray();
	}

	public static string[] ReadMessage_as(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		return reader.ReadArray_as();
	}

	public static byte[] ReadArray_ay(this ref Reader reader)
	{
		List<byte> list = new List<byte>();
		ArrayEnd iterator = reader.ReadArrayStart(DBusType.Byte);
		while (reader.HasNext(iterator))
		{
			list.Add(reader.ReadByte());
		}
		return list.ToArray();
	}

	public static byte[] ReadMessage_ay(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		return reader.ReadArray_ay();
	}

	public static Dictionary<string, DBusVariantItem> ReadDictionary_aesv(this ref Reader reader)
	{
		Dictionary<string, DBusVariantItem> dictionary = new Dictionary<string, DBusVariantItem>();
		ArrayEnd iterator = reader.ReadArrayStart(DBusType.Struct);
		while (reader.HasNext(iterator))
		{
			dictionary.Add(reader.ReadString(), reader.ReadDBusVariant());
		}
		return dictionary;
	}

	public static Dictionary<string, DBusVariantItem> ReadMessage_aesv(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		return reader.ReadDictionary_aesv();
	}

	public static (string Item1, string Item2, string Item3) ReadMessage_sss(Message message, object? _)
	{
		Reader bodyReader = message.GetBodyReader();
		string item = bodyReader.ReadString();
		string item2 = bodyReader.ReadString();
		string item3 = bodyReader.ReadString();
		return (Item1: item, Item2: item2, Item3: item3);
	}

	public static int ReadMessage_i(Message message, object? _)
	{
		return message.GetBodyReader().ReadInt32();
	}

	public static (string service, ObjectPath menuObjectPath) ReadMessage_so(Message message, object? _)
	{
		Reader bodyReader = message.GetBodyReader();
		string item = bodyReader.ReadString();
		ObjectPath item2 = bodyReader.ReadObjectPath();
		return (service: item, menuObjectPath: item2);
	}

	public static (string, int) ReadStruct_rsiz(this ref Reader reader)
	{
		reader.AlignStruct();
		return ValueTuple.Create(reader.ReadString(), reader.ReadInt32());
	}

	public static (string, int)[] ReadArray_arsiz(this ref Reader reader)
	{
		List<(string, int)> list = new List<(string, int)>();
		ArrayEnd iterator = reader.ReadArrayStart(DBusType.Struct);
		while (reader.HasNext(iterator))
		{
			list.Add(reader.ReadStruct_rsiz());
		}
		return list.ToArray();
	}

	public static ((string, int)[] str, int cursorpos) ReadMessage_arsizi(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		(string, int)[] item = reader.ReadArray_arsiz();
		int item2 = reader.ReadInt32();
		return (str: item, cursorpos: item2);
	}

	public static (uint keyval, uint state, int type) ReadMessage_uui(Message message, object? _)
	{
		Reader bodyReader = message.GetBodyReader();
		uint item = bodyReader.ReadUInt32();
		uint item2 = bodyReader.ReadUInt32();
		int item3 = bodyReader.ReadInt32();
		return (keyval: item, state: item2, type: item3);
	}

	public static (int offset, uint nchar) ReadMessage_iu(Message message, object? _)
	{
		Reader bodyReader = message.GetBodyReader();
		int item = bodyReader.ReadInt32();
		uint item2 = bodyReader.ReadUInt32();
		return (offset: item, nchar: item2);
	}

	public static (int icid, bool enable, uint keyval1, uint state1, uint keyval2, uint state2) ReadMessage_ibuuuu(Message message, object? _)
	{
		Reader bodyReader = message.GetBodyReader();
		int item = bodyReader.ReadInt32();
		bool item2 = bodyReader.ReadBool();
		uint item3 = bodyReader.ReadUInt32();
		uint item4 = bodyReader.ReadUInt32();
		uint item5 = bodyReader.ReadUInt32();
		uint item6 = bodyReader.ReadUInt32();
		return (icid: item, enable: item2, keyval1: item3, state1: item4, keyval2: item5, state2: item6);
	}

	public static (uint keyval, uint state, bool type) ReadMessage_uub(Message message, object? _)
	{
		Reader bodyReader = message.GetBodyReader();
		uint item = bodyReader.ReadUInt32();
		uint item2 = bodyReader.ReadUInt32();
		bool item3 = bodyReader.ReadBool();
		return (keyval: item, state: item2, type: item3);
	}

	public static (ObjectPath Item1, byte[] Item2) ReadMessage_oay(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		ObjectPath item = reader.ReadObjectPath();
		byte[] item2 = reader.ReadArray_ay();
		return (Item1: item, Item2: item2);
	}

	public static ObjectPath ReadMessage_o(Message message, object? _)
	{
		return message.GetBodyReader().ReadObjectPath();
	}

	public static DBusVariantItem ReadMessage_v(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		return reader.ReadDBusVariant();
	}

	public static (uint keyval, uint keycode, uint state) ReadMessage_uuu(Message message, object? _)
	{
		Reader bodyReader = message.GetBodyReader();
		uint item = bodyReader.ReadUInt32();
		uint item2 = bodyReader.ReadUInt32();
		uint item3 = bodyReader.ReadUInt32();
		return (keyval: item, keycode: item2, state: item3);
	}

	public static (DBusVariantItem text, uint cursor_pos, bool visible) ReadMessage_vub(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		DBusVariantItem item = reader.ReadDBusVariant();
		uint item2 = reader.ReadUInt32();
		bool item3 = reader.ReadBool();
		return (text: item, cursor_pos: item2, visible: item3);
	}

	public static (DBusVariantItem text, uint cursor_pos, bool visible, uint mode) ReadMessage_vubu(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		DBusVariantItem item = reader.ReadDBusVariant();
		uint item2 = reader.ReadUInt32();
		bool item3 = reader.ReadBool();
		uint item4 = reader.ReadUInt32();
		return (text: item, cursor_pos: item2, visible: item3, mode: item4);
	}

	public static (DBusVariantItem text, bool visible) ReadMessage_vb(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		DBusVariantItem item = reader.ReadDBusVariant();
		bool item2 = reader.ReadBool();
		return (text: item, visible: item2);
	}

	public static (uint, uint) ReadStruct_ruuz(this ref Reader reader)
	{
		reader.AlignStruct();
		return ValueTuple.Create(reader.ReadUInt32(), reader.ReadUInt32());
	}

	public static ValueTuple<bool> ReadStruct_rbz(this ref Reader reader)
	{
		reader.AlignStruct();
		return ValueTuple.Create(reader.ReadBool());
	}

	public static (uint response, Dictionary<string, DBusVariantItem> results) ReadMessage_uaesv(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		uint item = reader.ReadUInt32();
		Dictionary<string, DBusVariantItem> item2 = reader.ReadDictionary_aesv();
		return (response: item, results: item2);
	}

	public static Dictionary<string, Dictionary<string, DBusVariantItem>> ReadDictionary_aesaesv(this ref Reader reader)
	{
		Dictionary<string, Dictionary<string, DBusVariantItem>> dictionary = new Dictionary<string, Dictionary<string, DBusVariantItem>>();
		ArrayEnd iterator = reader.ReadArrayStart(DBusType.Struct);
		while (reader.HasNext(iterator))
		{
			dictionary.Add(reader.ReadString(), reader.ReadDictionary_aesv());
		}
		return dictionary;
	}

	public static Dictionary<string, Dictionary<string, DBusVariantItem>> ReadMessage_aesaesv(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		return reader.ReadDictionary_aesaesv();
	}

	public static (string @namespace, string key, DBusVariantItem value) ReadMessage_ssv(Message message, object? _)
	{
		Reader reader = message.GetBodyReader();
		string item = reader.ReadString();
		string item2 = reader.ReadString();
		DBusVariantItem item3 = reader.ReadDBusVariant();
		return (@namespace: item, key: item2, value: item3);
	}

	public static int[] ReadArray_ai(this ref Reader reader)
	{
		List<int> list = new List<int>();
		ArrayEnd iterator = reader.ReadArrayStart(DBusType.Int32);
		while (reader.HasNext(iterator))
		{
			list.Add(reader.ReadInt32());
		}
		return list.ToArray();
	}

	public static (int, string, DBusVariantItem, uint) ReadStruct_risvuz(this ref Reader reader)
	{
		reader.AlignStruct();
		return ValueTuple.Create(reader.ReadInt32(), reader.ReadString(), reader.ReadDBusVariant(), reader.ReadUInt32());
	}

	public static (int, string, DBusVariantItem, uint)[] ReadArray_arisvuz(this ref Reader reader)
	{
		List<(int, string, DBusVariantItem, uint)> list = new List<(int, string, DBusVariantItem, uint)>();
		ArrayEnd iterator = reader.ReadArrayStart(DBusType.Struct);
		while (reader.HasNext(iterator))
		{
			list.Add(reader.ReadStruct_risvuz());
		}
		return list.ToArray();
	}
}
