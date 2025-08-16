using System;
using System.Buffers;

namespace SixLabors.ImageSharp.Memory;

internal sealed class ByteMemoryOwner<T> : IMemoryOwner<T>, IDisposable where T : unmanaged
{
	private readonly IMemoryOwner<byte> memoryOwner;

	private readonly ByteMemoryManager<T> memoryManager;

	private bool disposedValue;

	public Memory<T> Memory => memoryManager.Memory;

	public ByteMemoryOwner(IMemoryOwner<byte> memoryOwner)
	{
		this.memoryOwner = memoryOwner;
		memoryManager = new ByteMemoryManager<T>(memoryOwner.Memory);
	}

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				memoryOwner.Dispose();
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
