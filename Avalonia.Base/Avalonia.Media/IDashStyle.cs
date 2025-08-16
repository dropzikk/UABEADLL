using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IDashStyle
{
	IReadOnlyList<double>? Dashes { get; }

	double Offset { get; }
}
