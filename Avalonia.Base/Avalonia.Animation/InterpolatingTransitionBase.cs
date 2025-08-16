using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public abstract class InterpolatingTransitionBase<T> : Transition<T>
{
	private class Animator : Animator<T>
	{
		private readonly InterpolatingTransitionBase<T> _parent;

		public Animator(InterpolatingTransitionBase<T> parent)
		{
			_parent = parent;
		}

		public override T Interpolate(double progress, T oldValue, T newValue)
		{
			return _parent.Interpolate(progress, oldValue, newValue);
		}
	}

	protected abstract T Interpolate(double progress, T from, T to);

	internal override IObservable<T> DoTransition(IObservable<double> progress, T oldValue, T newValue)
	{
		return new AnimatorTransitionObservable<T, Animator>(new Animator(this), progress, base.Easing, oldValue, newValue);
	}
}
