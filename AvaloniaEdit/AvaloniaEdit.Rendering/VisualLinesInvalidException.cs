using System;

namespace AvaloniaEdit.Rendering;

public class VisualLinesInvalidException : Exception
{
	public VisualLinesInvalidException()
	{
	}

	public VisualLinesInvalidException(string message)
		: base(message)
	{
	}

	public VisualLinesInvalidException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
