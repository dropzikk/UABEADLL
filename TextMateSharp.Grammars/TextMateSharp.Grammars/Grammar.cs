using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Grammar
{
	[JsonPropertyName("language")]
	public string Language { get; set; }

	[JsonPropertyName("scopeName")]
	public string ScopeName { get; set; }

	[JsonPropertyName("path")]
	public string Path { get; set; }
}
