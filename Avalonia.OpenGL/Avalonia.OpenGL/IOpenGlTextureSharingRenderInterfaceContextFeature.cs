using System.Collections.Generic;

namespace Avalonia.OpenGL;

public interface IOpenGlTextureSharingRenderInterfaceContextFeature
{
	bool CanCreateSharedContext { get; }

	IGlContext? CreateSharedContext(IEnumerable<GlVersion>? preferredVersions = null);

	ICompositionImportableOpenGlSharedTexture CreateSharedTextureForComposition(IGlContext context, PixelSize size);
}
