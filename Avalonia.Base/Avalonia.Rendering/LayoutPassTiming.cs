using System;

namespace Avalonia.Rendering;

internal readonly record struct LayoutPassTiming(int PassCounter, TimeSpan Elapsed);
