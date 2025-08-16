using System;
using Avalonia.OpenGL;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class GlSkiaImportedImage : IPlatformRenderInterfaceImportedImage, IPlatformRenderInterfaceImportedObject, IDisposable
{
	private readonly GlSkiaSharedTextureForComposition? _sharedTexture;

	private readonly GlSkiaGpu _gpu;

	private readonly IGlExternalImageTexture? _image;

	public GlSkiaImportedImage(GlSkiaGpu gpu, IGlExternalImageTexture image)
	{
		_gpu = gpu;
		_image = image;
	}

	public GlSkiaImportedImage(GlSkiaGpu gpu, GlSkiaSharedTextureForComposition sharedTexture)
	{
		_gpu = gpu;
		_sharedTexture = sharedTexture;
	}

	public void Dispose()
	{
		_image?.Dispose();
		_sharedTexture?.Dispose(_gpu.GlContext);
	}

	private SKColorType ConvertColorType(PlatformGraphicsExternalImageFormat format)
	{
		return format switch
		{
			PlatformGraphicsExternalImageFormat.B8G8R8A8UNorm => SKColorType.Bgra8888, 
			PlatformGraphicsExternalImageFormat.R8G8B8A8UNorm => SKColorType.Rgba8888, 
			_ => SKColorType.Rgba8888, 
		};
	}

	private SKSurface? TryCreateSurface(int textureId, int format, int width, int height, bool topLeft)
	{
		GRSurfaceOrigin origin = ((!topLeft) ? GRSurfaceOrigin.BottomLeft : GRSurfaceOrigin.TopLeft);
		using GRBackendTexture texture = new GRBackendTexture(width, height, mipmapped: false, new GRGlTextureInfo(3553u, (uint)textureId, (uint)format));
		SKSurface sKSurface = SKSurface.Create(_gpu.GrContext, texture, origin, SKColorType.Rgba8888);
		if (sKSurface != null)
		{
			return sKSurface;
		}
		using GRBackendTexture texture2 = new GRBackendTexture(width, height, mipmapped: false, new GRGlTextureInfo(3553u, (uint)textureId));
		return SKSurface.Create(_gpu.GrContext, texture2, origin, SKColorType.Rgba8888);
	}

	private IBitmapImpl TakeSnapshot()
	{
		int width = _image?.Properties.Width ?? _sharedTexture.Size.Width;
		int height = _image?.Properties.Height ?? _sharedTexture.Size.Height;
		int format = _image?.InternalFormat ?? _sharedTexture.InternalFormat;
		int num = _image?.TextureId ?? _sharedTexture.TextureId;
		bool topLeft = _image?.Properties.TopLeftOrigin ?? false;
		using (new GRBackendTexture(width, height, mipmapped: false, new GRGlTextureInfo(3553u, (uint)num, (uint)format)))
		{
			IBitmapImpl result;
			using (SKSurface sKSurface = TryCreateSurface(num, format, width, height, topLeft))
			{
				if (sKSurface == null)
				{
					throw new OpenGlException("Unable to consume provided texture");
				}
				result = new ImmutableBitmap(sKSurface.Snapshot());
			}
			_gpu.GrContext.Flush();
			_gpu.GlContext.GlInterface.Flush();
			return result;
		}
	}

	public IBitmapImpl SnapshotWithKeyedMutex(uint acquireIndex, uint releaseIndex)
	{
		if (_image == null)
		{
			throw new NotSupportedException("Only supported with an external image");
		}
		using (_gpu.EnsureCurrent())
		{
			_image.AcquireKeyedMutex(acquireIndex);
			try
			{
				return TakeSnapshot();
			}
			finally
			{
				_image.ReleaseKeyedMutex(releaseIndex);
			}
		}
	}

	public IBitmapImpl SnapshotWithSemaphores(IPlatformRenderInterfaceImportedSemaphore waitForSemaphore, IPlatformRenderInterfaceImportedSemaphore signalSemaphore)
	{
		if (_image == null)
		{
			throw new NotSupportedException("Only supported with an external image");
		}
		GlSkiaImportedSemaphore glSkiaImportedSemaphore = (GlSkiaImportedSemaphore)waitForSemaphore;
		GlSkiaImportedSemaphore glSkiaImportedSemaphore2 = (GlSkiaImportedSemaphore)signalSemaphore;
		using (_gpu.EnsureCurrent())
		{
			glSkiaImportedSemaphore.Semaphore.WaitSemaphore(_image);
			try
			{
				return TakeSnapshot();
			}
			finally
			{
				glSkiaImportedSemaphore2.Semaphore.SignalSemaphore(_image);
			}
		}
	}

	public IBitmapImpl SnapshotWithAutomaticSync()
	{
		using (_gpu.EnsureCurrent())
		{
			return TakeSnapshot();
		}
	}
}
