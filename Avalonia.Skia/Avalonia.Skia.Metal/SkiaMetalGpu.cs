using System;
using System.Collections.Generic;
using Avalonia.Metal;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia.Metal;

internal class SkiaMetalGpu : ISkiaGpu, IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	public class SkiaMetalRenderTarget : ISkiaGpuRenderTarget, IDisposable
	{
		private readonly SkiaMetalGpu _gpu;

		private IMetalPlatformSurfaceRenderTarget? _target;

		public bool IsCorrupted => false;

		public SkiaMetalRenderTarget(SkiaMetalGpu gpu, IMetalPlatformSurfaceRenderTarget target)
		{
			_gpu = gpu;
			_target = target;
		}

		public void Dispose()
		{
			_target?.Dispose();
			_target = null;
		}

		public ISkiaGpuRenderSession BeginRenderingSession()
		{
			IMetalPlatformSurfaceRenderingSession metalPlatformSurfaceRenderingSession = (_target ?? throw new ObjectDisposedException("SkiaMetalRenderTarget")).BeginRendering();
			GRBackendRenderTarget renderTarget = _gpu._api.CreateBackendRenderTarget(metalPlatformSurfaceRenderingSession.Size.Width, metalPlatformSurfaceRenderingSession.Size.Height, 1, metalPlatformSurfaceRenderingSession.Texture);
			SKSurface surface = SKSurface.Create(_gpu._context, renderTarget, metalPlatformSurfaceRenderingSession.IsYFlipped ? GRSurfaceOrigin.BottomLeft : GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888);
			return new SkiaMetalRenderSession(_gpu, surface, metalPlatformSurfaceRenderingSession);
		}
	}

	internal class SkiaMetalRenderSession : ISkiaGpuRenderSession, IDisposable
	{
		private readonly SkiaMetalGpu _gpu;

		private SKSurface? _surface;

		private IMetalPlatformSurfaceRenderingSession? _session;

		public GRContext GrContext => _gpu._context;

		public SKSurface SkSurface => _surface;

		public double ScaleFactor => _session.Scaling;

		public GRSurfaceOrigin SurfaceOrigin
		{
			get
			{
				if (!_session.IsYFlipped)
				{
					return GRSurfaceOrigin.TopLeft;
				}
				return GRSurfaceOrigin.BottomLeft;
			}
		}

		public SkiaMetalRenderSession(SkiaMetalGpu gpu, SKSurface surface, IMetalPlatformSurfaceRenderingSession session)
		{
			_gpu = gpu;
			_surface = surface;
			_session = session;
		}

		public void Dispose()
		{
			_surface?.Canvas.Flush();
			_surface?.Flush();
			_gpu._context?.Flush();
			_surface?.Dispose();
			_surface = null;
			_session?.Dispose();
			_session = null;
		}
	}

	private SkiaMetalApi _api = new SkiaMetalApi();

	private GRContext? _context;

	private readonly IMetalDevice _device;

	public bool IsLost => false;

	public SkiaMetalGpu(IMetalDevice device, long? maxResourceBytes)
	{
		_context = _api.CreateContext(device.Device, device.CommandQueue, new GRContextOptions
		{
			AvoidStencilBuffers = true
		});
		_device = device;
		if (maxResourceBytes.HasValue)
		{
			_context.SetResourceCacheLimit(maxResourceBytes.Value);
		}
	}

	public void Dispose()
	{
		_context?.Dispose();
		_context = null;
	}

	public object? TryGetFeature(Type featureType)
	{
		return null;
	}

	public IDisposable EnsureCurrent()
	{
		return _device.EnsureCurrent();
	}

	public ISkiaGpuRenderTarget? TryCreateRenderTarget(IEnumerable<object> surfaces)
	{
		foreach (object surface in surfaces)
		{
			if (surface is IMetalPlatformSurface metalPlatformSurface)
			{
				IMetalPlatformSurfaceRenderTarget target = metalPlatformSurface.CreateMetalRenderTarget(_device);
				return new SkiaMetalRenderTarget(this, target);
			}
		}
		return null;
	}

	public ISkiaSurface? TryCreateSurface(PixelSize size, ISkiaGpuRenderSession? session)
	{
		return null;
	}
}
