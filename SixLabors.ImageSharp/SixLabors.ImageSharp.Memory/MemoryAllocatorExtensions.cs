using System.Buffers;

namespace SixLabors.ImageSharp.Memory;

public static class MemoryAllocatorExtensions
{
	public static Buffer2D<T> Allocate2D<T>(this MemoryAllocator memoryAllocator, int width, int height, bool preferContiguosImageBuffers, AllocationOptions options = AllocationOptions.None) where T : struct
	{
		long num = (long)width * (long)height;
		MemoryGroup<T> memoryGroup = ((!preferContiguosImageBuffers || num >= int.MaxValue) ? memoryAllocator.AllocateGroup<T>(num, width, options) : MemoryGroup<T>.CreateContiguous(memoryAllocator.Allocate<T>((int)num, options), clear: false));
		return new Buffer2D<T>(memoryGroup, width, height);
	}

	public static Buffer2D<T> Allocate2D<T>(this MemoryAllocator memoryAllocator, int width, int height, AllocationOptions options = AllocationOptions.None) where T : struct
	{
		return memoryAllocator.Allocate2D<T>(width, height, preferContiguosImageBuffers: false, options);
	}

	public static Buffer2D<T> Allocate2D<T>(this MemoryAllocator memoryAllocator, Size size, bool preferContiguosImageBuffers, AllocationOptions options = AllocationOptions.None) where T : struct
	{
		return memoryAllocator.Allocate2D<T>(size.Width, size.Height, preferContiguosImageBuffers, options);
	}

	public static Buffer2D<T> Allocate2D<T>(this MemoryAllocator memoryAllocator, Size size, AllocationOptions options = AllocationOptions.None) where T : struct
	{
		return memoryAllocator.Allocate2D<T>(size.Width, size.Height, preferContiguosImageBuffers: false, options);
	}

	internal static Buffer2D<T> Allocate2DOveraligned<T>(this MemoryAllocator memoryAllocator, int width, int height, int alignmentMultiplier, AllocationOptions options = AllocationOptions.None) where T : struct
	{
		long totalLength = (long)width * (long)height;
		return new Buffer2D<T>(memoryAllocator.AllocateGroup<T>(totalLength, width * alignmentMultiplier, options), width, height);
	}

	internal static IMemoryOwner<byte> AllocatePaddedPixelRowBuffer(this MemoryAllocator memoryAllocator, int width, int pixelSizeInBytes, int paddingInBytes)
	{
		int length = width * pixelSizeInBytes + paddingInBytes;
		return memoryAllocator.Allocate<byte>(length);
	}
}
