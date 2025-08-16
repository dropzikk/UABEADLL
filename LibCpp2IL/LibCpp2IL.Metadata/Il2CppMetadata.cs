using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Logging;

namespace LibCpp2IL.Metadata;

public class Il2CppMetadata : ClassReadingBinaryReader
{
	public Il2CppGlobalMetadataHeader metadataHeader;

	public Il2CppAssemblyDefinition[] AssemblyDefinitions;

	public Il2CppImageDefinition[] imageDefinitions;

	public Il2CppTypeDefinition[] typeDefs;

	internal Il2CppInterfaceOffset[] interfaceOffsets;

	public uint[] VTableMethodIndices;

	public Il2CppMethodDefinition[] methodDefs;

	public Il2CppParameterDefinition[] parameterDefs;

	public Il2CppFieldDefinition[] fieldDefs;

	private Il2CppFieldDefaultValue[] fieldDefaultValues;

	private Il2CppParameterDefaultValue[] parameterDefaultValues;

	public Il2CppPropertyDefinition[] propertyDefs;

	public Il2CppCustomAttributeTypeRange[] attributeTypeRanges;

	private Il2CppStringLiteral[] stringLiterals;

	public Il2CppMetadataUsageList[] metadataUsageLists;

	private Il2CppMetadataUsagePair[] metadataUsagePairs;

	public Il2CppRGCTXDefinition[] RgctxDefinitions;

	public int[] attributeTypes;

	public int[] interfaceIndices;

	public List<Il2CppCustomAttributeDataRange> AttributeDataRanges;

	public Dictionary<uint, SortedDictionary<uint, uint>> metadataUsageDic;

	public long maxMetadataUsages;

	public int[] nestedTypeIndices;

	public Il2CppEventDefinition[] eventDefs;

	public Il2CppGenericContainer[] genericContainers;

	public Il2CppFieldRef[] fieldRefs;

	public Il2CppGenericParameter[] genericParameters;

	public int[] constraintIndices;

	private readonly Dictionary<int, Il2CppFieldDefaultValue> _fieldDefaultValueLookup = new Dictionary<int, Il2CppFieldDefaultValue>();

	private readonly Dictionary<Il2CppFieldDefinition, Il2CppFieldDefaultValue> _fieldDefaultLookupNew = new Dictionary<Il2CppFieldDefinition, Il2CppFieldDefaultValue>();

	private ConcurrentDictionary<int, string> _cachedStrings = new ConcurrentDictionary<int, string>();

	private ConcurrentDictionary<Il2CppImageDefinition, Il2CppCustomAttributeTypeRange[]> _typeRangesByAssembly = new ConcurrentDictionary<Il2CppImageDefinition, Il2CppCustomAttributeTypeRange[]>();

	public static Il2CppMetadata? ReadFrom(byte[] bytes, int[] unityVer)
	{
		if (BitConverter.ToUInt32(bytes, 0) != 4205910959u)
		{
			throw new FormatException("Invalid or corrupt metadata (magic number check failed)");
		}
		int num = BitConverter.ToInt32(bytes, 4);
		if (num < 24 || num > 29)
		{
			throw new FormatException("Unsupported metadata version found! We support 24-29, got " + num);
		}
		LibLogger.VerboseNewline($"\tIL2CPP Metadata Declares its version as {num}");
		UnityVersion unityVersion = UnityVersion.Parse(string.Join(".", unityVer));
		float num2 = num switch
		{
			27 => (!unityVersion.IsGreaterEqual(2020, 2, 4)) ? ((float)num) : 27.1f, 
			24 => (!unityVersion.IsGreaterEqual(2020, 1, 11)) ? ((!unityVersion.IsGreaterEqual(2020)) ? ((!unityVersion.IsGreaterEqual(2019, 4, 21)) ? ((!unityVersion.IsGreaterEqual(2019, 4, 15)) ? ((!unityVersion.IsGreaterEqual(2019, 3, 7)) ? ((!unityVersion.IsGreaterEqual(2019)) ? ((!unityVersion.IsGreaterEqual(2018, 4, 34)) ? ((!unityVersion.IsGreaterEqual(2018, 3)) ? ((float)num) : 24.1f) : 24.15f) : 24.2f) : 24.3f) : 24.4f) : 24.5f) : 24.3f) : 24.4f, 
			29 => (!unityVersion.IsGreaterEqual(2022, 1, 0, UnityVersionType.Beta, 7)) ? 29f : 29.1f, 
			_ => num, 
		};
		LibLogger.InfoNewline($"\tUsing actual IL2CPP Metadata version {num2}");
		LibCpp2IlMain.MetadataVersion = num2;
		return new Il2CppMetadata(new MemoryStream(bytes));
	}

	private Il2CppMetadata(MemoryStream stream)
		: base(stream)
	{
		metadataHeader = ReadClassAtRawAddr<Il2CppGlobalMetadataHeader>(-1L);
		if (metadataHeader.magicNumber != 4205910959u)
		{
			throw new Exception("ERROR: Magic number mismatch. Expecting " + 4205910959u + " but got " + metadataHeader.magicNumber);
		}
		if (metadataHeader.version < 24)
		{
			throw new Exception("ERROR: Invalid metadata version, we only support v24+, this metadata is using v" + metadataHeader.version);
		}
		LibLogger.Verbose("\tReading image definitions...");
		DateTime now = DateTime.Now;
		imageDefinitions = ReadMetadataClassArray<Il2CppImageDefinition>(metadataHeader.imagesOffset, metadataHeader.imagesCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading assembly definitions...");
		now = DateTime.Now;
		AssemblyDefinitions = ReadMetadataClassArray<Il2CppAssemblyDefinition>(metadataHeader.assembliesOffset, metadataHeader.assembliesCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading type definitions...");
		now = DateTime.Now;
		typeDefs = ReadMetadataClassArray<Il2CppTypeDefinition>(metadataHeader.typeDefinitionsOffset, metadataHeader.typeDefinitionsCount);
		LibLogger.VerboseNewline($"{typeDefs.Length} OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading interface offsets...");
		now = DateTime.Now;
		interfaceOffsets = ReadMetadataClassArray<Il2CppInterfaceOffset>(metadataHeader.interfaceOffsetsOffset, metadataHeader.interfaceOffsetsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading vtable indices...");
		now = DateTime.Now;
		VTableMethodIndices = ReadMetadataClassArray<uint>(metadataHeader.vtableMethodsOffset, metadataHeader.vtableMethodsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading method definitions...");
		now = DateTime.Now;
		methodDefs = ReadMetadataClassArray<Il2CppMethodDefinition>(metadataHeader.methodsOffset, metadataHeader.methodsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading method parameter definitions...");
		now = DateTime.Now;
		parameterDefs = ReadMetadataClassArray<Il2CppParameterDefinition>(metadataHeader.parametersOffset, metadataHeader.parametersCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading field definitions...");
		now = DateTime.Now;
		fieldDefs = ReadMetadataClassArray<Il2CppFieldDefinition>(metadataHeader.fieldsOffset, metadataHeader.fieldsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading default field values...");
		now = DateTime.Now;
		fieldDefaultValues = ReadMetadataClassArray<Il2CppFieldDefaultValue>(metadataHeader.fieldDefaultValuesOffset, metadataHeader.fieldDefaultValuesCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading default parameter values...");
		now = DateTime.Now;
		parameterDefaultValues = ReadMetadataClassArray<Il2CppParameterDefaultValue>(metadataHeader.parameterDefaultValuesOffset, metadataHeader.parameterDefaultValuesCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading property definitions...");
		now = DateTime.Now;
		propertyDefs = ReadMetadataClassArray<Il2CppPropertyDefinition>(metadataHeader.propertiesOffset, metadataHeader.propertiesCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading interface definitions...");
		now = DateTime.Now;
		interfaceIndices = ReadClassArrayAtRawAddr<int>(metadataHeader.interfacesOffset, metadataHeader.interfacesCount / 4);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading nested type definitions...");
		now = DateTime.Now;
		nestedTypeIndices = ReadClassArrayAtRawAddr<int>(metadataHeader.nestedTypesOffset, metadataHeader.nestedTypesCount / 4);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading event definitions...");
		now = DateTime.Now;
		eventDefs = ReadMetadataClassArray<Il2CppEventDefinition>(metadataHeader.eventsOffset, metadataHeader.eventsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading generic container definitions...");
		now = DateTime.Now;
		genericContainers = ReadMetadataClassArray<Il2CppGenericContainer>(metadataHeader.genericContainersOffset, metadataHeader.genericContainersCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading generic parameter definitions...");
		now = DateTime.Now;
		genericParameters = ReadMetadataClassArray<Il2CppGenericParameter>(metadataHeader.genericParametersOffset, metadataHeader.genericParametersCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading generic parameter constraint indices...");
		now = DateTime.Now;
		constraintIndices = ReadMetadataClassArray<int>(metadataHeader.genericParameterConstraintsOffset, metadataHeader.genericParameterConstraintsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading string definitions...");
		now = DateTime.Now;
		stringLiterals = ReadMetadataClassArray<Il2CppStringLiteral>(metadataHeader.stringLiteralOffset, metadataHeader.stringLiteralCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		if (LibCpp2IlMain.MetadataVersion < 24.2f)
		{
			LibLogger.Verbose("\tReading RGCTX data...");
			now = DateTime.Now;
			RgctxDefinitions = ReadMetadataClassArray<Il2CppRGCTXDefinition>(metadataHeader.rgctxEntriesOffset, metadataHeader.rgctxEntriesCount);
			LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		}
		if (LibCpp2IlMain.MetadataVersion < 27f)
		{
			LibLogger.Verbose("\tReading usage data...");
			now = DateTime.Now;
			metadataUsageLists = ReadMetadataClassArray<Il2CppMetadataUsageList>(metadataHeader.metadataUsageListsOffset, metadataHeader.metadataUsageListsCount);
			metadataUsagePairs = ReadMetadataClassArray<Il2CppMetadataUsagePair>(metadataHeader.metadataUsagePairsOffset, metadataHeader.metadataUsagePairsCount);
			DecipherMetadataUsage();
			LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		}
		LibLogger.Verbose("\tReading field references...");
		now = DateTime.Now;
		fieldRefs = ReadMetadataClassArray<Il2CppFieldRef>(metadataHeader.fieldRefsOffset, metadataHeader.fieldRefsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		if (LibCpp2IlMain.MetadataVersion < 29f)
		{
			LibLogger.Verbose("\tReading attribute types...");
			now = DateTime.Now;
			attributeTypeRanges = ReadMetadataClassArray<Il2CppCustomAttributeTypeRange>(metadataHeader.attributesInfoOffset, metadataHeader.attributesInfoCount);
			attributeTypes = ReadClassArrayAtRawAddr<int>(metadataHeader.attributeTypesOffset, metadataHeader.attributeTypesCount / 4);
			LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		}
		else
		{
			LibLogger.Verbose("\tReading Attribute data...");
			now = DateTime.Now;
			AttributeDataRanges = ReadClassArrayAtRawAddr<Il2CppCustomAttributeDataRange>(metadataHeader.attributeDataRangeOffset, metadataHeader.attributeDataRangeCount / 8).ToList();
			LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		}
		LibLogger.Verbose("\tBuilding Lookup Table for field defaults...");
		now = DateTime.Now;
		Il2CppFieldDefaultValue[] array = fieldDefaultValues;
		foreach (Il2CppFieldDefaultValue il2CppFieldDefaultValue in array)
		{
			_fieldDefaultValueLookup[il2CppFieldDefaultValue.fieldIndex] = il2CppFieldDefaultValue;
			_fieldDefaultLookupNew[fieldDefs[il2CppFieldDefaultValue.fieldIndex]] = il2CppFieldDefaultValue;
		}
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
	}

	private T[] ReadMetadataClassArray<T>(int offset, int length) where T : new()
	{
		return ReadClassArrayAtRawAddr<T>(offset, length / LibCpp2ILUtils.VersionAwareSizeOf(typeof(T), dontCheckVersionAttributes: false, downsize: false));
	}

	private void DecipherMetadataUsage()
	{
		metadataUsageDic = new Dictionary<uint, SortedDictionary<uint, uint>>();
		for (uint num = 1u; num <= 6; num++)
		{
			metadataUsageDic[num] = new SortedDictionary<uint, uint>();
		}
		Il2CppMetadataUsageList[] array = metadataUsageLists;
		foreach (Il2CppMetadataUsageList il2CppMetadataUsageList in array)
		{
			for (int j = 0; j < il2CppMetadataUsageList.count; j++)
			{
				long num2 = il2CppMetadataUsageList.start + j;
				Il2CppMetadataUsagePair il2CppMetadataUsagePair = metadataUsagePairs[num2];
				uint encodedIndexType = GetEncodedIndexType(il2CppMetadataUsagePair.encodedSourceIndex);
				uint decodedMethodIndex = GetDecodedMethodIndex(il2CppMetadataUsagePair.encodedSourceIndex);
				metadataUsageDic[encodedIndexType][il2CppMetadataUsagePair.destinationIndex] = decodedMethodIndex;
			}
		}
		maxMetadataUsages = metadataUsageDic.Max<KeyValuePair<uint, SortedDictionary<uint, uint>>, uint>((KeyValuePair<uint, SortedDictionary<uint, uint>> x) => x.Value.Max((KeyValuePair<uint, uint> y) => y.Key)) + 1;
	}

	private uint GetEncodedIndexType(uint index)
	{
		return (index & 0xE0000000u) >> 29;
	}

	private uint GetDecodedMethodIndex(uint index)
	{
		return index & 0x1FFFFFFF;
	}

	public Il2CppFieldDefaultValue? GetFieldDefaultValueFromIndex(int index)
	{
		return _fieldDefaultValueLookup.GetValueOrDefault(index);
	}

	public Il2CppFieldDefaultValue? GetFieldDefaultValue(Il2CppFieldDefinition field)
	{
		return _fieldDefaultLookupNew.GetValueOrDefault(field);
	}

	public (int ptr, int type) GetFieldDefaultValue(int fieldIdx)
	{
		Il2CppFieldDefinition il2CppFieldDefinition = fieldDefs[fieldIdx];
		if ((LibCpp2IlMain.Binary.GetType(il2CppFieldDefinition.typeIndex).attrs & 0x100) != 0)
		{
			Il2CppFieldDefaultValue fieldDefaultValueFromIndex = GetFieldDefaultValueFromIndex(fieldIdx);
			if (fieldDefaultValueFromIndex == null)
			{
				return (ptr: -1, type: -1);
			}
			return (ptr: fieldDefaultValueFromIndex.dataIndex, type: fieldDefaultValueFromIndex.typeIndex);
		}
		return (ptr: -1, type: -1);
	}

	public Il2CppParameterDefaultValue? GetParameterDefaultValueFromIndex(int index)
	{
		return parameterDefaultValues.FirstOrDefault((Il2CppParameterDefaultValue x) => x.parameterIndex == index);
	}

	public int GetDefaultValueFromIndex(int index)
	{
		return metadataHeader.fieldAndParameterDefaultValueDataOffset + index;
	}

	public string GetStringFromIndex(int index)
	{
		if (!_cachedStrings.ContainsKey(index))
		{
			_cachedStrings[index] = ReadStringToNull(metadataHeader.stringOffset + index);
		}
		return _cachedStrings[index];
	}

	public Il2CppCustomAttributeTypeRange? GetCustomAttributeData(Il2CppImageDefinition imageDef, int customAttributeIndex, uint token)
	{
		if (LibCpp2IlMain.MetadataVersion <= 24f)
		{
			return attributeTypeRanges[customAttributeIndex];
		}
		Il2CppCustomAttributeTypeRange[] array;
		lock (_typeRangesByAssembly)
		{
			if (!_typeRangesByAssembly.ContainsKey(imageDef))
			{
				array = attributeTypeRanges.SubArray(imageDef.customAttributeStart, (int)imageDef.customAttributeCount);
				_typeRangesByAssembly.TryAdd(imageDef, array);
			}
			else
			{
				array = _typeRangesByAssembly[imageDef];
			}
		}
		Il2CppCustomAttributeTypeRange[] array2 = array;
		foreach (Il2CppCustomAttributeTypeRange il2CppCustomAttributeTypeRange in array2)
		{
			if (il2CppCustomAttributeTypeRange.token == token)
			{
				return il2CppCustomAttributeTypeRange;
			}
		}
		return null;
	}

	public string GetStringLiteralFromIndex(uint index)
	{
		Il2CppStringLiteral il2CppStringLiteral = stringLiterals[index];
		return Encoding.UTF8.GetString(ReadByteArrayAtRawAddress(metadataHeader.stringLiteralDataOffset + il2CppStringLiteral.dataIndex, (int)il2CppStringLiteral.length));
	}
}
