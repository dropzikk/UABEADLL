namespace LibCpp2IL.Wasm;

public enum WasmTypeEnum : byte
{
	i32 = 127,
	i64 = 126,
	f32 = 125,
	f64 = 124,
	funcRef = 112,
	externRef = 111
}
