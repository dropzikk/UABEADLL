using System;
using System.Reflection;
using System.Xml;

namespace Avalonia.Remote.Protocol.Designer;

public class ExceptionDetails
{
	public string ExceptionType { get; set; }

	public string Message { get; set; }

	public int? LineNumber { get; set; }

	public int? LinePosition { get; set; }

	public ExceptionDetails()
	{
	}

	public ExceptionDetails(Exception e)
	{
		if (e is TargetInvocationException)
		{
			e = e.InnerException;
		}
		ExceptionType = e.GetType().Name;
		Message = e.Message;
		if (e is XmlException ex)
		{
			LineNumber = ex.LineNumber;
			LinePosition = ex.LinePosition;
		}
	}
}
