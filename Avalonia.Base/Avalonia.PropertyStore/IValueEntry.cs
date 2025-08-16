using System;
using Avalonia.Data;

namespace Avalonia.PropertyStore;

internal interface IValueEntry
{
	bool HasValue { get; }

	AvaloniaProperty Property { get; }

	object? GetValue();

	bool GetDataValidationState(out BindingValueType state, out Exception? error);

	void Unsubscribe();
}
internal interface IValueEntry<T> : IValueEntry
{
	new T GetValue();
}
