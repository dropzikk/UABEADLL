using System;
using Avalonia.Platform;

namespace Avalonia.OpenGL;

public interface IGlExportableExternalImageTexture : IGlExternalImageTexture, IDisposable
{
	IPlatformHandle GetHandle();
}
