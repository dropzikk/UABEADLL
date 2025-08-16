using System;

namespace Avalonia.Rendering;

[Flags]
public enum RendererDebugOverlays
{
	None = 0,
	Fps = 1,
	DirtyRects = 2,
	LayoutTimeGraph = 4,
	RenderTimeGraph = 8
}
