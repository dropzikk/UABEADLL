using Avalonia.Controls.Embedding;
using Avalonia.Controls.Remote.Server;
using Avalonia.Metadata;
using Avalonia.Remote.Protocol;

namespace Avalonia.Controls.Remote;

[Unstable]
public class RemoteServer
{
	private class EmbeddableRemoteServerTopLevelImpl : RemoteServerTopLevelImpl
	{
		public EmbeddableRemoteServerTopLevelImpl(IAvaloniaRemoteTransportConnection transport)
			: base(transport)
		{
		}
	}

	private EmbeddableControlRoot _topLevel;

	public object? Content
	{
		get
		{
			return _topLevel.Content;
		}
		set
		{
			_topLevel.Content = value;
		}
	}

	public RemoteServer(IAvaloniaRemoteTransportConnection transport)
	{
		_topLevel = new EmbeddableControlRoot(new EmbeddableRemoteServerTopLevelImpl(transport));
		_topLevel.Prepare();
	}
}
