using System;

namespace Avalonia.Data;

public class DataValidationException : Exception
{
	public object? ErrorData { get; }

	public DataValidationException(object? errorData)
		: base(errorData?.ToString())
	{
		ErrorData = errorData;
	}
}
