using System;
using System.Threading.Tasks;

namespace Avalonia.Remote.Protocol;

public interface IAvaloniaRemoteTransportConnection : IDisposable
{
	event Action<IAvaloniaRemoteTransportConnection, object> OnMessage;

	event Action<IAvaloniaRemoteTransportConnection, Exception> OnException;

	Task Send(object data);

	void Start();
}
