using System;
using Avalonia.Data;

namespace Avalonia.PropertyStore;

internal sealed class SourceUntypedBindingEntry<TTarget> : BindingEntryBase<TTarget, object?>
{
	private readonly Func<TTarget, bool>? _validate;

	public new StyledProperty<TTarget> Property => (StyledProperty<TTarget>)base.Property;

	public SourceUntypedBindingEntry(AvaloniaObject target, ValueFrame frame, StyledProperty<TTarget> property, IObservable<object?> source)
		: base(target, frame, (AvaloniaProperty)property, source)
	{
		_validate = property.ValidateValue;
	}

	protected override BindingValue<TTarget> ConvertAndValidate(object? value)
	{
		return UntypedValueUtils.ConvertAndValidate(value, Property.PropertyType, _validate);
	}

	protected override BindingValue<TTarget> ConvertAndValidate(BindingValue<object?> value)
	{
		throw new NotSupportedException();
	}

	protected override TTarget GetDefaultValue(Type ownerType)
	{
		return Property.GetDefaultValue(ownerType);
	}
}
