using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TextMateSharp.Internal.Utils;
using TextMateSharp.Themes;

namespace TextMateSharp.Internal.Grammars;

public class BasicScopeAttributesProvider
{
	private static BasicScopeAttributes _NULL_SCOPE_METADATA = new BasicScopeAttributes(0, 0, null);

	private static Regex STANDARD_TOKEN_TYPE_REGEXP = new Regex("\\b(comment|string|regex|meta\\.embedded)\\b");

	private int _initialLanguage;

	private IThemeProvider _themeProvider;

	private Dictionary<string, BasicScopeAttributes> _cache = new Dictionary<string, BasicScopeAttributes>();

	private BasicScopeAttributes _defaultAttributes;

	private Dictionary<string, int> _embeddedLanguages;

	private Regex _embeddedLanguagesRegex;

	public BasicScopeAttributesProvider(int initialLanguage, IThemeProvider themeProvider, Dictionary<string, int> embeddedLanguages)
	{
		_initialLanguage = initialLanguage;
		_themeProvider = themeProvider;
		_defaultAttributes = new BasicScopeAttributes(_initialLanguage, OptionalStandardTokenType.NotSet, new List<ThemeTrieElementRule> { _themeProvider.GetDefaults() });
		_embeddedLanguages = new Dictionary<string, int>();
		if (embeddedLanguages != null)
		{
			foreach (string scope in embeddedLanguages.Keys)
			{
				int languageId = embeddedLanguages[scope];
				_embeddedLanguages[scope] = languageId;
			}
		}
		List<string> escapedScopes = _embeddedLanguages.Keys.Select((string s) => RegexSource.EscapeRegExpCharacters(s)).ToList();
		if (escapedScopes.Count == 0)
		{
			_embeddedLanguagesRegex = null;
			return;
		}
		List<string> list = new List<string>(escapedScopes);
		list.Sort();
		list.Reverse();
		_embeddedLanguagesRegex = new Regex("^((" + string.Join(")|(", escapedScopes) + "))($|\\.)");
	}

	public void OnDidChangeTheme()
	{
		_cache.Clear();
		_defaultAttributes = new BasicScopeAttributes(_initialLanguage, OptionalStandardTokenType.NotSet, new List<ThemeTrieElementRule> { _themeProvider.GetDefaults() });
	}

	public BasicScopeAttributes GetDefaultAttributes()
	{
		return _defaultAttributes;
	}

	public BasicScopeAttributes GetBasicScopeAttributes(string scopeName)
	{
		if (scopeName == null)
		{
			return _NULL_SCOPE_METADATA;
		}
		_cache.TryGetValue(scopeName, out var value);
		if (value != null)
		{
			return value;
		}
		value = DoGetMetadataForScope(scopeName);
		_cache[scopeName] = value;
		return value;
	}

	private BasicScopeAttributes DoGetMetadataForScope(string scopeName)
	{
		int languageId = ScopeToLanguage(scopeName);
		int standardTokenType = ToStandardTokenType(scopeName);
		List<ThemeTrieElementRule> themeData = _themeProvider.ThemeMatch(new string[1] { scopeName });
		return new BasicScopeAttributes(languageId, standardTokenType, themeData);
	}

	private int ScopeToLanguage(string scope)
	{
		if (scope == null)
		{
			return 0;
		}
		if (_embeddedLanguagesRegex == null)
		{
			return 0;
		}
		Match m = _embeddedLanguagesRegex.Match(scope);
		if (!m.Success)
		{
			return 0;
		}
		string scopeName = m.Groups[1].Value;
		if (!_embeddedLanguages.ContainsKey(scopeName))
		{
			return 0;
		}
		return _embeddedLanguages[scopeName];
	}

	private static int ToStandardTokenType(string tokenType)
	{
		Match m = STANDARD_TOKEN_TYPE_REGEXP.Match(tokenType);
		if (!m.Success)
		{
			return OptionalStandardTokenType.NotSet;
		}
		return m.Value switch
		{
			"comment" => OptionalStandardTokenType.Comment, 
			"string" => OptionalStandardTokenType.String, 
			"regex" => OptionalStandardTokenType.RegEx, 
			"meta.embedded" => OptionalStandardTokenType.Other, 
			_ => throw new TMException("Unexpected match for standard token type!"), 
		};
	}
}
