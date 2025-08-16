using System;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace Avalonia.Rendering.Composition.Animations;

public abstract class KeyFrameAnimation : CompositionAnimation
{
	private TimeSpan _duration = TimeSpan.FromMilliseconds(1.0);

	public AnimationDelayBehavior DelayBehavior { get; set; }

	public TimeSpan DelayTime { get; set; }

	public PlaybackDirection Direction { get; set; }

	public TimeSpan Duration
	{
		get
		{
			return _duration;
		}
		set
		{
			if (_duration < TimeSpan.FromMilliseconds(1.0) || _duration > TimeSpan.FromDays(1.0))
			{
				throw new ArgumentException("Minimum allowed value is 1ms and maximum allowed value is 24 days.");
			}
			_duration = value;
		}
	}

	public AnimationIterationBehavior IterationBehavior { get; set; }

	public int IterationCount { get; set; } = 1;

	public AnimationStopBehavior StopBehavior { get; set; }

	private protected abstract IKeyFrames KeyFrames { get; }

	internal KeyFrameAnimation(Compositor compositor)
		: base(compositor)
	{
	}

	public void InsertExpressionKeyFrame(float normalizedProgressKey, string value, Easing? easingFunction = null)
	{
		KeyFrames.InsertExpressionKeyFrame(normalizedProgressKey, value, easingFunction ?? base.Compositor.DefaultEasing);
	}
}
