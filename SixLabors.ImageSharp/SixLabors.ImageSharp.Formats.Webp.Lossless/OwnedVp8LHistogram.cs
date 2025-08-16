using System;
using System.Buffers;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal sealed class OwnedVp8LHistogram : Vp8LHistogram, IDisposable
{
	private readonly IMemoryOwner<uint> bufferOwner;

	private MemoryHandle bufferHandle;

	private bool isDisposed;

	private unsafe OwnedVp8LHistogram(IMemoryOwner<uint> bufferOwner, ref MemoryHandle bufferHandle, uint* basePointer, int paletteCodeBits)
		: base(basePointer, paletteCodeBits)
	{
		this.bufferOwner = bufferOwner;
		this.bufferHandle = bufferHandle;
	}

	public unsafe static OwnedVp8LHistogram Create(MemoryAllocator memoryAllocator, int paletteCodeBits)
	{
		IMemoryOwner<uint> memoryOwner = memoryAllocator.Allocate<uint>(2118, AllocationOptions.Clean);
		MemoryHandle memoryHandle = memoryOwner.Memory.Pin();
		return new OwnedVp8LHistogram(memoryOwner, ref memoryHandle, (uint*)memoryHandle.Pointer, paletteCodeBits);
	}

	public static OwnedVp8LHistogram Create(MemoryAllocator memoryAllocator, Vp8LBackwardRefs refs, int paletteCodeBits)
	{
		OwnedVp8LHistogram ownedVp8LHistogram = Create(memoryAllocator, paletteCodeBits);
		ownedVp8LHistogram.StoreRefs(refs);
		return ownedVp8LHistogram;
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			bufferHandle.Dispose();
			bufferOwner.Dispose();
			isDisposed = true;
		}
	}
}
