using Avalonia.Native.Interop;
using Avalonia.OpenGL;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class AvaloniaNativeGlPlatformGraphics : IPlatformGraphics
{
	private readonly IAvnGlDisplay _display;

	internal GlDisplay GlDisplay;

	private readonly GlVersion _version;

	public bool UsesSharedContext => true;

	public bool CanShareContexts => true;

	public bool CanCreateContexts => true;

	internal GlContext SharedContext { get; }

	public AvaloniaNativeGlPlatformGraphics(IAvnGlDisplay display)
	{
		_display = display;
		IAvnGlContext avnGlContext = display.CreateContext(null);
		GlInterface glInterface;
		using (avnGlContext.MakeCurrent())
		{
			GlBasicInfoInterface glBasicInfoInterface = new GlBasicInfoInterface(display.GetProcAddress);
			glBasicInfoInterface.GetIntegerv(33307, out var rv);
			glBasicInfoInterface.GetIntegerv(33308, out var rv2);
			_version = new GlVersion(GlProfileType.OpenGL, rv, rv2);
			glInterface = new GlInterface(_version, (string name) => _display.GetProcAddress(name));
		}
		GlDisplay = new GlDisplay(display, glInterface, avnGlContext.SampleCount, avnGlContext.StencilSize);
		SharedContext = (GlContext)CreateContext();
	}

	public IPlatformGraphicsContext CreateContext()
	{
		return new GlContext(GlDisplay, null, _display.CreateContext(null), _version);
	}

	public IPlatformGraphicsContext GetSharedContext()
	{
		return SharedContext;
	}
}
