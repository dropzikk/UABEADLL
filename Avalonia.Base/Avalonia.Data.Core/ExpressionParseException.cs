using System;

namespace Avalonia.Data.Core;

public class ExpressionParseException : Exception
{
	public int Column { get; }

	public ExpressionParseException(int column, string message, Exception? innerException = null)
		: base(message, innerException)
	{
		Column = column;
	}
}
