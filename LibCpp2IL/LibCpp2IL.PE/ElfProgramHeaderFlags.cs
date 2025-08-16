using System;

namespace LibCpp2IL.PE;

[Flags]
public enum ElfProgramHeaderFlags : uint
{
	PF_X = 1u,
	PF_W = 2u,
	PF_R = 4u
}
