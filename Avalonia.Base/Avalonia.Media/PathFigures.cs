using Avalonia.Collections;
using Avalonia.Visuals.Platform;

namespace Avalonia.Media;

public sealed class PathFigures : AvaloniaList<PathFigure>
{
	public static PathFigures Parse(string pathData)
	{
		PathGeometry pathGeometry = new PathGeometry();
		using (PathGeometryContext geometryContext = new PathGeometryContext(pathGeometry))
		{
			using PathMarkupParser pathMarkupParser = new PathMarkupParser(geometryContext);
			pathMarkupParser.Parse(pathData);
		}
		return pathGeometry.Figures;
	}
}
