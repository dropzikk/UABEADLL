using System;

namespace Avalonia.OpenGL.Egl;

public class EglDisplayCreationOptions : EglDisplayOptions
{
	public int? PlatformType { get; set; }

	public IntPtr PlatformDisplay { get; set; }

	public int[]? PlatformDisplayAttrs { get; set; }
}
