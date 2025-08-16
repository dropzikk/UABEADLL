using System;
using System.Collections.Generic;
using Avalonia.OpenGL;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;

namespace Avalonia.Skia;

internal class GlSkiaExternalObjectsFeature : IExternalObjectsRenderInterfaceContextFeature
{
	private readonly GlSkiaGpu _gpu;

	private readonly IGlContextExternalObjectsFeature? _feature;

	public IReadOnlyList<string> SupportedImageHandleTypes => _feature?.SupportedImportableExternalImageTypes ?? Array.Empty<string>();

	public IReadOnlyList<string> SupportedSemaphoreTypes => _feature?.SupportedImportableExternalSemaphoreTypes ?? Array.Empty<string>();

	public byte[]? DeviceUuid => _feature?.DeviceUuid;

	public byte[]? DeviceLuid => _feature?.DeviceLuid;

	public GlSkiaExternalObjectsFeature(GlSkiaGpu gpu, IGlContextExternalObjectsFeature? feature)
	{
		_gpu = gpu;
		_feature = feature;
	}

	public IPlatformRenderInterfaceImportedImage ImportImage(IPlatformHandle handle, PlatformGraphicsExternalImageProperties properties)
	{
		if (_feature == null)
		{
			throw new NotSupportedException("Importing this platform handle is not supported");
		}
		using (_gpu.EnsureCurrent())
		{
			IGlExternalImageTexture image = _feature.ImportImage(handle, properties);
			return new GlSkiaImportedImage(_gpu, image);
		}
	}

	public IPlatformRenderInterfaceImportedImage ImportImage(ICompositionImportableSharedGpuContextImage image)
	{
		GlSkiaSharedTextureForComposition glSkiaSharedTextureForComposition = (GlSkiaSharedTextureForComposition)image;
		if (!glSkiaSharedTextureForComposition.Context.IsSharedWith(_gpu.GlContext))
		{
			throw new InvalidOperationException("Contexts do not belong to the same share group");
		}
		return new GlSkiaImportedImage(_gpu, glSkiaSharedTextureForComposition);
	}

	public IPlatformRenderInterfaceImportedSemaphore ImportSemaphore(IPlatformHandle handle)
	{
		if (_feature == null)
		{
			throw new NotSupportedException("Importing this platform handle is not supported");
		}
		using (_gpu.EnsureCurrent())
		{
			IGlExternalSemaphore semaphore = _feature.ImportSemaphore(handle);
			return new GlSkiaImportedSemaphore(_gpu, semaphore);
		}
	}

	public CompositionGpuImportedImageSynchronizationCapabilities GetSynchronizationCapabilities(string imageHandleType)
	{
		return _feature?.GetSynchronizationCapabilities(imageHandleType) ?? ((CompositionGpuImportedImageSynchronizationCapabilities)0);
	}
}
