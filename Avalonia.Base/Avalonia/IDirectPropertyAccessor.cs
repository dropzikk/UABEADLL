using System;

namespace Avalonia;

internal interface IDirectPropertyAccessor
{
	bool IsReadOnly { get; }

	Type Owner { get; }

	object? GetValue(AvaloniaObject instance);

	void SetValue(AvaloniaObject instance, object? value);
}
