using System;
using Avalonia.Data;
using Avalonia.Threading;

namespace Avalonia.PropertyStore;

internal class LocalValueBindingObserverBase<T> : IObserver<T>, IObserver<BindingValue<T>>, IDisposable
{
	private readonly ValueStore _owner;

	private readonly bool _hasDataValidation;

	protected IDisposable? _subscription;

	private T? _defaultValue;

	private bool _isDefaultValueInitialized;

	public StyledProperty<T> Property { get; }

	protected LocalValueBindingObserverBase(ValueStore owner, StyledProperty<T> property)
	{
		_owner = owner;
		Property = property;
		_hasDataValidation = property.GetMetadata(owner.Owner.GetType()).EnableDataValidation == true;
	}

	public void Start(IObservable<T> source)
	{
		_subscription = source.Subscribe(this);
	}

	public void Start(IObservable<BindingValue<T>> source)
	{
		_subscription = source.Subscribe(this);
	}

	public void Dispose()
	{
		_subscription?.Dispose();
		_subscription = null;
		OnCompleted();
	}

	public void OnCompleted()
	{
		if (_hasDataValidation)
		{
			_owner.Owner.OnUpdateDataValidation(Property, BindingValueType.UnsetValue, null);
		}
		_owner.OnLocalValueBindingCompleted(Property, this);
	}

	public void OnError(Exception error)
	{
		OnCompleted();
	}

	public void OnNext(T value)
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			Execute(this, value);
			return;
		}
		T newValue = value;
		Dispatcher.UIThread.Post(delegate
		{
			Execute(this, newValue);
		});
		static void Execute(LocalValueBindingObserverBase<T> instance, T value)
		{
			ValueStore owner = instance._owner;
			StyledProperty<T> property = instance.Property;
			Func<T, bool>? validateValue = property.ValidateValue;
			if (validateValue != null && !validateValue(value))
			{
				value = instance.GetCachedDefaultValue();
			}
			owner.SetLocalValue(property, value);
			if (instance._hasDataValidation)
			{
				owner.Owner.OnUpdateDataValidation(property, BindingValueType.Value, null);
			}
		}
	}

	public void OnNext(BindingValue<T> value)
	{
		if (value.Type == BindingValueType.DoNothing)
		{
			return;
		}
		if (Dispatcher.UIThread.CheckAccess())
		{
			Execute(this, value);
			return;
		}
		BindingValue<T> newValue = value;
		Dispatcher.UIThread.Post(delegate
		{
			Execute(this, newValue);
		});
		static void Execute(LocalValueBindingObserverBase<T> instance, BindingValue<T> value)
		{
			ValueStore owner = instance._owner;
			StyledProperty<T> property = instance.Property;
			BindingValueType type = value.Type;
			LoggingUtils.LogIfNecessary(owner.Owner, property, value);
			if (value.HasValue)
			{
				Func<T, bool>? validateValue = property.ValidateValue;
				if (validateValue != null && !validateValue(value.Value))
				{
					goto IL_0061;
				}
			}
			if (!value.HasValue && value.Type != BindingValueType.DataValidationError)
			{
				goto IL_0061;
			}
			goto IL_0070;
			IL_0070:
			if (value.HasValue)
			{
				owner.SetLocalValue(property, value.Value);
			}
			if (instance._hasDataValidation)
			{
				owner.Owner.OnUpdateDataValidation(property, type, value.Error);
			}
			return;
			IL_0061:
			value = value.WithValue(instance.GetCachedDefaultValue());
			goto IL_0070;
		}
	}

	private T GetCachedDefaultValue()
	{
		if (!_isDefaultValueInitialized)
		{
			_defaultValue = Property.GetDefaultValue(_owner.Owner.GetType());
			_isDefaultValueInitialized = true;
		}
		return _defaultValue;
	}
}
