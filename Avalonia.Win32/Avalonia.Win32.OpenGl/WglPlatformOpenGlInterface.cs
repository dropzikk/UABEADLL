using System;
using System.Linq;
using Avalonia.Logging;
using Avalonia.OpenGL;
using Avalonia.Platform;

namespace Avalonia.Win32.OpenGl;

internal class WglPlatformOpenGlInterface : IPlatformGraphics
{
	public WglContext PrimaryContext { get; }

	public bool UsesSharedContext => false;

	IPlatformGraphicsContext IPlatformGraphics.CreateContext()
	{
		return CreateContext();
	}

	public IPlatformGraphicsContext GetSharedContext()
	{
		throw new NotSupportedException();
	}

	public IGlContext CreateContext()
	{
		return WglDisplay.CreateContext(new GlVersion[1] { PrimaryContext.Version }, null) ?? throw new OpenGlException("Unable to create additional WGL context");
	}

	private WglPlatformOpenGlInterface(WglContext primary)
	{
		PrimaryContext = primary;
	}

	public static WglPlatformOpenGlInterface? TryCreate()
	{
		try
		{
			WglContext wglContext = WglDisplay.CreateContext((AvaloniaLocator.Current.GetService<Win32PlatformOptions>() ?? new Win32PlatformOptions()).WglProfiles.ToArray(), null);
			if (wglContext != null)
			{
				return new WglPlatformOpenGlInterface(wglContext);
			}
		}
		catch (Exception ex)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("WGL", "Unable to initialize WGL: " + ex);
		}
		return null;
	}
}
