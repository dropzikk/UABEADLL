using Avalonia.Platform;

namespace Avalonia.Media;

public class RectangleGeometry : Geometry
{
	public static readonly StyledProperty<Rect> RectProperty;

	public Rect Rect
	{
		get
		{
			return GetValue(RectProperty);
		}
		set
		{
			SetValue(RectProperty, value);
		}
	}

	static RectangleGeometry()
	{
		RectProperty = AvaloniaProperty.Register<RectangleGeometry, Rect>("Rect");
		Geometry.AffectsGeometry(RectProperty);
	}

	public RectangleGeometry()
	{
	}

	public RectangleGeometry(Rect rect)
	{
		Rect = rect;
	}

	public override Geometry Clone()
	{
		return new RectangleGeometry(Rect);
	}

	private protected sealed override IGeometryImpl? CreateDefiningGeometry()
	{
		return AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().CreateRectangleGeometry(Rect);
	}
}
