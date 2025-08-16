using System;
using System.Linq;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

public class LightDismissOverlayLayer : Border, ICustomHitTest
{
	public IInputElement? InputPassThroughElement { get; set; }

	static LightDismissOverlayLayer()
	{
		Border.BackgroundProperty.OverrideDefaultValue<LightDismissOverlayLayer>(Brushes.Transparent);
	}

	public static LightDismissOverlayLayer? GetLightDismissOverlayLayer(Visual visual)
	{
		visual = visual ?? throw new ArgumentNullException("visual");
		return ((!(visual is TopLevel control)) ? visual.FindAncestorOfType<VisualLayerManager>() : control.GetTemplateChildren().OfType<VisualLayerManager>().FirstOrDefault())?.LightDismissOverlayLayer;
	}

	public bool HitTest(Point point)
	{
		if (InputPassThroughElement is Visual visual && base.VisualRoot is IInputElement element && element.InputHitTest(point, (Visual x) => x != this) is Visual target)
		{
			return !visual.IsVisualAncestorOf(target);
		}
		return true;
	}
}
