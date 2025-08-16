using System;

namespace Avalonia.Styling;

internal class NestingSelector : Selector
{
	internal override bool InTemplate => false;

	internal override bool IsCombinator => false;

	internal override Type? TargetType => null;

	public override string ToString(Style? owner)
	{
		return owner?.Parent?.ToString() ?? "^";
	}

	private protected override SelectorMatch Evaluate(StyledElement control, IStyle? parent, bool subscribe)
	{
		if (parent is Style { Selector: not null } style)
		{
			return style.Selector.Match(control, style.Parent, subscribe);
		}
		if (parent is ControlTheme controlTheme)
		{
			if ((object)controlTheme.TargetType == null)
			{
				throw new InvalidOperationException("ControlTheme has no TargetType.");
			}
			if (!controlTheme.TargetType.IsAssignableFrom(StyledElement.GetStyleKey(control)))
			{
				return SelectorMatch.NeverThisType;
			}
			return SelectorMatch.AlwaysThisType;
		}
		throw new InvalidOperationException("Nesting selector was specified but cannot determine parent selector.");
	}

	private protected override Selector? MovePrevious()
	{
		return null;
	}

	private protected override Selector? MovePreviousOrParent()
	{
		return null;
	}
}
