using System;
using System.Collections.Generic;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Server;

namespace Avalonia.Rendering.Composition.Animations;

internal class ExpressionAnimationInstance : AnimationInstanceBase, IAnimationInstance, IServerClockItem
{
	private readonly Expression _expression;

	private ExpressionVariant _startingValue;

	private readonly ExpressionVariant? _finalValue;

	protected override ExpressionVariant EvaluateCore(TimeSpan now, ExpressionVariant currentValue)
	{
		ExpressionEvaluationContext expressionEvaluationContext = default(ExpressionEvaluationContext);
		expressionEvaluationContext.Parameters = base.Parameters;
		expressionEvaluationContext.Target = base.TargetObject;
		expressionEvaluationContext.ForeignFunctionInterface = BuiltInExpressionFfi.Instance;
		expressionEvaluationContext.StartingValue = _startingValue;
		expressionEvaluationContext.FinalValue = _finalValue ?? _startingValue;
		expressionEvaluationContext.CurrentValue = currentValue;
		ExpressionEvaluationContext context = expressionEvaluationContext;
		return _expression.Evaluate(ref context);
	}

	public override void Initialize(TimeSpan startedAt, ExpressionVariant startingValue, CompositionProperty property)
	{
		_startingValue = startingValue;
		HashSet<(string, string)> hashSet = new HashSet<(string, string)>();
		_expression.CollectReferences(hashSet);
		Initialize(property, hashSet);
	}

	public ExpressionAnimationInstance(Expression expression, ServerObject target, ExpressionVariant? finalValue, PropertySetSnapshot parameters)
		: base(target, parameters)
	{
		_expression = expression;
		_finalValue = finalValue;
	}
}
