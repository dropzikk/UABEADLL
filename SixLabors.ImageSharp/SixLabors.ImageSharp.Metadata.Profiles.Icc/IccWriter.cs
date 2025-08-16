using System.Collections.Generic;
using System.Linq;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccWriter
{
	public static byte[] Write(IccProfile profile)
	{
		Guard.NotNull(profile, "profile");
		using IccDataWriter iccDataWriter = new IccDataWriter();
		IccTagTableEntry[] table = WriteTagData(iccDataWriter, profile.Entries);
		WriteTagTable(iccDataWriter, table);
		WriteHeader(iccDataWriter, profile.Header);
		return iccDataWriter.GetData();
	}

	private static void WriteHeader(IccDataWriter writer, IccProfileHeader header)
	{
		writer.SetIndex(0);
		writer.WriteUInt32(writer.Length);
		writer.WriteAsciiString(header.CmmType, 4, ensureNullTerminator: false);
		writer.WriteVersionNumber(header.Version);
		writer.WriteUInt32((uint)header.Class);
		writer.WriteUInt32((uint)header.DataColorSpace);
		writer.WriteUInt32((uint)header.ProfileConnectionSpace);
		writer.WriteDateTime(header.CreationDate);
		writer.WriteAsciiString("acsp");
		writer.WriteUInt32((uint)header.PrimaryPlatformSignature);
		writer.WriteInt32((int)header.Flags);
		writer.WriteUInt32(header.DeviceManufacturer);
		writer.WriteUInt32(header.DeviceModel);
		writer.WriteInt64((long)header.DeviceAttributes);
		writer.WriteUInt32((uint)header.RenderingIntent);
		writer.WriteXyzNumber(header.PcsIlluminant);
		writer.WriteAsciiString(header.CreatorSignature, 4, ensureNullTerminator: false);
		writer.WriteProfileId(IccProfile.CalculateHash(writer.GetData()));
	}

	private static void WriteTagTable(IccDataWriter writer, IccTagTableEntry[] table)
	{
		writer.SetIndex(128);
		writer.WriteUInt32((uint)table.Length);
		for (int i = 0; i < table.Length; i++)
		{
			IccTagTableEntry iccTagTableEntry = table[i];
			writer.WriteUInt32((uint)iccTagTableEntry.Signature);
			writer.WriteUInt32(iccTagTableEntry.Offset);
			writer.WriteUInt32(iccTagTableEntry.DataSize);
		}
	}

	private static IccTagTableEntry[] WriteTagData(IccDataWriter writer, IccTagDataEntry[] entries)
	{
		IEnumerable<IGrouping<IccTagDataEntry, IccTagDataEntry>> enumerable = from t in entries
			group t by t;
		writer.SetIndex(132 + entries.Length * 12);
		List<IccTagTableEntry> list = new List<IccTagTableEntry>();
		foreach (IGrouping<IccTagDataEntry, IccTagDataEntry> item in enumerable)
		{
			writer.WriteTagDataEntry(item.Key, out var table);
			foreach (IccTagDataEntry item2 in item)
			{
				list.Add(new IccTagTableEntry(item2.TagSignature, table.Offset, table.DataSize));
			}
		}
		return list.ToArray();
	}
}
