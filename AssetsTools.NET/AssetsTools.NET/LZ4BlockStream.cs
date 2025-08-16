using System;
using System.Collections.Generic;
using System.IO;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Extra.Decompressors.LZ4;

namespace AssetsTools.NET;

public class LZ4BlockStream : Stream
{
	private readonly long length;

	private readonly long blockSize;

	private readonly AssetBundleBlockInfo[] blockInfos;

	private readonly long[] blockPoses;

	private readonly Dictionary<int, MemoryStream> decompressedBlockMap;

	private readonly Queue<int> decompressedBlockQueue;

	public int maxBlockMapSize;

	public const int DEFAULT_MAX_BLOCK_MAP_SIZE = 382;

	public Stream BaseStream { get; }

	public long BaseOffset { get; }

	public override long Position { get; set; }

	public override long Length => length;

	public override bool CanRead => BaseStream.CanRead;

	public override bool CanSeek => BaseStream.CanSeek;

	public override bool CanWrite => BaseStream.CanWrite;

	public LZ4BlockStream(Stream baseStream, long baseOffset, AssetBundleBlockInfo[] blockInfos, int maxBlockMapSize = 382)
	{
		if (baseOffset < 0 || baseOffset > baseStream.Length)
		{
			throw new ArgumentOutOfRangeException("baseOffset");
		}
		if (length >= 0 && baseOffset + length > baseStream.Length)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		decompressedBlockMap = new Dictionary<int, MemoryStream>();
		decompressedBlockQueue = new Queue<int>();
		if (blockInfos.Length == 0)
		{
			length = 0L;
			blockSize = 131072L;
			BaseStream = new MemoryStream();
			BaseOffset = baseOffset;
			this.blockInfos = new AssetBundleBlockInfo[0];
			blockPoses = new long[0];
			return;
		}
		long num = blockInfos[0].CompressedSize;
		blockPoses = new long[blockInfos.Length];
		blockPoses[0] = 0L;
		blockSize = GetLz4BlockSize(blockInfos);
		length = blockInfos[0].DecompressedSize;
		for (int i = 1; i < blockInfos.Length; i++)
		{
			if (blockInfos[i].DecompressedSize != blockSize && i != blockInfos.Length - 1)
			{
				throw new NotImplementedException("Cannot handle bundles with multiple block sizes yet.");
			}
			length += blockInfos[i].DecompressedSize;
			blockPoses[i] = num;
			num += blockInfos[i].CompressedSize;
		}
		if (blockSize > int.MaxValue)
		{
			throw new NotImplementedException("Block size too large!");
		}
		this.blockInfos = blockInfos;
		this.maxBlockMapSize = maxBlockMapSize;
		BaseStream = baseStream;
		BaseOffset = baseOffset;
	}

	public override void Flush()
	{
		BaseStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = 0;
		while (num < count)
		{
			int num2 = (int)(Position / blockSize);
			MemoryStream memoryStream2;
			if (!decompressedBlockMap.ContainsKey(num2))
			{
				if (decompressedBlockMap.Count >= maxBlockMapSize)
				{
					int key = decompressedBlockQueue.Dequeue();
					decompressedBlockMap[key].Close();
					decompressedBlockMap.Remove(key);
				}
				BaseStream.Position = BaseOffset + blockPoses[num2];
				MemoryStream memoryStream = new MemoryStream();
				BaseStream.CopyToCompat(memoryStream, blockInfos[num2].CompressedSize);
				memoryStream.Position = 0L;
				byte compressionType = blockInfos[num2].GetCompressionType();
				if (compressionType == 0)
				{
					memoryStream2 = memoryStream;
				}
				else
				{
					if (compressionType != 2 && compressionType != 3)
					{
						throw new Exception("Invalid block compression type in supposed LZ4 only stream!");
					}
					byte[] array = new byte[blockInfos[num2].DecompressedSize];
					using (Lz4DecoderStream lz4DecoderStream = new Lz4DecoderStream(memoryStream))
					{
						lz4DecoderStream.Read(array, 0, array.Length);
					}
					memoryStream2 = new MemoryStream(array);
				}
				decompressedBlockMap[num2] = memoryStream2;
				decompressedBlockQueue.Enqueue(num2);
			}
			else
			{
				memoryStream2 = decompressedBlockMap[num2];
			}
			memoryStream2.Position = Position % blockSize;
			int num3 = memoryStream2.Read(buffer, offset + num, (int)Math.Min(memoryStream2.Length, count - num));
			if (num3 == 0)
			{
				break;
			}
			num += num3;
			Position += num3;
		}
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		long num = origin switch
		{
			SeekOrigin.Begin => offset, 
			SeekOrigin.Current => Position + offset, 
			SeekOrigin.End => Position + Length + offset, 
			_ => throw new ArgumentException(), 
		};
		if (num < 0 || num > Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		Position = num;
		return Position;
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException("LZ4BlockStream cannot be written to, only read from.");
	}

	private long GetLz4BlockSize(AssetBundleBlockInfo[] blockInfos)
	{
		for (int i = 0; i < blockInfos.Length; i++)
		{
			if (blockInfos[i].GetCompressionType() == 2 || blockInfos[i].GetCompressionType() == 3)
			{
				return blockInfos[i].DecompressedSize;
			}
		}
		if (blockInfos[0].GetCompressionType() == 0)
		{
			return blockInfos[0].DecompressedSize;
		}
		throw new Exception("No LZ4 blocks were found in block infos. Can't find block size.");
	}
}
