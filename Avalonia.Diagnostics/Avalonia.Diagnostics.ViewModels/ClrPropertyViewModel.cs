using System;
using System.Reflection;

namespace Avalonia.Diagnostics.ViewModels;

internal class ClrPropertyViewModel : PropertyViewModel
{
	private readonly object _target;

	private Type _assignedType;

	private object? _value;

	private readonly Type _propertyType;

	public PropertyInfo Property { get; }

	public override object Key => Name;

	public override string Name { get; }

	public override string Group => "CLR Properties";

	public override Type AssignedType => _assignedType;

	public override Type PropertyType => _propertyType;

	public override bool IsReadonly => !Property.CanWrite;

	public override object? Value
	{
		get
		{
			return _value;
		}
		set
		{
			try
			{
				Property.SetValue(_target, value);
				Update();
			}
			catch
			{
			}
		}
	}

	public override string Priority => string.Empty;

	public override bool? IsAttached => null;

	public override Type? DeclaringType { get; }

	public ClrPropertyViewModel(object o, PropertyInfo property)
	{
		_target = o;
		Property = property;
		if (property.DeclaringType == null || !property.DeclaringType.IsInterface)
		{
			Name = property.Name;
		}
		else
		{
			Name = property.DeclaringType.Name + "." + property.Name;
		}
		DeclaringType = property.DeclaringType;
		_propertyType = property.PropertyType;
		Update();
	}

	public override void Update()
	{
		Type type = null;
		object obj;
		try
		{
			obj = Property.GetValue(_target);
			type = obj?.GetType();
		}
		catch (Exception ex)
		{
			obj = ex.GetBaseException();
		}
		RaiseAndSetIfChanged(ref _value, obj, "Value");
		RaiseAndSetIfChanged(ref _assignedType, type ?? Property.PropertyType, "AssignedType");
		RaisePropertyChanged("Type");
	}
}
