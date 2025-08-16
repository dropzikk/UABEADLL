using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class ClosingPairJsonConverter : JsonConverter<AutoClosingPairs>
{
	public override AutoClosingPairs Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		AutoClosingPairs autoClosingPairs = new AutoClosingPairs();
		if (reader.TokenType == JsonTokenType.StartArray)
		{
			List<IList<char>> list = new List<IList<char>>();
			List<AutoPair> list2 = new List<AutoPair>();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				switch (reader.TokenType)
				{
				case JsonTokenType.StartArray:
				{
					List<char> list3 = new List<char>();
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					{
						if (reader.TokenType == JsonTokenType.String)
						{
							list3.Add(reader.GetString().ToCharArray().First());
						}
					}
					list.Add(list3);
					break;
				}
				case JsonTokenType.StartObject:
				{
					AutoPair autoPair = new AutoPair();
					string text = string.Empty;
					while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
					{
						switch (reader.TokenType)
						{
						case JsonTokenType.StartArray:
							if (string.Compare(text, "notIn") == 0)
							{
								autoPair.NotIn = JsonSerializer.Deserialize(ref reader, StringPairSerializationContext.Default.IListString);
							}
							break;
						case JsonTokenType.PropertyName:
							text = reader.GetString();
							break;
						case JsonTokenType.String:
							if (!(text == "open"))
							{
								if (text == "close")
								{
									autoPair.Close = reader.GetString();
								}
							}
							else
							{
								autoPair.Open = reader.GetString();
							}
							break;
						}
					}
					list2.Add(autoPair);
					break;
				}
				}
			}
			autoClosingPairs.CharPairs = list.ToArray();
			autoClosingPairs.AutoPairs = list2.ToArray();
		}
		return autoClosingPairs;
	}

	public override void Write(Utf8JsonWriter writer, AutoClosingPairs value, JsonSerializerOptions options)
	{
	}
}
