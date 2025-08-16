using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Pbm;

public sealed class PbmFormat : IImageFormat<PbmMetadata>, IImageFormat
{
	public static PbmFormat Instance { get; } = new PbmFormat();

	public string Name => "PBM";

	public string DefaultMimeType => "image/x-portable-pixmap";

	public IEnumerable<string> MimeTypes => PbmConstants.MimeTypes;

	public IEnumerable<string> FileExtensions => PbmConstants.FileExtensions;

	private PbmFormat()
	{
	}

	public PbmMetadata CreateDefaultFormatMetadata()
	{
		return new PbmMetadata();
	}
}
