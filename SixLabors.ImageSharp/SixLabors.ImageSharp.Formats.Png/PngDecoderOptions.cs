namespace SixLabors.ImageSharp.Formats.Png;

public sealed class PngDecoderOptions : ISpecializedDecoderOptions
{
	public DecoderOptions GeneralOptions { get; init; } = new DecoderOptions();

	public PngCrcChunkHandling PngCrcChunkHandling { get; init; } = PngCrcChunkHandling.IgnoreNonCritical;

	public int MaxUncompressedAncillaryChunkSizeBytes { get; init; } = 8388608;
}
