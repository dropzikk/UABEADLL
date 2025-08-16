using System;
using Avalonia.Platform;

namespace Avalonia.OpenGL;

public interface IGlExternalImageTexture : IDisposable
{
	int TextureId { get; }

	int InternalFormat { get; }

	PlatformGraphicsExternalImageProperties Properties { get; }

	void AcquireKeyedMutex(uint key);

	void ReleaseKeyedMutex(uint key);
}
