using System;
using System.Runtime.CompilerServices;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition;

public abstract class CompositionObject : ICompositorSerializable
{
	private protected InlineDictionary<CompositionProperty, IAnimationInstance> PendingAnimations;

	private bool _registeredForSerialization;

	public ImplicitAnimationCollection? ImplicitAnimations { get; set; }

	public Compositor Compositor { get; }

	internal SimpleServerObject? Server { get; }

	public bool IsDisposed { get; private set; }

	internal CompositionObject(Compositor compositor, SimpleServerObject? server)
	{
		Compositor = compositor;
		Server = server;
	}

	SimpleServerObject ICompositorSerializable.TryGetServer(Compositor c)
	{
		return Server ?? ThrowInvalidOperation();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static SimpleServerObject ThrowInvalidOperation()
	{
		throw new InvalidOperationException("There is no server-side counterpart for this object");
	}

	protected internal void Dispose()
	{
		if (!IsDisposed && Server != null)
		{
			Compositor.DisposeOnNextBatch(Server);
		}
		IsDisposed = true;
	}

	public void StartAnimation(string propertyName, CompositionAnimation animation)
	{
		StartAnimation(propertyName, animation, null);
	}

	internal virtual void StartAnimation(string propertyName, CompositionAnimation animation, ExpressionVariant? finalValue)
	{
		throw new ArgumentException("Unknown property " + propertyName);
	}

	public void StartAnimationGroup(ICompositionAnimationBase grp)
	{
		if (grp is CompositionAnimation compositionAnimation)
		{
			if (compositionAnimation.Target == null)
			{
				throw new ArgumentException("Animation Target can't be null");
			}
			StartAnimation(compositionAnimation.Target, compositionAnimation);
		}
		else
		{
			if (!(grp is CompositionAnimationGroup compositionAnimationGroup))
			{
				return;
			}
			foreach (CompositionAnimation animation in compositionAnimationGroup.Animations)
			{
				if (animation.Target == null)
				{
					throw new ArgumentException("Animation Target can't be null");
				}
				StartAnimation(animation.Target, animation);
			}
		}
	}

	private bool StartAnimationGroupPart(CompositionAnimation animation, string target, ExpressionVariant finalValue)
	{
		if (animation.Target == null)
		{
			throw new ArgumentException("Animation Target can't be null");
		}
		if (animation.Target == target)
		{
			StartAnimation(animation.Target, animation, finalValue);
			return true;
		}
		StartAnimation(animation.Target, animation);
		return false;
	}

	internal bool StartAnimationGroup(ICompositionAnimationBase grp, string target, ExpressionVariant finalValue)
	{
		if (grp is CompositionAnimation animation)
		{
			return StartAnimationGroupPart(animation, target, finalValue);
		}
		if (grp is CompositionAnimationGroup compositionAnimationGroup)
		{
			bool result = false;
			{
				foreach (CompositionAnimation animation2 in compositionAnimationGroup.Animations)
				{
					if (animation2.Target == null)
					{
						throw new ArgumentException("Animation Target can't be null");
					}
					if (StartAnimationGroupPart(animation2, target, finalValue))
					{
						result = true;
					}
				}
				return result;
			}
		}
		throw new ArgumentException();
	}

	protected void RegisterForSerialization()
	{
		if (Server == null)
		{
			throw new InvalidOperationException("The object doesn't have an associated server counterpart");
		}
		if (!_registeredForSerialization)
		{
			_registeredForSerialization = true;
			Compositor.RegisterForSerialization(this);
		}
	}

	void ICompositorSerializable.SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		_registeredForSerialization = false;
		SerializeChangesCore(writer);
	}

	private protected virtual void SerializeChangesCore(BatchStreamWriter writer)
	{
	}
}
