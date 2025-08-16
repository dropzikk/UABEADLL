using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public class PointTransition : Transition<Point>
{
	internal override IObservable<Point> DoTransition(IObservable<double> progress, Point oldValue, Point newValue)
	{
		return AnimatorDrivenTransition<Point, PointAnimator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
