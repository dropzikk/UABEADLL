using System;
using System.Collections.Generic;
using Avalonia.Platform;
using Avalonia.Reactive;

namespace Avalonia.Rendering;

internal class PlatformRenderInterfaceContextManager
{
	private readonly IPlatformGraphics? _graphics;

	private IPlatformRenderInterfaceContext? _backend;

	private OwnedDisposable<IPlatformGraphicsContext>? _gpuContext;

	public IPlatformRenderInterfaceContext Value
	{
		get
		{
			EnsureValidBackendContext();
			return _backend;
		}
	}

	internal IPlatformGraphicsContext? GpuContext => _gpuContext?.Value;

	public PlatformRenderInterfaceContextManager(IPlatformGraphics? graphics)
	{
		_graphics = graphics;
	}

	public void EnsureValidBackendContext()
	{
		if (_backend != null)
		{
			ref OwnedDisposable<IPlatformGraphicsContext>? gpuContext = ref _gpuContext;
			if (!gpuContext.HasValue || !gpuContext.GetValueOrDefault().Value.IsLost)
			{
				return;
			}
		}
		_backend?.Dispose();
		_backend = null;
		_gpuContext?.Dispose();
		_gpuContext = null;
		if (_graphics != null)
		{
			if (_graphics.UsesSharedContext)
			{
				_gpuContext = new OwnedDisposable<IPlatformGraphicsContext>(_graphics.GetSharedContext(), owns: false);
			}
			else
			{
				_gpuContext = new OwnedDisposable<IPlatformGraphicsContext>(_graphics.CreateContext(), owns: true);
			}
		}
		_backend = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().CreateBackendContext(_gpuContext?.Value);
	}

	public IDisposable EnsureCurrent()
	{
		EnsureValidBackendContext();
		if (_gpuContext.HasValue)
		{
			return _gpuContext.Value.Value.EnsureCurrent();
		}
		return Disposable.Empty;
	}

	public IRenderTarget CreateRenderTarget(IEnumerable<object> surfaces)
	{
		EnsureValidBackendContext();
		return _backend.CreateRenderTarget(surfaces);
	}
}
