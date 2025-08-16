using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class IntentationRulesJsonConverter : JsonConverter<Indentation>
{
	public override Indentation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		Indentation indentation = new Indentation();
		if (reader.TokenType == JsonTokenType.StartObject)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
				switch (reader.TokenType)
				{
				case JsonTokenType.PropertyName:
					text = reader.GetString();
					continue;
				case JsonTokenType.String:
					switch (text)
					{
					case "increaseIndentPattern":
						indentation.Increase = reader.GetString();
						break;
					case "decreaseIndentPattern":
						indentation.Decrease = reader.GetString();
						break;
					case "unIndentedLinePattern":
						indentation.Unindent = reader.GetString();
						break;
					}
					continue;
				case JsonTokenType.StartObject:
					break;
				default:
					continue;
				}
				while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
				{
					switch (reader.TokenType)
					{
					case JsonTokenType.PropertyName:
						text2 = reader.GetString();
						break;
					case JsonTokenType.String:
						if (text2 == "pattern")
						{
							switch (text)
							{
							case "increaseIndentPattern":
								indentation.Increase = reader.GetString();
								break;
							case "decreaseIndentPattern":
								indentation.Decrease = reader.GetString();
								break;
							case "unIndentedLinePattern":
								indentation.Unindent = reader.GetString();
								break;
							}
						}
						break;
					}
				}
			}
		}
		return indentation;
	}

	public override void Write(Utf8JsonWriter writer, Indentation value, JsonSerializerOptions options)
	{
	}
}
