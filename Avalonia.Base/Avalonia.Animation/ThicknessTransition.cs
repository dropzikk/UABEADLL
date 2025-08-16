using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public class ThicknessTransition : Transition<Thickness>
{
	internal override IObservable<Thickness> DoTransition(IObservable<double> progress, Thickness oldValue, Thickness newValue)
	{
		return AnimatorDrivenTransition<Thickness, ThicknessAnimator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
