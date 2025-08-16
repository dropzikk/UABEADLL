using System;
using Avalonia.Data;
using Avalonia.PropertyStore;

namespace Avalonia;

public abstract class DirectPropertyBase<TValue> : AvaloniaProperty<TValue>
{
	public Type Owner { get; }

	private protected DirectPropertyBase(string name, Type ownerType, AvaloniaPropertyMetadata metadata)
		: base(name, ownerType, ownerType, metadata, (Action<AvaloniaObject, bool>?)null)
	{
		Owner = ownerType;
	}

	private protected DirectPropertyBase(DirectPropertyBase<TValue> source, Type ownerType, AvaloniaPropertyMetadata metadata)
		: base((AvaloniaProperty<TValue>)source, ownerType, metadata)
	{
		Owner = ownerType;
	}

	internal abstract TValue InvokeGetter(AvaloniaObject instance);

	internal abstract void InvokeSetter(AvaloniaObject instance, BindingValue<TValue> value);

	public TValue GetUnsetValue(Type type)
	{
		type = type ?? throw new ArgumentNullException("type");
		return GetMetadata(type).UnsetValue;
	}

	public new DirectPropertyMetadata<TValue> GetMetadata(Type type)
	{
		return (DirectPropertyMetadata<TValue>)base.GetMetadata(type);
	}

	public void OverrideMetadata<T>(DirectPropertyMetadata<TValue> metadata) where T : AvaloniaObject
	{
		OverrideMetadata(typeof(T), (AvaloniaPropertyMetadata)metadata);
	}

	public void OverrideMetadata(Type type, DirectPropertyMetadata<TValue> metadata)
	{
		OverrideMetadata(type, (AvaloniaPropertyMetadata)metadata);
	}

	internal override EffectiveValue CreateEffectiveValue(AvaloniaObject o)
	{
		throw new InvalidOperationException("Cannot create EffectiveValue for direct property.");
	}

	internal override void RouteClearValue(AvaloniaObject o)
	{
		o.ClearValue(this);
	}

	internal override void RouteCoerceDefaultValue(AvaloniaObject o)
	{
	}

	internal override object? RouteGetValue(AvaloniaObject o)
	{
		return o.GetValue(this);
	}

	internal override object? RouteGetBaseValue(AvaloniaObject o)
	{
		return o.GetValue(this);
	}

	internal override IDisposable? RouteSetValue(AvaloniaObject o, object? value, BindingPriority priority)
	{
		BindingValue<object> bindingValue = TryConvert(value);
		if (bindingValue.HasValue)
		{
			o.SetValue(this, (TValue)bindingValue.Value);
		}
		else if (bindingValue.Type == BindingValueType.UnsetValue)
		{
			o.ClearValue(this);
		}
		else if (bindingValue.HasError)
		{
			throw bindingValue.Error;
		}
		return null;
	}

	internal override void RouteSetCurrentValue(AvaloniaObject o, object? value)
	{
		RouteSetValue(o, value, BindingPriority.LocalValue);
	}

	internal override IDisposable RouteBind(AvaloniaObject o, IObservable<object?> source, BindingPriority priority)
	{
		return o.Bind(this, source);
	}
}
