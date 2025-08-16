using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextMateSharp.Grammars;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(EnterRule))]
[GeneratedCode("System.Text.Json.SourceGeneration", "7.0.8.6910")]
internal sealed class EnterRuleSerializationContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<EnterRule>? _EnterRule;

	private static EnterRuleSerializationContext? s_defaultContext;

	public JsonTypeInfo<EnterRule> EnterRule => _EnterRule ?? (_EnterRule = Create_EnterRule(base.Options, makeReadOnly: true));

	private static JsonSerializerOptions s_defaultOptions { get; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IgnoreReadOnlyFields = false,
		IgnoreReadOnlyProperties = false,
		IncludeFields = false,
		WriteIndented = false
	};

	public static EnterRuleSerializationContext Default => s_defaultContext ?? (s_defaultContext = new EnterRuleSerializationContext(new JsonSerializerOptions(s_defaultOptions)));

	protected override JsonSerializerOptions? GeneratedSerializerOptions { get; } = s_defaultOptions;

	private JsonTypeInfo<EnterRule> Create_EnterRule(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<EnterRule> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(EnterRule))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<EnterRule>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonConverter jsonConverter = new EnterRuleJsonConverter();
			Type typeFromHandle = typeof(EnterRule);
			if (!jsonConverter.CanConvert(typeFromHandle))
			{
				throw new InvalidOperationException($"The converter '{jsonConverter.GetType()}' is not compatible with the type '{typeFromHandle}'.");
			}
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<EnterRule>(options, jsonConverter);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	public EnterRuleSerializationContext()
		: base(null)
	{
	}

	public EnterRuleSerializationContext(JsonSerializerOptions options)
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
		if (type == typeof(EnterRule))
		{
			return EnterRule;
		}
		return null;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(EnterRule))
		{
			return Create_EnterRule(options, makeReadOnly: false);
		}
		return null;
	}
}
