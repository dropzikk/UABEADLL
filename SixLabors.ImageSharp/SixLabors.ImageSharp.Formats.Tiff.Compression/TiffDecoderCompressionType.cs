namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

internal enum TiffDecoderCompressionType
{
	None,
	PackBits,
	Deflate,
	Lzw,
	T4,
	T6,
	HuffmanRle,
	Jpeg,
	Webp,
	OldJpeg
}
