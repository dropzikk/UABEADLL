using System;
using System.Collections.Generic;
using TextMateSharp.Internal.Matcher;

namespace TextMateSharp.Internal.Grammars;

public class BalancedBracketSelectors
{
	private Predicate<List<string>>[] _balancedBracketScopes;

	private Predicate<List<string>>[] _unbalancedBracketScopes;

	private bool _allowAny;

	public BalancedBracketSelectors(List<string> balancedBracketScopes, List<string> unbalancedBracketScopes)
	{
		_balancedBracketScopes = CreateBalancedBracketScopes(balancedBracketScopes);
		_unbalancedBracketScopes = CreateUnbalancedBracketScopes(unbalancedBracketScopes);
	}

	public bool MatchesAlways()
	{
		if (_allowAny)
		{
			return _unbalancedBracketScopes.Length == 0;
		}
		return false;
	}

	public bool MatchesNever()
	{
		if (!_allowAny)
		{
			return _balancedBracketScopes.Length == 0;
		}
		return false;
	}

	public bool Match(List<string> scopes)
	{
		Predicate<List<string>>[] unbalancedBracketScopes = _unbalancedBracketScopes;
		for (int i = 0; i < unbalancedBracketScopes.Length; i++)
		{
			if (unbalancedBracketScopes[i](scopes))
			{
				return false;
			}
		}
		unbalancedBracketScopes = _balancedBracketScopes;
		for (int i = 0; i < unbalancedBracketScopes.Length; i++)
		{
			if (unbalancedBracketScopes[i](scopes))
			{
				return true;
			}
		}
		return _allowAny;
	}

	private Predicate<List<string>>[] CreateBalancedBracketScopes(List<string> balancedBracketScopes)
	{
		List<Predicate<List<string>>> result = new List<Predicate<List<string>>>();
		foreach (string selector in balancedBracketScopes)
		{
			if ("*".Equals(selector))
			{
				_allowAny = true;
				return new Predicate<List<string>>[0];
			}
			foreach (MatcherWithPriority<List<string>> matches in TextMateSharp.Internal.Matcher.Matcher.CreateMatchers(selector))
			{
				result.Add(matches.Matcher);
			}
		}
		return result.ToArray();
	}

	private Predicate<List<string>>[] CreateUnbalancedBracketScopes(List<string> unbalancedBracketScopes)
	{
		List<Predicate<List<string>>> result = new List<Predicate<List<string>>>();
		foreach (string unbalancedBracketScope in unbalancedBracketScopes)
		{
			foreach (MatcherWithPriority<List<string>> matches in TextMateSharp.Internal.Matcher.Matcher.CreateMatchers(unbalancedBracketScope))
			{
				result.Add(matches.Matcher);
			}
		}
		return result.ToArray();
	}
}
