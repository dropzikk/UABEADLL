using System.Collections.Generic;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Parser;
using TextMateSharp.Internal.Types;
using TextMateSharp.Internal.Utils;
using TextMateSharp.Themes;

namespace TextMateSharp.Internal.Grammars;

public class SyncRegistry : IGrammarRepository, IThemeProvider
{
	private Dictionary<string, IGrammar> _grammars;

	private Dictionary<string, IRawGrammar> _rawGrammars;

	private Dictionary<string, ICollection<string>> _injectionGrammars;

	private Theme _theme;

	public SyncRegistry(Theme theme)
	{
		_theme = theme;
		_grammars = new Dictionary<string, IGrammar>();
		_rawGrammars = new Dictionary<string, IRawGrammar>();
		_injectionGrammars = new Dictionary<string, ICollection<string>>();
	}

	public Theme GetTheme()
	{
		return _theme;
	}

	public void SetTheme(Theme theme)
	{
		_theme = theme;
		foreach (Grammar value in _grammars.Values)
		{
			value.OnDidChangeTheme();
		}
	}

	public ICollection<string> GetColorMap()
	{
		return _theme.GetColorMap();
	}

	public ICollection<string> AddGrammar(IRawGrammar grammar, ICollection<string> injectionScopeNames)
	{
		_rawGrammars.Add(grammar.GetScopeName(), grammar);
		ICollection<string> includedScopes = new List<string>();
		CollectIncludedScopes(includedScopes, grammar);
		if (injectionScopeNames != null)
		{
			_injectionGrammars.Add(grammar.GetScopeName(), injectionScopeNames);
			foreach (string injectionScopeName in injectionScopeNames)
			{
				AddIncludedScope(injectionScopeName, includedScopes);
			}
		}
		return includedScopes;
	}

	public IRawGrammar Lookup(string scopeName)
	{
		_rawGrammars.TryGetValue(scopeName, out var result);
		return result;
	}

	public ICollection<string> Injections(string targetScope)
	{
		_injectionGrammars.TryGetValue(targetScope, out var result);
		return result;
	}

	public ThemeTrieElementRule GetDefaults()
	{
		return _theme.GetDefaults();
	}

	public List<ThemeTrieElementRule> ThemeMatch(IList<string> scopeNames)
	{
		return _theme.Match(scopeNames);
	}

	public IGrammar GrammarForScopeName(string scopeName, int initialLanguage, Dictionary<string, int> embeddedLanguages, Dictionary<string, int> tokenTypes, BalancedBracketSelectors balancedBracketSelectors)
	{
		if (!_grammars.ContainsKey(scopeName))
		{
			IRawGrammar rawGrammar = Lookup(scopeName);
			if (rawGrammar == null)
			{
				return null;
			}
			_grammars.Add(scopeName, new Grammar(scopeName, rawGrammar, initialLanguage, embeddedLanguages, tokenTypes, balancedBracketSelectors, this, this));
		}
		return _grammars[scopeName];
	}

	private static void CollectIncludedScopes(ICollection<string> result, IRawGrammar grammar)
	{
		ICollection<IRawRule> patterns = grammar.GetPatterns();
		if (patterns != null)
		{
			ExtractIncludedScopesInPatterns(result, patterns);
		}
		IRawRepository repository = grammar.GetRepository();
		if (repository != null)
		{
			ExtractIncludedScopesInRepository(result, repository);
		}
		result.Remove(grammar.GetScopeName());
	}

	private static void ExtractIncludedScopesInPatterns(ICollection<string> result, ICollection<IRawRule> patterns)
	{
		foreach (IRawRule pattern in patterns)
		{
			ICollection<IRawRule> p = pattern.GetPatterns();
			if (p != null)
			{
				ExtractIncludedScopesInPatterns(result, p);
			}
			string include = pattern.GetInclude();
			if (include != null && !include.Equals("$base") && !include.Equals("$self") && include[0] != '#')
			{
				int sharpIndex = include.IndexOf('#');
				if (sharpIndex >= 0)
				{
					AddIncludedScope(include.SubstringAtIndexes(0, sharpIndex), result);
				}
				else
				{
					AddIncludedScope(include, result);
				}
			}
		}
	}

	private static void AddIncludedScope(string scopeName, ICollection<string> includedScopes)
	{
		if (!includedScopes.Contains(scopeName))
		{
			includedScopes.Add(scopeName);
		}
	}

	private static void ExtractIncludedScopesInRepository(ICollection<string> result, IRawRepository repository)
	{
		if (!(repository is Raw))
		{
			return;
		}
		Raw rawRepository = (Raw)repository;
		foreach (string key in rawRepository.Keys)
		{
			IRawRule obj = (IRawRule)rawRepository[key];
			ICollection<IRawRule> patterns = obj.GetPatterns();
			IRawRepository repositoryRule = obj.GetRepository();
			if (patterns != null)
			{
				ExtractIncludedScopesInPatterns(result, patterns);
			}
			if (repositoryRule != null)
			{
				ExtractIncludedScopesInRepository(result, repositoryRule);
			}
		}
	}
}
