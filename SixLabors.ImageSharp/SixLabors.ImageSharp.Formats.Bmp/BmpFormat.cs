using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Bmp;

public sealed class BmpFormat : IImageFormat<BmpMetadata>, IImageFormat
{
	public static BmpFormat Instance { get; } = new BmpFormat();

	public string Name => "BMP";

	public string DefaultMimeType => "image/bmp";

	public IEnumerable<string> MimeTypes => BmpConstants.MimeTypes;

	public IEnumerable<string> FileExtensions => BmpConstants.FileExtensions;

	private BmpFormat()
	{
	}

	public BmpMetadata CreateDefaultFormatMetadata()
	{
		return new BmpMetadata();
	}
}
