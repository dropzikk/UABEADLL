using System;

namespace LibCpp2IL.Wasm;

public class ConstantExpression
{
	public enum ConstantInstruction : byte
	{
		I32_CONST = 65,
		I64_CONST = 66,
		F32_CONST = 67,
		F64_CONST = 68,
		REF_NULL_FUNCREF = 208,
		REF_NULL_EXTERNREF = 209,
		REF_FUNC = 210,
		GLOBAL_GET = 35
	}

	public ConstantInstruction Type;

	public IConvertible? Value;

	public ConstantExpression(WasmFile file)
	{
		Type = (ConstantInstruction)file.ReadByte();
		switch (Type)
		{
		case ConstantInstruction.GLOBAL_GET:
		case ConstantInstruction.I32_CONST:
		case ConstantInstruction.I64_CONST:
		case ConstantInstruction.REF_FUNC:
			Value = file.BaseStream.ReadLEB128Unsigned();
			break;
		case ConstantInstruction.F32_CONST:
			Value = file.ReadSingle();
			break;
		case ConstantInstruction.F64_CONST:
			Value = file.ReadDouble();
			break;
		case ConstantInstruction.REF_NULL_FUNCREF:
		{
			byte b = file.ReadByte();
			switch (b)
			{
			case 111:
				Type = ConstantInstruction.REF_NULL_EXTERNREF;
				break;
			case 112:
				Type = ConstantInstruction.REF_NULL_FUNCREF;
				break;
			default:
				throw new Exception($"Invalid subtype {b}");
			}
			Value = null;
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		byte b2 = file.ReadByte();
		if (b2 != 11)
		{
			throw new Exception($"Invalid end byte, got 0x{b2:X2}, expecting 0x0B");
		}
	}
}
