using System;
using System.Runtime.Serialization;

namespace Metsys.Bson;

internal class BsonException : Exception
{
	public BsonException()
	{
	}

	public BsonException(string message)
		: base(message)
	{
	}

	public BsonException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected BsonException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
