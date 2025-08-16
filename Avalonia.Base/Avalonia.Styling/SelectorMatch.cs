using System;
using Avalonia.Styling.Activators;

namespace Avalonia.Styling;

internal readonly record struct SelectorMatch(SelectorMatchResult result)
{
	public bool IsMatch => Result >= SelectorMatchResult.Sometimes;

	public IStyleActivator? Activator { get; }

	public static readonly SelectorMatch NeverThisType = new SelectorMatch(SelectorMatchResult.NeverThisType);

	public static readonly SelectorMatch NeverThisInstance = new SelectorMatch(SelectorMatchResult.NeverThisInstance);

	public static readonly SelectorMatch AlwaysThisType = new SelectorMatch(SelectorMatchResult.AlwaysThisType);

	public static readonly SelectorMatch AlwaysThisInstance = new SelectorMatch(SelectorMatchResult.AlwaysThisInstance);

	public SelectorMatch(IStyleActivator match)
	{
		match = match ?? throw new ArgumentNullException("match");
		Result = SelectorMatchResult.Sometimes;
		Activator = match;
	}

	public SelectorMatch(SelectorMatchResult result)
	{
		Result = result;
		Activator = null;
	}

	public SelectorMatch And(in SelectorMatch other)
	{
		SelectorMatchResult selectorMatchResult = (SelectorMatchResult)Math.Min((int)Result, (int)other.Result);
		if (selectorMatchResult == SelectorMatchResult.Sometimes)
		{
			AndActivatorBuilder andActivatorBuilder = default(AndActivatorBuilder);
			andActivatorBuilder.Add(Activator);
			andActivatorBuilder.Add(other.Activator);
			return new SelectorMatch(andActivatorBuilder.Get());
		}
		return new SelectorMatch(selectorMatchResult);
	}

	public override string ToString()
	{
		return Result.ToString();
	}
}
