using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Platform;
using Avalonia.Reactive;

namespace Avalonia.Media;

[TypeConverter(typeof(GeometryTypeConverter))]
public abstract class Geometry : AvaloniaObject
{
	public static readonly StyledProperty<Transform?> TransformProperty;

	private bool _isDirty = true;

	private IGeometryImpl? _platformImpl;

	public Rect Bounds => PlatformImpl?.Bounds ?? default(Rect);

	internal IGeometryImpl? PlatformImpl
	{
		get
		{
			if (_isDirty)
			{
				IGeometryImpl geometryImpl = CreateDefiningGeometry();
				Transform transform = Transform;
				if (geometryImpl != null && transform != null && transform.Value != Matrix.Identity)
				{
					geometryImpl = geometryImpl.WithTransform(transform.Value);
				}
				_platformImpl = geometryImpl;
				_isDirty = false;
			}
			return _platformImpl;
		}
	}

	public Transform? Transform
	{
		get
		{
			return GetValue(TransformProperty);
		}
		set
		{
			SetValue(TransformProperty, value);
		}
	}

	public double ContourLength => PlatformImpl?.ContourLength ?? 0.0;

	public event EventHandler? Changed;

	static Geometry()
	{
		TransformProperty = AvaloniaProperty.Register<Geometry, Transform>("Transform");
		TransformProperty.Changed.AddClassHandler(delegate(Geometry x, AvaloniaPropertyChangedEventArgs e)
		{
			x.TransformChanged(e);
		});
	}

	internal Geometry()
	{
	}

	public static Geometry Parse(string s)
	{
		return StreamGeometry.Parse(s);
	}

	public abstract Geometry Clone();

	public Rect GetRenderBounds(IPen pen)
	{
		return PlatformImpl?.GetRenderBounds(pen) ?? default(Rect);
	}

	public bool FillContains(Point point)
	{
		return PlatformImpl?.FillContains(point) ?? false;
	}

	public bool StrokeContains(IPen pen, Point point)
	{
		return PlatformImpl?.StrokeContains(pen, point) ?? false;
	}

	protected static void AffectsGeometry(params AvaloniaProperty[] properties)
	{
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(AffectsGeometryInvalidate);
		for (int i = 0; i < properties.Length; i++)
		{
			properties[i].Changed.Subscribe(observer);
		}
	}

	private protected abstract IGeometryImpl? CreateDefiningGeometry();

	protected void InvalidateGeometry()
	{
		_isDirty = true;
		_platformImpl = null;
		this.Changed?.Invoke(this, EventArgs.Empty);
	}

	private void TransformChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Transform transform = (Transform)e.OldValue;
		Transform transform2 = (Transform)e.NewValue;
		if (transform != null)
		{
			transform.Changed -= TransformChanged;
		}
		if (transform2 != null)
		{
			transform2.Changed += TransformChanged;
		}
		TransformChanged(transform2, EventArgs.Empty);
	}

	private void TransformChanged(object? sender, EventArgs e)
	{
		Matrix? matrix = ((Transform)sender)?.Value;
		if (_platformImpl is ITransformedGeometryImpl transformedGeometryImpl)
		{
			if (!matrix.HasValue || matrix == Matrix.Identity)
			{
				_platformImpl = transformedGeometryImpl.SourceGeometry;
			}
			else if (matrix != transformedGeometryImpl.Transform)
			{
				_platformImpl = transformedGeometryImpl.SourceGeometry.WithTransform(matrix.Value);
			}
		}
		else if (_platformImpl != null && matrix.HasValue && matrix != Matrix.Identity)
		{
			_platformImpl = _platformImpl.WithTransform(matrix.Value);
		}
		this.Changed?.Invoke(this, EventArgs.Empty);
	}

	private static void AffectsGeometryInvalidate(AvaloniaPropertyChangedEventArgs e)
	{
		(e.Sender as Geometry)?.InvalidateGeometry();
	}

	public static Geometry Combine(Geometry geometry1, RectangleGeometry geometry2, GeometryCombineMode combineMode, Transform? transform = null)
	{
		return new CombinedGeometry(combineMode, geometry1, geometry2, transform);
	}

	public bool TryGetPointAtDistance(double distance, out Point point)
	{
		if (PlatformImpl == null)
		{
			point = default(Point);
			return false;
		}
		return PlatformImpl.TryGetPointAtDistance(distance, out point);
	}

	public bool TryGetPointAndTangentAtDistance(double distance, out Point point, out Point tangent)
	{
		if (PlatformImpl == null)
		{
			point = (tangent = default(Point));
			return false;
		}
		return PlatformImpl.TryGetPointAndTangentAtDistance(distance, out point, out tangent);
	}

	public bool TryGetSegment(double startDistance, double stopDistance, bool startOnBeginFigure, [NotNullWhen(true)] out Geometry? segmentGeometry)
	{
		segmentGeometry = null;
		if (PlatformImpl == null)
		{
			return false;
		}
		if (!PlatformImpl.TryGetSegment(startDistance, stopDistance, startOnBeginFigure, out IGeometryImpl segmentGeometry2))
		{
			return false;
		}
		segmentGeometry = new PlatformGeometry(segmentGeometry2);
		return true;
	}
}
