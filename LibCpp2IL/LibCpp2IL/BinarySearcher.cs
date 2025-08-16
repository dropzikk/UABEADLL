using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Logging;

namespace LibCpp2IL;

public class BinarySearcher
{
	private class Section
	{
		public ulong RawStartAddress;

		public ulong RawEndAddress;

		public ulong VirtualStartAddress;
	}

	private static readonly byte[] FeatureBytes2019 = new byte[13]
	{
		109, 115, 99, 111, 114, 108, 105, 98, 46, 100,
		108, 108, 0
	};

	private readonly Il2CppBinary _binary;

	private readonly byte[] binaryBytes;

	private readonly int methodCount;

	private readonly int typeDefinitionsCount;

	public BinarySearcher(Il2CppBinary binary, int methodCount, int typeDefinitionsCount)
	{
		_binary = binary;
		binaryBytes = binary.GetRawBinaryContent();
		this.methodCount = methodCount;
		this.typeDefinitionsCount = typeDefinitionsCount;
	}

	private int FindBytes(byte[] blob, byte[] signature, int requiredAlignment = 1, int startOffset = 0)
	{
		int num = Array.IndexOf(blob, signature[0], startOffset);
		byte[] array = new byte[signature.Length];
		while (num >= 0 && num <= blob.Length - signature.Length)
		{
			Buffer.BlockCopy(blob, num, array, 0, signature.Length);
			if (num % requiredAlignment == 0 && array.SequenceEqual(signature))
			{
				return num;
			}
			num = Array.IndexOf(blob, signature[0], num + 1);
		}
		return -1;
	}

	private IEnumerable<uint> FindAllBytes(byte[] signature, int alignment = 0)
	{
		LibLogger.VerboseNewline("\t\t\tLooking for bytes: " + string.Join(" ", signature.Select((byte b) => b.ToString("x2"))));
		int offset = 0;
		int ptrSize = (_binary.is32Bit ? 4 : 8);
		while (offset != -1)
		{
			offset = FindBytes(binaryBytes, signature, (alignment != 0) ? alignment : ptrSize, offset);
			if (offset != -1)
			{
				yield return (uint)offset;
				offset += ptrSize;
			}
		}
	}

	private IEnumerable<uint> FindAllStrings(string str)
	{
		return FindAllBytes(Encoding.ASCII.GetBytes(str), 1);
	}

	private IEnumerable<uint> FindAllDWords(uint word)
	{
		return FindAllBytes(BitConverter.GetBytes(word), 1);
	}

	private IEnumerable<uint> FindAllQWords(ulong word)
	{
		return FindAllBytes(BitConverter.GetBytes(word), 1);
	}

	private IEnumerable<uint> FindAllWords(ulong word)
	{
		if (!_binary.is32Bit)
		{
			return FindAllQWords(word);
		}
		return FindAllDWords((uint)word);
	}

	private IEnumerable<ulong> MapOffsetsToVirt(IEnumerable<uint> offsets)
	{
		foreach (uint offset2 in offsets)
		{
			uint offset = offset2;
			if (_binary.TryMapRawAddressToVirtual(in offset, out var va))
			{
				yield return va;
			}
		}
	}

	private IEnumerable<ulong> FindAllMappedWords(ulong word)
	{
		IEnumerable<uint> offsets = FindAllWords(word);
		return MapOffsetsToVirt(offsets);
	}

	private IEnumerable<ulong> FindAllMappedWords(IEnumerable<ulong> va)
	{
		return va.SelectMany(FindAllMappedWords);
	}

	private IEnumerable<ulong> FindAllMappedWords(IEnumerable<uint> va)
	{
		return va.SelectMany((uint a) => FindAllMappedWords(a));
	}

	public ulong FindCodeRegistrationPre2019()
	{
		List<ulong> list = MapOffsetsToVirt(FindAllBytes(BitConverter.GetBytes(methodCount), 1)).ToList();
		LibLogger.VerboseNewline(string.Format("\t\t\tFound {0} instances of the method count {1}, as bytes {2}", list.Count, methodCount, string.Join(", ", from x in BitConverter.GetBytes((ulong)methodCount)
			select $"0x{x:X}")));
		if (list.Count == 0)
		{
			return 0uL;
		}
		foreach (ulong item in list)
		{
			if (_binary.ReadClassAtVirtualAddress<Il2CppCodeRegistration>(item).customAttributeCount == (ulong)LibCpp2IlMain.TheMetadata.attributeTypeRanges.Length)
			{
				return item;
			}
		}
		return 0uL;
	}

	internal ulong FindCodeRegistrationPost2019()
	{
		List<ulong> list = (from idx in FindAllStrings("mscorlib.dll\0")
			select _binary.MapRawAddressToVirtual(idx)).ToList();
		LibLogger.VerboseNewline(string.Format("\t\t\tFound {0} occurrences of mscorlib.dll: [{1}]", list.Count, string.Join(", ", list.Select((ulong p) => p.ToString("X")))));
		List<ulong> list2 = FindAllMappedWords(list).ToList();
		LibLogger.VerboseNewline(string.Format("\t\t\tFound {0} potential codegen modules for mscorlib: [{1}]", list2.Count, string.Join(", ", list2.Select((ulong p) => p.ToString("X")))));
		List<ulong> list3 = FindAllMappedWords(list2).ToList();
		LibLogger.VerboseNewline(string.Format("\t\t\tFound {0} address for potential codegen modules in potential codegen module lists: [{1}]", list3.Count, string.Join(", ", list3.Select((ulong p) => p.ToString("X")))));
		uint ptrSize = (_binary.is32Bit ? 4u : 8u);
		List<ulong> list4 = null;
		if (!(LibCpp2IlMain.MetadataVersion >= 27f))
		{
			list4 = FindAllMappedWords(list3).ToList();
		}
		else
		{
			ulong num = 200uL;
			IEnumerable<ulong> source = list3.AsEnumerable();
			int num2 = LibCpp2IlMain.TheMetadata.imageDefinitions.Length;
			ulong initialBacktrack = (ulong)num2 - 5uL;
			source = source.Select((ulong va) => va - ptrSize * initialBacktrack);
			for (ulong num3 = initialBacktrack; num3 < num; num3++)
			{
				if ((list4?.Count() ?? 0) == 1)
				{
					break;
				}
				list4 = FindAllMappedWords(source).ToList();
				if (list4.Count == 1 && _binary.ReadClassAtVirtualAddress<uint>(list4.First() - ptrSize) > num)
				{
					list4 = new List<ulong>();
				}
				source = source.Select((ulong va) => va - ptrSize);
			}
			if (list4 == null || !list4.Any())
			{
				throw new Exception("Failed to find pCodegenModules");
			}
			if (list4.Count() > 1)
			{
				throw new Exception("Found more than 1 pointer as pCodegenModules");
			}
		}
		LibLogger.VerboseNewline(string.Format("\t\t\tFound {0} potential pCodegenModules addresses: [{1}]", list4.Count, string.Join(", ", list4.Select((ulong p) => p.ToString("X")))));
		ulong num4 = (ulong)(LibCpp2ILUtils.VersionAwareSizeOf(typeof(Il2CppCodeRegistration)) - ptrSize);
		LibLogger.VerboseNewline($"\t\t\tpCodegenModules is the second-to-last field of the codereg struct. Therefore on this version and architecture, we need to subtract {num4} bytes from its address to get pCodeReg");
		Dictionary<string, FieldInfo> fieldsByName = typeof(Il2CppCodeRegistration).GetFields().ToDictionary((FieldInfo f) => f.Name);
		foreach (ulong item in list4)
		{
			ulong num5 = item - num4;
			if (list4.Count == 1)
			{
				LibLogger.Verbose($"\t\t\tOnly found one codegen module pointer, so assuming it's correct and returning pCodeReg = 0x{num5:X}");
				return num5;
			}
			LibLogger.Verbose($"\t\t\tConsidering potential code registration at 0x{num5:X}...");
			if (ValidateCodeRegistration(LibCpp2IlMain.Binary.ReadClassAtVirtualAddress<Il2CppCodeRegistration>(num5), fieldsByName))
			{
				LibLogger.VerboseNewline("Looks good!");
				return num5;
			}
		}
		return 0uL;
	}

	public static bool ValidateCodeRegistration(Il2CppCodeRegistration codeReg, Dictionary<string, FieldInfo> fieldsByName)
	{
		bool result = true;
		foreach (KeyValuePair<string, FieldInfo> item in fieldsByName)
		{
			ulong num = (ulong)item.Value.GetValue(codeReg);
			if (num == 0L)
			{
				continue;
			}
			long result2;
			if (item.Key.EndsWith("count", StringComparison.OrdinalIgnoreCase))
			{
				if (num > 458752)
				{
					LibLogger.VerboseNewline($"Rejected due to unreasonable count field 0x{num:X} for field {item.Key}");
					result = false;
					break;
				}
			}
			else if (!LibCpp2IlMain.Binary.TryMapVirtualAddressToRaw(num, out result2))
			{
				LibLogger.VerboseNewline($"Rejected due to invalid pointer 0x{num:X} for field {item.Key}");
				result = false;
				break;
			}
		}
		return result;
	}

	public ulong FindMetadataRegistrationPre24_5()
	{
		ulong num = (ulong)LibCpp2ILUtils.VersionAwareSizeOf(typeof(Il2CppMetadataRegistration));
		ulong num2 = (ulong)(_binary.is32Bit ? 4 : 8);
		ulong bytesToSubtract = num - num2 * 4;
		List<ulong> list = MapOffsetsToVirt(FindAllBytes(BitConverter.GetBytes(LibCpp2IlMain.TheMetadata.typeDefs.Length), 1)).ToList();
		LibLogger.VerboseNewline($"\t\t\tFound {list.Count} instances of the number of type defs, {LibCpp2IlMain.TheMetadata.typeDefs.Length}");
		list = list.Select((ulong p) => p - bytesToSubtract).ToList();
		foreach (ulong item in list)
		{
			Il2CppMetadataRegistration il2CppMetadataRegistration = _binary.ReadClassAtVirtualAddress<Il2CppMetadataRegistration>(item);
			if (il2CppMetadataRegistration.metadataUsagesCount == (ulong)LibCpp2IlMain.TheMetadata.metadataUsageLists.Length)
			{
				LibLogger.VerboseNewline($"\t\t\tFound and selected probably valid metadata registration at 0x{item:X}.");
				return item;
			}
			LibLogger.VerboseNewline($"\t\t\tSkipping 0x{item:X} as the metadata reg, metadata usage count was 0x{il2CppMetadataRegistration.metadataUsagesCount:X}, expecting 0x{LibCpp2IlMain.TheMetadata.metadataUsageLists.Length:X}");
		}
		return 0uL;
	}

	public ulong FindMetadataRegistrationPost24_5()
	{
		ulong ptrSize = (ulong)(_binary.is32Bit ? 4 : 8);
		uint sizeOfMr = (uint)LibCpp2ILUtils.VersionAwareSizeOf(typeof(Il2CppMetadataRegistration));
		LibLogger.VerboseNewline($"\t\t\tLooking for the number of type definitions, 0x{typeDefinitionsCount:X}");
		List<ulong> list = FindAllMappedWords((ulong)typeDefinitionsCount).ToList();
		LibLogger.VerboseNewline(string.Format("\t\t\tFound {0} instances of the number of type definitions: [{1}]", list.Count, string.Join(", ", list.Select((ulong p) => p.ToString("X")))));
		List<ulong> list2 = list.Select((ulong a) => a - sizeOfMr + ptrSize * 4).ToList();
		LibLogger.VerboseNewline(string.Format("\t\t\tFound {0} potential metadata registrations: [{1}]", list2.Count, string.Join(", ", list2.Select((ulong p) => p.ToString("X")))));
		ulong num = sizeOfMr / ptrSize;
		foreach (ulong item in list2)
		{
			ulong[] array = _binary.ReadClassArrayAtVirtualAddress<ulong>(item, (int)num);
			bool flag = true;
			for (int i = 0; i < array.Length && flag; i++)
			{
				if (i % 2 == 0)
				{
					flag = array[i] < 655360;
					if (!flag && array[i] < 1048575)
					{
						LibLogger.VerboseNewline($"\t\t\tRejected Metadata registration at 0x{item:X}, because it has a count field 0x{array[i]:X} which is above sanity limit of 0xA0000. If metadata registration detection fails, need to bump up the limit.");
					}
				}
				else if (array[i] == 0L)
				{
					flag = i >= 14;
				}
				else
				{
					flag = _binary.TryMapVirtualAddressToRaw(array[i], out var _);
					if (!flag)
					{
						LibLogger.VerboseNewline($"\t\t\tRejecting metadata registration 0x{item:X} because the pointer at index {i}, which is 0x{array[i]:X}, can't be mapped to the binary.");
					}
				}
				if (!flag)
				{
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			Il2CppMetadataRegistration il2CppMetadataRegistration = _binary.ReadClassAtVirtualAddress<Il2CppMetadataRegistration>(item);
			if (LibCpp2IlMain.MetadataVersion >= 27f && (il2CppMetadataRegistration.metadataUsagesCount != 0L || il2CppMetadataRegistration.metadataUsages != 0L))
			{
				LibLogger.VerboseNewline($"\t\t\tWarning: metadata registration 0x{item:X} has {il2CppMetadataRegistration.metadataUsagesCount} metadata usages at a pointer of 0x{il2CppMetadataRegistration.metadataUsages:X}. We're on v27, these should be 0.");
			}
			if (il2CppMetadataRegistration.typeDefinitionsSizesCount != LibCpp2IlMain.TheMetadata.typeDefs.Length)
			{
				LibLogger.VerboseNewline($"\t\t\tRejecting metadata registration 0x{item:X} because it has {il2CppMetadataRegistration.typeDefinitionsSizesCount} type def sizes, while we have {LibCpp2IlMain.TheMetadata.typeDefs.Length} type defs");
				continue;
			}
			if (il2CppMetadataRegistration.numTypes < LibCpp2IlMain.TheMetadata.typeDefs.Length)
			{
				LibLogger.VerboseNewline($"\t\t\tRejecting metadata registration 0x{item:X} because it has {il2CppMetadataRegistration.numTypes} types, while we have {LibCpp2IlMain.TheMetadata.typeDefs.Length} type defs");
				continue;
			}
			LibLogger.VerboseNewline($"\t\t\tAccepting metadata reg as VA 0x{item:X}");
			return item;
		}
		return 0uL;
	}
}
