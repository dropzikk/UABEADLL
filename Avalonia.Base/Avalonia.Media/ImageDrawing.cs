namespace Avalonia.Media;

public sealed class ImageDrawing : Drawing
{
	public static readonly StyledProperty<IImage?> ImageSourceProperty = AvaloniaProperty.Register<ImageDrawing, IImage>("ImageSource");

	public static readonly StyledProperty<Rect> RectProperty = AvaloniaProperty.Register<ImageDrawing, Rect>("Rect");

	public IImage? ImageSource
	{
		get
		{
			return GetValue(ImageSourceProperty);
		}
		set
		{
			SetValue(ImageSourceProperty, value);
		}
	}

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

	internal override void DrawCore(DrawingContext context)
	{
		IImage imageSource = ImageSource;
		Rect rect = Rect;
		if (imageSource != null && (rect.Width != 0.0 || rect.Height != 0.0))
		{
			context.DrawImage(imageSource, rect);
		}
	}

	public override Rect GetBounds()
	{
		return Rect;
	}
}
