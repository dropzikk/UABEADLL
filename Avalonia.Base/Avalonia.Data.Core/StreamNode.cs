using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data.Core.Plugins;
using Avalonia.Reactive;

namespace Avalonia.Data.Core;

[RequiresUnreferencedCode("ExpressionNode might require unreferenced code.")]
internal class StreamNode : ExpressionNode
{
	private IStreamPlugin? _customPlugin;

	private IDisposable? _subscription;

	public override string Description => "^";

	public StreamNode()
	{
	}

	public StreamNode(IStreamPlugin customPlugin)
	{
		_customPlugin = customPlugin;
	}

	protected override void StartListeningCore(WeakReference<object?> reference)
	{
		_subscription = GetPlugin(reference)?.Start(reference).Subscribe(base.ValueChanged);
	}

	protected override void StopListeningCore()
	{
		_subscription?.Dispose();
		_subscription = null;
	}

	private IStreamPlugin? GetPlugin(WeakReference<object?> reference)
	{
		if (_customPlugin != null)
		{
			return _customPlugin;
		}
		foreach (IStreamPlugin streamHandler in ExpressionObserver.StreamHandlers)
		{
			if (streamHandler.Match(reference))
			{
				return streamHandler;
			}
		}
		ValueChanged(new BindingNotification(new MarkupBindingChainException("Stream operator applied to unsupported type", Description), BindingErrorType.Error));
		return null;
	}
}
