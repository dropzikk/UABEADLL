using System;
using Avalonia.Metadata;

namespace Avalonia.Media;

public class DrawingImage : AvaloniaObject, IImage, IAffectsRender
{
	public static readonly StyledProperty<Drawing?> DrawingProperty = AvaloniaProperty.Register<DrawingImage, Drawing>("Drawing");

	[Content]
	public Drawing? Drawing
	{
		get
		{
			return GetValue(DrawingProperty);
		}
		set
		{
			SetValue(DrawingProperty, value);
		}
	}

	public Size Size => Drawing?.GetBounds().Size ?? default(Size);

	public event EventHandler? Invalidated;

	public DrawingImage()
	{
	}

	public DrawingImage(Drawing drawing)
	{
		Drawing = drawing;
	}

	void IImage.Draw(DrawingContext context, Rect sourceRect, Rect destRect)
	{
		Drawing drawing = Drawing;
		if (drawing == null)
		{
			return;
		}
		Rect bounds = drawing.GetBounds();
		Matrix matrix = Matrix.CreateScale(destRect.Width / sourceRect.Width, destRect.Height / sourceRect.Height);
		Matrix matrix2 = Matrix.CreateTranslation(0.0 - sourceRect.X + destRect.X - bounds.X, 0.0 - sourceRect.Y + destRect.Y - bounds.Y);
		using (context.PushClip(destRect))
		{
			using (context.PushTransform(matrix2 * matrix))
			{
				Drawing?.Draw(context);
			}
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == DrawingProperty)
		{
			RaiseInvalidated(EventArgs.Empty);
		}
	}

	protected void RaiseInvalidated(EventArgs e)
	{
		this.Invalidated?.Invoke(this, e);
	}
}
