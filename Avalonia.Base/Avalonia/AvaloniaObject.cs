using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Logging;
using Avalonia.PropertyStore;
using Avalonia.Threading;

namespace Avalonia;

public class AvaloniaObject : IAvaloniaObjectDebug, INotifyPropertyChanged
{
	private readonly ValueStore _values;

	private AvaloniaObject? _inheritanceParent;

	private PropertyChangedEventHandler? _inpcChanged;

	private EventHandler<AvaloniaPropertyChangedEventArgs>? _propertyChanged;

	private List<AvaloniaObject>? _inheritanceChildren;

	protected internal AvaloniaObject? InheritanceParent
	{
		get
		{
			return _inheritanceParent;
		}
		set
		{
			VerifyAccess();
			if (_inheritanceParent != value)
			{
				_inheritanceParent?.RemoveInheritanceChild(this);
				_inheritanceParent = value;
				_inheritanceParent?.AddInheritanceChild(this);
				_values.SetInheritanceParent(value);
			}
		}
	}

	public object? this[AvaloniaProperty property]
	{
		get
		{
			return GetValue(property);
		}
		set
		{
			SetValue(property, value);
		}
	}

	public IBinding this[IndexerDescriptor binding]
	{
		get
		{
			return new IndexerBinding(this, binding.Property, binding.Mode);
		}
		set
		{
			this.Bind(binding.Property, value);
		}
	}

	public event EventHandler<AvaloniaPropertyChangedEventArgs>? PropertyChanged
	{
		add
		{
			_propertyChanged = (EventHandler<AvaloniaPropertyChangedEventArgs>)Delegate.Combine(_propertyChanged, value);
		}
		remove
		{
			_propertyChanged = (EventHandler<AvaloniaPropertyChangedEventArgs>)Delegate.Remove(_propertyChanged, value);
		}
	}

	event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
	{
		add
		{
			_inpcChanged = (PropertyChangedEventHandler)Delegate.Combine(_inpcChanged, value);
		}
		remove
		{
			_inpcChanged = (PropertyChangedEventHandler)Delegate.Remove(_inpcChanged, value);
		}
	}

	public AvaloniaObject()
	{
		VerifyAccess();
		_values = new ValueStore(this);
	}

	public bool CheckAccess()
	{
		return Dispatcher.UIThread.CheckAccess();
	}

	public void VerifyAccess()
	{
		Dispatcher.UIThread.VerifyAccess();
	}

	public void ClearValue(AvaloniaProperty property)
	{
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		VerifyAccess();
		_values.ClearValue(property);
	}

	public void ClearValue<T>(AvaloniaProperty<T> property)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		if (!(property is StyledProperty<T> property2))
		{
			if (!(property is DirectPropertyBase<T> property3))
			{
				throw new NotSupportedException("Unsupported AvaloniaProperty type.");
			}
			ClearValue(property3);
		}
		else
		{
			ClearValue(property2);
		}
	}

	public void ClearValue<T>(StyledProperty<T> property)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		_values.ClearValue(property);
	}

	public void ClearValue<T>(DirectPropertyBase<T> property)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		DirectPropertyBase<T> registeredDirect = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(this, property);
		registeredDirect.InvokeSetter(this, registeredDirect.GetUnsetValue(GetType()));
	}

	public sealed override bool Equals(object? obj)
	{
		return base.Equals(obj);
	}

	public sealed override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public object? GetValue(AvaloniaProperty property)
	{
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		if (property.IsDirect)
		{
			return property.RouteGetValue(this);
		}
		return _values.GetValue(property);
	}

	public T GetValue<T>(StyledProperty<T> property)
	{
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		VerifyAccess();
		return _values.GetValue(property);
	}

	public T GetValue<T>(DirectPropertyBase<T> property)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		return AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(this, property).InvokeGetter(this);
	}

	public Optional<T> GetBaseValue<T>(StyledProperty<T> property)
	{
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		VerifyAccess();
		return _values.GetBaseValue(property);
	}

	public bool IsAnimating(AvaloniaProperty property)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		return _values.IsAnimating(property);
	}

	public bool IsSet(AvaloniaProperty property)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		return _values.IsSet(property);
	}

	public IDisposable? SetValue(AvaloniaProperty property, object? value, BindingPriority priority = BindingPriority.LocalValue)
	{
		property = property ?? throw new ArgumentNullException("property");
		return property.RouteSetValue(this, value, priority);
	}

	public IDisposable? SetValue<T>(StyledProperty<T> property, T value, BindingPriority priority = BindingPriority.LocalValue)
	{
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		VerifyAccess();
		ValidatePriority(priority);
		LogPropertySet(property, value, BindingPriority.LocalValue);
		if (value is UnsetValueType)
		{
			if (priority == BindingPriority.LocalValue)
			{
				_values.ClearValue(property);
			}
		}
		else if (!(value is DoNothingType))
		{
			return _values.SetValue(property, value, priority);
		}
		return null;
	}

	public void SetValue<T>(DirectPropertyBase<T> property, T value)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		property = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(this, property);
		LogPropertySet(property, value, BindingPriority.LocalValue);
		SetDirectValueUnchecked(property, value);
	}

	public void SetCurrentValue(AvaloniaProperty property, object? value)
	{
		property.RouteSetCurrentValue(this, value);
	}

	public void SetCurrentValue<T>(StyledProperty<T> property, T value)
	{
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		VerifyAccess();
		LogPropertySet(property, value, BindingPriority.LocalValue);
		if (value is UnsetValueType)
		{
			_values.ClearValue(property);
		}
		else if (!(value is DoNothingType))
		{
			_values.SetCurrentValue(property, value);
		}
	}

	public IDisposable Bind(AvaloniaProperty property, IObservable<object?> source, BindingPriority priority = BindingPriority.LocalValue)
	{
		return property.RouteBind(this, source, priority);
	}

	public IDisposable Bind<T>(StyledProperty<T> property, IObservable<object?> source, BindingPriority priority = BindingPriority.LocalValue)
	{
		property = property ?? throw new ArgumentNullException("property");
		source = source ?? throw new ArgumentNullException("source");
		VerifyAccess();
		ValidatePriority(priority);
		return _values.AddBinding(property, source, priority);
	}

	public IDisposable Bind<T>(StyledProperty<T> property, IObservable<T> source, BindingPriority priority = BindingPriority.LocalValue)
	{
		property = property ?? throw new ArgumentNullException("property");
		source = source ?? throw new ArgumentNullException("source");
		VerifyAccess();
		ValidatePriority(priority);
		return _values.AddBinding(property, source, priority);
	}

	public IDisposable Bind<T>(StyledProperty<T> property, IObservable<BindingValue<T>> source, BindingPriority priority = BindingPriority.LocalValue)
	{
		property = property ?? throw new ArgumentNullException("property");
		source = source ?? throw new ArgumentNullException("source");
		VerifyAccess();
		ValidatePriority(priority);
		return _values.AddBinding(property, source, priority);
	}

	public IDisposable Bind<T>(DirectPropertyBase<T> property, IObservable<object?> source)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		property = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(this, property);
		if (property.IsReadOnly)
		{
			throw new ArgumentException("The property " + property.Name + " is readonly.");
		}
		return _values.AddBinding(property, source);
	}

	public IDisposable Bind<T>(DirectPropertyBase<T> property, IObservable<T> source)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		property = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(this, property);
		if (property.IsReadOnly)
		{
			throw new ArgumentException("The property " + property.Name + " is readonly.");
		}
		return _values.AddBinding(property, source);
	}

	public IDisposable Bind<T>(DirectPropertyBase<T> property, IObservable<BindingValue<T>> source)
	{
		property = property ?? throw new ArgumentNullException("property");
		VerifyAccess();
		property = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(this, property);
		if (property.IsReadOnly)
		{
			throw new ArgumentException("The property " + property.Name + " is readonly.");
		}
		return _values.AddBinding(property, source);
	}

	public void CoerceValue(AvaloniaProperty property)
	{
		_values.CoerceValue(property);
	}

	internal void AddInheritanceChild(AvaloniaObject child)
	{
		if (_inheritanceChildren == null)
		{
			_inheritanceChildren = new List<AvaloniaObject>();
		}
		_inheritanceChildren.Add(child);
	}

	internal void RemoveInheritanceChild(AvaloniaObject child)
	{
		_inheritanceChildren?.Remove(child);
	}

	Delegate[]? IAvaloniaObjectDebug.GetPropertyChangedSubscribers()
	{
		return _propertyChanged?.GetInvocationList();
	}

	internal AvaloniaPropertyValue GetDiagnosticInternal(AvaloniaProperty property)
	{
		if (property.IsDirect)
		{
			return new AvaloniaPropertyValue(property, GetValue(property), BindingPriority.LocalValue, null, isOverriddenCurrentValue: false);
		}
		return _values.GetDiagnostic(property);
	}

	internal ValueStore GetValueStore()
	{
		return _values;
	}

	internal IReadOnlyList<AvaloniaObject>? GetInheritanceChildren()
	{
		return _inheritanceChildren;
	}

	internal virtual ParametrizedLogger? GetBindingWarningLogger(AvaloniaProperty property, Exception? e)
	{
		return Logger.TryGet(LogEventLevel.Warning, "Binding");
	}

	protected virtual void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
	}

	protected virtual void OnPropertyChangedCore(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.IsEffectiveValueChange)
		{
			OnPropertyChanged(change);
		}
	}

	protected virtual void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
	}

	protected void RaisePropertyChanged<T>(DirectPropertyBase<T> property, T oldValue, T newValue)
	{
		RaisePropertyChanged(property, oldValue, newValue, BindingPriority.LocalValue, isEffectiveValue: true);
	}

	internal void RaisePropertyChanged<T>(AvaloniaProperty<T> property, Optional<T> oldValue, BindingValue<T> newValue, BindingPriority priority, bool isEffectiveValue)
	{
		AvaloniaPropertyChangedEventArgs<T> avaloniaPropertyChangedEventArgs = new AvaloniaPropertyChangedEventArgs<T>(this, property, oldValue, newValue, priority, isEffectiveValue);
		OnPropertyChangedCore(avaloniaPropertyChangedEventArgs);
		if (isEffectiveValue)
		{
			property.NotifyChanged(avaloniaPropertyChangedEventArgs);
			_propertyChanged?.Invoke(this, avaloniaPropertyChangedEventArgs);
			_inpcChanged?.Invoke(this, new PropertyChangedEventArgs(property.Name));
		}
	}

	protected bool SetAndRaise<T>(DirectPropertyBase<T> property, ref T field, T value)
	{
		VerifyAccess();
		if (EqualityComparer<T>.Default.Equals(field, value))
		{
			return false;
		}
		T val = field;
		field = value;
		RaisePropertyChanged(property, val, value, BindingPriority.LocalValue, isEffectiveValue: true);
		return true;
	}

	internal void SetDirectValueUnchecked<T>(DirectPropertyBase<T> property, T value)
	{
		if (value is UnsetValueType)
		{
			property.InvokeSetter(this, property.GetUnsetValue(GetType()));
		}
		else if (!(value is DoNothingType))
		{
			property.InvokeSetter(this, value);
		}
	}

	internal void SetDirectValueUnchecked<T>(DirectPropertyBase<T> property, BindingValue<T> value)
	{
		LoggingUtils.LogIfNecessary(this, property, value);
		switch (value.Type)
		{
		case BindingValueType.UnsetValue:
		case BindingValueType.BindingError:
		{
			BindingValue<T> value2 = (value.HasValue ? value : value.WithValue(property.GetUnsetValue(GetType())));
			property.InvokeSetter(this, value2);
			break;
		}
		case BindingValueType.DataValidationError:
			property.InvokeSetter(this, value);
			break;
		case BindingValueType.Value:
		case BindingValueType.BindingErrorWithFallback:
		case BindingValueType.DataValidationErrorWithFallback:
			property.InvokeSetter(this, value);
			break;
		}
		if (property.GetMetadata(GetType()).EnableDataValidation == true)
		{
			UpdateDataValidation(property, value.Type, value.Error);
		}
	}

	internal void OnUpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		UpdateDataValidation(property, state, error);
	}

	private string GetDescription(object o)
	{
		return (o as IDescription)?.Description ?? o.ToString() ?? o.GetType().Name;
	}

	private void LogPropertySet<T>(AvaloniaProperty<T> property, T value, BindingPriority priority)
	{
		Logger.TryGet(LogEventLevel.Verbose, "Property")?.Log(this, "Set {Property} to {$Value} with priority {Priority}", property, value, priority);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ValidatePriority(BindingPriority priority)
	{
		if (priority < BindingPriority.Animation || priority >= BindingPriority.Inherited)
		{
			ThrowInvalidPriority(priority);
		}
	}

	private static void ThrowInvalidPriority(BindingPriority priority)
	{
		throw new ArgumentException($"Invalid priority ${priority}", "priority");
	}
}
