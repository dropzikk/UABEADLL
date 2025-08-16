using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextMateSharp.Grammars;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(LanguageSnippet))]
[GeneratedCode("System.Text.Json.SourceGeneration", "7.0.8.6910")]
internal sealed class LanguageSnippetSerializationContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<LanguageSnippet>? _LanguageSnippet;

	private static LanguageSnippetSerializationContext? s_defaultContext;

	public JsonTypeInfo<LanguageSnippet> LanguageSnippet => _LanguageSnippet ?? (_LanguageSnippet = Create_LanguageSnippet(base.Options, makeReadOnly: true));

	private static JsonSerializerOptions s_defaultOptions { get; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IgnoreReadOnlyFields = false,
		IgnoreReadOnlyProperties = false,
		IncludeFields = false,
		WriteIndented = false
	};

	public static LanguageSnippetSerializationContext Default => s_defaultContext ?? (s_defaultContext = new LanguageSnippetSerializationContext(new JsonSerializerOptions(s_defaultOptions)));

	protected override JsonSerializerOptions? GeneratedSerializerOptions { get; } = s_defaultOptions;

	private JsonTypeInfo<LanguageSnippet> Create_LanguageSnippet(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<LanguageSnippet> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(LanguageSnippet))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<LanguageSnippet>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonConverter jsonConverter = new LanguageSnippetJsonConverter();
			Type typeFromHandle = typeof(LanguageSnippet);
			if (!jsonConverter.CanConvert(typeFromHandle))
			{
				throw new InvalidOperationException($"The converter '{jsonConverter.GetType()}' is not compatible with the type '{typeFromHandle}'.");
			}
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<LanguageSnippet>(options, jsonConverter);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	public LanguageSnippetSerializationContext()
		: base(null)
	{
	}

	public LanguageSnippetSerializationContext(JsonSerializerOptions options)
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
		if (type == typeof(LanguageSnippet))
		{
			return LanguageSnippet;
		}
		return null;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(LanguageSnippet))
		{
			return Create_LanguageSnippet(options, makeReadOnly: false);
		}
		return null;
	}
}
