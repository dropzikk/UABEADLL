using System;
using System.Collections.Generic;
using TextMateSharp.Internal.Rules;
using TextMateSharp.Internal.Types;

namespace TextMateSharp.Grammars;

public class Injection
{
	private Predicate<List<string>> _matcher;

	public int Priority { get; private set; }

	public RuleId RuleId { get; private set; }

	public IRawGrammar Grammar { get; private set; }

	public Injection(Predicate<List<string>> matcher, RuleId ruleId, IRawGrammar grammar, int priority)
	{
		RuleId = ruleId;
		Grammar = grammar;
		Priority = priority;
		_matcher = matcher;
	}

	public bool Match(List<string> states)
	{
		return _matcher(states);
	}
}
