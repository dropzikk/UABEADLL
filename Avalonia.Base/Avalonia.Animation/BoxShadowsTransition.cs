using System;
using Avalonia.Animation.Animators;
using Avalonia.Media;

namespace Avalonia.Animation;

public class BoxShadowsTransition : Transition<BoxShadows>
{
	internal override IObservable<BoxShadows> DoTransition(IObservable<double> progress, BoxShadows oldValue, BoxShadows newValue)
	{
		return AnimatorDrivenTransition<BoxShadows, BoxShadowsAnimator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
