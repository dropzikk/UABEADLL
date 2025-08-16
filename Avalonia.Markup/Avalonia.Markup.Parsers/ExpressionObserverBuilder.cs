using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Data.Core;
using Avalonia.Utilities;

namespace Avalonia.Markup.Parsers;

internal static class ExpressionObserverBuilder
{
	[RequiresUnreferencedCode("BindingExpression and ReflectionBinding heavily use reflection. Consider using CompiledBindings instead.")]
	internal static (ExpressionNode Node, SourceMode Mode) Parse(string expression, bool enableValidation = false, Func<string?, string, Type>? typeResolver = null, INameScope? nameScope = null)
	{
		if (string.IsNullOrWhiteSpace(expression))
		{
			return (Node: new EmptyExpressionNode(), Mode: SourceMode.Data);
		}
		CharacterReader r = new CharacterReader(expression.AsSpan());
		(ExpressionNode Node, SourceMode Mode) result = new ExpressionParser(enableValidation, typeResolver, nameScope).Parse(ref r);
		if (!r.End)
		{
			throw new ExpressionParseException(r.Position, "Expected end of expression.");
		}
		return result;
	}

	[RequiresUnreferencedCode("BindingExpression and ReflectionBinding heavily use reflection. Consider using CompiledBindings instead.")]
	public static ExpressionObserver Build(object root, string expression, bool enableDataValidation = false, string? description = null, Func<string?, string, Type>? typeResolver = null)
	{
		return new ExpressionObserver(root, Parse(expression, enableDataValidation, typeResolver).Node, description ?? expression);
	}

	[RequiresUnreferencedCode("BindingExpression and ReflectionBinding heavily use reflection. Consider using CompiledBindings instead.")]
	public static ExpressionObserver Build(IObservable<object> rootObservable, string expression, bool enableDataValidation = false, string? description = null, Func<string?, string, Type>? typeResolver = null)
	{
		if (rootObservable == null)
		{
			throw new ArgumentNullException("rootObservable");
		}
		return new ExpressionObserver(rootObservable, Parse(expression, enableDataValidation, typeResolver).Node, description ?? expression);
	}

	[RequiresUnreferencedCode("BindingExpression and ReflectionBinding heavily use reflection. Consider using CompiledBindings instead.")]
	public static ExpressionObserver Build(Func<object> rootGetter, string expression, IObservable<ValueTuple> update, bool enableDataValidation = false, string? description = null, Func<string?, string, Type>? typeResolver = null)
	{
		if (rootGetter == null)
		{
			throw new ArgumentNullException("rootGetter");
		}
		return new ExpressionObserver(rootGetter, Parse(expression, enableDataValidation, typeResolver).Node, update, description ?? expression);
	}
}
