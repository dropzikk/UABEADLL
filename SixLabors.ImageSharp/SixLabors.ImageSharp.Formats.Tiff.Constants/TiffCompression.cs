namespace SixLabors.ImageSharp.Formats.Tiff.Constants;

public enum TiffCompression : ushort
{
	Invalid = 0,
	None = 1,
	Ccitt1D = 2,
	CcittGroup3Fax = 3,
	CcittGroup4Fax = 4,
	Lzw = 5,
	OldJpeg = 6,
	Jpeg = 7,
	Deflate = 8,
	ItuTRecT82 = 9,
	ItuTRecT43 = 10,
	NeXT = 32766,
	PackBits = 32773,
	ThunderScan = 32809,
	OldDeflate = 32946,
	Webp = 50001
}
