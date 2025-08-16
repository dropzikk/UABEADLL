using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibCpp2IL.Elf;
using LibCpp2IL.Logging;
using LibCpp2IL.MachO;
using LibCpp2IL.Metadata;
using LibCpp2IL.NintendoSwitch;
using LibCpp2IL.PE;
using LibCpp2IL.Reflection;
using LibCpp2IL.Wasm;

namespace LibCpp2IL;

public static class LibCpp2IlMain
{
	public class LibCpp2IlSettings
	{
		public bool AllowManualMetadataAndCodeRegInput;

		public bool DisableMethodPointerMapping;

		public bool DisableGlobalResolving;
	}

	public static readonly LibCpp2IlSettings Settings = new LibCpp2IlSettings();

	public static float MetadataVersion = 24f;

	public static Il2CppBinary? Binary;

	public static Il2CppMetadata? TheMetadata;

	private static readonly Dictionary<ulong, List<Il2CppMethodDefinition>> MethodsByPtr = new Dictionary<ulong, List<Il2CppMethodDefinition>>();

	public static void Reset()
	{
		LibCpp2IlGlobalMapper.Reset();
		LibCpp2ILUtils.Reset();
		MethodsByPtr.Clear();
		MetadataVersion = 0f;
		Binary?.Dispose();
		TheMetadata?.Dispose();
		Binary = null;
		TheMetadata = null;
	}

	public static List<Il2CppMethodDefinition>? GetManagedMethodImplementationsAtAddress(ulong addr)
	{
		MethodsByPtr.TryGetValue(addr, out List<Il2CppMethodDefinition> value);
		return value;
	}

	public static MetadataUsage? GetAnyGlobalByAddress(ulong address)
	{
		if (MetadataVersion >= 27f)
		{
			return LibCpp2IlGlobalMapper.CheckForPost27GlobalAt(address);
		}
		MetadataUsage metadataUsage = GetLiteralGlobalByAddress(address);
		if (metadataUsage == null)
		{
			metadataUsage = GetMethodGlobalByAddress(address);
		}
		if (metadataUsage == null)
		{
			metadataUsage = GetRawFieldGlobalByAddress(address);
		}
		if (metadataUsage == null)
		{
			metadataUsage = GetRawTypeGlobalByAddress(address);
		}
		return metadataUsage;
	}

	public static MetadataUsage? GetLiteralGlobalByAddress(ulong address)
	{
		if (MetadataVersion < 27f)
		{
			return LibCpp2IlGlobalMapper.LiteralsByAddress.GetValueOrDefault(address);
		}
		return GetAnyGlobalByAddress(address);
	}

	public static string? GetLiteralByAddress(ulong address)
	{
		return GetLiteralGlobalByAddress(address)?.AsLiteral();
	}

	public static MetadataUsage? GetRawTypeGlobalByAddress(ulong address)
	{
		if (MetadataVersion < 27f)
		{
			return LibCpp2IlGlobalMapper.TypeRefsByAddress.GetValueOrDefault(address);
		}
		return GetAnyGlobalByAddress(address);
	}

	public static Il2CppTypeReflectionData? GetTypeGlobalByAddress(ulong address)
	{
		if (TheMetadata == null)
		{
			return null;
		}
		return GetRawTypeGlobalByAddress(address)?.AsType();
	}

	public static MetadataUsage? GetRawFieldGlobalByAddress(ulong address)
	{
		if (MetadataVersion < 27f)
		{
			return LibCpp2IlGlobalMapper.FieldRefsByAddress.GetValueOrDefault(address);
		}
		return GetAnyGlobalByAddress(address);
	}

	public static Il2CppFieldDefinition? GetFieldGlobalByAddress(ulong address)
	{
		if (TheMetadata == null)
		{
			return null;
		}
		return GetRawFieldGlobalByAddress(address)?.AsField();
	}

	public static MetadataUsage? GetMethodGlobalByAddress(ulong address)
	{
		if (TheMetadata == null)
		{
			return null;
		}
		if (MetadataVersion < 27f)
		{
			return LibCpp2IlGlobalMapper.MethodRefsByAddress.GetValueOrDefault(address);
		}
		return GetAnyGlobalByAddress(address);
	}

	public static Il2CppMethodDefinition? GetMethodDefinitionByGlobalAddress(ulong address)
	{
		MetadataUsage methodGlobalByAddress = GetMethodGlobalByAddress(address);
		if (methodGlobalByAddress != null && methodGlobalByAddress.Type == MetadataUsageType.MethodRef)
		{
			return methodGlobalByAddress.AsGenericMethodRef().BaseMethod;
		}
		return methodGlobalByAddress?.AsMethod();
	}

	public static bool Initialize(byte[] binaryBytes, byte[] metadataBytes, int[] unityVersion)
	{
		LibCpp2IlReflection.ResetCaches();
		DateTime now = DateTime.Now;
		LibLogger.InfoNewline("Initializing Metadata...");
		TheMetadata = Il2CppMetadata.ReadFrom(metadataBytes, unityVersion);
		LibLogger.InfoNewline($"Initialized Metadata in {(DateTime.Now - now).TotalMilliseconds:F0}ms");
		if (TheMetadata == null)
		{
			return false;
		}
		LibLogger.InfoNewline("Searching Binary for Required Data...");
		now = DateTime.Now;
		ulong num;
		ulong num2;
		if (BitConverter.ToInt16(binaryBytes, 0) == 23117)
		{
			(num, num2) = (Binary = new LibCpp2IL.PE.PE(new MemoryStream(binaryBytes, 0, binaryBytes.Length, writable: false, publiclyVisible: true), TheMetadata.maxMetadataUsages)).PlusSearch(TheMetadata.methodDefs.Count((Il2CppMethodDefinition x) => x.methodIndex >= 0), TheMetadata.typeDefs.Length);
		}
		else if (BitConverter.ToInt32(binaryBytes, 0) == 1179403647)
		{
			(num, num2) = ((ElfFile)(Binary = new ElfFile(new MemoryStream(binaryBytes, 0, binaryBytes.Length, writable: true, publiclyVisible: true), TheMetadata.maxMetadataUsages))).FindCodeAndMetadataReg();
		}
		else if (BitConverter.ToInt32(binaryBytes, 0) == 810505038)
		{
			(num, num2) = (Binary = new NsoFile(new MemoryStream(binaryBytes, 0, binaryBytes.Length, writable: true, publiclyVisible: true), TheMetadata.maxMetadataUsages).Decompress()).PlusSearch(TheMetadata.methodDefs.Count((Il2CppMethodDefinition x) => x.methodIndex >= 0), TheMetadata.typeDefs.Length);
		}
		else if (BitConverter.ToInt32(binaryBytes, 0) == 1836278016)
		{
			(num, num2) = (Binary = new WasmFile(new MemoryStream(binaryBytes, 0, binaryBytes.Length, writable: false, publiclyVisible: true), TheMetadata.maxMetadataUsages)).PlusSearch(TheMetadata.methodDefs.Count((Il2CppMethodDefinition x) => x.methodIndex >= 0), TheMetadata.typeDefs.Length);
		}
		else
		{
			uint num3 = BitConverter.ToUInt32(binaryBytes, 0);
			if (num3 != 4277009102u && num3 != 4277009103u)
			{
				throw new Exception("Unknown binary type");
			}
			(num, num2) = (Binary = new MachOFile(new MemoryStream(binaryBytes, 0, binaryBytes.Length, writable: false, publiclyVisible: true), TheMetadata.maxMetadataUsages)).PlusSearch(TheMetadata.methodDefs.Count((Il2CppMethodDefinition x) => x.methodIndex >= 0), TheMetadata.typeDefs.Length);
		}
		if (num == 0L || num2 == 0L)
		{
			throw new Exception("Failed to find Binary code or metadata registration");
		}
		LibLogger.InfoNewline($"Got Binary codereg: 0x{num:X}, metareg: 0x{num2:X} in {(DateTime.Now - now).TotalMilliseconds:F0}ms.");
		LibLogger.InfoNewline("Initializing Binary...");
		now = DateTime.Now;
		Binary.Init(num, num2);
		LibLogger.InfoNewline($"Initialized Binary in {(DateTime.Now - now).TotalMilliseconds:F0}ms");
		if (!Settings.DisableGlobalResolving && MetadataVersion < 27f)
		{
			now = DateTime.Now;
			LibLogger.Info("Mapping Globals...");
			LibCpp2IlGlobalMapper.MapGlobalIdentifiers(TheMetadata, Binary);
			LibLogger.InfoNewline($"OK ({(DateTime.Now - now).TotalMilliseconds:F0}ms)");
		}
		if (!Settings.DisableMethodPointerMapping)
		{
			now = DateTime.Now;
			LibLogger.Info("Mapping pointers to Il2CppMethodDefinitions...");
			int num4 = 0;
			foreach (var (item, key) in TheMetadata.methodDefs.Select((Il2CppMethodDefinition method) => (method: method, ptr: method.MethodPointer)))
			{
				if (!MethodsByPtr.ContainsKey(key))
				{
					MethodsByPtr[key] = new List<Il2CppMethodDefinition>();
				}
				MethodsByPtr[key].Add(item);
				num4++;
			}
			LibLogger.InfoNewline($"Processed {num4} OK ({(DateTime.Now - now).TotalMilliseconds:F0}ms)");
		}
		return true;
	}

	public static bool LoadFromFile(string pePath, string metadataPath, int[] unityVersion)
	{
		byte[] metadataBytes = File.ReadAllBytes(metadataPath);
		return Initialize(File.ReadAllBytes(pePath), metadataBytes, unityVersion);
	}
}
