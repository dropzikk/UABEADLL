using System;
using Avalonia.Styling.Activators;

namespace Avalonia.Styling;

public abstract class Selector
{
	internal abstract bool InTemplate { get; }

	internal abstract bool IsCombinator { get; }

	internal abstract Type? TargetType { get; }

	internal SelectorMatch Match(StyledElement control, IStyle? parent = null, bool subscribe = true)
	{
		SelectorMatch result = MatchUntilCombinator(control, this, parent, subscribe, out Selector combinator);
		if (result.IsMatch && combinator != null)
		{
			result = result.And(combinator.Match(control, parent, subscribe));
			result = result.Result switch
			{
				SelectorMatchResult.AlwaysThisType => SelectorMatch.AlwaysThisInstance, 
				SelectorMatchResult.NeverThisType => SelectorMatch.NeverThisInstance, 
				_ => result, 
			};
		}
		return result;
	}

	public override string ToString()
	{
		return ToString(null);
	}

	public abstract string ToString(Style? owner);

	private protected abstract SelectorMatch Evaluate(StyledElement control, IStyle? parent, bool subscribe);

	private protected abstract Selector? MovePrevious();

	private protected abstract Selector? MovePreviousOrParent();

	internal virtual void ValidateNestingSelector(bool inControlTheme)
	{
		Selector selector = this;
		int num = 0;
		do
		{
			if (inControlTheme)
			{
				if (!selector.InTemplate && selector.IsCombinator)
				{
					throw new InvalidOperationException("ControlTheme style may not directly contain a child or descendent selector.");
				}
				if (selector is TemplateSelector && num++ > 0)
				{
					throw new InvalidOperationException("ControlTemplate styles cannot contain multiple template selectors.");
				}
			}
			Selector? selector2 = selector.MovePreviousOrParent();
			if (selector2 == null && !(selector is NestingSelector))
			{
				throw new InvalidOperationException("Child styles must have a nesting selector.");
			}
			selector = selector2;
		}
		while (selector != null);
	}

	private static SelectorMatch MatchUntilCombinator(StyledElement control, Selector start, IStyle? parent, bool subscribe, out Selector? combinator)
	{
		combinator = null;
		AndActivatorBuilder activators = default(AndActivatorBuilder);
		SelectorMatchResult selectorMatchResult = Match(control, start, parent, subscribe, ref activators, ref combinator);
		if (selectorMatchResult != SelectorMatchResult.Sometimes)
		{
			return new SelectorMatch(selectorMatchResult);
		}
		return new SelectorMatch(activators.Get());
	}

	private static SelectorMatchResult Match(StyledElement control, Selector selector, IStyle? parent, bool subscribe, ref AndActivatorBuilder activators, ref Selector? combinator)
	{
		Selector selector2 = selector.MovePrevious();
		if (selector2 != null && !selector2.IsCombinator)
		{
			SelectorMatchResult selectorMatchResult = Match(control, selector2, parent, subscribe, ref activators, ref combinator);
			if (selectorMatchResult < SelectorMatchResult.Sometimes)
			{
				return selectorMatchResult;
			}
		}
		SelectorMatch selectorMatch = selector.Evaluate(control, parent, subscribe);
		if (!selectorMatch.IsMatch)
		{
			combinator = null;
			return selectorMatch.Result;
		}
		if (selectorMatch.Activator != null)
		{
			activators.Add(selectorMatch.Activator);
		}
		if (selector2 != null && selector2.IsCombinator)
		{
			combinator = selector2;
		}
		return selectorMatch.Result;
	}
}
