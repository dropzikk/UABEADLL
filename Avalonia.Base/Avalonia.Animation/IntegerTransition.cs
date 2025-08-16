using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public class IntegerTransition : Transition<int>
{
	internal override IObservable<int> DoTransition(IObservable<double> progress, int oldValue, int newValue)
	{
		return AnimatorDrivenTransition<int, Int32Animator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
