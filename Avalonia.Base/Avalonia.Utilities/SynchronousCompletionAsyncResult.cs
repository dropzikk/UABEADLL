using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Utilities;

public record struct SynchronousCompletionAsyncResult<T>(SynchronousCompletionAsyncResultSource<T> source) : INotifyCompletion
{
	public bool IsCompleted
	{
		get
		{
			if (!_isValid)
			{
				ThrowNotInitialized();
			}
			if (_source != null)
			{
				return _source.IsCompleted;
			}
			return true;
		}
	}

	private readonly SynchronousCompletionAsyncResultSource<T>? _source;

	private readonly T? _result;

	private readonly bool _isValid;

	internal SynchronousCompletionAsyncResult(SynchronousCompletionAsyncResultSource<T> source)
	{
		_source = source;
		_result = default(T);
		_isValid = true;
	}

	public SynchronousCompletionAsyncResult(T result)
	{
		_result = result;
		_source = null;
		_isValid = true;
	}

	private static void ThrowNotInitialized()
	{
		throw new InvalidOperationException("This SynchronousCompletionAsyncResult was not initialized");
	}

	public T GetResult()
	{
		if (!_isValid)
		{
			ThrowNotInitialized();
		}
		if (_source != null)
		{
			return _source.Result;
		}
		return _result;
	}

	public void OnCompleted(Action continuation)
	{
		if (!_isValid)
		{
			ThrowNotInitialized();
		}
		if (_source == null)
		{
			continuation();
		}
		else
		{
			_source.OnCompleted(continuation);
		}
	}

	public SynchronousCompletionAsyncResult<T> GetAwaiter()
	{
		return this;
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("IsCompleted = ");
		builder.Append(IsCompleted.ToString());
		return true;
	}
}
