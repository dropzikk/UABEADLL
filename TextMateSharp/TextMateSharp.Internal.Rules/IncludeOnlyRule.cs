using System.Collections.Generic;

namespace TextMateSharp.Internal.Rules;

public class IncludeOnlyRule : Rule
{
	private RegExpSourceList _cachedCompiledPatterns;

	public bool HasMissingPatterns { get; private set; }

	public IList<RuleId> Patterns { get; private set; }

	public IncludeOnlyRule(RuleId id, string name, string contentName, CompilePatternsResult patterns)
		: base(id, name, contentName)
	{
		Patterns = patterns.Patterns;
		HasMissingPatterns = patterns.HasMissingPatterns;
		_cachedCompiledPatterns = null;
	}

	public override void CollectPatternsRecursive(IRuleRegistry grammar, RegExpSourceList sourceList, bool isFirst)
	{
		foreach (RuleId pattern in Patterns)
		{
			grammar.GetRule(pattern).CollectPatternsRecursive(grammar, sourceList, isFirst: false);
		}
	}

	public override CompiledRule Compile(IRuleRegistry grammar, string endRegexSource, bool allowA, bool allowG)
	{
		if (_cachedCompiledPatterns == null)
		{
			_cachedCompiledPatterns = new RegExpSourceList();
			CollectPatternsRecursive(grammar, _cachedCompiledPatterns, isFirst: true);
		}
		return _cachedCompiledPatterns.Compile(allowA, allowG);
	}
}
