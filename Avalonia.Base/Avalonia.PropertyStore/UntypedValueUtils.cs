using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data;
using Avalonia.Utilities;

namespace Avalonia.PropertyStore;

internal static class UntypedValueUtils
{
	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Implicit conversion methods might be removed by the linker. We don't have a reliable way to prevent it, except converting everything in compile time when possible.")]
	public static BindingValue<T> ConvertAndValidate<T>(object? value, Type targetType, Func<T, bool>? validate)
	{
		BindingValue<T> result = BindingValue<T>.FromUntyped(value, targetType);
		if (result.HasValue && validate != null && !validate(result.Value))
		{
			return BindingValue<T>.BindingError(new InvalidCastException($"'{result.Value}' is not a valid value."));
		}
		return result;
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Implicit conversion methods might be removed by the linker. We don't have a reliable way to prevent it, except converting everything in compile time when possible.")]
	public static bool TryConvertAndValidate<T>(StyledProperty<T> property, object? value, [MaybeNullWhen(false)] out T result)
	{
		if (TypeUtilities.TryConvertImplicit(typeof(T), value, out object result2))
		{
			result = (T)result2;
			Func<T, bool>? validateValue = property.ValidateValue;
			if (validateValue == null || validateValue(result))
			{
				return true;
			}
		}
		result = default(T);
		return false;
	}
}
