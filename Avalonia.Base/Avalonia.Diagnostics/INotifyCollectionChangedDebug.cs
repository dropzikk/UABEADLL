using System;

namespace Avalonia.Diagnostics;

internal interface INotifyCollectionChangedDebug
{
	Delegate[]? GetCollectionChangedSubscribers();
}
