using System;

namespace AvaloniaEdit.Highlighting;

public class HighlightingDefinitionInvalidException : Exception
{
	public HighlightingDefinitionInvalidException()
	{
	}

	public HighlightingDefinitionInvalidException(string message)
		: base(message)
	{
	}

	public HighlightingDefinitionInvalidException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
