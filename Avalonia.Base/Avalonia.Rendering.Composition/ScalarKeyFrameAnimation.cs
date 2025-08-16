using Avalonia.Animation.Easings;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Server;

namespace Avalonia.Rendering.Composition;

public class ScalarKeyFrameAnimation : KeyFrameAnimation
{
	private KeyFrames<float> _keyFrames = new KeyFrames<float>();

	private protected override IKeyFrames KeyFrames => _keyFrames;

	public ScalarKeyFrameAnimation(Compositor compositor)
		: base(compositor)
	{
	}

	internal override IAnimationInstance CreateInstance(ServerObject targetObject, ExpressionVariant? finalValue)
	{
		return new KeyFrameAnimationInstance<float>(ScalarInterpolator.Instance, _keyFrames.Snapshot(), CreateSnapshot(), finalValue?.CastOrDefault<float>(), targetObject, base.DelayBehavior, base.DelayTime, base.Direction, base.Duration, base.IterationBehavior, base.IterationCount, base.StopBehavior);
	}

	public void InsertKeyFrame(float normalizedProgressKey, float value, IEasing easingFunction)
	{
		_keyFrames.Insert(normalizedProgressKey, value, easingFunction);
	}

	public void InsertKeyFrame(float normalizedProgressKey, float value)
	{
		_keyFrames.Insert(normalizedProgressKey, value, base.Compositor.DefaultEasing);
	}
}
