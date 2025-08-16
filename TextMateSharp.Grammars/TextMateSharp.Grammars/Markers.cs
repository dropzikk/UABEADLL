using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Markers
{
	[JsonPropertyName("start")]
	public string Start { get; set; }

	[JsonPropertyName("end")]
	public string End { get; set; }
}
