using System;

namespace Avalonia.Input.GestureRecognizers;

internal record VelocityEstimate(Vector PixelsPerSecond, double Confidence, TimeSpan Duration, Vector Offset);
