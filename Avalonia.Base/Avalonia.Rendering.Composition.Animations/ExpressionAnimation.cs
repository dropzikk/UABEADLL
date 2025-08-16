using System;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Server;

namespace Avalonia.Rendering.Composition.Animations;

public class ExpressionAnimation : CompositionAnimation
{
	private string? _expression;

	private Expression? _parsedExpression;

	public string? Expression
	{
		get
		{
			return _expression;
		}
		set
		{
			_expression = value;
			_parsedExpression = null;
		}
	}

	private Expression ParsedExpression => _parsedExpression ?? (_parsedExpression = ExpressionParser.Parse(_expression.AsSpan()));

	internal ExpressionAnimation(Compositor compositor)
		: base(compositor)
	{
	}

	internal override IAnimationInstance CreateInstance(ServerObject targetObject, ExpressionVariant? finalValue)
	{
		return new ExpressionAnimationInstance(ParsedExpression, targetObject, finalValue, CreateSnapshot());
	}
}
