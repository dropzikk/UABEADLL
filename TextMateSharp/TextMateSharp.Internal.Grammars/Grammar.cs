using System;
using System.Collections.Generic;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Parser;
using TextMateSharp.Internal.Matcher;
using TextMateSharp.Internal.Rules;
using TextMateSharp.Internal.Types;
using TextMateSharp.Themes;

namespace TextMateSharp.Internal.Grammars;

public class Grammar : IGrammar, IRuleFactoryHelper, IRuleRegistry, IGrammarRegistry
{
	private class GrammarRepository : IGrammarRepository
	{
		private Grammar _grammar;

		internal GrammarRepository(Grammar grammar)
		{
			_grammar = grammar;
		}

		public IRawGrammar Lookup(string scopeName)
		{
			if (scopeName.Equals(_grammar._rootScopeName))
			{
				return _grammar._rawGrammar;
			}
			return _grammar.GetExternalGrammar(scopeName, null);
		}

		public ICollection<string> Injections(string targetScope)
		{
			return _grammar._grammarRepository.Injections(targetScope);
		}
	}

	private string _rootScopeName;

	private RuleId _rootId;

	private int _lastRuleId;

	private volatile bool _isCompiling;

	private Dictionary<RuleId, Rule> _ruleId2desc;

	private Dictionary<string, IRawGrammar> _includedGrammars;

	private IGrammarRepository _grammarRepository;

	private IRawGrammar _rawGrammar;

	private List<Injection> _injections;

	private BasicScopeAttributesProvider _basicScopeAttributesProvider;

	private List<TokenTypeMatcher> _tokenTypeMatchers;

	private BalancedBracketSelectors _balancedBracketSelectors;

	public bool IsCompiling => _isCompiling;

	public Grammar(string scopeName, IRawGrammar grammar, int initialLanguage, Dictionary<string, int> embeddedLanguages, Dictionary<string, int> tokenTypes, BalancedBracketSelectors balancedBracketSelectors, IGrammarRepository grammarRepository, IThemeProvider themeProvider)
	{
		_rootScopeName = scopeName;
		_basicScopeAttributesProvider = new BasicScopeAttributesProvider(initialLanguage, themeProvider, embeddedLanguages);
		_balancedBracketSelectors = balancedBracketSelectors;
		_rootId = null;
		_lastRuleId = 0;
		_includedGrammars = new Dictionary<string, IRawGrammar>();
		_grammarRepository = grammarRepository;
		_rawGrammar = InitGrammar(grammar, null);
		_ruleId2desc = new Dictionary<RuleId, Rule>();
		_injections = null;
		_tokenTypeMatchers = GenerateTokenTypeMatchers(tokenTypes);
	}

	public void OnDidChangeTheme()
	{
		_basicScopeAttributesProvider.OnDidChangeTheme();
	}

	public BasicScopeAttributes GetMetadataForScope(string scope)
	{
		return _basicScopeAttributesProvider.GetBasicScopeAttributes(scope);
	}

	public List<Injection> GetInjections()
	{
		if (_injections == null)
		{
			_injections = new List<Injection>();
			GrammarRepository grammarRepository = new GrammarRepository(this);
			string scopeName = _rootScopeName;
			IRawGrammar grammar = grammarRepository.Lookup(scopeName);
			if (grammar != null)
			{
				Dictionary<string, IRawRule> rawInjections = grammar.GetInjections();
				if (rawInjections != null)
				{
					foreach (string expression in rawInjections.Keys)
					{
						IRawRule rule = rawInjections[expression];
						CollectInjections(_injections, expression, rule, this, grammar);
					}
				}
			}
			ICollection<string> injectionScopeNames = _grammarRepository.Injections(scopeName);
			if (injectionScopeNames != null)
			{
				foreach (string injectionScopeName in injectionScopeNames)
				{
					IRawGrammar injectionGrammar = GetExternalGrammar(injectionScopeName);
					if (injectionGrammar != null)
					{
						string selector = injectionGrammar.GetInjectionSelector();
						if (selector != null)
						{
							CollectInjections(_injections, selector, (IRawRule)injectionGrammar, this, injectionGrammar);
						}
					}
				}
			}
			_injections.Sort((Injection i1, Injection i2) => i1.Priority - i2.Priority);
		}
		return _injections;
	}

	private void CollectInjections(List<Injection> result, string selector, IRawRule rule, IRuleFactoryHelper ruleFactoryHelper, IRawGrammar grammar)
	{
		ICollection<MatcherWithPriority<List<string>>> collection = TextMateSharp.Internal.Matcher.Matcher.CreateMatchers(selector);
		RuleId ruleId = RuleFactory.GetCompiledRuleId(rule, ruleFactoryHelper, grammar.GetRepository());
		foreach (MatcherWithPriority<List<string>> matcher in collection)
		{
			result.Add(new Injection(matcher.Matcher, ruleId, grammar, matcher.Priority));
		}
	}

	public Rule RegisterRule(Func<RuleId, Rule> factory)
	{
		RuleId id = RuleId.Of(++_lastRuleId);
		Rule result = factory(id);
		_ruleId2desc[id] = result;
		return result;
	}

	public Rule GetRule(RuleId patternId)
	{
		_ruleId2desc.TryGetValue(patternId, out var result);
		return result;
	}

	public IRawGrammar GetExternalGrammar(string scopeName)
	{
		return GetExternalGrammar(scopeName, null);
	}

	public IRawGrammar GetExternalGrammar(string scopeName, IRawRepository repository)
	{
		if (_includedGrammars.ContainsKey(scopeName))
		{
			return _includedGrammars[scopeName];
		}
		if (_grammarRepository != null)
		{
			IRawGrammar rawIncludedGrammar = _grammarRepository.Lookup(scopeName);
			if (rawIncludedGrammar != null)
			{
				_includedGrammars[scopeName] = InitGrammar(rawIncludedGrammar, repository?.GetBase());
				return _includedGrammars[scopeName];
			}
		}
		return null;
	}

	private IRawGrammar InitGrammar(IRawGrammar grammar, IRawRule ruleBase)
	{
		grammar = grammar.Clone();
		if (grammar.GetRepository() == null)
		{
			((Raw)grammar).SetRepository(new Raw());
		}
		Raw self = new Raw();
		self.SetPatterns(grammar.GetPatterns());
		self.SetName(grammar.GetScopeName());
		grammar.GetRepository().SetSelf(self);
		if (ruleBase != null)
		{
			grammar.GetRepository().SetBase(ruleBase);
		}
		else
		{
			grammar.GetRepository().SetBase(grammar.GetRepository().GetSelf());
		}
		return grammar;
	}

	private IRawGrammar Clone(IRawGrammar grammar)
	{
		return ((Raw)grammar).Clone();
	}

	public ITokenizeLineResult TokenizeLine(string lineText)
	{
		return TokenizeLine(lineText, null, TimeSpan.MaxValue);
	}

	public ITokenizeLineResult TokenizeLine(string lineText, IStateStack prevState, TimeSpan timeLimit)
	{
		return (ITokenizeLineResult)Tokenize(lineText, (StateStack)prevState, emitBinaryTokens: false, timeLimit);
	}

	public ITokenizeLineResult2 TokenizeLine2(string lineText)
	{
		return TokenizeLine2(lineText, null, TimeSpan.MaxValue);
	}

	public ITokenizeLineResult2 TokenizeLine2(string lineText, IStateStack prevState, TimeSpan timeLimit)
	{
		return (ITokenizeLineResult2)Tokenize(lineText, (StateStack)prevState, emitBinaryTokens: true, timeLimit);
	}

	private object Tokenize(string lineText, StateStack prevState, bool emitBinaryTokens, TimeSpan timeLimit)
	{
		if (_rootId == null)
		{
			GenerateRootId();
		}
		bool isFirstLine;
		if (prevState == null || prevState.Equals(StateStack.NULL))
		{
			isFirstLine = true;
			BasicScopeAttributes rawDefaultMetadata = _basicScopeAttributesProvider.GetDefaultAttributes();
			ThemeTrieElementRule defaultTheme = rawDefaultMetadata.ThemeData[0];
			int defaultMetadata = EncodedTokenAttributes.Set(0, rawDefaultMetadata.LanguageId, rawDefaultMetadata.TokenType, null, defaultTheme.fontStyle, defaultTheme.foreground, defaultTheme.background);
			string rootScopeName = GetRule(_rootId)?.GetName(null, null);
			if (rootScopeName == null)
			{
				return null;
			}
			BasicScopeAttributes rawRootMetadata = _basicScopeAttributesProvider.GetBasicScopeAttributes(rootScopeName);
			int rootMetadata = AttributedScopeStack.MergeAttributes(defaultMetadata, null, rawRootMetadata);
			AttributedScopeStack scopeList = new AttributedScopeStack(null, rootScopeName, rootMetadata);
			prevState = new StateStack(null, _rootId, -1, -1, beginRuleCapturedEOL: false, null, scopeList, scopeList);
		}
		else
		{
			isFirstLine = false;
			prevState.Reset();
		}
		if (string.IsNullOrEmpty(lineText) || lineText[lineText.Length - 1] != '\n')
		{
			lineText += "\n";
		}
		int lineLength = lineText.Length;
		LineTokens lineTokens = new LineTokens(emitBinaryTokens, lineText, _tokenTypeMatchers, _balancedBracketSelectors);
		TokenizeStringResult tokenizeResult = LineTokenizer.TokenizeString(this, lineText, isFirstLine, 0, prevState, lineTokens, checkWhileConditions: true, timeLimit);
		if (emitBinaryTokens)
		{
			return new TokenizeLineResult2(lineTokens.GetBinaryResult(tokenizeResult.Stack, lineLength), tokenizeResult.Stack, tokenizeResult.StoppedEarly);
		}
		return new TokenizeLineResult(lineTokens.GetResult(tokenizeResult.Stack, lineLength), tokenizeResult.Stack, tokenizeResult.StoppedEarly);
	}

	private void GenerateRootId()
	{
		_isCompiling = true;
		try
		{
			_rootId = RuleFactory.GetCompiledRuleId(_rawGrammar.GetRepository().GetSelf(), this, _rawGrammar.GetRepository());
		}
		finally
		{
			_isCompiling = false;
		}
	}

	private List<TokenTypeMatcher> GenerateTokenTypeMatchers(Dictionary<string, int> tokenTypes)
	{
		List<TokenTypeMatcher> result = new List<TokenTypeMatcher>();
		if (tokenTypes == null)
		{
			return result;
		}
		foreach (string selector in tokenTypes.Keys)
		{
			foreach (MatcherWithPriority<List<string>> matcher in TextMateSharp.Internal.Matcher.Matcher.CreateMatchers(selector))
			{
				result.Add(new TokenTypeMatcher(tokenTypes[selector], matcher.Matcher));
			}
		}
		return result;
	}

	public string GetName()
	{
		return _rawGrammar.GetName();
	}

	public string GetScopeName()
	{
		return _rootScopeName;
	}

	public ICollection<string> GetFileTypes()
	{
		return _rawGrammar.GetFileTypes();
	}
}
