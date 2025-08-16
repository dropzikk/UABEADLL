using System.Collections.Generic;

namespace LibCpp2IL.Wasm;

public class WasmTableSection : WasmSection
{
	public ulong TableCount;

	public readonly List<WasmTableType> Tables = new List<WasmTableType>();

	internal WasmTableSection(WasmSectionId type, long pointer, ulong size, WasmFile file)
		: base(type, pointer, size)
	{
		TableCount = file.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < TableCount; num++)
		{
			Tables.Add(new WasmTableType(file));
		}
	}
}
