namespace SixLabors.ImageSharp.Formats.Webp;

public class WebpMetadata : IDeepCloneable
{
	public WebpFileFormatType? FileFormat { get; set; }

	public ushort RepeatCount { get; set; } = 1;

	public Color BackgroundColor { get; set; }

	public WebpMetadata()
	{
	}

	private WebpMetadata(WebpMetadata other)
	{
		FileFormat = other.FileFormat;
		RepeatCount = other.RepeatCount;
		BackgroundColor = other.BackgroundColor;
	}

	public IDeepCloneable DeepClone()
	{
		return new WebpMetadata(this);
	}

	internal static WebpMetadata FromAnimatedMetadata(AnimatedImageMetadata metadata)
	{
		return new WebpMetadata
		{
			FileFormat = WebpFileFormatType.Lossless,
			BackgroundColor = metadata.BackgroundColor,
			RepeatCount = metadata.RepeatCount
		};
	}
}
