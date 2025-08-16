using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Collections;
using Avalonia.Logging;
using Avalonia.Media;

namespace Avalonia.Animation.Animators;

internal class EffectAnimator : Animator<IEffect?>
{
	private static bool s_Registered;

	public override IDisposable? Apply(Animation animation, Animatable control, IClock? clock, IObservable<bool> match, Action? onComplete)
	{
		if (TryCreateAnimator<BlurEffectAnimator, IBlurEffect>(out IAnimator animator) || TryCreateAnimator<DropShadowEffectAnimator, IDropShadowEffect>(out animator))
		{
			return animator.Apply(animation, control, clock, match, onComplete);
		}
		Logger.TryGet(LogEventLevel.Error, "Animations")?.Log(this, "The animation's keyframe value types set is not supported.");
		return base.Apply(animation, control, clock, match, onComplete);
	}

	private bool TryCreateAnimator<TAnimator, TInterface>([NotNullWhen(true)] out IAnimator? animator) where TAnimator : EffectAnimatorBase<TInterface>, new() where TInterface : class, IEffect
	{
		TAnimator val = null;
		using (AvaloniaList<AnimatorKeyFrame>.Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				AnimatorKeyFrame current = enumerator.Current;
				if (current.Value is TInterface)
				{
					if (val == null)
					{
						val = new TAnimator
						{
							Property = base.Property
						};
					}
					val.Add(new AnimatorKeyFrame(typeof(TAnimator), () => new TAnimator(), current.Cue, current.KeySpline)
					{
						Value = current.Value
					});
					continue;
				}
				animator = null;
				return false;
			}
		}
		animator = val;
		return animator != null;
	}

	public override IEffect? Interpolate(double progress, IEffect? oldValue, IEffect? newValue)
	{
		if (!(progress >= 0.5))
		{
			return oldValue;
		}
		return newValue;
	}

	public static void EnsureRegistered()
	{
		if (!s_Registered)
		{
			s_Registered = true;
		}
	}
}
