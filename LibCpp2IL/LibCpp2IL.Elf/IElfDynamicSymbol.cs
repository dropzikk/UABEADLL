namespace LibCpp2IL.Elf;

public interface IElfDynamicSymbol
{
	uint NameOffset { get; }

	ulong Value { get; }

	ulong Size { get; }

	byte Info { get; }

	byte Other { get; }

	ushort Shndx { get; }

	ElfDynamicSymbolType Type { get; }
}
