using System;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IGlyphRunImpl : IDisposable
{
	IGlyphTypeface GlyphTypeface { get; }

	double FontRenderingEmSize { get; }

	Point BaselineOrigin { get; }

	Rect Bounds { get; }

	IReadOnlyList<float> GetIntersections(float lowerLimit, float upperLimit);
}
