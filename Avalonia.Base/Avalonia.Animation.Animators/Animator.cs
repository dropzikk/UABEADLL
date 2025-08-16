using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Data;
using Avalonia.Reactive;

namespace Avalonia.Animation.Animators;

internal abstract class Animator<T> : AvaloniaList<AnimatorKeyFrame>, IAnimator, IList<AnimatorKeyFrame>, ICollection<AnimatorKeyFrame>, IEnumerable<AnimatorKeyFrame>, IEnumerable
{
	private readonly List<AnimatorKeyFrame> _convertedKeyframes = new List<AnimatorKeyFrame>();

	private bool _isVerifiedAndConverted;

	public AvaloniaProperty? Property { get; set; }

	public Animator()
	{
		base.CollectionChanged += delegate
		{
			_isVerifiedAndConverted = false;
		};
	}

	public virtual IDisposable? Apply(Animation animation, Animatable control, IClock? clock, IObservable<bool> match, Action? onComplete)
	{
		if (!_isVerifiedAndConverted)
		{
			VerifyConvertKeyFrames();
		}
		DisposeAnimationInstanceSubject<T> disposeAnimationInstanceSubject = new DisposeAnimationInstanceSubject<T>(this, animation, control, clock, onComplete);
		return new CompositeDisposable(match.Subscribe(disposeAnimationInstanceSubject), disposeAnimationInstanceSubject);
	}

	protected T InterpolationHandler(double animationTime, T neutralValue)
	{
		AnimatorKeyFrame animatorKeyFrame;
		AnimatorKeyFrame animatorKeyFrame2;
		if (_convertedKeyframes.Count > 2)
		{
			if (animationTime <= 0.0)
			{
				animatorKeyFrame = _convertedKeyframes[0];
				animatorKeyFrame2 = _convertedKeyframes[1];
			}
			else if (animationTime >= 1.0)
			{
				animatorKeyFrame = _convertedKeyframes[_convertedKeyframes.Count - 2];
				animatorKeyFrame2 = _convertedKeyframes[_convertedKeyframes.Count - 1];
			}
			else
			{
				int num = FindClosestBeforeKeyFrame(animationTime);
				animatorKeyFrame = _convertedKeyframes[num];
				animatorKeyFrame2 = _convertedKeyframes[num + 1];
			}
		}
		else
		{
			animatorKeyFrame = _convertedKeyframes[0];
			animatorKeyFrame2 = _convertedKeyframes[1];
		}
		double cueValue = animatorKeyFrame.Cue.CueValue;
		double cueValue2 = animatorKeyFrame2.Cue.CueValue;
		double num2 = (animationTime - cueValue) / (cueValue2 - cueValue);
		T oldValue = (T)((animatorKeyFrame.isNeutral || !(animatorKeyFrame.Value is T val)) ? ((object)neutralValue) : ((object)val));
		T newValue = (T)((animatorKeyFrame2.isNeutral || !(animatorKeyFrame2.Value is T val2)) ? ((object)neutralValue) : ((object)val2));
		if (animatorKeyFrame2.KeySpline != null)
		{
			num2 = animatorKeyFrame2.KeySpline.GetSplineProgress(num2);
		}
		return Interpolate(num2, oldValue, newValue);
	}

	private int FindClosestBeforeKeyFrame(double time)
	{
		for (int i = 0; i < _convertedKeyframes.Count; i++)
		{
			if (_convertedKeyframes[i].Cue.CueValue > time)
			{
				return i - 1;
			}
		}
		throw new Exception("Index time is out of keyframe time range.");
	}

	public virtual IDisposable BindAnimation(Animatable control, IObservable<T> instance)
	{
		if ((object)Property == null)
		{
			throw new InvalidOperationException("Animator has no property specified.");
		}
		return control.Bind((AvaloniaProperty<T>)Property, instance, BindingPriority.Animation);
	}

	internal IDisposable Run(Animation animation, Animatable control, IClock? clock, Action? onComplete)
	{
		AnimationInstance<T> instance = new AnimationInstance<T>(animation, control, this, clock ?? control.Clock ?? Clock.GlobalClock, onComplete, InterpolationHandler);
		return BindAnimation(control, instance);
	}

	public abstract T Interpolate(double progress, T oldValue, T newValue);

	private void VerifyConvertKeyFrames()
	{
		using (AvaloniaList<AnimatorKeyFrame>.Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				AnimatorKeyFrame current = enumerator.Current;
				_convertedKeyframes.Add(current);
			}
		}
		AddNeutralKeyFramesIfNeeded();
		_isVerifiedAndConverted = true;
	}

	private void AddNeutralKeyFramesIfNeeded()
	{
		bool flag;
		bool flag2 = (flag = false);
		foreach (AnimatorKeyFrame convertedKeyframe in _convertedKeyframes)
		{
			if (convertedKeyframe.Cue.CueValue == 0.0)
			{
				flag2 = true;
			}
			else if (convertedKeyframe.Cue.CueValue == 1.0)
			{
				flag = true;
			}
		}
		if (!flag2 || !flag)
		{
			AddNeutralKeyFrames(flag2, flag);
		}
	}

	private void AddNeutralKeyFrames(bool hasStartKey, bool hasEndKey)
	{
		if (!hasStartKey)
		{
			_convertedKeyframes.Insert(0, new AnimatorKeyFrame(null, null, new Cue(0.0))
			{
				Value = default(T),
				isNeutral = true
			});
		}
		if (!hasEndKey)
		{
			_convertedKeyframes.Add(new AnimatorKeyFrame(null, null, new Cue(1.0))
			{
				Value = default(T),
				isNeutral = true
			});
		}
	}
}
