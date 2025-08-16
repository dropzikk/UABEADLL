using System;
using Avalonia.Animation.Animators;
using Avalonia.Animation.Easings;

namespace Avalonia.Animation;

internal static class AnimatorDrivenTransition<T, TAnimator> where TAnimator : Animator<T>, new()
{
	private static readonly TAnimator s_animator = new TAnimator();

	public static IObservable<T> Transition(IEasing easing, IObservable<double> progress, T oldValue, T newValue)
	{
		return new AnimatorTransitionObservable<T, TAnimator>(s_animator, progress, easing, oldValue, newValue);
	}
}
