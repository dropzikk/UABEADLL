using System;
using System.Collections.Generic;
using Avalonia.Platform;

namespace Avalonia.OpenGL;

public interface IGlContext : IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	GlVersion Version { get; }

	GlInterface GlInterface { get; }

	int SampleCount { get; }

	int StencilSize { get; }

	bool CanCreateSharedContext { get; }

	IDisposable MakeCurrent();

	bool IsSharedWith(IGlContext context);

	IGlContext? CreateSharedContext(IEnumerable<GlVersion>? preferredVersions = null);
}
