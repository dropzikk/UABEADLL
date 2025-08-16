using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

[JsonConverter(typeof(EnterRuleJsonConverter))]
public class EnterRule
{
	public string BeforeText { get; set; }

	public string AfterText { get; set; }

	public string ActionIndent { get; set; }

	public string AppendText { get; set; }
}
