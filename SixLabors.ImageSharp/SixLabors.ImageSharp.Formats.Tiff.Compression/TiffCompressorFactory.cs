using System.IO;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

internal static class TiffCompressorFactory
{
	public static TiffBaseCompressor Create(TiffCompression method, Stream output, MemoryAllocator allocator, int width, int bitsPerPixel, DeflateCompressionLevel compressionLevel, TiffPredictor predictor)
	{
		switch (method)
		{
		case TiffCompression.None:
		case TiffCompression.OldJpeg:
		case TiffCompression.ItuTRecT82:
		case TiffCompression.ItuTRecT43:
		case TiffCompression.OldDeflate:
			return new NoCompressor(output, allocator, width, bitsPerPixel);
		case TiffCompression.Jpeg:
			return new TiffJpegCompressor(output, allocator, width, bitsPerPixel);
		case TiffCompression.PackBits:
			return new PackBitsCompressor(output, allocator, width, bitsPerPixel);
		case TiffCompression.Deflate:
			return new DeflateCompressor(output, allocator, width, bitsPerPixel, predictor, compressionLevel);
		case TiffCompression.Lzw:
			return new LzwCompressor(output, allocator, width, bitsPerPixel, predictor);
		case TiffCompression.CcittGroup3Fax:
			return new T4BitCompressor(output, allocator, width, bitsPerPixel);
		case TiffCompression.CcittGroup4Fax:
			return new T6BitCompressor(output, allocator, width, bitsPerPixel);
		case TiffCompression.Ccitt1D:
			return new T4BitCompressor(output, allocator, width, bitsPerPixel, useModifiedHuffman: true);
		default:
			throw TiffThrowHelper.NotSupportedCompressor(method.ToString());
		}
	}
}
