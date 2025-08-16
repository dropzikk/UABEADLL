using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Png;

public sealed class PngFormat : IImageFormat<PngMetadata, PngFrameMetadata>, IImageFormat<PngMetadata>, IImageFormat
{
	public static PngFormat Instance { get; } = new PngFormat();

	public string Name => "PNG";

	public string DefaultMimeType => "image/png";

	public IEnumerable<string> MimeTypes => PngConstants.MimeTypes;

	public IEnumerable<string> FileExtensions => PngConstants.FileExtensions;

	private PngFormat()
	{
	}

	public PngMetadata CreateDefaultFormatMetadata()
	{
		return new PngMetadata();
	}

	public PngFrameMetadata CreateDefaultFormatFrameMetadata()
	{
		return new PngFrameMetadata();
	}
}
