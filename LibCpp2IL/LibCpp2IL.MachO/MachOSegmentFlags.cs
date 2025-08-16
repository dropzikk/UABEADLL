using System;

namespace LibCpp2IL.MachO;

[Flags]
public enum MachOSegmentFlags
{
	SG_NONE = 0,
	SG_HIGHVM = 1,
	SG_FVMLIB = 2,
	SG_NORELOC = 4,
	SG_PROTECTED_VERSION_1 = 8
}
