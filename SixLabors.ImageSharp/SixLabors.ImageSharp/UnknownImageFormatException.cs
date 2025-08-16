namespace SixLabors.ImageSharp;

public sealed class UnknownImageFormatException : ImageFormatException
{
	public UnknownImageFormatException(string errorMessage)
		: base(errorMessage)
	{
	}
}
