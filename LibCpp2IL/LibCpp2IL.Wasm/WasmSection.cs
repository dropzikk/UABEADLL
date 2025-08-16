using LibCpp2IL.Logging;

namespace LibCpp2IL.Wasm;

public class WasmSection
{
	public WasmSectionId Type;

	public long Pointer;

	public ulong Size;

	protected WasmSection(WasmSectionId type, long pointer, ulong size)
	{
		Type = type;
		Pointer = pointer;
		Size = size;
	}

	public static WasmSection MakeSection(WasmFile file)
	{
		long position = file.Position;
		WasmSectionId wasmSectionId = (WasmSectionId)file.ReadByte();
		ulong num = file.BaseStream.ReadLEB128Unsigned();
		LibLogger.VerboseNewline($"\t\tFound section of type {wasmSectionId} at 0x{position:X} with length 0x{num:X}");
		num += (ulong)(file.Position - position);
		return wasmSectionId switch
		{
			WasmSectionId.SEC_TYPE => new WasmTypeSection(wasmSectionId, position, num, file), 
			WasmSectionId.SEC_IMPORT => new WasmImportSection(wasmSectionId, position, num, file), 
			WasmSectionId.SEC_DATA => new WasmDataSection(wasmSectionId, position, num, file), 
			WasmSectionId.SEC_CODE => new WasmCodeSection(wasmSectionId, position, num, file), 
			WasmSectionId.SEC_FUNCTION => new WasmFunctionSection(wasmSectionId, position, num, file), 
			WasmSectionId.SEC_TABLE => new WasmTableSection(wasmSectionId, position, num, file), 
			WasmSectionId.SEC_GLOBAL => new WasmGlobalSection(wasmSectionId, position, num, file), 
			WasmSectionId.SEC_ELEMENT => new WasmElementSection(wasmSectionId, position, num, file), 
			WasmSectionId.SEC_EXPORT => new WasmExportSection(wasmSectionId, position, num, file), 
			_ => new WasmSection(wasmSectionId, position, num), 
		};
	}
}
