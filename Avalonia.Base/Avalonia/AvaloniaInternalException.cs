using System;

namespace Avalonia;

public class AvaloniaInternalException : Exception
{
	public AvaloniaInternalException(string message)
		: base(message)
	{
	}
}
