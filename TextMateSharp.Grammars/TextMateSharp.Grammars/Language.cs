using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Language
{
	[JsonPropertyName("id")]
	public string Id { get; set; }

	[JsonPropertyName("extensions")]
	public List<string> Extensions { get; set; }

	[JsonPropertyName("aliases")]
	public List<string> Aliases { get; set; }

	[JsonPropertyName("configuration")]
	public string ConfigurationFile { get; set; }

	public LanguageConfiguration Configuration { get; set; }

	public override string ToString()
	{
		if (Aliases != null && Aliases.Count > 0)
		{
			return $"{Aliases[0]} ({Id})";
		}
		return Id;
	}
}
