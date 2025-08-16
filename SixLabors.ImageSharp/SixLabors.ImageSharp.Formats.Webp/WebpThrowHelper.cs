using System;
using System.Diagnostics.CodeAnalysis;

namespace SixLabors.ImageSharp.Formats.Webp;

internal static class WebpThrowHelper
{
	[DoesNotReturn]
	public static void ThrowInvalidImageContentException(string errorMessage)
	{
		throw new InvalidImageContentException(errorMessage);
	}

	[DoesNotReturn]
	public static void ThrowImageFormatException(string errorMessage)
	{
		throw new ImageFormatException(errorMessage);
	}

	[DoesNotReturn]
	public static void ThrowNotSupportedException(string errorMessage)
	{
		throw new NotSupportedException(errorMessage);
	}

	[DoesNotReturn]
	public static void ThrowInvalidImageDimensions(string errorMessage)
	{
		throw new InvalidImageContentException(errorMessage);
	}
}
