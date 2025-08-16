using System;
using Avalonia.LogicalTree;

namespace Avalonia.Styling;

internal class ChildSelector : Selector
{
	private readonly Selector _parent;

	private string? _selectorString;

	internal override bool InTemplate => _parent.InTemplate;

	internal override bool IsCombinator => true;

	internal override Type? TargetType => null;

	public ChildSelector(Selector parent)
	{
		if (parent == null)
		{
			throw new InvalidOperationException("Child selector must be preceeded by a selector.");
		}
		_parent = parent;
	}

	public override string ToString(Style? owner)
	{
		if (_selectorString == null)
		{
			_selectorString = _parent.ToString(owner) + " > ";
		}
		return _selectorString;
	}

	private protected override SelectorMatch Evaluate(StyledElement control, IStyle? parent, bool subscribe)
	{
		ILogical logicalParent = ((ILogical)control).LogicalParent;
		if (logicalParent != null)
		{
			SelectorMatch result = _parent.Match((StyledElement)logicalParent, parent, subscribe);
			if (result.Result == SelectorMatchResult.Sometimes)
			{
				return result;
			}
			if (result.IsMatch)
			{
				return SelectorMatch.AlwaysThisInstance;
			}
			return SelectorMatch.NeverThisInstance;
		}
		return SelectorMatch.NeverThisInstance;
	}

	private protected override Selector? MovePrevious()
	{
		return null;
	}

	private protected override Selector? MovePreviousOrParent()
	{
		return _parent;
	}
}
