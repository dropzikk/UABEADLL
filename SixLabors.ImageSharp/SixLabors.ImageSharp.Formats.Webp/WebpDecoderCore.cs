using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Formats.Webp.Lossless;
using SixLabors.ImageSharp.Formats.Webp.Lossy;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp;

internal sealed class WebpDecoderCore : IImageDecoderInternals, IDisposable
{
	private readonly Configuration configuration;

	private readonly bool skipMetadata;

	private readonly uint maxFrames;

	private IMemoryOwner<byte>? alphaData;

	private readonly MemoryAllocator memoryAllocator;

	private WebpImageInfo? webImageInfo;

	private readonly BackgroundColorHandling backgroundColorHandling;

	public DecoderOptions Options { get; }

	public Size Dimensions => new Size((int)webImageInfo.Width, (int)webImageInfo.Height);

	public WebpDecoderCore(WebpDecoderOptions options)
	{
		Options = options.GeneralOptions;
		backgroundColorHandling = options.BackgroundColorHandling;
		configuration = options.GeneralOptions.Configuration;
		skipMetadata = options.GeneralOptions.SkipMetadata;
		maxFrames = options.GeneralOptions.MaxFrames;
		memoryAllocator = configuration.MemoryAllocator;
	}

	public Image<TPixel> Decode<TPixel>(BufferedReadStream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Image<TPixel> image = null;
		try
		{
			ImageMetadata metadata = new ImageMetadata();
			Span<byte> buffer = stackalloc byte[4];
			uint completeDataSize = ReadImageHeader(stream, buffer);
			using (webImageInfo = ReadVp8Info(stream, metadata))
			{
				WebpFeatures features = webImageInfo.Features;
				if (features != null && features.Animation)
				{
					using (WebpAnimationDecoder webpAnimationDecoder = new WebpAnimationDecoder(memoryAllocator, configuration, maxFrames, backgroundColorHandling))
					{
						return webpAnimationDecoder.Decode<TPixel>(stream, webImageInfo.Features, webImageInfo.Width, webImageInfo.Height, completeDataSize);
					}
				}
				image = new Image<TPixel>(configuration, (int)webImageInfo.Width, (int)webImageInfo.Height, metadata);
				Buffer2D<TPixel> rootFramePixelBuffer = image.GetRootFramePixelBuffer();
				if (webImageInfo.IsLossless)
				{
					new WebpLosslessDecoder(webImageInfo.Vp8LBitReader, memoryAllocator, configuration).Decode(rootFramePixelBuffer, image.Width, image.Height);
				}
				else
				{
					new WebpLossyDecoder(webImageInfo.Vp8BitReader, memoryAllocator, configuration).Decode(rootFramePixelBuffer, image.Width, image.Height, webImageInfo, alphaData);
				}
				if (webImageInfo.Features != null)
				{
					ParseOptionalChunks(stream, metadata, webImageInfo.Features, buffer);
				}
				return image;
			}
		}
		catch
		{
			image?.Dispose();
			throw;
		}
	}

	public ImageInfo Identify(BufferedReadStream stream, CancellationToken cancellationToken)
	{
		Span<byte> buffer = stackalloc byte[4];
		ReadImageHeader(stream, buffer);
		ImageMetadata metadata = new ImageMetadata();
		using (webImageInfo = ReadVp8Info(stream, metadata, ignoreAlpha: true))
		{
			return new ImageInfo(new PixelTypeInfo((int)webImageInfo.BitsPerPixel), new Size((int)webImageInfo.Width, (int)webImageInfo.Height), metadata);
		}
	}

	private static uint ReadImageHeader(BufferedReadStream stream, Span<byte> buffer)
	{
		stream.Skip(4);
		uint result = WebpChunkParsingUtils.ReadChunkSize(stream, buffer);
		stream.Skip(4);
		return result;
	}

	private WebpImageInfo ReadVp8Info(BufferedReadStream stream, ImageMetadata metadata, bool ignoreAlpha = false)
	{
		WebpMetadata formatMetadata = metadata.GetFormatMetadata(WebpFormat.Instance);
		Span<byte> buffer = stackalloc byte[4];
		WebpChunkType webpChunkType = WebpChunkParsingUtils.ReadChunkType(stream, buffer);
		WebpFeatures features = new WebpFeatures();
		switch (webpChunkType)
		{
		case WebpChunkType.Vp8:
			formatMetadata.FileFormat = WebpFileFormatType.Lossy;
			return WebpChunkParsingUtils.ReadVp8Header(memoryAllocator, stream, buffer, features);
		case WebpChunkType.Vp8L:
			formatMetadata.FileFormat = WebpFileFormatType.Lossless;
			return WebpChunkParsingUtils.ReadVp8LHeader(memoryAllocator, stream, buffer, features);
		case WebpChunkType.Vp8X:
		{
			WebpImageInfo result = WebpChunkParsingUtils.ReadVp8XHeader(stream, buffer, features);
			while (stream.Position < stream.Length)
			{
				webpChunkType = WebpChunkParsingUtils.ReadChunkType(stream, buffer);
				switch (webpChunkType)
				{
				case WebpChunkType.Vp8:
					formatMetadata.FileFormat = WebpFileFormatType.Lossy;
					result = WebpChunkParsingUtils.ReadVp8Header(memoryAllocator, stream, buffer, features);
					continue;
				case WebpChunkType.Vp8L:
					formatMetadata.FileFormat = WebpFileFormatType.Lossless;
					result = WebpChunkParsingUtils.ReadVp8LHeader(memoryAllocator, stream, buffer, features);
					continue;
				}
				if (WebpChunkParsingUtils.IsOptionalVp8XChunk(webpChunkType))
				{
					if (ParseOptionalExtendedChunks(stream, metadata, webpChunkType, features, ignoreAlpha, buffer))
					{
						return result;
					}
				}
				else
				{
					uint count = ReadChunkSize(stream, buffer);
					stream.Skip((int)count);
				}
			}
			return result;
		}
		default:
			WebpThrowHelper.ThrowImageFormatException("Unrecognized VP8 header");
			return new WebpImageInfo();
		}
	}

	private bool ParseOptionalExtendedChunks(BufferedReadStream stream, ImageMetadata metadata, WebpChunkType chunkType, WebpFeatures features, bool ignoreAlpha, Span<byte> buffer)
	{
		switch (chunkType)
		{
		case WebpChunkType.Iccp:
			ReadIccProfile(stream, metadata, buffer);
			break;
		case WebpChunkType.Exif:
			ReadExifProfile(stream, metadata, buffer);
			break;
		case WebpChunkType.Xmp:
			ReadXmpProfile(stream, metadata, buffer);
			break;
		case WebpChunkType.AnimationParameter:
			ReadAnimationParameters(stream, features, buffer);
			return true;
		case WebpChunkType.Alpha:
			ReadAlphaData(stream, features, ignoreAlpha, buffer);
			break;
		default:
			WebpThrowHelper.ThrowImageFormatException("Unexpected chunk followed VP8X header");
			break;
		}
		return false;
	}

	private void ParseOptionalChunks(BufferedReadStream stream, ImageMetadata metadata, WebpFeatures features, Span<byte> buffer)
	{
		if (skipMetadata || (!features.ExifProfile && !features.XmpMetaData))
		{
			return;
		}
		long length = stream.Length;
		while (stream.Position < length)
		{
			WebpChunkType webpChunkType = ReadChunkType(stream, buffer);
			if (webpChunkType == WebpChunkType.Exif && metadata.ExifProfile == null)
			{
				ReadExifProfile(stream, metadata, buffer);
				continue;
			}
			if (webpChunkType == WebpChunkType.Xmp && metadata.XmpProfile == null)
			{
				ReadXmpProfile(stream, metadata, buffer);
				continue;
			}
			uint count = ReadChunkSize(stream, buffer);
			stream.Skip((int)count);
		}
	}

	private void ReadExifProfile(BufferedReadStream stream, ImageMetadata metadata, Span<byte> buffer)
	{
		uint num = ReadChunkSize(stream, buffer);
		if (skipMetadata)
		{
			stream.Skip((int)num);
			return;
		}
		byte[] array = new byte[num];
		if (stream.Read(array, 0, (int)num) == num)
		{
			ExifProfile exifProfile = new ExifProfile(array);
			double exifResolutionValue = GetExifResolutionValue(exifProfile, ExifTag.XResolution);
			double exifResolutionValue2 = GetExifResolutionValue(exifProfile, ExifTag.YResolution);
			if (exifResolutionValue > 0.0 && exifResolutionValue2 > 0.0)
			{
				metadata.HorizontalResolution = exifResolutionValue;
				metadata.VerticalResolution = exifResolutionValue2;
				metadata.ResolutionUnits = UnitConverter.ExifProfileToResolutionUnit(exifProfile);
			}
			metadata.ExifProfile = exifProfile;
		}
	}

	private static double GetExifResolutionValue(ExifProfile exifProfile, ExifTag<Rational> tag)
	{
		if (exifProfile.TryGetValue(tag, out IExifValue<Rational> exifValue))
		{
			return exifValue.Value.ToDouble();
		}
		return 0.0;
	}

	private void ReadXmpProfile(BufferedReadStream stream, ImageMetadata metadata, Span<byte> buffer)
	{
		uint num = ReadChunkSize(stream, buffer);
		if (skipMetadata)
		{
			stream.Skip((int)num);
			return;
		}
		byte[] array = new byte[num];
		if (stream.Read(array, 0, (int)num) == num)
		{
			metadata.XmpProfile = new XmpProfile(array);
		}
	}

	private void ReadIccProfile(BufferedReadStream stream, ImageMetadata metadata, Span<byte> buffer)
	{
		uint num = ReadChunkSize(stream, buffer);
		if (skipMetadata)
		{
			stream.Skip((int)num);
			return;
		}
		byte[] array = new byte[num];
		if (stream.Read(array, 0, (int)num) != num)
		{
			WebpThrowHelper.ThrowInvalidImageContentException("Not enough data to read the iccp chunk");
		}
		IccProfile iccProfile = new IccProfile(array);
		if (iccProfile.CheckIsValid())
		{
			metadata.IccProfile = iccProfile;
		}
	}

	private static void ReadAnimationParameters(BufferedReadStream stream, WebpFeatures features, Span<byte> buffer)
	{
		features.Animation = true;
		WebpChunkParsingUtils.ReadChunkSize(stream, buffer);
		byte b = (byte)stream.ReadByte();
		byte g = (byte)stream.ReadByte();
		byte r = (byte)stream.ReadByte();
		byte a = (byte)stream.ReadByte();
		features.AnimationBackgroundColor = new Color(new Rgba32(r, g, b, a));
		if (stream.Read(buffer, 0, 2) != 2)
		{
			WebpThrowHelper.ThrowInvalidImageContentException("Not enough data to read the animation loop count");
		}
		features.AnimationLoopCount = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
	}

	private void ReadAlphaData(BufferedReadStream stream, WebpFeatures features, bool ignoreAlpha, Span<byte> buffer)
	{
		uint num = WebpChunkParsingUtils.ReadChunkSize(stream, buffer);
		if (ignoreAlpha)
		{
			stream.Skip((int)num);
			return;
		}
		features.AlphaChunkHeader = (byte)stream.ReadByte();
		int num2 = (int)(num - 1);
		alphaData = memoryAllocator.Allocate<byte>(num2);
		Span<byte> span = alphaData.GetSpan();
		if (stream.Read(span, 0, num2) != num2)
		{
			WebpThrowHelper.ThrowInvalidImageContentException("Not enough data to read the alpha data from the stream");
		}
	}

	private static WebpChunkType ReadChunkType(BufferedReadStream stream, Span<byte> buffer)
	{
		if (stream.Read(buffer, 0, 4) == 4)
		{
			return (WebpChunkType)BinaryPrimitives.ReadUInt32BigEndian(buffer);
		}
		throw new ImageFormatException("Invalid Webp data.");
	}

	private static uint ReadChunkSize(BufferedReadStream stream, Span<byte> buffer)
	{
		if (stream.Read(buffer, 0, 4) == 4)
		{
			uint num = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
			if (num % 2 != 0)
			{
				return num + 1;
			}
			return num;
		}
		throw new ImageFormatException("Invalid Webp data.");
	}

	public void Dispose()
	{
		alphaData?.Dispose();
	}
}
