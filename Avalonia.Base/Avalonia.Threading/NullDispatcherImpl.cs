using System;

namespace Avalonia.Threading;

internal class NullDispatcherImpl : IDispatcherImpl
{
	public bool CurrentThreadIsLoopThread => true;

	public long Now => 0L;

	public event Action? Signaled;

	public event Action? Timer;

	public void Signal()
	{
	}

	public void UpdateTimer(long? dueTimeInMs)
	{
	}
}
