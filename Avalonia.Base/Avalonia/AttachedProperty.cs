using System;

namespace Avalonia;

public class AttachedProperty<TValue> : StyledProperty<TValue>
{
	internal AttachedProperty(string name, Type ownerType, Type hostType, StyledPropertyMetadata<TValue> metadata, bool inherits = false, Func<TValue, bool>? validate = null)
		: base(name, ownerType, hostType, metadata, inherits, validate, (Action<AvaloniaObject, bool>?)null)
	{
		base.IsAttached = true;
	}

	public new AttachedProperty<TValue> AddOwner<TOwner>(StyledPropertyMetadata<TValue>? metadata = null) where TOwner : AvaloniaObject
	{
		AvaloniaPropertyRegistry.Instance.Register(typeof(TOwner), this);
		if (metadata != null)
		{
			OverrideMetadata<TOwner>(metadata);
		}
		return this;
	}
}
