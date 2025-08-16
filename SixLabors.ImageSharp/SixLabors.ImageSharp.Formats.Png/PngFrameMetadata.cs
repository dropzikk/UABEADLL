using SixLabors.ImageSharp.Formats.Png.Chunks;

namespace SixLabors.ImageSharp.Formats.Png;

public class PngFrameMetadata : IDeepCloneable
{
	public Rational FrameDelay { get; set; } = new Rational(0u);

	public PngDisposalMethod DisposalMethod { get; set; }

	public PngBlendMethod BlendMethod { get; set; }

	public PngFrameMetadata()
	{
	}

	private PngFrameMetadata(PngFrameMetadata other)
	{
		FrameDelay = other.FrameDelay;
		DisposalMethod = other.DisposalMethod;
		BlendMethod = other.BlendMethod;
	}

	internal void FromChunk(in FrameControl frameControl)
	{
		FrameDelay = new Rational(frameControl.DelayNumerator, frameControl.DelayDenominator);
		DisposalMethod = frameControl.DisposeOperation;
		BlendMethod = frameControl.BlendOperation;
	}

	public IDeepCloneable DeepClone()
	{
		return new PngFrameMetadata(this);
	}

	internal static PngFrameMetadata FromAnimatedMetadata(AnimatedImageFrameMetadata metadata)
	{
		return new PngFrameMetadata
		{
			FrameDelay = new Rational(metadata.Duration.TotalMilliseconds / 1000.0),
			DisposalMethod = GetMode(metadata.DisposalMode),
			BlendMethod = ((metadata.BlendMode != 0) ? PngBlendMethod.Over : PngBlendMethod.Source)
		};
	}

	private static PngDisposalMethod GetMode(FrameDisposalMode mode)
	{
		return mode switch
		{
			FrameDisposalMode.RestoreToBackground => PngDisposalMethod.RestoreToBackground, 
			FrameDisposalMode.RestoreToPrevious => PngDisposalMethod.RestoreToPrevious, 
			FrameDisposalMode.DoNotDispose => PngDisposalMethod.DoNotDispose, 
			_ => PngDisposalMethod.DoNotDispose, 
		};
	}
}
