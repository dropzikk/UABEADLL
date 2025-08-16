using System;

namespace SevenZip;

/// <summary>
/// The exception that is thrown when an error in input stream occurs during decoding.
/// </summary>
internal class DataErrorException : ApplicationException
{
	public DataErrorException()
		: base("Data Error")
	{
	}
}
