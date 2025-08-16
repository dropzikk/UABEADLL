using System.Collections.Generic;
using System.Linq;
using LibCpp2IL.Metadata;

namespace LibCpp2IL;

public static class LibCpp2IlGlobalMapper
{
	internal static List<MetadataUsage> TypeRefs = new List<MetadataUsage>();

	internal static List<MetadataUsage> MethodRefs = new List<MetadataUsage>();

	internal static List<MetadataUsage> FieldRefs = new List<MetadataUsage>();

	internal static List<MetadataUsage> Literals = new List<MetadataUsage>();

	internal static Dictionary<ulong, MetadataUsage> TypeRefsByAddress = new Dictionary<ulong, MetadataUsage>();

	internal static Dictionary<ulong, MetadataUsage> MethodRefsByAddress = new Dictionary<ulong, MetadataUsage>();

	internal static Dictionary<ulong, MetadataUsage> FieldRefsByAddress = new Dictionary<ulong, MetadataUsage>();

	internal static Dictionary<ulong, MetadataUsage> LiteralsByAddress = new Dictionary<ulong, MetadataUsage>();

	internal static void Reset()
	{
		TypeRefs.Clear();
		MethodRefs.Clear();
		FieldRefs.Clear();
		Literals.Clear();
		TypeRefsByAddress.Clear();
		MethodRefsByAddress.Clear();
		FieldRefsByAddress.Clear();
		LiteralsByAddress.Clear();
	}

	internal static void MapGlobalIdentifiers(Il2CppMetadata metadata, Il2CppBinary cppAssembly)
	{
		if (LibCpp2IlMain.MetadataVersion < 27f)
		{
			MapGlobalIdentifiersPre27(metadata, cppAssembly);
		}
		else
		{
			MapGlobalIdentifiersPost27(metadata, cppAssembly);
		}
	}

	private static void MapGlobalIdentifiersPost27(Il2CppMetadata metadata, Il2CppBinary cppAssembly)
	{
	}

	private static void MapGlobalIdentifiersPre27(Il2CppMetadata metadata, Il2CppBinary cppAssembly)
	{
		TypeRefs = metadata.metadataUsageDic[1u].Select((KeyValuePair<uint, uint> kvp) => new MetadataUsage(MetadataUsageType.Type, cppAssembly.GetRawMetadataUsage(kvp.Key), kvp.Value)).ToList();
		TypeRefs.AddRange(metadata.metadataUsageDic[2u].Select((KeyValuePair<uint, uint> kvp) => new MetadataUsage(MetadataUsageType.Type, cppAssembly.GetRawMetadataUsage(kvp.Key), kvp.Value)));
		MethodRefs = metadata.metadataUsageDic[3u].Select((KeyValuePair<uint, uint> kvp) => new MetadataUsage(MetadataUsageType.MethodDef, cppAssembly.GetRawMetadataUsage(kvp.Key), kvp.Value)).ToList();
		FieldRefs = metadata.metadataUsageDic[4u].Select((KeyValuePair<uint, uint> kvp) => new MetadataUsage(MetadataUsageType.FieldInfo, cppAssembly.GetRawMetadataUsage(kvp.Key), kvp.Value)).ToList();
		Literals = metadata.metadataUsageDic[5u].Select((KeyValuePair<uint, uint> kvp) => new MetadataUsage(MetadataUsageType.StringLiteral, cppAssembly.GetRawMetadataUsage(kvp.Key), kvp.Value)).ToList();
		foreach (var (index, value) in metadata.metadataUsageDic[6u])
		{
			MethodRefs.Add(new MetadataUsage(MetadataUsageType.MethodRef, cppAssembly.GetRawMetadataUsage(index), value));
		}
		foreach (MetadataUsage typeRef in TypeRefs)
		{
			TypeRefsByAddress[typeRef.Offset] = typeRef;
		}
		foreach (MetadataUsage methodRef in MethodRefs)
		{
			MethodRefsByAddress[methodRef.Offset] = methodRef;
		}
		foreach (MetadataUsage fieldRef in FieldRefs)
		{
			FieldRefsByAddress[fieldRef.Offset] = fieldRef;
		}
		foreach (MetadataUsage literal in Literals)
		{
			LiteralsByAddress[literal.Offset] = literal;
		}
	}

	public static MetadataUsage? CheckForPost27GlobalAt(ulong address)
	{
		if (!LibCpp2IlMain.Binary.TryMapVirtualAddressToRaw(address, out var result) || result >= LibCpp2IlMain.Binary.RawLength)
		{
			return null;
		}
		MetadataUsage metadataUsage = MetadataUsage.DecodeMetadataUsage(LibCpp2IlMain.Binary.ReadClassAtVirtualAddress<ulong>(address), address);
		if (metadataUsage == null || !metadataUsage.IsValid)
		{
			return null;
		}
		return metadataUsage;
	}
}
