using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class LanguageSnippetsJsonConverter : JsonConverter<LanguageSnippets>
{
	public override LanguageSnippets Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		LanguageSnippets languageSnippets = new LanguageSnippets();
		if (reader.TokenType == JsonTokenType.StartObject)
		{
			string key = string.Empty;
			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
				switch (reader.TokenType)
				{
				case JsonTokenType.PropertyName:
					key = reader.GetString();
					break;
				case JsonTokenType.StartObject:
				{
					LanguageSnippet value = JsonSerializer.Deserialize(ref reader, LanguageSnippetSerializationContext.Default.LanguageSnippet);
					languageSnippets.Snippets.Add(key, value);
					break;
				}
				}
			}
		}
		return languageSnippets;
	}

	public override void Write(Utf8JsonWriter writer, LanguageSnippets value, JsonSerializerOptions options)
	{
	}
}
