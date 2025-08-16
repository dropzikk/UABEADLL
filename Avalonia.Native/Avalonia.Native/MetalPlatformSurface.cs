using Avalonia.Metal;
using Avalonia.Native.Interop;
using Avalonia.Threading;

namespace Avalonia.Native;

internal class MetalPlatformSurface : IMetalPlatformSurface
{
	private readonly IAvnWindowBase _window;

	public MetalPlatformSurface(IAvnWindowBase window)
	{
		_window = window;
	}

	public IMetalPlatformSurfaceRenderTarget CreateMetalRenderTarget(IMetalDevice device)
	{
		if (!Dispatcher.UIThread.CheckAccess())
		{
			throw new RenderTargetNotReadyException();
		}
		MetalDevice metalDevice = (MetalDevice)device;
		return new MetalRenderTarget(_window.CreateMetalRenderTarget(metalDevice.Native));
	}
}
