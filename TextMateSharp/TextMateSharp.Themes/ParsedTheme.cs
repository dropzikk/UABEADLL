using System;
using System.Collections.Generic;
using System.Linq;
using TextMateSharp.Internal.Utils;
using TextMateSharp.Registry;

namespace TextMateSharp.Themes;

internal class ParsedTheme
{
	private ThemeTrieElement _root;

	private ThemeTrieElementRule _defaults;

	private Dictionary<string, List<ThemeTrieElementRule>> _cachedMatchRoot;

	internal static List<ParsedThemeRule> ParseTheme(IRawTheme source, int priority)
	{
		List<ParsedThemeRule> result = new List<ParsedThemeRule>();
		LookupThemeRules(source.GetSettings(), result, priority);
		LookupThemeRules(source.GetTokenColors(), result, priority);
		return result;
	}

	internal static List<ParsedThemeRule> ParseInclude(IRawTheme source, IRegistryOptions registryOptions, int priority)
	{
		List<ParsedThemeRule> result = new List<ParsedThemeRule>();
		string include = source.GetInclude();
		if (string.IsNullOrEmpty(include))
		{
			return result;
		}
		IRawTheme themeInclude = registryOptions.GetTheme(include);
		if (themeInclude == null)
		{
			return result;
		}
		return ParseTheme(themeInclude, priority);
	}

	private static void LookupThemeRules(ICollection<IRawThemeSetting> settings, List<ParsedThemeRule> parsedThemeRules, int priority)
	{
		if (settings == null)
		{
			return;
		}
		int i = 0;
		foreach (IRawThemeSetting entry in settings)
		{
			if (entry.GetSetting() == null)
			{
				continue;
			}
			object settingScope = entry.GetScope();
			List<string> scopes = new List<string>();
			if (settingScope is string)
			{
				scopes = new List<string>(((string)settingScope).Trim(new char[1] { ',' }).Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries));
			}
			else if (settingScope is IList<object>)
			{
				scopes = new List<string>(((IList<object>)settingScope).Cast<string>());
			}
			else
			{
				scopes.Add("");
			}
			int fontStyle = -1;
			object settingsFontStyle = entry.GetSetting().GetFontStyle();
			if (settingsFontStyle is string)
			{
				fontStyle = 0;
				string[] array = ((string)settingsFontStyle).Split(new string[1] { " " }, StringSplitOptions.None);
				for (int j = 0; j < array.Length; j++)
				{
					switch (array[j])
					{
					case "italic":
						fontStyle |= 1;
						break;
					case "bold":
						fontStyle |= 2;
						break;
					case "underline":
						fontStyle |= 4;
						break;
					case "strikethrough":
						fontStyle |= 8;
						break;
					}
				}
			}
			string foreground = null;
			object settingsForeground = entry.GetSetting().GetForeground();
			if (settingsForeground is string && StringUtils.IsValidHexColor((string)settingsForeground))
			{
				foreground = (string)settingsForeground;
			}
			string background = null;
			object settingsBackground = entry.GetSetting().GetBackground();
			if (settingsBackground is string && StringUtils.IsValidHexColor((string)settingsBackground))
			{
				background = (string)settingsBackground;
			}
			int k = 0;
			for (int lenJ = scopes.Count; k < lenJ; k++)
			{
				List<string> segments = new List<string>(scopes[k].Trim().Split(new string[1] { " " }, StringSplitOptions.None));
				string scope = segments[segments.Count - 1];
				List<string> parentScopes = null;
				if (segments.Count > 1)
				{
					parentScopes = new List<string>(segments);
					parentScopes.Reverse();
				}
				ParsedThemeRule t = new ParsedThemeRule(entry.GetName(), scope, parentScopes, i, fontStyle, foreground, background);
				parsedThemeRules.Add(t);
			}
			i++;
		}
	}

	public static ParsedTheme CreateFromParsedTheme(List<ParsedThemeRule> source, ColorMap colorMap)
	{
		return ResolveParsedThemeRules(source, colorMap);
	}

	private static ParsedTheme ResolveParsedThemeRules(List<ParsedThemeRule> parsedThemeRules, ColorMap colorMap)
	{
		parsedThemeRules.Sort(delegate(ParsedThemeRule a, ParsedThemeRule b)
		{
			int num = StringUtils.StrCmp(a.scope, b.scope);
			if (num != 0)
			{
				return num;
			}
			num = StringUtils.StrArrCmp(a.parentScopes, b.parentScopes);
			return (num != 0) ? num : a.index.CompareTo(b.index);
		});
		int defaultFontStyle = 0;
		string defaultForeground = "#000000";
		string defaultBackground = "#ffffff";
		while (parsedThemeRules.Count >= 1 && "".Equals(parsedThemeRules[0].scope))
		{
			ParsedThemeRule incomingDefaults = parsedThemeRules[0];
			parsedThemeRules.RemoveAt(0);
			if (incomingDefaults.fontStyle != -1)
			{
				defaultFontStyle = incomingDefaults.fontStyle;
			}
			if (incomingDefaults.foreground != null)
			{
				defaultForeground = incomingDefaults.foreground;
			}
			if (incomingDefaults.background != null)
			{
				defaultBackground = incomingDefaults.background;
			}
		}
		ThemeTrieElementRule defaults = new ThemeTrieElementRule(string.Empty, 0, null, defaultFontStyle, colorMap.GetId(defaultForeground), colorMap.GetId(defaultBackground));
		ThemeTrieElement root = new ThemeTrieElement(new ThemeTrieElementRule(string.Empty, 0, null, -1, 0, 0), new List<ThemeTrieElementRule>());
		foreach (ParsedThemeRule rule in parsedThemeRules)
		{
			root.Insert(rule.name, 0, rule.scope, rule.parentScopes, rule.fontStyle, colorMap.GetId(rule.foreground), colorMap.GetId(rule.background));
		}
		return new ParsedTheme(defaults, root);
	}

	private ParsedTheme(ThemeTrieElementRule defaults, ThemeTrieElement root)
	{
		_root = root;
		_defaults = defaults;
		_cachedMatchRoot = new Dictionary<string, List<ThemeTrieElementRule>>();
	}

	internal List<ThemeTrieElementRule> Match(string scopeName)
	{
		lock (_cachedMatchRoot)
		{
			if (!_cachedMatchRoot.ContainsKey(scopeName))
			{
				_cachedMatchRoot[scopeName] = _root.Match(scopeName);
			}
			return _cachedMatchRoot[scopeName];
		}
	}

	internal ThemeTrieElementRule GetDefaults()
	{
		return _defaults;
	}
}
