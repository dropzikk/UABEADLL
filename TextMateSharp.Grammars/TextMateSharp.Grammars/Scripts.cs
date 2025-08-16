using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Scripts
{
	[JsonPropertyName("update-grammar")]
	public string UpdateGrammar { get; set; }
}
