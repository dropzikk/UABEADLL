namespace LibCpp2IL.Wasm;

public class WasmDataSegment
{
	public ulong Index;

	public ConstantExpression? OffsetExpr;

	public long FileOffset;

	public ulong Size;

	public byte[] Data;

	public ulong VirtualOffset
	{
		get
		{
			ConstantExpression? offsetExpr = OffsetExpr;
			if (offsetExpr != null && offsetExpr.Type == ConstantExpression.ConstantInstruction.I32_CONST)
			{
				return (ulong)(object)OffsetExpr.Value;
			}
			return ulong.MaxValue;
		}
	}

	public WasmDataSegment(WasmFile readFrom)
	{
		byte b = readFrom.ReadByte();
		if (b == 2)
		{
			Index = readFrom.BaseStream.ReadLEB128Unsigned();
		}
		else
		{
			Index = 0uL;
		}
		if (b == 0 || b == 2)
		{
			OffsetExpr = new ConstantExpression(readFrom);
		}
		else
		{
			OffsetExpr = null;
		}
		Size = readFrom.BaseStream.ReadLEB128Unsigned();
		FileOffset = readFrom.Position;
		Data = readFrom.ReadByteArrayAtRawAddress(FileOffset, (int)Size);
	}
}
