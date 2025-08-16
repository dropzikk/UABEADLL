using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Tiff.Constants;

internal static class TiffConstants
{
	public const byte ByteOrderLittleEndian = 73;

	public const byte ByteOrderBigEndian = 77;

	public const ushort ByteOrderLittleEndianShort = 18761;

	public const ushort ByteOrderBigEndianShort = 19789;

	public const ushort HeaderMagicNumber = 42;

	public const ushort BigTiffHeaderMagicNumber = 43;

	public const ushort BigTiffBytesize = 8;

	public const int RowsPerStripInfinity = int.MaxValue;

	public const int SizeOfRational = 8;

	public const int DefaultStripSize = 8192;

	public static readonly TiffBitsPerSample BitsPerSample1Bit = new TiffBitsPerSample(1, 0, 0, 0);

	public static readonly TiffBitsPerSample BitsPerSample4Bit = new TiffBitsPerSample(4, 0, 0, 0);

	public static readonly TiffBitsPerSample BitsPerSample8Bit = new TiffBitsPerSample(8, 0, 0, 0);

	public static readonly TiffBitsPerSample BitsPerSample16Bit = new TiffBitsPerSample(16, 0, 0, 0);

	public static readonly TiffBitsPerSample BitsPerSampleRgb8Bit = new TiffBitsPerSample(8, 8, 8, 0);

	public static readonly IEnumerable<string> MimeTypes = new string[2] { "image/tiff", "image/tiff-fx" };

	public static readonly IEnumerable<string> FileExtensions = new string[2] { "tiff", "tif" };
}
