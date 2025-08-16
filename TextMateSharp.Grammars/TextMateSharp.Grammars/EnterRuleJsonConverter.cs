using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class EnterRuleJsonConverter : JsonConverter<EnterRule>
{
	public override EnterRule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		EnterRule enterRule = new EnterRule();
		string text = string.Empty;
		string text2 = string.Empty;
		if (reader.TokenType == JsonTokenType.StartObject)
		{
			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
				switch (reader.TokenType)
				{
				case JsonTokenType.PropertyName:
					text = reader.GetString();
					break;
				case JsonTokenType.StartObject:
					while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
					{
						switch (reader.TokenType)
						{
						case JsonTokenType.PropertyName:
							text2 = reader.GetString();
							break;
						case JsonTokenType.String:
							switch (text2)
							{
							case "pattern":
								if (!(text == "beforeText"))
								{
									if (text == "afterText")
									{
										enterRule.AfterText = reader.GetString();
									}
								}
								else
								{
									enterRule.BeforeText = reader.GetString();
								}
								break;
							case "indent":
								enterRule.ActionIndent = reader.GetString();
								break;
							case "appendText":
								enterRule.AppendText = reader.GetString();
								break;
							}
							break;
						}
					}
					break;
				case JsonTokenType.String:
					if (!(text == "beforeText"))
					{
						if (text == "afterText")
						{
							enterRule.AfterText = reader.GetString();
						}
					}
					else
					{
						enterRule.BeforeText = reader.GetString();
					}
					break;
				}
			}
		}
		return enterRule;
	}

	public override void Write(Utf8JsonWriter writer, EnterRule value, JsonSerializerOptions options)
	{
	}
}
