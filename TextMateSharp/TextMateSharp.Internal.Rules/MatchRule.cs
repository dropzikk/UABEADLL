using System.Collections.Generic;

namespace TextMateSharp.Internal.Rules;

public class MatchRule : Rule
{
	private RegExpSource _match;

	private RegExpSourceList _cachedCompiledPatterns;

	public List<CaptureRule> Captures { get; private set; }

	public MatchRule(RuleId id, string name, string match, List<CaptureRule> captures)
		: base(id, name, null)
	{
		_match = new RegExpSource(match, base.Id);
		Captures = captures;
		_cachedCompiledPatterns = null;
	}

	public override void CollectPatternsRecursive(IRuleRegistry grammar, RegExpSourceList sourceList, bool isFirst)
	{
		sourceList.Push(_match);
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
