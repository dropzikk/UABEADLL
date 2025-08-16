using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TextMateSharp.Grammars.Resources;

namespace TextMateSharp.Grammars;

public class LanguageConfiguration
{
	[JsonPropertyName("autoCloseBefore")]
	public string AutoCloseBefore { get; set; }

	[JsonPropertyName("folding")]
	public Folding Folding { get; set; }

	[JsonPropertyName("brackets")]
	public IList<string>[] Brackets { get; set; }

	[JsonPropertyName("comments")]
	public Comments Comments { get; set; }

	[JsonPropertyName("autoClosingPairs")]
	[JsonConverter(typeof(ClosingPairJsonConverter))]
	public AutoClosingPairs AutoClosingPairs { get; set; }

	[JsonPropertyName("indentationRules")]
	[JsonConverter(typeof(IntentationRulesJsonConverter))]
	public Indentation IndentationRules { get; set; }

	[JsonPropertyName("onEnterRules")]
	[JsonConverter(typeof(EnterRulesJsonConverter))]
	public EnterRules EnterRules { get; set; }

	public static LanguageConfiguration Load(string grammarName, string configurationFile)
	{
		if (string.IsNullOrEmpty(configurationFile))
		{
			return null;
		}
		using Stream stream = ResourceLoader.TryOpenLanguageConfiguration(grammarName, configurationFile);
		if (stream == null)
		{
			return null;
		}
		using (new StreamReader(stream))
		{
			return JsonSerializer.Deserialize<LanguageConfiguration>(stream, new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				ReadCommentHandling = JsonCommentHandling.Skip
			});
		}
	}
}
