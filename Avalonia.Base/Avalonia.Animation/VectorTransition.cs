using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public class VectorTransition : Transition<Vector>
{
	internal override IObservable<Vector> DoTransition(IObservable<double> progress, Vector oldValue, Vector newValue)
	{
		return AnimatorDrivenTransition<Vector, VectorAnimator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
