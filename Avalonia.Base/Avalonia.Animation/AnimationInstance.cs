using System;
using System.Linq;
using Avalonia.Animation.Animators;
using Avalonia.Animation.Easings;
using Avalonia.Data;
using Avalonia.Reactive;

namespace Avalonia.Animation;

internal class AnimationInstance<T> : SingleSubscriberObservableBase<T>
{
	private T _lastInterpValue;

	private T _firstKFValue;

	private ulong? _iterationCount;

	private ulong _currentIteration;

	private bool _gotFirstKFValue;

	private bool _playbackReversed;

	private FillMode _fillMode;

	private PlaybackDirection _playbackDirection;

	private Animator<T> _animator;

	private Animation _animation;

	private Animatable _targetControl;

	private T _neutralValue;

	private double _speedRatioConv;

	private TimeSpan _initialDelay;

	private TimeSpan _iterationDelay;

	private TimeSpan _duration;

	private Easing? _easeFunc;

	private Action? _onCompleteAction;

	private Func<double, T, T> _interpolator;

	private IDisposable? _timerSub;

	private readonly IClock _baseClock;

	private IClock? _clock;

	private EventHandler<AvaloniaPropertyChangedEventArgs>? _propertyChangedDelegate;

	public AnimationInstance(Animation animation, Animatable control, Animator<T> animator, IClock baseClock, Action? OnComplete, Func<double, T, T> Interpolator)
	{
		_animator = animator;
		_animation = animation;
		_targetControl = control;
		_onCompleteAction = OnComplete;
		_interpolator = Interpolator;
		_baseClock = baseClock;
		_lastInterpValue = default(T);
		_firstKFValue = default(T);
		_neutralValue = default(T);
		FetchProperties();
	}

	private void FetchProperties()
	{
		if (_animation.SpeedRatio < 0.0)
		{
			throw new InvalidOperationException("SpeedRatio value should not be negative.");
		}
		if (_animation.Duration < TimeSpan.Zero)
		{
			throw new InvalidOperationException("Duration value cannot be negative.");
		}
		if (_animation.Delay < TimeSpan.Zero)
		{
			throw new InvalidOperationException("Delay value cannot be negative.");
		}
		_easeFunc = _animation.Easing;
		_speedRatioConv = 1.0 / _animation.SpeedRatio;
		_initialDelay = _animation.Delay;
		_duration = _animation.Duration;
		_iterationDelay = _animation.DelayBetweenIterations;
		if (_animation.IterationCount.RepeatType == IterationType.Many)
		{
			_iterationCount = _animation.IterationCount.Value;
		}
		else
		{
			_iterationCount = null;
		}
		_playbackDirection = _animation.PlaybackDirection;
		_fillMode = _animation.FillMode;
	}

	protected override void Unsubscribed()
	{
		ApplyFinalFill();
		_targetControl.PropertyChanged -= _propertyChangedDelegate;
		_timerSub?.Dispose();
		_clock.PlayState = PlayState.Stop;
	}

	protected override void Subscribed()
	{
		_clock = new Clock(_baseClock);
		_timerSub = _clock.Subscribe(Step);
		if (_propertyChangedDelegate == null)
		{
			_propertyChangedDelegate = ControlPropertyChanged;
		}
		_targetControl.PropertyChanged += _propertyChangedDelegate;
		UpdateNeutralValue();
	}

	public void Step(TimeSpan frameTick)
	{
		try
		{
			InternalStep(frameTick);
		}
		catch (Exception error)
		{
			PublishError(error);
		}
	}

	private void ApplyFinalFill()
	{
		if ((object)_animator.Property == null)
		{
			throw new InvalidOperationException("Animator has no property specified.");
		}
		FillMode fillMode = _fillMode;
		if (fillMode == FillMode.Forward || fillMode == FillMode.Both)
		{
			_targetControl.SetValue(_animator.Property, _lastInterpValue);
		}
	}

	private void DoComplete()
	{
		ApplyFinalFill();
		_onCompleteAction?.Invoke();
		PublishCompleted();
	}

	private void DoDelay()
	{
		FillMode fillMode = _fillMode;
		if (fillMode == FillMode.Backward || fillMode == FillMode.Both)
		{
			PublishNext((_currentIteration == 0L) ? _firstKFValue : _lastInterpValue);
		}
	}

	private void DoPlayStates()
	{
		if (_clock.PlayState == PlayState.Stop || _baseClock.PlayState == PlayState.Stop)
		{
			DoComplete();
		}
		if (!_gotFirstKFValue)
		{
			_firstKFValue = (T)_animator.First().Value;
			_gotFirstKFValue = true;
		}
	}

	private void InternalStep(TimeSpan time)
	{
		DoPlayStates();
		FetchProperties();
		long ticks = time.Ticks;
		double num = (double)_duration.Ticks * _speedRatioConv;
		double num2 = (double)_iterationDelay.Ticks * _speedRatioConv;
		double num3 = (double)_initialDelay.Ticks * _speedRatioConv;
		if (num3 > 0.0 && (double)ticks <= num3)
		{
			DoDelay();
			return;
		}
		double num4 = num + num2;
		double num5 = (double)ticks - num3;
		double num6 = num5 % num4;
		_currentIteration = (ulong)(num5 / num4);
		if (_currentIteration + 1 > _iterationCount || _duration == TimeSpan.Zero)
		{
			double arg = _easeFunc.Ease(_playbackReversed ? 0.0 : 1.0);
			_lastInterpValue = _interpolator(arg, _neutralValue);
			DoComplete();
		}
		if (num6 <= num)
		{
			double num7 = num6 / num;
			switch (_playbackDirection)
			{
			case PlaybackDirection.Normal:
				_playbackReversed = false;
				break;
			case PlaybackDirection.Reverse:
				_playbackReversed = true;
				break;
			case PlaybackDirection.Alternate:
				_playbackReversed = _currentIteration % 2 != 0;
				break;
			case PlaybackDirection.AlternateReverse:
				_playbackReversed = _currentIteration % 2 == 0;
				break;
			default:
				throw new InvalidOperationException($"Animation direction value is unknown: {_playbackDirection}");
			}
			if (_playbackReversed)
			{
				num7 = 1.0 - num7;
			}
			double arg2 = _easeFunc.Ease(num7);
			_lastInterpValue = _interpolator(arg2, _neutralValue);
			PublishNext(_lastInterpValue);
		}
		else if (num6 > num && num6 <= num4 && num2 > 0.0)
		{
			if (_currentIteration + 1 < _iterationCount)
			{
				DoDelay();
			}
			else
			{
				DoComplete();
			}
		}
	}

	private void UpdateNeutralValue()
	{
		AvaloniaProperty property = _animator.Property ?? throw new InvalidOperationException("Animator has no property specified.");
		object baseValue = _targetControl.GetBaseValue(property);
		_neutralValue = ((baseValue != AvaloniaProperty.UnsetValue) ? ((T)baseValue) : ((T)_targetControl.GetValue(property)));
	}

	private void ControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == _animator.Property && e.Priority > BindingPriority.Animation)
		{
			UpdateNeutralValue();
		}
	}
}
