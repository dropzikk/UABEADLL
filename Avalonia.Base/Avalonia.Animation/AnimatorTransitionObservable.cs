using System;
using Avalonia.Animation.Animators;
using Avalonia.Animation.Easings;

namespace Avalonia.Animation;

internal class AnimatorTransitionObservable<T, TAnimator> : TransitionObservableBase<T> where TAnimator : Animator<T>
{
	private readonly TAnimator _animator;

	private readonly T _oldValue;

	private readonly T _newValue;

	public AnimatorTransitionObservable(TAnimator animator, IObservable<double> progress, IEasing easing, T oldValue, T newValue)
		: base(progress, easing)
	{
		_animator = animator;
		_oldValue = oldValue;
		_newValue = newValue;
	}

	protected override T ProduceValue(double progress)
	{
		return _animator.Interpolate(progress, _oldValue, _newValue);
	}
}
