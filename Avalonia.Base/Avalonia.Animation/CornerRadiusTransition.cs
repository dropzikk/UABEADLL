using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public class CornerRadiusTransition : Transition<CornerRadius>
{
	internal override IObservable<CornerRadius> DoTransition(IObservable<double> progress, CornerRadius oldValue, CornerRadius newValue)
	{
		return AnimatorDrivenTransition<CornerRadius, CornerRadiusAnimator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
