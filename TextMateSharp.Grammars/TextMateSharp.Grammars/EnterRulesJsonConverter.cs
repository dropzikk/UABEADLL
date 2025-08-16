using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class EnterRulesJsonConverter : JsonConverter<EnterRules>
{
	public override EnterRules Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		EnterRules enterRules = new EnterRules();
		if (reader.TokenType == JsonTokenType.StartArray)
		{
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				switch (reader.TokenType)
				{
				case JsonTokenType.StartObject:
				{
					EnterRule item = JsonSerializer.Deserialize(ref reader, EnterRuleSerializationContext.Default.EnterRule);
					enterRules.Rules.Add(item);
					break;
				}
				}
			}
		}
		return enterRules;
	}

	public override void Write(Utf8JsonWriter writer, EnterRules value, JsonSerializerOptions options)
	{
	}
}
