using System;
using System.IO;
using System.Text.Json;

namespace TextMateSharp.Internal.Parser.Json;

public class JSONPListParser<T>
{
	private readonly bool theme;

	public JSONPListParser(bool theme)
	{
		this.theme = theme;
	}

	public T Parse(StreamReader contents)
	{
		PList<T> pList = new PList<T>(theme);
		byte[] buffer = new byte[2048];
		JsonReaderOptions jsonReaderOptions = default(JsonReaderOptions);
		jsonReaderOptions.AllowTrailingCommas = true;
		jsonReaderOptions.CommentHandling = JsonCommentHandling.Skip;
		JsonReaderOptions options = jsonReaderOptions;
		int size = contents.BaseStream.Read(buffer, 0, buffer.Length);
		Utf8JsonReader reader = new Utf8JsonReader(buffer.AsSpan(0, size), isFinalBlock: false, new JsonReaderState(options));
		while (true)
		{
			int bytesRead = -1;
			while (!reader.Read())
			{
				bytesRead = GetMoreBytesFromStream(contents.BaseStream, ref buffer, ref reader);
				if (bytesRead == 0)
				{
					break;
				}
			}
			if (bytesRead == 0)
			{
				break;
			}
			switch (reader.TokenType)
			{
			case JsonTokenType.StartArray:
				pList.StartElement("array");
				break;
			case JsonTokenType.EndArray:
				pList.EndElement("array");
				break;
			case JsonTokenType.StartObject:
				pList.StartElement("dict");
				break;
			case JsonTokenType.EndObject:
				pList.EndElement("dict");
				break;
			case JsonTokenType.PropertyName:
				pList.StartElement("key");
				pList.AddString(reader.GetString());
				pList.EndElement("key");
				break;
			case JsonTokenType.String:
				pList.StartElement("string");
				pList.AddString(reader.GetString());
				pList.EndElement("string");
				break;
			}
		}
		return pList.GetResult();
	}

	private static int GetMoreBytesFromStream(Stream stream, ref byte[] buffer, ref Utf8JsonReader reader)
	{
		int bytesRead;
		if (reader.BytesConsumed < buffer.Length)
		{
			ReadOnlySpan<byte> leftover = buffer.AsSpan((int)reader.BytesConsumed);
			if (leftover.Length == buffer.Length)
			{
				Array.Resize(ref buffer, buffer.Length * 2);
			}
			leftover.CopyTo(buffer);
			bytesRead = stream.Read(buffer, leftover.Length, buffer.Length - leftover.Length);
			reader = new Utf8JsonReader(buffer.AsSpan(0, bytesRead + leftover.Length), bytesRead == 0, reader.CurrentState);
		}
		else
		{
			bytesRead = stream.Read(buffer, 0, buffer.Length);
			reader = new Utf8JsonReader(buffer.AsSpan(0, bytesRead), bytesRead == 0, reader.CurrentState);
		}
		return bytesRead;
	}
}
