using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.ImageSharp.Formats.Pbm;

internal sealed class PbmDecoderCore : IImageDecoderInternals
{
	private int maxPixelValue;

	private readonly Configuration configuration;

	private PbmColorType colorType;

	private Size pixelSize;

	private PbmComponentType componentType;

	private PbmEncoding encoding;

	private ImageMetadata? metadata;

	public DecoderOptions Options { get; }

	public Size Dimensions => pixelSize;

	public PbmDecoderCore(DecoderOptions options)
	{
		Options = options;
		configuration = options.Configuration;
	}

	public Image<TPixel> Decode<TPixel>(BufferedReadStream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		ProcessHeader(stream);
		Image<TPixel> image = new Image<TPixel>(configuration, pixelSize.Width, pixelSize.Height, metadata);
		Buffer2D<TPixel> rootFramePixelBuffer = image.GetRootFramePixelBuffer();
		ProcessPixels(stream, rootFramePixelBuffer);
		if (NeedsUpscaling())
		{
			ProcessUpscaling(image);
		}
		return image;
	}

	public ImageInfo Identify(BufferedReadStream stream, CancellationToken cancellationToken)
	{
		ProcessHeader(stream);
		return new ImageInfo(new PixelTypeInfo((componentType == PbmComponentType.Short) ? 16 : 8), new Size(pixelSize.Width, pixelSize.Height), metadata);
	}

	private void ProcessHeader(BufferedReadStream stream)
	{
		Span<byte> buffer = stackalloc byte[2];
		if (stream.Read(buffer) != 2 || buffer[0] != 80)
		{
			throw new InvalidImageContentException("Empty or not an PPM image.");
		}
		switch ((char)buffer[1])
		{
		case '1':
			colorType = PbmColorType.BlackAndWhite;
			encoding = PbmEncoding.Plain;
			break;
		case '2':
			colorType = PbmColorType.Grayscale;
			encoding = PbmEncoding.Plain;
			break;
		case '3':
			colorType = PbmColorType.Rgb;
			encoding = PbmEncoding.Plain;
			break;
		case '4':
			colorType = PbmColorType.BlackAndWhite;
			encoding = PbmEncoding.Binary;
			break;
		case '5':
			colorType = PbmColorType.Grayscale;
			encoding = PbmEncoding.Binary;
			break;
		case '6':
			colorType = PbmColorType.Rgb;
			encoding = PbmEncoding.Binary;
			break;
		default:
			throw new InvalidImageContentException("Unknown of not implemented image type encountered.");
		}
		int value = default(int);
		int value2 = default(int);
		if (!stream.SkipWhitespaceAndComments() || !stream.ReadDecimal(out value) || !stream.SkipWhitespaceAndComments() || !stream.ReadDecimal(out value2) || !stream.SkipWhitespaceAndComments())
		{
			ThrowPrematureEof();
		}
		if (colorType != 0)
		{
			if (!stream.ReadDecimal(out maxPixelValue))
			{
				ThrowPrematureEof();
			}
			if (maxPixelValue > 255)
			{
				componentType = PbmComponentType.Short;
			}
			else
			{
				componentType = PbmComponentType.Byte;
			}
			stream.SkipWhitespaceAndComments();
		}
		else
		{
			componentType = PbmComponentType.Bit;
		}
		pixelSize = new Size(value, value2);
		metadata = new ImageMetadata();
		PbmMetadata pbmMetadata = metadata.GetPbmMetadata();
		pbmMetadata.Encoding = encoding;
		pbmMetadata.ColorType = colorType;
		pbmMetadata.ComponentType = componentType;
		[DoesNotReturn]
		static void ThrowPrematureEof()
		{
			throw new InvalidImageContentException("Reached EOF while reading the header.");
		}
	}

	private void ProcessPixels<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (encoding == PbmEncoding.Binary)
		{
			BinaryDecoder.Process(configuration, pixels, stream, colorType, componentType);
		}
		else
		{
			PlainDecoder.Process(configuration, pixels, stream, colorType, componentType);
		}
	}

	private void ProcessUpscaling<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = ((componentType == PbmComponentType.Short) ? 65535 : 255);
		float factor = num / maxPixelValue;
		image.Mutate(delegate(IImageProcessingContext x)
		{
			x.Brightness(factor);
		});
	}

	private bool NeedsUpscaling()
	{
		if (colorType != 0)
		{
			int num = maxPixelValue;
			if (num != 255)
			{
				return num != 65535;
			}
			return false;
		}
		return false;
	}
}
