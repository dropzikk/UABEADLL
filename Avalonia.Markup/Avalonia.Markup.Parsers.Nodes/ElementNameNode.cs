using System;
using Avalonia.Controls;
using Avalonia.Data.Core;
using Avalonia.Reactive;

namespace Avalonia.Markup.Parsers.Nodes;

internal class ElementNameNode : ExpressionNode
{
	private readonly WeakReference<INameScope> _nameScope;

	private readonly string _name;

	private IDisposable? _subscription;

	public override string Description => "#" + _name;

	public ElementNameNode(INameScope nameScope, string name)
	{
		_nameScope = new WeakReference<INameScope>(nameScope);
		_name = name;
	}

	protected override void StartListeningCore(WeakReference<object?> reference)
	{
		if (_nameScope.TryGetTarget(out INameScope target))
		{
			_subscription = NameScopeLocator.Track(target, _name).Subscribe(base.ValueChanged);
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
