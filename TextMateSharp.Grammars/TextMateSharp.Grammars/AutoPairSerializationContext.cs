using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextMateSharp.Grammars;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(AutoPair))]
[GeneratedCode("System.Text.Json.SourceGeneration", "7.0.8.6910")]
internal sealed class AutoPairSerializationContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<string>? _String;

	private JsonTypeInfo<IList<string>>? _IListString;

	private JsonTypeInfo<AutoPair>? _AutoPair;

	private static AutoPairSerializationContext? s_defaultContext;

	public JsonTypeInfo<string> String => _String ?? (_String = Create_String(base.Options, makeReadOnly: true));

	public JsonTypeInfo<IList<string>> IListString => _IListString ?? (_IListString = Create_IListString(base.Options, makeReadOnly: true));

	public JsonTypeInfo<AutoPair> AutoPair => _AutoPair ?? (_AutoPair = Create_AutoPair(base.Options, makeReadOnly: true));

	private static JsonSerializerOptions s_defaultOptions { get; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IgnoreReadOnlyFields = false,
		IgnoreReadOnlyProperties = false,
		IncludeFields = false,
		WriteIndented = false
	};

	public static AutoPairSerializationContext Default => s_defaultContext ?? (s_defaultContext = new AutoPairSerializationContext(new JsonSerializerOptions(s_defaultOptions)));

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

	private JsonTypeInfo<AutoPair> Create_AutoPair(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<AutoPair> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(AutoPair))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<AutoPair>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<AutoPair> objectInfo = new JsonObjectInfoValues<AutoPair>
			{
				ObjectCreator = () => new AutoPair(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => AutoPairPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] AutoPairPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AutoPair),
			Converter = null,
			Getter = (object obj) => ((AutoPair)obj).Open,
			Setter = delegate(object obj, string? value)
			{
				((AutoPair)obj).Open = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Open",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AutoPair),
			Converter = null,
			Getter = (object obj) => ((AutoPair)obj).Close,
			Setter = delegate(object obj, string? value)
			{
				((AutoPair)obj).Close = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Close",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		JsonPropertyInfoValues<IList<string>> propertyInfo3 = new JsonPropertyInfoValues<IList<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AutoPair),
			Converter = null,
			Getter = (object obj) => ((AutoPair)obj).NotIn,
			Setter = delegate(object obj, IList<string>? value)
			{
				((AutoPair)obj).NotIn = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NotIn",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo3 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2] = jsonPropertyInfo3;
		return array;
	}

	public AutoPairSerializationContext()
		: base(null)
	{
	}

	public AutoPairSerializationContext(JsonSerializerOptions options)
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
		if (type == typeof(AutoPair))
		{
			return AutoPair;
		}
		if (type == typeof(string))
		{
			return String;
		}
		if (type == typeof(IList<string>))
		{
			return IListString;
		}
		return null;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(AutoPair))
		{
			return Create_AutoPair(options, makeReadOnly: false);
		}
		if (type == typeof(string))
		{
			return Create_String(options, makeReadOnly: false);
		}
		if (type == typeof(IList<string>))
		{
			return Create_IListString(options, makeReadOnly: false);
		}
		return null;
	}
}
