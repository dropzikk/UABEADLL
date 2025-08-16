using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

public class SizeTransition : Transition<Size>
{
	internal override IObservable<Size> DoTransition(IObservable<double> progress, Size oldValue, Size newValue)
	{
		return AnimatorDrivenTransition<Size, SizeAnimator>.Transition(base.Easing, progress, oldValue, newValue);
	}
}
