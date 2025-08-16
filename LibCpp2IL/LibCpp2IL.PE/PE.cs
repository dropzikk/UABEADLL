using System;
using System.IO;
using System.Linq;
using System.Text;
using LibCpp2IL.Logging;

namespace LibCpp2IL.PE;

public sealed class PE : Il2CppBinary
{
	internal readonly byte[] raw;

	internal readonly SectionHeader[] peSectionHeaders;

	internal readonly ulong peImageBase;

	private readonly OptionalHeader64? peOptionalHeader64;

	private readonly OptionalHeader? peOptionalHeader32;

	private uint[]? peExportedFunctionPointers;

	private uint[] peExportedFunctionNamePtrs;

	private ushort[] peExportedFunctionOrdinals;

	public override long RawLength => raw.Length;

	public PE(MemoryStream input, long maxMetadataUsages)
		: base(input, maxMetadataUsages)
	{
		raw = input.GetBuffer();
		LibLogger.Verbose("\tReading PE File Header...");
		DateTime now = DateTime.Now;
		if (ReadUInt16() != 23117)
		{
			throw new FormatException("ERROR: Magic number mismatch.");
		}
		base.Position = 60L;
		base.Position = ReadUInt32();
		if (ReadUInt32() != 17744)
		{
			throw new FormatException("ERROR: Invalid PE file signature");
		}
		FileHeader fileHeader = ReadClassAtRawAddr<FileHeader>(-1L);
		if (fileHeader.Machine == 332)
		{
			is32Bit = true;
			InstructionSet = InstructionSet.X86_32;
			peOptionalHeader32 = ReadClassAtRawAddr<OptionalHeader>(-1L);
			peOptionalHeader32.DataDirectory = ReadClassArrayAtRawAddr<DataDirectory>(-1L, peOptionalHeader32.NumberOfRvaAndSizes);
			peImageBase = peOptionalHeader32.ImageBase;
		}
		else
		{
			if (fileHeader.Machine != 34404)
			{
				throw new NotSupportedException("ERROR: Unsupported machine.");
			}
			InstructionSet = InstructionSet.X86_64;
			peOptionalHeader64 = ReadClassAtRawAddr<OptionalHeader64>(-1L);
			peOptionalHeader64.DataDirectory = ReadClassArrayAtRawAddr<DataDirectory>(-1L, peOptionalHeader64.NumberOfRvaAndSizes);
			peImageBase = peOptionalHeader64.ImageBase;
		}
		peSectionHeaders = new SectionHeader[fileHeader.NumberOfSections];
		for (int i = 0; i < fileHeader.NumberOfSections; i++)
		{
			peSectionHeaders[i] = new SectionHeader
			{
				Name = Encoding.UTF8.GetString(ReadBytes(8)).Trim(new char[1]),
				VirtualSize = ReadUInt32(),
				VirtualAddress = ReadUInt32(),
				SizeOfRawData = ReadUInt32(),
				PointerToRawData = ReadUInt32(),
				PointerToRelocations = ReadUInt32(),
				PointerToLinenumbers = ReadUInt32(),
				NumberOfRelocations = ReadUInt16(),
				NumberOfLinenumbers = ReadUInt16(),
				Characteristics = ReadUInt32()
			};
		}
		LibLogger.VerboseNewline($"OK ({(DateTime.Now - now).TotalMilliseconds} ms)");
		LibLogger.VerboseNewline($"\t\tImage Base at 0x{peImageBase:X}");
		LibLogger.VerboseNewline("\t\tDLL is " + (is32Bit ? "32" : "64") + "-bit");
	}

	public override long MapVirtualAddressToRaw(ulong uiAddr)
	{
		if (uiAddr < peImageBase)
		{
			throw new OverflowException($"Provided address, 0x{uiAddr:X}, was less than image base, 0x{peImageBase:X}");
		}
		uint addr = (uint)(uiAddr - peImageBase);
		if (addr == 2147483648u)
		{
			throw new OverflowException($"Provided address, 0x{uiAddr:X}, was less than image base, 0x{peImageBase:X}");
		}
		SectionHeader sectionHeader = peSectionHeaders[peSectionHeaders.Length - 1];
		if (addr > sectionHeader.VirtualAddress + sectionHeader.VirtualSize)
		{
			return 0L;
		}
		SectionHeader sectionHeader2 = peSectionHeaders.FirstOrDefault((SectionHeader x) => addr >= x.VirtualAddress && addr <= x.VirtualAddress + x.VirtualSize);
		if (sectionHeader2 == null)
		{
			return 0L;
		}
		return addr - (sectionHeader2.VirtualAddress - sectionHeader2.PointerToRawData);
	}

	public override ulong MapRawAddressToVirtual(uint offset)
	{
		SectionHeader sectionHeader = peSectionHeaders.First((SectionHeader x) => offset >= x.PointerToRawData && offset < x.PointerToRawData + x.SizeOfRawData);
		return peImageBase + sectionHeader.VirtualAddress + offset - sectionHeader.PointerToRawData;
	}

	private void LoadPeExportTable()
	{
		uint virtualAddress;
		if (is32Bit)
		{
			if (peOptionalHeader32?.DataDirectory == null || peOptionalHeader32.DataDirectory.Length == 0)
			{
				throw new InvalidDataException("Could not load 32-bit optional header or data directory, or data directory was empty!");
			}
			virtualAddress = peOptionalHeader32.DataDirectory.First().VirtualAddress;
		}
		else
		{
			if (peOptionalHeader64?.DataDirectory == null || peOptionalHeader64.DataDirectory.Length == 0)
			{
				throw new InvalidDataException("Could not load 64-bit optional header or data directory, or data directory was empty!");
			}
			virtualAddress = peOptionalHeader64.DataDirectory.First().VirtualAddress;
		}
		try
		{
			PeDirectoryEntryExport peDirectoryEntryExport = ReadClassAtVirtualAddress<PeDirectoryEntryExport>(virtualAddress + peImageBase);
			peExportedFunctionPointers = ReadClassArrayAtVirtualAddress<uint>(peDirectoryEntryExport.RawAddressOfExportTable + peImageBase, peDirectoryEntryExport.NumberOfExports);
			peExportedFunctionNamePtrs = ReadClassArrayAtVirtualAddress<uint>(peDirectoryEntryExport.RawAddressOfExportNameTable + peImageBase, peDirectoryEntryExport.NumberOfExportNames);
			peExportedFunctionOrdinals = ReadClassArrayAtVirtualAddress<ushort>(peDirectoryEntryExport.RawAddressOfExportOrdinalTable + peImageBase, peDirectoryEntryExport.NumberOfExportNames);
		}
		catch (EndOfStreamException)
		{
			LibLogger.WarnNewline($"PE does not appear to contain a valid export table! It would be apparently located at virt address 0x{virtualAddress + peImageBase:X}, raw 0x{MapVirtualAddressToRaw(virtualAddress + peImageBase):X}, but that's beyond the end of the binary. No exported functions will be accessible.");
			peExportedFunctionPointers = Array.Empty<uint>();
			peExportedFunctionNamePtrs = Array.Empty<uint>();
			peExportedFunctionOrdinals = Array.Empty<ushort>();
		}
	}

	public override ulong[] GetAllExportedIl2CppFunctionPointers()
	{
		if (peExportedFunctionPointers == null)
		{
			LoadPeExportTable();
		}
		return peExportedFunctionPointers.Where((uint e, int i) => GetExportedFunctionName(i).StartsWith("il2cpp_")).Select((Func<uint, ulong>)((uint e) => e)).ToArray();
	}

	private string GetExportedFunctionName(int index)
	{
		if (peExportedFunctionPointers == null)
		{
			LoadPeExportTable();
		}
		uint num = peExportedFunctionNamePtrs[index];
		long offset = MapVirtualAddressToRaw(num + peImageBase);
		return ReadStringToNull(offset);
	}

	public override ulong GetVirtualAddressOfExportedFunctionByName(string toFind)
	{
		if (peExportedFunctionPointers == null)
		{
			LoadPeExportTable();
		}
		int num = Array.FindIndex(peExportedFunctionNamePtrs, delegate(uint stringAddress)
		{
			long offset = MapVirtualAddressToRaw(stringAddress + peImageBase);
			return ReadStringToNull(offset) == toFind;
		});
		if (num < 0)
		{
			return 0uL;
		}
		ushort num2 = peExportedFunctionOrdinals[num];
		return peExportedFunctionPointers[num2] + peImageBase;
	}

	public override ulong GetRVA(ulong pointer)
	{
		return pointer - peImageBase;
	}

	public override byte[] GetEntirePrimaryExecutableSection()
	{
		SectionHeader sectionHeader = peSectionHeaders.FirstOrDefault((SectionHeader s) => s.Name == ".text");
		if (sectionHeader == null)
		{
			return Array.Empty<byte>();
		}
		return GetRawBinaryContent().SubArray((int)sectionHeader.PointerToRawData, (int)sectionHeader.SizeOfRawData);
	}

	public override ulong GetVirtualAddressOfPrimaryExecutableSection()
	{
		return (peSectionHeaders.FirstOrDefault((SectionHeader s) => s.Name == ".text")?.VirtualAddress + peImageBase).GetValueOrDefault();
	}

	public override byte GetByteAtRawAddress(ulong addr)
	{
		return raw[addr];
	}

	public override byte[] GetRawBinaryContent()
	{
		return raw;
	}
}
