using System;
using System.Collections.Generic;
using System.Text;

namespace SixLabors.ImageSharp.Formats.Png;

internal static class PngConstants
{
	public static readonly Encoding Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");

	public static readonly Encoding LanguageEncoding = System.Text.Encoding.ASCII;

	public static readonly Encoding TranslatedEncoding = System.Text.Encoding.UTF8;

	public static readonly IEnumerable<string> MimeTypes = new string[2] { "image/png", "image/apng" };

	public static readonly IEnumerable<string> FileExtensions = new string[2] { "png", "apng" };

	public const ulong HeaderValue = 9894494448401390090uL;

	public static readonly Dictionary<PngColorType, byte[]> ColorTypes = new Dictionary<PngColorType, byte[]>
	{
		[PngColorType.Grayscale] = new byte[5] { 1, 2, 4, 8, 16 },
		[PngColorType.Rgb] = new byte[2] { 8, 16 },
		[PngColorType.Palette] = new byte[4] { 1, 2, 4, 8 },
		[PngColorType.GrayscaleWithAlpha] = new byte[2] { 8, 16 },
		[PngColorType.RgbWithAlpha] = new byte[2] { 8, 16 }
	};

	public const int MaxTextKeywordLength = 79;

	public const int MinTextKeywordLength = 1;

	public static ReadOnlySpan<byte> HeaderBytes => new byte[8] { 137, 80, 78, 71, 13, 10, 26, 10 };

	public static ReadOnlySpan<byte> XmpKeyword => "XML:com.adobe.xmp"u8;
}
