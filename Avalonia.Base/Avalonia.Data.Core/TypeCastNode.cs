using System;
using System.Runtime.Serialization;

namespace Avalonia.Data.Core;

internal class TypeCastNode : ExpressionNode
{
	public override string Description => "as " + TargetType.FullName;

	public Type TargetType { get; }

	public TypeCastNode(Type type)
	{
		TargetType = type;
	}

	protected virtual object? Cast(object? value)
	{
		if (!TargetType.IsInstanceOfType(value))
		{
			return null;
		}
		return value;
	}

	protected override void StartListeningCore(WeakReference<object?> reference)
	{
		if (reference.TryGetTarget(out object target))
		{
			target = Cast(target);
			reference = (WeakReference<object?>)((target == null) ? ((ISerializable)ExpressionNode.NullReference) : ((ISerializable)new WeakReference<object>(target)));
		}
		base.StartListeningCore(reference);
	}
}
