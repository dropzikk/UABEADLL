using System.Collections.Generic;

namespace LibCpp2IL.Wasm;

public class WasmElementSection : WasmSection
{
	public ulong ElementCount;

	public readonly List<WasmElementSegment> Elements = new List<WasmElementSegment>();

	internal WasmElementSection(WasmSectionId type, long pointer, ulong size, WasmFile file)
		: base(type, pointer, size)
	{
		ElementCount = file.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < ElementCount; num++)
		{
			Elements.Add(new WasmElementSegment(file));
		}
	}
}
