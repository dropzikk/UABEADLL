using System;
using System.Buffers;
using SixLabors.ImageSharp.Formats.Webp.Chunks;
using SixLabors.ImageSharp.Formats.Webp.Lossless;
using SixLabors.ImageSharp.Formats.Webp.Lossy;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp;

internal class WebpAnimationDecoder : IDisposable
{
	private readonly MemoryAllocator memoryAllocator;

	private readonly Configuration configuration;

	private readonly uint maxFrames;

	private Rectangle? restoreArea;

	private ImageMetadata? metadata;

	private WebpMetadata? webpMetadata;

	private IMemoryOwner<byte>? alphaData;

	private readonly BackgroundColorHandling backgroundColorHandling;

	public WebpAnimationDecoder(MemoryAllocator memoryAllocator, Configuration configuration, uint maxFrames, BackgroundColorHandling backgroundColorHandling)
	{
		this.memoryAllocator = memoryAllocator;
		this.configuration = configuration;
		this.maxFrames = maxFrames;
		this.backgroundColorHandling = backgroundColorHandling;
	}

	public Image<TPixel> Decode<TPixel>(BufferedReadStream stream, WebpFeatures features, uint width, uint height, uint completeDataSize) where TPixel : unmanaged, IPixel<TPixel>
	{
		Image<TPixel> image = null;
		ImageFrame<TPixel> previousFrame = null;
		metadata = new ImageMetadata();
		webpMetadata = metadata.GetWebpMetadata();
		webpMetadata.RepeatCount = features.AnimationLoopCount;
		Span<byte> buffer = stackalloc byte[4];
		uint num = 0u;
		int num2 = (int)completeDataSize;
		while (num2 > 0)
		{
			WebpChunkType webpChunkType = WebpChunkParsingUtils.ReadChunkType(stream, buffer);
			num2 -= 4;
			switch (webpChunkType)
			{
			case WebpChunkType.FrameData:
			{
				Color backgroundColor = ((backgroundColorHandling == BackgroundColorHandling.Ignore) ? new Color(new Bgra32(0, 0, 0, 0)) : features.AnimationBackgroundColor.Value);
				uint num3 = ReadFrame(stream, ref image, ref previousFrame, width, height, backgroundColor);
				num2 -= (int)num3;
				break;
			}
			case WebpChunkType.Exif:
			case WebpChunkType.Xmp:
				WebpChunkParsingUtils.ParseOptionalChunks(stream, webpChunkType, image.Metadata, ignoreMetaData: false, buffer);
				break;
			default:
				WebpThrowHelper.ThrowImageFormatException("Read unexpected webp chunk data");
				break;
			}
			if (stream.Position == stream.Length || ++num == maxFrames)
			{
				break;
			}
		}
		return image;
	}

	private uint ReadFrame<TPixel>(BufferedReadStream stream, ref Image<TPixel>? image, ref ImageFrame<TPixel>? previousFrame, uint width, uint height, Color backgroundColor) where TPixel : unmanaged, IPixel<TPixel>
	{
		WebpFrameData frameData = WebpFrameData.Parse(stream);
		long position = stream.Position;
		Span<byte> buffer = stackalloc byte[4];
		WebpChunkType webpChunkType = WebpChunkParsingUtils.ReadChunkType(stream, buffer);
		bool flag = false;
		byte alphaChunkHeader = 0;
		if (webpChunkType == WebpChunkType.Alpha)
		{
			alphaChunkHeader = ReadAlphaData(stream);
			flag = true;
			webpChunkType = WebpChunkParsingUtils.ReadChunkType(stream, buffer);
		}
		WebpImageInfo webpInfo = null;
		WebpFeatures webpFeatures = new WebpFeatures();
		switch (webpChunkType)
		{
		case WebpChunkType.Vp8:
			webpInfo = WebpChunkParsingUtils.ReadVp8Header(memoryAllocator, stream, buffer, webpFeatures);
			webpFeatures.Alpha = flag;
			webpFeatures.AlphaChunkHeader = alphaChunkHeader;
			break;
		case WebpChunkType.Vp8L:
			if (flag)
			{
				WebpThrowHelper.ThrowNotSupportedException("Alpha channel is not supported for lossless webp images.");
			}
			webpInfo = WebpChunkParsingUtils.ReadVp8LHeader(memoryAllocator, stream, buffer, webpFeatures);
			break;
		default:
			WebpThrowHelper.ThrowImageFormatException("Read unexpected chunk type, should be VP8 or VP8L");
			break;
		}
		ImageFrame<TPixel> imageFrame = null;
		ImageFrame<TPixel> imageFrame2;
		if (previousFrame == null)
		{
			image = new Image<TPixel>(configuration, (int)width, (int)height, backgroundColor.ToPixel<TPixel>(), metadata);
			SetFrameMetadata(image.Frames.RootFrame.Metadata, frameData);
			imageFrame2 = image.Frames.RootFrame;
		}
		else
		{
			imageFrame = image.Frames.AddFrame(previousFrame);
			SetFrameMetadata(imageFrame.Metadata, frameData);
			imageFrame2 = imageFrame;
		}
		Rectangle bounds = frameData.Bounds;
		if (frameData.DisposalMethod == WebpDisposalMethod.RestoreToBackground)
		{
			RestoreToBackground(imageFrame2, backgroundColor);
		}
		using Buffer2D<TPixel> decodedImageFrame = DecodeImageFrameData<TPixel>(frameData, webpInfo);
		bool blend = previousFrame != null && frameData.BlendingMethod == WebpBlendMethod.Over;
		DrawDecodedImageFrameOnCanvas(decodedImageFrame, imageFrame2, bounds, blend);
		previousFrame = imageFrame ?? image.Frames.RootFrame;
		restoreArea = bounds;
		return (uint)(stream.Position - position);
	}

	private static void SetFrameMetadata(ImageFrameMetadata meta, WebpFrameData frameData)
	{
		WebpFrameMetadata webpFrameMetadata = meta.GetWebpMetadata();
		webpFrameMetadata.FrameDelay = frameData.Duration;
		webpFrameMetadata.BlendMethod = frameData.BlendingMethod;
		webpFrameMetadata.DisposalMethod = frameData.DisposalMethod;
	}

	private byte ReadAlphaData(BufferedReadStream stream)
	{
		alphaData?.Dispose();
		Span<byte> buffer = stackalloc byte[4];
		int num = (int)(WebpChunkParsingUtils.ReadChunkSize(stream, buffer) - 1);
		alphaData = memoryAllocator.Allocate<byte>(num);
		byte result = (byte)stream.ReadByte();
		Span<byte> span = alphaData.GetSpan();
		stream.Read(span, 0, num);
		return result;
	}

	private Buffer2D<TPixel> DecodeImageFrameData<TPixel>(WebpFrameData frameData, WebpImageInfo webpInfo) where TPixel : unmanaged, IPixel<TPixel>
	{
		ImageFrame<TPixel> imageFrame = new ImageFrame<TPixel>(configuration, (int)frameData.Width, (int)frameData.Height);
		try
		{
			Buffer2D<TPixel> pixelBuffer = imageFrame.PixelBuffer;
			if (webpInfo.IsLossless)
			{
				new WebpLosslessDecoder(webpInfo.Vp8LBitReader, memoryAllocator, configuration).Decode(pixelBuffer, (int)webpInfo.Width, (int)webpInfo.Height);
			}
			else
			{
				new WebpLossyDecoder(webpInfo.Vp8BitReader, memoryAllocator, configuration).Decode(pixelBuffer, (int)webpInfo.Width, (int)webpInfo.Height, webpInfo, alphaData);
			}
			return pixelBuffer;
		}
		catch
		{
			imageFrame?.Dispose();
			throw;
		}
		finally
		{
			webpInfo.Dispose();
		}
	}

	private static void DrawDecodedImageFrameOnCanvas<TPixel>(Buffer2D<TPixel> decodedImageFrame, ImageFrame<TPixel> imageFrame, Rectangle restoreArea, bool blend) where TPixel : unmanaged, IPixel<TPixel>
	{
		Buffer2DRegion<TPixel> region = imageFrame.PixelBuffer.GetRegion(restoreArea);
		Span<TPixel> span2;
		if (blend)
		{
			PixelBlender<TPixel> pixelBlender = PixelOperations<TPixel>.Instance.GetPixelBlender(PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcOver);
			for (int i = 0; i < restoreArea.Height; i++)
			{
				Span<TPixel> span = region.DangerousGetRowSpan(i);
				span2 = decodedImageFrame.DangerousGetRowSpan(i);
				Span<TPixel> span3 = span2.Slice(0, restoreArea.Width);
				pixelBlender.Blend<TPixel>(imageFrame.Configuration, span, span, span3, 1f);
			}
		}
		else
		{
			for (int j = 0; j < restoreArea.Height; j++)
			{
				Span<TPixel> destination = region.DangerousGetRowSpan(j);
				span2 = decodedImageFrame.DangerousGetRowSpan(j);
				span2.Slice(0, restoreArea.Width).CopyTo(destination);
			}
		}
	}

	private void RestoreToBackground<TPixel>(ImageFrame<TPixel> imageFrame, Color backgroundColor) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (restoreArea.HasValue)
		{
			Rectangle rectangle = Rectangle.Intersect(imageFrame.Bounds(), restoreArea.Value);
			Buffer2DRegion<TPixel> region = imageFrame.PixelBuffer.GetRegion(rectangle);
			TPixel value = backgroundColor.ToPixel<TPixel>();
			region.Fill(value);
		}
	}

	public void Dispose()
	{
		alphaData?.Dispose();
	}
}
