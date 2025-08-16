using System;
using Avalonia.Data;

namespace Avalonia;

public abstract class AvaloniaPropertyChangedEventArgs : EventArgs
{
	public AvaloniaObject Sender { get; }

	public AvaloniaProperty Property => GetProperty();

	public object? OldValue => GetOldValue();

	public object? NewValue => GetNewValue();

	public BindingPriority Priority { get; private set; }

	internal bool IsEffectiveValueChange { get; private set; }

	public AvaloniaPropertyChangedEventArgs(AvaloniaObject sender, BindingPriority priority)
	{
		Sender = sender;
		Priority = priority;
		IsEffectiveValueChange = true;
	}

	internal AvaloniaPropertyChangedEventArgs(AvaloniaObject sender, BindingPriority priority, bool isEffectiveValueChange)
	{
		Sender = sender;
		Priority = priority;
		IsEffectiveValueChange = isEffectiveValueChange;
	}

	protected abstract AvaloniaProperty GetProperty();

	protected abstract object? GetOldValue();

	protected abstract object? GetNewValue();
}
public class AvaloniaPropertyChangedEventArgs<T> : AvaloniaPropertyChangedEventArgs
{
	public new AvaloniaProperty<T> Property { get; }

	public new Optional<T> OldValue { get; private set; }

	public new BindingValue<T> NewValue { get; private set; }

	public AvaloniaPropertyChangedEventArgs(AvaloniaObject sender, AvaloniaProperty<T> property, Optional<T> oldValue, BindingValue<T> newValue, BindingPriority priority)
		: this(sender, property, oldValue, newValue, priority, isEffectiveValueChange: true)
	{
	}

	internal AvaloniaPropertyChangedEventArgs(AvaloniaObject sender, AvaloniaProperty<T> property, Optional<T> oldValue, BindingValue<T> newValue, BindingPriority priority, bool isEffectiveValueChange)
		: base(sender, priority, isEffectiveValueChange)
	{
		Property = property;
		OldValue = oldValue;
		NewValue = newValue;
	}

	protected override AvaloniaProperty GetProperty()
	{
		return Property;
	}

	protected override object? GetOldValue()
	{
		return OldValue.GetValueOrDefault(AvaloniaProperty.UnsetValue);
	}

	protected override object? GetNewValue()
	{
		return NewValue.GetValueOrDefault(AvaloniaProperty.UnsetValue);
	}
}
