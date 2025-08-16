using System;
using System.Threading;

namespace AvaloniaEdit.Utils;

internal sealed class CallbackOnDispose : IDisposable
{
	private Action _action;

	public CallbackOnDispose(Action action)
	{
		_action = action ?? throw new ArgumentNullException("action");
	}

	public void Dispose()
	{
		Interlocked.Exchange(ref _action, null)?.Invoke();
	}
}
