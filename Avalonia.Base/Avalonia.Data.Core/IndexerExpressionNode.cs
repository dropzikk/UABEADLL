using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Avalonia.Data.Core;

internal class IndexerExpressionNode : IndexerNodeBase
{
	private readonly ParameterExpression _parameter;

	private readonly IndexExpression _expression;

	private readonly Delegate _setDelegate;

	private readonly Delegate _getDelegate;

	private readonly Delegate _firstArgumentDelegate;

	public override Type PropertyType => _expression.Type;

	public override string Description => _expression.ToString();

	public IndexerExpressionNode(IndexExpression expression)
	{
		_parameter = Expression.Parameter(expression.Object.Type);
		_expression = expression.Update(_parameter, expression.Arguments);
		_getDelegate = Expression.Lambda(_expression, _parameter).Compile();
		ParameterExpression parameterExpression = Expression.Parameter(expression.Type);
		_setDelegate = Expression.Lambda(Expression.Assign(_expression, parameterExpression), _parameter, parameterExpression).Compile();
		_firstArgumentDelegate = Expression.Lambda(_expression.Arguments[0], _parameter).Compile();
	}

	protected override bool SetTargetValueCore(object? value, BindingPriority priority)
	{
		try
		{
			base.Target.TryGetTarget(out object target);
			_setDelegate.DynamicInvoke(target, value);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	protected override object? GetValue(object? target)
	{
		try
		{
			return _getDelegate.DynamicInvoke(target);
		}
		catch (TargetInvocationException ex) when (ex.InnerException is ArgumentOutOfRangeException || ex.InnerException is IndexOutOfRangeException || ex.InnerException is KeyNotFoundException)
		{
			return AvaloniaProperty.UnsetValue;
		}
	}

	protected override bool ShouldUpdate(object? sender, PropertyChangedEventArgs e)
	{
		if (!(_expression.Indexer == null))
		{
			return _expression.Indexer.Name == e.PropertyName;
		}
		return true;
	}

	protected override int? TryGetFirstArgumentAsInt()
	{
		base.Target.TryGetTarget(out object target);
		return _firstArgumentDelegate.DynamicInvoke(target) as int?;
	}
}
