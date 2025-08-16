using System.Collections.Generic;
using LibCpp2IL.Logging;

namespace LibCpp2IL.Wasm;

public class WasmDataSection : WasmSection
{
	public ulong DataCount;

	public List<WasmDataSegment> DataEntries = new List<WasmDataSegment>();

	internal WasmDataSection(WasmSectionId type, long pointer, ulong size, WasmFile file)
		: base(type, pointer, size)
	{
		DataCount = file.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < DataCount; num++)
		{
			DataEntries.Add(new WasmDataSegment(file));
		}
		LibLogger.VerboseNewline($"\t\tRead {DataEntries.Count} data segments");
	}
}
