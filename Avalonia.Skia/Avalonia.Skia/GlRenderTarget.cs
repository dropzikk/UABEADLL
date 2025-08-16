using System;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Surfaces;
using SkiaSharp;

namespace Avalonia.Skia;

internal class GlRenderTarget : ISkiaGpuRenderTarget, IDisposable
{
	private class GlGpuSession : ISkiaGpuRenderSession, IDisposable
	{
		private readonly GRBackendRenderTarget _backendRenderTarget;

		private readonly SKSurface _surface;

		private readonly IGlPlatformSurfaceRenderingSession _glSession;

		public GRSurfaceOrigin SurfaceOrigin { get; }

		public GRContext GrContext { get; }

		public SKSurface SkSurface => _surface;

		public double ScaleFactor => _glSession.Scaling;

		public GlGpuSession(GRContext grContext, GRBackendRenderTarget backendRenderTarget, SKSurface surface, IGlPlatformSurfaceRenderingSession glSession)
		{
			GrContext = grContext;
			_backendRenderTarget = backendRenderTarget;
			_surface = surface;
			_glSession = glSession;
			SurfaceOrigin = ((!glSession.IsYFlipped) ? GRSurfaceOrigin.BottomLeft : GRSurfaceOrigin.TopLeft);
		}

		public void Dispose()
		{
			_surface.Canvas.Flush();
			_surface.Dispose();
			_backendRenderTarget.Dispose();
			GrContext.Flush();
			_glSession.Dispose();
		}
	}

	private readonly GRContext _grContext;

	private IGlPlatformSurfaceRenderTarget _surface;

	private static readonly SKSurfaceProperties _surfaceProperties = new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal);

	public bool IsCorrupted => (_surface as IGlPlatformSurfaceRenderTargetWithCorruptionInfo)?.IsCorrupted ?? false;

	public GlRenderTarget(GRContext grContext, IGlContext glContext, IGlPlatformSurface glSurface)
	{
		_grContext = grContext;
		using (glContext.EnsureCurrent())
		{
			_surface = glSurface.CreateGlRenderTarget(glContext);
		}
	}

	public void Dispose()
	{
		_surface.Dispose();
	}

	public ISkiaGpuRenderSession BeginRenderingSession()
	{
		IGlPlatformSurfaceRenderingSession glPlatformSurfaceRenderingSession = _surface.BeginDraw();
		bool flag = false;
		try
		{
			IGlContext context = glPlatformSurfaceRenderingSession.Context;
			context.GlInterface.GetIntegerv(36006, out var rv);
			PixelSize size = glPlatformSurfaceRenderingSession.Size;
			SKColorType colorType = SKColorType.Rgba8888;
			double scaling = glPlatformSurfaceRenderingSession.Scaling;
			if (size.Width <= 0 || size.Height <= 0 || scaling < 0.0)
			{
				glPlatformSurfaceRenderingSession.Dispose();
				throw new InvalidOperationException($"Can't create drawing context for surface with {size} size and {scaling} scaling");
			}
			lock (_grContext)
			{
				_grContext.ResetContext();
				int num = context.SampleCount;
				int maxSurfaceSampleCount = _grContext.GetMaxSurfaceSampleCount(colorType);
				if (num > maxSurfaceSampleCount)
				{
					num = maxSurfaceSampleCount;
				}
				GRBackendRenderTarget gRBackendRenderTarget = new GRBackendRenderTarget(glInfo: new GRGlFramebufferInfo((uint)rv, colorType.ToGlSizedFormat()), width: size.Width, height: size.Height, sampleCount: num, stencilBits: context.StencilSize);
				SKSurface surface = SKSurface.Create(_grContext, gRBackendRenderTarget, (!glPlatformSurfaceRenderingSession.IsYFlipped) ? GRSurfaceOrigin.BottomLeft : GRSurfaceOrigin.TopLeft, colorType, _surfaceProperties);
				flag = true;
				return new GlGpuSession(_grContext, gRBackendRenderTarget, surface, glPlatformSurfaceRenderingSession);
			}
		}
		finally
		{
			if (!flag)
			{
				glPlatformSurfaceRenderingSession.Dispose();
			}
		}
	}
}
