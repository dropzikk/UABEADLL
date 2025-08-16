using System;
using System.Numerics;

namespace SixLabors.ImageSharp.Processing.Processors.Effects;

public interface IPixelRowDelegate
{
	void Invoke(Span<Vector4> span, Point offset);
}
