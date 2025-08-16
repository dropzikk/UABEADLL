using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextMateSharp.Grammars;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(IList<string>))]
[GeneratedCode("System.Text.Json.SourceGeneration", "7.0.8.6910")]
internal sealed class StringPairSerializationContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<string>? _String;

	private JsonTypeInfo<IList<string>>? _IListString;

	private static StringPairSerializationContext? s_defaultContext;

	public JsonTypeInfo<string> String => _String ?? (_String = Create_String(base.Options, makeReadOnly: true));

	public JsonTypeInfo<IList<string>> IListString => _IListString ?? (_IListString = Create_IListString(base.Options, makeReadOnly: true));

	private static JsonSerializerOptions s_defaultOptions { get; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IgnoreReadOnlyFields = false,
		IgnoreReadOnlyProperties = false,
		IncludeFields = false,
		WriteIndented = false
	};

	public static StringPairSerializationContext Default => s_defaultContext ?? (s_defaultContext = new StringPairSerializationContext(new JsonSerializerOptions(s_defaultOptions)));

	protected override JsonSerializerOptions? GeneratedSerializerOptions { get; } = s_defaultOptions;

	private JsonTypeInfo<string> Create_String(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<string> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		jsonTypeInfo = ((options.Converters.Count <= 0 || (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(string))) == null) ? JsonMetadataServices.CreateValueInfo<string>(options, JsonMetadataServices.StringConverter) : JsonMetadataServices.CreateValueInfo<string>(options, runtimeProvidedCustomConverter));
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<IList<string>> Create_IListString(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<IList<string>> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(IList<string>))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<IList<string>>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<IList<string>> collectionInfo = new JsonCollectionInfoValues<IList<string>>
			{
				ObjectCreator = null,
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateIListInfo<IList<string>, string>(options, collectionInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	public StringPairSerializationContext()
		: base(null)
	{
	}

	public StringPairSerializationContext(JsonSerializerOptions options)
		: base(options)
	{
	}

	private static JsonConverter? GetRuntimeProvidedCustomConverter(JsonSerializerOptions options, Type type)
	{
		IList<JsonConverter> converters = options.Converters;
		for (int i = 0; i < converters.Count; i++)
		{
			JsonConverter jsonConverter = converters[i];
			if (!jsonConverter.CanConvert(type))
			{
				continue;
			}
			if (jsonConverter is JsonConverterFactory jsonConverterFactory)
			{
				jsonConverter = jsonConverterFactory.CreateConverter(type, options);
				if (jsonConverter == null || jsonConverter is JsonConverterFactory)
				{
					throw new InvalidOperationException($"The converter '{jsonConverterFactory.GetType()}' cannot return null or a JsonConverterFactory instance.");
				}
			}
			return jsonConverter;
		}
		return null;
	}

	public override JsonTypeInfo GetTypeInfo(Type type)
	{
		if (type == typeof(IList<string>))
		{
			return IListString;
		}
		if (type == typeof(string))
		{
			return String;
		}
		return null;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(IList<string>))
		{
			return Create_IListString(options, makeReadOnly: false);
		}
		if (type == typeof(string))
		{
			return Create_String(options, makeReadOnly: false);
		}
		return null;
	}
}
