using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Platform;

namespace Avalonia.Skia;

internal class SkiaContext : IPlatformRenderInterfaceContext, IOptionalFeatureProvider, IDisposable
{
	private ISkiaGpu? _gpu;

	public bool IsLost => _gpu?.IsLost ?? false;

	public SkiaContext(ISkiaGpu? gpu)
	{
		_gpu = gpu;
	}

	public void Dispose()
	{
		_gpu?.Dispose();
		_gpu = null;
	}

	public IRenderTarget CreateRenderTarget(IEnumerable<object> surfaces)
	{
		if (!(surfaces is IList))
		{
			surfaces = surfaces.ToList();
		}
		ISkiaGpuRenderTarget skiaGpuRenderTarget = _gpu?.TryCreateRenderTarget(surfaces);
		if (skiaGpuRenderTarget != null)
		{
			return new SkiaGpuRenderTarget(_gpu, skiaGpuRenderTarget);
		}
		foreach (object surface in surfaces)
		{
			if (surface is IFramebufferPlatformSurface platformSurface)
			{
				return new FramebufferRenderTarget(platformSurface);
			}
		}
		throw new NotSupportedException("Don't know how to create a Skia render target from any of provided surfaces");
	}

	public object? TryGetFeature(Type featureType)
	{
		return _gpu?.TryGetFeature(featureType);
	}
}
