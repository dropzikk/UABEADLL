using System;
using System.Runtime.InteropServices;
using System.Threading;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal class WebpTiffCompression : TiffBaseDecompressor
{
	private readonly DecoderOptions options;

	public WebpTiffCompression(DecoderOptions options, MemoryAllocator memoryAllocator, int width, int bitsPerPixel, TiffPredictor predictor = TiffPredictor.None)
		: base(memoryAllocator, width, bitsPerPixel, predictor)
	{
		this.options = options;
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		using WebpDecoderCore webpDecoderCore = new WebpDecoderCore(new WebpDecoderOptions());
		using Image<Rgb24> image = webpDecoderCore.Decode<Rgb24>(stream, cancellationToken);
		CopyImageBytesToBuffer(buffer, image.Frames.RootFrame.PixelBuffer);
	}

	private static void CopyImageBytesToBuffer(Span<byte> buffer, Buffer2D<Rgb24> pixelBuffer)
	{
		int num = 0;
		for (int i = 0; i < pixelBuffer.Height; i++)
		{
			Span<byte> span = MemoryMarshal.AsBytes(pixelBuffer.DangerousGetRowSpan(i));
			int num2 = num;
			span.CopyTo(buffer.Slice(num2, buffer.Length - num2));
			num += span.Length;
		}
	}

	protected override void Dispose(bool disposing)
	{
	}
}
