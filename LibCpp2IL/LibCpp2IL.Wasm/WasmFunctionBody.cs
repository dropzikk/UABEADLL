using System.Collections.Generic;

namespace LibCpp2IL.Wasm;

public class WasmFunctionBody
{
	public ulong BodySize;

	public ulong LocalCount;

	public readonly List<WasmLocalEntry> Locals = new List<WasmLocalEntry>();

	public long InstructionsOffset;

	public byte[] Instructions;

	public WasmFunctionBody(WasmFile file)
	{
		BodySize = file.BaseStream.ReadLEB128Unsigned();
		long position = file.Position;
		LocalCount = file.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < LocalCount; num++)
		{
			Locals.Add(new WasmLocalEntry(file));
		}
		InstructionsOffset = file.Position;
		Instructions = file.ReadByteArrayAtRawAddress(InstructionsOffset, (int)(position + (long)BodySize - InstructionsOffset));
	}
}
