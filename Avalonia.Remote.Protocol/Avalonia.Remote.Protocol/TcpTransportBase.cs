using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Avalonia.Remote.Protocol;

public abstract class TcpTransportBase
{
	private class DisposableServer : IDisposable
	{
		private readonly TcpListener _l;

		public DisposableServer(TcpListener l)
		{
			_l = l;
		}

		public void Dispose()
		{
			try
			{
				_l.Stop();
			}
			catch
			{
			}
		}
	}

	private readonly IMessageTypeResolver _resolver;

	public TcpTransportBase(IMessageTypeResolver resolver)
	{
		_resolver = resolver;
	}

	protected abstract IAvaloniaRemoteTransportConnection CreateTransport(IMessageTypeResolver resolver, Stream stream, Action disposeCallback);

	public IDisposable Listen(IPAddress address, int port, Action<IAvaloniaRemoteTransportConnection> cb)
	{
		TcpListener server = new TcpListener(address, port);
		server.Start();
		AcceptNew();
		return new DisposableServer(server);
		async void AcceptNew()
		{
			_ = 1;
			try
			{
				TcpClient cl = await server.AcceptTcpClientAsync().ConfigureAwait(continueOnCapturedContext: false);
				AcceptNew();
				await Task.Run(async delegate
				{
					TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
					IAvaloniaRemoteTransportConnection obj = CreateTransport(_resolver, cl.GetStream(), delegate
					{
						tcs.TrySetResult(0);
					});
					cb(obj);
					await tcs.Task;
				}).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
			}
		}
	}

	public async Task<IAvaloniaRemoteTransportConnection> Connect(IPAddress address, int port)
	{
		TcpClient c = new TcpClient();
		await c.ConnectAsync(address, port).ConfigureAwait(continueOnCapturedContext: false);
		return CreateTransport(_resolver, c.GetStream(), ((IDisposable)c).Dispose);
	}
}
