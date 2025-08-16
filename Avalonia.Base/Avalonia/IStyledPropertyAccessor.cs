using System;

namespace Avalonia;

internal interface IStyledPropertyAccessor
{
	object? GetDefaultValue(Type type);

	bool ValidateValue(object? value);
}
