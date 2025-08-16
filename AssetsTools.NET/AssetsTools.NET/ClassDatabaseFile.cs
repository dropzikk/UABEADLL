using System;
using System.Collections.Generic;
using System.IO;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Extra.Decompressors.LZ4;
using LZ4ps;
using SevenZip.Compression.LZMA;

namespace AssetsTools.NET;

public class ClassDatabaseFile
{
	public ClassDatabaseFileHeader Header { get; set; }

	public List<ClassDatabaseType> Classes { get; set; }

	public ClassDatabaseStringTable StringTable { get; set; }

	public List<ushort> CommonStringBufferIndices { get; set; }

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.ClassDatabaseFile" /> with the provided reader.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	public void Read(AssetsFileReader reader)
	{
		if (Header == null)
		{
			ClassDatabaseFileHeader classDatabaseFileHeader2 = (Header = new ClassDatabaseFileHeader());
		}
		Header.Read(reader);
		AssetsFileReader decompressedReader = GetDecompressedReader(reader);
		int num = decompressedReader.ReadInt32();
		Classes = new List<ClassDatabaseType>(num);
		for (int i = 0; i < num; i++)
		{
			ClassDatabaseType classDatabaseType = new ClassDatabaseType();
			classDatabaseType.Read(decompressedReader);
			Classes.Add(classDatabaseType);
		}
		if (StringTable == null)
		{
			ClassDatabaseStringTable classDatabaseStringTable2 = (StringTable = new ClassDatabaseStringTable());
		}
		StringTable.Read(decompressedReader);
		if (CommonStringBufferIndices == null)
		{
			List<ushort> list2 = (CommonStringBufferIndices = new List<ushort>());
		}
		int num2 = decompressedReader.ReadInt32();
		for (int j = 0; j < num2; j++)
		{
			CommonStringBufferIndices.Add(decompressedReader.ReadUInt16());
		}
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.ClassDatabaseFile" /> with the provided writer and compression type.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	/// <param name="compressionType">The compression method to use.</param>
	public void Write(AssetsFileWriter writer, ClassFileCompressionType compressionType)
	{
		Header.CompressionType = compressionType;
		MemoryStream memoryStream = new MemoryStream();
		AssetsFileWriter assetsFileWriter = new AssetsFileWriter(memoryStream);
		assetsFileWriter.Write(Classes.Count);
		for (int i = 0; i < Classes.Count; i++)
		{
			Classes[i].Write(assetsFileWriter);
		}
		StringTable.Write(assetsFileWriter);
		assetsFileWriter.Write(CommonStringBufferIndices.Count);
		for (int j = 0; j < CommonStringBufferIndices.Count; j++)
		{
			assetsFileWriter.Write(CommonStringBufferIndices[j]);
		}
		using MemoryStream memoryStream2 = GetCompressedStream(memoryStream);
		Header.CompressedSize = (int)memoryStream2.Length;
		Header.DecompressedSize = (int)memoryStream.Length;
		Header.Write(writer);
		memoryStream2.CopyToCompat(writer.BaseStream, -1L);
	}

	private AssetsFileReader GetDecompressedReader(AssetsFileReader reader)
	{
		AssetsFileReader result = reader;
		if (Header.CompressionType != 0)
		{
			MemoryStream stream;
			if (Header.CompressionType == ClassFileCompressionType.Lz4)
			{
				byte[] buffer = new byte[Header.DecompressedSize];
				using (MemoryStream input = new MemoryStream(reader.ReadBytes(Header.CompressedSize)))
				{
					Lz4DecoderStream lz4DecoderStream = new Lz4DecoderStream(input);
					lz4DecoderStream.Read(buffer, 0, Header.DecompressedSize);
					lz4DecoderStream.Dispose();
				}
				stream = new MemoryStream(buffer);
			}
			else
			{
				if (Header.CompressionType != ClassFileCompressionType.Lzma)
				{
					throw new Exception($"Class database is using invalid compression type {Header.CompressionType}!");
				}
				using MemoryStream newInStream = new MemoryStream(reader.ReadBytes(Header.CompressedSize));
				stream = SevenZipHelper.StreamDecompress(newInStream);
			}
			result = new AssetsFileReader(stream);
		}
		return result;
	}

	private MemoryStream GetCompressedStream(MemoryStream inStream)
	{
		if (Header.CompressionType != 0)
		{
			if (Header.CompressionType == ClassFileCompressionType.Lz4)
			{
				byte[] buffer = LZ4Codec.Encode32HC(inStream.ToArray(), 0, (int)inStream.Length);
				return new MemoryStream(buffer);
			}
			if (Header.CompressionType == ClassFileCompressionType.Lzma)
			{
				MemoryStream memoryStream = new MemoryStream();
				SevenZipHelper.Compress(inStream, memoryStream);
				memoryStream.Position = 0L;
				return memoryStream;
			}
			throw new Exception($"Class database is using invalid compression type {Header.CompressionType}!");
		}
		inStream.Position = 0L;
		return inStream;
	}

	/// <summary>
	/// Find a class database type by type ID.
	/// </summary>
	/// <param name="id">The type's type ID to search for.</param>
	/// <returns>The type of that type ID.</returns>
	public ClassDatabaseType FindAssetClassByID(int id)
	{
		if (id < 0)
		{
			id = 114;
		}
		foreach (ClassDatabaseType @class in Classes)
		{
			if (@class.ClassId == id)
			{
				return @class;
			}
		}
		return null;
	}

	/// <summary>
	/// Find a class database type by type name.
	/// </summary>
	/// <param name="name">The type's type name to search for.</param>
	/// <returns>The type of that type name.</returns>
	public ClassDatabaseType FindAssetClassByName(string name)
	{
		foreach (ClassDatabaseType @class in Classes)
		{
			if (GetString(@class.Name) == name)
			{
				return @class;
			}
		}
		return null;
	}

	/// <summary>
	/// Get a string from the string table.
	/// </summary>
	/// <param name="index">The index of the string in the table.</param>
	/// <returns>The string at that index.</returns>
	public string GetString(ushort index)
	{
		return StringTable.GetString(index);
	}
}
