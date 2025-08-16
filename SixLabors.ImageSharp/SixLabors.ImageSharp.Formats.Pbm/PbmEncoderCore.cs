using System;
using System.Buffers.Text;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Pbm;

internal sealed class PbmEncoderCore : IImageEncoderInternals
{
	private const byte NewLine = 10;

	private const byte Space = 32;

	private const byte P = 80;

	private Configuration configuration;

	private readonly PbmEncoder encoder;

	private PbmEncoding encoding;

	private PbmColorType colorType;

	private PbmComponentType componentType;

	public PbmEncoderCore(Configuration configuration, PbmEncoder encoder)
	{
		this.configuration = configuration;
		this.encoder = encoder;
	}

	public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(image, "image");
		Guard.NotNull(stream, "stream");
		SanitizeAndSetEncoderOptions(image);
		byte signature = DeduceSignature();
		WriteHeader(stream, signature, image.Size);
		WritePixels(stream, image.Frames.RootFrame);
		stream.Flush();
	}

	private void SanitizeAndSetEncoderOptions<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		configuration = image.Configuration;
		PbmMetadata pbmMetadata = image.Metadata.GetPbmMetadata();
		encoding = encoder.Encoding ?? pbmMetadata.Encoding;
		colorType = encoder.ColorType ?? pbmMetadata.ColorType;
		if (colorType != 0)
		{
			componentType = encoder.ComponentType ?? pbmMetadata.ComponentType;
		}
		else
		{
			componentType = PbmComponentType.Bit;
		}
	}

	private byte DeduceSignature()
	{
		if (colorType == PbmColorType.BlackAndWhite)
		{
			if (encoding == PbmEncoding.Plain)
			{
				return 49;
			}
			return 52;
		}
		if (colorType == PbmColorType.Grayscale)
		{
			if (encoding == PbmEncoding.Plain)
			{
				return 50;
			}
			return 53;
		}
		if (encoding == PbmEncoding.Plain)
		{
			return 51;
		}
		return 54;
	}

	private void WriteHeader(Stream stream, byte signature, Size pixelSize)
	{
		Span<byte> buffer = stackalloc byte[128];
		int num = 3;
		buffer[0] = 80;
		buffer[1] = signature;
		buffer[2] = 10;
		int width = pixelSize.Width;
		ref Span<byte> reference = ref buffer;
		int num2 = num;
		Utf8Formatter.TryFormat(width, reference.Slice(num2, reference.Length - num2), out var bytesWritten);
		num += bytesWritten;
		buffer[num++] = 32;
		int height = pixelSize.Height;
		reference = ref buffer;
		num2 = num;
		Utf8Formatter.TryFormat(height, reference.Slice(num2, reference.Length - num2), out bytesWritten);
		num += bytesWritten;
		buffer[num++] = 10;
		if (colorType != 0)
		{
			int value = ((componentType == PbmComponentType.Short) ? 65535 : 255);
			reference = ref buffer;
			num2 = num;
			Utf8Formatter.TryFormat(value, reference.Slice(num2, reference.Length - num2), out bytesWritten);
			num += bytesWritten;
			buffer[num++] = 10;
		}
		stream.Write(buffer, 0, num);
	}

	private void WritePixels<TPixel>(Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (encoding == PbmEncoding.Plain)
		{
			PlainEncoder.WritePixels(configuration, stream, image, colorType, componentType);
		}
		else
		{
			BinaryEncoder.WritePixels(configuration, stream, image, colorType, componentType);
		}
	}
}
