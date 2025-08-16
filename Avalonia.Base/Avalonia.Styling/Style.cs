using System;
using Avalonia.PropertyStore;

namespace Avalonia.Styling;

public class Style : StyleBase
{
	private Selector? _selector;

	public Selector? Selector
	{
		get
		{
			return _selector;
		}
		set
		{
			_selector = ValidateSelector(value);
		}
	}

	public Style()
	{
	}

	public Style(Func<Selector?, Selector> selector)
	{
		Selector = selector(null);
	}

	public override string ToString()
	{
		return Selector?.ToString(this) ?? "Style";
	}

	internal override void SetParent(StyleBase? parent)
	{
		if (parent is Style { Selector: not null })
		{
			if (Selector == null)
			{
				throw new InvalidOperationException("Child styles must have a selector.");
			}
			Selector.ValidateNestingSelector(inControlTheme: false);
		}
		else if (parent is ControlTheme)
		{
			if (Selector == null)
			{
				throw new InvalidOperationException("Child styles must have a selector.");
			}
			Selector.ValidateNestingSelector(inControlTheme: true);
		}
		base.SetParent(parent);
	}

	internal SelectorMatchResult TryAttach(StyledElement target, object? host, FrameType type)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		SelectorMatchResult result = SelectorMatchResult.NeverThisType;
		if (base.HasSettersOrAnimations)
		{
			SelectorMatch selectorMatch = Selector?.Match(target, base.Parent) ?? ((target == host) ? SelectorMatch.AlwaysThisInstance : SelectorMatch.NeverThisInstance);
			if (selectorMatch.IsMatch)
			{
				Attach(target, selectorMatch.Activator, type);
			}
			result = selectorMatch.Result;
		}
		return result;
	}

	private static Selector? ValidateSelector(Selector? selector)
	{
		if (selector is TemplateSelector)
		{
			throw new InvalidOperationException("Invalid selector: Template selector must be followed by control selector.");
		}
		return selector;
	}
}
