using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Extra.Decompressors.LZ4;
using LZ4ps;
using SevenZip.Compression.LZMA;

namespace AssetsTools.NET;

public class AssetBundleFile
{
	public AssetsFileReader Reader;

	/// <summary>
	/// Bundle header. Contains bundle engine version.
	/// </summary>
	public AssetBundleHeader Header { get; set; }

	/// <summary>
	/// List of compression blocks and file info (file names, address in file, etc.)
	/// </summary>
	public AssetBundleBlockAndDirInfo BlockAndDirInfo { get; set; }

	/// <summary>
	/// Reader for data block of bundle
	/// </summary>
	public AssetsFileReader DataReader { get; set; }

	/// <summary>
	/// Is data reader reading compressed data? Only LZMA bundles set this to true.
	/// </summary>
	public bool DataIsCompressed { get; set; }

	public void Close()
	{
		Reader.Close();
		DataReader.Close();
	}

	public void Read(AssetsFileReader reader)
	{
		Reader = reader;
		Reader.Position = 0L;
		Reader.BigEndian = true;
		string text = reader.ReadNullTerminated();
		uint num = reader.ReadUInt32();
		if (num >= 6 || num <= 8)
		{
			Reader.Position = 0L;
			Header = new AssetBundleHeader();
			Header.Read(reader);
			if (Header.Version >= 7)
			{
				reader.Align16();
			}
			if (Header.Signature == "UnityFS")
			{
				UnpackInfoOnly();
			}
			else
			{
				new NotImplementedException("Non UnityFS bundles are not supported yet.");
			}
		}
		else
		{
			new NotImplementedException($"Version {num} bundles are not supported yet.");
		}
	}

	public void Write(AssetsFileWriter writer, List<BundleReplacer> replacers, ClassDatabaseFile typeMeta = null)
	{
		if (Header == null)
		{
			throw new Exception("Header must be loaded! (Did you forget to call bundle.Read?)");
		}
		if (Header.Signature != "UnityFS")
		{
			throw new NotImplementedException("Non UnityFS bundles are not supported yet.");
		}
		if (DataIsCompressed)
		{
			throw new Exception("Bundles must be decompressed before writing.");
		}
		writer.Position = 0L;
		AssetBundleDirectoryInfo[] directoryInfos = BlockAndDirInfo.DirectoryInfos;
		Header.Write(writer);
		if (Header.Version >= 7)
		{
			writer.Align16();
		}
		AssetBundleBlockInfo assetBundleBlockInfo = new AssetBundleBlockInfo
		{
			CompressedSize = 0u,
			DecompressedSize = 0u,
			Flags = 64
		};
		AssetBundleBlockAndDirInfo assetBundleBlockAndDirInfo = new AssetBundleBlockAndDirInfo();
		assetBundleBlockAndDirInfo.Hash = default(Hash128);
		assetBundleBlockAndDirInfo.BlockInfos = new AssetBundleBlockInfo[1] { assetBundleBlockInfo };
		AssetBundleBlockAndDirInfo assetBundleBlockAndDirInfo2 = assetBundleBlockAndDirInfo;
		Dictionary<AssetBundleDirectoryInfo, AssetBundleDirectoryInfo> dictionary = new Dictionary<AssetBundleDirectoryInfo, AssetBundleDirectoryInfo>();
		List<AssetBundleDirectoryInfo> list = new List<AssetBundleDirectoryInfo>();
		List<AssetBundleDirectoryInfo> list2 = new List<AssetBundleDirectoryInfo>();
		List<BundleReplacer> list3 = replacers.ToList();
		foreach (AssetBundleDirectoryInfo assetBundleDirectoryInfo in directoryInfos)
		{
			list.Add(assetBundleDirectoryInfo);
			AssetBundleDirectoryInfo newInfo = new AssetBundleDirectoryInfo
			{
				Offset = 0L,
				DecompressedSize = 0L,
				Flags = assetBundleDirectoryInfo.Flags,
				Name = assetBundleDirectoryInfo.Name
			};
			BundleReplacer bundleReplacer = list3.FirstOrDefault((BundleReplacer rep) => rep.GetOriginalEntryName() == newInfo.Name);
			if (bundleReplacer != null)
			{
				if (!bundleReplacer.Init(DataReader, assetBundleDirectoryInfo.Offset, assetBundleDirectoryInfo.DecompressedSize, typeMeta))
				{
					throw new Exception("Something went wrong initializing a replacer!");
				}
				list3.Remove(bundleReplacer);
				if (bundleReplacer.GetReplacementType() == BundleReplacementType.AddOrModify)
				{
					newInfo = new AssetBundleDirectoryInfo
					{
						Offset = 0L,
						DecompressedSize = 0L,
						Flags = assetBundleDirectoryInfo.Flags,
						Name = bundleReplacer.GetEntryName()
					};
				}
				else if (bundleReplacer.GetReplacementType() == BundleReplacementType.Rename)
				{
					newInfo = new AssetBundleDirectoryInfo
					{
						Offset = 0L,
						DecompressedSize = 0L,
						Flags = assetBundleDirectoryInfo.Flags,
						Name = bundleReplacer.GetEntryName()
					};
					dictionary[newInfo] = assetBundleDirectoryInfo;
				}
				else if (bundleReplacer.GetReplacementType() == BundleReplacementType.Remove)
				{
					continue;
				}
			}
			else
			{
				dictionary[newInfo] = assetBundleDirectoryInfo;
			}
			list2.Add(newInfo);
		}
		while (list3.Count > 0)
		{
			BundleReplacer bundleReplacer2 = list3[0];
			if (bundleReplacer2.GetReplacementType() == BundleReplacementType.AddOrModify)
			{
				AssetBundleDirectoryInfo item = new AssetBundleDirectoryInfo
				{
					Offset = 0L,
					DecompressedSize = 0L,
					Flags = (bundleReplacer2.HasSerializedData() ? 4u : 0u),
					Name = bundleReplacer2.GetEntryName()
				};
				list2.Add(item);
			}
			list3.Remove(bundleReplacer2);
		}
		long position = writer.Position;
		assetBundleBlockAndDirInfo2.DirectoryInfos = list2.ToArray();
		assetBundleBlockAndDirInfo2.Write(writer);
		if ((Header.FileStreamHeader.Flags & AssetBundleFSHeaderFlags.BlockInfoNeedPaddingAtStart) != 0)
		{
			writer.Align16();
		}
		long position2 = writer.Position;
		for (int j = 0; j < list2.Count; j++)
		{
			AssetBundleDirectoryInfo info = list2[j];
			BundleReplacer bundleReplacer3 = replacers.FirstOrDefault((BundleReplacer n) => n.GetEntryName() == info.Name);
			AssetBundleDirectoryInfo value;
			if (bundleReplacer3 != null && bundleReplacer3.GetReplacementType() != BundleReplacementType.Rename)
			{
				if (bundleReplacer3.GetReplacementType() == BundleReplacementType.AddOrModify)
				{
					long position3 = writer.Position;
					long num = bundleReplacer3.Write(writer);
					long decompressedSize = num - position3;
					list2[j].DecompressedSize = decompressedSize;
					list2[j].Offset = position3 - position2;
				}
				else if (bundleReplacer3.GetReplacementType() != BundleReplacementType.Remove)
				{
				}
			}
			else if (dictionary.TryGetValue(info, out value))
			{
				long position4 = writer.Position;
				DataReader.Position = value.Offset;
				DataReader.BaseStream.CopyToCompat(writer.BaseStream, value.DecompressedSize);
				list2[j].DecompressedSize = value.DecompressedSize;
				list2[j].Offset = position4 - position2;
			}
		}
		long position5 = writer.Position;
		uint num2 = (uint)(position5 - position2);
		writer.Position = position;
		assetBundleBlockInfo.DecompressedSize = num2;
		assetBundleBlockInfo.CompressedSize = num2;
		assetBundleBlockAndDirInfo2.DirectoryInfos = list2.ToArray();
		assetBundleBlockAndDirInfo2.Write(writer);
		uint num3 = (uint)(position2 - position);
		writer.Position = 0L;
		AssetBundleHeader assetBundleHeader = new AssetBundleHeader
		{
			Signature = Header.Signature,
			Version = Header.Version,
			GenerationVersion = Header.GenerationVersion,
			EngineVersion = Header.EngineVersion,
			FileStreamHeader = new AssetBundleFSHeader
			{
				TotalFileSize = position5,
				CompressedSize = num3,
				DecompressedSize = num3,
				Flags = (Header.FileStreamHeader.Flags & (AssetBundleFSHeaderFlags)(-129) & (AssetBundleFSHeaderFlags)(-64))
			}
		};
		assetBundleHeader.Write(writer);
	}

	public void Unpack(AssetsFileWriter writer)
	{
		if (Header == null)
		{
			new Exception("Header must be loaded! (Did you forget to call bundle.Read?)");
		}
		if (Header.Signature != "UnityFS")
		{
			new NotImplementedException("Non UnityFS bundles are not supported yet.");
		}
		AssetBundleFSHeader fileStreamHeader = Header.FileStreamHeader;
		AssetsFileReader dataReader = DataReader;
		AssetBundleBlockInfo[] blockInfos = BlockAndDirInfo.BlockInfos;
		AssetBundleDirectoryInfo[] directoryInfos = BlockAndDirInfo.DirectoryInfos;
		AssetBundleHeader assetBundleHeader = new AssetBundleHeader
		{
			Signature = Header.Signature,
			Version = Header.Version,
			GenerationVersion = Header.GenerationVersion,
			EngineVersion = Header.EngineVersion,
			FileStreamHeader = new AssetBundleFSHeader
			{
				TotalFileSize = 0L,
				CompressedSize = fileStreamHeader.DecompressedSize,
				DecompressedSize = fileStreamHeader.DecompressedSize,
				Flags = (AssetBundleFSHeaderFlags)(0x40 | (((fileStreamHeader.Flags & AssetBundleFSHeaderFlags.BlockInfoNeedPaddingAtStart) != 0) ? 512 : 0))
			}
		};
		long num = assetBundleHeader.GetFileDataOffset();
		for (int i = 0; i < blockInfos.Length; i++)
		{
			num += blockInfos[i].DecompressedSize;
		}
		assetBundleHeader.FileStreamHeader.TotalFileSize = num;
		AssetBundleBlockAndDirInfo assetBundleBlockAndDirInfo = new AssetBundleBlockAndDirInfo();
		assetBundleBlockAndDirInfo.Hash = default(Hash128);
		assetBundleBlockAndDirInfo.BlockInfos = new AssetBundleBlockInfo[blockInfos.Length];
		assetBundleBlockAndDirInfo.DirectoryInfos = new AssetBundleDirectoryInfo[directoryInfos.Length];
		AssetBundleBlockAndDirInfo assetBundleBlockAndDirInfo2 = assetBundleBlockAndDirInfo;
		for (int j = 0; j < assetBundleBlockAndDirInfo2.BlockInfos.Length; j++)
		{
			assetBundleBlockAndDirInfo2.BlockInfos[j] = new AssetBundleBlockInfo
			{
				CompressedSize = blockInfos[j].DecompressedSize,
				DecompressedSize = blockInfos[j].DecompressedSize,
				Flags = (ushort)(blockInfos[j].Flags & -64)
			};
		}
		for (int k = 0; k < assetBundleBlockAndDirInfo2.DirectoryInfos.Length; k++)
		{
			assetBundleBlockAndDirInfo2.DirectoryInfos[k] = new AssetBundleDirectoryInfo
			{
				Offset = directoryInfos[k].Offset,
				DecompressedSize = directoryInfos[k].DecompressedSize,
				Flags = directoryInfos[k].Flags,
				Name = directoryInfos[k].Name
			};
		}
		assetBundleHeader.Write(writer);
		if (assetBundleHeader.Version >= 7)
		{
			writer.Align16();
		}
		assetBundleBlockAndDirInfo2.Write(writer);
		if ((assetBundleHeader.FileStreamHeader.Flags & AssetBundleFSHeaderFlags.BlockInfoNeedPaddingAtStart) != 0)
		{
			writer.Align16();
		}
		dataReader.Position = 0L;
		if (DataIsCompressed)
		{
			for (int l = 0; l < assetBundleBlockAndDirInfo2.BlockInfos.Length; l++)
			{
				AssetBundleBlockInfo assetBundleBlockInfo = blockInfos[l];
				switch (assetBundleBlockInfo.GetCompressionType())
				{
				case 0:
					dataReader.BaseStream.CopyToCompat(writer.BaseStream, assetBundleBlockInfo.CompressedSize);
					break;
				case 1:
					SevenZipHelper.StreamDecompress(dataReader.BaseStream, writer.BaseStream, assetBundleBlockInfo.CompressedSize, assetBundleBlockInfo.DecompressedSize);
					break;
				case 2:
				case 3:
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						dataReader.BaseStream.CopyToCompat(memoryStream, assetBundleBlockInfo.CompressedSize);
						memoryStream.Position = 0L;
						using Lz4DecoderStream input = new Lz4DecoderStream(memoryStream);
						input.CopyToCompat(writer.BaseStream, assetBundleBlockInfo.DecompressedSize);
					}
					break;
				}
				}
			}
		}
		else
		{
			for (int m = 0; m < assetBundleBlockAndDirInfo2.BlockInfos.Length; m++)
			{
				AssetBundleBlockInfo assetBundleBlockInfo2 = blockInfos[m];
				dataReader.BaseStream.CopyToCompat(writer.BaseStream, assetBundleBlockInfo2.DecompressedSize);
			}
		}
	}

	public void Pack(AssetsFileReader reader, AssetsFileWriter writer, AssetBundleCompressionType compType, bool blockDirAtEnd = true, IAssetBundleCompressProgress progress = null)
	{
		if (Header == null)
		{
			throw new Exception("Header must be loaded! (Did you forget to call bundle.Read?)");
		}
		if (Header.Signature != "UnityFS")
		{
			throw new NotImplementedException("Non UnityFS bundles are not supported yet.");
		}
		if (DataIsCompressed)
		{
			throw new Exception("Bundles must be decompressed before writing.");
		}
		reader.Position = 0L;
		writer.Position = 0L;
		AssetBundleFSHeader assetBundleFSHeader = new AssetBundleFSHeader
		{
			TotalFileSize = 0L,
			CompressedSize = 0u,
			DecompressedSize = 0u,
			Flags = (AssetBundleFSHeaderFlags)(0x43 | (blockDirAtEnd ? 128 : 0))
		};
		AssetBundleHeader assetBundleHeader = new AssetBundleHeader
		{
			Signature = Header.Signature,
			Version = Header.Version,
			GenerationVersion = Header.GenerationVersion,
			EngineVersion = Header.EngineVersion,
			FileStreamHeader = assetBundleFSHeader
		};
		AssetBundleBlockAndDirInfo assetBundleBlockAndDirInfo = new AssetBundleBlockAndDirInfo
		{
			Hash = default(Hash128),
			BlockInfos = null,
			DirectoryInfos = BlockAndDirInfo.DirectoryInfos
		};
		long position = writer.Position;
		assetBundleHeader.Write(writer);
		if (assetBundleHeader.Version >= 7)
		{
			writer.Align16();
		}
		int num = (int)(writer.Position - position);
		long num2 = 0L;
		List<AssetBundleBlockInfo> list = new List<AssetBundleBlockInfo>();
		List<Stream> list2 = new List<Stream>();
		Stream baseStream = DataReader.BaseStream;
		baseStream.Position = 0L;
		int num3 = (int)baseStream.Length;
		switch (compType)
		{
		case AssetBundleCompressionType.LZMA:
		{
			Stream stream2 = ((!blockDirAtEnd) ? GetTempFileStream() : writer.BaseStream);
			AssetBundleLZMAProgress progress2 = new AssetBundleLZMAProgress(progress, baseStream.Length);
			long position2 = stream2.Position;
			SevenZipHelper.Compress(baseStream, stream2, progress2);
			uint compressedSize = (uint)(stream2.Position - position2);
			AssetBundleBlockInfo assetBundleBlockInfo4 = new AssetBundleBlockInfo
			{
				CompressedSize = compressedSize,
				DecompressedSize = (uint)num3,
				Flags = 65
			};
			num2 += assetBundleBlockInfo4.CompressedSize;
			list.Add(assetBundleBlockInfo4);
			if (!blockDirAtEnd)
			{
				list2.Add(stream2);
			}
			progress?.SetProgress(1f);
			break;
		}
		case AssetBundleCompressionType.LZ4:
		{
			BinaryReader binaryReader = new BinaryReader(baseStream);
			Stream stream = ((!blockDirAtEnd) ? GetTempFileStream() : writer.BaseStream);
			byte[] array = binaryReader.ReadBytes(131072);
			while (array.Length != 0)
			{
				byte[] array2 = LZ4Codec.Encode32HC(array, 0, array.Length);
				progress?.SetProgress((float)binaryReader.BaseStream.Position / (float)binaryReader.BaseStream.Length);
				if (array2.Length > array.Length)
				{
					stream.Write(array, 0, array.Length);
					AssetBundleBlockInfo assetBundleBlockInfo2 = new AssetBundleBlockInfo
					{
						CompressedSize = (uint)array.Length,
						DecompressedSize = (uint)array.Length,
						Flags = 0
					};
					num2 += assetBundleBlockInfo2.CompressedSize;
					list.Add(assetBundleBlockInfo2);
				}
				else
				{
					stream.Write(array2, 0, array2.Length);
					AssetBundleBlockInfo assetBundleBlockInfo3 = new AssetBundleBlockInfo
					{
						CompressedSize = (uint)array2.Length,
						DecompressedSize = (uint)array.Length,
						Flags = 3
					};
					num2 += assetBundleBlockInfo3.CompressedSize;
					list.Add(assetBundleBlockInfo3);
				}
				array = binaryReader.ReadBytes(131072);
			}
			if (!blockDirAtEnd)
			{
				list2.Add(stream);
			}
			progress?.SetProgress(1f);
			break;
		}
		case AssetBundleCompressionType.None:
		{
			AssetBundleBlockInfo assetBundleBlockInfo = new AssetBundleBlockInfo
			{
				CompressedSize = (uint)num3,
				DecompressedSize = (uint)num3,
				Flags = 0
			};
			num2 += assetBundleBlockInfo.CompressedSize;
			list.Add(assetBundleBlockInfo);
			if (blockDirAtEnd)
			{
				baseStream.CopyToCompat(writer.BaseStream, -1L);
			}
			else
			{
				list2.Add(baseStream);
			}
			break;
		}
		}
		assetBundleBlockAndDirInfo.BlockInfos = list.ToArray();
		byte[] array3;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			AssetsFileWriter assetsFileWriter = new AssetsFileWriter(memoryStream);
			assetsFileWriter.BigEndian = writer.BigEndian;
			assetBundleBlockAndDirInfo.Write(assetsFileWriter);
			array3 = memoryStream.ToArray();
		}
		byte[] array4 = LZ4Codec.Encode32HC(array3, 0, array3.Length);
		long totalFileSize = num + array4.Length + num2;
		assetBundleFSHeader.TotalFileSize = totalFileSize;
		assetBundleFSHeader.DecompressedSize = (uint)array3.Length;
		assetBundleFSHeader.CompressedSize = (uint)array4.Length;
		if (!blockDirAtEnd)
		{
			writer.Write(array4);
			foreach (Stream item in list2)
			{
				item.Position = 0L;
				item.CopyToCompat(writer.BaseStream, -1L);
				item.Close();
			}
		}
		else
		{
			writer.Write(array4);
		}
		writer.Position = 0L;
		assetBundleHeader.Write(writer);
		if (assetBundleHeader.Version >= 7)
		{
			writer.Align16();
		}
	}

	public void UnpackInfoOnly()
	{
		Reader.Position = Header.GetBundleInfoOffset();
		if (Header.GetCompressionType() == 0)
		{
			BlockAndDirInfo = new AssetBundleBlockAndDirInfo();
			BlockAndDirInfo.Read(Reader);
		}
		else
		{
			int compressedSize = (int)Header.FileStreamHeader.CompressedSize;
			int decompressedSize = (int)Header.FileStreamHeader.DecompressedSize;
			MemoryStream memoryStream;
			switch (Header.GetCompressionType())
			{
			case 1:
			{
				using (MemoryStream compressedStream = new MemoryStream(Reader.ReadBytes(compressedSize)))
				{
					memoryStream = new MemoryStream();
					SevenZipHelper.StreamDecompress(compressedStream, memoryStream, compressedSize, decompressedSize);
				}
				break;
			}
			case 2:
			case 3:
			{
				byte[] buffer = new byte[Header.FileStreamHeader.DecompressedSize];
				using (MemoryStream input = new MemoryStream(Reader.ReadBytes(compressedSize)))
				{
					Lz4DecoderStream lz4DecoderStream = new Lz4DecoderStream(input);
					lz4DecoderStream.Read(buffer, 0, (int)Header.FileStreamHeader.DecompressedSize);
					lz4DecoderStream.Dispose();
				}
				memoryStream = new MemoryStream(buffer);
				break;
			}
			default:
				memoryStream = null;
				break;
			}
			AssetsFileReader assetsFileReader;
			using (assetsFileReader = new AssetsFileReader(memoryStream))
			{
				assetsFileReader.Position = 0L;
				assetsFileReader.BigEndian = Reader.BigEndian;
				BlockAndDirInfo = new AssetBundleBlockAndDirInfo();
				BlockAndDirInfo.Read(assetsFileReader);
			}
		}
		switch (GetCompressionType(BlockAndDirInfo.BlockInfos))
		{
		case AssetBundleCompressionType.None:
		{
			SegmentStream stream3 = new SegmentStream(Reader.BaseStream, Header.GetFileDataOffset());
			DataReader = new AssetsFileReader(stream3);
			DataIsCompressed = false;
			break;
		}
		case AssetBundleCompressionType.LZMA:
		{
			SegmentStream stream2 = new SegmentStream(Reader.BaseStream, Header.GetFileDataOffset());
			DataReader = new AssetsFileReader(stream2);
			DataIsCompressed = true;
			break;
		}
		case AssetBundleCompressionType.LZ4:
		{
			LZ4BlockStream stream = new LZ4BlockStream(Reader.BaseStream, Header.GetFileDataOffset(), BlockAndDirInfo.BlockInfos);
			DataReader = new AssetsFileReader(stream);
			DataIsCompressed = false;
			break;
		}
		}
	}

	public AssetBundleCompressionType GetCompressionType(AssetBundleBlockInfo[] blockInfos)
	{
		for (int i = 0; i < blockInfos.Length; i++)
		{
			byte compressionType = blockInfos[i].GetCompressionType();
			if (compressionType == 2 || compressionType == 3)
			{
				return AssetBundleCompressionType.LZ4;
			}
			if (compressionType == 1)
			{
				return AssetBundleCompressionType.LZMA;
			}
		}
		return AssetBundleCompressionType.None;
	}

	public bool IsAssetsFile(int index)
	{
		GetFileRange(index, out var offset, out var length);
		return AssetsFile.IsAssetsFile(DataReader, offset, length);
	}

	public int GetFileIndex(string name)
	{
		if (Header == null)
		{
			throw new Exception("Header must be loaded! (Did you forget to call bundle.Read?)");
		}
		for (int i = 0; i < BlockAndDirInfo.DirectoryInfos.Length; i++)
		{
			if (BlockAndDirInfo.DirectoryInfos[i].Name == name)
			{
				return i;
			}
		}
		return -1;
	}

	public string GetFileName(int index)
	{
		if (Header == null)
		{
			throw new Exception("Header must be loaded! (Did you forget to call bundle.Read?)");
		}
		return BlockAndDirInfo.DirectoryInfos[index].Name;
	}

	public void GetFileRange(int index, out long offset, out long length)
	{
		if (Header == null)
		{
			throw new Exception("Header must be loaded! (Did you forget to call bundle.Read?)");
		}
		AssetBundleDirectoryInfo assetBundleDirectoryInfo = BlockAndDirInfo.DirectoryInfos[index];
		offset = assetBundleDirectoryInfo.Offset;
		length = assetBundleDirectoryInfo.DecompressedSize;
	}

	public List<string> GetAllFileNames()
	{
		List<string> list = new List<string>();
		AssetBundleDirectoryInfo[] directoryInfos = BlockAndDirInfo.DirectoryInfos;
		AssetBundleDirectoryInfo[] array = directoryInfos;
		foreach (AssetBundleDirectoryInfo assetBundleDirectoryInfo in array)
		{
			list.Add(assetBundleDirectoryInfo.Name);
		}
		return list;
	}

	private FileStream GetTempFileStream()
	{
		string tempFileName = Path.GetTempFileName();
		return new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose);
	}
}
