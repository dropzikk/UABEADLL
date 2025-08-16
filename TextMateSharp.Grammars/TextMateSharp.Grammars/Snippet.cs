using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Snippet
{
	[JsonPropertyName("language")]
	public string Language { get; set; }

	[JsonPropertyName("path")]
	public string Path { get; set; }
}
