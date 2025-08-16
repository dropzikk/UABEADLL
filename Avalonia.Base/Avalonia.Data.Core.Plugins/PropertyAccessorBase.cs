using System;

namespace Avalonia.Data.Core.Plugins;

public abstract class PropertyAccessorBase : IPropertyAccessor, IDisposable
{
	private Action<object?>? _listener;

	public abstract Type? PropertyType { get; }

	public abstract object? Value { get; }

	public void Dispose()
	{
		if (_listener != null)
		{
			Unsubscribe();
		}
	}

	public abstract bool SetValue(object? value, BindingPriority priority);

	public void Subscribe(Action<object?> listener)
	{
		if (listener == null)
		{
			throw new ArgumentNullException("listener");
		}
		if (_listener != null)
		{
			throw new InvalidOperationException("A member accessor can be subscribed to only once.");
		}
		_listener = listener;
		SubscribeCore();
	}

	public void Unsubscribe()
	{
		if (_listener == null)
		{
			throw new InvalidOperationException("The member accessor was not subscribed.");
		}
		UnsubscribeCore();
		_listener = null;
	}

	protected void PublishValue(object? value)
	{
		_listener?.Invoke(value);
	}

	protected abstract void SubscribeCore();

	protected abstract void UnsubscribeCore();
}
