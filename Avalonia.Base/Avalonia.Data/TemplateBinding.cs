using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Reactive;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Avalonia.Data;

public class TemplateBinding : IObservable<object?>, IBinding, IDescription, IAvaloniaSubject<object?>, IObserver<object?>, ISetterValue, IDisposable
{
	private IObserver<object?>? _observer;

	private bool _isSetterValue;

	private StyledElement? _target;

	private Type? _targetType;

	private bool _hasProducedValue;

	public IValueConverter? Converter { get; set; }

	public object? ConverterParameter { get; set; }

	public BindingMode Mode { get; set; }

	public AvaloniaProperty? Property { get; set; }

	public string Description => "TemplateBinding: " + Property;

	public TemplateBinding()
	{
	}

	public TemplateBinding(AvaloniaProperty property)
	{
		Property = property;
	}

	public IDisposable Subscribe(IObserver<object?> observer)
	{
		if (observer == null)
		{
			throw new ArgumentNullException("observer");
		}
		Dispatcher.UIThread.VerifyAccess();
		if (_observer != null)
		{
			throw new InvalidOperationException("The observable can only be subscribed once.");
		}
		_observer = observer;
		Subscribed();
		return this;
	}

	public virtual void Dispose()
	{
		Unsubscribed();
		_observer = null;
	}

	public InstancedBinding? Initiate(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor = null, bool enableDataValidation = false)
	{
		if (_target == null && !_isSetterValue)
		{
			_target = (StyledElement)target;
			_targetType = targetProperty?.PropertyType;
			return new InstancedBinding(this, (Mode == BindingMode.Default) ? BindingMode.OneWay : Mode, BindingPriority.Template);
		}
		return new TemplateBinding
		{
			Converter = Converter,
			ConverterParameter = ConverterParameter,
			Property = Property
		}.Initiate(target, targetProperty, anchor, enableDataValidation);
	}

	void IObserver<object>.OnCompleted()
	{
		throw new NotImplementedException();
	}

	void IObserver<object>.OnError(Exception error)
	{
		throw new NotImplementedException();
	}

	void IObserver<object>.OnNext(object? value)
	{
		AvaloniaObject avaloniaObject = _target?.TemplatedParent;
		if (avaloniaObject != null && (object)Property != null)
		{
			if (Converter != null)
			{
				value = Converter.ConvertBack(value, Property.PropertyType, ConverterParameter, CultureInfo.CurrentCulture);
			}
			avaloniaObject.SetCurrentValue(Property, value);
		}
	}

	void ISetterValue.Initialize(SetterBase setter)
	{
		_isSetterValue = true;
	}

	private void Subscribed()
	{
		TemplatedParentChanged();
		if (_target != null)
		{
			_target.PropertyChanged += TargetPropertyChanged;
		}
	}

	private void Unsubscribed()
	{
		AvaloniaObject avaloniaObject = _target?.TemplatedParent;
		if (avaloniaObject != null)
		{
			avaloniaObject.PropertyChanged -= TemplatedParentPropertyChanged;
		}
		if (_target != null)
		{
			_target.PropertyChanged -= TargetPropertyChanged;
		}
	}

	private void PublishValue()
	{
		AvaloniaObject avaloniaObject = _target?.TemplatedParent;
		if (avaloniaObject != null)
		{
			object value = (((object)Property != null) ? avaloniaObject.GetValue(Property) : _target.TemplatedParent);
			if (Converter != null)
			{
				value = Converter.Convert(value, _targetType ?? typeof(object), ConverterParameter, CultureInfo.CurrentCulture);
			}
			_observer?.OnNext(value);
			_hasProducedValue = true;
		}
		else if (_hasProducedValue)
		{
			_observer?.OnNext(AvaloniaProperty.UnsetValue);
			_hasProducedValue = false;
		}
	}

	private void TemplatedParentChanged()
	{
		AvaloniaObject avaloniaObject = _target?.TemplatedParent;
		if (avaloniaObject != null)
		{
			avaloniaObject.PropertyChanged += TemplatedParentPropertyChanged;
		}
		PublishValue();
	}

	private void TargetPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == StyledElement.TemplatedParentProperty)
		{
			if (e.OldValue is AvaloniaObject avaloniaObject)
			{
				avaloniaObject.PropertyChanged -= TemplatedParentPropertyChanged;
			}
			TemplatedParentChanged();
		}
	}

	private void TemplatedParentPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == Property)
		{
			PublishValue();
		}
	}

	public IBinding ProvideValue()
	{
		return this;
	}
}
