using System;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp;

public sealed class InvalidImageContentException : ImageFormatException
{
	public InvalidImageContentException(string errorMessage)
		: base(errorMessage)
	{
	}

	public InvalidImageContentException(string errorMessage, Exception innerException)
		: base(errorMessage, innerException)
	{
	}

	internal InvalidImageContentException(Size size, InvalidMemoryOperationException memoryException)
		: this($"Cannot decode image. Failed to allocate buffers for possibly degenerate dimensions: {size.Width}x{size.Height}.", memoryException)
	{
	}
}
