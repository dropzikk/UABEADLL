using Avalonia.Media;

namespace Avalonia.Rendering.Utilities;

internal class TileBrushCalculator
{
	private readonly Size _imageSize;

	private readonly Rect _drawRect;

	public bool IsValid { get; }

	public Rect DestinationRect { get; }

	public Rect IntermediateClip => _drawRect;

	public Size IntermediateSize { get; }

	public Matrix IntermediateTransform { get; }

	public bool NeedsIntermediate
	{
		get
		{
			if (IntermediateTransform != Matrix.Identity)
			{
				return true;
			}
			if (SourceRect.Position != default(Point))
			{
				return true;
			}
			if (SourceRect.Size.AspectRatio == _imageSize.AspectRatio)
			{
				return false;
			}
			if (SourceRect.Width != _imageSize.Width || SourceRect.Height != _imageSize.Height)
			{
				return true;
			}
			return false;
		}
	}

	public Rect SourceRect { get; }

	public TileBrushCalculator(ITileBrush brush, Size contentSize, Size targetSize)
		: this(brush.TileMode, brush.Stretch, brush.AlignmentX, brush.AlignmentY, brush.SourceRect, brush.DestinationRect, contentSize, targetSize)
	{
	}

	public TileBrushCalculator(TileMode tileMode, Stretch stretch, AlignmentX alignmentX, AlignmentY alignmentY, RelativeRect sourceRect, RelativeRect destinationRect, Size contentSize, Size targetSize)
	{
		_imageSize = contentSize;
		SourceRect = sourceRect.ToPixels(_imageSize);
		DestinationRect = destinationRect.ToPixels(targetSize);
		Vector scale = stretch.CalculateScaling(DestinationRect.Size, SourceRect.Size);
		Vector translate = CalculateTranslate(alignmentX, alignmentY, SourceRect, DestinationRect, scale);
		IntermediateSize = ((tileMode == TileMode.None) ? targetSize : DestinationRect.Size);
		IntermediateTransform = CalculateIntermediateTransform(tileMode, SourceRect, DestinationRect, scale, translate, out _drawRect);
	}

	public static Vector CalculateTranslate(AlignmentX alignmentX, AlignmentY alignmentY, Rect sourceRect, Rect destinationRect, Vector scale)
	{
		double num = 0.0;
		double num2 = 0.0;
		Size size = sourceRect.Size * scale;
		switch (alignmentX)
		{
		case AlignmentX.Center:
			num += (destinationRect.Width - size.Width) / 2.0;
			break;
		case AlignmentX.Right:
			num += destinationRect.Width - size.Width;
			break;
		}
		switch (alignmentY)
		{
		case AlignmentY.Center:
			num2 += (destinationRect.Height - size.Height) / 2.0;
			break;
		case AlignmentY.Bottom:
			num2 += destinationRect.Height - size.Height;
			break;
		}
		return new Vector(num, num2);
	}

	public static Matrix CalculateIntermediateTransform(TileMode tileMode, Rect sourceRect, Rect destinationRect, Vector scale, Vector translate, out Rect drawRect)
	{
		Matrix result = Matrix.CreateTranslation(-sourceRect.Position) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(translate);
		Rect rect;
		if (tileMode == TileMode.None)
		{
			rect = destinationRect;
			result *= Matrix.CreateTranslation(destinationRect.Position);
		}
		else
		{
			rect = new Rect(destinationRect.Size);
		}
		drawRect = rect;
		return result;
	}
}
