using System;
using System.Threading.Tasks;
using Avalonia.Remote.Protocol;

namespace Avalonia.DesignerSupport.Remote;

internal class DetachableTransportConnection : IAvaloniaRemoteTransportConnection, IDisposable
{
	private IAvaloniaRemoteTransportConnection _inner;

	public event Action<IAvaloniaRemoteTransportConnection, object> OnMessage;

	public event Action<IAvaloniaRemoteTransportConnection, Exception> OnException
	{
		add
		{
		}
		remove
		{
		}
	}

	public DetachableTransportConnection(IAvaloniaRemoteTransportConnection inner)
	{
		_inner = inner;
		_inner.OnMessage += FireOnMessage;
	}

	public void Dispose()
	{
		if (_inner != null)
		{
			_inner.OnMessage -= FireOnMessage;
		}
		_inner = null;
	}

	public void FireOnMessage(IAvaloniaRemoteTransportConnection transport, object obj)
	{
		this.OnMessage?.Invoke(transport, obj);
	}

	public Task Send(object data)
	{
		return _inner?.Send(data);
	}

	public void Start()
	{
		_inner?.Start();
	}
}
