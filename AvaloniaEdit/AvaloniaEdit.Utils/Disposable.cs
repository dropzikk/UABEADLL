using System;
using System.Threading;

namespace AvaloniaEdit.Utils;

internal sealed class Disposable : IDisposable
{
	private volatile Action _dispose;

	public bool IsDisposed => _dispose == null;

	public Disposable(Action dispose)
	{
		_dispose = dispose;
	}

	public void Dispose()
	{
		Interlocked.Exchange(ref _dispose, null)?.Invoke();
	}
}
