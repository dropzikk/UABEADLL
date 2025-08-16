namespace LibCpp2IL.Elf;

public class ElfRelocation
{
	public ElfRelocationType Type;

	public ulong Offset;

	public ulong? Addend;

	public ulong pRelatedSymbolTable;

	public ulong IndexInSymbolTable;

	private static ulong GetTypeBitsFromInfo(ulong info, ElfFile f)
	{
		if (!f.is32Bit)
		{
			return info & 0xFFFFFFFFu;
		}
		return info & 0xFF;
	}

	private static ulong GetSymBitsFromInfo(ulong info, ElfFile f)
	{
		if (!f.is32Bit)
		{
			return info >> 32;
		}
		return info >> 8;
	}

	public ElfRelocation(ElfFile f, ElfRelEntry relocation, ulong tablePointer)
	{
		Offset = relocation.Offset;
		Addend = null;
		Type = (ElfRelocationType)GetTypeBitsFromInfo(relocation.Info, f);
		IndexInSymbolTable = GetSymBitsFromInfo(relocation.Info, f);
		pRelatedSymbolTable = tablePointer;
	}

	public ElfRelocation(ElfFile f, ElfRelaEntry relocation, ulong tablePointer)
	{
		Offset = relocation.Offset;
		Addend = relocation.Addend;
		Type = (ElfRelocationType)GetTypeBitsFromInfo(relocation.Info, f);
		IndexInSymbolTable = GetSymBitsFromInfo(relocation.Info, f);
		pRelatedSymbolTable = tablePointer;
	}

	protected bool Equals(ElfRelocation other)
	{
		return Offset == other.Offset;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((ElfRelocation)obj);
	}

	public override int GetHashCode()
	{
		return Offset.GetHashCode();
	}

	public static bool operator ==(ElfRelocation? left, ElfRelocation? right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(ElfRelocation? left, ElfRelocation? right)
	{
		return !object.Equals(left, right);
	}
}
