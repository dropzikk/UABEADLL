using System.Collections.Generic;
using TextMateSharp.Internal.Types;
using TextMateSharp.Internal.Utils;

namespace TextMateSharp.Internal.Rules;

public class RuleFactory
{
	public static CaptureRule CreateCaptureRule(IRuleFactoryHelper helper, string name, string contentName, RuleId retokenizeCapturedWithRuleId)
	{
		return (CaptureRule)helper.RegisterRule((RuleId id) => new CaptureRule(id, name, contentName, retokenizeCapturedWithRuleId));
	}

	public static RuleId GetCompiledRuleId(IRawRule desc, IRuleFactoryHelper helper, IRawRepository repository)
	{
		if (desc == null)
		{
			return null;
		}
		if (desc.GetId() == null)
		{
			helper.RegisterRule(delegate(RuleId id)
			{
				desc.SetId(id);
				if (desc.GetMatch() != null)
				{
					return new MatchRule(desc.GetId(), desc.GetName(), desc.GetMatch(), CompileCaptures(desc.GetCaptures(), helper, repository));
				}
				if (desc.GetBegin() == null)
				{
					IRawRepository repository2 = repository;
					if (desc.GetRepository() != null)
					{
						repository2 = repository.Merge(desc.GetRepository());
					}
					return new IncludeOnlyRule(desc.GetId(), desc.GetName(), desc.GetContentName(), CompilePatterns(desc.GetPatterns(), helper, repository2));
				}
				string @while = desc.GetWhile();
				return (@while != null) ? ((Rule)new BeginWhileRule(desc.GetId(), desc.GetName(), desc.GetContentName(), desc.GetBegin(), CompileCaptures((desc.GetBeginCaptures() != null) ? desc.GetBeginCaptures() : desc.GetCaptures(), helper, repository), @while, CompileCaptures((desc.GetWhileCaptures() != null) ? desc.GetWhileCaptures() : desc.GetCaptures(), helper, repository), CompilePatterns(desc.GetPatterns(), helper, repository))) : ((Rule)new BeginEndRule(desc.GetId(), desc.GetName(), desc.GetContentName(), desc.GetBegin(), CompileCaptures((desc.GetBeginCaptures() != null) ? desc.GetBeginCaptures() : desc.GetCaptures(), helper, repository), desc.GetEnd(), CompileCaptures((desc.GetEndCaptures() != null) ? desc.GetEndCaptures() : desc.GetCaptures(), helper, repository), desc.IsApplyEndPatternLast(), CompilePatterns(desc.GetPatterns(), helper, repository)));
			});
		}
		return desc.GetId();
	}

	private static List<CaptureRule> CompileCaptures(IRawCaptures captures, IRuleFactoryHelper helper, IRawRepository repository)
	{
		List<CaptureRule> r = new List<CaptureRule>();
		if (captures != null)
		{
			int maximumCaptureId = 0;
			foreach (string capture in captures)
			{
				int numericCaptureId = ParseInt(capture);
				if (numericCaptureId > maximumCaptureId)
				{
					maximumCaptureId = numericCaptureId;
				}
			}
			for (int i = 0; i <= maximumCaptureId; i++)
			{
				r.Add(null);
			}
			foreach (string captureId in captures)
			{
				int numericCaptureId = ParseInt(captureId);
				RuleId retokenizeCapturedWithRuleId = null;
				IRawRule rule = captures.GetCapture(captureId);
				if (rule.GetPatterns() != null)
				{
					retokenizeCapturedWithRuleId = GetCompiledRuleId(captures.GetCapture(captureId), helper, repository);
				}
				r[numericCaptureId] = CreateCaptureRule(helper, rule.GetName(), rule.GetContentName(), retokenizeCapturedWithRuleId);
			}
		}
		return r;
	}

	private static int ParseInt(string str)
	{
		int result = 0;
		int.TryParse(str, out result);
		return result;
	}

	private static CompilePatternsResult CompilePatterns(ICollection<IRawRule> patterns, IRuleFactoryHelper helper, IRawRepository repository)
	{
		List<RuleId> r = new List<RuleId>();
		if (patterns != null)
		{
			foreach (IRawRule pattern in patterns)
			{
				RuleId patternId = null;
				if (pattern.GetInclude() != null)
				{
					if (pattern.GetInclude()[0] == '#')
					{
						IRawRule localIncludedRule = repository.GetProp(pattern.GetInclude().Substring(1));
						if (localIncludedRule != null)
						{
							patternId = GetCompiledRuleId(localIncludedRule, helper, repository);
						}
					}
					else if (pattern.GetInclude().Equals("$base") || pattern.GetInclude().Equals("$self"))
					{
						patternId = GetCompiledRuleId(repository.GetProp(pattern.GetInclude()), helper, repository);
					}
					else
					{
						string externalGrammarName = null;
						string externalGrammarInclude = null;
						int sharpIndex = pattern.GetInclude().IndexOf('#');
						if (sharpIndex >= 0)
						{
							externalGrammarName = pattern.GetInclude().SubstringAtIndexes(0, sharpIndex);
							externalGrammarInclude = pattern.GetInclude().Substring(sharpIndex + 1);
						}
						else
						{
							externalGrammarName = pattern.GetInclude();
						}
						IRawGrammar externalGrammar = helper.GetExternalGrammar(externalGrammarName, repository);
						if (externalGrammar != null)
						{
							if (externalGrammarInclude != null)
							{
								IRawRule externalIncludedRule = externalGrammar.GetRepository().GetProp(externalGrammarInclude);
								if (externalIncludedRule != null)
								{
									patternId = GetCompiledRuleId(externalIncludedRule, helper, externalGrammar.GetRepository());
								}
							}
							else
							{
								patternId = GetCompiledRuleId(externalGrammar.GetRepository().GetSelf(), helper, externalGrammar.GetRepository());
							}
						}
					}
				}
				else
				{
					patternId = GetCompiledRuleId(pattern, helper, repository);
				}
				if (patternId == null)
				{
					continue;
				}
				Rule rule = helper.GetRule(patternId);
				bool skipRule = false;
				if (rule is IncludeOnlyRule)
				{
					IncludeOnlyRule ior = (IncludeOnlyRule)rule;
					if (ior.HasMissingPatterns && ior.Patterns.Count == 0)
					{
						skipRule = true;
					}
				}
				else if (rule is BeginEndRule)
				{
					BeginEndRule br = (BeginEndRule)rule;
					if (br.HasMissingPatterns && br.Patterns.Count == 0)
					{
						skipRule = true;
					}
				}
				else if (rule is BeginWhileRule)
				{
					BeginWhileRule br2 = (BeginWhileRule)rule;
					if (br2.HasMissingPatterns && br2.Patterns.Count == 0)
					{
						skipRule = true;
					}
				}
				if (!skipRule)
				{
					r.Add(patternId);
				}
			}
		}
		return new CompilePatternsResult(r, (patterns?.Count ?? 0) != r.Count);
	}
}
