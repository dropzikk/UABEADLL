using System.Collections.Generic;
using LibCpp2IL.Logging;

namespace LibCpp2IL.Wasm;

public class WasmCodeSection : WasmSection
{
	private readonly WasmFile _file;

	public ulong FunctionCount;

	public readonly List<WasmFunctionBody> Functions = new List<WasmFunctionBody>();

	public byte[] RawSectionContent => _file.GetRawBinaryContent().SubArray((int)Pointer, (int)Size);

	internal WasmCodeSection(WasmSectionId type, long pointer, ulong size, WasmFile file)
		: base(type, pointer, size)
	{
		_file = file;
		FunctionCount = file.BaseStream.ReadLEB128Unsigned();
		for (ulong num = 0uL; num < FunctionCount; num++)
		{
			Functions.Add(new WasmFunctionBody(file));
		}
		LibLogger.VerboseNewline($"\t\tRead {Functions.Count} function bodies");
	}
}
