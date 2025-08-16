using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Logging;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Angle;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;

namespace Avalonia.Win32.OpenGl.Angle;

internal class AngleWin32PlatformGraphics : IPlatformGraphics, IPlatformGraphicsOpenGlContextFactory
{
	private readonly Win32AngleEglInterface _egl;

	private readonly AngleWin32EglDisplay? _sharedDisplay;

	private EglContext? _sharedContext;

	[MemberNotNullWhen(true, "_sharedDisplay")]
	public bool UsesSharedContext
	{
		[MemberNotNullWhen(true, "_sharedDisplay")]
		get
		{
			return _sharedDisplay != null;
		}
	}

	public AngleOptions.PlatformApi PlatformApi { get; }

	public IPlatformGraphicsContext CreateContext()
	{
		if (UsesSharedContext)
		{
			throw new InvalidOperationException();
		}
		AngleWin32EglDisplay angleWin32EglDisplay = AngleWin32EglDisplay.CreateD3D11Display(_egl);
		bool flag = false;
		try
		{
			EglContext result = angleWin32EglDisplay.CreateContext(new EglContextOptions
			{
				DisposeCallback = angleWin32EglDisplay.Dispose,
				ExtraFeatures = new Dictionary<Type, Func<EglContext, object>>
				{
					[typeof(IGlPlatformSurfaceRenderTargetFactory)] = (EglContext _) => new AngleD3DTextureFeature(),
					[typeof(IGlContextExternalObjectsFeature)] = (EglContext context) => new AngleExternalObjectsFeature(context)
				}
			});
			flag = true;
			return result;
		}
		finally
		{
			if (!flag)
			{
				angleWin32EglDisplay.Dispose();
			}
		}
	}

	public IPlatformGraphicsContext GetSharedContext()
	{
		if (!UsesSharedContext)
		{
			throw new InvalidOperationException();
		}
		if (_sharedContext == null || _sharedContext.IsLost)
		{
			_sharedContext?.Dispose();
			_sharedContext = null;
			_sharedContext = _sharedDisplay.CreateContext(new EglContextOptions());
		}
		return _sharedContext;
	}

	public AngleWin32PlatformGraphics(Win32AngleEglInterface egl, AngleWin32EglDisplay display)
		: this(egl, display.PlatformApi)
	{
		_sharedDisplay = display;
	}

	public AngleWin32PlatformGraphics(Win32AngleEglInterface egl, AngleOptions.PlatformApi api)
	{
		_egl = egl;
		PlatformApi = api;
	}

	public static AngleWin32PlatformGraphics? TryCreate(AngleOptions? options)
	{
		Win32AngleEglInterface egl;
		try
		{
			egl = new Win32AngleEglInterface();
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log(null, "Unable to load ANGLE: {0}", propertyValue);
			return null;
		}
		foreach (AngleOptions.PlatformApi item in (options?.AllowedPlatformApis ?? new AngleOptions.PlatformApi[1] { AngleOptions.PlatformApi.DirectX11 }).Distinct())
		{
			if (item == AngleOptions.PlatformApi.DirectX11)
			{
				try
				{
					using AngleWin32EglDisplay angleWin32EglDisplay = AngleWin32EglDisplay.CreateD3D11Display(egl);
					using EglContext eglContext = angleWin32EglDisplay.CreateContext(new EglContextOptions());
					eglContext.MakeCurrent().Dispose();
				}
				catch (Exception propertyValue2)
				{
					Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log(null, "Unable to initialize ANGLE-based rendering with DirectX11 : {0}", propertyValue2);
					continue;
				}
				return new AngleWin32PlatformGraphics(egl, AngleOptions.PlatformApi.DirectX11);
			}
			AngleWin32EglDisplay angleWin32EglDisplay2 = null;
			try
			{
				angleWin32EglDisplay2 = AngleWin32EglDisplay.CreateD3D9Display(egl);
				using (EglContext eglContext2 = angleWin32EglDisplay2.CreateContext(new EglContextOptions()))
				{
					eglContext2.MakeCurrent().Dispose();
				}
				return new AngleWin32PlatformGraphics(egl, angleWin32EglDisplay2);
			}
			catch (Exception propertyValue3)
			{
				angleWin32EglDisplay2?.Dispose();
				Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log(null, "Unable to initialize ANGLE-based rendering with DirectX9 : {0}", propertyValue3);
			}
		}
		return null;
	}

	public IGlContext CreateContext(IEnumerable<GlVersion>? versions)
	{
		if (UsesSharedContext)
		{
			throw new InvalidOperationException();
		}
		if (versions != null && versions.All((GlVersion v) => v.Type != GlProfileType.OpenGLES || v.Major != 3))
		{
			throw new OpenGlException("Unable to create context with requested version");
		}
		return (IGlContext)CreateContext();
	}
}
