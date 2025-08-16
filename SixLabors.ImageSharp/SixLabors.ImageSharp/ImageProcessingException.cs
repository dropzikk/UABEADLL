using System;

namespace SixLabors.ImageSharp;

public sealed class ImageProcessingException : Exception
{
	public ImageProcessingException()
	{
	}

	public ImageProcessingException(string errorMessage)
		: base(errorMessage)
	{
	}

	public ImageProcessingException(string errorMessage, Exception innerException)
		: base(errorMessage, innerException)
	{
	}
}
