using System.Collections.Generic;
using Avalonia.OpenGL;
using Avalonia.Platform;

namespace Avalonia;

public class Win32PlatformOptions
{
	public bool OverlayPopups { get; set; }

	public IReadOnlyList<Win32RenderingMode> RenderingMode { get; set; } = new Win32RenderingMode[2]
	{
		Win32RenderingMode.AngleEgl,
		Win32RenderingMode.Software
	};

	public IReadOnlyList<Win32CompositionMode> CompositionMode { get; set; } = new Win32CompositionMode[2]
	{
		Win32CompositionMode.WinUIComposition,
		Win32CompositionMode.RedirectionSurface
	};

	public float? WinUICompositionBackdropCornerRadius { get; set; }

	public bool ShouldRenderOnUIThread { get; set; }

	public IList<GlVersion> WglProfiles { get; set; } = new List<GlVersion>
	{
		new GlVersion(GlProfileType.OpenGL, 4, 0),
		new GlVersion(GlProfileType.OpenGL, 3, 2)
	};

	public IPlatformGraphics? CustomPlatformGraphics { get; set; }
}
