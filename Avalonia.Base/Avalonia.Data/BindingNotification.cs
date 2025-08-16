using System;

namespace Avalonia.Data;

public class BindingNotification
{
	public static readonly BindingNotification Null = new BindingNotification(null);

	public static readonly BindingNotification UnsetValue = new BindingNotification(AvaloniaProperty.UnsetValue);

	private object? _value;

	public object? Value => _value;

	public bool HasValue => _value != AvaloniaProperty.UnsetValue;

	public Exception? Error { get; set; }

	public BindingErrorType ErrorType { get; set; }

	public BindingNotification(object? value)
	{
		_value = value;
	}

	public BindingNotification(Exception error, BindingErrorType errorType)
	{
		if (errorType == BindingErrorType.None)
		{
			throw new ArgumentException("'errorType' may not be None");
		}
		Error = error;
		ErrorType = errorType;
		_value = AvaloniaProperty.UnsetValue;
	}

	public BindingNotification(Exception error, BindingErrorType errorType, object? fallbackValue)
		: this(error, errorType)
	{
		_value = fallbackValue;
	}

	public static bool operator ==(BindingNotification? a, BindingNotification? b)
	{
		if ((object)a == b)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		if (a.HasValue == b.HasValue && a.ErrorType == b.ErrorType && (!a.HasValue || object.Equals(a.Value, b.Value)))
		{
			if (a.ErrorType != 0)
			{
				return ExceptionEquals(a.Error, b.Error);
			}
			return true;
		}
		return false;
	}

	public static bool operator !=(BindingNotification? a, BindingNotification? b)
	{
		return !(a == b);
	}

	public static object? ExtractValue(object? o)
	{
		if (!(o is BindingNotification bindingNotification))
		{
			return o;
		}
		return bindingNotification.Value;
	}

	public static object? ExtractError(object? o)
	{
		if (!(o is BindingNotification bindingNotification))
		{
			return o;
		}
		return bindingNotification.Error;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as BindingNotification);
	}

	public bool Equals(BindingNotification? other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public void AddError(Exception e, BindingErrorType type)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		if (type == BindingErrorType.None)
		{
			throw new ArgumentException("BindingErrorType may not be None", "type");
		}
		Error = ((Error != null) ? new AggregateException(Error, e) : e);
		if (type == BindingErrorType.Error || ErrorType == BindingErrorType.Error)
		{
			ErrorType = BindingErrorType.Error;
		}
	}

	public void ClearValue()
	{
		_value = AvaloniaProperty.UnsetValue;
	}

	public void SetValue(object? value)
	{
		_value = value;
	}

	public override string ToString()
	{
		if (ErrorType != 0)
		{
			if (HasValue)
			{
				return $"{{{ErrorType}: {Error}, Fallback: {Value}}}";
			}
			return $"{{{ErrorType}: {Error}}}";
		}
		return $"{{Value: {Value}}}";
	}

	private static bool ExceptionEquals(Exception? a, Exception? b)
	{
		if (a?.GetType() == b?.GetType())
		{
			return a?.Message == b?.Message;
		}
		return false;
	}
}
