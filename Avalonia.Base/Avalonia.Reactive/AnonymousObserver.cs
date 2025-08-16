using System;
using System.Threading.Tasks;

namespace Avalonia.Reactive;

public class AnonymousObserver<T> : IObserver<T>
{
	private static readonly Action<Exception> ThrowsOnError = delegate(Exception ex)
	{
		throw ex;
	};

	private static readonly Action NoOpCompleted = delegate
	{
	};

	private readonly Action<T> _onNext;

	private readonly Action<Exception> _onError;

	private readonly Action _onCompleted;

	public AnonymousObserver(TaskCompletionSource<T> tcs)
	{
		if (tcs == null)
		{
			throw new ArgumentNullException("tcs");
		}
		_onNext = tcs.SetResult;
		_onError = tcs.SetException;
		_onCompleted = NoOpCompleted;
	}

	public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
	{
		_onNext = onNext ?? throw new ArgumentNullException("onNext");
		_onError = onError ?? throw new ArgumentNullException("onError");
		_onCompleted = onCompleted ?? throw new ArgumentNullException("onCompleted");
	}

	public AnonymousObserver(Action<T> onNext)
		: this(onNext, ThrowsOnError, NoOpCompleted)
	{
	}

	public AnonymousObserver(Action<T> onNext, Action<Exception> onError)
		: this(onNext, onError, NoOpCompleted)
	{
	}

	public AnonymousObserver(Action<T> onNext, Action onCompleted)
		: this(onNext, ThrowsOnError, onCompleted)
	{
	}

	public void OnCompleted()
	{
		_onCompleted();
	}

	public void OnError(Exception error)
	{
		_onError(error);
	}

	public void OnNext(T value)
	{
		_onNext(value);
	}
}
