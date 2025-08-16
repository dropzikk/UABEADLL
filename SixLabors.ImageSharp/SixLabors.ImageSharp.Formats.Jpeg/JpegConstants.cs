using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Jpeg;

internal static class JpegConstants
{
	internal static class Markers
	{
		public const byte XFF = byte.MaxValue;

		public const int XFFInt = 255;

		public const byte SOI = 216;

		public const byte EOI = 217;

		public const byte APP0 = 224;

		public const byte APP1 = 225;

		public const byte APP2 = 226;

		public const byte APP3 = 227;

		public const byte APP4 = 228;

		public const byte APP5 = 229;

		public const byte APP6 = 230;

		public const byte APP7 = 231;

		public const byte APP8 = 232;

		public const byte APP9 = 233;

		public const byte APP10 = 234;

		public const byte APP11 = 235;

		public const byte APP12 = 236;

		public const byte APP13 = 237;

		public const byte APP14 = 238;

		public const byte APP15 = 239;

		public const byte DAC = 204;

		public const byte COM = 254;

		public const byte DQT = 219;

		public const byte SOF0 = 192;

		public const byte SOF1 = 193;

		public const byte SOF2 = 194;

		public const byte SOF3 = 195;

		public const byte SOF5 = 197;

		public const byte SOF6 = 198;

		public const byte SOF7 = 199;

		public const byte SOF9 = 201;

		public const byte SOF10 = 202;

		public const byte SOF11 = 203;

		public const byte SOF13 = 205;

		public const byte SOF14 = 206;

		public const byte SOF15 = 207;

		public const byte DHT = 196;

		public const byte DRI = 221;

		public const byte SOS = 218;

		public const byte RST0 = 208;

		public const byte RST7 = 215;
	}

	internal static class Adobe
	{
		public const byte ColorTransformUnknown = 0;

		public const byte ColorTransformYCbCr = 1;

		public const byte ColorTransformYcck = 2;
	}

	internal static class Huffman
	{
		public const int RegisterSize = 64;

		public const int FetchBits = 48;

		public const int FetchLoop = 6;

		public const int MinBits = 16;

		public const int LookupBits = 8;

		public const int SlowBits = 9;

		public const int LookupSize = 256;
	}

	public const ushort MaxLength = ushort.MaxValue;

	public static readonly IEnumerable<string> MimeTypes = new string[2] { "image/jpeg", "image/pjpeg" };

	public static readonly IEnumerable<string> FileExtensions = new string[3] { "jpg", "jpeg", "jfif" };
}
