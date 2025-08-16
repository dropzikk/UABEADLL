using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextMateSharp.Grammars;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(LanguageConfiguration))]
[GeneratedCode("System.Text.Json.SourceGeneration", "7.0.8.6910")]
internal sealed class LanguageConfigurationSerializationContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<string>? _String;

	private JsonTypeInfo<bool>? _Boolean;

	private JsonTypeInfo<Markers>? _Markers;

	private JsonTypeInfo<Folding>? _Folding;

	private JsonTypeInfo<IList<string>>? _IListString;

	private JsonTypeInfo<IList<string>[]>? _IListStringArray;

	private JsonTypeInfo<Comments>? _Comments;

	private JsonTypeInfo<char>? _Char;

	private JsonTypeInfo<IList<char>>? _IListChar;

	private JsonTypeInfo<IList<char>[]>? _IListCharArray;

	private JsonTypeInfo<AutoPair>? _AutoPair;

	private JsonTypeInfo<AutoPair[]>? _AutoPairArray;

	private JsonTypeInfo<AutoClosingPairs>? _AutoClosingPairs;

	private JsonTypeInfo<Indentation>? _Indentation;

	private JsonTypeInfo<EnterRule>? _EnterRule;

	private JsonTypeInfo<IList<EnterRule>>? _IListEnterRule;

	private JsonTypeInfo<EnterRules>? _EnterRules;

	private JsonTypeInfo<LanguageConfiguration>? _LanguageConfiguration;

	private static LanguageConfigurationSerializationContext? s_defaultContext;

	public JsonTypeInfo<string> String => _String ?? (_String = Create_String(base.Options, makeReadOnly: true));

	public JsonTypeInfo<bool> Boolean => _Boolean ?? (_Boolean = Create_Boolean(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Markers> Markers => _Markers ?? (_Markers = Create_Markers(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Folding> Folding => _Folding ?? (_Folding = Create_Folding(base.Options, makeReadOnly: true));

	public JsonTypeInfo<IList<string>> IListString => _IListString ?? (_IListString = Create_IListString(base.Options, makeReadOnly: true));

	public JsonTypeInfo<IList<string>[]> IListStringArray => _IListStringArray ?? (_IListStringArray = Create_IListStringArray(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Comments> Comments => _Comments ?? (_Comments = Create_Comments(base.Options, makeReadOnly: true));

	public JsonTypeInfo<char> Char => _Char ?? (_Char = Create_Char(base.Options, makeReadOnly: true));

	public JsonTypeInfo<IList<char>> IListChar => _IListChar ?? (_IListChar = Create_IListChar(base.Options, makeReadOnly: true));

	public JsonTypeInfo<IList<char>[]> IListCharArray => _IListCharArray ?? (_IListCharArray = Create_IListCharArray(base.Options, makeReadOnly: true));

	public JsonTypeInfo<AutoPair> AutoPair => _AutoPair ?? (_AutoPair = Create_AutoPair(base.Options, makeReadOnly: true));

	public JsonTypeInfo<AutoPair[]> AutoPairArray => _AutoPairArray ?? (_AutoPairArray = Create_AutoPairArray(base.Options, makeReadOnly: true));

	public JsonTypeInfo<AutoClosingPairs> AutoClosingPairs => _AutoClosingPairs ?? (_AutoClosingPairs = Create_AutoClosingPairs(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Indentation> Indentation => _Indentation ?? (_Indentation = Create_Indentation(base.Options, makeReadOnly: true));

	public JsonTypeInfo<EnterRule> EnterRule => _EnterRule ?? (_EnterRule = Create_EnterRule(base.Options, makeReadOnly: true));

	public JsonTypeInfo<IList<EnterRule>> IListEnterRule => _IListEnterRule ?? (_IListEnterRule = Create_IListEnterRule(base.Options, makeReadOnly: true));

	public JsonTypeInfo<EnterRules> EnterRules => _EnterRules ?? (_EnterRules = Create_EnterRules(base.Options, makeReadOnly: true));

	public JsonTypeInfo<LanguageConfiguration> LanguageConfiguration => _LanguageConfiguration ?? (_LanguageConfiguration = Create_LanguageConfiguration(base.Options, makeReadOnly: true));

	private static JsonSerializerOptions s_defaultOptions { get; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IgnoreReadOnlyFields = false,
		IgnoreReadOnlyProperties = false,
		IncludeFields = false,
		WriteIndented = false
	};

	public static LanguageConfigurationSerializationContext Default => s_defaultContext ?? (s_defaultContext = new LanguageConfigurationSerializationContext(new JsonSerializerOptions(s_defaultOptions)));

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

	private JsonTypeInfo<bool> Create_Boolean(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<bool> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		jsonTypeInfo = ((options.Converters.Count <= 0 || (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(bool))) == null) ? JsonMetadataServices.CreateValueInfo<bool>(options, JsonMetadataServices.BooleanConverter) : JsonMetadataServices.CreateValueInfo<bool>(options, runtimeProvidedCustomConverter));
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Markers> Create_Markers(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Markers> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Markers))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Markers>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Markers> objectInfo = new JsonObjectInfoValues<Markers>
			{
				ObjectCreator = () => new Markers(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => MarkersPropInit(options),
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

	private static JsonPropertyInfo[] MarkersPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Markers),
			Converter = null,
			Getter = (object obj) => ((Markers)obj).Start,
			Setter = delegate(object obj, string? value)
			{
				((Markers)obj).Start = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Start",
			JsonPropertyName = "start"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Markers),
			Converter = null,
			Getter = (object obj) => ((Markers)obj).End,
			Setter = delegate(object obj, string? value)
			{
				((Markers)obj).End = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "End",
			JsonPropertyName = "end"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		return array;
	}

	private JsonTypeInfo<Folding> Create_Folding(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Folding> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Folding))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Folding>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Folding> objectInfo = new JsonObjectInfoValues<Folding>
			{
				ObjectCreator = () => new Folding(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => FoldingPropInit(options),
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

	private static JsonPropertyInfo[] FoldingPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<bool> propertyInfo = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Folding),
			Converter = null,
			Getter = (object obj) => ((Folding)obj).OffSide,
			Setter = delegate(object obj, bool value)
			{
				((Folding)obj).OffSide = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "OffSide",
			JsonPropertyName = "offSide"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<Markers> propertyInfo2 = new JsonPropertyInfoValues<Markers>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Folding),
			Converter = null,
			Getter = (object obj) => ((Folding)obj).Markers,
			Setter = delegate(object obj, Markers? value)
			{
				((Folding)obj).Markers = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Markers",
			JsonPropertyName = "markers"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		JsonPropertyInfoValues<bool> propertyInfo3 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Folding),
			Converter = null,
			Getter = (object obj) => ((Folding)obj).IsEmpty,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IsEmpty",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo3 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2] = jsonPropertyInfo3;
		return array;
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

	private JsonTypeInfo<IList<string>[]> Create_IListStringArray(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<IList<string>[]> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(IList<string>[]))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<IList<string>[]>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<IList<string>[]> collectionInfo = new JsonCollectionInfoValues<IList<string>[]>
			{
				ObjectCreator = null,
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateArrayInfo(options, collectionInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Comments> Create_Comments(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Comments> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Comments))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Comments>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Comments> objectInfo = new JsonObjectInfoValues<Comments>
			{
				ObjectCreator = () => new Comments(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => CommentsPropInit(options),
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

	private static JsonPropertyInfo[] CommentsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Comments),
			Converter = null,
			Getter = (object obj) => ((Comments)obj).LineComment,
			Setter = delegate(object obj, string? value)
			{
				((Comments)obj).LineComment = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "LineComment",
			JsonPropertyName = "lineComment"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<IList<string>> propertyInfo2 = new JsonPropertyInfoValues<IList<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Comments),
			Converter = null,
			Getter = (object obj) => ((Comments)obj).BlockComment,
			Setter = delegate(object obj, IList<string>? value)
			{
				((Comments)obj).BlockComment = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BlockComment",
			JsonPropertyName = "blockComment"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		return array;
	}

	private JsonTypeInfo<char> Create_Char(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<char> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		jsonTypeInfo = ((options.Converters.Count <= 0 || (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(char))) == null) ? JsonMetadataServices.CreateValueInfo<char>(options, JsonMetadataServices.CharConverter) : JsonMetadataServices.CreateValueInfo<char>(options, runtimeProvidedCustomConverter));
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<IList<char>> Create_IListChar(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<IList<char>> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(IList<char>))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<IList<char>>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<IList<char>> collectionInfo = new JsonCollectionInfoValues<IList<char>>
			{
				ObjectCreator = null,
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateIListInfo<IList<char>, char>(options, collectionInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<IList<char>[]> Create_IListCharArray(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<IList<char>[]> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(IList<char>[]))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<IList<char>[]>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<IList<char>[]> collectionInfo = new JsonCollectionInfoValues<IList<char>[]>
			{
				ObjectCreator = null,
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateArrayInfo(options, collectionInfo);
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

	private JsonTypeInfo<AutoPair[]> Create_AutoPairArray(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<AutoPair[]> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(AutoPair[]))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<AutoPair[]>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<AutoPair[]> collectionInfo = new JsonCollectionInfoValues<AutoPair[]>
			{
				ObjectCreator = null,
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateArrayInfo(options, collectionInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<AutoClosingPairs> Create_AutoClosingPairs(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<AutoClosingPairs> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(AutoClosingPairs))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<AutoClosingPairs>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<AutoClosingPairs> objectInfo = new JsonObjectInfoValues<AutoClosingPairs>
			{
				ObjectCreator = () => new AutoClosingPairs(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => AutoClosingPairsPropInit(options),
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

	private static JsonPropertyInfo[] AutoClosingPairsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<IList<char>[]> propertyInfo = new JsonPropertyInfoValues<IList<char>[]>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AutoClosingPairs),
			Converter = null,
			Getter = (object obj) => ((AutoClosingPairs)obj).CharPairs,
			Setter = delegate(object obj, IList<char>[]? value)
			{
				((AutoClosingPairs)obj).CharPairs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CharPairs",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<AutoPair[]> propertyInfo2 = new JsonPropertyInfoValues<AutoPair[]>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AutoClosingPairs),
			Converter = null,
			Getter = (object obj) => ((AutoClosingPairs)obj).AutoPairs,
			Setter = delegate(object obj, AutoPair[]? value)
			{
				((AutoClosingPairs)obj).AutoPairs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AutoPairs",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		return array;
	}

	private JsonTypeInfo<Indentation> Create_Indentation(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Indentation> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Indentation))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Indentation>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Indentation> objectInfo = new JsonObjectInfoValues<Indentation>
			{
				ObjectCreator = () => new Indentation(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => IndentationPropInit(options),
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

	private static JsonPropertyInfo[] IndentationPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Indentation),
			Converter = null,
			Getter = (object obj) => ((Indentation)obj).Increase,
			Setter = delegate(object obj, string? value)
			{
				((Indentation)obj).Increase = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Increase",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Indentation),
			Converter = null,
			Getter = (object obj) => ((Indentation)obj).Decrease,
			Setter = delegate(object obj, string? value)
			{
				((Indentation)obj).Decrease = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Decrease",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Indentation),
			Converter = null,
			Getter = (object obj) => ((Indentation)obj).Unindent,
			Setter = delegate(object obj, string? value)
			{
				((Indentation)obj).Unindent = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Unindent",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo3 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2] = jsonPropertyInfo3;
		JsonPropertyInfoValues<bool> propertyInfo4 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Indentation),
			Converter = null,
			Getter = (object obj) => ((Indentation)obj).IsEmpty,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IsEmpty",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo4 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3] = jsonPropertyInfo4;
		return array;
	}

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

	private JsonTypeInfo<IList<EnterRule>> Create_IListEnterRule(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<IList<EnterRule>> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(IList<EnterRule>))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<IList<EnterRule>>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<IList<EnterRule>> collectionInfo = new JsonCollectionInfoValues<IList<EnterRule>>
			{
				ObjectCreator = null,
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateIListInfo<IList<EnterRule>, EnterRule>(options, collectionInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<EnterRules> Create_EnterRules(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<EnterRules> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(EnterRules))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<EnterRules>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<EnterRules> objectInfo = new JsonObjectInfoValues<EnterRules>
			{
				ObjectCreator = () => new EnterRules(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => EnterRulesPropInit(options),
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

	private static JsonPropertyInfo[] EnterRulesPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[1];
		JsonPropertyInfoValues<IList<EnterRule>> propertyInfo = new JsonPropertyInfoValues<IList<EnterRule>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EnterRules),
			Converter = null,
			Getter = (object obj) => ((EnterRules)obj).Rules,
			Setter = delegate(object obj, IList<EnterRule>? value)
			{
				((EnterRules)obj).Rules = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Rules",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		return array;
	}

	private JsonTypeInfo<LanguageConfiguration> Create_LanguageConfiguration(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<LanguageConfiguration> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(LanguageConfiguration))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<LanguageConfiguration>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<LanguageConfiguration> objectInfo = new JsonObjectInfoValues<LanguageConfiguration>
			{
				ObjectCreator = () => new LanguageConfiguration(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => LanguageConfigurationPropInit(options),
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

	private static JsonPropertyInfo[] LanguageConfigurationPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[7];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LanguageConfiguration),
			Converter = null,
			Getter = (object obj) => ((LanguageConfiguration)obj).AutoCloseBefore,
			Setter = delegate(object obj, string? value)
			{
				((LanguageConfiguration)obj).AutoCloseBefore = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AutoCloseBefore",
			JsonPropertyName = "autoCloseBefore"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<Folding> propertyInfo2 = new JsonPropertyInfoValues<Folding>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LanguageConfiguration),
			Converter = null,
			Getter = (object obj) => ((LanguageConfiguration)obj).Folding,
			Setter = delegate(object obj, Folding? value)
			{
				((LanguageConfiguration)obj).Folding = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Folding",
			JsonPropertyName = "folding"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		JsonPropertyInfoValues<IList<string>[]> propertyInfo3 = new JsonPropertyInfoValues<IList<string>[]>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LanguageConfiguration),
			Converter = null,
			Getter = (object obj) => ((LanguageConfiguration)obj).Brackets,
			Setter = delegate(object obj, IList<string>[]? value)
			{
				((LanguageConfiguration)obj).Brackets = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Brackets",
			JsonPropertyName = "brackets"
		};
		JsonPropertyInfo jsonPropertyInfo3 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2] = jsonPropertyInfo3;
		JsonPropertyInfoValues<Comments> propertyInfo4 = new JsonPropertyInfoValues<Comments>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LanguageConfiguration),
			Converter = null,
			Getter = (object obj) => ((LanguageConfiguration)obj).Comments,
			Setter = delegate(object obj, Comments? value)
			{
				((LanguageConfiguration)obj).Comments = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Comments",
			JsonPropertyName = "comments"
		};
		JsonPropertyInfo jsonPropertyInfo4 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3] = jsonPropertyInfo4;
		JsonPropertyInfoValues<AutoClosingPairs> propertyInfo5 = new JsonPropertyInfoValues<AutoClosingPairs>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LanguageConfiguration),
			Converter = new ClosingPairJsonConverter(),
			Getter = (object obj) => ((LanguageConfiguration)obj).AutoClosingPairs,
			Setter = delegate(object obj, AutoClosingPairs? value)
			{
				((LanguageConfiguration)obj).AutoClosingPairs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AutoClosingPairs",
			JsonPropertyName = "autoClosingPairs"
		};
		JsonPropertyInfo jsonPropertyInfo5 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4] = jsonPropertyInfo5;
		JsonPropertyInfoValues<Indentation> propertyInfo6 = new JsonPropertyInfoValues<Indentation>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LanguageConfiguration),
			Converter = new IntentationRulesJsonConverter(),
			Getter = (object obj) => ((LanguageConfiguration)obj).IndentationRules,
			Setter = delegate(object obj, Indentation? value)
			{
				((LanguageConfiguration)obj).IndentationRules = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IndentationRules",
			JsonPropertyName = "indentationRules"
		};
		JsonPropertyInfo jsonPropertyInfo6 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		array[5] = jsonPropertyInfo6;
		JsonPropertyInfoValues<EnterRules> propertyInfo7 = new JsonPropertyInfoValues<EnterRules>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LanguageConfiguration),
			Converter = new EnterRulesJsonConverter(),
			Getter = (object obj) => ((LanguageConfiguration)obj).EnterRules,
			Setter = delegate(object obj, EnterRules? value)
			{
				((LanguageConfiguration)obj).EnterRules = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EnterRules",
			JsonPropertyName = "onEnterRules"
		};
		JsonPropertyInfo jsonPropertyInfo7 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		array[6] = jsonPropertyInfo7;
		return array;
	}

	public LanguageConfigurationSerializationContext()
		: base(null)
	{
	}

	public LanguageConfigurationSerializationContext(JsonSerializerOptions options)
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
		if (type == typeof(LanguageConfiguration))
		{
			return LanguageConfiguration;
		}
		if (type == typeof(string))
		{
			return String;
		}
		if (type == typeof(Folding))
		{
			return Folding;
		}
		if (type == typeof(bool))
		{
			return Boolean;
		}
		if (type == typeof(Markers))
		{
			return Markers;
		}
		if (type == typeof(IList<string>[]))
		{
			return IListStringArray;
		}
		if (type == typeof(IList<string>))
		{
			return IListString;
		}
		if (type == typeof(Comments))
		{
			return Comments;
		}
		if (type == typeof(AutoClosingPairs))
		{
			return AutoClosingPairs;
		}
		if (type == typeof(IList<char>[]))
		{
			return IListCharArray;
		}
		if (type == typeof(IList<char>))
		{
			return IListChar;
		}
		if (type == typeof(char))
		{
			return Char;
		}
		if (type == typeof(AutoPair[]))
		{
			return AutoPairArray;
		}
		if (type == typeof(AutoPair))
		{
			return AutoPair;
		}
		if (type == typeof(Indentation))
		{
			return Indentation;
		}
		if (type == typeof(EnterRules))
		{
			return EnterRules;
		}
		if (type == typeof(IList<EnterRule>))
		{
			return IListEnterRule;
		}
		if (type == typeof(EnterRule))
		{
			return EnterRule;
		}
		return null;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(LanguageConfiguration))
		{
			return Create_LanguageConfiguration(options, makeReadOnly: false);
		}
		if (type == typeof(string))
		{
			return Create_String(options, makeReadOnly: false);
		}
		if (type == typeof(Folding))
		{
			return Create_Folding(options, makeReadOnly: false);
		}
		if (type == typeof(bool))
		{
			return Create_Boolean(options, makeReadOnly: false);
		}
		if (type == typeof(Markers))
		{
			return Create_Markers(options, makeReadOnly: false);
		}
		if (type == typeof(IList<string>[]))
		{
			return Create_IListStringArray(options, makeReadOnly: false);
		}
		if (type == typeof(IList<string>))
		{
			return Create_IListString(options, makeReadOnly: false);
		}
		if (type == typeof(Comments))
		{
			return Create_Comments(options, makeReadOnly: false);
		}
		if (type == typeof(AutoClosingPairs))
		{
			return Create_AutoClosingPairs(options, makeReadOnly: false);
		}
		if (type == typeof(IList<char>[]))
		{
			return Create_IListCharArray(options, makeReadOnly: false);
		}
		if (type == typeof(IList<char>))
		{
			return Create_IListChar(options, makeReadOnly: false);
		}
		if (type == typeof(char))
		{
			return Create_Char(options, makeReadOnly: false);
		}
		if (type == typeof(AutoPair[]))
		{
			return Create_AutoPairArray(options, makeReadOnly: false);
		}
		if (type == typeof(AutoPair))
		{
			return Create_AutoPair(options, makeReadOnly: false);
		}
		if (type == typeof(Indentation))
		{
			return Create_Indentation(options, makeReadOnly: false);
		}
		if (type == typeof(EnterRules))
		{
			return Create_EnterRules(options, makeReadOnly: false);
		}
		if (type == typeof(IList<EnterRule>))
		{
			return Create_IListEnterRule(options, makeReadOnly: false);
		}
		if (type == typeof(EnterRule))
		{
			return Create_EnterRule(options, makeReadOnly: false);
		}
		return null;
	}
}
