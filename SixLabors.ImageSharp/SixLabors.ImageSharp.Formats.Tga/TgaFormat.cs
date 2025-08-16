using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Tga;

public sealed class TgaFormat : IImageFormat<TgaMetadata>, IImageFormat
{
	public static TgaFormat Instance { get; } = new TgaFormat();

	public string Name => "TGA";

	public string DefaultMimeType => "image/tga";

	public IEnumerable<string> MimeTypes => TgaConstants.MimeTypes;

	public IEnumerable<string> FileExtensions => TgaConstants.FileExtensions;

	public TgaMetadata CreateDefaultFormatMetadata()
	{
		return new TgaMetadata();
	}
}
