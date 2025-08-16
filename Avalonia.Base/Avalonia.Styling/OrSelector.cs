using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Styling.Activators;

namespace Avalonia.Styling;

internal sealed class OrSelector : Selector
{
	private readonly IReadOnlyList<Selector> _selectors;

	private string? _selectorString;

	private Type? _targetType;

	internal override bool InTemplate => false;

	internal override bool IsCombinator => false;

	internal override Type? TargetType => _targetType ?? (_targetType = EvaluateTargetType());

	public OrSelector(IReadOnlyList<Selector> selectors)
	{
		if (selectors == null)
		{
			throw new ArgumentNullException("selectors");
		}
		if (selectors.Count <= 1)
		{
			throw new ArgumentException("Need more than one selector to OR.");
		}
		_selectors = selectors;
	}

	public override string ToString(Style? owner)
	{
		if (_selectorString == null)
		{
			_selectorString = string.Join(", ", _selectors.Select((Selector x) => x.ToString(owner)));
		}
		return _selectorString;
	}

	private protected override SelectorMatch Evaluate(StyledElement control, IStyle? parent, bool subscribe)
	{
		OrActivatorBuilder orActivatorBuilder = default(OrActivatorBuilder);
		bool flag = false;
		int count = _selectors.Count;
		for (int i = 0; i < count; i++)
		{
			SelectorMatch result = _selectors[i].Match(control, parent, subscribe);
			switch (result.Result)
			{
			case SelectorMatchResult.AlwaysThisInstance:
			case SelectorMatchResult.AlwaysThisType:
				return result;
			case SelectorMatchResult.NeverThisInstance:
				flag = true;
				break;
			case SelectorMatchResult.Sometimes:
				orActivatorBuilder.Add(result.Activator);
				break;
			}
		}
		if (orActivatorBuilder.Count > 0)
		{
			return new SelectorMatch(orActivatorBuilder.Get());
		}
		if (flag)
		{
			return SelectorMatch.NeverThisInstance;
		}
		return SelectorMatch.NeverThisType;
	}

	private protected override Selector? MovePrevious()
	{
		return null;
	}

	private protected override Selector? MovePreviousOrParent()
	{
		return null;
	}

	internal override void ValidateNestingSelector(bool inControlTheme)
	{
		int count = _selectors.Count;
		for (int i = 0; i < count; i++)
		{
			_selectors[i].ValidateNestingSelector(inControlTheme);
		}
	}

	private Type? EvaluateTargetType()
	{
		Type type = null;
		int count = _selectors.Count;
		for (int i = 0; i < count; i++)
		{
			Selector selector = _selectors[i];
			if (selector.TargetType == null)
			{
				return null;
			}
			if (type == null)
			{
				type = selector.TargetType;
				continue;
			}
			while ((object)type != null && !type.IsAssignableFrom(selector.TargetType))
			{
				type = type.BaseType;
			}
		}
		return type;
	}
}
