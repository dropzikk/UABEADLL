using System;

namespace SixLabors.ImageSharp;

public class ImageFormatException : Exception
{
	internal ImageFormatException(string errorMessage)
		: base(errorMessage)
	{
	}

	internal ImageFormatException(string errorMessage, Exception innerException)
		: base(errorMessage, innerException)
	{
	}
}
