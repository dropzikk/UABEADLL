using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TextMateSharp.Grammars.Resources;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace TextMateSharp.Grammars;

public class RegistryOptions : IRegistryOptions
{
	private ThemeName _defaultTheme;

	private Dictionary<string, GrammarDefinition> _availableGrammars = new Dictionary<string, GrammarDefinition>();

	public RegistryOptions(ThemeName defaultTheme)
	{
		_defaultTheme = defaultTheme;
		InitializeAvailableGrammars();
	}

	public List<Language> GetAvailableLanguages()
	{
		List<Language> list = new List<Language>();
		foreach (GrammarDefinition value in _availableGrammars.Values)
		{
			foreach (Language language in value.Contributes.Languages)
			{
				if (language.Aliases != null && language.Aliases.Count != 0 && HasGrammar(language.Id, value.Contributes.Grammars))
				{
					list.Add(language);
				}
			}
		}
		return list;
	}

	public IEnumerable<GrammarDefinition> GetAvailableGrammarDefinitions()
	{
		return new List<GrammarDefinition>(_availableGrammars.Values);
	}

	public Language GetLanguageByExtension(string extension)
	{
		foreach (GrammarDefinition value in _availableGrammars.Values)
		{
			foreach (Language language in value.Contributes.Languages)
			{
				if (language.Extensions == null)
				{
					continue;
				}
				foreach (string extension2 in language.Extensions)
				{
					if (extension.Equals(extension2, StringComparison.OrdinalIgnoreCase))
					{
						return language;
					}
				}
			}
		}
		return null;
	}

	public string GetScopeByExtension(string extension)
	{
		foreach (GrammarDefinition value in _availableGrammars.Values)
		{
			foreach (Language language in value.Contributes.Languages)
			{
				if (language.Extensions == null)
				{
					continue;
				}
				foreach (string extension2 in language.Extensions)
				{
					if (!extension.Equals(extension2, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					using List<Grammar>.Enumerator enumerator4 = value.Contributes.Grammars.GetEnumerator();
					if (enumerator4.MoveNext())
					{
						return enumerator4.Current.ScopeName;
					}
				}
			}
		}
		return null;
	}

	public string GetScopeByLanguageId(string languageId)
	{
		if (string.IsNullOrEmpty(languageId))
		{
			return null;
		}
		foreach (GrammarDefinition value in _availableGrammars.Values)
		{
			foreach (Grammar grammar in value.Contributes.Grammars)
			{
				if (languageId.Equals(grammar.Language))
				{
					return grammar.ScopeName;
				}
			}
		}
		return null;
	}

	public IRawTheme LoadTheme(ThemeName name)
	{
		return GetTheme(GetThemeFile(name));
	}

	public ICollection<string> GetInjections(string scopeName)
	{
		return null;
	}

	public IRawTheme GetTheme(string scopeName)
	{
		Stream stream = ResourceLoader.TryOpenThemeStream(scopeName.Replace("./", string.Empty));
		if (stream == null)
		{
			return null;
		}
		using (stream)
		{
			using StreamReader reader = new StreamReader(stream);
			return ThemeReader.ReadThemeSync(reader);
		}
	}

	public IRawGrammar GetGrammar(string scopeName)
	{
		Stream stream = ResourceLoader.TryOpenGrammarStream(GetGrammarFile(scopeName));
		if (stream == null)
		{
			return null;
		}
		using (stream)
		{
			using StreamReader reader = new StreamReader(stream);
			return GrammarReader.ReadGrammarSync(reader);
		}
	}

	public IRawTheme GetDefaultTheme()
	{
		return LoadTheme(_defaultTheme);
	}

	private void InitializeAvailableGrammars()
	{
		string[] supportedGrammars = GrammarNames.SupportedGrammars;
		foreach (string text in supportedGrammars)
		{
			using Stream utf8Json = ResourceLoader.OpenGrammarPackage(text);
			GrammarDefinition grammarDefinition = JsonSerializer.Deserialize(utf8Json, GrammarDefinitionSerializationContext.Default.GrammarDefinition);
			foreach (Language language in grammarDefinition.Contributes.Languages)
			{
				language.Configuration = LanguageConfiguration.Load(text, language.ConfigurationFile);
			}
			grammarDefinition.LanguageSnippets = LanguageSnippets.Load(text, grammarDefinition.Contributes);
			_availableGrammars.Add(text, grammarDefinition);
		}
	}

	private string GetGrammarFile(string scopeName)
	{
		foreach (string key in _availableGrammars.Keys)
		{
			foreach (Grammar grammar in _availableGrammars[key].Contributes.Grammars)
			{
				if (scopeName.Equals(grammar.ScopeName))
				{
					string text = grammar.Path;
					if (text.StartsWith("./"))
					{
						text = text.Substring(2);
					}
					text = text.Replace("/", ".");
					return key.ToLower() + "." + text;
				}
			}
		}
		return null;
	}

	private string GetThemeFile(ThemeName name)
	{
		return name switch
		{
			ThemeName.Abbys => "abyss-color-theme.json", 
			ThemeName.Dark => "dark_vs.json", 
			ThemeName.DarkPlus => "dark_plus.json", 
			ThemeName.DimmedMonokai => "dimmed-monokai-color-theme.json", 
			ThemeName.KimbieDark => "kimbie-dark-color-theme.json", 
			ThemeName.Light => "light_vs.json", 
			ThemeName.LightPlus => "light_plus.json", 
			ThemeName.Monokai => "monokai-color-theme.json", 
			ThemeName.QuietLight => "quietlight-color-theme.json", 
			ThemeName.Red => "Red-color-theme.json", 
			ThemeName.SolarizedDark => "solarized-dark-color-theme.json", 
			ThemeName.SolarizedLight => "solarized-light-color-theme.json", 
			ThemeName.TomorrowNightBlue => "tomorrow-night-blue-color-theme.json", 
			ThemeName.HighContrastLight => "hc_light.json", 
			ThemeName.HighContrastDark => "hc_black.json", 
			_ => null, 
		};
	}

	private static bool HasGrammar(string id, List<Grammar> grammars)
	{
		foreach (Grammar grammar in grammars)
		{
			if (id == grammar.Language)
			{
				return true;
			}
		}
		return false;
	}
}
