using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Utilities;

namespace Avalonia.Data;

public readonly record struct BindingValue<T>(T value)
{
	public bool HasError => Type.HasAllFlags(BindingValueType.HasError);

	public bool HasValue => Type.HasAllFlags(BindingValueType.HasValue);

	public BindingValueType Type { get; }

	public T Value
	{
		get
		{
			if (!HasValue)
			{
				throw new InvalidOperationException("BindingValue has no value.");
			}
			return _value;
		}
	}

	public Exception? Error { get; }

	public static BindingValue<T> Unset => new BindingValue<T>(BindingValueType.UnsetValue, default(T), null);

	public static BindingValue<T> DoNothing => new BindingValue<T>(BindingValueType.DoNothing, default(T), null);

	private readonly T _value;

	public BindingValue(T value)
	{
		Type = BindingValueType.Value;
		Error = null;
	}

	private BindingValue(BindingValueType type, T? value, Exception? error)
	{
		Type = type;
		Error = error;
	}

	public Optional<T> ToOptional()
	{
		if (!HasValue)
		{
			return default(Optional<T>);
		}
		return new Optional<T>(_value);
	}

	public override string ToString()
	{
		object obj;
		if (!HasError)
		{
			T value = _value;
			obj = ((value != null) ? value.ToString() : null);
			if (obj == null)
			{
				return "(null)";
			}
		}
		else
		{
			obj = "Error: " + Error.Message;
		}
		return (string)obj;
	}

	public object? ToUntyped()
	{
		return Type switch
		{
			BindingValueType.UnsetValue => AvaloniaProperty.UnsetValue, 
			BindingValueType.DoNothing => BindingOperations.DoNothing, 
			BindingValueType.Value => _value, 
			BindingValueType.BindingError => new BindingNotification(Error, BindingErrorType.Error), 
			BindingValueType.BindingErrorWithFallback => new BindingNotification(Error, BindingErrorType.Error, Value), 
			BindingValueType.DataValidationError => new BindingNotification(Error, BindingErrorType.DataValidationError), 
			BindingValueType.DataValidationErrorWithFallback => new BindingNotification(Error, BindingErrorType.DataValidationError, Value), 
			_ => throw new NotSupportedException("Invalid BindingValueType."), 
		};
	}

	public BindingValue<T> WithValue(T value)
	{
		if (Type == BindingValueType.DoNothing)
		{
			throw new InvalidOperationException("Cannot add value to DoNothing binding value.");
		}
		return new BindingValue<T>(((Type == BindingValueType.UnsetValue) ? BindingValueType.Value : Type) | BindingValueType.HasValue, value, Error);
	}

	public T? GetValueOrDefault()
	{
		if (!HasValue)
		{
			return default(T);
		}
		return _value;
	}

	public T? GetValueOrDefault(T defaultValue)
	{
		if (!HasValue)
		{
			return defaultValue;
		}
		return _value;
	}

	public TResult? GetValueOrDefault<TResult>()
	{
		if (!HasValue)
		{
			return default(TResult);
		}
		T value = _value;
		if (value is TResult)
		{
			object obj = value;
			return (TResult)((obj is TResult) ? obj : null);
		}
		return default(TResult);
	}

	public TResult? GetValueOrDefault<TResult>(TResult defaultValue)
	{
		if (!HasValue)
		{
			return defaultValue;
		}
		T value = _value;
		if (value is TResult)
		{
			object obj = value;
			return (TResult)((obj is TResult) ? obj : null);
		}
		return default(TResult);
	}

	[RequiresUnreferencedCode("Implicit conversion methods are required for type conversion.")]
	public static BindingValue<T> FromUntyped(object? value)
	{
		return FromUntyped(value, typeof(T));
	}

	[RequiresUnreferencedCode("Implicit conversion methods are required for type conversion.")]
	public static BindingValue<T> FromUntyped(object? value, Type targetType)
	{
		if (value == AvaloniaProperty.UnsetValue)
		{
			return Unset;
		}
		if (value == BindingOperations.DoNothing)
		{
			return DoNothing;
		}
		BindingValueType bindingValueType = BindingValueType.Value;
		T value2 = default(T);
		Exception ex = null;
		List<Exception> list = null;
		if (value is BindingNotification bindingNotification)
		{
			ex = bindingNotification.Error;
			bindingValueType = bindingNotification.ErrorType switch
			{
				BindingErrorType.Error => BindingValueType.BindingError, 
				BindingErrorType.DataValidationError => BindingValueType.DataValidationError, 
				_ => BindingValueType.Value, 
			};
			if (bindingNotification.HasValue)
			{
				bindingValueType |= BindingValueType.HasValue;
			}
			value = bindingNotification.Value;
		}
		if ((bindingValueType & BindingValueType.HasValue) != 0)
		{
			if (TypeUtilities.TryConvertImplicit(targetType, value, out object result))
			{
				value2 = (T)result;
			}
			else
			{
				InvalidCastException ex2 = new InvalidCastException($"Unable to convert object '{value ?? "(null)"}' of type '{value?.GetType()}' to type '{targetType}'.");
				if (ex == null)
				{
					ex = ex2;
				}
				else
				{
					if (list == null)
					{
						list = new List<Exception> { ex };
					}
					list.Add(ex2);
				}
				bindingValueType = BindingValueType.BindingError;
			}
		}
		if (list != null)
		{
			ex = new AggregateException(list);
		}
		return new BindingValue<T>(bindingValueType, value2, ex);
	}

	public static implicit operator BindingValue<T>(T value)
	{
		return new BindingValue<T>(value);
	}

	public static implicit operator BindingValue<T>(Optional<T> optional)
	{
		if (!optional.HasValue)
		{
			return Unset;
		}
		return optional.Value;
	}

	public static BindingValue<T> BindingError(Exception e)
	{
		e = e ?? throw new ArgumentNullException("e");
		return new BindingValue<T>(BindingValueType.BindingError, default(T), e);
	}

	public static BindingValue<T> BindingError(Exception e, T fallbackValue)
	{
		e = e ?? throw new ArgumentNullException("e");
		return new BindingValue<T>(BindingValueType.BindingErrorWithFallback, fallbackValue, e);
	}

	public static BindingValue<T> BindingError(Exception e, Optional<T> fallbackValue)
	{
		e = e ?? throw new ArgumentNullException("e");
		return new BindingValue<T>(fallbackValue.HasValue ? BindingValueType.BindingErrorWithFallback : BindingValueType.BindingError, (T?)(fallbackValue.HasValue ? ((object)fallbackValue.Value) : ((object)default(T))), e);
	}

	public static BindingValue<T> DataValidationError(Exception e)
	{
		e = e ?? throw new ArgumentNullException("e");
		return new BindingValue<T>(BindingValueType.DataValidationError, default(T), e);
	}

	public static BindingValue<T> DataValidationError(Exception e, T fallbackValue)
	{
		e = e ?? throw new ArgumentNullException("e");
		return new BindingValue<T>(BindingValueType.DataValidationErrorWithFallback, fallbackValue, e);
	}

	public static BindingValue<T> DataValidationError(Exception e, Optional<T> fallbackValue)
	{
		e = e ?? throw new ArgumentNullException("e");
		return new BindingValue<T>(fallbackValue.HasValue ? BindingValueType.DataValidationErrorWithFallback : BindingValueType.DataValidationError, (T?)(fallbackValue.HasValue ? ((object)fallbackValue.Value) : ((object)default(T))), e);
	}

	[Conditional("DEBUG")]
	private static void ValidateValue(T value)
	{
		if (value is UnsetValueType)
		{
			throw new InvalidOperationException("AvaloniaValue.UnsetValue is not a valid value for BindingValue<>.");
		}
		if (value is DoNothingType)
		{
			throw new InvalidOperationException("BindingOperations.DoNothing is not a valid value for BindingValue<>.");
		}
		if (value is BindingValue<object>)
		{
			throw new InvalidOperationException("BindingValue<object> cannot be wrapped in a BindingValue<>.");
		}
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("HasError = ");
		builder.Append(HasError.ToString());
		builder.Append(", HasValue = ");
		builder.Append(HasValue.ToString());
		builder.Append(", Type = ");
		builder.Append(Type.ToString());
		builder.Append(", Value = ");
		builder.Append(Value);
		builder.Append(", Error = ");
		builder.Append(Error);
		return true;
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return (EqualityComparer<T>.Default.GetHashCode(_value) * -1521134295 + EqualityComparer<BindingValueType>.Default.GetHashCode(Type)) * -1521134295 + EqualityComparer<Exception>.Default.GetHashCode(Error);
	}

	[CompilerGenerated]
	public bool Equals(BindingValue<T> other)
	{
		if (EqualityComparer<T>.Default.Equals(_value, other._value) && EqualityComparer<BindingValueType>.Default.Equals(Type, other.Type))
		{
			return EqualityComparer<Exception>.Default.Equals(Error, other.Error);
		}
		return false;
	}
}
