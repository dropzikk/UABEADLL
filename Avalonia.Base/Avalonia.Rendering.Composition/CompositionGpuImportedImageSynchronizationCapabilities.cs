using System;

namespace Avalonia.Rendering.Composition;

[Flags]
public enum CompositionGpuImportedImageSynchronizationCapabilities
{
	Semaphores = 1,
	KeyedMutex = 2,
	Automatic = 4
}
