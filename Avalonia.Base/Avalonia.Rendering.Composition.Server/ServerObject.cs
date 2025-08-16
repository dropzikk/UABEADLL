using System;
using System.Collections.Generic;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition.Server;

internal abstract class ServerObject : SimpleServerObject, IExpressionObject
{
	private class ServerObjectSubscriptionStore
	{
		public bool IsValid;

		public RefTrackingDictionary<IAnimationInstance>? Subscribers;

		public void Invalidate()
		{
			if (IsValid)
			{
				return;
			}
			IsValid = false;
			if (Subscribers == null)
			{
				return;
			}
			foreach (KeyValuePair<IAnimationInstance, int> subscriber in Subscribers)
			{
				subscriber.Key.Invalidate();
			}
		}
	}

	private uint _activationCount;

	private InlineDictionary<CompositionProperty, ServerObjectSubscriptionStore> _subscriptions;

	private InlineDictionary<CompositionProperty, IAnimationInstance> _animations;

	public bool IsActive => _activationCount != 0;

	public ServerObject(ServerCompositor compositor)
		: base(compositor)
	{
	}

	public virtual ExpressionVariant GetPropertyForAnimation(string name)
	{
		return default(ExpressionVariant);
	}

	ExpressionVariant IExpressionObject.GetProperty(string name)
	{
		return GetPropertyForAnimation(name);
	}

	public void Activate()
	{
		_activationCount++;
		if (_activationCount == 1)
		{
			Activated();
		}
	}

	public void Deactivate()
	{
		_activationCount--;
		if (_activationCount == 0)
		{
			Deactivated();
		}
	}

	protected void Activated()
	{
		foreach (KeyValuePair<CompositionProperty, IAnimationInstance> animation in _animations)
		{
			animation.Value.Activate();
		}
	}

	protected void Deactivated()
	{
		foreach (KeyValuePair<CompositionProperty, IAnimationInstance> animation in _animations)
		{
			animation.Value.Deactivate();
		}
	}

	private void InvalidateSubscriptions(CompositionProperty property)
	{
		if (_subscriptions.TryGetValue(property, out ServerObjectSubscriptionStore value))
		{
			value.Invalidate();
		}
	}

	protected new void SetValue<T>(CompositionProperty prop, ref T field, T value)
	{
		field = value;
		InvalidateSubscriptions(prop);
	}

	protected new T GetValue<T>(CompositionProperty prop, ref T field)
	{
		if (_subscriptions.TryGetValue(prop, out ServerObjectSubscriptionStore value))
		{
			value.IsValid = true;
		}
		return field;
	}

	protected void SetAnimatedValue<T>(CompositionProperty prop, ref T field, TimeSpan committedAt, IAnimationInstance animation) where T : struct
	{
		if (IsActive && _animations.TryGetValue(prop, out IAnimationInstance value))
		{
			value.Deactivate();
		}
		_animations[prop] = animation;
		animation.Initialize(committedAt, ExpressionVariant.Create(field), prop);
		if (IsActive)
		{
			animation.Activate();
		}
		InvalidateSubscriptions(prop);
	}

	protected void SetAnimatedValue<T>(CompositionProperty property, out T field, T value)
	{
		if (_animations.TryGetAndRemoveValue(property, out IAnimationInstance value2) && IsActive)
		{
			value2.Deactivate();
		}
		field = value;
		InvalidateSubscriptions(property);
	}

	protected T GetAnimatedValue<T>(CompositionProperty property, ref T field) where T : struct
	{
		if (_subscriptions.TryGetValue(property, out ServerObjectSubscriptionStore value))
		{
			value.IsValid = true;
		}
		if (_animations.TryGetValue(property, out IAnimationInstance value2))
		{
			field = value2.Evaluate(base.Compositor.ServerNow, ExpressionVariant.Create(field)).CastOrDefault<T>();
		}
		return field;
	}

	public virtual void NotifyAnimatedValueChanged(CompositionProperty prop)
	{
		InvalidateSubscriptions(prop);
		ValuesInvalidated();
	}

	public void SubscribeToInvalidation(CompositionProperty member, IAnimationInstance animation)
	{
		if (!_subscriptions.TryGetValue(member, out ServerObjectSubscriptionStore value))
		{
			value = (_subscriptions[member] = new ServerObjectSubscriptionStore());
		}
		if (value.Subscribers == null)
		{
			value.Subscribers = new RefTrackingDictionary<IAnimationInstance>();
		}
		value.Subscribers.AddRef(animation);
	}

	public void UnsubscribeFromInvalidation(CompositionProperty member, IAnimationInstance animation)
	{
		if (_subscriptions.TryGetValue(member, out ServerObjectSubscriptionStore value))
		{
			value.Subscribers?.ReleaseRef(animation);
		}
	}

	public virtual CompositionProperty? GetCompositionProperty(string fieldName)
	{
		return null;
	}
}
