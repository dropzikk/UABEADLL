using System;

namespace Avalonia.Controls;

public class CalendarDatePickerDateValidationErrorEventArgs : EventArgs
{
	private bool _throwException;

	public Exception Exception { get; private set; }

	public string Text { get; private set; }

	public bool ThrowException
	{
		get
		{
			return _throwException;
		}
		set
		{
			if (value && Exception == null)
			{
				throw new ArgumentException("Cannot Throw Null Exception");
			}
			_throwException = value;
		}
	}

	public CalendarDatePickerDateValidationErrorEventArgs(Exception exception, string text)
	{
		Text = text;
		Exception = exception;
	}
}
