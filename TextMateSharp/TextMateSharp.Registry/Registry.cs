using System;
using System.Collections.Generic;
using System.IO;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Themes;

namespace TextMateSharp.Registry;

public class Registry
{
	private IRegistryOptions locator;

	private SyncRegistry syncRegistry;

	public Registry()
		: this(new DefaultLocator())
	{
	}

	public Registry(IRegistryOptions locator)
	{
		this.locator = locator;
		syncRegistry = new SyncRegistry(Theme.CreateFromRawTheme(locator.GetDefaultTheme(), locator));
	}

	public void SetTheme(IRawTheme theme)
	{
		syncRegistry.SetTheme(Theme.CreateFromRawTheme(theme, locator));
	}

	public ICollection<string> GetColorMap()
	{
		return syncRegistry.GetColorMap();
	}

	public IGrammar LoadGrammar(string initialScopeName)
	{
		if (string.IsNullOrEmpty(initialScopeName))
		{
			return null;
		}
		List<string> remainingScopeNames = new List<string>();
		remainingScopeNames.Add(initialScopeName);
		List<string> seenScopeNames = new List<string>();
		seenScopeNames.Add(initialScopeName);
		while (remainingScopeNames.Count > 0)
		{
			string scopeName = remainingScopeNames[0];
			remainingScopeNames.RemoveAt(0);
			if (syncRegistry.Lookup(scopeName) != null)
			{
				continue;
			}
			try
			{
				IRawGrammar grammar = locator.GetGrammar(scopeName);
				if (grammar == null)
				{
					continue;
				}
				ICollection<string> injections = locator.GetInjections(scopeName);
				foreach (string dep in syncRegistry.AddGrammar(grammar, injections))
				{
					if (!seenScopeNames.Contains(dep))
					{
						seenScopeNames.Add(dep);
						remainingScopeNames.Add(dep);
					}
				}
			}
			catch (Exception cause)
			{
				if (scopeName.Equals(initialScopeName))
				{
					throw new TMException("Unknown location for grammar <" + initialScopeName + ">", cause);
				}
			}
		}
		return GrammarForScopeName(initialScopeName);
	}

	public IGrammar LoadGrammarFromPathSync(string path, int initialLanguage, Dictionary<string, int> embeddedLanguages)
	{
		IRawGrammar rawGrammar = null;
		using (StreamReader sr = new StreamReader(path))
		{
			rawGrammar = GrammarReader.ReadGrammarSync(sr);
		}
		ICollection<string> injections = locator.GetInjections(rawGrammar.GetScopeName());
		syncRegistry.AddGrammar(rawGrammar, injections);
		return GrammarForScopeName(rawGrammar.GetScopeName(), initialLanguage, embeddedLanguages);
	}

	public IGrammar GrammarForScopeName(string scopeName)
	{
		return GrammarForScopeName(scopeName, 0, null);
	}

	public IGrammar GrammarForScopeName(string scopeName, int initialLanguage, Dictionary<string, int> embeddedLanguages)
	{
		return syncRegistry.GrammarForScopeName(scopeName, initialLanguage, embeddedLanguages, null, null);
	}

	public Theme GetTheme()
	{
		return syncRegistry.GetTheme();
	}

	public IRegistryOptions GetLocator()
	{
		return locator;
	}
}
