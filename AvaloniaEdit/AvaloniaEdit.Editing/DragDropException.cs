using System;

namespace AvaloniaEdit.Editing;

public class DragDropException : Exception
{
	public DragDropException()
	{
	}

	public DragDropException(string message)
		: base(message)
	{
	}

	public DragDropException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
