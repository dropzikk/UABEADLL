using System.Numerics;
using Avalonia.Animation.Easings;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Server;

namespace Avalonia.Rendering.Composition;

public class Vector2KeyFrameAnimation : KeyFrameAnimation
{
	private KeyFrames<Vector2> _keyFrames = new KeyFrames<Vector2>();

	private protected override IKeyFrames KeyFrames => _keyFrames;

	public Vector2KeyFrameAnimation(Compositor compositor)
		: base(compositor)
	{
	}

	internal override IAnimationInstance CreateInstance(ServerObject targetObject, ExpressionVariant? finalValue)
	{
		return new KeyFrameAnimationInstance<Vector2>(Vector2Interpolator.Instance, _keyFrames.Snapshot(), CreateSnapshot(), finalValue?.CastOrDefault<Vector2>(), targetObject, base.DelayBehavior, base.DelayTime, base.Direction, base.Duration, base.IterationBehavior, base.IterationCount, base.StopBehavior);
	}

	public void InsertKeyFrame(float normalizedProgressKey, Vector2 value, IEasing easingFunction)
	{
		_keyFrames.Insert(normalizedProgressKey, value, easingFunction);
	}

	public void InsertKeyFrame(float normalizedProgressKey, Vector2 value)
	{
		_keyFrames.Insert(normalizedProgressKey, value, base.Compositor.DefaultEasing);
	}
}
