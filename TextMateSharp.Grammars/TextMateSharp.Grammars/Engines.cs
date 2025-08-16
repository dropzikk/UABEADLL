using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Engines
{
	[JsonPropertyName("engines")]
	public string VsCode { get; set; }
}
