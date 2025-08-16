using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextMateSharp.Grammars;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(GrammarDefinition))]
[GeneratedCode("System.Text.Json.SourceGeneration", "7.0.8.6910")]
internal sealed class GrammarDefinitionSerializationContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<string>? _String;

	private JsonTypeInfo<Engines>? _Engines;

	private JsonTypeInfo<Scripts>? _Scripts;

	private JsonTypeInfo<List<string>>? _ListString;

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

	private JsonTypeInfo<Language>? _Language;

	private JsonTypeInfo<List<Language>>? _ListLanguage;

	private JsonTypeInfo<Grammar>? _Grammar;

	private JsonTypeInfo<List<Grammar>>? _ListGrammar;

	private JsonTypeInfo<Snippet>? _Snippet;

	private JsonTypeInfo<List<Snippet>>? _ListSnippet;

	private JsonTypeInfo<Contributes>? _Contributes;

	private JsonTypeInfo<Repository>? _Repository;

	private JsonTypeInfo<LanguageSnippets>? _LanguageSnippets;

	private JsonTypeInfo<GrammarDefinition>? _GrammarDefinition;

	private static GrammarDefinitionSerializationContext? s_defaultContext;

	public JsonTypeInfo<string> String => _String ?? (_String = Create_String(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Engines> Engines => _Engines ?? (_Engines = Create_Engines(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Scripts> Scripts => _Scripts ?? (_Scripts = Create_Scripts(base.Options, makeReadOnly: true));

	public JsonTypeInfo<List<string>> ListString => _ListString ?? (_ListString = Create_ListString(base.Options, makeReadOnly: true));

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

	public JsonTypeInfo<Language> Language => _Language ?? (_Language = Create_Language(base.Options, makeReadOnly: true));

	public JsonTypeInfo<List<Language>> ListLanguage => _ListLanguage ?? (_ListLanguage = Create_ListLanguage(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Grammar> Grammar => _Grammar ?? (_Grammar = Create_Grammar(base.Options, makeReadOnly: true));

	public JsonTypeInfo<List<Grammar>> ListGrammar => _ListGrammar ?? (_ListGrammar = Create_ListGrammar(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Snippet> Snippet => _Snippet ?? (_Snippet = Create_Snippet(base.Options, makeReadOnly: true));

	public JsonTypeInfo<List<Snippet>> ListSnippet => _ListSnippet ?? (_ListSnippet = Create_ListSnippet(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Contributes> Contributes => _Contributes ?? (_Contributes = Create_Contributes(base.Options, makeReadOnly: true));

	public JsonTypeInfo<Repository> Repository => _Repository ?? (_Repository = Create_Repository(base.Options, makeReadOnly: true));

	public JsonTypeInfo<LanguageSnippets> LanguageSnippets => _LanguageSnippets ?? (_LanguageSnippets = Create_LanguageSnippets(base.Options, makeReadOnly: true));

	public JsonTypeInfo<GrammarDefinition> GrammarDefinition => _GrammarDefinition ?? (_GrammarDefinition = Create_GrammarDefinition(base.Options, makeReadOnly: true));

	private static JsonSerializerOptions s_defaultOptions { get; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		IgnoreReadOnlyFields = false,
		IgnoreReadOnlyProperties = false,
		IncludeFields = false,
		WriteIndented = false
	};

	public static GrammarDefinitionSerializationContext Default => s_defaultContext ?? (s_defaultContext = new GrammarDefinitionSerializationContext(new JsonSerializerOptions(s_defaultOptions)));

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

	private JsonTypeInfo<Engines> Create_Engines(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Engines> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Engines))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Engines>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Engines> objectInfo = new JsonObjectInfoValues<Engines>
			{
				ObjectCreator = () => new Engines(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => EnginesPropInit(options),
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

	private static JsonPropertyInfo[] EnginesPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[1];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Engines),
			Converter = null,
			Getter = (object obj) => ((Engines)obj).VsCode,
			Setter = delegate(object obj, string? value)
			{
				((Engines)obj).VsCode = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "VsCode",
			JsonPropertyName = "engines"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		return array;
	}

	private JsonTypeInfo<Scripts> Create_Scripts(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Scripts> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Scripts))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Scripts>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Scripts> objectInfo = new JsonObjectInfoValues<Scripts>
			{
				ObjectCreator = () => new Scripts(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => ScriptsPropInit(options),
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

	private static JsonPropertyInfo[] ScriptsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[1];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Scripts),
			Converter = null,
			Getter = (object obj) => ((Scripts)obj).UpdateGrammar,
			Setter = delegate(object obj, string? value)
			{
				((Scripts)obj).UpdateGrammar = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UpdateGrammar",
			JsonPropertyName = "update-grammar"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		return array;
	}

	private JsonTypeInfo<List<string>> Create_ListString(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<List<string>> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(List<string>))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<List<string>>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<List<string>> collectionInfo = new JsonCollectionInfoValues<List<string>>
			{
				ObjectCreator = () => new List<string>(),
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<string>, string>(options, collectionInfo);
		}
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

	private JsonTypeInfo<Language> Create_Language(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Language> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Language))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Language>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Language> objectInfo = new JsonObjectInfoValues<Language>
			{
				ObjectCreator = () => new Language(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => LanguagePropInit(options),
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

	private static JsonPropertyInfo[] LanguagePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[5];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Language),
			Converter = null,
			Getter = (object obj) => ((Language)obj).Id,
			Setter = delegate(object obj, string? value)
			{
				((Language)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<List<string>> propertyInfo2 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Language),
			Converter = null,
			Getter = (object obj) => ((Language)obj).Extensions,
			Setter = delegate(object obj, List<string>? value)
			{
				((Language)obj).Extensions = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Extensions",
			JsonPropertyName = "extensions"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		JsonPropertyInfoValues<List<string>> propertyInfo3 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Language),
			Converter = null,
			Getter = (object obj) => ((Language)obj).Aliases,
			Setter = delegate(object obj, List<string>? value)
			{
				((Language)obj).Aliases = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Aliases",
			JsonPropertyName = "aliases"
		};
		JsonPropertyInfo jsonPropertyInfo3 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2] = jsonPropertyInfo3;
		JsonPropertyInfoValues<string> propertyInfo4 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Language),
			Converter = null,
			Getter = (object obj) => ((Language)obj).ConfigurationFile,
			Setter = delegate(object obj, string? value)
			{
				((Language)obj).ConfigurationFile = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ConfigurationFile",
			JsonPropertyName = "configuration"
		};
		JsonPropertyInfo jsonPropertyInfo4 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3] = jsonPropertyInfo4;
		JsonPropertyInfoValues<LanguageConfiguration> propertyInfo5 = new JsonPropertyInfoValues<LanguageConfiguration>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Language),
			Converter = null,
			Getter = (object obj) => ((Language)obj).Configuration,
			Setter = delegate(object obj, LanguageConfiguration? value)
			{
				((Language)obj).Configuration = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Configuration",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo5 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4] = jsonPropertyInfo5;
		return array;
	}

	private JsonTypeInfo<List<Language>> Create_ListLanguage(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<List<Language>> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(List<Language>))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<List<Language>>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<List<Language>> collectionInfo = new JsonCollectionInfoValues<List<Language>>
			{
				ObjectCreator = () => new List<Language>(),
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<Language>, Language>(options, collectionInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Grammar> Create_Grammar(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Grammar> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Grammar))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Grammar>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Grammar> objectInfo = new JsonObjectInfoValues<Grammar>
			{
				ObjectCreator = () => new Grammar(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => GrammarPropInit(options),
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

	private static JsonPropertyInfo[] GrammarPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Grammar),
			Converter = null,
			Getter = (object obj) => ((Grammar)obj).Language,
			Setter = delegate(object obj, string? value)
			{
				((Grammar)obj).Language = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Language",
			JsonPropertyName = "language"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Grammar),
			Converter = null,
			Getter = (object obj) => ((Grammar)obj).ScopeName,
			Setter = delegate(object obj, string? value)
			{
				((Grammar)obj).ScopeName = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ScopeName",
			JsonPropertyName = "scopeName"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Grammar),
			Converter = null,
			Getter = (object obj) => ((Grammar)obj).Path,
			Setter = delegate(object obj, string? value)
			{
				((Grammar)obj).Path = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Path",
			JsonPropertyName = "path"
		};
		JsonPropertyInfo jsonPropertyInfo3 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2] = jsonPropertyInfo3;
		return array;
	}

	private JsonTypeInfo<List<Grammar>> Create_ListGrammar(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<List<Grammar>> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(List<Grammar>))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<List<Grammar>>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<List<Grammar>> collectionInfo = new JsonCollectionInfoValues<List<Grammar>>
			{
				ObjectCreator = () => new List<Grammar>(),
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<Grammar>, Grammar>(options, collectionInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Snippet> Create_Snippet(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Snippet> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Snippet))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Snippet>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Snippet> objectInfo = new JsonObjectInfoValues<Snippet>
			{
				ObjectCreator = () => new Snippet(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SnippetPropInit(options),
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

	private static JsonPropertyInfo[] SnippetPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Snippet),
			Converter = null,
			Getter = (object obj) => ((Snippet)obj).Language,
			Setter = delegate(object obj, string? value)
			{
				((Snippet)obj).Language = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Language",
			JsonPropertyName = "language"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Snippet),
			Converter = null,
			Getter = (object obj) => ((Snippet)obj).Path,
			Setter = delegate(object obj, string? value)
			{
				((Snippet)obj).Path = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Path",
			JsonPropertyName = "path"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		return array;
	}

	private JsonTypeInfo<List<Snippet>> Create_ListSnippet(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<List<Snippet>> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(List<Snippet>))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<List<Snippet>>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonCollectionInfoValues<List<Snippet>> collectionInfo = new JsonCollectionInfoValues<List<Snippet>>
			{
				ObjectCreator = () => new List<Snippet>(),
				NumberHandling = JsonNumberHandling.Strict,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<Snippet>, Snippet>(options, collectionInfo);
		}
		if (makeReadOnly)
		{
			jsonTypeInfo.MakeReadOnly();
		}
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Contributes> Create_Contributes(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Contributes> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Contributes))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Contributes>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Contributes> objectInfo = new JsonObjectInfoValues<Contributes>
			{
				ObjectCreator = () => new Contributes(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => ContributesPropInit(options),
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

	private static JsonPropertyInfo[] ContributesPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<List<Language>> propertyInfo = new JsonPropertyInfoValues<List<Language>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Contributes),
			Converter = null,
			Getter = (object obj) => ((Contributes)obj).Languages,
			Setter = delegate(object obj, List<Language>? value)
			{
				((Contributes)obj).Languages = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Languages",
			JsonPropertyName = "languages"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<List<Grammar>> propertyInfo2 = new JsonPropertyInfoValues<List<Grammar>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Contributes),
			Converter = null,
			Getter = (object obj) => ((Contributes)obj).Grammars,
			Setter = delegate(object obj, List<Grammar>? value)
			{
				((Contributes)obj).Grammars = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Grammars",
			JsonPropertyName = "grammars"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		JsonPropertyInfoValues<List<Snippet>> propertyInfo3 = new JsonPropertyInfoValues<List<Snippet>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Contributes),
			Converter = null,
			Getter = (object obj) => ((Contributes)obj).Snippets,
			Setter = delegate(object obj, List<Snippet>? value)
			{
				((Contributes)obj).Snippets = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Snippets",
			JsonPropertyName = "snippets"
		};
		JsonPropertyInfo jsonPropertyInfo3 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2] = jsonPropertyInfo3;
		return array;
	}

	private JsonTypeInfo<Repository> Create_Repository(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<Repository> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(Repository))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<Repository>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<Repository> objectInfo = new JsonObjectInfoValues<Repository>
			{
				ObjectCreator = () => new Repository(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => RepositoryPropInit(options),
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

	private static JsonPropertyInfo[] RepositoryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Repository),
			Converter = null,
			Getter = (object obj) => ((Repository)obj).Type,
			Setter = delegate(object obj, string? value)
			{
				((Repository)obj).Type = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Type",
			JsonPropertyName = "type"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Repository),
			Converter = null,
			Getter = (object obj) => ((Repository)obj).Url,
			Setter = delegate(object obj, string? value)
			{
				((Repository)obj).Url = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Url",
			JsonPropertyName = "url"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		return array;
	}

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

	private JsonTypeInfo<GrammarDefinition> Create_GrammarDefinition(JsonSerializerOptions options, bool makeReadOnly)
	{
		JsonTypeInfo<GrammarDefinition> jsonTypeInfo = null;
		JsonConverter runtimeProvidedCustomConverter;
		if (options.Converters.Count > 0 && (runtimeProvidedCustomConverter = GetRuntimeProvidedCustomConverter(options, typeof(GrammarDefinition))) != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<GrammarDefinition>(options, runtimeProvidedCustomConverter);
		}
		else
		{
			JsonObjectInfoValues<GrammarDefinition> objectInfo = new JsonObjectInfoValues<GrammarDefinition>
			{
				ObjectCreator = () => new GrammarDefinition(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => GrammarDefinitionPropInit(options),
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

	private static JsonPropertyInfo[] GrammarDefinitionPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[11];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).Name,
			Setter = delegate(object obj, string? value)
			{
				((GrammarDefinition)obj).Name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Name",
			JsonPropertyName = "name"
		};
		JsonPropertyInfo jsonPropertyInfo = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0] = jsonPropertyInfo;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).DisplayName,
			Setter = delegate(object obj, string? value)
			{
				((GrammarDefinition)obj).DisplayName = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DisplayName",
			JsonPropertyName = "displayName"
		};
		JsonPropertyInfo jsonPropertyInfo2 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1] = jsonPropertyInfo2;
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).Description,
			Setter = delegate(object obj, string? value)
			{
				((GrammarDefinition)obj).Description = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Description",
			JsonPropertyName = "description"
		};
		JsonPropertyInfo jsonPropertyInfo3 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2] = jsonPropertyInfo3;
		JsonPropertyInfoValues<string> propertyInfo4 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).Version,
			Setter = delegate(object obj, string? value)
			{
				((GrammarDefinition)obj).Version = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Version",
			JsonPropertyName = "version"
		};
		JsonPropertyInfo jsonPropertyInfo4 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3] = jsonPropertyInfo4;
		JsonPropertyInfoValues<string> propertyInfo5 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).Publisher,
			Setter = delegate(object obj, string? value)
			{
				((GrammarDefinition)obj).Publisher = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Publisher",
			JsonPropertyName = "publisher"
		};
		JsonPropertyInfo jsonPropertyInfo5 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4] = jsonPropertyInfo5;
		JsonPropertyInfoValues<string> propertyInfo6 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).License,
			Setter = delegate(object obj, string? value)
			{
				((GrammarDefinition)obj).License = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "License",
			JsonPropertyName = "license"
		};
		JsonPropertyInfo jsonPropertyInfo6 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		array[5] = jsonPropertyInfo6;
		JsonPropertyInfoValues<Engines> propertyInfo7 = new JsonPropertyInfoValues<Engines>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).Engines,
			Setter = delegate(object obj, Engines? value)
			{
				((GrammarDefinition)obj).Engines = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Engines",
			JsonPropertyName = "engines"
		};
		JsonPropertyInfo jsonPropertyInfo7 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		array[6] = jsonPropertyInfo7;
		JsonPropertyInfoValues<Scripts> propertyInfo8 = new JsonPropertyInfoValues<Scripts>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).Scripts,
			Setter = delegate(object obj, Scripts? value)
			{
				((GrammarDefinition)obj).Scripts = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Scripts",
			JsonPropertyName = "scripts"
		};
		JsonPropertyInfo jsonPropertyInfo8 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		array[7] = jsonPropertyInfo8;
		JsonPropertyInfoValues<Contributes> propertyInfo9 = new JsonPropertyInfoValues<Contributes>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).Contributes,
			Setter = delegate(object obj, Contributes? value)
			{
				((GrammarDefinition)obj).Contributes = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Contributes",
			JsonPropertyName = "contributes"
		};
		JsonPropertyInfo jsonPropertyInfo9 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		array[8] = jsonPropertyInfo9;
		JsonPropertyInfoValues<Repository> propertyInfo10 = new JsonPropertyInfoValues<Repository>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).Repository,
			Setter = delegate(object obj, Repository? value)
			{
				((GrammarDefinition)obj).Repository = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Repository",
			JsonPropertyName = "repository"
		};
		JsonPropertyInfo jsonPropertyInfo10 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		array[9] = jsonPropertyInfo10;
		JsonPropertyInfoValues<LanguageSnippets> propertyInfo11 = new JsonPropertyInfoValues<LanguageSnippets>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(GrammarDefinition),
			Converter = null,
			Getter = (object obj) => ((GrammarDefinition)obj).LanguageSnippets,
			Setter = delegate(object obj, LanguageSnippets? value)
			{
				((GrammarDefinition)obj).LanguageSnippets = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "LanguageSnippets",
			JsonPropertyName = null
		};
		JsonPropertyInfo jsonPropertyInfo11 = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		array[10] = jsonPropertyInfo11;
		return array;
	}

	public GrammarDefinitionSerializationContext()
		: base(null)
	{
	}

	public GrammarDefinitionSerializationContext(JsonSerializerOptions options)
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
		if (type == typeof(GrammarDefinition))
		{
			return GrammarDefinition;
		}
		if (type == typeof(string))
		{
			return String;
		}
		if (type == typeof(Engines))
		{
			return Engines;
		}
		if (type == typeof(Scripts))
		{
			return Scripts;
		}
		if (type == typeof(Contributes))
		{
			return Contributes;
		}
		if (type == typeof(List<Language>))
		{
			return ListLanguage;
		}
		if (type == typeof(Language))
		{
			return Language;
		}
		if (type == typeof(List<string>))
		{
			return ListString;
		}
		if (type == typeof(LanguageConfiguration))
		{
			return LanguageConfiguration;
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
		if (type == typeof(List<Grammar>))
		{
			return ListGrammar;
		}
		if (type == typeof(Grammar))
		{
			return Grammar;
		}
		if (type == typeof(List<Snippet>))
		{
			return ListSnippet;
		}
		if (type == typeof(Snippet))
		{
			return Snippet;
		}
		if (type == typeof(Repository))
		{
			return Repository;
		}
		if (type == typeof(LanguageSnippets))
		{
			return LanguageSnippets;
		}
		return null;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(GrammarDefinition))
		{
			return Create_GrammarDefinition(options, makeReadOnly: false);
		}
		if (type == typeof(string))
		{
			return Create_String(options, makeReadOnly: false);
		}
		if (type == typeof(Engines))
		{
			return Create_Engines(options, makeReadOnly: false);
		}
		if (type == typeof(Scripts))
		{
			return Create_Scripts(options, makeReadOnly: false);
		}
		if (type == typeof(Contributes))
		{
			return Create_Contributes(options, makeReadOnly: false);
		}
		if (type == typeof(List<Language>))
		{
			return Create_ListLanguage(options, makeReadOnly: false);
		}
		if (type == typeof(Language))
		{
			return Create_Language(options, makeReadOnly: false);
		}
		if (type == typeof(List<string>))
		{
			return Create_ListString(options, makeReadOnly: false);
		}
		if (type == typeof(LanguageConfiguration))
		{
			return Create_LanguageConfiguration(options, makeReadOnly: false);
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
		if (type == typeof(List<Grammar>))
		{
			return Create_ListGrammar(options, makeReadOnly: false);
		}
		if (type == typeof(Grammar))
		{
			return Create_Grammar(options, makeReadOnly: false);
		}
		if (type == typeof(List<Snippet>))
		{
			return Create_ListSnippet(options, makeReadOnly: false);
		}
		if (type == typeof(Snippet))
		{
			return Create_Snippet(options, makeReadOnly: false);
		}
		if (type == typeof(Repository))
		{
			return Create_Repository(options, makeReadOnly: false);
		}
		if (type == typeof(LanguageSnippets))
		{
			return Create_LanguageSnippets(options, makeReadOnly: false);
		}
		return null;
	}
}
