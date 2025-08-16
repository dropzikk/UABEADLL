using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.Utilities;
using SkiaSharp;

namespace Avalonia.Skia;

internal class DrawingContextImpl : IDrawingContextImpl, IDisposable, IDrawingContextWithAcrylicLikeSupport, IDrawingContextImplWithEffects
{
	public struct CreateInfo
	{
		public SKCanvas? Canvas;

		public SKSurface? Surface;

		public Vector Dpi;

		public bool DisableSubpixelTextRendering;

		public GRContext? GrContext;

		public ISkiaGpu? Gpu;

		public ISkiaGpuRenderSession? CurrentSession;
	}

	private class SkiaLeaseFeature : ISkiaSharpApiLeaseFeature
	{
		private class ApiLease : ISkiaSharpApiLease, IDisposable
		{
			private readonly DrawingContextImpl _context;

			private readonly SKMatrix _revertTransform;

			private bool _isDisposed;

			public SKCanvas SkCanvas => _context.Canvas;

			public GRContext? GrContext => _context.GrContext;

			public SKSurface? SkSurface => _context.Surface;

			public double CurrentOpacity => _context._currentOpacity;

			public ApiLease(DrawingContextImpl context)
			{
				_revertTransform = context.Canvas.TotalMatrix;
				_context = context;
				_context._leased = true;
			}

			public void Dispose()
			{
				if (!_isDisposed)
				{
					_context.Canvas.SetMatrix(_revertTransform);
					_context._leased = false;
					_isDisposed = true;
				}
			}
		}

		private readonly DrawingContextImpl _context;

		public SkiaLeaseFeature(DrawingContextImpl context)
		{
			_context = context;
		}

		public ISkiaSharpApiLease Lease()
		{
			_context.CheckLease();
			return new ApiLease(_context);
		}
	}

	private struct BoxShadowFilter : IDisposable
	{
		public readonly SKPaint Paint;

		private readonly SKImageFilter? _filter;

		public readonly SKClipOperation ClipOperation;

		private BoxShadowFilter(SKPaint paint, SKImageFilter? filter, SKClipOperation clipOperation)
		{
			Paint = paint;
			_filter = filter;
			ClipOperation = clipOperation;
		}

		public static BoxShadowFilter Create(SKPaint paint, BoxShadow shadow, double opacity)
		{
			Color color = shadow.Color;
			SKImageFilter sKImageFilter = SKImageFilter.CreateBlur(SkBlurRadiusToSigma(shadow.Blur), SkBlurRadiusToSigma(shadow.Blur));
			SKColor color2 = new SKColor(color.R, color.G, color.B, (byte)((double)(int)color.A * opacity));
			paint.Reset();
			paint.IsAntialias = true;
			paint.Color = color2;
			paint.ImageFilter = sKImageFilter;
			SKClipOperation clipOperation = (shadow.IsInset ? SKClipOperation.Intersect : SKClipOperation.Difference);
			return new BoxShadowFilter(paint, sKImageFilter, clipOperation);
		}

		public void Dispose()
		{
			Paint?.Reset();
			_filter?.Dispose();
		}
	}

	private readonly struct PaintState : IDisposable
	{
		private readonly SKColor _color;

		private readonly SKShader _shader;

		private readonly SKPaint _paint;

		public PaintState(SKPaint paint, SKColor color, SKShader shader)
		{
			_paint = paint;
			_color = color;
			_shader = shader;
		}

		public void Dispose()
		{
			_paint.Color = _color;
			_paint.Shader = _shader;
		}
	}

	internal struct PaintWrapper : IDisposable
	{
		public readonly SKPaint Paint;

		private IDisposable? _disposable1;

		private IDisposable? _disposable2;

		private IDisposable? _disposable3;

		public PaintWrapper(SKPaint paint)
		{
			Paint = paint;
			_disposable1 = null;
			_disposable2 = null;
			_disposable3 = null;
		}

		public IDisposable ApplyTo(SKPaint paint)
		{
			PaintState paintState = new PaintState(paint, paint.Color, paint.Shader);
			paint.Color = Paint.Color;
			paint.Shader = Paint.Shader;
			return paintState;
		}

		public void AddDisposable(IDisposable disposable)
		{
			if (_disposable1 == null)
			{
				_disposable1 = disposable;
				return;
			}
			if (_disposable2 == null)
			{
				_disposable2 = disposable;
				return;
			}
			if (_disposable3 == null)
			{
				_disposable3 = disposable;
				return;
			}
			throw new InvalidOperationException("PaintWrapper disposable object limit reached. You need to add extra struct fields to support more disposables.");
		}

		public void Dispose()
		{
			Paint?.Reset();
			_disposable1?.Dispose();
			_disposable2?.Dispose();
			_disposable3?.Dispose();
		}
	}

	private IDisposable?[]? _disposables;

	private readonly Vector _dpi;

	private readonly Stack<PaintWrapper> _maskStack = new Stack<PaintWrapper>();

	private readonly Stack<double> _opacityStack = new Stack<double>();

	private readonly Matrix? _postTransform;

	private double _currentOpacity = 1.0;

	private readonly bool _disableSubpixelTextRendering;

	private Matrix _currentTransform;

	private bool _disposed;

	private GRContext? _grContext;

	private readonly ISkiaGpu? _gpu;

	private readonly SKPaint _strokePaint = SKCacheBase<SKPaint, SKPaintCache>.Shared.Get();

	private readonly SKPaint _fillPaint = SKCacheBase<SKPaint, SKPaintCache>.Shared.Get();

	private readonly SKPaint _boxShadowPaint = SKCacheBase<SKPaint, SKPaintCache>.Shared.Get();

	private static SKShader? s_acrylicNoiseShader;

	private readonly ISkiaGpuRenderSession? _session;

	private bool _leased;

	private bool _useOpacitySaveLayer;

	public GRContext? GrContext => _grContext;

	public SKCanvas Canvas { get; }

	public SKSurface? Surface { get; }

	public RenderOptions RenderOptions { get; set; }

	public Matrix Transform
	{
		get
		{
			return _currentTransform;
		}
		set
		{
			CheckLease();
			if (!(_currentTransform == value))
			{
				_currentTransform = value;
				Matrix m = value;
				if (_postTransform.HasValue)
				{
					m *= _postTransform.Value;
				}
				Canvas.SetMatrix(m.ToSKMatrix());
			}
		}
	}

	public DrawingContextImpl(CreateInfo createInfo, params IDisposable?[]? disposables)
	{
		Canvas = createInfo.Canvas ?? createInfo.Surface?.Canvas ?? throw new ArgumentException("Invalid create info - no Canvas provided", "createInfo");
		_dpi = createInfo.Dpi;
		_disposables = disposables;
		_disableSubpixelTextRendering = createInfo.DisableSubpixelTextRendering;
		_grContext = createInfo.GrContext;
		_gpu = createInfo.Gpu;
		if (_grContext != null)
		{
			Monitor.Enter(_grContext);
		}
		Surface = createInfo.Surface;
		_session = createInfo.CurrentSession;
		if (!_dpi.NearlyEquals(SkiaPlatform.DefaultDpi))
		{
			_postTransform = Matrix.CreateScale(_dpi.X / SkiaPlatform.DefaultDpi.X, _dpi.Y / SkiaPlatform.DefaultDpi.Y);
		}
		Transform = Matrix.Identity;
		SkiaOptions service = AvaloniaLocator.Current.GetService<SkiaOptions>();
		if (service != null)
		{
			_useOpacitySaveLayer = service.UseOpacitySaveLayer;
		}
	}

	private void CheckLease()
	{
		if (_leased)
		{
			throw new InvalidOperationException("The underlying graphics API is currently leased");
		}
	}

	public void Clear(Color color)
	{
		CheckLease();
		Canvas.Clear(color.ToSKColor());
	}

	public void DrawBitmap(IBitmapImpl source, double opacity, Rect sourceRect, Rect destRect)
	{
		CheckLease();
		IDrawableBitmapImpl obj = (IDrawableBitmapImpl)source;
		SKRect sourceRect2 = sourceRect.ToSKRect();
		SKRect destRect2 = destRect.ToSKRect();
		SKPaint sKPaint = SKCacheBase<SKPaint, SKPaintCache>.Shared.Get();
		sKPaint.Color = new SKColor(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(255.0 * opacity * (_useOpacitySaveLayer ? 1.0 : _currentOpacity)));
		sKPaint.FilterQuality = RenderOptions.BitmapInterpolationMode.ToSKFilterQuality();
		sKPaint.BlendMode = RenderOptions.BitmapBlendingMode.ToSKBlendMode();
		obj.Draw(this, sourceRect2, destRect2, sKPaint);
		SKCacheBase<SKPaint, SKPaintCache>.Shared.ReturnReset(sKPaint);
	}

	public void DrawBitmap(IBitmapImpl source, IBrush opacityMask, Rect opacityMaskRect, Rect destRect)
	{
		CheckLease();
		PushOpacityMask(opacityMask, opacityMaskRect);
		DrawBitmap(source, 1.0, new Rect(0.0, 0.0, source.PixelSize.Width, source.PixelSize.Height), destRect);
		PopOpacityMask();
	}

	public void DrawLine(IPen? pen, Point p1, Point p2)
	{
		CheckLease();
		if (pen == null)
		{
			return;
		}
		PaintWrapper? paintWrapper = TryCreatePaint(_strokePaint, pen, new Size(Math.Abs(p2.X - p1.X), Math.Abs(p2.Y - p1.Y)));
		if (paintWrapper.HasValue)
		{
			PaintWrapper valueOrDefault = paintWrapper.GetValueOrDefault();
			using (valueOrDefault)
			{
				Canvas.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, valueOrDefault.Paint);
			}
		}
	}

	public void DrawGeometry(IBrush? brush, IPen? pen, IGeometryImpl geometry)
	{
		CheckLease();
		GeometryImpl geometryImpl = (GeometryImpl)geometry;
		Size size = geometry.Bounds.Size;
		if (brush != null && geometryImpl.FillPath != null)
		{
			PaintWrapper paintWrapper = CreatePaint(_fillPaint, brush, size);
			try
			{
				Canvas.DrawPath(geometryImpl.FillPath, paintWrapper.Paint);
			}
			finally
			{
				((IDisposable)paintWrapper/*cast due to .constrained prefix*/).Dispose();
			}
		}
		if (pen == null || geometryImpl.StrokePath == null)
		{
			return;
		}
		PaintWrapper? paintWrapper2 = TryCreatePaint(_strokePaint, pen, size.Inflate(new Thickness(pen.Thickness / 2.0)));
		if (paintWrapper2.HasValue)
		{
			PaintWrapper valueOrDefault = paintWrapper2.GetValueOrDefault();
			using (valueOrDefault)
			{
				Canvas.DrawPath(geometryImpl.StrokePath, valueOrDefault.Paint);
			}
		}
	}

	private static float SkBlurRadiusToSigma(double radius)
	{
		if (radius <= 0.0)
		{
			return 0f;
		}
		return 0.288675f * (float)radius + 0.5f;
	}

	private static SKRect AreaCastingShadowInHole(SKRect hole_rect, float shadow_blur, float shadow_spread, float offsetX, float offsetY)
	{
		SKRect sKRect = hole_rect;
		sKRect.Inflate(shadow_blur, shadow_blur);
		if (shadow_spread < 0f)
		{
			sKRect.Inflate(0f - shadow_spread, 0f - shadow_spread);
		}
		SKRect rect = sKRect;
		rect.Offset(0f - offsetX, 0f - offsetY);
		sKRect.Union(rect);
		return sKRect;
	}

	public void DrawRectangle(IExperimentalAcrylicMaterial? material, RoundedRect rect)
	{
		if (rect.Rect.Height <= 0.0 || rect.Rect.Width <= 0.0)
		{
			return;
		}
		CheckLease();
		SKRect rect2 = rect.Rect.ToSKRect();
		SKRoundRect sKRoundRect = null;
		if (rect.IsRounded)
		{
			sKRoundRect = SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.Get();
			sKRoundRect.SetRectRadii(rect2, new SKPoint[4]
			{
				rect.RadiiTopLeft.ToSKPoint(),
				rect.RadiiTopRight.ToSKPoint(),
				rect.RadiiBottomRight.ToSKPoint(),
				rect.RadiiBottomLeft.ToSKPoint()
			});
		}
		if (material == null)
		{
			return;
		}
		PaintWrapper paintWrapper = CreateAcrylicPaint(_fillPaint, material);
		try
		{
			if (sKRoundRect != null)
			{
				Canvas.DrawRoundRect(sKRoundRect, paintWrapper.Paint);
				SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.Return(sKRoundRect);
			}
			else
			{
				Canvas.DrawRect(rect2, paintWrapper.Paint);
			}
		}
		finally
		{
			((IDisposable)paintWrapper/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public void DrawRectangle(IBrush? brush, IPen? pen, RoundedRect rect, BoxShadows boxShadows = default(BoxShadows))
	{
		if (rect.Rect.Height <= 0.0 || rect.Rect.Width <= 0.0)
		{
			return;
		}
		CheckLease();
		if (rect.Rect.Height > 8192.0 || rect.Rect.Width > 8192.0)
		{
			boxShadows = default(BoxShadows);
		}
		SKRect rectangle = rect.Rect.ToSKRect();
		bool isRounded = rect.IsRounded;
		bool num = rect.IsRounded || boxShadows.HasInsetShadows;
		SKRoundRect sKRoundRect = null;
		if (num)
		{
			sKRoundRect = SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.GetAndSetRadii(in rectangle, in rect);
		}
		BoxShadows.BoxShadowsEnumerator enumerator = boxShadows.GetEnumerator();
		while (enumerator.MoveNext())
		{
			BoxShadow current = enumerator.Current;
			if (!(current != default(BoxShadow)) || current.IsInset)
			{
				continue;
			}
			BoxShadowFilter boxShadowFilter = BoxShadowFilter.Create(_boxShadowPaint, current, _useOpacitySaveLayer ? 1.0 : _currentOpacity);
			try
			{
				float num2 = (float)current.Spread;
				if (current.IsInset)
				{
					num2 = 0f - num2;
				}
				Canvas.Save();
				if (isRounded)
				{
					SKRoundRect andSetRadii = SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.GetAndSetRadii(sKRoundRect.Rect, sKRoundRect.Radii);
					if (num2 != 0f)
					{
						andSetRadii.Inflate(num2, num2);
					}
					Canvas.ClipRoundRect(sKRoundRect, boxShadowFilter.ClipOperation, antialias: true);
					Matrix transform = Transform;
					Transform = transform * Matrix.CreateTranslation(current.OffsetX, current.OffsetY);
					Canvas.DrawRoundRect(andSetRadii, boxShadowFilter.Paint);
					Transform = transform;
					SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.Return(andSetRadii);
				}
				else
				{
					SKRect rect2 = rectangle;
					if (num2 != 0f)
					{
						rect2.Inflate(num2, num2);
					}
					Canvas.ClipRect(rectangle, boxShadowFilter.ClipOperation);
					Matrix transform2 = Transform;
					Transform = transform2 * Matrix.CreateTranslation(current.OffsetX, current.OffsetY);
					Canvas.DrawRect(rect2, boxShadowFilter.Paint);
					Transform = transform2;
				}
				Canvas.Restore();
			}
			finally
			{
				((IDisposable)boxShadowFilter/*cast due to .constrained prefix*/).Dispose();
			}
		}
		if (brush != null)
		{
			PaintWrapper paintWrapper = CreatePaint(_fillPaint, brush, rect.Rect.Size);
			try
			{
				if (isRounded)
				{
					Canvas.DrawRoundRect(sKRoundRect, paintWrapper.Paint);
				}
				else
				{
					Canvas.DrawRect(rectangle, paintWrapper.Paint);
				}
			}
			finally
			{
				((IDisposable)paintWrapper/*cast due to .constrained prefix*/).Dispose();
			}
		}
		enumerator = boxShadows.GetEnumerator();
		while (enumerator.MoveNext())
		{
			BoxShadow current2 = enumerator.Current;
			if (!(current2 != default(BoxShadow)) || !current2.IsInset)
			{
				continue;
			}
			BoxShadowFilter boxShadowFilter2 = BoxShadowFilter.Create(_boxShadowPaint, current2, _useOpacitySaveLayer ? 1.0 : _currentOpacity);
			try
			{
				float num3 = (float)current2.Spread;
				float offsetX = (float)current2.OffsetX;
				float offsetY = (float)current2.OffsetY;
				SKRect rect3 = AreaCastingShadowInHole(rectangle, (float)current2.Blur, num3, offsetX, offsetY);
				Canvas.Save();
				SKRoundRect andSetRadii2 = SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.GetAndSetRadii(sKRoundRect.Rect, sKRoundRect.Radii);
				if (num3 != 0f)
				{
					andSetRadii2.Deflate(num3, num3);
				}
				Canvas.ClipRoundRect(sKRoundRect, boxShadowFilter2.ClipOperation, antialias: true);
				Matrix transform3 = Transform;
				Transform = transform3 * Matrix.CreateTranslation(current2.OffsetX, current2.OffsetY);
				using (SKRoundRect outer = new SKRoundRect(rect3))
				{
					Canvas.DrawRoundRectDifference(outer, andSetRadii2, boxShadowFilter2.Paint);
				}
				Transform = transform3;
				Canvas.Restore();
				SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.Return(andSetRadii2);
			}
			finally
			{
				((IDisposable)boxShadowFilter2/*cast due to .constrained prefix*/).Dispose();
			}
		}
		if (pen != null)
		{
			PaintWrapper? paintWrapper2 = TryCreatePaint(_strokePaint, pen, rect.Rect.Size.Inflate(new Thickness(pen.Thickness / 2.0)));
			if (paintWrapper2.HasValue)
			{
				PaintWrapper valueOrDefault = paintWrapper2.GetValueOrDefault();
				using (valueOrDefault)
				{
					if (isRounded)
					{
						Canvas.DrawRoundRect(sKRoundRect, valueOrDefault.Paint);
					}
					else
					{
						Canvas.DrawRect(rectangle, valueOrDefault.Paint);
					}
				}
			}
		}
		if (sKRoundRect != null)
		{
			SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.Return(sKRoundRect);
		}
	}

	public void DrawEllipse(IBrush? brush, IPen? pen, Rect rect)
	{
		if (rect.Height <= 0.0 || rect.Width <= 0.0)
		{
			return;
		}
		CheckLease();
		SKRect rect2 = rect.ToSKRect();
		if (brush != null)
		{
			PaintWrapper paintWrapper = CreatePaint(_fillPaint, brush, rect.Size);
			try
			{
				Canvas.DrawOval(rect2, paintWrapper.Paint);
			}
			finally
			{
				((IDisposable)paintWrapper/*cast due to .constrained prefix*/).Dispose();
			}
		}
		if (pen == null)
		{
			return;
		}
		PaintWrapper? paintWrapper2 = TryCreatePaint(_strokePaint, pen, rect.Size.Inflate(new Thickness(pen.Thickness / 2.0)));
		if (!paintWrapper2.HasValue)
		{
			return;
		}
		PaintWrapper valueOrDefault = paintWrapper2.GetValueOrDefault();
		using (valueOrDefault)
		{
			Canvas.DrawOval(rect2, valueOrDefault.Paint);
		}
	}

	public void DrawGlyphRun(IBrush? foreground, IGlyphRunImpl glyphRun)
	{
		CheckLease();
		if (foreground == null)
		{
			return;
		}
		PaintWrapper paintWrapper = CreatePaint(_fillPaint, foreground, glyphRun.Bounds.Size);
		try
		{
			GlyphRunImpl glyphRunImpl = (GlyphRunImpl)glyphRun;
			RenderOptions renderOptions = RenderOptions;
			if (_disableSubpixelTextRendering)
			{
				TextRenderingMode textRenderingMode = renderOptions.TextRenderingMode;
				if (textRenderingMode != 0)
				{
					if (textRenderingMode == TextRenderingMode.SubpixelAntialias)
					{
						goto IL_0063;
					}
				}
				else if (renderOptions.EdgeMode == EdgeMode.Antialias || renderOptions.EdgeMode == EdgeMode.Unspecified)
				{
					goto IL_0063;
				}
			}
			goto IL_0071;
			IL_0071:
			SKTextBlob textBlob = glyphRunImpl.GetTextBlob(renderOptions);
			Canvas.DrawText(textBlob, (float)glyphRun.BaselineOrigin.X, (float)glyphRun.BaselineOrigin.Y, paintWrapper.Paint);
			return;
			IL_0063:
			renderOptions = renderOptions with
			{
				TextRenderingMode = TextRenderingMode.Antialias
			};
			goto IL_0071;
		}
		finally
		{
			((IDisposable)paintWrapper/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public IDrawingContextLayerImpl CreateLayer(Size size)
	{
		CheckLease();
		return CreateRenderTarget(size, isLayer: true, null);
	}

	public void PushClip(Rect clip)
	{
		CheckLease();
		Canvas.Save();
		Canvas.ClipRect(clip.ToSKRect());
	}

	public void PushClip(RoundedRect clip)
	{
		CheckLease();
		Canvas.Save();
		SKRect rect = clip.Rect.ToSKRect();
		SKRoundRect sKRoundRect = SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.Get();
		sKRoundRect.SetRectRadii(rect, new SKPoint[4]
		{
			clip.RadiiTopLeft.ToSKPoint(),
			clip.RadiiTopRight.ToSKPoint(),
			clip.RadiiBottomRight.ToSKPoint(),
			clip.RadiiBottomLeft.ToSKPoint()
		});
		Canvas.ClipRoundRect(sKRoundRect, SKClipOperation.Intersect, antialias: true);
		SKCacheBase<SKRoundRect, SKRoundRectCache>.Shared.Return(sKRoundRect);
	}

	public void PopClip()
	{
		CheckLease();
		Canvas.Restore();
	}

	public void PushOpacity(double opacity, Rect? bounds)
	{
		CheckLease();
		if (_useOpacitySaveLayer)
		{
			if (bounds.HasValue)
			{
				SKRect limit = bounds.Value.ToSKRect();
				Canvas.SaveLayer(limit, new SKPaint
				{
					ColorF = new SKColorF(0f, 0f, 0f, (float)opacity)
				});
			}
			else
			{
				Canvas.SaveLayer(new SKPaint
				{
					ColorF = new SKColorF(0f, 0f, 0f, (float)opacity)
				});
			}
		}
		else
		{
			_opacityStack.Push(_currentOpacity);
			_currentOpacity *= opacity;
		}
	}

	public void PopOpacity()
	{
		CheckLease();
		if (_useOpacitySaveLayer)
		{
			Canvas.Restore();
		}
		else
		{
			_currentOpacity = _opacityStack.Pop();
		}
	}

	public virtual void Dispose()
	{
		if (_disposed)
		{
			return;
		}
		CheckLease();
		try
		{
			SKCacheBase<SKPaint, SKPaintCache>.Shared.ReturnReset(_strokePaint);
			SKCacheBase<SKPaint, SKPaintCache>.Shared.ReturnReset(_fillPaint);
			SKCacheBase<SKPaint, SKPaintCache>.Shared.ReturnReset(_boxShadowPaint);
			if (_grContext != null)
			{
				Monitor.Exit(_grContext);
				_grContext = null;
			}
			if (_disposables != null)
			{
				IDisposable[] disposables = _disposables;
				for (int i = 0; i < disposables.Length; i++)
				{
					disposables[i]?.Dispose();
				}
				_disposables = null;
			}
		}
		finally
		{
			_disposed = true;
		}
	}

	public void PushGeometryClip(IGeometryImpl clip)
	{
		CheckLease();
		Canvas.Save();
		Canvas.ClipPath(((GeometryImpl)clip).FillPath, SKClipOperation.Intersect, antialias: true);
	}

	public void PopGeometryClip()
	{
		CheckLease();
		Canvas.Restore();
	}

	public void PushOpacityMask(IBrush mask, Rect bounds)
	{
		CheckLease();
		SKPaint paint = SKCacheBase<SKPaint, SKPaintCache>.Shared.Get();
		Canvas.SaveLayer(bounds.ToSKRect(), paint);
		_maskStack.Push(CreatePaint(paint, mask, bounds.Size));
	}

	public void PopOpacityMask()
	{
		CheckLease();
		SKPaint sKPaint = SKCacheBase<SKPaint, SKPaintCache>.Shared.Get();
		sKPaint.BlendMode = SKBlendMode.DstIn;
		Canvas.SaveLayer(sKPaint);
		SKCacheBase<SKPaint, SKPaintCache>.Shared.ReturnReset(sKPaint);
		PaintWrapper paintWrapper;
		using (paintWrapper = _maskStack.Pop())
		{
			Canvas.DrawPaint(paintWrapper.Paint);
		}
		SKCacheBase<SKPaint, SKPaintCache>.Shared.Return(paintWrapper.Paint);
		Canvas.Restore();
		Canvas.Restore();
	}

	public object? GetFeature(Type t)
	{
		if (t == typeof(ISkiaSharpApiLeaseFeature))
		{
			return new SkiaLeaseFeature(this);
		}
		return null;
	}

	private static void ConfigureGradientBrush(ref PaintWrapper paintWrapper, Size targetSize, IGradientBrush gradientBrush)
	{
		SKShaderTileMode mode = gradientBrush.SpreadMethod.ToSKShaderTileMode();
		SKColor[] array = gradientBrush.GradientStops.Select((IGradientStop s) => s.Color.ToSKColor()).ToArray();
		float[] array2 = gradientBrush.GradientStops.Select((IGradientStop s) => (float)s.Offset).ToArray();
		if (!(gradientBrush is ILinearGradientBrush { StartPoint: var startPoint } linearGradientBrush))
		{
			if (!(gradientBrush is IRadialGradientBrush { Center: var center } radialGradientBrush))
			{
				if (!(gradientBrush is IConicGradientBrush { Center: var center2 } conicGradientBrush))
				{
					return;
				}
				SKPoint center3 = center2.ToPixels(targetSize).ToSKPoint();
				SKMatrix localMatrix = SKMatrix.CreateRotationDegrees((float)(conicGradientBrush.Angle - 90.0), center3.X, center3.Y);
				if (conicGradientBrush.Transform != null)
				{
					Matrix matrix = Matrix.CreateTranslation(conicGradientBrush.TransformOrigin.ToPixels(targetSize));
					Matrix m = -matrix * conicGradientBrush.Transform.Value * matrix;
					localMatrix = localMatrix.PreConcat(m.ToSKMatrix());
				}
				using SKShader shader = SKShader.CreateSweepGradient(center3, array, array2, localMatrix);
				paintWrapper.Paint.Shader = shader;
				return;
			}
			SKPoint sKPoint = center.ToPixels(targetSize).ToSKPoint();
			float num = (float)(radialGradientBrush.Radius * targetSize.Width);
			SKPoint end = radialGradientBrush.GradientOrigin.ToPixels(targetSize).ToSKPoint();
			if (end.Equals(sKPoint))
			{
				if (radialGradientBrush.Transform == null)
				{
					using (SKShader shader2 = SKShader.CreateRadialGradient(sKPoint, num, array, array2, mode))
					{
						paintWrapper.Paint.Shader = shader2;
						return;
					}
				}
				Matrix matrix2 = Matrix.CreateTranslation(radialGradientBrush.TransformOrigin.ToPixels(targetSize));
				Matrix m2 = -matrix2 * radialGradientBrush.Transform.Value * matrix2;
				using SKShader shader3 = SKShader.CreateRadialGradient(sKPoint, num, array, array2, mode, m2.ToSKMatrix());
				paintWrapper.Paint.Shader = shader3;
				return;
			}
			SKColor[] array3 = new SKColor[array.Length];
			Array.Copy(array, array3, array.Length);
			Array.Reverse(array3);
			float[] array4 = new float[array2.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array4[i] = array2[i];
				if (array4[i] > 0f && array4[i] < 1f)
				{
					array4[i] = Math.Abs(1f - array2[i]);
				}
			}
			if (radialGradientBrush.Transform == null)
			{
				using (SKShader shader4 = SKShader.CreateCompose(SKShader.CreateColor(array3[0]), SKShader.CreateTwoPointConicalGradient(sKPoint, num, end, 0f, array3, array4, mode)))
				{
					paintWrapper.Paint.Shader = shader4;
					return;
				}
			}
			Matrix matrix3 = Matrix.CreateTranslation(radialGradientBrush.TransformOrigin.ToPixels(targetSize));
			Matrix m3 = -matrix3 * radialGradientBrush.Transform.Value * matrix3;
			using SKShader shader5 = SKShader.CreateCompose(SKShader.CreateColor(array3[0]), SKShader.CreateTwoPointConicalGradient(sKPoint, num, end, 0f, array3, array4, mode, m3.ToSKMatrix()));
			paintWrapper.Paint.Shader = shader5;
			return;
		}
		SKPoint start = startPoint.ToPixels(targetSize).ToSKPoint();
		SKPoint end2 = linearGradientBrush.EndPoint.ToPixels(targetSize).ToSKPoint();
		if (linearGradientBrush.Transform == null)
		{
			using (SKShader shader6 = SKShader.CreateLinearGradient(start, end2, array, array2, mode))
			{
				paintWrapper.Paint.Shader = shader6;
				return;
			}
		}
		Matrix matrix4 = Matrix.CreateTranslation(linearGradientBrush.TransformOrigin.ToPixels(targetSize));
		Matrix m4 = -matrix4 * linearGradientBrush.Transform.Value * matrix4;
		using SKShader shader7 = SKShader.CreateLinearGradient(start, end2, array, array2, mode, m4.ToSKMatrix());
		paintWrapper.Paint.Shader = shader7;
	}

	private void ConfigureTileBrush(ref PaintWrapper paintWrapper, Size targetSize, ITileBrush tileBrush, IDrawableBitmapImpl tileBrushImage)
	{
		TileBrushCalculator tileBrushCalculator = new TileBrushCalculator(tileBrush, tileBrushImage.PixelSize.ToSizeWithDpi(_dpi), targetSize);
		SurfaceRenderTarget surfaceRenderTarget = CreateRenderTarget(tileBrushCalculator.IntermediateSize, isLayer: false, null);
		paintWrapper.AddDisposable(surfaceRenderTarget);
		using (IDrawingContextImpl drawingContextImpl = surfaceRenderTarget.CreateDrawingContext())
		{
			Rect sourceRect = new Rect(tileBrushImage.PixelSize.ToSizeWithDpi(96.0));
			Rect destRect = new Rect(tileBrushImage.PixelSize.ToSizeWithDpi(_dpi));
			drawingContextImpl.Clear(Colors.Transparent);
			drawingContextImpl.PushClip(tileBrushCalculator.IntermediateClip);
			drawingContextImpl.Transform = tileBrushCalculator.IntermediateTransform;
			drawingContextImpl.RenderOptions = RenderOptions;
			drawingContextImpl.DrawBitmap(tileBrushImage, 1.0, sourceRect, destRect);
			drawingContextImpl.PopClip();
		}
		SKMatrix first = ((tileBrush.TileMode != 0) ? SKMatrix.CreateTranslation(0f - (float)tileBrushCalculator.DestinationRect.X, 0f - (float)tileBrushCalculator.DestinationRect.Y) : SKMatrix.CreateIdentity());
		SKShaderTileMode tileX = ((tileBrush.TileMode != 0) ? ((tileBrush.TileMode != TileMode.FlipX && tileBrush.TileMode != TileMode.FlipXY) ? SKShaderTileMode.Repeat : SKShaderTileMode.Mirror) : SKShaderTileMode.Clamp);
		SKShaderTileMode tileY = ((tileBrush.TileMode != 0) ? ((tileBrush.TileMode != TileMode.FlipY && tileBrush.TileMode != TileMode.FlipXY) ? SKShaderTileMode.Repeat : SKShaderTileMode.Mirror) : SKShaderTileMode.Clamp);
		SKImage sKImage = surfaceRenderTarget.SnapshotImage();
		paintWrapper.AddDisposable(sKImage);
		SKMatrix target = default(SKMatrix);
		SKMatrix.Concat(ref target, first, SKMatrix.CreateScale((float)(96.0 / _dpi.X), (float)(96.0 / _dpi.Y)));
		if (tileBrush.Transform != null)
		{
			Matrix matrix = Matrix.CreateTranslation(tileBrush.TransformOrigin.ToPixels(targetSize));
			Matrix m = -matrix * tileBrush.Transform.Value * matrix;
			target = target.PreConcat(m.ToSKMatrix());
		}
		using SKShader shader = sKImage.ToShader(tileX, tileY, target);
		paintWrapper.Paint.Shader = shader;
	}

	private void ConfigureSceneBrushContent(ref PaintWrapper paintWrapper, ISceneBrushContent content, Size targetSize)
	{
		if (content.UseScalableRasterization)
		{
			ConfigureSceneBrushContentWithPicture(ref paintWrapper, content, targetSize);
		}
		else
		{
			ConfigureSceneBrushContentWithSurface(ref paintWrapper, content, targetSize);
		}
	}

	private void ConfigureSceneBrushContentWithSurface(ref PaintWrapper paintWrapper, ISceneBrushContent content, Size targetSize)
	{
		Rect rect = content.Rect;
		Size size = rect.Size;
		if (!(size.Width >= 1.0) || !(size.Height >= 1.0))
		{
			return;
		}
		using SurfaceRenderTarget surfaceRenderTarget = CreateRenderTarget(size, isLayer: false, null);
		using (IDrawingContextImpl drawingContextImpl = surfaceRenderTarget.CreateDrawingContext())
		{
			drawingContextImpl.RenderOptions = RenderOptions;
			drawingContextImpl.Clear(Colors.Transparent);
			content.Render(drawingContextImpl, (rect.TopLeft == default(Point)) ? ((Matrix?)null) : new Matrix?(Matrix.CreateTranslation(0.0 - rect.X, 0.0 - rect.Y)));
		}
		ConfigureTileBrush(ref paintWrapper, targetSize, content.Brush, surfaceRenderTarget);
	}

	private void ConfigureSceneBrushContentWithPicture(ref PaintWrapper paintWrapper, ISceneBrushContent content, Size targetSize)
	{
		Rect rect = content.Rect;
		Size size = rect.Size;
		if (size.Width <= 0.0 || size.Height <= 0.0)
		{
			paintWrapper.Paint.Color = SKColor.Empty;
			return;
		}
		ITileBrush brush = content.Brush;
		Matrix value = ((rect.TopLeft == default(Point)) ? Matrix.Identity : Matrix.CreateTranslation(0.0 - rect.X, 0.0 - rect.Y));
		TileBrushCalculator tileBrushCalculator = new TileBrushCalculator(brush, size, targetSize);
		value *= tileBrushCalculator.IntermediateTransform;
		using PictureRenderTarget pictureRenderTarget = new PictureRenderTarget(_gpu, _grContext, _dpi);
		using (IDrawingContextImpl drawingContextImpl = pictureRenderTarget.CreateDrawingContext(tileBrushCalculator.IntermediateSize))
		{
			drawingContextImpl.RenderOptions = RenderOptions;
			drawingContextImpl.PushClip(tileBrushCalculator.IntermediateClip);
			content.Render(drawingContextImpl, value);
			drawingContextImpl.PopClip();
		}
		using SKPicture sKPicture = pictureRenderTarget.GetPicture();
		SKMatrix first = ((brush.TileMode != 0) ? SKMatrix.CreateTranslation(0f - (float)tileBrushCalculator.DestinationRect.X, 0f - (float)tileBrushCalculator.DestinationRect.Y) : SKMatrix.CreateIdentity());
		SKShaderTileMode tmx = ((brush.TileMode != 0) ? ((brush.TileMode != TileMode.FlipX && brush.TileMode != TileMode.FlipXY) ? SKShaderTileMode.Repeat : SKShaderTileMode.Mirror) : SKShaderTileMode.Clamp);
		SKShaderTileMode tmy = ((brush.TileMode != 0) ? ((brush.TileMode != TileMode.FlipY && brush.TileMode != TileMode.FlipXY) ? SKShaderTileMode.Repeat : SKShaderTileMode.Mirror) : SKShaderTileMode.Clamp);
		first = SKMatrix.Concat(first, SKMatrix.CreateScale((float)(96.0 / _dpi.X), (float)(96.0 / _dpi.Y)));
		if (brush.Transform != null)
		{
			Matrix matrix = Matrix.CreateTranslation(brush.TransformOrigin.ToPixels(targetSize));
			Matrix m = -matrix * brush.Transform.Value * matrix;
			first = first.PreConcat(m.ToSKMatrix());
		}
		using SKShader shader = sKPicture.ToShader(tmx, tmy, first, new SKRect(0f, 0f, sKPicture.CullRect.Width, sKPicture.CullRect.Height));
		paintWrapper.Paint.FilterQuality = SKFilterQuality.None;
		paintWrapper.Paint.Shader = shader;
	}

	private static SKColorFilter CreateAlphaColorFilter(double opacity)
	{
		if (opacity > 1.0)
		{
			opacity = 1.0;
		}
		byte[] array = new byte[256];
		byte[] array2 = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			array[i] = (byte)i;
			array2[i] = (byte)((double)i * opacity);
		}
		return SKColorFilter.CreateTable(array2, array, array, array);
	}

	private static byte Blend(byte leftColor, byte leftAlpha, byte rightColor, byte rightAlpha)
	{
		double num = (double)(int)leftColor / 255.0;
		double num2 = (double)(int)leftAlpha / 255.0;
		double num3 = (double)(int)rightColor / 255.0;
		double num4 = (double)(int)rightAlpha / 255.0;
		return (byte)((num * num2 + num3 * num4 * (1.0 - num2)) / (num2 + num4 * (1.0 - num2)) * 255.0);
	}

	private static Color Blend(Color left, Color right)
	{
		double num = (double)(int)left.A / 255.0;
		double num2 = (double)(int)right.A / 255.0;
		return new Color((byte)((num + num2 * (1.0 - num)) * 255.0), Blend(left.R, left.A, right.R, right.A), Blend(left.G, left.A, right.G, right.A), Blend(left.B, left.A, right.B, right.A));
	}

	internal PaintWrapper CreateAcrylicPaint(SKPaint paint, IExperimentalAcrylicMaterial material)
	{
		PaintWrapper result = new PaintWrapper(paint);
		paint.IsAntialias = true;
		if (material.BackgroundSource == AcrylicBackgroundSource.Digger)
		{
			_ = material.TintOpacity;
		}
		Color tintColor = material.TintColor;
		SKColor color = new SKColor(tintColor.R, tintColor.G, tintColor.B, tintColor.A);
		if (s_acrylicNoiseShader == null)
		{
			using Stream stream = typeof(DrawingContextImpl).Assembly.GetManifestResourceStream("Avalonia.Skia.Assets.NoiseAsset_256X256_PNG.png");
			using SKBitmap src = SKBitmap.Decode(stream);
			s_acrylicNoiseShader = SKShader.CreateBitmap(src, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat).WithColorFilter(CreateAlphaColorFilter(0.0225));
		}
		using SKShader shaderA = SKShader.CreateColor(new SKColor(material.MaterialColor.R, material.MaterialColor.G, material.MaterialColor.B, material.MaterialColor.A));
		using SKShader shaderB = SKShader.CreateColor(color);
		using SKShader shaderA2 = SKShader.CreateCompose(shaderA, shaderB);
		using SKShader shader = SKShader.CreateCompose(shaderA2, s_acrylicNoiseShader);
		paint.Shader = shader;
		if (material.BackgroundSource == AcrylicBackgroundSource.Digger)
		{
			paint.BlendMode = SKBlendMode.Src;
		}
		return result;
	}

	internal PaintWrapper CreatePaint(SKPaint paint, IBrush brush, Size targetSize)
	{
		PaintWrapper paintWrapper = new PaintWrapper(paint);
		paint.IsAntialias = RenderOptions.EdgeMode != EdgeMode.Aliased;
		double num = brush.Opacity * (_useOpacitySaveLayer ? 1.0 : _currentOpacity);
		if (brush is ISolidColorBrush solidColorBrush)
		{
			paint.Color = new SKColor(solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B, (byte)((double)(int)solidColorBrush.Color.A * num));
			return paintWrapper;
		}
		paint.Color = new SKColor(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(255.0 * num));
		if (brush is IGradientBrush gradientBrush)
		{
			ConfigureGradientBrush(ref paintWrapper, targetSize, gradientBrush);
			return paintWrapper;
		}
		ITileBrush tileBrush = brush as ITileBrush;
		IDrawableBitmapImpl drawableBitmapImpl = null;
		if (brush is ISceneBrush sceneBrush)
		{
			using ISceneBrushContent sceneBrushContent = sceneBrush.CreateContent();
			if (sceneBrushContent != null)
			{
				ConfigureSceneBrushContent(ref paintWrapper, sceneBrushContent, targetSize);
				return paintWrapper;
			}
			paint.Color = default(SKColor);
		}
		else
		{
			if (brush is ISceneBrushContent content)
			{
				ConfigureSceneBrushContent(ref paintWrapper, content, targetSize);
				return paintWrapper;
			}
			drawableBitmapImpl = (tileBrush as IImageBrush)?.Source?.Bitmap?.Item as IDrawableBitmapImpl;
		}
		if (tileBrush != null && drawableBitmapImpl != null)
		{
			ConfigureTileBrush(ref paintWrapper, targetSize, tileBrush, drawableBitmapImpl);
		}
		else
		{
			paint.Color = new SKColor(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
		}
		return paintWrapper;
	}

	private PaintWrapper? TryCreatePaint(SKPaint paint, IPen pen, Size targetSize)
	{
		IBrush brush = pen.Brush;
		if (brush == null || pen.Thickness == 0.0)
		{
			return null;
		}
		PaintWrapper value = CreatePaint(paint, brush, targetSize);
		paint.IsStroke = true;
		paint.StrokeWidth = (float)pen.Thickness;
		switch (pen.LineCap)
		{
		case PenLineCap.Round:
			paint.StrokeCap = SKStrokeCap.Round;
			break;
		case PenLineCap.Square:
			paint.StrokeCap = SKStrokeCap.Square;
			break;
		default:
			paint.StrokeCap = SKStrokeCap.Butt;
			break;
		}
		switch (pen.LineJoin)
		{
		case PenLineJoin.Miter:
			paint.StrokeJoin = SKStrokeJoin.Miter;
			break;
		case PenLineJoin.Round:
			paint.StrokeJoin = SKStrokeJoin.Round;
			break;
		default:
			paint.StrokeJoin = SKStrokeJoin.Bevel;
			break;
		}
		paint.StrokeMiter = (float)pen.MiterLimit;
		if (pen.DashStyle?.Dashes != null && pen.DashStyle.Dashes.Count > 0)
		{
			IReadOnlyList<double> dashes = pen.DashStyle.Dashes;
			int num = ((dashes.Count % 2 == 0) ? dashes.Count : (dashes.Count * 2));
			float[] array = new float[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (float)dashes[i % dashes.Count] * paint.StrokeWidth;
			}
			float phase = (float)(pen.DashStyle.Offset * pen.Thickness);
			SKPathEffect disposable = (paint.PathEffect = SKPathEffect.CreateDash(array, phase));
			value.AddDisposable(disposable);
		}
		return value;
	}

	private SurfaceRenderTarget CreateRenderTarget(Size size, bool isLayer, PixelFormat? format = null)
	{
		PixelSize pixelSize = PixelSize.FromSizeWithDpi(size, _dpi);
		SurfaceRenderTarget.CreateInfo createInfo = default(SurfaceRenderTarget.CreateInfo);
		createInfo.Width = pixelSize.Width;
		createInfo.Height = pixelSize.Height;
		createInfo.Dpi = _dpi;
		createInfo.Format = format;
		createInfo.DisableTextLcdRendering = !isLayer || _disableSubpixelTextRendering;
		createInfo.GrContext = _grContext;
		createInfo.Gpu = _gpu;
		createInfo.Session = _session;
		createInfo.DisableManualFbo = !isLayer;
		return new SurfaceRenderTarget(createInfo);
	}

	public void PushEffect(IEffect effect)
	{
		CheckLease();
		using SKImageFilter imageFilter = CreateEffect(effect);
		SKPaint sKPaint = SKCacheBase<SKPaint, SKPaintCache>.Shared.Get();
		sKPaint.ImageFilter = imageFilter;
		Canvas.SaveLayer(sKPaint);
		SKCacheBase<SKPaint, SKPaintCache>.Shared.ReturnReset(sKPaint);
	}

	public void PopEffect()
	{
		CheckLease();
		Canvas.Restore();
	}

	private SKImageFilter? CreateEffect(IEffect effect)
	{
		if (effect is IBlurEffect blurEffect)
		{
			if (blurEffect.Radius <= 0.0)
			{
				return null;
			}
			float num = SkBlurRadiusToSigma(blurEffect.Radius);
			return SKImageFilter.CreateBlur(num, num);
		}
		if (effect is IDropShadowEffect dropShadowEffect)
		{
			float num2 = ((dropShadowEffect.BlurRadius > 0.0) ? SkBlurRadiusToSigma(dropShadowEffect.BlurRadius) : 0f);
			double num3 = (double)(int)dropShadowEffect.Color.A * dropShadowEffect.Opacity;
			if (!_useOpacitySaveLayer)
			{
				num3 *= _currentOpacity;
			}
			SKColor color = new SKColor(dropShadowEffect.Color.R, dropShadowEffect.Color.G, dropShadowEffect.Color.B, (byte)Math.Max(0.0, Math.Min(255.0, num3)));
			return SKImageFilter.CreateDropShadow((float)dropShadowEffect.OffsetX, (float)dropShadowEffect.OffsetY, num2, num2, color);
		}
		return null;
	}
}
