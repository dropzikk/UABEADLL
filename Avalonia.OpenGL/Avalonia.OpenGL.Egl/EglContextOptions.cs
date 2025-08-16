using System;
using System.Collections.Generic;

namespace Avalonia.OpenGL.Egl;

public class EglContextOptions
{
	public EglContext? ShareWith { get; set; }

	public EglSurface? OffscreenSurface { get; set; }

	public Action? DisposeCallback { get; set; }

	public Dictionary<Type, Func<EglContext, object>>? ExtraFeatures { get; set; }
}
