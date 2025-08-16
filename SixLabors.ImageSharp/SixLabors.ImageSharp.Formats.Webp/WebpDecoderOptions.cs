namespace SixLabors.ImageSharp.Formats.Webp;

public sealed class WebpDecoderOptions : ISpecializedDecoderOptions
{
	public DecoderOptions GeneralOptions { get; init; } = new DecoderOptions();

	public BackgroundColorHandling BackgroundColorHandling { get; init; }
}
