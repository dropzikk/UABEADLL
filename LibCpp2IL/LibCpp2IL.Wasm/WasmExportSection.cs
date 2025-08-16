using System.Collections.Generic;
using LibCpp2IL.Logging;

namespace LibCpp2IL.Wasm;

public class WasmExportSection : WasmSection
{
	public ulong ExportCount;

	public readonly List<WasmExportEntry> Exports = new List<WasmExportEntry>();

	internal WasmExportSection(WasmSectionId type, long pointer, ulong size, WasmFile file)
		: base(type, pointer, size)
	{
		ExportCount = file.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < ExportCount; num++)
		{
			WasmExportEntry wasmExportEntry = new WasmExportEntry(file);
			if (wasmExportEntry.Kind == WasmExternalKind.EXT_FUNCTION)
			{
				LibLogger.VerboseNewline($"\t\t\t- Found exported function {wasmExportEntry.Name}");
			}
			Exports.Add(wasmExportEntry);
		}
		LibLogger.VerboseNewline($"\t\tRead {Exports.Count} exported functions");
	}
}
