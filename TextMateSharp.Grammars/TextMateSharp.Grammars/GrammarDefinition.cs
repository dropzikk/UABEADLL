using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class GrammarDefinition
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("displayName")]
	public string DisplayName { get; set; }

	[JsonPropertyName("description")]
	public string Description { get; set; }

	[JsonPropertyName("version")]
	public string Version { get; set; }

	[JsonPropertyName("publisher")]
	public string Publisher { get; set; }

	[JsonPropertyName("license")]
	public string License { get; set; }

	[JsonPropertyName("engines")]
	public Engines Engines { get; set; }

	[JsonPropertyName("scripts")]
	public Scripts Scripts { get; set; }

	[JsonPropertyName("contributes")]
	public Contributes Contributes { get; set; }

	[JsonPropertyName("repository")]
	public Repository Repository { get; set; }

	public LanguageSnippets LanguageSnippets { get; set; }
}
