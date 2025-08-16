using Avalonia.Platform;

namespace Avalonia.Media;

public class CombinedGeometry : Geometry
{
	public static readonly StyledProperty<Geometry?> Geometry1Property = AvaloniaProperty.Register<CombinedGeometry, Geometry>("Geometry1");

	public static readonly StyledProperty<Geometry?> Geometry2Property = AvaloniaProperty.Register<CombinedGeometry, Geometry>("Geometry2");

	public static readonly StyledProperty<GeometryCombineMode> GeometryCombineModeProperty = AvaloniaProperty.Register<CombinedGeometry, GeometryCombineMode>("GeometryCombineMode", GeometryCombineMode.Union);

	public Geometry? Geometry1
	{
		get
		{
			return GetValue(Geometry1Property);
		}
		set
		{
			SetValue(Geometry1Property, value);
		}
	}

	public Geometry? Geometry2
	{
		get
		{
			return GetValue(Geometry2Property);
		}
		set
		{
			SetValue(Geometry2Property, value);
		}
	}

	public GeometryCombineMode GeometryCombineMode
	{
		get
		{
			return GetValue(GeometryCombineModeProperty);
		}
		set
		{
			SetValue(GeometryCombineModeProperty, value);
		}
	}

	public CombinedGeometry()
	{
	}

	public CombinedGeometry(Geometry geometry1, Geometry geometry2)
	{
		Geometry1 = geometry1;
		Geometry2 = geometry2;
	}

	public CombinedGeometry(GeometryCombineMode combineMode, Geometry? geometry1, Geometry? geometry2)
	{
		Geometry1 = geometry1;
		Geometry2 = geometry2;
		GeometryCombineMode = combineMode;
	}

	public CombinedGeometry(GeometryCombineMode combineMode, Geometry? geometry1, Geometry? geometry2, Transform? transform)
	{
		Geometry1 = geometry1;
		Geometry2 = geometry2;
		GeometryCombineMode = combineMode;
		base.Transform = transform;
	}

	public override Geometry Clone()
	{
		return new CombinedGeometry(GeometryCombineMode, Geometry1, Geometry2, base.Transform);
	}

	private protected sealed override IGeometryImpl? CreateDefiningGeometry()
	{
		Geometry geometry = Geometry1;
		Geometry geometry2 = Geometry2;
		if (geometry?.PlatformImpl != null && geometry2?.PlatformImpl != null)
		{
			return AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().CreateCombinedGeometry(GeometryCombineMode, geometry.PlatformImpl, geometry2.PlatformImpl);
		}
		if (GeometryCombineMode == GeometryCombineMode.Intersect)
		{
			return null;
		}
		object obj = geometry?.PlatformImpl;
		if (obj == null)
		{
			if (geometry2 == null)
			{
				return null;
			}
			obj = geometry2.PlatformImpl;
		}
		return (IGeometryImpl?)obj;
	}
}
