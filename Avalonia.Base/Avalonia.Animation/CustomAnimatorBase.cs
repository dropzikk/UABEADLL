using System;
using Avalonia.Animation.Animators;

namespace Avalonia.Animation;

[Obsolete("This class will be removed before 11.0, use InterpolatingAnimator<T>", true)]
public abstract class CustomAnimatorBase
{
	internal abstract Type WrapperType { get; }

	internal abstract IAnimator CreateWrapper();
}
[Obsolete("This class will be removed before 11.0, use InterpolatingAnimator<T>", true)]
public abstract class CustomAnimatorBase<T> : CustomAnimatorBase
{
	internal class AnimatorWrapper : Animator<T>
	{
		private readonly CustomAnimatorBase<T> _parent;

		public AnimatorWrapper(CustomAnimatorBase<T> parent)
		{
			_parent = parent;
		}

		public override T Interpolate(double progress, T oldValue, T newValue)
		{
			return _parent.Interpolate(progress, oldValue, newValue);
		}
	}

	internal override Type WrapperType => typeof(AnimatorWrapper);

	public abstract T Interpolate(double progress, T oldValue, T newValue);

	internal override IAnimator CreateWrapper()
	{
		return new AnimatorWrapper(this);
	}
}
