namespace LibCpp2IL.Metadata;

public class Il2CppGlobalMetadataHeader
{
	public uint magicNumber;

	public int version;

	public int stringLiteralOffset;

	public int stringLiteralCount;

	public int stringLiteralDataOffset;

	public int stringLiteralDataCount;

	public int stringOffset;

	public int stringCount;

	public int eventsOffset;

	public int eventsCount;

	public int propertiesOffset;

	public int propertiesCount;

	public int methodsOffset;

	public int methodsCount;

	public int parameterDefaultValuesOffset;

	public int parameterDefaultValuesCount;

	public int fieldDefaultValuesOffset;

	public int fieldDefaultValuesCount;

	public int fieldAndParameterDefaultValueDataOffset;

	public int fieldAndParameterDefaultValueDataCount;

	public int fieldMarshaledSizesOffset;

	public int fieldMarshaledSizesCount;

	public int parametersOffset;

	public int parametersCount;

	public int fieldsOffset;

	public int fieldsCount;

	public int genericParametersOffset;

	public int genericParametersCount;

	public int genericParameterConstraintsOffset;

	public int genericParameterConstraintsCount;

	public int genericContainersOffset;

	public int genericContainersCount;

	public int nestedTypesOffset;

	public int nestedTypesCount;

	public int interfacesOffset;

	public int interfacesCount;

	public int vtableMethodsOffset;

	public int vtableMethodsCount;

	public int interfaceOffsetsOffset;

	public int interfaceOffsetsCount;

	public int typeDefinitionsOffset;

	public int typeDefinitionsCount;

	[Version(Max = 24.15f)]
	public int rgctxEntriesOffset;

	[Version(Max = 24.15f)]
	public int rgctxEntriesCount;

	public int imagesOffset;

	public int imagesCount;

	public int assembliesOffset;

	public int assembliesCount;

	[Version(Max = 24.5f)]
	public int metadataUsageListsOffset;

	[Version(Max = 24.5f)]
	public int metadataUsageListsCount;

	[Version(Max = 24.5f)]
	public int metadataUsagePairsOffset;

	[Version(Max = 24.5f)]
	public int metadataUsagePairsCount;

	public int fieldRefsOffset;

	public int fieldRefsCount;

	public int referencedAssembliesOffset;

	public int referencedAssembliesCount;

	[Version(Max = 27.1f)]
	public int attributesInfoOffset;

	[Version(Max = 27.1f)]
	public int attributesInfoCount;

	[Version(Max = 27.1f)]
	public int attributeTypesOffset;

	[Version(Max = 27.1f)]
	public int attributeTypesCount;

	[Version(Min = 27.1f)]
	public int attributeDataOffset;

	[Version(Min = 27.1f)]
	public int attributeDataCount;

	[Version(Min = 27.1f)]
	public int attributeDataRangeOffset;

	[Version(Min = 27.1f)]
	public int attributeDataRangeCount;

	public int unresolvedVirtualCallParameterTypesOffset;

	public int unresolvedVirtualCallParameterTypesCount;

	public int unresolvedVirtualCallParameterRangesOffset;

	public int unresolvedVirtualCallParameterRangesCount;

	public int windowsRuntimeTypeNamesOffset;

	public int windowsRuntimeTypeNamesSize;

	[Version(Min = 27f)]
	public int windowsRuntimeStringsOffset;

	[Version(Min = 27f)]
	public int windowsRuntimeStringsSize;

	public int exportedTypeDefinitionsOffset;

	public int exportedTypeDefinitionsCount;
}
