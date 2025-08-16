using System;
using System.Collections.Generic;

namespace Avalonia.OpenGL.Egl;

public class EglDisplayOptions
{
	public EglInterface? Egl { get; set; }

	public bool SupportsContextSharing { get; set; }

	public bool SupportsMultipleContexts { get; set; }

	public bool ContextLossIsDisplayLoss { get; set; }

	public Func<bool>? DeviceLostCheckCallback { get; set; }

	public Action? DisposeCallback { get; set; }

	public IEnumerable<GlVersion>? GlVersions { get; set; }
}
