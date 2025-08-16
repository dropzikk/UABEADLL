using System;
using System.Numerics;

namespace SixLabors.ImageSharp.Processing;

public delegate void PixelRowOperation(Span<Vector4> span);
public delegate void PixelRowOperation<in T>(Span<Vector4> span, T value);
