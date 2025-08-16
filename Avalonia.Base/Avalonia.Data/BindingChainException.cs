using System;

namespace Avalonia.Data;

public class BindingChainException : Exception
{
	private string _message;

	public string? Expression { get; protected set; }

	public string? ExpressionErrorPoint { get; protected set; }

	public override string Message
	{
		get
		{
			if (Expression != null && ExpressionErrorPoint != null)
			{
				return $"{_message} in expression '{Expression}' at '{ExpressionErrorPoint}'.";
			}
			if (ExpressionErrorPoint != null)
			{
				return _message + " in expression '" + ExpressionErrorPoint + "'.";
			}
			return _message + " in expression.";
		}
	}

	public BindingChainException()
	{
		_message = "Binding error";
	}

	public BindingChainException(string message)
	{
		_message = message;
	}

	public BindingChainException(string message, string expression, string errorPoint)
	{
		_message = message;
		Expression = expression;
		ExpressionErrorPoint = errorPoint;
	}
}
