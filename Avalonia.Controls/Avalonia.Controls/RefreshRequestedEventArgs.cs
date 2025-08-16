using System;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class RefreshRequestedEventArgs : RoutedEventArgs
{
	private RefreshCompletionDeferral _refreshCompletionDeferral;

	public RefreshCompletionDeferral GetDeferral()
	{
		return _refreshCompletionDeferral.Get();
	}

	public RefreshRequestedEventArgs(Action deferredAction, RoutedEvent? routedEvent)
		: base(routedEvent)
	{
		_refreshCompletionDeferral = new RefreshCompletionDeferral(deferredAction);
	}

	public RefreshRequestedEventArgs(RefreshCompletionDeferral completionDeferral, RoutedEvent? routedEvent)
		: base(routedEvent)
	{
		_refreshCompletionDeferral = completionDeferral;
	}

	internal void IncrementCount()
	{
		_refreshCompletionDeferral?.Get();
	}

	internal void DecrementCount()
	{
		_refreshCompletionDeferral?.Complete();
	}
}
