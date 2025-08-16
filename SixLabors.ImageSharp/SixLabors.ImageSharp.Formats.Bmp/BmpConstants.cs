using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Bmp;

internal static class BmpConstants
{
	internal static class TypeMarkers
	{
		public const int Bitmap = 19778;

		public const int BitmapArray = 16706;

		public const int ColorIcon = 18755;

		public const int ColorPointer = 20547;

		public const int Icon = 17225;

		public const int Pointer = 21584;
	}

	public static readonly IEnumerable<string> MimeTypes = new string[2] { "image/bmp", "image/x-windows-bmp" };

	public static readonly IEnumerable<string> FileExtensions = new string[3] { "bm", "bmp", "dip" };
}
