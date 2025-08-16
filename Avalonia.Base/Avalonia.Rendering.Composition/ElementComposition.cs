using System;
using Avalonia.VisualTree;

namespace Avalonia.Rendering.Composition;

public static class ElementComposition
{
	public static CompositionVisual? GetElementVisual(Visual visual)
	{
		return visual.CompositionVisual;
	}

	public static void SetElementChildVisual(Visual visual, CompositionVisual? compositionVisual)
	{
		if (compositionVisual != null && visual.CompositionVisual != null && compositionVisual.Compositor != visual.CompositionVisual.Compositor)
		{
			throw new InvalidOperationException("Composition visuals belong to different compositor instances");
		}
		visual.ChildCompositionVisual = compositionVisual;
		visual.GetVisualRoot()?.Renderer.RecalculateChildren(visual);
	}

	public static CompositionVisual? GetElementChildVisual(Visual visual)
	{
		return visual.ChildCompositionVisual;
	}
}
