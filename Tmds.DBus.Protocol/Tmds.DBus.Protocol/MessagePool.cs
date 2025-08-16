using System.Buffers;
using System.Threading;
using Nerdbank.Streams;

namespace Tmds.DBus.Protocol;

internal class MessagePool
{
	private const int MinimumSpanLength = 512;

	private Message? _pooled;

	public Message Rent()
	{
		Message message = Interlocked.Exchange(ref _pooled, null);
		if (message != null)
		{
			return message;
		}
		Sequence<byte> sequence = new Sequence<byte>(ArrayPool<byte>.Shared)
		{
			MinimumSpanLength = 512
		};
		return new Message(this, sequence);
	}

	internal void Return(Message value)
	{
		Volatile.Write(ref _pooled, value);
	}
}
