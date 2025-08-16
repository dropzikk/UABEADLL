namespace SixLabors.ImageSharp.Formats.Bmp;

public sealed class BmpDecoderOptions : ISpecializedDecoderOptions
{
	public DecoderOptions GeneralOptions { get; init; } = new DecoderOptions();

	public RleSkippedPixelHandling RleSkippedPixelHandling { get; init; }
}
