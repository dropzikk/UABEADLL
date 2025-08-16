using System;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class MetalPlatformGraphics : IPlatformGraphics
{
	private readonly IAvnMetalDisplay _display;

	public bool UsesSharedContext => false;

	public MetalPlatformGraphics(IAvaloniaNativeFactory factory)
	{
		_display = factory.ObtainMetalDisplay();
	}

	public IPlatformGraphicsContext CreateContext()
	{
		return new MetalDevice(_display.CreateDevice());
	}

	public IPlatformGraphicsContext GetSharedContext()
	{
		throw new NotSupportedException();
	}
}
