using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal abstract class GeometryImpl : IGeometryImpl
{
	private struct PathCache
	{
		private float _cachedStrokeWidth;

		public const float Tolerance = float.Epsilon;

		public SKPath? CachedStrokePath { get; private set; }

		public Rect CachedGeometryRenderBounds { get; private set; }

		public bool HasCacheFor(float strokeWidth)
		{
			if (CachedStrokePath != null)
			{
				return Math.Abs(_cachedStrokeWidth - strokeWidth) < float.Epsilon;
			}
			return false;
		}

		public void Cache(SKPath path, float strokeWidth, Rect geometryRenderBounds)
		{
			if (CachedStrokePath != path)
			{
				CachedStrokePath?.Dispose();
			}
			CachedStrokePath = path;
			CachedGeometryRenderBounds = geometryRenderBounds;
			_cachedStrokeWidth = strokeWidth;
		}

		public void Invalidate()
		{
			CachedStrokePath?.Dispose();
			CachedStrokePath = null;
			CachedGeometryRenderBounds = default(Rect);
			_cachedStrokeWidth = 0f;
		}
	}

	private PathCache _pathCache;

	private SKPathMeasure? _cachedPathMeasure;

	private SKPathMeasure CachedPathMeasure => _cachedPathMeasure ?? (_cachedPathMeasure = new SKPathMeasure(StrokePath));

	public abstract Rect Bounds { get; }

	public double ContourLength
	{
		get
		{
			if (StrokePath == null)
			{
				return 0.0;
			}
			return CachedPathMeasure.Length;
		}
	}

	public abstract SKPath? StrokePath { get; }

	public abstract SKPath? FillPath { get; }

	public bool FillContains(Point point)
	{
		return PathContainsCore(FillPath, point);
	}

	public bool StrokeContains(IPen? pen, Point point)
	{
		float strokeWidth = (float)(pen?.Thickness ?? 0.0);
		if (!_pathCache.HasCacheFor(strokeWidth))
		{
			UpdatePathCache(strokeWidth);
		}
		return PathContainsCore(_pathCache.CachedStrokePath, point);
	}

	private void UpdatePathCache(float strokeWidth)
	{
		SKPath sKPath = new SKPath();
		if (Math.Abs(strokeWidth) < float.Epsilon)
		{
			_pathCache.Cache(sKPath, strokeWidth, Bounds);
			return;
		}
		SKPaint sKPaint = SKCacheBase<SKPaint, SKPaintCache>.Shared.Get();
		sKPaint.IsStroke = true;
		sKPaint.StrokeWidth = strokeWidth;
		sKPaint.GetFillPath(StrokePath, sKPath);
		SKCacheBase<SKPaint, SKPaintCache>.Shared.ReturnReset(sKPaint);
		_pathCache.Cache(sKPath, strokeWidth, sKPath.TightBounds.ToAvaloniaRect());
	}

	private static bool PathContainsCore(SKPath? path, Point point)
	{
		return path?.Contains((float)point.X, (float)point.Y) ?? false;
	}

	public IGeometryImpl? Intersect(IGeometryImpl geometry)
	{
		if (!(geometry is GeometryImpl g))
		{
			return null;
		}
		return CombinedGeometryImpl.TryCreate(GeometryCombineMode.Intersect, this, g);
	}

	public Rect GetRenderBounds(IPen? pen)
	{
		float strokeWidth = (float)(pen?.Thickness ?? 0.0);
		if (!_pathCache.HasCacheFor(strokeWidth))
		{
			UpdatePathCache(strokeWidth);
		}
		return _pathCache.CachedGeometryRenderBounds;
	}

	public ITransformedGeometryImpl WithTransform(Matrix transform)
	{
		return new TransformedGeometryImpl(this, transform);
	}

	public bool TryGetPointAtDistance(double distance, out Point point)
	{
		if (StrokePath == null)
		{
			point = default(Point);
			return false;
		}
		SKPoint position;
		bool position2 = CachedPathMeasure.GetPosition((float)distance, out position);
		point = new Point(position.X, position.Y);
		return position2;
	}

	public bool TryGetPointAndTangentAtDistance(double distance, out Point point, out Point tangent)
	{
		if (StrokePath == null)
		{
			point = default(Point);
			tangent = default(Point);
			return false;
		}
		SKPoint position;
		SKPoint tangent2;
		bool positionAndTangent = CachedPathMeasure.GetPositionAndTangent((float)distance, out position, out tangent2);
		point = new Point(position.X, position.Y);
		tangent = new Point(tangent2.X, tangent2.Y);
		return positionAndTangent;
	}

	public bool TryGetSegment(double startDistance, double stopDistance, bool startOnBeginFigure, [NotNullWhen(true)] out IGeometryImpl? segmentGeometry)
	{
		if (StrokePath == null)
		{
			segmentGeometry = null;
			return false;
		}
		segmentGeometry = null;
		SKPath sKPath = new SKPath();
		bool segment = CachedPathMeasure.GetSegment((float)startDistance, (float)stopDistance, sKPath, startOnBeginFigure);
		if (segment)
		{
			segmentGeometry = new StreamGeometryImpl(sKPath, null, null);
		}
		return segment;
	}

	protected void InvalidateCaches()
	{
		_pathCache.Invalidate();
	}
}
