using Avalonia.Rendering.Composition.Server;

namespace Avalonia.Rendering.Composition;

public class CompositionContainerVisual : CompositionVisual
{
	public CompositionVisualCollection Children { get; private set; }

	internal new ServerCompositionContainerVisual Server { get; }

	private protected override void OnRootChangedCore()
	{
		foreach (CompositionVisual child in Children)
		{
			child.Root = base.Root;
		}
		base.OnRootChangedCore();
	}

	internal CompositionContainerVisual(Compositor compositor, ServerCompositionContainerVisual server)
		: base(compositor, server)
	{
		Server = server;
		InitializeDefaults();
	}

	private void InitializeDefaults()
	{
		InitializeDefaultsExtra();
	}

	private void InitializeDefaultsExtra()
	{
		Children = new CompositionVisualCollection(this, Server.Children);
	}
}
