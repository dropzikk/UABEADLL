using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Tga;

internal static class TgaConstants
{
	public static readonly IEnumerable<string> MimeTypes = new string[2] { "image/x-tga", "image/x-targa" };

	public static readonly IEnumerable<string> FileExtensions = new string[4] { "tga", "vda", "icb", "vst" };

	public const int FileHeaderLength = 18;
}
