using System;

namespace SevenZip;

/// <summary>
/// The exception that is thrown when the value of an argument is outside the allowable range.
/// </summary>
internal class InvalidParamException : ApplicationException
{
	public InvalidParamException()
		: base("Invalid Parameter")
	{
	}
}
