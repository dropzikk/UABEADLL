using System.Collections.Generic;
using TextMateSharp.Internal.Types;
using TextMateSharp.Themes;

namespace TextMateSharp.Registry;

public class DefaultLocator : IRegistryOptions
{
	public ICollection<string> GetInjections(string scopeName)
	{
		return null;
	}

	public IRawTheme GetDefaultTheme()
	{
		return null;
	}

	public IRawTheme GetTheme(string scopeName)
	{
		return null;
	}

	public IRawGrammar GetGrammar(string scopeName)
	{
		return null;
	}
}
