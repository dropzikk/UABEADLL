using System.Collections.Generic;
using LibCpp2IL.Elf;

namespace LibCpp2IL;

internal static class MiniArm64Decompiler
{
	private static (uint reg, ulong page)? GetAdrp(uint inst, ulong pc)
	{
		if ((inst.Bits(24, 8) & 0x8F) != 128)
		{
			return null;
		}
		uint num = inst.Bits(29, 2);
		uint num2 = (inst.Bits(5, 19) << 14) + (num << 12);
		ulong num3 = pc & 0xFFFFFFFFFFFFF000uL;
		return (inst.Bits(0, 5), num3 + num2);
	}

	private static (uint reg, ulong addr)? GetAdr(uint inst, ulong pc)
	{
		if (inst.Bits(24, 5) != 16 || inst.Bits(31, 1) != 0)
		{
			return null;
		}
		ulong num = (inst.Bits(5, 19) << 2) + inst.Bits(29, 2);
		num = (((num & 0x100000) == 0L) ? num : (num | 0xFFFFFFFFFFE00000uL));
		return (inst.Bits(0, 5), pc + num);
	}

	private static (uint reg_n, uint reg_d, uint imm)? GetAdd64(uint inst)
	{
		if (inst.Bits(22, 10) != 580)
		{
			return null;
		}
		uint item = inst.Bits(10, 12);
		uint item2 = inst.Bits(5, 5);
		uint item3 = inst.Bits(0, 5);
		return (item2, item3, item);
	}

	private static (uint reg_t, uint reg_n, uint simm)? GetLdr64ImmOffset(uint inst)
	{
		if (inst.Bits(22, 10) != 997)
		{
			return null;
		}
		uint item = inst.Bits(10, 12);
		uint item2 = inst.Bits(0, 5);
		uint item3 = inst.Bits(5, 5);
		return (item2, item3, item);
	}

	public static bool IsB(uint inst)
	{
		return inst.Bits(26, 6) == 5;
	}

	public static Dictionary<uint, ulong> GetAddressesLoadedIntoRegisters(List<uint> funcBody, ulong baseAddress, ElfFile image)
	{
		Dictionary<uint, ulong> dictionary = new Dictionary<uint, ulong>();
		ulong num = baseAddress;
		foreach (uint item5 in funcBody)
		{
			(uint, ulong)? adrp = GetAdrp(item5, num);
			if (adrp.HasValue)
			{
				(uint, ulong) valueOrDefault = adrp.GetValueOrDefault();
				uint item = valueOrDefault.Item1;
				ulong item2 = valueOrDefault.Item2;
				dictionary[item] = item2;
			}
			(uint, ulong)? adr = GetAdr(item5, num);
			if (adr.HasValue)
			{
				(uint, ulong) valueOrDefault2 = adr.GetValueOrDefault();
				uint item3 = valueOrDefault2.Item1;
				ulong item4 = valueOrDefault2.Item2;
				dictionary[item3] = item4;
			}
			(uint, uint, uint)? add = GetAdd64(item5);
			if (add.HasValue)
			{
				var (num2, num3, num4) = add.GetValueOrDefault();
				if (num2 == num3 && dictionary.ContainsKey(num3))
				{
					dictionary[num3] += num4;
				}
			}
			(uint, uint, uint)? ldr64ImmOffset = GetLdr64ImmOffset(item5);
			if (ldr64ImmOffset.HasValue)
			{
				var (num5, num6, num7) = ldr64ImmOffset.GetValueOrDefault();
				if (num5 == num6 && dictionary.ContainsKey(num6))
				{
					dictionary[num6] += num7 * 8;
					dictionary[num6] = image.ReadClassAtVirtualAddress<ulong>(dictionary[num6]);
				}
			}
			num += 4;
		}
		return dictionary;
	}

	public static List<uint> ReadFunctionAtRawAddress(ElfFile file, uint loc, uint maxLength)
	{
		List<uint> list = new List<uint>();
		uint num;
		do
		{
			num = file.ReadClassAtVirtualAddress<uint>(loc);
			list.Add(num);
			loc += 4;
		}
		while (!IsB(num) && list.Count < maxLength);
		return list;
	}
}
