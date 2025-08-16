using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class LanguageSnippetJsonConverter : JsonConverter<LanguageSnippet>
{
	public override LanguageSnippet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		LanguageSnippet languageSnippet = new LanguageSnippet();
		if (reader.TokenType == JsonTokenType.StartObject)
		{
			string text = string.Empty;
			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
				switch (reader.TokenType)
				{
				case JsonTokenType.PropertyName:
					text = reader.GetString();
					break;
				case JsonTokenType.StartArray:
				{
					IList<string> list = new List<string>();
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					{
						if (string.Compare(text, "body") == 0 && reader.TokenType == JsonTokenType.String)
						{
							list.Add(reader.GetString());
						}
					}
					languageSnippet.Body = list.ToArray();
					break;
				}
				case JsonTokenType.String:
					if (!(text == "prefix"))
					{
						if (text == "description")
						{
							languageSnippet.Description = reader.GetString();
						}
					}
					else
					{
						languageSnippet.Prefix = reader.GetString();
					}
					break;
				}
			}
		}
		return languageSnippet;
	}

	public override void Write(Utf8JsonWriter writer, LanguageSnippet value, JsonSerializerOptions options)
	{
	}
}
