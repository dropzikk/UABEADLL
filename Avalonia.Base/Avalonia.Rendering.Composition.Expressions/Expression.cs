using System;
using System.Collections.Generic;
using System.Reflection;

namespace Avalonia.Rendering.Composition.Expressions;

internal abstract class Expression
{
	public abstract ExpressionType Type { get; }

	public static Expression Parse(string expression)
	{
		return ExpressionParser.Parse(expression.AsSpan());
	}

	public abstract ExpressionVariant Evaluate(ref ExpressionEvaluationContext context);

	public virtual void CollectReferences(HashSet<(string parameter, string property)> references)
	{
	}

	protected abstract string Print();

	public override string ToString()
	{
		return Print();
	}

	internal static string OperatorName(ExpressionType t)
	{
		PrettyPrintStringAttribute customAttribute = typeof(ExpressionType).GetMember(t.ToString())[0].GetCustomAttribute<PrettyPrintStringAttribute>();
		if (customAttribute != null)
		{
			return customAttribute.Name;
		}
		return t.ToString();
	}
}
