using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Contributes
{
	[JsonPropertyName("languages")]
	public List<Language> Languages { get; set; }

	[JsonPropertyName("grammars")]
	public List<Grammar> Grammars { get; set; }

	[JsonPropertyName("snippets")]
	public List<Snippet> Snippets { get; set; }
}
