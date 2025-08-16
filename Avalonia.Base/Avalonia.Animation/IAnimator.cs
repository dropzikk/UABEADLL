using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Animation;

internal interface IAnimator : IList<AnimatorKeyFrame>, ICollection<AnimatorKeyFrame>, IEnumerable<AnimatorKeyFrame>, IEnumerable
{
	AvaloniaProperty? Property { get; set; }

	IDisposable? Apply(Animation animation, Animatable control, IClock? clock, IObservable<bool> match, Action? onComplete);
}
