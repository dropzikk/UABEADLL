using System;
using Avalonia.Data;

namespace Avalonia.PropertyStore;

internal sealed class TypedBindingEntry<T> : BindingEntryBase<T, T>
{
	public new StyledProperty<T> Property => (StyledProperty<T>)base.Property;

	public TypedBindingEntry(AvaloniaObject target, ValueFrame frame, StyledProperty<T> property, IObservable<T> source)
		: base(target, frame, (AvaloniaProperty)property, source)
	{
	}

	public TypedBindingEntry(AvaloniaObject target, ValueFrame frame, StyledProperty<T> property, IObservable<BindingValue<T>> source)
		: base(target, frame, (AvaloniaProperty)property, source)
	{
	}

	protected override BindingValue<T> ConvertAndValidate(T value)
	{
		Func<T, bool>? validateValue = Property.ValidateValue;
		if (validateValue != null && !validateValue(value))
		{
			return BindingValue<T>.BindingError(new InvalidCastException($"'{value}' is not a valid value."));
		}
		return value;
	}

	protected override BindingValue<T> ConvertAndValidate(BindingValue<T> value)
	{
		if (value.HasValue)
		{
			Func<T, bool>? validateValue = Property.ValidateValue;
			if (validateValue != null && !validateValue(value.Value))
			{
				return BindingValue<T>.BindingError(new InvalidCastException($"'{value.Value}' is not a valid value."));
			}
		}
		return value;
	}

	protected override T GetDefaultValue(Type ownerType)
	{
		return Property.GetDefaultValue(ownerType);
	}
}
