using System.Collections.Generic;
using SixLabors.ImageSharp.Formats.Tiff.Constants;

namespace SixLabors.ImageSharp.Formats.Tiff;

public sealed class TiffFormat : IImageFormat<TiffMetadata, TiffFrameMetadata>, IImageFormat<TiffMetadata>, IImageFormat
{
	public static TiffFormat Instance { get; } = new TiffFormat();

	public string Name => "TIFF";

	public string DefaultMimeType => "image/tiff";

	public IEnumerable<string> MimeTypes => TiffConstants.MimeTypes;

	public IEnumerable<string> FileExtensions => TiffConstants.FileExtensions;

	private TiffFormat()
	{
	}

	public TiffMetadata CreateDefaultFormatMetadata()
	{
		return new TiffMetadata();
	}

	public TiffFrameMetadata CreateDefaultFormatFrameMetadata()
	{
		return new TiffFrameMetadata();
	}
}
