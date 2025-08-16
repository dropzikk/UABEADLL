using System.Collections.Generic;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal static class WriterExtensions
{
	public static void WriteDictionary_aess(this ref MessageWriter writer, Dictionary<string, string> values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.Struct);
		foreach (KeyValuePair<string, string> value in values)
		{
			writer.WriteStructureStart();
			writer.WriteString(value.Key);
			writer.WriteString(value.Value);
		}
		writer.WriteArrayEnd(start);
	}

	public static void WriteStruct_rssz(this ref MessageWriter writer, (string, string) value)
	{
		writer.WriteStructureStart();
		writer.WriteString(value.Item1);
		writer.WriteString(value.Item2);
	}

	public static void WriteArray_arssz(this ref MessageWriter writer, (string, string)[] values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.Struct);
		foreach ((string, string) value in values)
		{
			writer.WriteStruct_rssz(value);
		}
		writer.WriteArrayEnd(start);
	}

	public static void WriteArray_ad(this ref MessageWriter writer, double[] values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.Double);
		foreach (double value in values)
		{
			writer.WriteDouble(value);
		}
		writer.WriteArrayEnd(start);
	}

	public static void WriteDictionary_aesv(this ref MessageWriter writer, Dictionary<string, DBusVariantItem> values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.Struct);
		foreach (KeyValuePair<string, DBusVariantItem> value in values)
		{
			writer.WriteStructureStart();
			writer.WriteString(value.Key);
			writer.WriteDBusVariant(value.Value);
		}
		writer.WriteArrayEnd(start);
	}

	public static void WriteArray_as(this ref MessageWriter writer, string[] values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.String);
		foreach (string value in values)
		{
			writer.WriteString(value);
		}
		writer.WriteArrayEnd(start);
	}

	public static void WriteArray_av(this ref MessageWriter writer, DBusVariantItem[] values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.Variant);
		foreach (DBusVariantItem value in values)
		{
			writer.WriteDBusVariant(value);
		}
		writer.WriteArrayEnd(start);
	}

	public static void WriteStruct_riaesvavz(this ref MessageWriter writer, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]) value)
	{
		writer.WriteStructureStart();
		writer.WriteInt32(value.Item1);
		writer.WriteDictionary_aesv(value.Item2);
		writer.WriteArray_av(value.Item3);
	}

	public static void WriteStruct_riaesvz(this ref MessageWriter writer, (int, Dictionary<string, DBusVariantItem>) value)
	{
		writer.WriteStructureStart();
		writer.WriteInt32(value.Item1);
		writer.WriteDictionary_aesv(value.Item2);
	}

	public static void WriteArray_ariaesvz(this ref MessageWriter writer, (int, Dictionary<string, DBusVariantItem>)[] values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.Struct);
		foreach ((int, Dictionary<string, DBusVariantItem>) value in values)
		{
			writer.WriteStruct_riaesvz(value);
		}
		writer.WriteArrayEnd(start);
	}

	public static void WriteArray_ai(this ref MessageWriter writer, int[] values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.Int32);
		foreach (int value in values)
		{
			writer.WriteInt32(value);
		}
		writer.WriteArrayEnd(start);
	}

	public static void WriteStruct_riasz(this ref MessageWriter writer, (int, string[]) value)
	{
		writer.WriteStructureStart();
		writer.WriteInt32(value.Item1);
		writer.WriteArray_as(value.Item2);
	}

	public static void WriteArray_ariasz(this ref MessageWriter writer, (int, string[])[] values)
	{
		ArrayStart start = writer.WriteArrayStart(DBusType.Struct);
		foreach ((int, string[]) value in values)
		{
			writer.WriteStruct_riasz(value);
		}
		writer.WriteArrayEnd(start);
	}
}
