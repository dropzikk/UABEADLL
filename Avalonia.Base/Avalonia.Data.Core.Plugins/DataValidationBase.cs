using System;

namespace Avalonia.Data.Core.Plugins;

public abstract class DataValidationBase : PropertyAccessorBase, IObserver<object?>
{
	private readonly IPropertyAccessor _inner;

	public override Type? PropertyType => _inner.PropertyType;

	public override object? Value => _inner.Value;

	protected DataValidationBase(IPropertyAccessor inner)
	{
		_inner = inner;
	}

	public override bool SetValue(object? value, BindingPriority priority)
	{
		return _inner.SetValue(value, priority);
	}

	void IObserver<object>.OnCompleted()
	{
	}

	void IObserver<object>.OnError(Exception error)
	{
	}

	void IObserver<object>.OnNext(object? value)
	{
		InnerValueChanged(value);
	}

	protected override void SubscribeCore()
	{
		_inner.Subscribe(InnerValueChanged);
	}

	protected override void UnsubscribeCore()
	{
		_inner.Dispose();
	}

	protected virtual void InnerValueChanged(object? value)
	{
		BindingNotification value2 = (value as BindingNotification) ?? new BindingNotification(value);
		PublishValue(value2);
	}
}
