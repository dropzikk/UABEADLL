using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Logging;
using Avalonia.Reactive;
using Avalonia.Utilities;

namespace Avalonia.Data.Core;

[RequiresUnreferencedCode("Conversion methods are required for type conversion, including op_Implicit, op_Explicit, Parse and TypeConverter.")]
internal class BindingExpression : LightweightObservableBase<object?>, IAvaloniaSubject<object?>, IObserver<object?>, IObservable<object?>, IDescription
{
	public class InnerListener : IObserver<object?>, IDisposable
	{
		private readonly BindingExpression _owner;

		private readonly IDisposable _dispose;

		public InnerListener(BindingExpression owner)
		{
			_owner = owner;
			_dispose = owner._inner.Subscribe(this);
		}

		public void Dispose()
		{
			_dispose.Dispose();
		}

		public void OnCompleted()
		{
			_owner.PublishCompleted();
		}

		public void OnError(Exception error)
		{
			_owner.PublishError(error);
		}

		public void OnNext(object? value)
		{
			if (value != BindingOperations.DoNothing)
			{
				object obj = _owner.ConvertValue(value);
				if (obj != BindingOperations.DoNothing)
				{
					_owner._value = ((obj != null) ? new WeakReference<object>(obj) : null);
					_owner.PublishNext(obj);
				}
			}
		}
	}

	private readonly ExpressionObserver _inner;

	private readonly Type _targetType;

	private readonly object? _fallbackValue;

	private readonly object? _targetNullValue;

	private readonly BindingPriority _priority;

	private InnerListener? _innerListener;

	private WeakReference<object>? _value;

	public IValueConverter Converter { get; }

	public object? ConverterParameter { get; }

	string? IDescription.Description => _inner.Expression;

	public BindingExpression(ExpressionObserver inner, Type targetType)
		: this(inner, targetType, DefaultValueConverter.Instance)
	{
	}

	public BindingExpression(ExpressionObserver inner, Type targetType, IValueConverter converter, object? converterParameter = null, BindingPriority priority = BindingPriority.LocalValue)
		: this(inner, targetType, AvaloniaProperty.UnsetValue, AvaloniaProperty.UnsetValue, converter, converterParameter, priority)
	{
	}

	public BindingExpression(ExpressionObserver inner, Type targetType, object? fallbackValue, object? targetNullValue, IValueConverter converter, object? converterParameter = null, BindingPriority priority = BindingPriority.LocalValue)
	{
		if (inner == null)
		{
			throw new ArgumentNullException("inner");
		}
		if ((object)targetType == null)
		{
			throw new ArgumentNullException("targetType");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		_inner = inner;
		_targetType = targetType;
		Converter = converter;
		ConverterParameter = converterParameter;
		_fallbackValue = fallbackValue;
		_targetNullValue = targetNullValue;
		_priority = priority;
	}

	public void OnCompleted()
	{
	}

	public void OnError(Exception error)
	{
	}

	public void OnNext(object? value)
	{
		if (value == BindingOperations.DoNothing)
		{
			return;
		}
		using (_inner.Subscribe(delegate
		{
		}))
		{
			Type resultType = _inner.ResultType;
			if (!(resultType != null))
			{
				return;
			}
			object result = Converter.ConvertBack(value, resultType, ConverterParameter, CultureInfo.CurrentCulture);
			if (result == BindingOperations.DoNothing)
			{
				return;
			}
			if (result == AvaloniaProperty.UnsetValue)
			{
				result = TypeUtilities.Default(resultType);
				_inner.SetValue(result, _priority);
			}
			else if (result is BindingNotification bindingNotification)
			{
				if (bindingNotification.ErrorType == BindingErrorType.None)
				{
					throw new AvaloniaInternalException("IValueConverter should not return non-errored BindingNotification.");
				}
				PublishNext(bindingNotification);
				if (_fallbackValue != AvaloniaProperty.UnsetValue)
				{
					if (TypeUtilities.TryConvert(resultType, _fallbackValue, CultureInfo.InvariantCulture, out result))
					{
						_inner.SetValue(result, _priority);
					}
					else
					{
						Logger.TryGet(LogEventLevel.Error, "Binding")?.Log(this, "Could not convert FallbackValue {FallbackValue} to {Type}", _fallbackValue, resultType);
					}
				}
			}
			else
			{
				_inner.SetValue(result, _priority);
			}
		}
	}

	protected override void Initialize()
	{
		_innerListener = new InnerListener(this);
	}

	protected override void Deinitialize()
	{
		_innerListener?.Dispose();
	}

	protected override void Subscribed(IObserver<object> observer, bool first)
	{
		if (!first && _value != null && _value.TryGetTarget(out object target))
		{
			observer.OnNext(target);
		}
	}

	private object? ConvertValue(object? value)
	{
		if (value == null && _targetNullValue != AvaloniaProperty.UnsetValue)
		{
			return _targetNullValue;
		}
		if (value == BindingOperations.DoNothing)
		{
			return value;
		}
		BindingNotification bindingNotification = value as BindingNotification;
		if (bindingNotification == null)
		{
			object obj = Converter.Convert(value, _targetType, ConverterParameter, CultureInfo.CurrentCulture);
			if (obj == BindingOperations.DoNothing)
			{
				return obj;
			}
			if (obj is BindingNotification { ErrorType: BindingErrorType.None } bindingNotification2)
			{
				obj = bindingNotification2.Value;
			}
			if (_fallbackValue != AvaloniaProperty.UnsetValue && (obj == AvaloniaProperty.UnsetValue || obj is BindingNotification))
			{
				BindingNotification b = ConvertFallback();
				obj = Merge(obj, b);
			}
			return obj;
		}
		return ConvertValue(bindingNotification);
	}

	private BindingNotification ConvertValue(BindingNotification notification)
	{
		if (notification.HasValue)
		{
			object b = ConvertValue(notification.Value);
			notification = Merge(notification, b);
		}
		else if (_fallbackValue != AvaloniaProperty.UnsetValue)
		{
			BindingNotification b2 = ConvertFallback();
			notification = Merge(notification, b2);
		}
		return notification;
	}

	private BindingNotification ConvertFallback()
	{
		if (_fallbackValue == AvaloniaProperty.UnsetValue)
		{
			throw new AvaloniaInternalException("Cannot call ConvertFallback with no fallback value");
		}
		if (TypeUtilities.TryConvert(_targetType, _fallbackValue, CultureInfo.InvariantCulture, out object result))
		{
			return new BindingNotification(result);
		}
		return new BindingNotification(new InvalidCastException($"Could not convert FallbackValue '{_fallbackValue}' to '{_targetType}'"), BindingErrorType.Error);
	}

	private static BindingNotification Merge(object a, BindingNotification b)
	{
		BindingNotification bindingNotification = a as BindingNotification;
		if (bindingNotification != null)
		{
			Merge(bindingNotification, b);
			return bindingNotification;
		}
		return b;
	}

	private static BindingNotification Merge(BindingNotification a, object? b)
	{
		BindingNotification bindingNotification = b as BindingNotification;
		if (bindingNotification != null)
		{
			Merge(a, bindingNotification);
		}
		else
		{
			a.SetValue(b);
		}
		return a;
	}

	private static BindingNotification Merge(BindingNotification a, BindingNotification b)
	{
		if (b.HasValue)
		{
			a.SetValue(b.Value);
		}
		else
		{
			a.ClearValue();
		}
		if (b.Error != null)
		{
			a.AddError(b.Error, b.ErrorType);
		}
		return a;
	}
}
