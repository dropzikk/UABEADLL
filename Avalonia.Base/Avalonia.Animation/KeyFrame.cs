using System;
using Avalonia.Collections;
using Avalonia.Metadata;

namespace Avalonia.Animation;

public sealed class KeyFrame : AvaloniaObject
{
	private TimeSpan _ktimeSpan;

	private Cue _kCue;

	private KeySpline? _kKeySpline;

	[Content]
	public AvaloniaList<IAnimationSetter> Setters { get; } = new AvaloniaList<IAnimationSetter>();

	internal KeyFrameTimingMode TimingMode { get; private set; }

	public TimeSpan KeyTime
	{
		get
		{
			return _ktimeSpan;
		}
		set
		{
			if (TimingMode == KeyFrameTimingMode.Cue)
			{
				throw new InvalidOperationException("You can only set either KeyTime or Cue.");
			}
			TimingMode = KeyFrameTimingMode.TimeSpan;
			_ktimeSpan = value;
		}
	}

	public Cue Cue
	{
		get
		{
			return _kCue;
		}
		set
		{
			if (TimingMode == KeyFrameTimingMode.TimeSpan)
			{
				throw new InvalidOperationException("You can only set either KeyTime or Cue.");
			}
			TimingMode = KeyFrameTimingMode.Cue;
			_kCue = value;
		}
	}

	public KeySpline? KeySpline
	{
		get
		{
			return _kKeySpline;
		}
		set
		{
			_kKeySpline = value;
			if (value != null && !value.IsValid())
			{
				throw new ArgumentException("KeySpline must have X coordinates >= 0.0 and <= 1.0.");
			}
		}
	}
}
