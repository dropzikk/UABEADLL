using System;
using Avalonia.Platform;
using Avalonia.Reactive;
using SkiaSharp;

namespace Avalonia.Skia;

internal class PictureRenderTarget : IDisposable
{
	private readonly ISkiaGpu? _gpu;

	private readonly GRContext? _grContext;

	private readonly Vector _dpi;

	private SKPicture? _picture;

	public PictureRenderTarget(ISkiaGpu? gpu, GRContext? grContext, Vector dpi)
	{
		_gpu = gpu;
		_grContext = grContext;
		_dpi = dpi;
	}

	public SKPicture GetPicture()
	{
		SKPicture? result = _picture ?? throw new InvalidOperationException();
		_picture = null;
		return result;
	}

	public IDrawingContextImpl CreateDrawingContext(Size size)
	{
		SKPictureRecorder recorder = new SKPictureRecorder();
		SKCanvas canvas = recorder.BeginRecording(new SKRect(0f, 0f, (float)(size.Width * _dpi.X / 96.0), (float)(size.Height * _dpi.Y / 96.0)));
		canvas.RestoreToCount(-1);
		canvas.ResetMatrix();
		DrawingContextImpl.CreateInfo createInfo = default(DrawingContextImpl.CreateInfo);
		createInfo.Canvas = canvas;
		createInfo.Dpi = _dpi;
		createInfo.DisableSubpixelTextRendering = true;
		createInfo.GrContext = _grContext;
		createInfo.Gpu = _gpu;
		return new DrawingContextImpl(createInfo, Disposable.Create(delegate
		{
			_picture = recorder.EndRecording();
			canvas.Dispose();
			recorder.Dispose();
		}));
	}

	public void Dispose()
	{
		_picture?.Dispose();
	}
}
