using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Pbm;

internal static class PbmConstants
{
	public const ushort MaxLength = ushort.MaxValue;

	public static readonly IEnumerable<string> MimeTypes = new string[2] { "image/x-portable-pixmap", "image/x-portable-anymap" };

	public static readonly IEnumerable<string> FileExtensions = new string[3] { "ppm", "pbm", "pgm" };
}
