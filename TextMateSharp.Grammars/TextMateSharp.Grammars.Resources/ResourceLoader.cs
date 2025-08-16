using System.IO;
using System.Reflection;

namespace TextMateSharp.Grammars.Resources;

internal class ResourceLoader
{
	private const string GrammarPrefix = "TextMateSharp.Grammars.Resources.Grammars.";

	private const string ThemesPrefix = "TextMateSharp.Grammars.Resources.Themes.";

	private const string SnippetPrefix = "TextMateSharp.Grammars.Resources.Grammars.";

	internal static Stream OpenGrammarPackage(string grammarName)
	{
		string text = "TextMateSharp.Grammars.Resources.Grammars." + grammarName.ToLowerInvariant() + ".package.json";
		return typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(text) ?? throw new FileNotFoundException("The grammar package '" + text + "' was not found.");
	}

	internal static Stream TryOpenLanguageConfiguration(string grammarName, string configurationFileName)
	{
		configurationFileName = configurationFileName.Replace('/', '.').TrimStart(new char[1] { '.' });
		string name = "TextMateSharp.Grammars.Resources.Grammars." + grammarName.ToLowerInvariant() + "." + configurationFileName;
		return typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(name);
	}

	internal static Stream TryOpenLanguageSnippet(string grammarName, string snippetFileName)
	{
		snippetFileName = snippetFileName.Replace('/', '.').TrimStart(new char[1] { '.' });
		string name = "TextMateSharp.Grammars.Resources.Grammars." + grammarName.ToLowerInvariant() + "." + snippetFileName;
		return typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(name);
	}

	internal static Stream TryOpenGrammarStream(string path)
	{
		return typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream("TextMateSharp.Grammars.Resources.Grammars." + path);
	}

	internal static Stream TryOpenThemeStream(string path)
	{
		return typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream("TextMateSharp.Grammars.Resources.Themes." + path);
	}
}
