using System;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public sealed class DegenerateTransformException : Exception
{
	public DegenerateTransformException()
	{
	}

	public DegenerateTransformException(string message)
		: base(message)
	{
	}

	public DegenerateTransformException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
