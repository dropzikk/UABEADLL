using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Repository
{
	[JsonPropertyName("type")]
	public string Type { get; set; }

	[JsonPropertyName("url")]
	public string Url { get; set; }
}
