using System;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace Avalonia.Animation.Animators;

internal class TransformOperationsAnimator : Animator<TransformOperations>
{
	public TransformOperationsAnimator()
	{
		base.Validate = ValidateTransform;
	}

	public override TransformOperations Interpolate(double progress, TransformOperations oldValue, TransformOperations newValue)
	{
		TransformOperations from = EnsureOperations(oldValue);
		TransformOperations to = EnsureOperations(newValue);
		return TransformOperations.Interpolate(from, to, progress);
	}

	internal static TransformOperations EnsureOperations(ITransform value)
	{
		return (value as TransformOperations) ?? TransformOperations.Identity;
	}

	private void ValidateTransform(AnimatorKeyFrame kf)
	{
		if (!(kf.Value is TransformOperations))
		{
			throw new InvalidOperationException($"All keyframes must be of type {typeof(TransformOperations)}.");
		}
	}
}
