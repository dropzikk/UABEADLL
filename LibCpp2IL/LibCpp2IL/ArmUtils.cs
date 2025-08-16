namespace LibCpp2IL;

public static class ArmUtils
{
	public const uint PC_REG = 15u;

	public static (uint register, ushort immediateValue) GetOperandsForLiteralLdr(uint inst)
	{
		if (inst.Bits(16, 16) != 58783)
		{
			return (register: 0u, immediateValue: 0);
		}
		return (register: inst.Bits(12, 4), immediateValue: (ushort)inst.Bits(0, 12));
	}

	public static (uint firstReg, uint secondReg, uint thirdReg) GetOperandsForRegisterLdr(uint inst)
	{
		if (inst.Bits(20, 12) != 3705)
		{
			return (firstReg: 0u, secondReg: 0u, thirdReg: 0u);
		}
		uint item = inst.Bits(16, 4);
		uint item2 = inst.Bits(12, 4);
		uint item3 = inst.Bits(0, 4);
		return (firstReg: item2, secondReg: item, thirdReg: item3);
	}

	public static (uint firstReg, uint secondReg, uint thirdReg) GetOperandsForRegisterAdd(uint inst)
	{
		if (inst.Bits(21, 11) != 1796)
		{
			return (firstReg: 0u, secondReg: 0u, thirdReg: 0u);
		}
		uint item = inst.Bits(12, 4);
		uint item2 = inst.Bits(16, 4);
		uint item3 = inst.Bits(0, 4);
		return (firstReg: item, secondReg: item2, thirdReg: item3);
	}
}
