using System;
using Avalonia.Data.Core;
using Avalonia.LogicalTree;
using Avalonia.Reactive;

namespace Avalonia.Markup.Parsers.Nodes;

internal class FindAncestorNode : ExpressionNode
{
	private readonly int _level;

	private readonly Type? _ancestorType;

	private IDisposable? _subscription;

	public override string Description
	{
		get
		{
			if (!(_ancestorType == null))
			{
				return $"$parent[{_ancestorType.Name}, {_level}]";
			}
			return $"$parent[{_level}]";
		}
	}

	public FindAncestorNode(Type? ancestorType, int level)
	{
		_level = level;
		_ancestorType = ancestorType;
	}

	protected override void StartListeningCore(WeakReference<object?> reference)
	{
		if (reference.TryGetTarget(out object target) && target is ILogical relativeTo)
		{
			_subscription = ControlLocator.Track(relativeTo, _level, _ancestorType).Subscribe(base.ValueChanged);
		}
		else
		{
			_subscription = null;
		}
	}

	protected override void StopListeningCore()
	{
		_subscription?.Dispose();
		_subscription = null;
	}
}
