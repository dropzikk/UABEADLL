using System.Collections.Generic;
using TextMateSharp.Internal.Types;
using TextMateSharp.Themes;

namespace TextMateSharp.Registry;

public interface IRegistryOptions
{
	IRawTheme GetTheme(string scopeName);

	IRawGrammar GetGrammar(string scopeName);

	ICollection<string> GetInjections(string scopeName);

	IRawTheme GetDefaultTheme();
}
