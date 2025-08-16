using System.Collections.Generic;

namespace LibCpp2IL.Wasm;

public class WasmElementSegment
{
	public enum ElementSegmentMode
	{
		Active,
		Passive,
		Declarative
	}

	public byte Flags;

	public ElementSegmentMode Mode;

	public ulong TableIdx;

	public ConstantExpression? Offset;

	public ulong Count;

	public byte ElemKind;

	public List<ulong>? FunctionIndices;

	public WasmTypeEnum ElemType;

	public List<ConstantExpression>? ConstantExpressions;

	public WasmElementSegment(WasmFile file)
	{
		Flags = file.ReadByte();
		if ((Flags & 3) == 2)
		{
			TableIdx = file.BaseStream.ReadLEB128Unsigned();
		}
		else
		{
			TableIdx = 0uL;
		}
		if ((Flags & 1) == 0)
		{
			Mode = ElementSegmentMode.Active;
			Offset = new ConstantExpression(file);
		}
		else if ((Flags & 2) == 0)
		{
			Mode = ElementSegmentMode.Passive;
		}
		else
		{
			Mode = ElementSegmentMode.Declarative;
		}
		if ((Flags & 3) == 0)
		{
			ElemKind = 0;
			ElemType = WasmTypeEnum.funcRef;
		}
		else
		{
			byte b = file.ReadByte();
			if ((Flags & 4) == 0)
			{
				ElemKind = b;
			}
			else
			{
				ElemType = (WasmTypeEnum)b;
			}
		}
		Count = file.BaseStream.ReadLEB128Unsigned();
		if ((Flags & 4) == 0)
		{
			FunctionIndices = new List<ulong>();
			for (ulong num = 0uL; num < Count; num++)
			{
				FunctionIndices.Add(file.BaseStream.ReadLEB128Unsigned());
			}
		}
		else
		{
			ConstantExpressions = new List<ConstantExpression>();
			for (ulong num2 = 0uL; num2 < Count; num2++)
			{
				ConstantExpressions.Add(new ConstantExpression(file));
			}
		}
	}
}
