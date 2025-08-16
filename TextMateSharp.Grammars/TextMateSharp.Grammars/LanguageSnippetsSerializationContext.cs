using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextMateSharp.Grammars;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(LanguageSnippets))]
[GeneratedCode("System.Text.Json.SourceGeneration", "7.0.8.6910")]
internal sealed class LanguageSnippetsSerializationContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<LanguageSnippets>? _LanguageSnippets;

	private static LanguageSnippetsSerializationContext? s_defaultContext;

	public JsonTypeInfo<LanguageSnippets> LanguageSnippets => _LanguageSnippets ?? (_LanguageSnippets = Create_LanguageSnippets(base.Options, makeReadOnly: true));

	private static JsonSerializerOptions s_defaultOptions { get; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IgnoreReadOnlyFields = false,
		IgnoreReadOnlyProperties = false,
		IncludeFields = false,
		WriteIndented = false
	};

	public static LanguageSnippetsSerializationContext Default => s_defaultContext ?? (s_defaultContext = new LanguageSnippetsSerializationContext(new JsonSerializerOptions(s_defaultOptions)));

	protected override JsonSerializerOptions? GeneratedSerializerOptions { get; } = s_defaultOptions;

	private JsonTypeInfo<LanguageSnippets> Create_LanguageSnippets(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<LanguageSnippets> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(LanguageSnippets))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<LanguageSnippets>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonConverter jsonConverter = new LanguageSnippetsJsonConverter();
			Type typeFromHandle = typeof(LanguageSnippets);
			if (!jsonConverter.CanConvert(typeFromHandle))
			{
				throw new InvalidOperationException($"The converter '{jsonConverter.GetType()}' is not compatible with the type '{typeFromHandle}'.");
			}
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<LanguageSnippets>(options, jsonConverter);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	public LanguageSnippetsSerializationContext()
		: base(null)
	{
	}

	public LanguageSnippetsSerializationContext(JsonSerializerOptions options)
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
		if (type == typeof(LanguageSnippets))
		{
			return LanguageSnippets;
		}
		return null;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(LanguageSnippets))
		{
			return Create_LanguageSnippets(options, makeReadOnly: false);
		}
		return null;
	}
}
