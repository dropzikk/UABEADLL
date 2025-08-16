using System;
using System.Buffers;
using System.Collections.Generic;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal class Vp8LDecoder : IDisposable
{
	public int Width { get; set; }

	public int Height { get; set; }

	public Vp8LMetadata Metadata { get; set; }

	public List<Vp8LTransform> Transforms { get; set; }

	public IMemoryOwner<uint> Pixels { get; }

	public Vp8LDecoder(int width, int height, MemoryAllocator memoryAllocator)
	{
		Width = width;
		Height = height;
		Metadata = new Vp8LMetadata();
		Pixels = memoryAllocator.Allocate<uint>(width * height, AllocationOptions.Clean);
	}

	public void Dispose()
	{
		Pixels.Dispose();
		Metadata?.HuffmanImage?.Dispose();
		if (Transforms == null)
		{
			return;
		}
		foreach (Vp8LTransform transform in Transforms)
		{
			transform.Data?.Dispose();
		}
	}
}
