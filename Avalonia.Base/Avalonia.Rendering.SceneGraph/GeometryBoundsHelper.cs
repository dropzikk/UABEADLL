using System;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Rendering.SceneGraph;

internal static class GeometryBoundsHelper
{
	public static Rect CalculateBoundsWithLineCaps(this Rect originalBounds, IPen? pen)
	{
		if (pen == null || MathUtilities.IsZero(pen.Thickness))
		{
			return originalBounds;
		}
		return pen.LineCap switch
		{
			PenLineCap.Flat => originalBounds, 
			PenLineCap.Round => originalBounds.Inflate(pen.Thickness / 2.0), 
			PenLineCap.Square => originalBounds.Inflate(pen.Thickness), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
