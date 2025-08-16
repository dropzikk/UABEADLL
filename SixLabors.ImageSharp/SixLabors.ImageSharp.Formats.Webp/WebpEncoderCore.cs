using System.IO;
using System.Threading;
using SixLabors.ImageSharp.Formats.Webp.Chunks;
using SixLabors.ImageSharp.Formats.Webp.Lossless;
using SixLabors.ImageSharp.Formats.Webp.Lossy;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp;

internal sealed class WebpEncoderCore : IImageEncoderInternals
{
	private readonly MemoryAllocator memoryAllocator;

	private readonly bool alphaCompression;

	private readonly uint quality;

	private readonly WebpEncodingMethod method;

	private readonly int entropyPasses;

	private readonly int spatialNoiseShaping;

	private readonly int filterStrength;

	private readonly WebpTransparentColorMode transparentColorMode;

	private readonly bool skipMetadata;

	private readonly bool nearLossless;

	private readonly int nearLosslessQuality;

	private readonly WebpFileFormatType? fileFormat;

	private readonly Configuration configuration;

	public WebpEncoderCore(WebpEncoder encoder, Configuration configuration)
	{
		this.configuration = configuration;
		memoryAllocator = configuration.MemoryAllocator;
		alphaCompression = encoder.UseAlphaCompression;
		fileFormat = encoder.FileFormat;
		quality = (uint)encoder.Quality;
		method = encoder.Method;
		entropyPasses = encoder.EntropyPasses;
		spatialNoiseShaping = encoder.SpatialNoiseShaping;
		filterStrength = encoder.FilterStrength;
		transparentColorMode = encoder.TransparentColorMode;
		skipMetadata = encoder.SkipMetadata;
		nearLossless = encoder.NearLossless;
		nearLosslessQuality = encoder.NearLosslessQuality;
	}

	public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(image, "image");
		Guard.NotNull(stream, "stream");
		WebpFileFormatType? webpFileFormatType = fileFormat;
		if ((!webpFileFormatType.HasValue) ? (WebpCommonUtils.GetWebpMetadata(image).FileFormat == WebpFileFormatType.Lossless) : (fileFormat == WebpFileFormatType.Lossless))
		{
			bool flag = image.Frames.Count > 1;
			using Vp8LEncoder vp8LEncoder = new Vp8LEncoder(memoryAllocator, configuration, image.Width, image.Height, quality, skipMetadata, method, transparentColorMode, nearLossless, nearLosslessQuality);
			long position = stream.Position;
			bool flag2 = false;
			WebpVp8X vp8x = vp8LEncoder.EncodeHeader(image, stream, flag);
			ImageFrame<TPixel> imageFrame = image.Frames.RootFrame;
			WebpFrameMetadata webpFrameMetadata = WebpCommonUtils.GetWebpFrameMetadata(imageFrame);
			flag2 |= vp8LEncoder.Encode(imageFrame, imageFrame.Bounds(), webpFrameMetadata, stream, flag);
			if (flag)
			{
				WebpDisposalMethod disposalMethod = webpFrameMetadata.DisposalMethod;
				using ImageFrame<TPixel> imageFrame3 = new ImageFrame<TPixel>(image.Configuration, imageFrame.Size());
				for (int i = 1; i < image.Frames.Count; i++)
				{
					ImageFrame<TPixel> previousFrame = ((disposalMethod == WebpDisposalMethod.RestoreToBackground) ? null : imageFrame);
					ImageFrame<TPixel> imageFrame2 = image.Frames[i];
					ImageFrame<TPixel> nextFrame = ((i < image.Frames.Count - 1) ? image.Frames[i + 1] : null);
					webpFrameMetadata = WebpCommonUtils.GetWebpFrameMetadata(imageFrame2);
					bool blend = webpFrameMetadata.BlendMethod == WebpBlendMethod.Over;
					Rectangle item = AnimationUtilities.DeDuplicatePixels(image.Configuration, previousFrame, imageFrame2, nextFrame, imageFrame3, Color.Transparent, blend, ClampingMode.Even).Bounds;
					using Vp8LEncoder vp8LEncoder2 = new Vp8LEncoder(memoryAllocator, configuration, item.Width, item.Height, quality, skipMetadata, method, transparentColorMode, nearLossless, nearLosslessQuality);
					flag2 |= vp8LEncoder2.Encode(imageFrame3, item, webpFrameMetadata, stream, flag);
					imageFrame = imageFrame2;
					disposalMethod = webpFrameMetadata.DisposalMethod;
				}
			}
			vp8LEncoder.EncodeFooter(image, in vp8x, flag2, stream, position);
			return;
		}
		using Vp8Encoder vp8Encoder = new Vp8Encoder(memoryAllocator, configuration, image.Width, image.Height, quality, skipMetadata, method, entropyPasses, filterStrength, spatialNoiseShaping, alphaCompression);
		long position2 = stream.Position;
		bool flag3 = false;
		WebpVp8X vp8x2 = default(WebpVp8X);
		if (image.Frames.Count > 1)
		{
			vp8x2 = vp8Encoder.EncodeHeader(image, stream, hasAlpha: false, hasAnimation: true);
			ImageFrame<TPixel> imageFrame4 = image.Frames.RootFrame;
			WebpFrameMetadata webpFrameMetadata2 = WebpCommonUtils.GetWebpFrameMetadata(imageFrame4);
			WebpDisposalMethod disposalMethod2 = webpFrameMetadata2.DisposalMethod;
			flag3 |= vp8Encoder.EncodeAnimation(imageFrame4, stream, imageFrame4.Bounds(), webpFrameMetadata2);
			using ImageFrame<TPixel> imageFrame6 = new ImageFrame<TPixel>(image.Configuration, imageFrame4.Size());
			for (int j = 1; j < image.Frames.Count; j++)
			{
				ImageFrame<TPixel> previousFrame2 = ((disposalMethod2 == WebpDisposalMethod.RestoreToBackground) ? null : imageFrame4);
				ImageFrame<TPixel> imageFrame5 = image.Frames[j];
				ImageFrame<TPixel> nextFrame2 = ((j < image.Frames.Count - 1) ? image.Frames[j + 1] : null);
				webpFrameMetadata2 = WebpCommonUtils.GetWebpFrameMetadata(imageFrame5);
				bool blend2 = webpFrameMetadata2.BlendMethod == WebpBlendMethod.Over;
				Rectangle item2 = AnimationUtilities.DeDuplicatePixels(image.Configuration, previousFrame2, imageFrame5, nextFrame2, imageFrame6, Color.Transparent, blend2, ClampingMode.Even).Bounds;
				using Vp8Encoder vp8Encoder2 = new Vp8Encoder(memoryAllocator, configuration, item2.Width, item2.Height, quality, skipMetadata, method, entropyPasses, filterStrength, spatialNoiseShaping, alphaCompression);
				flag3 |= vp8Encoder2.EncodeAnimation(imageFrame6, stream, item2, webpFrameMetadata2);
				imageFrame4 = imageFrame5;
				disposalMethod2 = webpFrameMetadata2.DisposalMethod;
			}
			vp8Encoder.EncodeFooter(image, in vp8x2, flag3, stream, position2);
			return;
		}
		vp8Encoder.EncodeStatic(stream, image);
		vp8Encoder.EncodeFooter(image, in vp8x2, flag3, stream, position2);
	}
}
