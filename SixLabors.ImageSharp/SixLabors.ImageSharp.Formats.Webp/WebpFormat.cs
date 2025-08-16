using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Webp;

public sealed class WebpFormat : IImageFormat<WebpMetadata, WebpFrameMetadata>, IImageFormat<WebpMetadata>, IImageFormat
{
	public static WebpFormat Instance { get; } = new WebpFormat();

	public string Name => "Webp";

	public string DefaultMimeType => "image/webp";

	public IEnumerable<string> MimeTypes => WebpConstants.MimeTypes;

	public IEnumerable<string> FileExtensions => WebpConstants.FileExtensions;

	private WebpFormat()
	{
	}

	public WebpMetadata CreateDefaultFormatMetadata()
	{
		return new WebpMetadata();
	}

	public WebpFrameMetadata CreateDefaultFormatFrameMetadata()
	{
		return new WebpFrameMetadata();
	}
}
