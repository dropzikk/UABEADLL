using System.Collections.Generic;
using TextMateSharp.Internal.Oniguruma;

namespace TextMateSharp.Internal.Rules;

public class BeginWhileRule : Rule
{
	private RegExpSource _begin;

	private RegExpSource _while;

	private RegExpSourceList _cachedCompiledPatterns;

	private RegExpSourceList _cachedCompiledWhilePatterns;

	public List<CaptureRule> BeginCaptures { get; private set; }

	public List<CaptureRule> WhileCaptures { get; private set; }

	public bool WhileHasBackReferences { get; private set; }

	public bool HasMissingPatterns { get; private set; }

	public IList<RuleId> Patterns { get; private set; }

	public BeginWhileRule(RuleId id, string name, string contentName, string begin, List<CaptureRule> beginCaptures, string whileStr, List<CaptureRule> whileCaptures, CompilePatternsResult patterns)
		: base(id, name, contentName)
	{
		_begin = new RegExpSource(begin, base.Id);
		_while = new RegExpSource(whileStr, RuleId.WHILE_RULE);
		BeginCaptures = beginCaptures;
		WhileCaptures = whileCaptures;
		WhileHasBackReferences = _while.HasBackReferences();
		Patterns = patterns.Patterns;
		HasMissingPatterns = patterns.HasMissingPatterns;
		_cachedCompiledPatterns = null;
		_cachedCompiledWhilePatterns = null;
	}

	public string getWhileWithResolvedBackReferences(string lineText, IOnigCaptureIndex[] captureIndices)
	{
		return _while.ResolveBackReferences(lineText, captureIndices);
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
		Precompile(grammar);
		return _cachedCompiledPatterns.Compile(allowA, allowG);
	}

	private void Precompile(IRuleRegistry grammar)
	{
		if (_cachedCompiledPatterns == null)
		{
			_cachedCompiledPatterns = new RegExpSourceList();
			CollectPatternsRecursive(grammar, _cachedCompiledPatterns, isFirst: true);
		}
	}

	public CompiledRule CompileWhile(string endRegexSource, bool allowA, bool allowG)
	{
		PrecompileWhile();
		if (_while.HasBackReferences())
		{
			_cachedCompiledWhilePatterns.SetSource(0, endRegexSource);
		}
		return _cachedCompiledWhilePatterns.Compile(allowA, allowG);
	}

	private void PrecompileWhile()
	{
		if (_cachedCompiledWhilePatterns == null)
		{
			_cachedCompiledWhilePatterns = new RegExpSourceList();
			_cachedCompiledWhilePatterns.Push(_while.HasBackReferences() ? _while.Clone() : _while);
		}
	}
}
