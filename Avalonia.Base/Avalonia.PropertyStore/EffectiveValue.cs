using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data;

namespace Avalonia.PropertyStore;

internal abstract class EffectiveValue
{
	public object? Value => GetBoxedValue();

	public BindingPriority Priority { get; protected set; }

	public BindingPriority BasePriority { get; protected set; }

	public IValueEntry? ValueEntry { get; private set; }

	public IValueEntry? BaseValueEntry { get; private set; }

	public bool HasCoercion { get; protected set; }

	public bool IsOverridenCurrentValue { get; set; }

	public bool IsCoercedDefaultValue { get; set; }

	public void BeginReevaluation(bool clearLocalValue = false)
	{
		if (clearLocalValue || (Priority != 0 && !IsOverridenCurrentValue))
		{
			Priority = BindingPriority.Unset;
		}
		if (clearLocalValue || (BasePriority != 0 && !IsOverridenCurrentValue))
		{
			BasePriority = BindingPriority.Unset;
		}
	}

	public void EndReevaluation(ValueStore owner, AvaloniaProperty property)
	{
		if (Priority == BindingPriority.Unset && HasCoercion)
		{
			CoerceDefaultValueAndRaise(owner, property);
		}
	}

	public bool CanRemove()
	{
		if (Priority == BindingPriority.Unset && !IsOverridenCurrentValue)
		{
			return !IsCoercedDefaultValue;
		}
		return false;
	}

	public void UnsubscribeIfNecessary()
	{
		if (Priority == BindingPriority.Unset)
		{
			ValueEntry?.Unsubscribe();
			ValueEntry = null;
		}
		if (BasePriority == BindingPriority.Unset)
		{
			BaseValueEntry?.Unsubscribe();
			BaseValueEntry = null;
		}
	}

	public abstract void SetAndRaise(ValueStore owner, IValueEntry value, BindingPriority priority);

	public abstract void RaiseInheritedValueChanged(AvaloniaObject owner, AvaloniaProperty property, EffectiveValue? oldValue, EffectiveValue? newValue);

	public abstract void RemoveAnimationAndRaise(ValueStore owner, AvaloniaProperty property);

	public abstract void CoerceValue(ValueStore owner, AvaloniaProperty property);

	public abstract void DisposeAndRaiseUnset(ValueStore owner, AvaloniaProperty property);

	protected abstract void CoerceDefaultValueAndRaise(ValueStore owner, AvaloniaProperty property);

	protected abstract object? GetBoxedValue();

	protected void UpdateValueEntry(IValueEntry? entry, BindingPriority priority)
	{
		if (priority <= BindingPriority.Animation)
		{
			if (Priority > BindingPriority.LocalValue && Priority < BindingPriority.Inherited)
			{
				BaseValueEntry = ValueEntry;
				ValueEntry = null;
			}
			if (ValueEntry != entry)
			{
				ValueEntry?.Unsubscribe();
				ValueEntry = entry;
			}
		}
		else if (Priority <= BindingPriority.Animation)
		{
			if (BaseValueEntry != entry)
			{
				BaseValueEntry?.Unsubscribe();
				BaseValueEntry = entry;
			}
		}
		else if (ValueEntry != entry)
		{
			ValueEntry?.Unsubscribe();
			ValueEntry = entry;
		}
	}
}
internal sealed class EffectiveValue<T> : EffectiveValue
{
	private class UncommonFields
	{
		public Func<AvaloniaObject, T, T>? _coerce;

		public T? _uncoercedValue;

		public T? _uncoercedBaseValue;
	}

	private readonly StyledPropertyMetadata<T> _metadata;

	private T? _baseValue;

	private UncommonFields? _uncommon;

	public new T Value { get; private set; }

	public EffectiveValue(AvaloniaObject owner, StyledProperty<T> property, EffectiveValue<T>? inherited)
	{
		base.Priority = BindingPriority.Unset;
		base.BasePriority = BindingPriority.Unset;
		_metadata = property.GetMetadata(owner.GetType());
		T val = (T)((inherited == null) ? ((object)_metadata.DefaultValue) : ((object)inherited.Value));
		Func<AvaloniaObject, T, T> coerceValue = _metadata.CoerceValue;
		if (coerceValue != null)
		{
			base.HasCoercion = true;
			_uncommon = new UncommonFields
			{
				_coerce = coerceValue,
				_uncoercedValue = val,
				_uncoercedBaseValue = val
			};
		}
		Value = val;
	}

	public override void SetAndRaise(ValueStore owner, IValueEntry value, BindingPriority priority)
	{
		UpdateValueEntry(value, priority);
		SetAndRaiseCore(owner, (StyledProperty<T>)value.Property, GetValue(value), priority);
		if (priority > BindingPriority.LocalValue && value.GetDataValidationState(out BindingValueType state, out Exception error))
		{
			owner.Owner.OnUpdateDataValidation(value.Property, state, error);
		}
	}

	public void SetLocalValueAndRaise(ValueStore owner, StyledProperty<T> property, T value)
	{
		SetAndRaiseCore(owner, property, value, BindingPriority.LocalValue);
	}

	public void SetCurrentValueAndRaise(ValueStore owner, StyledProperty<T> property, T value)
	{
		SetAndRaiseCore(owner, property, value, base.Priority, isOverriddenCurrentValue: true);
	}

	public void SetCoercedDefaultValueAndRaise(ValueStore owner, StyledProperty<T> property, T value)
	{
		SetAndRaiseCore(owner, property, value, base.Priority, isOverriddenCurrentValue: false, isCoercedDefaultValue: true);
	}

	public bool TryGetBaseValue([MaybeNullWhen(false)] out T value)
	{
		value = _baseValue;
		return base.BasePriority != BindingPriority.Unset;
	}

	public override void RaiseInheritedValueChanged(AvaloniaObject owner, AvaloniaProperty property, EffectiveValue? oldValue, EffectiveValue? newValue)
	{
		StyledProperty<T> property2 = (StyledProperty<T>)property;
		T val = ((oldValue != null) ? ((EffectiveValue<T>)oldValue).Value : _metadata.DefaultValue);
		T val2 = ((newValue != null) ? ((EffectiveValue<T>)newValue).Value : _metadata.DefaultValue);
		BindingPriority priority = ((newValue != null) ? BindingPriority.Inherited : BindingPriority.Unset);
		if (!EqualityComparer<T>.Default.Equals(val, val2))
		{
			owner.RaisePropertyChanged(property2, val, val2, priority, isEffectiveValue: true);
		}
	}

	public override void RemoveAnimationAndRaise(ValueStore owner, AvaloniaProperty property)
	{
		UpdateValueEntry(null, BindingPriority.Animation);
		SetAndRaiseCore(owner, (StyledProperty<T>)property, _baseValue, base.BasePriority);
	}

	public override void CoerceValue(ValueStore owner, AvaloniaProperty property)
	{
		if (_uncommon != null)
		{
			SetAndRaiseCore(owner, (StyledProperty<T>)property, _uncommon._uncoercedValue, base.Priority, _uncommon._uncoercedBaseValue, base.BasePriority);
		}
	}

	public override void DisposeAndRaiseUnset(ValueStore owner, AvaloniaProperty property)
	{
		base.ValueEntry?.Unsubscribe();
		base.BaseValueEntry?.Unsubscribe();
		StyledProperty<T> property2 = (StyledProperty<T>)property;
		T val;
		BindingPriority priority;
		if (property.Inherits && owner.TryGetInheritedValue(property, out EffectiveValue result))
		{
			val = ((EffectiveValue<T>)result).Value;
			priority = BindingPriority.Inherited;
		}
		else
		{
			val = _metadata.DefaultValue;
			priority = BindingPriority.Unset;
		}
		if (!EqualityComparer<T>.Default.Equals(val, Value))
		{
			owner.Owner.RaisePropertyChanged(property2, Value, val, priority, isEffectiveValue: true);
			if (property.Inherits)
			{
				owner.OnInheritedEffectiveValueDisposed(property2, Value, val);
			}
		}
		IValueEntry? valueEntry = base.ValueEntry;
		bool dataValidationState;
		BindingValueType state;
		Exception error;
		if (valueEntry == null)
		{
			IValueEntry? baseValueEntry = base.BaseValueEntry;
			if (baseValueEntry == null)
			{
				return;
			}
			dataValidationState = baseValueEntry.GetDataValidationState(out state, out error);
		}
		else
		{
			dataValidationState = valueEntry.GetDataValidationState(out state, out error);
		}
		if (dataValidationState)
		{
			owner.Owner.OnUpdateDataValidation(property2, BindingValueType.UnsetValue, null);
		}
	}

	protected override void CoerceDefaultValueAndRaise(ValueStore owner, AvaloniaProperty property)
	{
		T val = _uncommon._coerce(owner.Owner, _metadata.DefaultValue);
		if (!EqualityComparer<T>.Default.Equals(_metadata.DefaultValue, val))
		{
			SetCoercedDefaultValueAndRaise(owner, (StyledProperty<T>)property, val);
		}
	}

	protected override object? GetBoxedValue()
	{
		return Value;
	}

	private static T GetValue(IValueEntry entry)
	{
		if (entry is IValueEntry<T> valueEntry)
		{
			return valueEntry.GetValue();
		}
		return (T)entry.GetValue();
	}

	private void SetAndRaiseCore(ValueStore owner, StyledProperty<T> property, T value, BindingPriority priority, bool isOverriddenCurrentValue = false, bool isCoercedDefaultValue = false)
	{
		T value2 = Value;
		bool flag = false;
		bool flag2 = false;
		T val = value;
		base.IsOverridenCurrentValue = isOverriddenCurrentValue;
		base.IsCoercedDefaultValue = isCoercedDefaultValue;
		if (!isCoercedDefaultValue)
		{
			Func<AvaloniaObject, T, T> func = _uncommon?._coerce;
			if (func != null)
			{
				val = func(owner.Owner, value);
			}
		}
		if (priority <= base.Priority)
		{
			flag = !EqualityComparer<T>.Default.Equals(Value, val);
			Value = val;
			base.Priority = priority;
			if (!isCoercedDefaultValue && _uncommon != null)
			{
				_uncommon._uncoercedValue = value;
			}
		}
		if (priority <= base.BasePriority && priority >= BindingPriority.LocalValue)
		{
			flag2 = !EqualityComparer<T>.Default.Equals(_baseValue, val);
			_baseValue = val;
			base.BasePriority = priority;
			if (!isCoercedDefaultValue && _uncommon != null)
			{
				_uncommon._uncoercedBaseValue = value;
			}
		}
		if (flag)
		{
			using (PropertyNotifying.Start(owner.Owner, property))
			{
				owner.Owner.RaisePropertyChanged(property, value2, Value, base.Priority, isEffectiveValue: true);
				if (property.Inherits)
				{
					owner.OnInheritedEffectiveValueChanged(property, value2, this);
				}
				return;
			}
		}
		if (flag2)
		{
			owner.Owner.RaisePropertyChanged(property, default(Optional<T>), _baseValue, base.BasePriority, isEffectiveValue: false);
		}
	}

	private void SetAndRaiseCore(ValueStore owner, StyledProperty<T> property, T value, BindingPriority priority, T baseValue, BindingPriority basePriority)
	{
		T value2 = Value;
		bool flag = false;
		bool flag2 = false;
		T val = value;
		T y = baseValue;
		Func<AvaloniaObject, T, T> func = _uncommon?._coerce;
		if (func != null)
		{
			val = func(owner.Owner, value);
			if (priority != basePriority)
			{
				y = func(owner.Owner, baseValue);
			}
		}
		if (!EqualityComparer<T>.Default.Equals(Value, val))
		{
			Value = val;
			flag = true;
			if (_uncommon != null)
			{
				_uncommon._uncoercedValue = value;
			}
		}
		if (!EqualityComparer<T>.Default.Equals(_baseValue, y))
		{
			_baseValue = val;
			flag2 = true;
			if (_uncommon != null)
			{
				_uncommon._uncoercedValue = baseValue;
			}
		}
		base.Priority = priority;
		base.BasePriority = basePriority;
		if (flag)
		{
			using (PropertyNotifying.Start(owner.Owner, property))
			{
				owner.Owner.RaisePropertyChanged(property, value2, Value, base.Priority, isEffectiveValue: true);
				if (property.Inherits)
				{
					owner.OnInheritedEffectiveValueChanged(property, value2, this);
				}
			}
		}
		if (flag2)
		{
			owner.Owner.RaisePropertyChanged(property, default(Optional<T>), _baseValue, base.BasePriority, isEffectiveValue: false);
		}
	}
}
