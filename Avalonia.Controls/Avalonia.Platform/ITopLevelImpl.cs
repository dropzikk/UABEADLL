using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Metadata;
using Avalonia.Rendering.Composition;

namespace Avalonia.Platform;

[Unstable]
public interface ITopLevelImpl : IOptionalFeatureProvider, IDisposable
{
	Size ClientSize { get; }

	Size? FrameSize { get; }

	double RenderScaling { get; }

	IEnumerable<object> Surfaces { get; }

	Action<RawInputEventArgs>? Input { get; set; }

	Action<Rect>? Paint { get; set; }

	Action<Size, WindowResizeReason>? Resized { get; set; }

	Action<double>? ScalingChanged { get; set; }

	Action<WindowTransparencyLevel>? TransparencyLevelChanged { get; set; }

	Compositor Compositor { get; }

	Action? Closed { get; set; }

	Action? LostFocus { get; set; }

	WindowTransparencyLevel TransparencyLevel { get; }

	AcrylicPlatformCompensationLevels AcrylicCompensationLevels { get; }

	void SetInputRoot(IInputRoot inputRoot);

	Point PointToClient(PixelPoint point);

	PixelPoint PointToScreen(Point point);

	void SetCursor(ICursorImpl? cursor);

	IPopupImpl? CreatePopup();

	void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevels);

	void SetFrameThemeVariant(PlatformThemeVariant themeVariant);
}
