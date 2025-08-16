using System;
using System.Threading.Tasks;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Threading;

namespace Avalonia.Rendering.Composition;

public class CompositionDrawingSurface : CompositionSurface, IDisposable
{
	internal new ServerCompositionDrawingSurface Server => (ServerCompositionDrawingSurface)base.Server;

	internal CompositionDrawingSurface(Compositor compositor)
		: base(compositor, new ServerCompositionDrawingSurface(compositor.Server))
	{
	}

	public Task UpdateWithKeyedMutexAsync(ICompositionImportedGpuImage image, uint acquireIndex, uint releaseIndex)
	{
		CompositionImportedGpuImage img = (CompositionImportedGpuImage)image;
		return base.Compositor.InvokeServerJobAsync(delegate
		{
			Server.UpdateWithKeyedMutex(img, acquireIndex, releaseIndex);
		});
	}

	public Task UpdateWithSemaphoresAsync(ICompositionImportedGpuImage image, ICompositionImportedGpuSemaphore waitForSemaphore, ICompositionImportedGpuSemaphore signalSemaphore)
	{
		CompositionImportedGpuImage img = (CompositionImportedGpuImage)image;
		CompositionImportedGpuSemaphore wait = (CompositionImportedGpuSemaphore)waitForSemaphore;
		CompositionImportedGpuSemaphore signal = (CompositionImportedGpuSemaphore)signalSemaphore;
		return base.Compositor.InvokeServerJobAsync(delegate
		{
			Server.UpdateWithSemaphores(img, wait, signal);
		});
	}

	public Task UpdateAsync(ICompositionImportedGpuImage image)
	{
		CompositionImportedGpuImage img = (CompositionImportedGpuImage)image;
		return base.Compositor.InvokeServerJobAsync(delegate
		{
			Server.UpdateWithAutomaticSync(img);
		});
	}

	~CompositionDrawingSurface()
	{
		Dispatcher.UIThread.Post(Dispose);
	}

	public new void Dispose()
	{
		base.Dispose();
	}
}
