using System;

namespace AvaloniaEdit.Search;

public class SearchPatternException : Exception
{
	public SearchPatternException()
	{
	}

	public SearchPatternException(string message)
		: base(message)
	{
	}

	public SearchPatternException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
