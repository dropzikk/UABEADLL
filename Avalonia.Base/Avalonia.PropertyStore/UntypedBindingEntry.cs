using System;
using Avalonia.Data;

namespace Avalonia.PropertyStore;

internal class UntypedBindingEntry : BindingEntryBase<object?, object?>
{
	private readonly Func<object?, bool>? _validate;

	public UntypedBindingEntry(AvaloniaObject target, ValueFrame frame, AvaloniaProperty property, IObservable<object?> source)
		: base(target, frame, property, source)
	{
		_validate = ((IStyledPropertyAccessor)property).ValidateValue;
	}

	protected override BindingValue<object?> ConvertAndValidate(object? value)
	{
		return UntypedValueUtils.ConvertAndValidate(value, base.Property.PropertyType, _validate);
	}

	protected override BindingValue<object?> ConvertAndValidate(BindingValue<object?> value)
	{
		throw new NotSupportedException();
	}

	protected override object? GetDefaultValue(Type ownerType)
	{
		return ((IStyledPropertyMetadata)base.Property.GetMetadata(ownerType)).DefaultValue;
	}
}
