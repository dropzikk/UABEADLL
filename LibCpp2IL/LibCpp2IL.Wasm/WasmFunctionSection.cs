using System.Collections.Generic;

namespace LibCpp2IL.Wasm;

public class WasmFunctionSection : WasmSection
{
	public ulong EntryCount;

	public readonly List<ulong> Types = new List<ulong>();

	internal WasmFunctionSection(WasmSectionId type, long pointer, ulong size, WasmFile file)
		: base(type, pointer, size)
	{
		EntryCount = file.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < EntryCount; num++)
		{
			Types.Add(file.BaseStream.ReadLEB128Unsigned());
		}
	}
}
