using System;
using System.Collections.Generic;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Server;

namespace Avalonia.Rendering.Composition.Animations;

internal abstract class AnimationInstanceBase : IAnimationInstance, IServerClockItem
{
	private List<(ServerObject obj, CompositionProperty member)>? _trackedObjects;

	private bool _invalidated;

	protected PropertySetSnapshot Parameters { get; }

	public ServerObject TargetObject { get; }

	protected CompositionProperty Property { get; private set; }

	public AnimationInstanceBase(ServerObject target, PropertySetSnapshot parameters)
	{
		Parameters = parameters;
		TargetObject = target;
	}

	protected void Initialize(CompositionProperty property, HashSet<(string name, string member)> trackedObjects)
	{
		if (trackedObjects.Count > 0)
		{
			_trackedObjects = new List<(ServerObject, CompositionProperty)>();
			foreach (var trackedObject in trackedObjects)
			{
				if (Parameters.GetObjectParameter(trackedObject.name) is ServerObject serverObject)
				{
					CompositionProperty compositionProperty = serverObject.GetCompositionProperty(trackedObject.member);
					if (compositionProperty != null)
					{
						_trackedObjects.Add((serverObject, compositionProperty));
					}
				}
			}
		}
		Property = property;
	}

	public abstract void Initialize(TimeSpan startedAt, ExpressionVariant startingValue, CompositionProperty property);

	protected abstract ExpressionVariant EvaluateCore(TimeSpan now, ExpressionVariant currentValue);

	public ExpressionVariant Evaluate(TimeSpan now, ExpressionVariant currentValue)
	{
		_invalidated = false;
		return EvaluateCore(now, currentValue);
	}

	public virtual void Activate()
	{
		if (_trackedObjects == null)
		{
			return;
		}
		foreach (var trackedObject in _trackedObjects)
		{
			trackedObject.obj.SubscribeToInvalidation(trackedObject.member, this);
		}
	}

	public virtual void Deactivate()
	{
		if (_trackedObjects == null)
		{
			return;
		}
		foreach (var trackedObject in _trackedObjects)
		{
			trackedObject.obj.UnsubscribeFromInvalidation(trackedObject.member, this);
		}
	}

	public void Invalidate()
	{
		if (!_invalidated)
		{
			_invalidated = true;
			TargetObject.NotifyAnimatedValueChanged(Property);
		}
	}

	public void OnTick()
	{
		Invalidate();
	}
}
