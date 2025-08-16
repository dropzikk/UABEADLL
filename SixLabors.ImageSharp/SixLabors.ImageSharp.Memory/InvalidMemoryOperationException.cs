using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Memory;

public class InvalidMemoryOperationException : InvalidOperationException
{
	public InvalidMemoryOperationException(string message)
		: base(message)
	{
	}

	public InvalidMemoryOperationException()
	{
	}

	[DoesNotReturn]
	internal static void ThrowNegativeAllocationException(long length)
	{
		throw new InvalidMemoryOperationException($"Attempted to allocate a buffer of negative length={length}.");
	}

	[DoesNotReturn]
	internal static void ThrowInvalidAlignmentException(long alignment)
	{
		throw new InvalidMemoryOperationException($"The buffer capacity of the provided MemoryAllocator is insufficient for the requested buffer alignment: {alignment}.");
	}

	[DoesNotReturn]
	internal static void ThrowAllocationOverLimitException(ulong length, long limit)
	{
		throw new InvalidMemoryOperationException($"Attempted to allocate a buffer of length={length} that exceeded the limit {limit}.");
	}
}
