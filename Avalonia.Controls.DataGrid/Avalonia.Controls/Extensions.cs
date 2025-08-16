namespace Avalonia.Controls;

internal static class Extensions
{
	internal static Point Translate(this Visual fromElement, Visual toElement, Point fromPoint)
	{
		if (fromElement == toElement)
		{
			return fromPoint;
		}
		Matrix? matrix = fromElement.TransformToVisual(toElement);
		if (matrix.HasValue)
		{
			return fromPoint.Transform(matrix.Value);
		}
		return fromPoint;
	}
}
