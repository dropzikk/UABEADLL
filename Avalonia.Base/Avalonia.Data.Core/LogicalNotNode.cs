using System;
using System.Globalization;

namespace Avalonia.Data.Core;

internal class LogicalNotNode : ExpressionNode, ITransformNode
{
	public override string Description => "!";

	protected override void NextValueChanged(object? value)
	{
		base.NextValueChanged(Negate(value));
	}

	private static object Negate(object? value)
	{
		BindingNotification notification = value as BindingNotification;
		object obj = BindingNotification.ExtractValue(value);
		if (obj != AvaloniaProperty.UnsetValue)
		{
			if (obj is string text)
			{
				if (bool.TryParse(text, out var result))
				{
					return !result;
				}
				return GenerateError(new InvalidCastException("Unable to convert '" + text + "' to bool."));
			}
			try
			{
				bool flag = Convert.ToBoolean(obj, CultureInfo.InvariantCulture);
				if ((object)notification != null)
				{
					notification.SetValue(!flag);
					return notification;
				}
				return !flag;
			}
			catch (InvalidCastException)
			{
				return GenerateError(new InvalidCastException($"Unable to convert '{obj}' to bool."));
			}
			catch (Exception e2)
			{
				return GenerateError(e2);
			}
		}
		return notification ?? AvaloniaProperty.UnsetValue;
		BindingNotification GenerateError(Exception e)
		{
			if ((object)notification == null)
			{
				notification = new BindingNotification(AvaloniaProperty.UnsetValue);
			}
			notification.AddError(e, BindingErrorType.Error);
			notification.ClearValue();
			return notification;
		}
	}

	public object? Transform(object? value)
	{
		if (value == null)
		{
			return null;
		}
		Type type = value.GetType();
		object obj = Negate(value);
		if (obj is BindingNotification)
		{
			return obj;
		}
		return Convert.ChangeType(obj, type);
	}
}
