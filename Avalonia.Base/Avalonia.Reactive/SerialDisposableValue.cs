using System;

namespace Avalonia.Reactive;

internal sealed class SerialDisposableValue : IDisposable
{
	private IDisposable? _current;

	private bool _disposed;

	public IDisposable? Disposable
	{
		get
		{
			return _current;
		}
		set
		{
			_current?.Dispose();
			_current = value;
			if (_disposed)
			{
				_current?.Dispose();
				_current = null;
			}
		}
	}

	public void Dispose()
	{
		_disposed = true;
		_current?.Dispose();
	}
}
