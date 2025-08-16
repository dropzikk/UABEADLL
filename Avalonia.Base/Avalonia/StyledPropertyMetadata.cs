using System;
using Avalonia.Data;

namespace Avalonia;

public class StyledPropertyMetadata<TValue> : AvaloniaPropertyMetadata, IStyledPropertyMetadata
{
	private Optional<TValue> _defaultValue;

	public TValue DefaultValue => _defaultValue.GetValueOrDefault();

	public Func<AvaloniaObject, TValue, TValue>? CoerceValue { get; private set; }

	object? IStyledPropertyMetadata.DefaultValue => DefaultValue;

	public StyledPropertyMetadata(Optional<TValue> defaultValue = default(Optional<TValue>), BindingMode defaultBindingMode = BindingMode.Default, Func<AvaloniaObject, TValue, TValue>? coerce = null, bool enableDataValidation = false)
		: base(defaultBindingMode, enableDataValidation)
	{
		_defaultValue = defaultValue;
		CoerceValue = coerce;
	}

	public override void Merge(AvaloniaPropertyMetadata baseMetadata, AvaloniaProperty property)
	{
		base.Merge(baseMetadata, property);
		if (baseMetadata is StyledPropertyMetadata<TValue> styledPropertyMetadata)
		{
			if (!_defaultValue.HasValue)
			{
				_defaultValue = styledPropertyMetadata.DefaultValue;
			}
			if (CoerceValue == null)
			{
				CoerceValue = styledPropertyMetadata.CoerceValue;
			}
		}
	}

	public override AvaloniaPropertyMetadata GenerateTypeSafeMetadata()
	{
		return new StyledPropertyMetadata<TValue>(DefaultValue, base.DefaultBindingMode, null, base.EnableDataValidation == true);
	}
}
