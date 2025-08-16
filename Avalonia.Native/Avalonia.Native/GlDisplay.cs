using Avalonia.Native.Interop;
using Avalonia.OpenGL;

namespace Avalonia.Native;

internal class GlDisplay
{
	private readonly IAvnGlDisplay _display;

	public GlInterface GlInterface { get; }

	public int SampleCount { get; }

	public int StencilSize { get; }

	public GlDisplay(IAvnGlDisplay display, GlInterface glInterface, int sampleCount, int stencilSize)
	{
		_display = display;
		SampleCount = sampleCount;
		StencilSize = stencilSize;
		GlInterface = glInterface;
	}

	public void ClearContext()
	{
		_display.LegacyClearCurrentContext();
	}

	public GlContext CreateSharedContext(GlContext share)
	{
		return new GlContext(this, share, _display.CreateContext(share.Context), share.Version);
	}
}
