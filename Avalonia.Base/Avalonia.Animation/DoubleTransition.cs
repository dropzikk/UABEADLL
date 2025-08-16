using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public class DoubleTransition : Transition<double>
{
	internal override IObservable<double> DoTransition(IObservable<double> progress, double oldValue, double newValue)
	{
		return AnimatorDrivenTransition<double, DoubleAnimator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
