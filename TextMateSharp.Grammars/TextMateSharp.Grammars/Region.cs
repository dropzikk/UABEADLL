using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Region
{
	[JsonPropertyName("prefix")]
	public string Prefix { get; set; }

	[JsonPropertyName("body")]
	public string[] Body { get; set; }

	[JsonPropertyName("description")]
	public string Description { get; set; }
}
