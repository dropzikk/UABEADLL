using System;
using System.IO;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal class TiffJpegCompressor : TiffBaseCompressor
{
	public override TiffCompression Method => TiffCompression.Jpeg;

	public TiffJpegCompressor(Stream output, MemoryAllocator memoryAllocator, int width, int bitsPerPixel, TiffPredictor predictor = TiffPredictor.None)
		: base(output, memoryAllocator, width, bitsPerPixel, predictor)
	{
	}

	public override void Initialize(int rowsPerStrip)
	{
	}

	public override void CompressStrip(Span<byte> rows, int height)
	{
		int width = rows.Length / 3 / height;
		using MemoryStream memoryStream = new MemoryStream();
		Image.LoadPixelData<Rgb24>(rows, width, height).Save(memoryStream, new JpegEncoder
		{
			ColorType = JpegEncodingColor.Rgb
		});
		memoryStream.Position = 0L;
		memoryStream.WriteTo(base.Output);
	}

	protected override void Dispose(bool disposing)
	{
	}
}
