using System;
using System.Threading;

namespace Avalonia.Reactive;

internal static class Disposable
{
	private sealed class EmptyDisposable : IDisposable
	{
		public static readonly EmptyDisposable Instance = new EmptyDisposable();

		private EmptyDisposable()
		{
		}

		public void Dispose()
		{
		}
	}

	internal sealed class AnonymousDisposable : IDisposable
	{
		private volatile Action? _dispose;

		public bool IsDisposed => _dispose == null;

		public AnonymousDisposable(Action dispose)
		{
			_dispose = dispose;
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref _dispose, null)?.Invoke();
		}
	}

	internal sealed class AnonymousDisposable<TState> : IDisposable
	{
		private TState _state;

		private volatile Action<TState>? _dispose;

		public bool IsDisposed => _dispose == null;

		public AnonymousDisposable(TState state, Action<TState> dispose)
		{
			_state = state;
			_dispose = dispose;
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref _dispose, null)?.Invoke(_state);
			_state = default(TState);
		}
	}

	public static IDisposable Empty => EmptyDisposable.Instance;

	public static IDisposable Create(Action dispose)
	{
		if (dispose == null)
		{
			throw new ArgumentNullException("dispose");
		}
		return new AnonymousDisposable(dispose);
	}

	public static IDisposable Create<TState>(TState state, Action<TState> dispose)
	{
		if (dispose == null)
		{
			throw new ArgumentNullException("dispose");
		}
		return new AnonymousDisposable<TState>(state, dispose);
	}
}
