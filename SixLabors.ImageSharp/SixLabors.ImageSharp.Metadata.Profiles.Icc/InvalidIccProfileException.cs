using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

public class InvalidIccProfileException : Exception
{
	public InvalidIccProfileException(string message)
		: base(message)
	{
	}

	public InvalidIccProfileException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
