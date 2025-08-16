using System;
using Avalonia.Platform;

namespace Avalonia.OpenGL;

public interface IGlExportableExternalSemaphore : IGlExternalSemaphore, IDisposable
{
	IPlatformHandle GetHandle();
}
