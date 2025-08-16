namespace SixLabors.ImageSharp.Formats.Webp;

public class WebpFrameMetadata : IDeepCloneable
{
	public WebpBlendMethod BlendMethod { get; set; }

	public WebpDisposalMethod DisposalMethod { get; set; }

	public uint FrameDelay { get; set; }

	public WebpFrameMetadata()
	{
	}

	private WebpFrameMetadata(WebpFrameMetadata other)
	{
		FrameDelay = other.FrameDelay;
		DisposalMethod = other.DisposalMethod;
		BlendMethod = other.BlendMethod;
	}

	public IDeepCloneable DeepClone()
	{
		return new WebpFrameMetadata(this);
	}

	internal static WebpFrameMetadata FromAnimatedMetadata(AnimatedImageFrameMetadata metadata)
	{
		return new WebpFrameMetadata
		{
			FrameDelay = (uint)metadata.Duration.TotalMilliseconds,
			BlendMethod = ((metadata.BlendMode != 0) ? WebpBlendMethod.Over : WebpBlendMethod.Source),
			DisposalMethod = ((metadata.DisposalMode == FrameDisposalMode.RestoreToBackground) ? WebpDisposalMethod.RestoreToBackground : WebpDisposalMethod.DoNotDispose)
		};
	}
}
