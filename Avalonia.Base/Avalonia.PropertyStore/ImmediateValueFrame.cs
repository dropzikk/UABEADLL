using System;
using Avalonia.Data;

namespace Avalonia.PropertyStore;

internal sealed class ImmediateValueFrame : ValueFrame
{
	public ImmediateValueFrame(BindingPriority priority)
		: base(priority, FrameType.Style)
	{
	}

	public TypedBindingEntry<T> AddBinding<T>(StyledProperty<T> property, IObservable<BindingValue<T>> source)
	{
		TypedBindingEntry<T> typedBindingEntry = new TypedBindingEntry<T>(base.Owner.Owner, this, property, source);
		Add(typedBindingEntry);
		return typedBindingEntry;
	}

	public TypedBindingEntry<T> AddBinding<T>(StyledProperty<T> property, IObservable<T> source)
	{
		TypedBindingEntry<T> typedBindingEntry = new TypedBindingEntry<T>(base.Owner.Owner, this, property, source);
		Add(typedBindingEntry);
		return typedBindingEntry;
	}

	public SourceUntypedBindingEntry<T> AddBinding<T>(StyledProperty<T> property, IObservable<object?> source)
	{
		SourceUntypedBindingEntry<T> sourceUntypedBindingEntry = new SourceUntypedBindingEntry<T>(base.Owner.Owner, this, property, source);
		Add(sourceUntypedBindingEntry);
		return sourceUntypedBindingEntry;
	}

	public ImmediateValueEntry<T> AddValue<T>(StyledProperty<T> property, T value)
	{
		ImmediateValueEntry<T> immediateValueEntry = new ImmediateValueEntry<T>(this, property, value);
		Add(immediateValueEntry);
		return immediateValueEntry;
	}

	public void OnEntryDisposed(IValueEntry value)
	{
		Remove(value.Property);
		base.Owner?.OnValueEntryRemoved(this, value.Property);
	}

	protected override bool GetIsActive(out bool hasChanged)
	{
		hasChanged = false;
		return true;
	}
}
