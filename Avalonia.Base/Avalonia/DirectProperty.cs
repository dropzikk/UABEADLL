using System;
using Avalonia.Data;

namespace Avalonia;

public class DirectProperty<TOwner, TValue> : DirectPropertyBase<TValue>, IDirectPropertyAccessor where TOwner : AvaloniaObject
{
	public Func<TOwner, TValue> Getter { get; }

	public Action<TOwner, TValue>? Setter { get; }

	internal DirectProperty(string name, Func<TOwner, TValue> getter, Action<TOwner, TValue>? setter, DirectPropertyMetadata<TValue> metadata)
		: base(name, typeof(TOwner), (AvaloniaPropertyMetadata)metadata)
	{
		Getter = getter ?? throw new ArgumentNullException("getter");
		Setter = setter;
		base.IsDirect = true;
		base.IsReadOnly = setter == null;
	}

	private DirectProperty(DirectPropertyBase<TValue> source, Func<TOwner, TValue> getter, Action<TOwner, TValue>? setter, DirectPropertyMetadata<TValue> metadata)
		: base(source, typeof(TOwner), (AvaloniaPropertyMetadata)metadata)
	{
		Getter = getter ?? throw new ArgumentNullException("getter");
		Setter = setter;
		base.IsDirect = true;
		base.IsReadOnly = setter == null;
	}

	public DirectProperty<TNewOwner, TValue> AddOwner<TNewOwner>(Func<TNewOwner, TValue> getter, Action<TNewOwner, TValue>? setter = null, TValue unsetValue = default(TValue), BindingMode defaultBindingMode = BindingMode.Default, bool enableDataValidation = false) where TNewOwner : AvaloniaObject
	{
		DirectPropertyMetadata<TValue> directPropertyMetadata = new DirectPropertyMetadata<TValue>(unsetValue, defaultBindingMode, enableDataValidation);
		directPropertyMetadata.Merge(GetMetadata<TOwner>(), this);
		DirectProperty<TNewOwner, TValue> directProperty = new DirectProperty<TNewOwner, TValue>(this, getter, setter, directPropertyMetadata);
		AvaloniaPropertyRegistry.Instance.Register(typeof(TNewOwner), directProperty);
		return directProperty;
	}

	internal override TValue InvokeGetter(AvaloniaObject instance)
	{
		return Getter((TOwner)instance);
	}

	internal override void InvokeSetter(AvaloniaObject instance, BindingValue<TValue> value)
	{
		if (Setter == null)
		{
			throw new ArgumentException("The property " + base.Name + " is readonly.");
		}
		if (value.HasValue)
		{
			Setter((TOwner)instance, value.Value);
		}
	}

	object? IDirectPropertyAccessor.GetValue(AvaloniaObject instance)
	{
		return Getter((TOwner)instance);
	}

	void IDirectPropertyAccessor.SetValue(AvaloniaObject instance, object? value)
	{
		if (Setter == null)
		{
			throw new ArgumentException("The property " + base.Name + " is readonly.");
		}
		Setter((TOwner)instance, (TValue)value);
	}
}
