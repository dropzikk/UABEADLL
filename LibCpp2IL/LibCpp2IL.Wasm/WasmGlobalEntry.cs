namespace LibCpp2IL.Wasm;

public class WasmGlobalEntry
{
	public WasmGlobalType Type;

	public ConstantExpression Expression;

	public WasmGlobalEntry(WasmFile file)
	{
		Type = new WasmGlobalType(file);
		Expression = new ConstantExpression(file);
	}
}
