using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia.Logging;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Surfaces;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class GlSkiaGpu : ISkiaGpu, IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider, IOpenGlTextureSharingRenderInterfaceContextFeature
{
	private class SurfaceWrapper : IGlPlatformSurface
	{
		private readonly object _surface;

		public SurfaceWrapper(object surface)
		{
			_surface = surface;
		}

		public IGlPlatformSurfaceRenderTarget CreateGlRenderTarget(IGlContext context)
		{
			return context.TryGetFeature<IGlPlatformSurfaceRenderTargetFactory>().CreateRenderTarget(context, _surface);
		}
	}

	private readonly GRContext _grContext;

	private readonly IGlContext _glContext;

	private readonly List<Action> _postDisposeCallbacks = new List<Action>();

	private bool? _canCreateSurfaces;

	private readonly IExternalObjectsRenderInterfaceContextFeature? _externalObjectsFeature;

	public GRContext GrContext => _grContext;

	public IGlContext GlContext => _glContext;

	public bool CanCreateSharedContext => _glContext.CanCreateSharedContext;

	public bool IsLost => _glContext.IsLost;

	public GlSkiaGpu(IGlContext context, long? maxResourceBytes)
	{
		_glContext = context;
		using (_glContext.EnsureCurrent())
		{
			using (GRGlInterface backendContext = ((context.Version.Type == GlProfileType.OpenGL) ? GRGlInterface.CreateOpenGl((string proc) => context.GlInterface.GetProcAddress(proc)) : GRGlInterface.CreateGles((string proc) => context.GlInterface.GetProcAddress(proc))))
			{
				_grContext = GRContext.CreateGl(backendContext, new GRContextOptions
				{
					AvoidStencilBuffers = true
				});
				if (maxResourceBytes.HasValue)
				{
					_grContext.SetResourceCacheLimit(maxResourceBytes.Value);
				}
			}
			context.TryGetFeature<IGlContextExternalObjectsFeature>(out var rv);
			_externalObjectsFeature = new GlSkiaExternalObjectsFeature(this, rv);
		}
	}

	public ISkiaGpuRenderTarget? TryCreateRenderTarget(IEnumerable<object> surfaces)
	{
		IGlPlatformSurfaceRenderTargetFactory glPlatformSurfaceRenderTargetFactory = _glContext.TryGetFeature<IGlPlatformSurfaceRenderTargetFactory>();
		foreach (object surface in surfaces)
		{
			if (glPlatformSurfaceRenderTargetFactory != null && glPlatformSurfaceRenderTargetFactory.CanRenderToSurface(_glContext, surface))
			{
				return new GlRenderTarget(_grContext, _glContext, new SurfaceWrapper(surface));
			}
			if (surface is IGlPlatformSurface glSurface)
			{
				return new GlRenderTarget(_grContext, _glContext, glSurface);
			}
		}
		return null;
	}

	public ISkiaSurface? TryCreateSurface(PixelSize size, ISkiaGpuRenderSession? session)
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return null;
		}
		if (!_glContext.GlInterface.IsBlitFramebufferAvailable)
		{
			return null;
		}
		size = new PixelSize(Math.Max(size.Width, 1), Math.Max(size.Height, 1));
		if (_canCreateSurfaces == false)
		{
			return null;
		}
		try
		{
			FboSkiaSurface result = new FboSkiaSurface(this, _grContext, _glContext, size, session?.SurfaceOrigin ?? GRSurfaceOrigin.TopLeft);
			_canCreateSurfaces = true;
			return result;
		}
		catch (Exception)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log(this, "Unable to create a Skia-compatible FBO manually");
			bool valueOrDefault = _canCreateSurfaces == true;
			if (!_canCreateSurfaces.HasValue)
			{
				valueOrDefault = false;
				_canCreateSurfaces = valueOrDefault;
			}
			return null;
		}
	}

	public IGlContext? CreateSharedContext(IEnumerable<GlVersion>? preferredVersions = null)
	{
		return _glContext.CreateSharedContext(preferredVersions);
	}

	public ICompositionImportableOpenGlSharedTexture CreateSharedTextureForComposition(IGlContext context, PixelSize size)
	{
		if (!context.IsSharedWith(_glContext))
		{
			throw new InvalidOperationException("Contexts do not belong to the same share group");
		}
		using (context.EnsureCurrent())
		{
			GlInterface glInterface = context.GlInterface;
			glInterface.GetIntegerv(32873, out var rv);
			int num = glInterface.GenTexture();
			int internalFormat = ((context.Version.Type == GlProfileType.OpenGLES && context.Version.Major == 2) ? 6408 : 32856);
			glInterface.BindTexture(3553, num);
			glInterface.TexImage2D(3553, 0, internalFormat, size.Width, size.Height, 0, 6408, 5121, IntPtr.Zero);
			glInterface.TexParameteri(3553, 10240, 9728);
			glInterface.TexParameteri(3553, 10241, 9728);
			glInterface.BindTexture(3553, rv);
			return new GlSkiaSharedTextureForComposition(context, num, internalFormat, size);
		}
	}

	public void Dispose()
	{
		if (_glContext.IsLost)
		{
			_grContext.AbandonContext();
		}
		else
		{
			_grContext.AbandonContext(releaseResources: true);
		}
		_grContext.Dispose();
		lock (_postDisposeCallbacks)
		{
			foreach (Action postDisposeCallback in _postDisposeCallbacks)
			{
				postDisposeCallback();
			}
		}
	}

	public IDisposable EnsureCurrent()
	{
		return _glContext.EnsureCurrent();
	}

	public object? TryGetFeature(Type featureType)
	{
		if (featureType == typeof(IOpenGlTextureSharingRenderInterfaceContextFeature))
		{
			return this;
		}
		if (featureType == typeof(IExternalObjectsRenderInterfaceContextFeature))
		{
			return _externalObjectsFeature;
		}
		return null;
	}

	public void AddPostDispose(Action dispose)
	{
		lock (_postDisposeCallbacks)
		{
			_postDisposeCallbacks.Add(dispose);
		}
	}
}
