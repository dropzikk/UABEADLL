using System;
using Avalonia.Rendering.Composition;

namespace Avalonia.OpenGL;

public interface ICompositionImportableOpenGlSharedTexture : ICompositionImportableSharedGpuContextImage, IDisposable
{
	int TextureId { get; }

	int InternalFormat { get; }

	PixelSize Size { get; }
}
