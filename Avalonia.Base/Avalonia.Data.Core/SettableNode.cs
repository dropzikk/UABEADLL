using System;

namespace Avalonia.Data.Core;

internal abstract class SettableNode : ExpressionNode
{
	public abstract Type? PropertyType { get; }

	public bool SetTargetValue(object? value, BindingPriority priority)
	{
		if (ShouldNotSet(value))
		{
			return true;
		}
		return SetTargetValueCore(value, priority);
	}

	private bool ShouldNotSet(object? value)
	{
		Type propertyType = PropertyType;
		if (propertyType == null)
		{
			return false;
		}
		if (base.LastValue == null)
		{
			return false;
		}
		if (!base.LastValue.TryGetTarget(out object target))
		{
			if (value == null && base.LastValue == ExpressionNode.NullReference)
			{
				return true;
			}
			return false;
		}
		if (propertyType.IsValueType)
		{
			return object.Equals(target, value);
		}
		return target == value;
	}

	protected abstract bool SetTargetValueCore(object? value, BindingPriority priority);
}
