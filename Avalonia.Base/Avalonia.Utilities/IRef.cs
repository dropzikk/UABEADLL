using System;

namespace Avalonia.Utilities;

internal interface IRef<out T> : IDisposable where T : class
{
	T Item { get; }

	int RefCount { get; }

	IRef<T> Clone();

	IRef<TResult> CloneAs<TResult>() where TResult : class;
}
