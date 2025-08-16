using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Logging;
using LibCpp2IL.Metadata;

namespace LibCpp2IL;

public abstract class Il2CppBinary : ClassReadingBinaryReader
{
	public InstructionSet InstructionSet;

	protected readonly long maxMetadataUsages;

	private Il2CppMetadataRegistration metadataRegistration;

	private Il2CppCodeRegistration codeRegistration;

	protected ulong[] methodPointers;

	private ulong[] genericMethodPointers;

	private ulong[] invokerPointers;

	private ulong[]? customAttributeGenerators;

	protected long[] fieldOffsets;

	protected ulong[] metadataUsages;

	protected ulong[][] codeGenModuleMethodPointers;

	protected Il2CppType[] types;

	private Il2CppGenericMethodFunctionsDefinitions[] genericMethodTables;

	protected Il2CppGenericInst[] genericInsts;

	protected Il2CppMethodSpec[] methodSpecs;

	protected Il2CppCodeGenModule[] codeGenModules;

	protected Il2CppTokenRangePair[][] codegenModuleRgctxRanges;

	protected Il2CppRGCTXDefinition[][] codegenModuleRgctxs;

	protected Dictionary<int, ulong> genericMethodDictionary;

	protected readonly Dictionary<ulong, Il2CppType> typesDict = new Dictionary<ulong, Il2CppType>();

	public readonly Dictionary<Il2CppMethodDefinition, List<Il2CppGenericMethodRef>> ConcreteGenericMethods = new Dictionary<Il2CppMethodDefinition, List<Il2CppGenericMethodRef>>();

	public readonly Dictionary<ulong, List<Il2CppGenericMethodRef>> ConcreteGenericImplementationsByAddress = new Dictionary<ulong, List<Il2CppGenericMethodRef>>();

	public ulong[] TypeDefinitionSizePointers;

	public abstract long RawLength { get; }

	public int NumTypes => types.Length;

	public ulong[] AllCustomAttributeGenerators
	{
		get
		{
			if (!(LibCpp2IlMain.MetadataVersion >= 29f))
			{
				if (!(LibCpp2IlMain.MetadataVersion >= 27f))
				{
					return customAttributeGenerators;
				}
				return AllCustomAttributeGeneratorsV27;
			}
			return Array.Empty<ulong>();
		}
	}

	private ulong[] AllCustomAttributeGeneratorsV27 => (from p in LibCpp2IlMain.TheMetadata.imageDefinitions.Select((Il2CppImageDefinition i) => (image: i, cgm: GetCodegenModuleByName(i.Name), ptrSize: (ulong)(is32Bit ? 4 : 8))).SelectMany<(Il2CppImageDefinition, Il2CppCodeGenModule, ulong), ulong>(((Il2CppImageDefinition image, Il2CppCodeGenModule cgm, ulong ptrSize) tuple) => from o in LibCpp2ILUtils.Range(0, (int)tuple.image.customAttributeCount)
			select tuple.cgm.customAttributeCacheGenerator + (ulong)((long)o * (long)tuple.ptrSize))
		select ReadClassAtVirtualAddress<ulong>(p)).ToArray();

	protected Il2CppBinary(MemoryStream input, long maxMetadataUsages)
		: base(input)
	{
		this.maxMetadataUsages = maxMetadataUsages;
	}

	public void Init(ulong pCodeRegistration, ulong pMetadataRegistration)
	{
		codeRegistration = ReadClassAtVirtualAddress<Il2CppCodeRegistration>(pCodeRegistration);
		metadataRegistration = ReadClassAtVirtualAddress<Il2CppMetadataRegistration>(pMetadataRegistration);
		LibLogger.Verbose("\tReading generic instances...");
		DateTime now = DateTime.Now;
		genericInsts = Array.ConvertAll(ReadClassArrayAtVirtualAddress<ulong>(metadataRegistration.genericInsts, metadataRegistration.genericInstsCount), ReadClassAtVirtualAddress<Il2CppGenericInst>);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading generic method pointers...");
		now = DateTime.Now;
		genericMethodPointers = ReadClassArrayAtVirtualAddress<ulong>(codeRegistration.genericMethodPointers, (long)codeRegistration.genericMethodPointersCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading invoker pointers...");
		now = DateTime.Now;
		invokerPointers = ReadClassArrayAtVirtualAddress<ulong>(codeRegistration.invokerPointers, (long)codeRegistration.invokerPointersCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		if (LibCpp2IlMain.MetadataVersion < 27f)
		{
			LibLogger.Verbose("\tReading custom attribute generators...");
			now = DateTime.Now;
			customAttributeGenerators = ReadClassArrayAtVirtualAddress<ulong>(codeRegistration.customAttributeGeneratorListAddress, (long)codeRegistration.customAttributeCount);
			LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		}
		LibLogger.Verbose("\tReading field offsets...");
		now = DateTime.Now;
		fieldOffsets = ReadClassArrayAtVirtualAddress<long>(metadataRegistration.fieldOffsetListAddress, metadataRegistration.fieldOffsetsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading types...");
		now = DateTime.Now;
		ulong[] array = ReadClassArrayAtVirtualAddress<ulong>(metadataRegistration.typeAddressListAddress, metadataRegistration.numTypes);
		types = new Il2CppType[metadataRegistration.numTypes];
		for (int i = 0; i < metadataRegistration.numTypes; i++)
		{
			types[i] = ReadClassAtVirtualAddress<Il2CppType>(array[i]);
			types[i].Init();
			typesDict[array[i]] = types[i];
		}
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading type definition sizes...");
		now = DateTime.Now;
		TypeDefinitionSizePointers = ReadClassArrayAtVirtualAddress<ulong>(metadataRegistration.typeDefinitionsSizes, metadataRegistration.typeDefinitionsSizesCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		if (metadataRegistration.metadataUsages != 0L)
		{
			LibLogger.Verbose("\tReading metadata usages...");
			now = DateTime.Now;
			metadataUsages = ReadClassArrayAtVirtualAddress<ulong>(metadataRegistration.metadataUsages, (long)Math.Max((decimal)metadataRegistration.metadataUsagesCount, (decimal)maxMetadataUsages));
			LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		}
		if (LibCpp2IlMain.MetadataVersion >= 24.2f)
		{
			LibLogger.VerboseNewline("\tReading code gen modules...");
			now = DateTime.Now;
			ulong[] array2 = ReadClassArrayAtVirtualAddress<ulong>(codeRegistration.addrCodeGenModulePtrs, (long)codeRegistration.codeGenModulesCount);
			codeGenModules = new Il2CppCodeGenModule[array2.Length];
			codeGenModuleMethodPointers = new ulong[array2.Length][];
			codegenModuleRgctxRanges = new Il2CppTokenRangePair[array2.Length][];
			codegenModuleRgctxs = new Il2CppRGCTXDefinition[array2.Length][];
			for (int j = 0; j < array2.Length; j++)
			{
				Il2CppCodeGenModule il2CppCodeGenModule = ReadClassAtVirtualAddress<Il2CppCodeGenModule>(array2[j]);
				codeGenModules[j] = il2CppCodeGenModule;
				string text = ReadStringToNull(MapVirtualAddressToRaw(il2CppCodeGenModule.moduleName));
				LibLogger.VerboseNewline($"\t\t-Read module data for {text}, contains {il2CppCodeGenModule.methodPointerCount} method pointers starting at 0x{il2CppCodeGenModule.methodPointers:X}");
				if (il2CppCodeGenModule.methodPointerCount > 0)
				{
					try
					{
						ulong[] array3 = ReadClassArrayAtVirtualAddress<ulong>(il2CppCodeGenModule.methodPointers, il2CppCodeGenModule.methodPointerCount);
						codeGenModuleMethodPointers[j] = array3;
						LibLogger.VerboseNewline($"\t\t\t-Read {il2CppCodeGenModule.methodPointerCount} method pointers.");
					}
					catch (Exception ex)
					{
						LibLogger.VerboseNewline("\t\t\tWARNING: Unable to get function pointers for " + text + ": " + ex.Message);
						codeGenModuleMethodPointers[j] = new ulong[il2CppCodeGenModule.methodPointerCount];
					}
				}
				if (il2CppCodeGenModule.rgctxRangesCount > 0)
				{
					try
					{
						Il2CppTokenRangePair[] array4 = ReadClassArrayAtVirtualAddress<Il2CppTokenRangePair>(il2CppCodeGenModule.pRgctxRanges, il2CppCodeGenModule.rgctxRangesCount);
						codegenModuleRgctxRanges[j] = array4;
						LibLogger.VerboseNewline($"\t\t\t-Read {il2CppCodeGenModule.rgctxRangesCount} RGCTX ranges.");
					}
					catch (Exception ex2)
					{
						LibLogger.VerboseNewline("\t\t\tWARNING: Unable to get RGCTX ranges for " + text + ": " + ex2.Message);
						codegenModuleRgctxRanges[j] = new Il2CppTokenRangePair[il2CppCodeGenModule.rgctxRangesCount];
					}
				}
				if (il2CppCodeGenModule.rgctxsCount > 0)
				{
					try
					{
						Il2CppRGCTXDefinition[] array5 = ReadClassArrayAtVirtualAddress<Il2CppRGCTXDefinition>(il2CppCodeGenModule.rgctxs, il2CppCodeGenModule.rgctxsCount);
						codegenModuleRgctxs[j] = array5;
						LibLogger.VerboseNewline($"\t\t\t-Read {il2CppCodeGenModule.rgctxsCount} RGCTXs.");
					}
					catch (Exception ex3)
					{
						LibLogger.VerboseNewline("\t\t\tWARNING: Unable to get RGCTXs for " + text + ": " + ex3.Message);
						codegenModuleRgctxs[j] = new Il2CppRGCTXDefinition[il2CppCodeGenModule.rgctxsCount];
					}
				}
			}
			LibLogger.VerboseNewline($"\tOK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		}
		else
		{
			LibLogger.Verbose("\tReading method pointers...");
			now = DateTime.Now;
			methodPointers = ReadClassArrayAtVirtualAddress<ulong>(codeRegistration.methodPointers, (long)codeRegistration.methodPointersCount);
			LibLogger.VerboseNewline($"Read {methodPointers.Length} OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		}
		LibLogger.Verbose("\tReading generic method tables...");
		now = DateTime.Now;
		genericMethodTables = ReadClassArrayAtVirtualAddress<Il2CppGenericMethodFunctionsDefinitions>(metadataRegistration.genericMethodTable, metadataRegistration.genericMethodTableCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading method specifications...");
		now = DateTime.Now;
		methodSpecs = ReadClassArrayAtVirtualAddress<Il2CppMethodSpec>(metadataRegistration.methodSpecs, metadataRegistration.methodSpecsCount);
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.Verbose("\tReading generic methods...");
		now = DateTime.Now;
		genericMethodDictionary = new Dictionary<int, ulong>();
		Il2CppGenericMethodFunctionsDefinitions[] array6 = genericMethodTables;
		foreach (Il2CppGenericMethodFunctionsDefinitions obj in array6)
		{
			int genericMethodIndex = obj.genericMethodIndex;
			int methodIndex = obj.indices.methodIndex;
			Il2CppGenericMethodRef genericMethodRef;
			int genericMethodFromIndex = GetGenericMethodFromIndex(genericMethodIndex, methodIndex, out genericMethodRef);
			if (!genericMethodDictionary.ContainsKey(genericMethodFromIndex) && methodIndex < genericMethodPointers.Length)
			{
				Extensions.TryAdd(genericMethodDictionary, genericMethodFromIndex, genericMethodPointers[methodIndex]);
			}
		}
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
	}

	private int GetGenericMethodFromIndex(int genericMethodIndex, int genericMethodPointerIndex, out Il2CppGenericMethodRef? genericMethodRef)
	{
		Il2CppMethodSpec methodSpec = GetMethodSpec(genericMethodIndex);
		int methodDefinitionIndex = methodSpec.methodDefinitionIndex;
		genericMethodRef = new Il2CppGenericMethodRef(methodSpec);
		if (genericMethodPointerIndex >= 0 && genericMethodPointerIndex < genericMethodPointers.Length)
		{
			genericMethodRef.GenericVariantPtr = genericMethodPointers[genericMethodPointerIndex];
		}
		if (!ConcreteGenericMethods.ContainsKey(genericMethodRef.BaseMethod))
		{
			ConcreteGenericMethods[genericMethodRef.BaseMethod] = new List<Il2CppGenericMethodRef>();
		}
		ConcreteGenericMethods[genericMethodRef.BaseMethod].Add(genericMethodRef);
		if (genericMethodRef.GenericVariantPtr != 0)
		{
			if (!ConcreteGenericImplementationsByAddress.ContainsKey(genericMethodRef.GenericVariantPtr))
			{
				ConcreteGenericImplementationsByAddress[genericMethodRef.GenericVariantPtr] = new List<Il2CppGenericMethodRef>();
			}
			ConcreteGenericImplementationsByAddress[genericMethodRef.GenericVariantPtr].Add(genericMethodRef);
		}
		return methodDefinitionIndex;
	}

	public abstract byte GetByteAtRawAddress(ulong addr);

	public abstract long MapVirtualAddressToRaw(ulong uiAddr);

	public abstract ulong MapRawAddressToVirtual(uint offset);

	public abstract ulong GetRVA(ulong pointer);

	public bool TryMapRawAddressToVirtual(in uint offset, out ulong va)
	{
		try
		{
			va = MapRawAddressToVirtual(offset);
			return true;
		}
		catch (Exception)
		{
			va = 0uL;
			return false;
		}
	}

	public bool TryMapVirtualAddressToRaw(ulong virtAddr, out long result)
	{
		try
		{
			result = MapVirtualAddressToRaw(virtAddr);
			return true;
		}
		catch (Exception)
		{
			result = 0L;
			return false;
		}
	}

	public T[] ReadClassArrayAtVirtualAddress<T>(ulong addr, long count) where T : new()
	{
		return ReadClassArrayAtRawAddr<T>(MapVirtualAddressToRaw(addr), count);
	}

	public T ReadClassAtVirtualAddress<T>(ulong addr) where T : new()
	{
		return ReadClassAtRawAddr<T>(MapVirtualAddressToRaw(addr));
	}

	public Il2CppGenericInst GetGenericInst(int index)
	{
		return genericInsts[index];
	}

	public Il2CppMethodSpec GetMethodSpec(int index)
	{
		if (index < methodSpecs.Length)
		{
			if (index >= 0)
			{
				return methodSpecs[index];
			}
			throw new ArgumentException($"GetMethodSpec: index {index} < 0");
		}
		throw new ArgumentException($"GetMethodSpec: index {index} >= length {methodSpecs.Length}");
	}

	public Il2CppType GetType(int index)
	{
		return types[index];
	}

	public ulong GetRawMetadataUsage(uint index)
	{
		return metadataUsages[index];
	}

	public ulong[] GetCodegenModuleMethodPointers(int codegenModuleIndex)
	{
		return codeGenModuleMethodPointers[codegenModuleIndex];
	}

	public Il2CppCodeGenModule? GetCodegenModuleByName(string name)
	{
		return codeGenModules.FirstOrDefault((Il2CppCodeGenModule m) => m.Name == name);
	}

	public int GetCodegenModuleIndex(Il2CppCodeGenModule module)
	{
		return Array.IndexOf(codeGenModules, module);
	}

	public int GetCodegenModuleIndexByName(string name)
	{
		Il2CppCodeGenModule codegenModuleByName = GetCodegenModuleByName(name);
		if (codegenModuleByName == null)
		{
			return -1;
		}
		return GetCodegenModuleIndex(codegenModuleByName);
	}

	public Il2CppTokenRangePair[] GetRGCTXRangePairsForModule(Il2CppCodeGenModule module)
	{
		return codegenModuleRgctxRanges[GetCodegenModuleIndex(module)];
	}

	public Il2CppRGCTXDefinition[] GetRGCTXDataForPair(Il2CppCodeGenModule module, Il2CppTokenRangePair rangePair)
	{
		return codegenModuleRgctxs[GetCodegenModuleIndex(module)].Skip(rangePair.start).Take(rangePair.length).ToArray();
	}

	public Il2CppType GetIl2CppTypeFromPointer(ulong pointer)
	{
		return typesDict[pointer];
	}

	public ulong[] GetPointers(ulong pointer, long count)
	{
		if (is32Bit)
		{
			return Array.ConvertAll(ReadClassArrayAtVirtualAddress<uint>(pointer, count), (Converter<uint, ulong>)((uint x) => x));
		}
		return ReadClassArrayAtVirtualAddress<ulong>(pointer, count);
	}

	public int GetFieldOffsetFromIndex(int typeIndex, int fieldIndexInType, int fieldIndex, bool isValueType, bool isStatic)
	{
		try
		{
			int num = -1;
			if (LibCpp2IlMain.MetadataVersion > 21f)
			{
				ulong num2 = (ulong)fieldOffsets[typeIndex];
				if (num2 != 0)
				{
					num = ReadClassAtRawAddr<int>(MapVirtualAddressToRaw(num2) + 4L * (long)fieldIndexInType);
				}
			}
			else
			{
				num = (int)fieldOffsets[fieldIndex];
			}
			if (num > 0 && isValueType && !isStatic)
			{
				num = ((!is32Bit) ? (num - 16) : (num - 8));
			}
			return num;
		}
		catch
		{
			return -1;
		}
	}

	public ulong GetMethodPointer(int methodIndex, int methodDefinitionIndex, int imageIndex, uint methodToken)
	{
		if (LibCpp2IlMain.MetadataVersion >= 24.2f)
		{
			if (genericMethodDictionary.TryGetValue(methodDefinitionIndex, out var value))
			{
				return value;
			}
			ulong[] obj = codeGenModuleMethodPointers[imageIndex];
			uint num = methodToken & 0xFFFFFF;
			return obj[num - 1];
		}
		if (methodIndex >= 0)
		{
			return methodPointers[methodIndex];
		}
		genericMethodDictionary.TryGetValue(methodDefinitionIndex, out var value2);
		return value2;
	}

	public ulong GetCustomAttributeGenerator(int index)
	{
		return customAttributeGenerators[index];
	}

	public abstract byte[] GetRawBinaryContent();

	public abstract ulong GetVirtualAddressOfExportedFunctionByName(string toFind);

	public abstract ulong[] GetAllExportedIl2CppFunctionPointers();

	public abstract byte[] GetEntirePrimaryExecutableSection();

	public abstract ulong GetVirtualAddressOfPrimaryExecutableSection();

	public (ulong pCodeRegistration, ulong pMetadataRegistration) PlusSearch(int methodCount, int typeDefinitionsCount)
	{
		ulong result = 0uL;
		LibLogger.VerboseNewline("\tAttempting to locate code and metadata registration functions...");
		BinarySearcher binarySearcher = new BinarySearcher(this, methodCount, typeDefinitionsCount);
		LibLogger.VerboseNewline("\t\t-Searching for MetadataReg...");
		ulong result2 = ((LibCpp2IlMain.MetadataVersion < 24.5f) ? binarySearcher.FindMetadataRegistrationPre24_5() : binarySearcher.FindMetadataRegistrationPost24_5());
		LibLogger.VerboseNewline("\t\t-Searching for CodeReg...");
		if (result == 0L)
		{
			if (LibCpp2IlMain.MetadataVersion >= 24.2f)
			{
				LibLogger.VerboseNewline("\t\t\tUsing mscorlib full-disassembly approach to get codereg, this may take a while...");
				result = binarySearcher.FindCodeRegistrationPost2019();
			}
			else
			{
				result = binarySearcher.FindCodeRegistrationPre2019();
			}
		}
		if (result == 0L && LibCpp2IlMain.Settings.AllowManualMetadataAndCodeRegInput)
		{
			LibLogger.Info("Couldn't identify a CodeRegistration address. If you know it, enter it now, otherwise enter nothing or zero to fail: ");
			ulong.TryParse(Console.ReadLine(), NumberStyles.HexNumber, null, out result);
		}
		if (result2 == 0L && LibCpp2IlMain.Settings.AllowManualMetadataAndCodeRegInput)
		{
			LibLogger.Info("Couldn't identify a MetadataRegistration address. If you know it, enter it now, otherwise enter nothing or zero to fail: ");
			ulong.TryParse(Console.ReadLine(), NumberStyles.HexNumber, null, out result2);
		}
		return (pCodeRegistration: result, pMetadataRegistration: result2);
	}
}
