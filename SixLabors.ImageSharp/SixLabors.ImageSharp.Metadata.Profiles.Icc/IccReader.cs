using System;
using System.Collections.Generic;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccReader
{
	public static IccProfile Read(byte[] data)
	{
		Guard.NotNull(data, "data");
		Guard.IsTrue(data.Length >= 128, "data", "Data length must be at least 128 to be a valid ICC profile");
		IccDataReader reader = new IccDataReader(data);
		IccProfileHeader header = ReadHeader(reader);
		IccTagDataEntry[] entries = ReadTagData(reader);
		return new IccProfile(header, entries);
	}

	public static IccProfileHeader ReadHeader(byte[] data)
	{
		Guard.NotNull(data, "data");
		Guard.IsTrue(data.Length >= 128, "data", "Data length must be at least 128 to be a valid profile header");
		return ReadHeader(new IccDataReader(data));
	}

	public static IccTagDataEntry[] ReadTagData(byte[] data)
	{
		Guard.NotNull(data, "data");
		Guard.IsTrue(data.Length >= 128, "data", "Data length must be at least 128 to be a valid ICC profile");
		return ReadTagData(new IccDataReader(data));
	}

	private static IccProfileHeader ReadHeader(IccDataReader reader)
	{
		reader.SetIndex(0);
		return new IccProfileHeader
		{
			Size = reader.ReadUInt32(),
			CmmType = reader.ReadAsciiString(4),
			Version = reader.ReadVersionNumber(),
			Class = (IccProfileClass)reader.ReadUInt32(),
			DataColorSpace = (IccColorSpaceType)reader.ReadUInt32(),
			ProfileConnectionSpace = (IccColorSpaceType)reader.ReadUInt32(),
			CreationDate = reader.ReadDateTime(),
			FileSignature = reader.ReadAsciiString(4),
			PrimaryPlatformSignature = (IccPrimaryPlatformType)reader.ReadUInt32(),
			Flags = (IccProfileFlag)reader.ReadInt32(),
			DeviceManufacturer = reader.ReadUInt32(),
			DeviceModel = reader.ReadUInt32(),
			DeviceAttributes = (IccDeviceAttribute)reader.ReadInt64(),
			RenderingIntent = (IccRenderingIntent)reader.ReadUInt32(),
			PcsIlluminant = reader.ReadXyzNumber(),
			CreatorSignature = reader.ReadAsciiString(4),
			Id = reader.ReadProfileId()
		};
	}

	private static IccTagDataEntry[] ReadTagData(IccDataReader reader)
	{
		IccTagTableEntry[] array = ReadTagTable(reader);
		List<IccTagDataEntry> list = new List<IccTagDataEntry>(array.Length);
		Dictionary<uint, IccTagDataEntry> dictionary = new Dictionary<uint, IccTagDataEntry>();
		IccTagTableEntry[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			IccTagTableEntry info = array2[i];
			IccTagDataEntry iccTagDataEntry;
			if (dictionary.TryGetValue(info.Offset, out var value))
			{
				iccTagDataEntry = value;
			}
			else
			{
				try
				{
					iccTagDataEntry = reader.ReadTagDataEntry(info);
				}
				catch
				{
					continue;
				}
				dictionary.Add(info.Offset, iccTagDataEntry);
			}
			iccTagDataEntry.TagSignature = info.Signature;
			list.Add(iccTagDataEntry);
		}
		return list.ToArray();
	}

	private static IccTagTableEntry[] ReadTagTable(IccDataReader reader)
	{
		reader.SetIndex(128);
		uint num = reader.ReadUInt32();
		if (num > 100)
		{
			return Array.Empty<IccTagTableEntry>();
		}
		List<IccTagTableEntry> list = new List<IccTagTableEntry>((int)num);
		for (int i = 0; i < num; i++)
		{
			uint signature = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			uint num3 = reader.ReadUInt32();
			if (num2 < reader.DataLength && num3 < reader.DataLength - 128)
			{
				list.Add(new IccTagTableEntry((IccProfileTag)signature, num2, num3));
			}
		}
		return list.ToArray();
	}
}
