using System.Collections.Generic;
using LibCpp2IL.Logging;

namespace LibCpp2IL.Wasm;

public class WasmImportSection : WasmSection
{
	public ulong ImportCount;

	public readonly List<WasmImportEntry> Entries = new List<WasmImportEntry>();

	internal WasmImportSection(WasmSectionId type, long pointer, ulong size, WasmFile readFrom)
		: base(type, pointer, size)
	{
		ImportCount = readFrom.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < ImportCount; num++)
		{
			Entries.Add(new WasmImportEntry(readFrom));
		}
		LibLogger.VerboseNewline($"\t\tRead {Entries.Count} imports");
	}
}
