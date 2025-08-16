using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

[JsonConverter(typeof(LanguageSnippetJsonConverter))]
public class LanguageSnippet
{
	public string Prefix { get; set; }

	public string[] Body { get; set; }

	public string Description { get; set; }
}
