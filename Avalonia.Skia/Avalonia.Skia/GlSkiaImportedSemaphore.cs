using System;
using Avalonia.OpenGL;
using Avalonia.Platform;

namespace Avalonia.Skia;

internal class GlSkiaImportedSemaphore : IPlatformRenderInterfaceImportedSemaphore, IPlatformRenderInterfaceImportedObject, IDisposable
{
	private readonly GlSkiaGpu _gpu;

	public IGlExternalSemaphore Semaphore { get; }

	public GlSkiaImportedSemaphore(GlSkiaGpu gpu, IGlExternalSemaphore semaphore)
	{
		_gpu = gpu;
		Semaphore = semaphore;
	}

	public void Dispose()
	{
		Semaphore.Dispose();
	}
}
