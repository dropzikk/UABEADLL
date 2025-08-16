using System;

namespace Tmds.DBus.Protocol;

public class MethodContext
{
	public Message Request { get; }

	public Connection Connection { get; }

	public bool ReplySent { get; private set; }

	public bool NoReplyExpected => (Request.MessageFlags & MessageFlags.NoReplyExpected) != 0;

	internal MethodContext(Connection connection, Message request)
	{
		Connection = connection;
		Request = request;
	}

	public MessageWriter CreateReplyWriter(string signature)
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteMethodReturnHeader(Request.Serial, Request.Sender, signature);
		return messageWriter;
	}

	public void Reply(MessageBuffer message)
	{
		if (ReplySent || NoReplyExpected)
		{
			message.Dispose();
			if (ReplySent)
			{
				throw new InvalidOperationException("A reply has already been sent.");
			}
		}
		ReplySent = true;
		Connection.TrySendMessage(message);
	}

	public void ReplyError(string? errorName = null, string? errorMsg = null)
	{
		using MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteError(Request.Serial, Request.Sender, errorName, errorMsg);
		Reply(messageWriter.CreateMessage());
	}
}
