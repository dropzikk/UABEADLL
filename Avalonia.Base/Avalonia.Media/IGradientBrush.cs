using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IGradientBrush : IBrush
{
	IReadOnlyList<IGradientStop> GradientStops { get; }

	GradientSpreadMethod SpreadMethod { get; }
}
