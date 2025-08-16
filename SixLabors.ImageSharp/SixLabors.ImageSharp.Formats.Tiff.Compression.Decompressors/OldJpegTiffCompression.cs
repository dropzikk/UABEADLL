using System;
using System.Threading;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class OldJpegTiffCompression : TiffBaseDecompressor
{
	private readonly JpegDecoderOptions options;

	private readonly uint startOfImageMarker;

	private readonly TiffPhotometricInterpretation photometricInterpretation;

	public OldJpegTiffCompression(JpegDecoderOptions options, MemoryAllocator memoryAllocator, int width, int bitsPerPixel, uint startOfImageMarker, TiffPhotometricInterpretation photometricInterpretation)
		: base(memoryAllocator, width, bitsPerPixel)
	{
		this.options = options;
		this.startOfImageMarker = startOfImageMarker;
		this.photometricInterpretation = photometricInterpretation;
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		long position = stream.Position;
		stream.Position = startOfImageMarker;
		DecodeJpegData(stream, buffer, cancellationToken);
		stream.Position = position + byteCount;
	}

	private void DecodeJpegData(BufferedReadStream stream, Span<byte> buffer, CancellationToken cancellationToken)
	{
		using JpegDecoderCore jpegDecoderCore = new JpegDecoderCore(options);
		Configuration configuration = options.GeneralOptions.Configuration;
		switch (photometricInterpretation)
		{
		case TiffPhotometricInterpretation.WhiteIsZero:
		case TiffPhotometricInterpretation.BlackIsZero:
		{
			using SpectralConverter<L8> spectralConverter2 = new GrayJpegSpectralConverter<L8>(configuration);
			jpegDecoderCore.ParseStream(stream, spectralConverter2, cancellationToken);
			using Buffer2D<L8> pixelBuffer2 = spectralConverter2.GetPixelBuffer(cancellationToken);
			JpegCompressionUtils.CopyImageBytesToBuffer(spectralConverter2.Configuration, buffer, pixelBuffer2);
			break;
		}
		case TiffPhotometricInterpretation.Rgb:
		case TiffPhotometricInterpretation.YCbCr:
		{
			using SpectralConverter<Rgb24> spectralConverter = new TiffOldJpegSpectralConverter<Rgb24>(configuration, photometricInterpretation);
			jpegDecoderCore.ParseStream(stream, spectralConverter, cancellationToken);
			using Buffer2D<Rgb24> pixelBuffer = spectralConverter.GetPixelBuffer(cancellationToken);
			JpegCompressionUtils.CopyImageBytesToBuffer(spectralConverter.Configuration, buffer, pixelBuffer);
			break;
		}
		default:
			TiffThrowHelper.ThrowNotSupported($"Jpeg compressed tiff with photometric interpretation {photometricInterpretation} is not supported");
			break;
		}
	}

	protected override void Dispose(bool disposing)
	{
	}
}
