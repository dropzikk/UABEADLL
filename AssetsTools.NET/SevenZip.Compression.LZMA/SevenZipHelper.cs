using System;
using System.IO;

namespace SevenZip.Compression.LZMA;

public static class SevenZipHelper
{
	private static CoderPropID[] propIDs = new CoderPropID[8]
	{
		CoderPropID.DictionarySize,
		CoderPropID.PosStateBits,
		CoderPropID.LitContextBits,
		CoderPropID.LitPosBits,
		CoderPropID.Algorithm,
		CoderPropID.NumFastBytes,
		CoderPropID.MatchFinder,
		CoderPropID.EndMarker
	};

	private static object[] properties = new object[8] { 2097152, 2, 3, 0, 2, 32, "bt4", false };

	public static byte[] Compress(byte[] inputBytes, ICodeProgress progress = null)
	{
		MemoryStream inStream = new MemoryStream(inputBytes);
		MemoryStream memoryStream = new MemoryStream();
		Compress(inStream, memoryStream, progress);
		return memoryStream.ToArray();
	}

	public static void Compress(Stream inStream, Stream outStream, ICodeProgress progress = null)
	{
		Encoder encoder = new Encoder();
		encoder.SetCoderProperties(propIDs, properties);
		encoder.WriteCoderProperties(outStream);
		encoder.Code(inStream, outStream, -1L, -1L, progress);
	}

	public static byte[] Decompress(byte[] inputBytes)
	{
		MemoryStream memoryStream = new MemoryStream(inputBytes);
		Decoder decoder = new Decoder();
		memoryStream.Seek(0L, SeekOrigin.Begin);
		MemoryStream memoryStream2 = new MemoryStream();
		byte[] array = new byte[5];
		if (memoryStream.Read(array, 0, 5) != 5)
		{
			throw new Exception("input .lzma is too short");
		}
		long num = 0L;
		for (int i = 0; i < 8; i++)
		{
			int num2 = memoryStream.ReadByte();
			if (num2 < 0)
			{
				throw new Exception("Can't Read 1");
			}
			num |= (long)((ulong)(byte)num2 << 8 * i);
		}
		decoder.SetDecoderProperties(array);
		long inSize = memoryStream.Length - memoryStream.Position;
		decoder.Code(memoryStream, memoryStream2, inSize, num, null);
		return memoryStream2.ToArray();
	}

	public static MemoryStream StreamDecompress(MemoryStream newInStream)
	{
		Decoder decoder = new Decoder();
		newInStream.Seek(0L, SeekOrigin.Begin);
		MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[5];
		if (newInStream.Read(array, 0, 5) != 5)
		{
			throw new Exception("input .lzma is too short");
		}
		long num = 0L;
		for (int i = 0; i < 8; i++)
		{
			int num2 = newInStream.ReadByte();
			if (num2 < 0)
			{
				throw new Exception("Can't Read 1");
			}
			num |= (long)((ulong)(byte)num2 << 8 * i);
		}
		decoder.SetDecoderProperties(array);
		long inSize = newInStream.Length - newInStream.Position;
		decoder.Code(newInStream, memoryStream, inSize, num, null);
		memoryStream.Position = 0L;
		return memoryStream;
	}

	public static MemoryStream StreamDecompress(MemoryStream newInStream, long outSize)
	{
		Decoder decoder = new Decoder();
		newInStream.Seek(0L, SeekOrigin.Begin);
		MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[5];
		if (newInStream.Read(array, 0, 5) != 5)
		{
			throw new Exception("input .lzma is too short");
		}
		decoder.SetDecoderProperties(array);
		long inSize = newInStream.Length - newInStream.Position;
		decoder.Code(newInStream, memoryStream, inSize, outSize, null);
		memoryStream.Position = 0L;
		return memoryStream;
	}

	public static void StreamDecompress(Stream compressedStream, Stream decompressedStream, long compressedSize, long decompressedSize)
	{
		long position = compressedStream.Position;
		Decoder decoder = new Decoder();
		byte[] array = new byte[5];
		if (compressedStream.Read(array, 0, 5) != 5)
		{
			throw new Exception("input .lzma is too short");
		}
		decoder.SetDecoderProperties(array);
		decoder.Code(compressedStream, decompressedStream, compressedSize - 5, decompressedSize, null);
		compressedStream.Position = position + compressedSize;
	}
}
