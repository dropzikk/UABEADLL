using System;
using System.IO;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.Skia.Helpers;
using SkiaSharp;

namespace Avalonia.Skia;

internal class SurfaceRenderTarget : IDrawingContextLayerImpl, IRenderTargetBitmapImpl, IBitmapImpl, IDisposable, IRenderTarget, IDrawableBitmapImpl
{
	private class SkiaSurfaceWrapper : ISkiaSurface, IDisposable
	{
		private SKSurface? _surface;

		public SKSurface Surface => _surface ?? throw new ObjectDisposedException("SkiaSurfaceWrapper");

		public bool CanBlit => false;

		public void Blit(SKCanvas canvas)
		{
			throw new NotSupportedException();
		}

		public SkiaSurfaceWrapper(SKSurface surface)
		{
			_surface = surface;
		}

		public void Dispose()
		{
			_surface?.Dispose();
			_surface = null;
		}
	}

	public struct CreateInfo
	{
		public int Width;

		public int Height;

		public Vector Dpi;

		public PixelFormat? Format;

		public bool DisableTextLcdRendering;

		public GRContext? GrContext;

		public ISkiaGpu? Gpu;

		public ISkiaGpuRenderSession? Session;

		public bool DisableManualFbo;
	}

	private readonly ISkiaSurface _surface;

	private readonly SKCanvas _canvas;

	private readonly bool _disableLcdRendering;

	private readonly GRContext? _grContext;

	private readonly ISkiaGpu? _gpu;

	public bool IsCorrupted => _gpu?.IsLost ?? false;

	public Vector Dpi { get; }

	public PixelSize PixelSize { get; }

	public int Version { get; private set; } = 1;

	public bool CanBlit => true;

	public SurfaceRenderTarget(CreateInfo createInfo)
	{
		PixelSize = new PixelSize(createInfo.Width, createInfo.Height);
		Dpi = createInfo.Dpi;
		_disableLcdRendering = createInfo.DisableTextLcdRendering;
		_grContext = createInfo.GrContext;
		_gpu = createInfo.Gpu;
		ISkiaSurface skiaSurface = null;
		if (!createInfo.DisableManualFbo)
		{
			skiaSurface = _gpu?.TryCreateSurface(PixelSize, createInfo.Session);
		}
		if (skiaSurface == null)
		{
			SKSurface sKSurface = CreateSurface(createInfo.GrContext, PixelSize.Width, PixelSize.Height, createInfo.Format);
			if (sKSurface != null)
			{
				skiaSurface = new SkiaSurfaceWrapper(sKSurface);
			}
		}
		SKCanvas sKCanvas = skiaSurface?.Surface.Canvas;
		if (sKCanvas == null)
		{
			throw new InvalidOperationException("Failed to create Skia render target surface");
		}
		_surface = skiaSurface;
		_canvas = sKCanvas;
	}

	private static SKSurface? CreateSurface(GRContext? gpu, int width, int height, PixelFormat? format)
	{
		SKImageInfo info = MakeImageInfo(width, height, format);
		if (gpu != null)
		{
			return SKSurface.Create(gpu, budgeted: false, info, new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal));
		}
		return SKSurface.Create(info, new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal));
	}

	public void Dispose()
	{
		_canvas.Dispose();
		_surface.Dispose();
	}

	public IDrawingContextImpl CreateDrawingContext()
	{
		_canvas.RestoreToCount(-1);
		_canvas.ResetMatrix();
		DrawingContextImpl.CreateInfo createInfo = default(DrawingContextImpl.CreateInfo);
		createInfo.Surface = _surface.Surface;
		createInfo.Dpi = Dpi;
		createInfo.DisableSubpixelTextRendering = _disableLcdRendering;
		createInfo.GrContext = _grContext;
		createInfo.Gpu = _gpu;
		return new DrawingContextImpl(createInfo, Disposable.Create(delegate
		{
			Version++;
		}));
	}

	public void Save(string fileName, int? quality = null)
	{
		using SKImage image = SnapshotImage();
		ImageSavingHelper.SaveImage(image, fileName, quality);
	}

	public void Save(Stream stream, int? quality = null)
	{
		using SKImage image = SnapshotImage();
		ImageSavingHelper.SaveImage(image, stream, quality);
	}

	public void Blit(IDrawingContextImpl contextImpl)
	{
		DrawingContextImpl drawingContextImpl = (DrawingContextImpl)contextImpl;
		if (_surface.CanBlit)
		{
			_surface.Surface.Canvas.Flush();
			_surface.Blit(drawingContextImpl.Canvas);
			return;
		}
		SKMatrix totalMatrix = drawingContextImpl.Canvas.TotalMatrix;
		drawingContextImpl.Canvas.ResetMatrix();
		_surface.Surface.Draw(drawingContextImpl.Canvas, 0f, 0f, null);
		drawingContextImpl.Canvas.SetMatrix(totalMatrix);
	}

	public void Draw(DrawingContextImpl context, SKRect sourceRect, SKRect destRect, SKPaint paint)
	{
		using SKImage image = SnapshotImage();
		context.Canvas.DrawImage(image, sourceRect, destRect, paint);
	}

	public SKImage SnapshotImage()
	{
		return _surface.Surface.Snapshot();
	}

	private static SKImageInfo MakeImageInfo(int width, int height, PixelFormat? format)
	{
		SKColorType colorType = PixelFormatHelper.ResolveColorType(format);
		return new SKImageInfo(Math.Max(width, 1), Math.Max(height, 1), colorType, SKAlphaType.Premul);
	}
}
