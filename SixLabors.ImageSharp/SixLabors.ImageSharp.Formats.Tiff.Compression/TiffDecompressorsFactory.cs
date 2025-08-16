using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

internal static class TiffDecompressorsFactory
{
	public static TiffBaseDecompressor Create(DecoderOptions options, TiffDecoderCompressionType method, MemoryAllocator allocator, TiffPhotometricInterpretation photometricInterpretation, int width, int bitsPerPixel, TiffColorType colorType, TiffPredictor predictor, FaxCompressionOptions faxOptions, byte[] jpegTables, uint oldJpegStartOfImageMarker, TiffFillOrder fillOrder, ByteOrder byteOrder)
	{
		return method switch
		{
			TiffDecoderCompressionType.None => new NoneTiffCompression(allocator, width, bitsPerPixel), 
			TiffDecoderCompressionType.PackBits => new PackBitsTiffCompression(allocator, width, bitsPerPixel), 
			TiffDecoderCompressionType.Deflate => new DeflateTiffCompression(allocator, width, bitsPerPixel, colorType, predictor, byteOrder == ByteOrder.BigEndian), 
			TiffDecoderCompressionType.Lzw => new LzwTiffCompression(allocator, width, bitsPerPixel, colorType, predictor, byteOrder == ByteOrder.BigEndian), 
			TiffDecoderCompressionType.T4 => new T4TiffCompression(allocator, fillOrder, width, bitsPerPixel, faxOptions, photometricInterpretation), 
			TiffDecoderCompressionType.T6 => new T6TiffCompression(allocator, fillOrder, width, bitsPerPixel, photometricInterpretation), 
			TiffDecoderCompressionType.HuffmanRle => new ModifiedHuffmanTiffCompression(allocator, fillOrder, width, bitsPerPixel, photometricInterpretation), 
			TiffDecoderCompressionType.Jpeg => new JpegTiffCompression(new JpegDecoderOptions
			{
				GeneralOptions = options
			}, allocator, width, bitsPerPixel, jpegTables, photometricInterpretation), 
			TiffDecoderCompressionType.OldJpeg => new OldJpegTiffCompression(new JpegDecoderOptions
			{
				GeneralOptions = options
			}, allocator, width, bitsPerPixel, oldJpegStartOfImageMarker, photometricInterpretation), 
			TiffDecoderCompressionType.Webp => new WebpTiffCompression(options, allocator, width, bitsPerPixel), 
			_ => throw TiffThrowHelper.NotSupportedDecompressor("method"), 
		};
	}
}
