using System.Collections.Generic;
using TextMateSharp.Internal.Oniguruma;

namespace TextMateSharp.Internal.Rules;

public class BeginEndRule : Rule
{
	private RegExpSource _begin;

	private RegExpSource _end;

	private RegExpSourceList _cachedCompiledPatterns;

	public List<CaptureRule> BeginCaptures { get; private set; }

	public bool EndHasBackReferences { get; private set; }

	public List<CaptureRule> EndCaptures { get; private set; }

	public bool ApplyEndPatternLast { get; private set; }

	public bool HasMissingPatterns { get; private set; }

	public IList<RuleId> Patterns { get; private set; }

	public BeginEndRule(RuleId id, string name, string contentName, string begin, List<CaptureRule> beginCaptures, string end, List<CaptureRule> endCaptures, bool applyEndPatternLast, CompilePatternsResult patterns)
		: base(id, name, contentName)
	{
		_begin = new RegExpSource(begin, base.Id);
		_end = new RegExpSource(end, RuleId.END_RULE);
		BeginCaptures = beginCaptures;
		EndHasBackReferences = _end.HasBackReferences();
		EndCaptures = endCaptures;
		ApplyEndPatternLast = applyEndPatternLast;
		Patterns = patterns.Patterns;
		HasMissingPatterns = patterns.HasMissingPatterns;
		_cachedCompiledPatterns = null;
	}

	public string GetEndWithResolvedBackReferences(string lineText, IOnigCaptureIndex[] captureIndices)
	{
		return _end.ResolveBackReferences(lineText, captureIndices);
	}

	public override void CollectPatternsRecursive(IRuleRegistry grammar, RegExpSourceList sourceList, bool isFirst)
	{
		if (isFirst)
		{
			foreach (RuleId pattern in Patterns)
			{
				grammar.GetRule(pattern).CollectPatternsRecursive(grammar, sourceList, isFirst: false);
			}
			return;
		}
		sourceList.Push(_begin);
	}

	public override CompiledRule Compile(IRuleRegistry grammar, string endRegexSource, bool allowA, bool allowG)
	{
		RegExpSourceList precompiled = Precompile(grammar);
		if (_end.HasBackReferences())
		{
			if (ApplyEndPatternLast)
			{
				precompiled.SetSource(precompiled.Length() - 1, endRegexSource);
			}
			else
			{
				precompiled.SetSource(0, endRegexSource);
			}
		}
		return _cachedCompiledPatterns.Compile(allowA, allowG);
	}

	private RegExpSourceList Precompile(IRuleRegistry grammar)
	{
		if (_cachedCompiledPatterns == null)
		{
			_cachedCompiledPatterns = new RegExpSourceList();
			CollectPatternsRecursive(grammar, _cachedCompiledPatterns, isFirst: true);
			if (ApplyEndPatternLast)
			{
				_cachedCompiledPatterns.Push(_end.HasBackReferences() ? _end.Clone() : _end);
			}
			else
			{
				_cachedCompiledPatterns.UnShift(_end.HasBackReferences() ? _end.Clone() : _end);
			}
		}
		return _cachedCompiledPatterns;
	}
}
