using System.Collections.Generic;

namespace LibCpp2IL.Wasm;

public class WasmGlobalSection : WasmSection
{
	public ulong GlobalCount;

	public readonly List<WasmGlobalEntry> Globals = new List<WasmGlobalEntry>();

	internal WasmGlobalSection(WasmSectionId type, long pointer, ulong size, WasmFile file)
		: base(type, pointer, size)
	{
		GlobalCount = file.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < GlobalCount; num++)
		{
			Globals.Add(new WasmGlobalEntry(file));
		}
	}
}
