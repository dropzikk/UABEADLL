namespace SixLabors.ImageSharp.Processing;

public static class RotateFlipExtensions
{
	public static IImageProcessingContext RotateFlip(this IImageProcessingContext source, RotateMode rotateMode, FlipMode flipMode)
	{
		return source.Rotate(rotateMode).Flip(flipMode);
	}
}
