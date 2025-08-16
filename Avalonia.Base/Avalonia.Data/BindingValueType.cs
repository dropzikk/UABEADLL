using System;

namespace Avalonia.Data;

[Flags]
public enum BindingValueType
{
	UnsetValue = 0,
	DoNothing = 1,
	Value = 0x102,
	BindingError = 0x203,
	DataValidationError = 0x204,
	BindingErrorWithFallback = 0x303,
	DataValidationErrorWithFallback = 0x304,
	TypeMask = 0xFF,
	HasValue = 0x100,
	HasError = 0x200
}
