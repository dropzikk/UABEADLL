using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data;
using Avalonia.PropertyStore;
using Avalonia.Utilities;

namespace Avalonia;

public class StyledProperty<TValue> : AvaloniaProperty<TValue>, IStyledPropertyAccessor
{
	public Func<TValue, bool>? ValidateValue { get; }

	internal StyledProperty(string name, Type ownerType, Type hostType, StyledPropertyMetadata<TValue> metadata, bool inherits = false, Func<TValue, bool>? validate = null, Action<AvaloniaObject, bool>? notifying = null)
		: base(name, ownerType, hostType, (AvaloniaPropertyMetadata)metadata, notifying)
	{
		base.Inherits = inherits;
		ValidateValue = validate;
		if (validate != null && !validate(metadata.DefaultValue))
		{
			throw new ArgumentException($"'{metadata.DefaultValue}' is not a valid default value for '{name}'.");
		}
	}

	public StyledProperty<TValue> AddOwner<TOwner>(StyledPropertyMetadata<TValue>? metadata = null) where TOwner : AvaloniaObject
	{
		AvaloniaPropertyRegistry.Instance.Register(typeof(TOwner), this);
		if (metadata != null)
		{
			OverrideMetadata<TOwner>(metadata);
		}
		return this;
	}

	public TValue CoerceValue(AvaloniaObject instance, TValue baseValue)
	{
		StyledPropertyMetadata<TValue> metadata = GetMetadata(instance.GetType());
		if (metadata.CoerceValue != null)
		{
			return metadata.CoerceValue(instance, baseValue);
		}
		return baseValue;
	}

	public TValue GetDefaultValue(Type type)
	{
		return GetMetadata(type).DefaultValue;
	}

	public new StyledPropertyMetadata<TValue> GetMetadata(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		return (StyledPropertyMetadata<TValue>)base.GetMetadata(type);
	}

	public void OverrideDefaultValue<T>(TValue defaultValue) where T : AvaloniaObject
	{
		OverrideDefaultValue(typeof(T), defaultValue);
	}

	public void OverrideDefaultValue(Type type, TValue defaultValue)
	{
		OverrideMetadata(type, new StyledPropertyMetadata<TValue>(defaultValue));
	}

	public void OverrideMetadata<T>(StyledPropertyMetadata<TValue> metadata) where T : AvaloniaObject
	{
		OverrideMetadata(typeof(T), metadata);
	}

	public void OverrideMetadata(Type type, StyledPropertyMetadata<TValue> metadata)
	{
		if (ValidateValue != null && !ValidateValue(metadata.DefaultValue))
		{
			throw new ArgumentException($"'{metadata.DefaultValue}' is not a valid default value for '{base.Name}'.");
		}
		OverrideMetadata(type, (AvaloniaPropertyMetadata)metadata);
	}

	public override string ToString()
	{
		return base.Name;
	}

	object? IStyledPropertyAccessor.GetDefaultValue(Type type)
	{
		return GetDefaultBoxedValue(type);
	}

	bool IStyledPropertyAccessor.ValidateValue(object? value)
	{
		if (value == null && !typeof(TValue).IsValueType)
		{
			return ValidateValue?.Invoke(default(TValue)) ?? true;
		}
		if (value is TValue arg)
		{
			return ValidateValue?.Invoke(arg) ?? true;
		}
		return false;
	}

	internal override EffectiveValue CreateEffectiveValue(AvaloniaObject o)
	{
		return o.GetValueStore().CreateEffectiveValue(this);
	}

	internal override void RouteClearValue(AvaloniaObject o)
	{
		o.ClearValue(this);
	}

	internal override void RouteCoerceDefaultValue(AvaloniaObject o)
	{
		o.GetValueStore().CoerceDefaultValue(this);
	}

	internal override object? RouteGetValue(AvaloniaObject o)
	{
		return o.GetValue(this);
	}

	internal override object? RouteGetBaseValue(AvaloniaObject o)
	{
		Optional<TValue> baseValue = o.GetBaseValue(this);
		if (!baseValue.HasValue)
		{
			return AvaloniaProperty.UnsetValue;
		}
		return baseValue.Value;
	}

	internal override IDisposable? RouteSetValue(AvaloniaObject target, object? value, BindingPriority priority)
	{
		if (ShouldSetValue(target, value, out TValue converted))
		{
			return target.SetValue(this, converted, priority);
		}
		return null;
	}

	internal override void RouteSetCurrentValue(AvaloniaObject target, object? value)
	{
		if (ShouldSetValue(target, value, out TValue converted))
		{
			target.SetCurrentValue(this, converted);
		}
	}

	internal override IDisposable RouteBind(AvaloniaObject target, IObservable<object?> source, BindingPriority priority)
	{
		return target.Bind(this, source, priority);
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Implicit conversion methods might be removed by the linker. We don't have a reliable way to prevent it, except converting everything in compile time when possible.")]
	private bool ShouldSetValue(AvaloniaObject target, object? value, [NotNullWhen(true)] out TValue? converted)
	{
		if (value == BindingOperations.DoNothing)
		{
			converted = default(TValue);
			return false;
		}
		if (value == AvaloniaProperty.UnsetValue)
		{
			target.ClearValue(this);
			converted = default(TValue);
			return false;
		}
		if (TypeUtilities.TryConvertImplicit(base.PropertyType, value, out object result))
		{
			converted = (TValue)result;
			return true;
		}
		string value2 = value?.GetType().FullName ?? "(null)";
		throw new ArgumentException($"Invalid value for Property '{base.Name}': '{value}' ({value2})");
	}

	private object? GetDefaultBoxedValue(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		return GetMetadata(type).DefaultValue;
	}
}
