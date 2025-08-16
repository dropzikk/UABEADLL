using System.Linq;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

public class OverlayLayer : Canvas
{
	protected override bool BypassFlowDirectionPolicies => true;

	public Size AvailableSize { get; private set; }

	public static OverlayLayer? GetOverlayLayer(Visual visual)
	{
		foreach (Visual visualAncestor in visual.GetVisualAncestors())
		{
			if (visualAncestor is VisualLayerManager { OverlayLayer: not null } visualLayerManager)
			{
				return visualLayerManager.OverlayLayer;
			}
		}
		if (visual is TopLevel visual2)
		{
			return visual2.GetVisualDescendants().OfType<VisualLayerManager>().FirstOrDefault()?.OverlayLayer;
		}
		return null;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		foreach (Control child in base.Children)
		{
			child.Measure(availableSize);
		}
		return availableSize;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		AvailableSize = finalSize;
		return base.ArrangeOverride(finalSize);
	}
}
