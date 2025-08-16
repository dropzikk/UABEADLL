using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Animation;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.PropertyStore;

namespace Avalonia.Styling;

public class Setter : SetterBase, IValueEntry, ISetterInstance, IAnimationSetter
{
	private object? _value;

	private DirectPropertySetterInstance? _direct;

	public AvaloniaProperty? Property { get; set; }

	[Content]
	[AssignBinding]
	[DependsOn("Property")]
	public object? Value
	{
		get
		{
			return _value;
		}
		set
		{
			(value as ISetterValue)?.Initialize(this);
			_value = value;
		}
	}

	bool IValueEntry.HasValue => true;

	AvaloniaProperty IValueEntry.Property => EnsureProperty();

	public Setter()
	{
	}

	public Setter(AvaloniaProperty property, object? value)
	{
		Property = property;
		Value = value;
	}

	public override string ToString()
	{
		return $"Setter: {Property} = {Value}";
	}

	void IValueEntry.Unsubscribe()
	{
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Implicit conversion methods might be removed by the linker. We don't have a reliable way to prevent it, except converting everything in compile time when possible.")]
	internal override ISetterInstance Instance(IStyleInstance instance, StyledElement target)
	{
		if (target == null)
		{
			throw new InvalidOperationException("Don't know how to instance a style on this type.");
		}
		if ((object)Property == null)
		{
			throw new InvalidOperationException("Setter.Property must be set.");
		}
		if (Property.IsDirect && instance.HasActivator)
		{
			throw new InvalidOperationException($"Cannot set direct property '{Property}' in '{instance.Source}' because the style has an activator.");
		}
		if (Value is IBinding binding)
		{
			return SetBinding((StyleInstance)instance, target, binding);
		}
		if (Value is ITemplate template && !typeof(ITemplate).IsAssignableFrom(Property.PropertyType))
		{
			return new PropertySetterTemplateInstance(Property, template);
		}
		if (!Property.IsValidValue(Value))
		{
			throw new InvalidCastException($"Setter value '{Value}' is not a valid value for property '{Property}'.");
		}
		if (Property.IsDirect)
		{
			return SetDirectValue(target);
		}
		return this;
	}

	object? IValueEntry.GetValue()
	{
		return Value;
	}

	bool IValueEntry.GetDataValidationState(out BindingValueType state, out Exception? error)
	{
		state = BindingValueType.Value;
		error = null;
		return false;
	}

	private AvaloniaProperty EnsureProperty()
	{
		return Property ?? throw new InvalidOperationException("Setter.Property must be set.");
	}

	private ISetterInstance SetBinding(StyleInstance instance, AvaloniaObject target, IBinding binding)
	{
		if (!Property.IsDirect)
		{
			bool valueOrDefault = Property.GetMetadata(target.GetType()).EnableDataValidation == true;
			InstancedBinding instancedBinding = binding.Initiate(target, Property, null, valueOrDefault);
			BindingMode bindingMode = instancedBinding.Mode;
			if (bindingMode == BindingMode.Default)
			{
				bindingMode = Property.GetMetadata(target.GetType()).DefaultBindingMode;
			}
			if (bindingMode == BindingMode.OneWay || bindingMode == BindingMode.TwoWay)
			{
				return new PropertySetterBindingInstance(target, instance, Property, bindingMode, instancedBinding.Source);
			}
			throw new NotSupportedException();
		}
		target.Bind(Property, binding);
		return new DirectPropertySetterBindingInstance();
	}

	private ISetterInstance SetDirectValue(StyledElement target)
	{
		target.SetValue(Property, Value);
		return _direct ?? (_direct = new DirectPropertySetterInstance());
	}
}
