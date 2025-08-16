using System;
using System.Text;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal static class ExifUcs2StringHelpers
{
	public static Encoding Ucs2Encoding => Encoding.GetEncoding("UCS-2");

	public static bool IsUcs2Tag(ExifTagValue tag)
	{
		if ((uint)(tag - 40091) <= 4u)
		{
			return true;
		}
		return false;
	}

	public static int Write(string value, Span<byte> destination)
	{
		return ExifEncodedStringHelpers.Write(Ucs2Encoding, value, destination);
	}
}
