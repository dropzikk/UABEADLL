using System;
using System.Threading.Tasks;

namespace Tmds.DBus.Protocol;

internal interface IMessageStream
{
	public delegate void MessageReceivedHandler<T>(Exception? closeReason, Message message, T state);

	void ReceiveMessages<T>(MessageReceivedHandler<T> handler, T state);

	ValueTask<bool> TrySendMessageAsync(MessageBuffer message);

	void Close(Exception closeReason);
}
