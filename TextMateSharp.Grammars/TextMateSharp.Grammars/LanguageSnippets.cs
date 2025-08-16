using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TextMateSharp.Grammars.Resources;

namespace TextMateSharp.Grammars;

[JsonConverter(typeof(LanguageSnippetsJsonConverter))]
public class LanguageSnippets
{
	public IDictionary<string, LanguageSnippet> Snippets { get; set; } = new Dictionary<string, LanguageSnippet>();

	public static LanguageSnippets Load(string grammarName, Contributes contributes)
	{
		if (contributes == null || contributes.Snippets == null)
		{
			return null;
		}
		LanguageSnippets result = new LanguageSnippets();
		foreach (Snippet snippet in contributes.Snippets)
		{
			using Stream stream = ResourceLoader.TryOpenLanguageSnippet(grammarName, snippet.Path);
			if (stream == null)
			{
				continue;
			}
			using (new StreamReader(stream))
			{
				return JsonSerializer.Deserialize<LanguageSnippets>(stream, new JsonSerializerOptions
				{
					AllowTrailingCommas = true,
					ReadCommentHandling = JsonCommentHandling.Skip
				});
			}
		}
		return result;
	}
}
