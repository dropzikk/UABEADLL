using System;

namespace Avalonia.Data.Core.Plugins;

public class PropertyError : IPropertyAccessor, IDisposable
{
	private BindingNotification _error;

	public Type? PropertyType => null;

	public object? Value => _error;

	public PropertyError(BindingNotification error)
	{
		_error = error;
	}

	public void Dispose()
	{
	}

	public bool SetValue(object? value, BindingPriority priority)
	{
		return false;
	}

	public void Subscribe(Action<object> listener)
	{
		listener(_error);
	}

	public void Unsubscribe()
	{
	}
}
