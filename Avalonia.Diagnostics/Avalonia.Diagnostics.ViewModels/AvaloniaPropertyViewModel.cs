using System;
using Avalonia.Data;

namespace Avalonia.Diagnostics.ViewModels;

internal class AvaloniaPropertyViewModel : PropertyViewModel
{
	private readonly AvaloniaObject _target;

	private Type _assignedType;

	private object? _value;

	private string _priority;

	private string _group;

	private readonly Type _propertyType;

	public AvaloniaProperty Property { get; }

	public override object Key => Property;

	public override string Name { get; }

	public override bool? IsAttached => Property.IsAttached;

	public override string Priority => _priority;

	public override Type AssignedType => _assignedType;

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
				_target.SetValue(Property, value);
				Update();
			}
			catch
			{
			}
		}
	}

	public override string Group => _group;

	public override Type? DeclaringType { get; }

	public override Type PropertyType => _propertyType;

	public override bool IsReadonly => Property.IsReadOnly;

	public AvaloniaPropertyViewModel(AvaloniaObject o, AvaloniaProperty property)
	{
		_target = o;
		Property = property;
		Name = (property.IsAttached ? $"[{property.OwnerType.Name}.{property.Name}]" : property.Name);
		DeclaringType = property.OwnerType;
		_propertyType = property.PropertyType;
		Update();
	}

	public override void Update()
	{
		if (Property.IsDirect)
		{
			Type type = null;
			object obj;
			try
			{
				obj = _target.GetValue(Property);
				type = obj?.GetType();
			}
			catch (Exception ex)
			{
				obj = ex.GetBaseException();
			}
			RaiseAndSetIfChanged(ref _value, obj, "Value");
			RaiseAndSetIfChanged(ref _assignedType, type ?? Property.PropertyType, "AssignedType");
			RaiseAndSetIfChanged(ref _priority, "Direct", "Priority");
			_group = "Properties";
		}
		else
		{
			Type type2 = null;
			BindingPriority? bindingPriority = null;
			object obj2;
			try
			{
				AvaloniaPropertyValue diagnostic = _target.GetDiagnostic(Property);
				obj2 = diagnostic.Value;
				type2 = obj2?.GetType();
				bindingPriority = diagnostic.Priority;
			}
			catch (Exception ex2)
			{
				obj2 = ex2.GetBaseException();
			}
			RaiseAndSetIfChanged(ref _value, obj2, "Value");
			RaiseAndSetIfChanged(ref _assignedType, type2 ?? Property.PropertyType, "AssignedType");
			if (bindingPriority.HasValue)
			{
				RaiseAndSetIfChanged(ref _priority, bindingPriority.ToString(), "Priority");
				RaiseAndSetIfChanged(ref _group, (IsAttached == true) ? "Attached Properties" : "Properties", "Group");
			}
			else
			{
				RaiseAndSetIfChanged(ref _priority, "Unset", "Priority");
				RaiseAndSetIfChanged(ref _group, "Unset", "Group");
			}
		}
		RaisePropertyChanged("Type");
	}
}
