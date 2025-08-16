using System;
using Avalonia.Styling.Activators;

namespace Avalonia.Styling;

internal class NotSelector : Selector
{
	private readonly Selector? _previous;

	private readonly Selector _argument;

	private string? _selectorString;

	internal override bool InTemplate => _argument.InTemplate;

	internal override bool IsCombinator => false;

	internal override Type? TargetType => _previous?.TargetType;

	public NotSelector(Selector? previous, Selector argument)
	{
		_previous = previous;
		_argument = argument ?? throw new InvalidOperationException("Not selector must have a selector argument.");
	}

	public override string ToString(Style? owner)
	{
		if (_selectorString == null)
		{
			_selectorString = $"{_previous?.ToString(owner)}:not({_argument})";
		}
		return _selectorString;
	}

	private protected override SelectorMatch Evaluate(StyledElement control, IStyle? parent, bool subscribe)
	{
		SelectorMatch selectorMatch = _argument.Match(control, parent, subscribe);
		return selectorMatch.Result switch
		{
			SelectorMatchResult.AlwaysThisInstance => SelectorMatch.NeverThisInstance, 
			SelectorMatchResult.AlwaysThisType => SelectorMatch.NeverThisType, 
			SelectorMatchResult.NeverThisInstance => SelectorMatch.AlwaysThisInstance, 
			SelectorMatchResult.NeverThisType => SelectorMatch.AlwaysThisType, 
			SelectorMatchResult.Sometimes => new SelectorMatch(new NotActivator(selectorMatch.Activator)), 
			_ => throw new InvalidOperationException("Invalid SelectorMatchResult."), 
		};
	}

	private protected override Selector? MovePrevious()
	{
		return _previous;
	}

	private protected override Selector? MovePreviousOrParent()
	{
		return _previous;
	}
}
