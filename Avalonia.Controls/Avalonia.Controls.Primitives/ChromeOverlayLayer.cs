using System.Linq;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

public class ChromeOverlayLayer : Panel
{
	public static Panel? GetOverlayLayer(Visual visual)
	{
		foreach (Visual visualAncestor in visual.GetVisualAncestors())
		{
			if (visualAncestor is VisualLayerManager { OverlayLayer: not null } visualLayerManager)
			{
				return visualLayerManager.ChromeOverlayLayer;
			}
		}
		if (visual is TopLevel visual2)
		{
			return visual2.GetVisualDescendants().OfType<VisualLayerManager>().FirstOrDefault()?.ChromeOverlayLayer;
		}
		return null;
	}

	public void Add(Control c)
	{
		base.Children.Add(c);
	}
}
