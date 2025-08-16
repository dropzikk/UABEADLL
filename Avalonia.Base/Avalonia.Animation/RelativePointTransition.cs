using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public class RelativePointTransition : Transition<RelativePoint>
{
	internal override IObservable<RelativePoint> DoTransition(IObservable<double> progress, RelativePoint oldValue, RelativePoint newValue)
	{
		return AnimatorDrivenTransition<RelativePoint, RelativePointAnimator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
