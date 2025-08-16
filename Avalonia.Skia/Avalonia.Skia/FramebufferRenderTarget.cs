using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Platform;
using Avalonia.Reactive;
using SkiaSharp;

namespace Avalonia.Skia;

internal class FramebufferRenderTarget : IRenderTarget, IDisposable
{
	private class PixelFormatConversionShim : IDisposable
	{
		private readonly SKBitmap _bitmap;

		private readonly SKImageInfo _destinationInfo;

		private readonly IntPtr _framebufferAddress;

		public SKSurface Surface { get; }

		public IDisposable SurfaceCopyHandler { get; }

		public PixelFormatConversionShim(SKImageInfo destinationInfo, IntPtr framebufferAddress)
		{
			_destinationInfo = destinationInfo;
			_framebufferAddress = framebufferAddress;
			_bitmap = new SKBitmap(destinationInfo.Width, destinationInfo.Height);
			if (!_bitmap.CanCopyTo(destinationInfo.ColorType))
			{
				SKColorType colorType = _bitmap.ColorType;
				_bitmap.Dispose();
				throw new Exception($"Unable to create pixel format shim for conversion from {colorType} to {destinationInfo.ColorType}");
			}
			Surface = SKSurface.Create(_bitmap.Info, _bitmap.GetPixels(), _bitmap.RowBytes, new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal));
			if (Surface == null)
			{
				SKColorType colorType = _bitmap.ColorType;
				_bitmap.Dispose();
				throw new Exception($"Unable to create pixel format shim surface for conversion from {colorType} to {destinationInfo.ColorType}");
			}
			SurfaceCopyHandler = Disposable.Create(CopySurface);
		}

		public void Dispose()
		{
			Surface.Dispose();
			_bitmap.Dispose();
		}

		private void CopySurface()
		{
			using SKImage sKImage = Surface.Snapshot();
			sKImage.ReadPixels(_destinationInfo, _framebufferAddress, _destinationInfo.RowBytes, 0, 0, SKImageCachingHint.Disallow);
		}
	}

	private SKImageInfo _currentImageInfo;

	private IntPtr _currentFramebufferAddress;

	private SKSurface? _framebufferSurface;

	private PixelFormatConversionShim? _conversionShim;

	private IDisposable? _preFramebufferCopyHandler;

	private IFramebufferRenderTarget? _renderTarget;

	public bool IsCorrupted => false;

	public FramebufferRenderTarget(IFramebufferPlatformSurface platformSurface)
	{
		_renderTarget = platformSurface.CreateFramebufferRenderTarget();
	}

	public void Dispose()
	{
		_renderTarget?.Dispose();
		_renderTarget = null;
		FreeSurface();
	}

	public IDrawingContextImpl CreateDrawingContext()
	{
		if (_renderTarget == null)
		{
			throw new ObjectDisposedException("FramebufferRenderTarget");
		}
		ILockedFramebuffer lockedFramebuffer = _renderTarget.Lock();
		SKImageInfo desiredImageInfo = new SKImageInfo(lockedFramebuffer.Size.Width, lockedFramebuffer.Size.Height, lockedFramebuffer.Format.ToSkColorType(), (lockedFramebuffer.Format == PixelFormat.Rgb565) ? SKAlphaType.Opaque : SKAlphaType.Premul);
		CreateSurface(desiredImageInfo, lockedFramebuffer);
		SKCanvas canvas = _framebufferSurface.Canvas;
		canvas.RestoreToCount(-1);
		canvas.Save();
		canvas.ResetMatrix();
		DrawingContextImpl.CreateInfo createInfo = default(DrawingContextImpl.CreateInfo);
		createInfo.Surface = _framebufferSurface;
		createInfo.Dpi = lockedFramebuffer.Dpi;
		return new DrawingContextImpl(createInfo, _preFramebufferCopyHandler, canvas, lockedFramebuffer);
	}

	private static bool AreImageInfosCompatible(SKImageInfo currentImageInfo, SKImageInfo desiredImageInfo)
	{
		if (currentImageInfo.Width == desiredImageInfo.Width && currentImageInfo.Height == desiredImageInfo.Height)
		{
			return currentImageInfo.ColorType == desiredImageInfo.ColorType;
		}
		return false;
	}

	[MemberNotNull("_framebufferSurface")]
	private void CreateSurface(SKImageInfo desiredImageInfo, ILockedFramebuffer framebuffer)
	{
		if (_framebufferSurface == null || !AreImageInfosCompatible(_currentImageInfo, desiredImageInfo) || !(_currentFramebufferAddress == framebuffer.Address))
		{
			FreeSurface();
			_currentFramebufferAddress = framebuffer.Address;
			SKSurface sKSurface = SKSurface.Create(desiredImageInfo, _currentFramebufferAddress, framebuffer.RowBytes, new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal));
			if (sKSurface == null)
			{
				_conversionShim = new PixelFormatConversionShim(desiredImageInfo, framebuffer.Address);
				_preFramebufferCopyHandler = _conversionShim.SurfaceCopyHandler;
				sKSurface = _conversionShim.Surface;
			}
			_framebufferSurface = sKSurface ?? throw new Exception("Unable to create a surface for pixel format " + framebuffer.Format.ToString() + " or pixel format translator");
			_currentImageInfo = desiredImageInfo;
		}
	}

	private void FreeSurface()
	{
		_conversionShim?.Dispose();
		_conversionShim = null;
		_preFramebufferCopyHandler = null;
		_framebufferSurface?.Dispose();
		_framebufferSurface = null;
		_currentFramebufferAddress = IntPtr.Zero;
	}
}
