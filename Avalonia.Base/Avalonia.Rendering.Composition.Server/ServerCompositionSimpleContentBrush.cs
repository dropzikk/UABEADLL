using System;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSimpleContentBrush : ServerCompositionSimpleTileBrush, ITileBrush, IBrush, ISceneBrush
{
	private CompositionRenderDataSceneBrushContent? _content;

	internal ServerCompositionSimpleContentBrush(ServerCompositor compositor)
		: base(compositor)
	{
	}

	public ISceneBrushContent? CreateContent()
	{
		return _content;
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		_content = reader.ReadObject<CompositionRenderDataSceneBrushContent>();
	}
}
