using System;

namespace LibCpp2IL.MachO;

[Flags]
public enum MachOVmProtection
{
	PROT_NONE = 0,
	PROT_READ = 1,
	PROT_WRITE = 2,
	PROT_EXEC = 4,
	PROT_NO_CHANGE = 8,
	PROT_COPY = 0x10,
	PROT_TRUSTED = 0x20,
	PROT_IS_MASK = 0x40,
	PROT_STRIP_READ = 0x80,
	PROT_COPY_FAIL_IF_EXECUTABLE = 0x100
}
