using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition;

internal class CompositionDrawListVisual : CompositionContainerVisual
{
	private bool _drawListChanged;

	private CompositionRenderData? _drawList;

	public Visual Visual { get; }

	public CompositionRenderData? DrawList
	{
		get
		{
			return _drawList;
		}
		set
		{
			_drawList?.Dispose();
			_drawList = value;
			_drawListChanged = true;
			RegisterForSerialization();
		}
	}

	private protected override void SerializeChangesCore(BatchStreamWriter writer)
	{
		writer.Write((byte)(_drawListChanged ? 1u : 0u));
		if (_drawListChanged)
		{
			writer.WriteObject(DrawList?.Server);
			_drawListChanged = false;
		}
		base.SerializeChangesCore(writer);
	}

	internal CompositionDrawListVisual(Compositor compositor, ServerCompositionDrawListVisual server, Visual visual)
		: base(compositor, server)
	{
		Visual = visual;
	}

	internal override bool HitTest(Point pt)
	{
		ICustomHitTest customHitTest = Visual as ICustomHitTest;
		if (DrawList == null && customHitTest == null)
		{
			return false;
		}
		return customHitTest?.HitTest(pt) ?? DrawList?.HitTest(pt) ?? false;
	}
}
