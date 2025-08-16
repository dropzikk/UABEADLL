using System;

namespace Avalonia.Diagnostics.ViewModels;

internal abstract class PropertyViewModel : ViewModelBase
{
	public abstract object Key { get; }

	public abstract string Name { get; }

	public abstract string Group { get; }

	public abstract Type AssignedType { get; }

	public abstract Type? DeclaringType { get; }

	public abstract object? Value { get; set; }

	public abstract string Priority { get; }

	public abstract bool? IsAttached { get; }

	public abstract Type PropertyType { get; }

	public string Type
	{
		get
		{
			if (!(PropertyType == AssignedType))
			{
				return PropertyType.GetTypeName() + " {" + AssignedType.GetTypeName() + "}";
			}
			return PropertyType.GetTypeName();
		}
	}

	public abstract bool IsReadonly { get; }

	public abstract void Update();
}
