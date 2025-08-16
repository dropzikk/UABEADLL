using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Comments
{
	[JsonPropertyName("lineComment")]
	public string LineComment { get; set; }

	[JsonPropertyName("blockComment")]
	public IList<string> BlockComment { get; set; }
}
