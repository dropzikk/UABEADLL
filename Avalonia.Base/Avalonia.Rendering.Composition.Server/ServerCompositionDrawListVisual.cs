using System;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionDrawListVisual : ServerCompositionContainerVisual, IServerRenderResourceObserver
{
	private ServerCompositionRenderData? _renderCommands;

	public override Rect OwnContentBounds => (_renderCommands?.Bounds).GetValueOrDefault();

	public ServerCompositionDrawListVisual(ServerCompositor compositor, Visual v)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		if (reader.Read<byte>() == 1)
		{
			_renderCommands?.Dispose();
			_renderCommands = reader.ReadObject<ServerCompositionRenderData>();
			_renderCommands?.AddObserver(this);
		}
		base.DeserializeChangesCore(reader, committedAt);
	}

	protected override void RenderCore(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
	{
		if (_renderCommands != null)
		{
			_renderCommands.Render(canvas);
		}
		base.RenderCore(canvas, currentTransformedClip);
	}

	public void DependencyQueuedInvalidate(IServerRenderResource sender)
	{
		ValuesInvalidated();
	}
}
