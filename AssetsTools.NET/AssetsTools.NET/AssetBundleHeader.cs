using System;

namespace AssetsTools.NET;

public class AssetBundleHeader
{
	/// <summary>
	/// Magic appearing at the beginning of all bundles. Possible options are:
	/// UnityFS, UnityWeb, UnityRaw, UnityArchive
	/// </summary>
	public string Signature { get; set; }

	/// <summary>
	/// Version of this file.
	/// </summary>
	public uint Version { get; set; }

	/// <summary>
	/// Generation version string. For Unity 5 bundles this is always "5.x.x"
	/// </summary>
	public string GenerationVersion { get; set; }

	/// <summary>
	/// Engine version. This is the specific version string being used. For example, "2019.4.2f1"
	/// </summary>
	public string EngineVersion { get; set; }

	/// <summary>
	/// Header for bundles with a UnityFS Signature.
	/// </summary>
	public AssetBundleFSHeader FileStreamHeader { get; set; }

	public void Read(AssetsFileReader reader)
	{
		reader.BigEndian = true;
		Signature = reader.ReadNullTerminated();
		Version = reader.ReadUInt32();
		GenerationVersion = reader.ReadNullTerminated();
		EngineVersion = reader.ReadNullTerminated();
		if (Signature == "UnityFS")
		{
			FileStreamHeader = new AssetBundleFSHeader();
			FileStreamHeader.Read(reader);
			return;
		}
		throw new NotSupportedException(Signature + " signature not supported!");
	}

	public void Write(AssetsFileWriter writer)
	{
		writer.BigEndian = true;
		writer.WriteNullTerminated(Signature);
		writer.Write(Version);
		writer.WriteNullTerminated(GenerationVersion);
		writer.WriteNullTerminated(EngineVersion);
		if (Signature == "UnityFS")
		{
			FileStreamHeader.Write(writer);
			return;
		}
		throw new NotSupportedException(Signature + " signature not supported!");
	}

	public long GetBundleInfoOffset()
	{
		if (Signature != "UnityFS")
		{
			throw new NotSupportedException(Signature + " signature not supported!");
		}
		AssetBundleFSHeaderFlags flags = FileStreamHeader.Flags;
		long totalFileSize = FileStreamHeader.TotalFileSize;
		long num = FileStreamHeader.CompressedSize;
		if ((flags & AssetBundleFSHeaderFlags.BlockAndDirAtEnd) != 0)
		{
			if (totalFileSize == 0)
			{
				return -1L;
			}
			return totalFileSize - num;
		}
		long num2 = GenerationVersion.Length + EngineVersion.Length + 26;
		if (Version >= 7)
		{
			if ((flags & AssetBundleFSHeaderFlags.OldWebPluginCompatibility) != 0)
			{
				return (num2 + 10 + 15) & -16;
			}
			return (num2 + Signature.Length + 1 + 15) & -16;
		}
		if ((flags & AssetBundleFSHeaderFlags.OldWebPluginCompatibility) != 0)
		{
			return num2 + 10;
		}
		return num2 + Signature.Length + 1;
	}

	public long GetFileDataOffset()
	{
		if (Signature != "UnityFS")
		{
			throw new NotSupportedException(Signature + " signature not supported!");
		}
		AssetBundleFSHeaderFlags flags = FileStreamHeader.Flags;
		long num = FileStreamHeader.CompressedSize;
		long num2 = GenerationVersion.Length + EngineVersion.Length + 26;
		num2 = (((flags & AssetBundleFSHeaderFlags.OldWebPluginCompatibility) == 0) ? (num2 + (Signature.Length + 1)) : (num2 + 10));
		if (Version >= 7)
		{
			num2 = (num2 + 15) & -16;
		}
		if ((flags & AssetBundleFSHeaderFlags.BlockAndDirAtEnd) == 0)
		{
			num2 += num;
		}
		if ((flags & AssetBundleFSHeaderFlags.BlockInfoNeedPaddingAtStart) != 0)
		{
			num2 = (num2 + 15) & -16;
		}
		return num2;
	}

	public byte GetCompressionType()
	{
		if (Signature != "UnityFS")
		{
			throw new NotSupportedException(Signature + " signature not supported!");
		}
		return (byte)(FileStreamHeader.Flags & AssetBundleFSHeaderFlags.CompressionMask);
	}
}
