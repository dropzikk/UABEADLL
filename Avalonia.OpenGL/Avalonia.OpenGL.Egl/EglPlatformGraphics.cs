using System;
using Avalonia.Logging;
using Avalonia.Platform;

namespace Avalonia.OpenGL.Egl;

public sealed class EglPlatformGraphics : IPlatformGraphics
{
	private readonly EglDisplay _display;

	public bool UsesSharedContext => false;

	public IPlatformGraphicsContext CreateContext()
	{
		return _display.CreateContext(null);
	}

	public IPlatformGraphicsContext GetSharedContext()
	{
		throw new NotSupportedException();
	}

	public EglPlatformGraphics(EglDisplay display)
	{
		_display = display;
	}

	public static EglPlatformGraphics? TryCreate()
	{
		return TryCreate(() => new EglDisplay(new EglDisplayCreationOptions
		{
			Egl = new EglInterface(),
			SupportsMultipleContexts = true,
			SupportsContextSharing = true
		}));
	}

	public static EglPlatformGraphics? TryCreate(Func<EglDisplay> displayFactory)
	{
		try
		{
			return new EglPlatformGraphics(displayFactory());
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log(null, "Unable to initialize EGL-based rendering: {0}", propertyValue);
			return null;
		}
	}
}
