using System;
using System.Buffers;
using System.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.BitReader;

internal abstract class BitReaderBase : IDisposable
{
	private bool isDisposed;

	public IMemoryOwner<byte> Data { get; }

	protected BitReaderBase(IMemoryOwner<byte> data)
	{
		Data = data;
	}

	protected BitReaderBase(Stream inputStream, int imageDataSize, MemoryAllocator memoryAllocator)
	{
		Data = ReadImageDataFromStream(inputStream, imageDataSize, memoryAllocator);
	}

	protected static IMemoryOwner<byte> ReadImageDataFromStream(Stream input, int bytesToRead, MemoryAllocator memoryAllocator)
	{
		IMemoryOwner<byte> memoryOwner = memoryAllocator.Allocate<byte>(bytesToRead);
		input.Read(memoryOwner.Memory.Span.Slice(0, bytesToRead), 0, bytesToRead);
		return memoryOwner;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			if (disposing)
			{
				Data.Dispose();
			}
			isDisposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
