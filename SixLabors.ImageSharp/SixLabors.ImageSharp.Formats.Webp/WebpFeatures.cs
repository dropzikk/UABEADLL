namespace SixLabors.ImageSharp.Formats.Webp;

internal class WebpFeatures
{
	public bool IccProfile { get; set; }

	public bool Alpha { get; set; }

	public byte AlphaChunkHeader { get; set; }

	public bool ExifProfile { get; set; }

	public bool XmpMetaData { get; set; }

	public bool Animation { get; set; }

	public ushort AnimationLoopCount { get; set; }

	public Color? AnimationBackgroundColor { get; set; }
}
