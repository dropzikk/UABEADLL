using System;
using Avalonia.Collections;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Reactive;

namespace Avalonia.Controls.Shapes;

public abstract class Shape : Control
{
	public static readonly StyledProperty<IBrush?> FillProperty;

	public static readonly StyledProperty<Stretch> StretchProperty;

	public static readonly StyledProperty<IBrush?> StrokeProperty;

	public static readonly StyledProperty<AvaloniaList<double>?> StrokeDashArrayProperty;

	public static readonly StyledProperty<double> StrokeDashOffsetProperty;

	public static readonly StyledProperty<double> StrokeThicknessProperty;

	public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty;

	public static readonly StyledProperty<PenLineJoin> StrokeJoinProperty;

	private Matrix _transform = Matrix.Identity;

	private Geometry? _definingGeometry;

	private Geometry? _renderedGeometry;

	public Geometry? DefiningGeometry
	{
		get
		{
			if (_definingGeometry == null)
			{
				_definingGeometry = CreateDefiningGeometry();
			}
			return _definingGeometry;
		}
	}

	public Geometry? RenderedGeometry
	{
		get
		{
			if (_renderedGeometry == null && DefiningGeometry != null)
			{
				if (_transform == Matrix.Identity)
				{
					_renderedGeometry = DefiningGeometry;
				}
				else
				{
					_renderedGeometry = DefiningGeometry.Clone();
					if (_renderedGeometry.Transform == null || _renderedGeometry.Transform.Value == Matrix.Identity)
					{
						_renderedGeometry.Transform = new MatrixTransform(_transform);
					}
					else
					{
						_renderedGeometry.Transform = new MatrixTransform(_renderedGeometry.Transform.Value * _transform);
					}
				}
			}
			return _renderedGeometry;
		}
	}

	public IBrush? Fill
	{
		get
		{
			return GetValue(FillProperty);
		}
		set
		{
			SetValue(FillProperty, value);
		}
	}

	public Stretch Stretch
	{
		get
		{
			return GetValue(StretchProperty);
		}
		set
		{
			SetValue(StretchProperty, value);
		}
	}

	public IBrush? Stroke
	{
		get
		{
			return GetValue(StrokeProperty);
		}
		set
		{
			SetValue(StrokeProperty, value);
		}
	}

	public AvaloniaList<double>? StrokeDashArray
	{
		get
		{
			return GetValue(StrokeDashArrayProperty);
		}
		set
		{
			SetValue(StrokeDashArrayProperty, value);
		}
	}

	public double StrokeDashOffset
	{
		get
		{
			return GetValue(StrokeDashOffsetProperty);
		}
		set
		{
			SetValue(StrokeDashOffsetProperty, value);
		}
	}

	public double StrokeThickness
	{
		get
		{
			return GetValue(StrokeThicknessProperty);
		}
		set
		{
			SetValue(StrokeThicknessProperty, value);
		}
	}

	public PenLineCap StrokeLineCap
	{
		get
		{
			return GetValue(StrokeLineCapProperty);
		}
		set
		{
			SetValue(StrokeLineCapProperty, value);
		}
	}

	public PenLineJoin StrokeJoin
	{
		get
		{
			return GetValue(StrokeJoinProperty);
		}
		set
		{
			SetValue(StrokeJoinProperty, value);
		}
	}

	static Shape()
	{
		FillProperty = AvaloniaProperty.Register<Shape, IBrush>("Fill");
		StretchProperty = AvaloniaProperty.Register<Shape, Stretch>("Stretch", Stretch.None);
		StrokeProperty = AvaloniaProperty.Register<Shape, IBrush>("Stroke");
		StrokeDashArrayProperty = AvaloniaProperty.Register<Shape, AvaloniaList<double>>("StrokeDashArray");
		StrokeDashOffsetProperty = AvaloniaProperty.Register<Shape, double>("StrokeDashOffset", 0.0);
		StrokeThicknessProperty = AvaloniaProperty.Register<Shape, double>("StrokeThickness", 0.0);
		StrokeLineCapProperty = AvaloniaProperty.Register<Shape, PenLineCap>("StrokeLineCap", PenLineCap.Flat);
		StrokeJoinProperty = AvaloniaProperty.Register<Shape, PenLineJoin>("StrokeJoin", PenLineJoin.Miter);
		Layoutable.AffectsMeasure<Shape>(new AvaloniaProperty[2] { StretchProperty, StrokeThicknessProperty });
		Visual.AffectsRender<Shape>(new AvaloniaProperty[7] { FillProperty, StrokeProperty, StrokeDashArrayProperty, StrokeDashOffsetProperty, StrokeThicknessProperty, StrokeLineCapProperty, StrokeJoinProperty });
	}

	public sealed override void Render(DrawingContext context)
	{
		Geometry renderedGeometry = RenderedGeometry;
		if (renderedGeometry == null)
		{
			return;
		}
		IBrush stroke = Stroke;
		ImmutablePen pen = null;
		if (stroke != null)
		{
			AvaloniaList<double> strokeDashArray = StrokeDashArray;
			ImmutableDashStyle dashStyle = null;
			if (strokeDashArray != null && strokeDashArray.Count > 0)
			{
				dashStyle = new ImmutableDashStyle(strokeDashArray, StrokeDashOffset);
			}
			pen = new ImmutablePen(stroke.ToImmutable(), StrokeThickness, dashStyle, StrokeLineCap, StrokeJoin);
		}
		context.DrawGeometry(Fill, pen, renderedGeometry);
	}

	protected static void AffectsGeometry<TShape>(params AvaloniaProperty[] properties) where TShape : Shape
	{
		for (int i = 0; i < properties.Length; i++)
		{
			properties[i].Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs e)
			{
				if (e.Sender is TShape control)
				{
					AffectsGeometryInvalidate(control, e);
				}
			});
		}
	}

	protected abstract Geometry? CreateDefiningGeometry();

	protected void InvalidateGeometry()
	{
		_renderedGeometry = null;
		_definingGeometry = null;
		InvalidateMeasure();
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (DefiningGeometry == null)
		{
			return default(Size);
		}
		return CalculateSizeAndTransform(availableSize, DefiningGeometry.Bounds, Stretch).size;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (DefiningGeometry != null)
		{
			Matrix item = CalculateSizeAndTransform(finalSize, DefiningGeometry.Bounds, Stretch).transform;
			if (_transform != item)
			{
				_transform = item;
				_renderedGeometry = null;
			}
			return finalSize;
		}
		return default(Size);
	}

	internal static (Size size, Matrix transform) CalculateSizeAndTransform(Size availableSize, Rect shapeBounds, Stretch Stretch)
	{
		Size size = new Size(shapeBounds.Right, shapeBounds.Bottom);
		Matrix matrix = Matrix.Identity;
		double width = availableSize.Width;
		double height = availableSize.Height;
		double num = 0.0;
		double num2 = 0.0;
		if (Stretch != 0)
		{
			size = shapeBounds.Size;
			matrix = Matrix.CreateTranslation(-(Vector)shapeBounds.Position);
		}
		if (double.IsInfinity(availableSize.Width))
		{
			width = size.Width;
		}
		if (double.IsInfinity(availableSize.Height))
		{
			height = size.Height;
		}
		if (shapeBounds.Width > 0.0)
		{
			num = width / size.Width;
		}
		if (shapeBounds.Height > 0.0)
		{
			num2 = height / size.Height;
		}
		if (double.IsInfinity(availableSize.Width))
		{
			num = num2;
		}
		if (double.IsInfinity(availableSize.Height))
		{
			num2 = num;
		}
		switch (Stretch)
		{
		case Stretch.Uniform:
			num = (num2 = Math.Min(num, num2));
			break;
		case Stretch.UniformToFill:
			num = (num2 = Math.Max(num, num2));
			break;
		case Stretch.Fill:
			if (double.IsInfinity(availableSize.Width))
			{
				num = 1.0;
			}
			if (double.IsInfinity(availableSize.Height))
			{
				num2 = 1.0;
			}
			break;
		default:
			num = (num2 = 1.0);
			break;
		}
		Matrix item = matrix * Matrix.CreateScale(num, num2);
		return (size: new Size(size.Width * num, size.Height * num2), transform: item);
	}

	private static void AffectsGeometryInvalidate(Shape control, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == Visual.BoundsProperty)
		{
			Rect rect = (Rect)e.OldValue;
			Rect rect2 = (Rect)e.NewValue;
			if (rect.Size == rect2.Size)
			{
				return;
			}
		}
		control.InvalidateGeometry();
	}
}
