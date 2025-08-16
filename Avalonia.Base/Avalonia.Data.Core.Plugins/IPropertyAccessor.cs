using System;

namespace Avalonia.Data.Core.Plugins;

public interface IPropertyAccessor : IDisposable
{
	Type? PropertyType { get; }

	object? Value { get; }

	bool SetValue(object? value, BindingPriority priority);

	void Subscribe(Action<object?> listener);

	void Unsubscribe();
}
