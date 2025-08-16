using System;
using System.Collections.Generic;
using Avalonia.Logging;
using Avalonia.OpenGL;
using Avalonia.Platform;

namespace Avalonia.X11.Glx;

internal class GlxPlatformGraphics : IPlatformGraphics
{
	public GlxDisplay Display { get; private set; }

	public bool CanCreateContexts => true;

	public bool CanShareContexts => true;

	public bool UsesSharedContext => false;

	IPlatformGraphicsContext IPlatformGraphics.CreateContext()
	{
		return Display.CreateContext();
	}

	public IPlatformGraphicsContext GetSharedContext()
	{
		throw new NotSupportedException();
	}

	public static GlxPlatformGraphics TryCreate(X11Info x11, IList<GlVersion> glProfiles)
	{
		try
		{
			GlxDisplay display = new GlxDisplay(x11, glProfiles);
			return new GlxPlatformGraphics
			{
				Display = display
			};
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log(null, "Unable to initialize GLX-based rendering: {0}", propertyValue);
			return null;
		}
	}
}
